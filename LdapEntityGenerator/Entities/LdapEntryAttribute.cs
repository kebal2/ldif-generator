namespace LdapEntityGenerator.Entities;

public class LdapEntryAttribute<T>
{
    public string Name { get; }
    public T Value { get; set; }

    public LdapEntryAttribute(string atributeName)
    {
        Name = atributeName;
    }

    public LdapEntryAttribute(string name, T value)
    {
        Name = name;
        Value = value;
    }

    public string AsValue()
    {
        return $"{Name}={Value}";
    }

    public string AsAttribute()
    {
        return $"{Name}: {Value}";
    }
}