using LdapEntityGenerator.Entities;
using LdapEntityGenerator.Interfaces;

namespace LdapEntityGenerator
{
    public class GenericEntityGenerator : BaseEntityGenerator, IGenericEntityGenerator
    {
        private readonly TextWriter tw;

        public GenericEntityGenerator(TextWriter tw)
        {
            this.tw = tw;
        }

        public List<LdapEntry> GetLdapEntries(LdapEntryOptions opts)
        {
            List<LdapEntry> entries = new();

            if (opts.CreateBaseOrganization)
                entries.Add(CreateBaseDn(opts));

            entries.AddRange(CreateAdmin(opts));

            if (opts.CreateRootOu)
                entries.Add(CreateRootOu(opts));

            var createdOUs = CreateOUs(opts);
            var groups = CreateGroups(opts, createdOUs);

            entries.AddRange(createdOUs);
            entries.AddRange(groups);

            if (opts.UserCount <= 0) return entries;
            entries.AddRange(CreateUsers(opts, tw, groups));

            return entries;
        }

        private static List<LdapEntry> CreateGroups(LdapEntryOptions opts, IList<LdapEntry> ous)
        {
            int baseGidNumber = 1000;

            List<LdapEntry> r = new();
            if (!opts.OrgUnits.Any()) return r;

            for (var i = 0; i < opts.Groups.Length; i++)
            {
                var @group = opts.Groups[i];
                var destOu = ous[Rnd.Next(ous.Count)];

                LdapEntry entry = new(opts.BaseDomain)
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

        private static List<LdapEntry> CreateUsers(LdapEntryOptions opts, TextWriter tw, IList<LdapEntry> groupsList)
        {
            List<LdapEntry> entries = new();
            HashSet<string> dnLut = new();

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
                    var fn = NameGen.GenerateRandomFirstName();
                    var ln = NameGen.GenerateRandomLastName();

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

                    isUnique = !dnLut.Contains(entry.dn.Value);

                    if (isUnique)
                        dnLut.Add(entry.dn.Value);

                    else
                        tw.WriteLine($"Retry User: {x}");
                }

                // Generate UID based on Name
                var uid = entry.GenerateUserId();

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
    }
}
