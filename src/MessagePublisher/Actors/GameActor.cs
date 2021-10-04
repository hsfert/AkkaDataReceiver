using Akka.Actor;
using MessagePublisher.Config;
using MessagePublisher.Messages;
using MessagePublisher.Models;
using MessagePublisher.Shared.Messages;
using MessagePublisher.Shared.Models;
using System;
using System.Collections.Generic;

namespace MessagePublisher.Actors
{
    /// <summary>
    /// Actor to simulate the change in a game,
    /// the change will be published to a MessageQueue through router.
    /// </summary>
    public class GameActor : ReceiveActor
    {
        private ICancelable _recurringNewInvestment;
        private ICancelable _recurringNewOddsChange;
        private ICancelable _recurringPublishNewInvestment;
        private GameActorConfig _config;
        private int _gameId;
        private Game _game;
        private IActorRef _investmentPublisher;
        private IActorRef _oddsChangePublisher;

        public GameActor(int gameID, IActorRef investmentPublisher, IActorRef oddsChangePublisher, GameActorConfig config)
        {
            _investmentPublisher = investmentPublisher;
            _oddsChangePublisher = oddsChangePublisher;
            _gameId = gameID;
            _config = config;
            _game = new Game(gameID);
            _game.InitializeGame();
            Become(UpdateInformation);
        }

        private void UpdateInformation()
        {
            Receive<AddInvestment>(_ =>
            {
                _game.AddInvestment();
            });

            Receive<OddsChange>(_ =>
            {
                var oddsBefore = GetOddsChangeFromGame(null);
                _game.ChangeOdds();
                var oddsChange = GetOddsChangeFromGame(oddsBefore);
                OddsChangeMessage message = new OddsChangeMessage(0,
                    null,
                    _gameId,
                    oddsChange);
                _oddsChangePublisher.Tell(message);
            });

            Receive<PublishInvestmentSnapshot>(_ =>
            {
                var poolInvestments = GetInvestmentFromGame();
                InvestmentSnapshotMessage message = new InvestmentSnapshotMessage(0,
                    null,
                    _gameId,
                    poolInvestments
                    );
                _investmentPublisher.Tell(message);
            });
        }

        private List<PoolOddsChange> GetOddsChangeFromGame(List<PoolOddsChange> oddsBefore)
        {
            List<PoolOddsChange> oddsChange = new List<PoolOddsChange>();
            for (int i = 0; i < _game.Pools.Length; i++)
            {
                var pool = _game.Pools[i];
                var poolOddsBefore = oddsBefore?[i];
                var poolOddsChange = GetOddsChangeFromPool(pool, poolOddsBefore);
                oddsChange.Add(poolOddsChange);
            }
            return oddsChange;
        }

        private PoolOddsChange GetOddsChangeFromPool(Pool pool, PoolOddsChange oddsBefore)
        {
            List<CombinationOddsChange> oddsChange = new List<CombinationOddsChange>();
            for (int i = 0; i < pool.Combinations.Length; i++)
            {
                var combination = pool.Combinations[i];
                var combinationOddsBefore = oddsBefore?.Combinations[i];
                var combinationOddsChange = GetOddsChangeFromCombination(pool.PoolId, combination, combinationOddsBefore);
                oddsChange.Add(combinationOddsChange);
            }
            return new PoolOddsChange(_gameId, pool.PoolId, oddsChange);
        }

        private CombinationOddsChange GetOddsChangeFromCombination(int poolId,
            Combination combination,
            CombinationOddsChange oddsBefore)
        {
            if (oddsBefore is null)
            {
                GPCKey key = new GPCKey(_gameId, poolId, combination.CombinationId);
                return new CombinationOddsChange(key, combination.Odds, 0);
            }
            else
            {
                return new CombinationOddsChange(oddsBefore.Key, oddsBefore.OddsBefore, combination.Odds);
            }
        }
        private List<PoolInvestment> GetInvestmentFromGame()
        {
            List<PoolInvestment> investments = new List<PoolInvestment>();
            for (int i = 0; i < _game.Pools.Length; i++)
            {
                var pool = _game.Pools[i];
                investments.Add(GetInvestmentFromPool(pool));
            }
            return investments;
        }

        private PoolInvestment GetInvestmentFromPool(Pool pool)
        {
            List<CombinationInvestment> combinations = new List<CombinationInvestment>();
            for (int i = 0; i < pool.Combinations.Length; i++)
            {
                combinations.Add(GetInvestmentFromCombination(pool.PoolId, pool.Combinations[i]));
            }
            PoolInvestment poolInvestment = new PoolInvestment(_gameId, pool.PoolId, combinations);
            return poolInvestment;
        }

        private CombinationInvestment GetInvestmentFromCombination(int poolId, Combination combination)
        {
            GPCKey key = new GPCKey(_gameId, poolId, combination.CombinationId);
            CombinationInvestment investment = new CombinationInvestment(key, combination.Investment, combination.Liabilities);
            return investment;
        }

        protected override void PreStart()
        {
            _recurringNewInvestment = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1 / _config.FrequencyOfInvestmentPerSecond), Self, AddInvestment.Instance, Self);
            _recurringPublishNewInvestment = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1),
                TimeSpan.FromMinutes(1 / _config.FrequencyOfPublishingInvestmentSnapshotPerMinute), Self, PublishInvestmentSnapshot.Instance, Self);
            _recurringNewOddsChange =
                Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1),
                    TimeSpan.FromMinutes(1 / _config.FrequencyOfOddsChangePerMinute), Self, OddsChange.Instance, Self);
            base.PreStart();
        }

        protected override void PostStop()
        {
            _recurringNewInvestment?.Cancel();
            _recurringNewOddsChange?.Cancel();
            _recurringPublishNewInvestment?.Cancel();
            base.PostStop();
        }

        public static Props Props(int gameId, IActorRef investmentRouter, IActorRef oddsChangeRouter, GameActorConfig config) {
            return Akka.Actor.Props.Create(() => new GameActor(gameId, investmentRouter, oddsChangeRouter, config));
        }
    }
}
