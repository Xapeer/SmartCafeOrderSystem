namespace Application.Common;

public class PagedResponse<T> : Response<List<T>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }

    public PagedResponse(List<T> data, int pageNumber, int pageSize, int totalRecords)
        : base(200, "Success", data)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
    }
}