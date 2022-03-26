namespace LdapEntityGenerator.Entities;

public class LdapEntryAttribute<T>
{
    private readonly string attributeDelim;
    private readonly string valueDelim;
    public string Name { get; }
    public T Value { get; set; }

    public LdapEntryAttribute(string attributeName, string attributeDelim = ": ", string valueDelim = "=")
    {
        this.attributeDelim = attributeDelim;
        this.valueDelim = valueDelim;
        Name = attributeName;
    }

    public LdapEntryAttribute(string name, T value, string attributeDelim = ": ", string valueDelim = "=") : this(name, attributeDelim, valueDelim)
    {
        Value = value;
    }

    public string AsValue()
    {
        return $"{Name}{valueDelim}{Value}";
    }

    public string AsAttribute()
    {
        return $"{Name}{attributeDelim}{Value}";
    }
}