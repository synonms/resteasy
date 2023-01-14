namespace Synonms.RestEasy.Abstractions.Domain;

public record EntityTag(byte[] Value)
{
    public static EntityTag Uninitialised => new (Array.Empty<byte>());
    
    public static implicit operator EntityTag(byte[] value) => new (value);

    public static implicit operator byte[](EntityTag tag) => tag.Value;

    public bool IsUninitialised => Value.Any() is false;
}