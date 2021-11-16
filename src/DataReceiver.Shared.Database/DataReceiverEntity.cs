using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace DataReceiver.Shared.Database
{
    public class DataReceiverEntity : IDisposable
    {
        private DataReceiverContext _context;

        public DataReceiverEntity()
        {
            _context = GetDataReceiverContext();
        }

        private static DataReceiverContext GetDataReceiverContext()
        {
            return new DataReceiverContext(DataReceiverEntityDbContextOptions.Instance.optionBuilder.Options);
        }

        public DbConnection Connection
        {
            get
            {
                return _context.Database.GetDbConnection();
            }
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public IEnumerable<T> SqlQuery<T>(string str, IDbContextTransaction transaction)
        {
            return Connection.Query<T>(str, transaction);
        }

        public void ExecuteSqlCommand(string str, IDbContextTransaction transaction)
        {
            Connection.Execute(str, transaction);
        }

        bool disposed = false;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposed)
            {
                return;
            }
            if(disposing)
            {
                _context.Dispose();
            }
            disposed = true;
        }
    }
}
