using System.Text.Json.Serialization;

namespace ECommerce.Presentation.Dtos.Shared.Pagination;

public class PagedList<T>
{
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public List<T> Items { get; }

    [JsonConstructor]
    public PagedList(
        List<T> items,
        int totalCount,
        int currentPage,
        int pageSize)
    {
        TotalCount = totalCount;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        Items = items;
    }
}