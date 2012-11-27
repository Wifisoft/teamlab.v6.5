using System;
using System.Data;

namespace ASC.Common.Data
{
    class DbNestedTransaction : IDbTransaction
    {
        private readonly IDbTransaction transaction;


        public DbNestedTransaction(IDbTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            this.transaction = transaction;
        }

        public IDbConnection Connection
        {
            get { return transaction.Connection; }
        }

        public IsolationLevel IsolationLevel
        {
            get { return transaction.IsolationLevel; }
        }

        public void Commit()
        {
        }

        public void Rollback()
        {
        }

        public void Dispose()
        {
        }
    }
}