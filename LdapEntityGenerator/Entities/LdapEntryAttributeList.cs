using System.Text;

namespace LdapEntityGenerator.Entities;

public class LdapEntryAttributeList<T>
{
    public string Name { get; }
    public List<T> Value { get; } = new();

    public LdapEntryAttributeList(string atributeName)
    {
        Name = atributeName;
    }

    public LdapEntryAttributeList(string name, IEnumerable<T> value)
    {
        Name = name;
        Value = new List<T>(value);
    }

    public string AsValue(bool reverse = false)
    {
        var vv = Value.ToList();

        if (reverse)
            vv.Reverse();

        return string.Join(",", vv.Select(v => $"{Name}={v}"));
    }

    public string AsAttribute(bool reverse = false)
    {
        StringBuilder sb = new();
        var vv = Value.ToList();

        if (reverse)
            vv.Reverse();

        foreach (var v in vv) sb.AppendLine($"{Name}: {v}");

        return sb.ToString();
    }

    public string AsAttribute(int index)
    {
        return $"{Name}: {Value[index]}";
    }

    public string LastAsAttribute()
    {
        return $"{Name}: {Value.Last()}";
    }
    public string FirstAsAttribute()
    {
        return $"{Name}: {Value.First()}";
    }
}