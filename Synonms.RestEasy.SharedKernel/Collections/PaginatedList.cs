namespace Synonms.RestEasy.SharedKernel.Collections;

public class PaginatedList<T> : List<T>
{
    public int Offset { get; }
    
    public int Limit { get; }
    
    public int Size { get; }

    public PaginatedList(List<T> items, int offset, int limit, int size)
    {
        Offset = offset;
        Limit = limit;
        Size = size;

        AddRange(items);
    }

    public bool HasPrevious => Offset > 1;

    public bool HasNext => (Offset + Limit) < Size;

    public static PaginatedList<T> Create(IQueryable<T> source, int offset, int limit)
    {
        int size = source.Count();
        List<T> items = source.Skip(offset).Take(limit).ToList();
        return new PaginatedList<T>(items, offset, limit, size);
    }
    
    public static PaginatedList<T> Create(IEnumerable<T> source, int offset, int limit, int size)
    {
        List<T> items = source.ToList();
        return new PaginatedList<T>(items, offset, limit, size);
    }
}