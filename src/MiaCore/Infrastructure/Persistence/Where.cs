namespace MiaCore.Infrastructure.Persistence
{
    public class Where
    {
        public WhereConditionType Type { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public enum WhereConditionType
    {
        Equal
    }
}