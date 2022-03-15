using System.Data.Common;
using System.Threading.Tasks;
using MiaCore.Models;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace MiaCore.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbConnection _connection;
        private DbTransaction _transaction;
        private readonly IOptions<MiaCoreOptions> _options;
        public UnitOfWork(IOptions<MiaCoreOptions> options)
        {
            _options = options;
            _connection = new MySqlConnection(options.Value.ConnectionString);
        }
        public async Task BeginTransactionAsync()
        {
            _connection.Open();
            if (_transaction is not null)
                throw new System.Exception("Transacrion is already opened");

            _transaction = await _connection.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction is null)
                throw new System.NullReferenceException(nameof(_transaction));
            await _transaction.CommitAsync();
        }

        public IGenericRepository<T> GetGenericRepository<T>() where T : IEntity
        {
            return new GenericRepository<T>(_connection, _options);
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction is null)
                throw new System.NullReferenceException(nameof(_transaction));
            await _transaction.RollbackAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}