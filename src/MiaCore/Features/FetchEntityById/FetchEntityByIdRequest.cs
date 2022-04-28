using System;
using MediatR;
using MiaCore.Models;

namespace MiaCore.Features.FetchEntityById
{
    public class FetchEntityByIdRequest<T> : IRequest<object> where T : IEntity
    {
        public long Id { get; set; }
        public string Withs { get; set; }
        private Type ReturnType;
        public FetchEntityByIdRequest<T> SetReturnType<Dto>()
        {
            ReturnType = typeof(Dto);
            return this;
        }
        public Type GetReturnType()
            => ReturnType;
    }
}