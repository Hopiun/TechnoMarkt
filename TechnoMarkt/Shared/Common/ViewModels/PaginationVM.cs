using TechnoMarkt.Models;
namespace TechnoMarkt.Shared.Common.ViewModels
{
    public class PaginationVM
    {
        public int CurrentPage { get; set; } = 1;
        public int TotalCount { get; set; }
        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}




