using MediatR;
using MiaCore.Models;

namespace MiaCore.Features.SaveNews
{
    internal class SaveNewsRequest : IRequest<News>
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; }
        public NewsContent Content { get; set; }
        public long? ParentId { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}