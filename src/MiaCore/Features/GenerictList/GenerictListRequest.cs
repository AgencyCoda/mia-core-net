using System;
using System.Collections.Generic;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;

namespace MiaCore.Features.GenerictList
{
    public class GenerictListRequest<T> : IRequest<object> where T : IEntity
    {
        public string[] With { get; set; }
        public int? Limit { get; set; } = 10;
        public int? Page { get; set; } = 1;
        public string Where { get; set; }
        public List<Where> Wheres { get; set; }
        public List<Order> Orders { get; set; }
        private Type ReturnType;
        public GenerictListRequest<T> SetReturnType<Dto>()
        {
            var tp = typeof(GenericListResponse<>);
            ReturnType = tp.MakeGenericType(typeof(Dto));
            return this;
        }
        public Type GetReturnType()
            => ReturnType;

    }
}