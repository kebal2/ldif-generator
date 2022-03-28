using LdapEntityGenerator.Entities;

using RandomNameGeneratorNG;

namespace LdapEntityGenerator;

public abstract class EntityGenerator
{
    protected static readonly Random Rnd = new();
    protected static readonly PersonNameGenerator nameGen = new();

    public abstract List<LdapEntry> GetLdapEntries(LdapEntryOptions opts, TextWriter tw);

    public List<LdapEntry> CreateGroups(LdapEntryOptions opts, IList<LdapEntry> ous)
    {
        int baseGidNumber = 1000;

        List<LdapEntry> r = new();
        if (!opts.OrgUnits.Any()) return r;

        for (var i = 0; i < opts.Groups.Length; i++)
        {
            var @group = opts.Groups[i];
            var destOu = ous[Rnd.Next(ous.Count)];

            LdapEntry entry = new LdapEntry(opts.BaseDomain)
            {
                cn = { Value = { @group } },
                changetype = { Value = opts.ChangeType }
            };

            entry.ou.Value.AddRange(destOu.ou.Value);
            entry.gidnumber.Value = baseGidNumber + i;
            entry.objectClass.Value.Add(ObjectClass.top);
            entry.objectClass.Value.Add(ObjectClass.posixGroup);

            r.Add(entry);
        }

        return r;
    }

    public List<LdapEntry> CreateUsers(LdapEntryOptions opts, TextWriter tw, IList<LdapEntry> groupsList)
    {
        List<LdapEntry> entries = new List<LdapEntry>();
        HashSet<string> dnLut = new HashSet<string>();

        int baseUidNumber = 2000;

        var groups = groupsList?.ToArray();

        for (int x = 1; x <= opts.UserCount; x++)
        {
            LdapEntry entry = default;

            bool isUnique = false;

            if (x % 10 == 0)
                tw.WriteLine($"Progress: {(x * 100.0 / opts.UserCount):F2} %");

            while (!isUnique)
            {
                entry = new LdapEntry(opts.BaseDomain)
                {
                    changetype = { Value = opts.ChangeType }
                };

                // Get the random items for this Entry that we use in other things
                var fn = nameGen.GenerateRandomFirstName();
                var ln = nameGen.GenerateRandomLastName();

                entry.fn.Value = fn;
                entry.gn.Value = ln;

                entry.ac.Value = (Rnd.Next(8999) + 1000).ToString();
                entry.l.Value = new RandomNameGeneratorNG.PlaceNameGenerator().GenerateRandomPlaceName();

                entry.objectClass.Value.Add(ObjectClass.top);
                entry.objectClass.Value.Add(ObjectClass.person);
                entry.objectClass.Value.Add(ObjectClass.organizationalPerson);

                if (groups is null || !groups.Any())
                {
                    entry.ou.Value.Add(opts.RootOu);
                    entry.ou.Value.Add(GetRand(opts.OrgUnits));

                    entry.objectClass.Value.Add(ObjectClass.user);
                }
                else
                {
                    var group = GetRand(groups);

                    entry.gidnumber.Value = group.gidnumber.Value;
                    entry.uidnumber.Value = baseUidNumber + x;
                    entry.homedirectory.Value = "/";
                    entry.cn.Value.AddRange(group.cn.Value);
                    entry.ou.Value.AddRange(group.ou.Value);
                    entry.objectClass.Value.Add(ObjectClass.posixAccount);
                }

                entry.cn.Value.Add($"{fn} {ln}");

                isUnique = !dnLut.Contains(entry.dn);

                if (isUnique)
                    dnLut.Add(entry.dn);

                else
                    tw.WriteLine($"Retry User: {x}");
            }

            // Generate UID based on Name
            var uid = new string(Guid.NewGuid().ToString("n").Take(4).ToArray());

            uid += entry.fn.Value.Length >= 7
                ? entry.fn.Value.Substring(0, 7) + entry.gn.Value.Substring(0, 1)
                : entry.fn.Value.Substring(0, entry.fn.Value.Length) + entry.gn.Value.Substring(0, 1);

            entry.uid.Value = uid;

            entry.description.Value = $"This is {entry.gn.Value} {entry.fn.Value}'s description";

            entry.mail.Value = $"{uid}@{opts.BaseDomain}";

            var ac = entry.ac.Value;

            entry.telephoneNumber.Value = $"+1 {ac} {Rnd.Next(100, 999)}-{Rnd.Next(1000, 9999)}";

            entry.mobile.Value = "+1 " + ac + " " + Rnd.Next(100, 999) + $"-" + Rnd.Next(1000, 9999);

            entries.Add(entry);

            entry.userPassword.Value = opts.Password;
        }

        return entries;
    }

    public LdapEntry CreateBaseDn(LdapEntryOptions p)
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

    public LdapEntry CreateRootOu(LdapEntryOptions p)
    {
        var entry = new LdapEntry(p.BaseDomain);

        entry.objectClass.Value.Add(ObjectClass.top);
        entry.objectClass.Value.Add(ObjectClass.organizationalUnit);
        entry.ou.Value.Add(p.RootOu);

        entry.changetype.Value = p.ChangeType;

        return entry;
    }

    public List<LdapEntry> CreateOUs(LdapEntryOptions p)
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

    public List<LdapEntry> CreateAdmin(LdapEntryOptions p)
    {
        List<LdapEntry> r = new();
        if (!p.CreateAdmin) return r;

        LdapEntry entry = new LdapEntry(p.BaseDomain)
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

    public LdapEntry GetEntry(string baseDn, string rootOU, string changeType)
    {
        var entry = new LdapEntry(baseDn)
        {
            changetype = { Value = changeType }
        };

        if (!string.IsNullOrEmpty(rootOU)) entry.ou.Value.Add(rootOU);

        return entry;
    }

    private static T GetRand<T>(T[] array)
    {
        var count = array.Length;
        if (count <= 0) throw new("Array length is 0");

        return array[Rnd.Next(count)];
    }
}