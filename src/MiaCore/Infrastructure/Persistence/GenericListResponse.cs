using System;
using System.Collections.Generic;
using MiaCore.Models;

namespace MiaCore.Infrastructure.Persistence
{
    public class GenericListResponse<T>
    {
        public long CurrentPage { get; set; }
        public long LastPage => PerPage == 0 ? 0 : (long)Math.Ceiling((double)Total / PerPage);
        public long PerPage { get; set; }
        public IEnumerable<T> Data { get; set; }
        public long From =>
                    CurrentPage > LastPage ?
                     0 :
                    (CurrentPage > 1 ? (CurrentPage - 1) * PerPage + 1 : 1);
        public long To => CurrentPage > LastPage ?
                    0 :
                    Math.Min(From + PerPage, Total);
        public long Total { get; set; }
    }
}