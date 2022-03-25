using System.Text;

namespace LdapEntityGenerator.Entities;

public class LdapEntry : IRenderableEntry
{
    public LdapEntryAttribute<string> initials => new(nameof(initials), string.IsNullOrEmpty(gn.Value) ? string.Empty : $"{gn.Value.First()}. {fn.Value.First()}.");
    public LdapEntryAttribute<string> userPassword { get; } = new(nameof(userPassword));
    public LdapEntryAttribute<string> telephoneNumber { get; } = new(nameof(telephoneNumber));
    public LdapEntryAttribute<string> mobile { get; } = new(nameof(mobile));
    public LdapEntryAttribute<string> mail { get; } = new(nameof(mail));
    public LdapEntryAttribute<string> unicodePwd { get; } = new(nameof(unicodePwd) + ":");
    public LdapEntryAttribute<string> accountExpires { get; } = new(nameof(accountExpires));
    public LdapEntryAttribute<string> userPrincipalName { get; } = new(nameof(userPrincipalName));
    public LdapEntryAttribute<string> userAccountControl { get; } = new(nameof(userAccountControl));
    public LdapEntryAttribute<string> sn => new(nameof(sn), fn.Value);
    public bool HassAMAccountName { get; set; }
    public LdapEntryAttribute<string> sAMAccountName { get; } = new(nameof(sAMAccountName));
    public LdapEntryAttributeList<string> ou { get; } = new(nameof(ou));
    public LdapEntryAttributeList<string> objectClass { get; } = new(nameof(objectClass));

    public LdapEntryAttributeList<string> cn { get; } = new(nameof(cn));

    public LdapEntryAttribute<int?> gidnumber { get; } = new(nameof(gidnumber));
    public LdapEntryAttribute<int?> uidnumber { get; } = new(nameof(uidnumber));
    public LdapEntryAttribute<string> homedirectory { get; } = new(nameof(homedirectory));

    public LdapEntryAttribute<string> fn { get; } = new(nameof(fn));
    public LdapEntryAttribute<string> gn { get; } = new(nameof(gn));
    public LdapEntryAttribute<string> givenName => new(nameof(givenName), gn.Value);
    public LdapEntryAttribute<string> ac { get; } = new(nameof(ac));
    public LdapEntryAttribute<string> l { get; } = new(nameof(l));
    public LdapEntryAttribute<string> uid { get; } = new(nameof(uid));

    public string dn => cn.Value.Any()
        ? $"dn: {string.Join(",", new[] { cn.AsValue(true), ou.AsValue(true), dc.AsValue() }.Where(e => !String.IsNullOrEmpty(e)))}"
        : $"dn: {string.Join(",", new[] { ou.AsValue(true), dc.AsValue() }.Where(e => !String.IsNullOrEmpty(e)))}";

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

        if (cn.Value.Any()) sb.AppendLine(cn.LastAsAttribute());
        if (!string.IsNullOrEmpty(description.Value)) sb.AppendLine(description.AsAttribute());
        if (gidnumber.Value.HasValue) sb.AppendLine(gidnumber.AsAttribute());
        if (!string.IsNullOrEmpty(givenName.Value)) sb.AppendLine(givenName.AsAttribute());
        if (!string.IsNullOrEmpty(initials.Value)) sb.AppendLine(initials.AsAttribute());
        if (!string.IsNullOrEmpty(l.Value)) sb.AppendLine(l.AsAttribute());
        if (!string.IsNullOrEmpty(mobile.Value)) sb.AppendLine(mobile.AsAttribute());
        if (!string.IsNullOrEmpty(homedirectory.Value)) sb.AppendLine(homedirectory.AsAttribute());

        sb.Append(objectClass.AsAttribute());


        if (!cn.Value.Any() && ou.Value.Any()) sb.AppendLine(ou.LastAsAttribute());

        if (!string.IsNullOrEmpty(sn.Value)) sb.AppendLine(sn.AsAttribute());

        if (!string.IsNullOrEmpty(sAMAccountName.Value)) sb.AppendLine(sAMAccountName.AsAttribute());

        if (!string.IsNullOrEmpty(telephoneNumber.Value)) sb.AppendLine(telephoneNumber.AsAttribute());


        if (!string.IsNullOrEmpty(uid.Value)) sb.AppendLine(uid.AsAttribute());
        if (!string.IsNullOrEmpty(mail.Value)) sb.AppendLine(mail.AsAttribute());

        if (uidnumber.Value.HasValue) sb.AppendLine(uidnumber.AsAttribute());
        if (!string.IsNullOrEmpty(userPassword.Value)) sb.AppendLine(userPassword.AsAttribute());

        if (!string.IsNullOrEmpty(userAccountControl.Value)) sb.AppendLine(userAccountControl.AsAttribute());
        if (!string.IsNullOrEmpty(unicodePwd.Value)) sb.AppendLine(unicodePwd.AsAttributeWOSpace());
        if (!string.IsNullOrEmpty(accountExpires.Value)) sb.AppendLine(accountExpires.AsAttribute());
        if (!string.IsNullOrEmpty(userPrincipalName.Value)) sb.AppendLine(userPrincipalName.AsAttribute());

        return sb.ToString();
    }
}