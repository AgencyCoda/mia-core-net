using System.Collections.Generic;
using System.Linq;

namespace MiaCore.Infrastructure.Persistence
{
    public class DynamicQueryBuilder
    {
        private string _table;
        private string _select;
        private string _from;
        private string _where;
        private string _limit;
        private string _order;

        public DynamicQueryBuilder(string tableName)
        {
            _table = getTableName(tableName);
            _select = $"SELECT {_table}.*";
            _from = $"from {_table}";
            _where = "";
            _limit = "";
            _order = "";
        }

        public DynamicQueryBuilder WithOne(string tableName)
        {
            tableName = getTableName(tableName);
            _select += $",{tableName}.*";
            _from += $" left join {tableName} on {tableName}.id = {_table}.{getColumnName(tableName)}";
            return this;
        }

        public DynamicQueryBuilder WithMany(string tableName)
        {
            tableName = getTableName(tableName);
            _select += $",{tableName}.*";
            _from += $" left join {tableName} on {tableName}.{getColumnName(_table)} = {_table}.id";
            return this;
        }

        public DynamicQueryBuilder Where(List<Where> wheres)
        {
            if (wheres != null)
                foreach (var where in wheres)
                {
                    _where += !_where.StartsWith("where") ? "where" : " and";
                    if (!long.TryParse(where.Value, out _))
                        where.Value = $"\"{where.Value}\"";
                    _where += $" {_table}.{where.Key} = {where.Value}";
                }
            return this;
        }

        public DynamicQueryBuilder OrderBy(List<Order> orders)
        {
            if (orders != null)
                foreach (var order in orders)
                {
                    _order += !_order.StartsWith("order by") ? "order by" : " ,";
                    _order += $" {order.Field} {order.Type}";
                }
            return this;
        }

        public DynamicQueryBuilder WithLimit(int? limit, int? page)
        {
            if (limit.HasValue)
                _limit = $"limit {((page ?? 0) <= 0 ? 0 : page - 1) * limit},{limit}";
            return this;
        }
        public string Build()
        {
            return $"{_select} {_from} {_where} {_order} {_limit}";
        }
        private string getTableName(string name)
            => string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();

        private string getColumnName(string name)
        {
            name = getTableName(name);
            name = name.StartsWith("mia_") ? name.Substring(4) : name;
            return name + "_id";
        }
    }
}