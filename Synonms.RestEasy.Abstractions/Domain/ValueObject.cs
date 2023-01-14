namespace Synonms.RestEasy.Abstractions.Domain;

public abstract record ValueObject<T>
{
    protected ValueObject(T value)
    {
        Value = value;
    }
    
    public T Value { get; private set; }

    public static Type UnderlyingType => typeof(T);
}
