using System.Collections.Generic;

namespace MiaCore.Infrastructure.Persistence
{
    public class Where
    {
        public Where()
        {

        }
        public Where(string key, object value)
        {
            Key = key;
            Value = value.ToString();
        }
        public Where(string key, object value, WhereConditionType type)
        {
            Key = key;
            Value = value.ToString();
            Type = type;
        }

        public WhereConditionType Type { get; set; } = WhereConditionType.Equal;
        public string Key { get; set; }
        public List<string> Keys { get; set; }
        public string Value { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }

    public enum WhereConditionType
    {
        Equal,
        Likes,
        In,
        Between,
        IsNull,
        IsNotNull,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
    }
}