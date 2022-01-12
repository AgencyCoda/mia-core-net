using MediatR;
using MiaCore.Models;

namespace MiaCore.Features.RemoveEntityById
{
    public class RemoveEntityByIdRequest<T> : IRequest<object> where T : IEntity
    {
        public long Id { get; set; }
    }
}