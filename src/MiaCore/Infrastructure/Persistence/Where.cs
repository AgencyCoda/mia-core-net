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

        public WhereConditionType Type { get; set; } = WhereConditionType.Equal;
        public string Key { get; set; }
        public List<string> Keys { get; set; }
        public string Value { get; set; }
    }

    public enum WhereConditionType
    {
        Equal,
        Like
    }
}