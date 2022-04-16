using System;
using System.Collections.Generic;
using System.Linq;
using MiaCore.Models;

namespace MiaCore.Infrastructure.Persistence
{
    public class DynamicQueryBuilder
    {
        private string _table;
        private string _fields;
        private string _join;
        private string _localJoin;
        private string _where;
        private string _limit;
        private string _order;
        private bool _isCount;


        public DynamicQueryBuilder(Type entityType)
        {
            _table = convertName(entityType.Name);
            _fields = $"{_table}.*";
            _where = "";
            _limit = "";
            _order = "";
            _isCount = false;

            if (typeof(IDeletableEntity).IsAssignableFrom(entityType))
                _where += $"where {_table}.deleted = 0 ";

        }

        public DynamicQueryBuilder WithOne(string tableName)
        {
            tableName = convertName(tableName);
            _fields += $",{tableName}.*";
            _join += $" left join {tableName} on {tableName}.id = {_table}.{getColumnName(tableName)}";
            return this;
        }

        public DynamicQueryBuilder WithMany(string tableName, string? intermediateTable = null)
        {
            tableName = convertName(tableName);
            _fields += $",{tableName}.*";
            if (intermediateTable is null)
                _join += $" left join {tableName} on {tableName}.{getColumnName(_table)} = {_table}.id";
            else
            {
                var intermediateTableName = convertName(intermediateTable);
                _join += @$" left join {intermediateTableName} on {intermediateTableName}.{getColumnName(_table)} = {_table}.id 
                         left join {tableName} on {tableName}.id = {intermediateTableName}.{getColumnName(tableName)}";
            }
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

                    var table = _table;
                    var split = where.Key.Split(".");
                    if (split.Count() == 2)
                    {
                        table = split[0];
                        where.Key = split[1];
                        _localJoin += $" left join {table} on {table}.{getColumnName(_table)} = {_table}.id";
                    }

                    _where += where.Type switch
                    {
                        WhereConditionType.Likes => " CONCAT_WS(' '," + string.Join(",", where.Keys.Select(x => $"{table}.{convertName(x)}")) + $") regexp {where.Value}",
                        WhereConditionType.In => $" {table}.{convertName(where.Key)} in ({where.Value})",
                        _ => $" {table}.{convertName(where.Key)} = {where.Value}"
                    };
                }
            return this;
        }

        public DynamicQueryBuilder OrderBy(List<Order> orders)
        {
            if (orders != null)
                foreach (var order in orders)
                {
                    _order += !_order.StartsWith("order by") ? "order by" : " ,";
                    _order += $" {_table}.{order.Field} {order.Type}";
                }
            return this;
        }

        public DynamicQueryBuilder WithLimit(int? limit, int? page)
        {
            if (limit.HasValue)
                _limit = $"limit {((page ?? 0) <= 0 ? 0 : page - 1) * limit},{limit}";
            return this;
        }

        public DynamicQueryBuilder WithCount()
        {
            _isCount = true;
            return this;
        }

        public string Build()
        {
            var fields = _isCount ? $"count(distinct {_table}.id) Count" : _fields;
            var limit = _isCount ? "" : _limit;

            return string.IsNullOrEmpty(_limit) ?
                $"SELECT {fields} from {_table} {_localJoin} {_join} {_where} {_order} {limit}" :
                $"SELECT {fields} from (select {_table}.* from {_table} {_localJoin} {_where} {_order} {limit}) as {_table} {_join}"
                ;
        }

        private string convertName(string name)
            => string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();

        private string getColumnName(string tableName)
        {
            tableName = convertName(tableName);
            tableName = tableName.StartsWith("mia_") ? tableName.Substring(4) : tableName;
            return tableName + "_id";
        }
    }
}