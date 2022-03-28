using LdapEntityGenerator.Entities;

using RandomNameGeneratorNG;

namespace LdapEntityGenerator;

public abstract class BaseEntityGenerator
{
    protected static readonly Random Rnd = new();
    protected static readonly PersonNameGenerator nameGen = new();

    protected static LdapEntry CreateBaseDn(LdapEntryOptions p)
    {
        var entry = new LdapEntry(p.BaseDomain)
        {
            changetype = { Value = p.ChangeType }
        };

        entry.o.Value = entry.dc.Value.First();

        entry.objectClass.Value.Add(ObjectClass.top);
        entry.objectClass.Value.Add(ObjectClass.dcObject);
        entry.objectClass.Value.Add(ObjectClass.organization);

        return entry;
    }

    protected static LdapEntry CreateRootOu(LdapEntryOptions p)
    {
        var entry = new LdapEntry(p.BaseDomain);

        entry.objectClass.Value.Add(ObjectClass.top);
        entry.objectClass.Value.Add(ObjectClass.organizationalUnit);
        entry.ou.Value.Add(p.RootOu);

        entry.changetype.Value = p.ChangeType;

        return entry;
    }

    protected static List<LdapEntry> CreateOUs(LdapEntryOptions p)
    {
        List<LdapEntry> r = new();
        if (!p.OrgUnits.Any()) return r;

        foreach (var orgUnit in p.OrgUnits)
        {
            LdapEntry entry = GetEntry(p.BaseDomain, p.RootOu, p.ChangeType);

            entry.ou.Value.Add(orgUnit);

            entry.objectClass.Value.Add(ObjectClass.top);
            entry.objectClass.Value.Add(ObjectClass.organizationalUnit);

            r.Add(entry);
        }

        return (r);
    }

    protected static List<LdapEntry> CreateAdmin(LdapEntryOptions p)
    {
        List<LdapEntry> r = new();
        if (!p.CreateAdmin) return r;

        LdapEntry entry = new(p.BaseDomain)
        {
            cn = { Value = { "admin" } },
            description = { Value = "LDAP Administrator" },
            userPassword = { Value = p.Password },
            changetype = { Value = p.ChangeType }
        };

        entry.objectClass.Value.Add(ObjectClass.simpleSecurityObject);
        entry.objectClass.Value.Add(ObjectClass.organizationalRole);

        r.Add(entry);

        return r;
    }

    private static LdapEntry GetEntry(string baseDn, string rootOU, string changeType)
    {
        var entry = new LdapEntry(baseDn)
        {
            changetype = { Value = changeType }
        };

        if (!string.IsNullOrEmpty(rootOU)) entry.ou.Value.Add(rootOU);

        return entry;
    }

    protected static T GetRand<T>(T[] array)
    {
        var count = array.Length;
        if (count <= 0) throw new("Array length is 0");

        return array[Rnd.Next(count)];
    }
}