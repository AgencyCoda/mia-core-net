using System;

namespace MiaCore.Models
{
    public class RelationAttribute : Attribute
    {
        public Type IntermediateEntity { get; set; }
        public string JoinField { get; set; }

        public RelationAttribute()
        {

        }

        public RelationAttribute(Type intermediateEntity)
        {
            IntermediateEntity = intermediateEntity;
        }

        public RelationAttribute(string joinField)
        {
            JoinField = joinField;
        }
    }
}