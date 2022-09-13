using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MiaCore.Exceptions;
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
        private int _joinCounter;
        private Type _entityType;


        public DynamicQueryBuilder(Type entityType)
        {
            _entityType = entityType;
            _table = convertName(entityType.Name);
            _fields = $"{_table}.*";
            _where = "";
            _limit = "";
            _order = "";
            _isCount = false;

            if (typeof(IDeletableEntity).IsAssignableFrom(entityType))
                _where += $"where {_table}.deleted = 0 ";

        }

        public DynamicQueryBuilder WithOne(string tableName, string joinField = null)
        {
            tableName = convertName(tableName);

            if (string.IsNullOrEmpty(joinField))
                joinField = getColumnName(tableName);
            else
                joinField = convertName(joinField);

            _joinCounter += 1;
            var alias = tableName + _joinCounter;
            _fields += $",{alias}.*";
            _join += $" left join {tableName} as {alias} on {alias}.id = {_table}.{joinField}";
            return this;
        }

        public DynamicQueryBuilder WithMany(string tableName, string? intermediateTable = null, string joinField = null)
        {
            if (string.IsNullOrEmpty(joinField))
                joinField = getColumnName(_table);
            else
                joinField = convertName(joinField);

            tableName = convertName(tableName);
            _joinCounter += 1;
            var alias = tableName + _joinCounter;

            _fields += $",{alias}.*";
            if (intermediateTable is null)
                _join += $" left join {tableName} as {alias} on {alias}.{joinField} = {_table}.id";
            else
            {
                var intermediateTableName = convertName(intermediateTable);
                var intermAlias = intermediateTableName + _joinCounter;
                _join += @$" left join {intermediateTableName} as {intermAlias} on {intermAlias}.{getColumnName(_table)} = {_table}.id 
                         left join {tableName} as {alias} on {alias}.id = {intermAlias}.{getColumnName(tableName)}";
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
                    if (!string.IsNullOrEmpty(where.Key))
                    {
                        var split = where.Key.Split(".");
                        if (split.Count() == 2)
                        {
                            var prop = _entityType.GetProperty(removeUndescores(split[0]), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (prop is null)
                                throw new Exception($"Error. Field:{split[1]} not found");
                            var isList = false;
                            if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                                isList = true;

                            _joinCounter += 1;
                            table = convertName(isList ? prop.PropertyType.GenericTypeArguments[0].Name : prop.PropertyType.Name);
                            var alias = table + _joinCounter;

                            where.Key = split[1];

                            if (isList)
                                _localJoin += $" left join {table} as {alias} on {alias}.{getColumnName(_table)} = {_table}.id";
                            else
                                _localJoin += $" left join {table} as {alias} on {alias}.id = {_table}.{getColumnName(table)}";
                            table = alias;
                        }
                    }

                    if (where.Type == WhereConditionType.Likes)
                    {
                        if (where.Keys != null && where.Keys.Count() > 0)
                        {
                            var childTable = _table;
                            var firstKey = where.Keys[0];
                            if (!string.IsNullOrEmpty(firstKey))
                            {
                                var split = firstKey.Split(".");
                                if (split.Count() == 2)
                                {
                                    var prop = _entityType.GetProperty(removeUndescores(split[0]), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                                    if (prop is null)
                                        throw new Exception($"Error. Field:{split[1]} not found");
                                    var isList = false;
                                    if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                                        isList = true;

                                    _joinCounter += 1;
                                    childTable = convertName(isList ? prop.PropertyType.GenericTypeArguments[0].Name : prop.PropertyType.Name);
                                    var alias = childTable + _joinCounter;

                                    if (isList)
                                        _localJoin += $" left join {childTable} as {alias} on {alias}.{getColumnName(_table)} = {_table}.id";
                                    else
                                        _localJoin += $" left join {childTable} as {alias} on {alias}.id = {_table}.{getColumnName(childTable)}";
                                    childTable = alias;

                                    List<string> newKeys = new List<string>();
                                    foreach (var key in where.Keys)
                                    {
                                        var columnSplit = key.Split(".");
                                        newKeys.Add(columnSplit[1]);
                                    }
                                    _where += " CONCAT_WS(' '," + string.Join(",", newKeys.Select(x => $"{childTable}.{convertName(x)}")) + $") regexp {where.Value}";
                                }
                                else
                                {
                                    _where += " CONCAT_WS(' '," + string.Join(",", where.Keys.Select(x => $"{childTable}.{convertName(x)}")) + $") regexp {where.Value}";
                                }
                            }
                        }
                    }

                    _where += where.Type switch
                    {
                        WhereConditionType.In => $" {table}.{convertName(where.Key)} in ({where.Value})",
                        WhereConditionType.Between => $" {table}.{convertName(where.Key)} between '{where.From}' and '{where.To}'",
                        WhereConditionType.IsNull => $" {table}.{convertName(where.Key)} is null",
                        WhereConditionType.IsNotNull => $" {table}.{convertName(where.Key)} is not null",
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

        private string removeUndescores(string name)
        => name.Replace("_", "");
    }
}