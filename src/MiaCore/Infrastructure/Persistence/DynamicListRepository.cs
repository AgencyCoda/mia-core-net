using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MiaCore.Models;
using MySql.Data.MySqlClient;

namespace MiaCore.Infrastructure.Persistence
{
    public class DynamicListRepository
    {
        protected readonly DbConnection Connection;

        public async Task<dynamic> GetListAsync(string entity, string[] relatedEntities)
        {
            using var conn = new MySqlConnection("Server=35.198.58.231;Database=ewire;Uid=root;Pwd=nwuPJwwus4IBMIaq");

            var mainEntity = convertWithUnderscores(entity);
            var query = $"select * from {mainEntity}";
            foreach (var relatedEntity in relatedEntities)
            {
                var related = convertWithUnderscores(relatedEntity);
                query += $" left join {related} on {related}.Id = {mainEntity}.{related}_id";
            }
            return await conn.QueryAsync(query, new[]{
                typeof(MiaUser),
                typeof(MiaUser)
            }, obj =>
            {
                return new MiaUser();
            });
        }

        private string convertWithUnderscores(string name)
        => string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));

    }
}