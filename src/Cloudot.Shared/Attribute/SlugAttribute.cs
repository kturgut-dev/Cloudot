namespace Cloudot.Shared.Attribute;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SlugifyAttribute : System.Attribute
{
    public string From { get; }

    public SlugifyAttribute(string from)
    {
        From = from;
    }
}
