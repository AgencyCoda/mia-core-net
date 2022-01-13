using System.Collections.Generic;
using MediatR;
using MiaCore.Models;

namespace MiaCore.Features.SearchNewsByLocation
{
    public class SearchNewsByLocationRequest : IRequest<List<News>>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<int> Categories { get; set; }
    }
}