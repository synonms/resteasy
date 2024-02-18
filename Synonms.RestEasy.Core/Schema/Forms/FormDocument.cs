namespace Synonms.RestEasy.Core.Schema.Forms;

public class FormDocument : Document
{
    public FormDocument(Link selfLink, Form form) 
        : base(selfLink)
    {
        Form = form;
    }
    
    public Form Form { get; }
}