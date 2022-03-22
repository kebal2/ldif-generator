using System.Text;

namespace LdapEntityGenerator.Entities;

public class LdapEntry : IRenderableEntry
{
    public LdapEntryAttribute<string> initials => new(nameof(initials), string.IsNullOrEmpty(gn.Value) ? string.Empty : $"{gn.Value.First()}. {fn.Value.First()}.");
    public LdapEntryAttribute<string> userPassword { get; } = new(nameof(userPassword));
    public LdapEntryAttribute<string> telephoneNumber { get; } = new(nameof(telephoneNumber));
    public LdapEntryAttribute<string> mobile { get; } = new(nameof(mobile));
    public LdapEntryAttribute<string> mail { get; } = new(nameof(mail));
    public string sn => fn.Value;
    public bool HassAMAccountName { get; set; }
    public string sAMAccountName => HassAMAccountName ? uid.Value : string.Empty;
    public LdapEntryAttributeList<string> ou { get; } = new(nameof(ou));
    public LdapEntryAttributeList<string> objectClass { get; } = new(nameof(objectClass));

    public string cn
    {
        get => string.IsNullOrEmpty(_cn) ? string.IsNullOrEmpty(gn.Value) ? string.Empty : $"{gn.Value} {fn.Value}" : _cn;
        set => _cn = value;
    }

    private string _cn;

    public LdapEntryAttribute<string> GetCn => new(nameof(cn), cn);
    public LdapEntryAttribute<string> fn { get; } = new(nameof(fn));
    public LdapEntryAttribute<string> gn { get; } = new(nameof(gn));
    public LdapEntryAttribute<string> givenName => new(nameof(givenName), gn.Value);
    public LdapEntryAttribute<string> ac { get; } = new(nameof(ac));
    public LdapEntryAttribute<string> l { get; } = new(nameof(l));
    public LdapEntryAttribute<string> uid { get; } = new(nameof(uid));

    public string dn => string.IsNullOrEmpty(cn)
        ? $"dn: {string.Join(",", new[] { ou.AsValue(true), dc.AsValue() }.Where(e => !String.IsNullOrEmpty(e)))}"
        : $"dn: {string.Join(",", new[] { GetCn.AsValue(), ou.AsValue(true), dc.AsValue() }.Where(e => !String.IsNullOrEmpty(e)))}";

    public LdapEntryAttribute<string> description { get; } = new(nameof(description));
    public LdapEntryAttribute<string> changetype { get; } = new(nameof(changetype));
    public LdapEntryAttribute<string> o { get; } = new(nameof(o));
    public LdapEntryAttributeList<string> dc { get; } = new(nameof(dc));

    public LdapEntry(string dc)
    {
        this.dc.Value.AddRange(dc.Split('.'));
    }

    string IRenderableEntry.Render()
    {
        StringBuilder sb = new();

        sb.AppendLine(dn);
        sb.AppendLine(changetype.AsAttribute());

        if (objectClass.Value.Contains(ObjectClass.dcObject)) sb.AppendLine(dc.FirstAsAttribute());
        if (!string.IsNullOrEmpty(o.Value)) sb.AppendLine(o.AsAttribute());
        if (ou.Value.Any()) sb.AppendLine(ou.LastAsAttribute());
        sb.Append(objectClass.AsAttribute());


        if (!string.IsNullOrEmpty(GetCn.Value)) sb.AppendLine(GetCn.AsAttribute());
        if (!string.IsNullOrEmpty(sn)) sb.AppendLine(sn);

        if (HassAMAccountName) sb.AppendLine(sAMAccountName);

        if (!string.IsNullOrEmpty(description.Value)) sb.AppendLine(description.AsAttribute());
        if (!string.IsNullOrEmpty(telephoneNumber.Value)) sb.AppendLine(telephoneNumber.AsAttribute());
        if (!string.IsNullOrEmpty(mobile.Value)) sb.AppendLine(mobile.AsAttribute());
        if (!string.IsNullOrEmpty(l.Value)) sb.AppendLine(l.AsAttribute());
        if (!string.IsNullOrEmpty(userPassword.Value)) sb.AppendLine(userPassword.AsAttribute());
        if (!string.IsNullOrEmpty(uid.Value)) sb.AppendLine(uid.AsAttribute());
        if (!string.IsNullOrEmpty(givenName.Value)) sb.AppendLine(givenName.AsAttribute());
        if (!string.IsNullOrEmpty(mail.Value)) sb.AppendLine(mail.AsAttribute());
        if (!string.IsNullOrEmpty(initials.Value)) sb.AppendLine(initials.AsAttribute());

        return sb.ToString();
    }
}