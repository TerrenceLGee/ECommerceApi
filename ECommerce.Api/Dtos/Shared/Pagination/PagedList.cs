using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Dtos.Shared.Pagination;

public class PagedList<T>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public List<T> Items { get; }

    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Items = items;
    }

    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}