using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ldifgen;

public class Generator
{
    private static Random rnd = new();

    public enum cbType
    {
        MAD, //ActiveDirectory
        GENERIC,
        EDIRECTORY
    }

    public static class objectClass
    {
        public const string top = nameof(top);
        public const string organizationalUnit = nameof(organizationalUnit);
        public const string organizationalPerson = nameof(organizationalPerson);
        public const string person = nameof(person);
        public const string inetOrgPerson = nameof(inetOrgPerson);
    }

    public class LdapEntryAttribute<T> where T : class
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
    }

    public class LdapEntry
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
        public string cn => string.IsNullOrEmpty(gn.Value) ? string.Empty : $"cn={gn.Value} {fn.Value}";
        public LdapEntryAttribute<string> fn { get; } = new(nameof(fn));
        public LdapEntryAttribute<string> gn { get; } = new(nameof(gn));
        public LdapEntryAttribute<string> givenName => new(nameof(givenName), gn.Value);
        public LdapEntryAttribute<string> ac { get; } = new(nameof(ac));
        public LdapEntryAttribute<string> l { get; } = new(nameof(l));
        public LdapEntryAttribute<string> uid { get; } = new(nameof(uid));
        public string dn => string.IsNullOrEmpty(cn) ? $"dn: {ou.AsValue(true)}, {dc}" : $"dn: {cn},{ou.AsValue(true)}, {dc}";
        public LdapEntryAttribute<string> description { get; } = new(nameof(description));

        public string dc { get; }

        public LdapEntry(string dc)
        {
            this.dc = dc;
        }

        public LdapEntryAttribute<string> changetype { get; } = new LdapEntryAttribute<string>(nameof(changetype));

        public string Generate()
        {
            StringBuilder sb = new();

            sb.AppendLine(dn);
            sb.AppendLine(changetype.AsAttribute());

            sb.Append(objectClass.AsAttribute());
            
            if (ou.Value.Any()) sb.AppendLine(ou.LastAsAttribute());

            if (!string.IsNullOrEmpty(cn))sb.AppendLine(cn);
            if (!string.IsNullOrEmpty(sn))sb.AppendLine(sn);

            if (HassAMAccountName)
                sb.AppendLine(sAMAccountName);

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

    public LdapEntry GetEntry(string baseDn, string rootOU, string changeType)
    {
        var entry = new LdapEntry(baseDn);

        if (!string.IsNullOrEmpty(rootOU)) entry.ou.Value.Add(rootOU);

        entry.changetype.Value = changeType != "None" ? changeType : "add";

        return entry;
    }

    internal string GenLDif(string baseDN, bool generateOrganizationalUnits, string rootOU, string[] orgUnits, string changeType, int userCount, string[] userObjectClasses, cbType cbType)
    {
        List<LdapEntry> entries = new List<LdapEntry>();

        if (generateOrganizationalUnits)
            // Generate OrganizationalUnits ?
            foreach (var orgUnit in orgUnits)
            {
                LdapEntry entry = GetEntry(baseDN, rootOU, changeType);

                entry.ou.Value.Add(orgUnit);

                entry.objectClass.Value.Add(objectClass.top);
                entry.objectClass.Value.Add(objectClass.organizationalUnit);

                entries.Add(entry);
            }

        if (userCount > 0)
        {
            var objectClass = userObjectClasses.ToArray();
            for (int x = 1; x <= userCount; x++)
            {
                LdapEntry entry = default;

                bool any = true;

                while (any)
                {
                    entry = GetEntry(baseDN, rootOU, changeType);

                    // Get the random items for this Entry that we use in other things
                    entry.fn.Value = new RandomNameGeneratorNG.PersonNameGenerator().GenerateRandomFirstName();
                    entry.gn.Value = new RandomNameGeneratorNG.PersonNameGenerator().GenerateRandomFirstName();
                    entry.ou.Value.Add(GetRand(orgUnits));
                    entry.ac.Value = (rnd.Next(8999) + 1000).ToString();
                    entry.l.Value = new RandomNameGeneratorNG.PlaceNameGenerator().GenerateRandomPlaceName();

                    any = entries.Any(e => e.dn == entry.dn);
                }

                entries.Add(entry);

                // Generate UID based on Name
                var uid = new string(Guid.NewGuid().ToString("n").Take(4).ToArray());

                uid += entry.fn.Value.Length >= 7
                    ? entry.fn.Value.Substring(0, 7) + entry.gn.Value.Substring(0, 1)
                    : entry.fn.Value.Substring(0, entry.fn.Value.Length) + entry.gn.Value.Substring(0, 1);

                entry.uid.Value = uid;

                entry.objectClass.Value.Add(Generator.objectClass.top);
                entry.objectClass.Value.Add(Generator.objectClass.person);
                entry.objectClass.Value.Add(Generator.objectClass.organizationalPerson);
                entry.objectClass.Value.Add(Generator.objectClass.inetOrgPerson);

                entry.HassAMAccountName = cbType == cbType.MAD;

                entry.description.Value = $"This is {entry.gn.Value} {entry.fn.Value}'s description";

                entry.mail.Value = entry.uid.Value + "@" + new string(Guid.NewGuid().ToString("n").Take(8).ToArray()) +
                                   ".com";

                var ac = entry.ac.Value;

                entry.telephoneNumber.Value = $"+1 {ac} {rnd.Next(100, 999)}-{rnd.Next(1000, 9999)}";
                entry.userPassword.Value = "Password1";
                entry.mobile.Value = "+1 " + ac + " " + rnd.Next(100, 999) + $"-" + rnd.Next(1000, 9999);

                //TODO: szervezeti szerepkör
            }
        }

        var output = new StringBuilder();

        output.AppendLine("version: 1");
        output.AppendLine();

        foreach (var entry in entries)
        {
            output.AppendLine(entry.Generate());
        }

        return output.ToString();
    }

    private string GetRand(string[] array)
    {
        if (array.Length <= 0) throw new("Array length is 0");

        return array[rnd.Next(array.Length)];
    }
}