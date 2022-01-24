using MediatR;
using MiaCore.Models;

namespace MiaCore.Features.FetchEntityById
{
    public class FetchEntityByIdRequest<T> : IRequest<T> where T : IEntity
    {
        public long Id { get; set; }
        public string Withs { get; set; }
    }
}