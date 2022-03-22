using System.Text;

using LdapEntityGenerator.Entities;

namespace LdapEntityGenerator;

public class EntityGenerator
{
    private static readonly Random Rnd = new();

    public class LdapEntryOptions
    {
        public LdapEntryOptions(string baseDomain, string rootOu, int userCount)
        {
            BaseDomain = baseDomain;
            RootOu = rootOu;
            UserCount = userCount;
        }

        public string BaseDomain { get; set; }
        public string RootOu { get; }
        public int UserCount { get; }
        public string[] OrgUnits { get; set; }
        public string ChangeType { get; set; } = "add";
        public CbType CbType { get; set; } = CbType.GENERIC;
        public bool CreateAdmin { get; set; }
        public string Password { get; set; }
        public bool CreateBaseOrganization { get; set; }
        public bool CreateRootOu { get; set; }
    }

    RandomNameGeneratorNG.PersonNameGenerator nameGen = new RandomNameGeneratorNG.PersonNameGenerator();

    public List<LdapEntry> GetLdapEntries(LdapEntryOptions opts, TextWriter tw)
    {
        List<LdapEntry> entries = new List<LdapEntry>();

        if (opts.CreateBaseOrganization)
            entries.Add(CreateBaseDn(opts));

        entries.AddRange(CreateAdmin(opts));

        if (opts.CreateBaseOrganization)
            entries.Add(CreateRootOu(opts));

        entries.AddRange(CreateOUs(opts));

        if (opts.UserCount <= 0) return entries;

        for (int x = 1; x <= opts.UserCount; x++)
        {
            LdapEntry entry = default;

            bool isUnique = false;

            if(x % 10 == 0)
                tw.WriteLine($"Progress: {(x * 100.0 / opts.UserCount):F2} %");

            while (!isUnique)
            {
                entry = GetEntry(opts.BaseDomain, opts.RootOu, opts.ChangeType);

                // Get the random items for this Entry that we use in other things
                entry.fn.Value = nameGen.GenerateRandomFirstName();
                entry.gn.Value = nameGen.GenerateRandomLastName();
                entry.ou.Value.Add(GetRand(opts.OrgUnits));
                entry.ac.Value = (Rnd.Next(8999) + 1000).ToString();
                entry.l.Value = new RandomNameGeneratorNG.PlaceNameGenerator().GenerateRandomPlaceName();

                isUnique = entries.All(e => e.dn != entry.dn);
                
                if (!isUnique)
                    tw.WriteLine($"Retry User: {x}");
            }

            entries.Add(entry);

            // Generate UID based on Name
            var uid = new string(Guid.NewGuid().ToString("n").Take(4).ToArray());

            uid += entry.fn.Value.Length >= 7
                ? entry.fn.Value.Substring(0, 7) + entry.gn.Value.Substring(0, 1)
                : entry.fn.Value.Substring(0, entry.fn.Value.Length) + entry.gn.Value.Substring(0, 1);

            entry.uid.Value = uid;

            entry.objectClass.Value.Add(ObjectClass.top);
            entry.objectClass.Value.Add(ObjectClass.person);
            entry.objectClass.Value.Add(ObjectClass.organizationalPerson);
            entry.objectClass.Value.Add(ObjectClass.inetOrgPerson);

            entry.HassAMAccountName = opts.CbType == CbType.MAD;

            entry.description.Value = $"This is {entry.gn.Value} {entry.fn.Value}'s description";

            entry.mail.Value = $"{entry.uid.Value}@{new string(Guid.NewGuid().ToString("n").Take(8).ToArray())}.com";

            var ac = entry.ac.Value;

            entry.telephoneNumber.Value = $"+1 {ac} {Rnd.Next(100, 999)}-{Rnd.Next(1000, 9999)}";
            entry.userPassword.Value = opts.Password;
            entry.mobile.Value = "+1 " + ac + " " + Rnd.Next(100, 999) + $"-" + Rnd.Next(1000, 9999);

            //TODO: szervezeti szerepkör
        }

        return entries;
    }

    private LdapEntry CreateBaseDn(LdapEntryOptions p)
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

    private LdapEntry CreateRootOu(LdapEntryOptions p)
    {
        var entry = new LdapEntry(p.BaseDomain);

        entry.objectClass.Value.Add(ObjectClass.top);
        entry.objectClass.Value.Add(ObjectClass.organizationalUnit);
        entry.ou.Value.Add(p.RootOu);

        entry.changetype.Value = p.ChangeType;

        return entry;
    }

    private List<LdapEntry> CreateOUs(LdapEntryOptions p)
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

        return r;
    }

    private List<LdapEntry> CreateAdmin(LdapEntryOptions p)
    {
        List<LdapEntry> r = new();
        if (!p.CreateAdmin) return r;

        LdapEntry entry = new LdapEntry(p.BaseDomain)
        {
            cn = "admin",
            description = { Value = "LDAP Administrator" },
            userPassword = { Value = "Password1" },
            changetype = { Value = p.ChangeType }
        };

        entry.objectClass.Value.Add(ObjectClass.simpleSecurityObject);
        entry.objectClass.Value.Add(ObjectClass.organizationalRole);

        r.Add(entry);

        return r;
    }

    private LdapEntry GetEntry(string baseDn, string rootOU, string changeType)
    {
        var entry = new LdapEntry(baseDn)
        {
            changetype = { Value = changeType }
        };

        if (!string.IsNullOrEmpty(rootOU)) entry.ou.Value.Add(rootOU);

        return entry;
    }

    private string GetRand(string[] array)
    {
        if (array.Length <= 0) throw new("Array length is 0");

        return array[Rnd.Next(array.Length)];
    }
}