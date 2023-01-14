namespace Synonms.RestEasy.Abstractions.Schema;

public class Form
{
    public Form(Link target, IEnumerable<FormField> fields)
    {
        Target = target;
        Fields = fields;
    }

    public Link Target { get; }
    
    public IEnumerable<FormField> Fields { get; }
}