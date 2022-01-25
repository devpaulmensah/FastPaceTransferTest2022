using System.Collections.Generic;

namespace FastPaceTransferTest2022.Api.Models.Responses
{
    public class PaginatedList<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}