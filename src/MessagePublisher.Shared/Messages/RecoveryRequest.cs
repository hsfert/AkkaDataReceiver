using MessagePublisher.Shared.Models;
using System.Collections.Generic;

namespace MessagePublisher.Shared.Messages
{
    public class RecoveryRequest
    {
        public IReadOnlyList<RecoveryInfo> Informations { get; private set; }

        public RecoveryRequest(IReadOnlyList<RecoveryInfo> informations)
        {
            Informations = informations;
        }
    }
}
