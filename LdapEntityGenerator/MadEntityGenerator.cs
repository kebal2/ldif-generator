using LdapEntityGenerator.Entities;
using LdapEntityGenerator.Interfaces;

namespace LdapEntityGenerator
{
    public static class LdapEntryExtensions
    {
        public static string GenerateUserId(this LdapEntry user)
        {
            var uid = new string(Guid.NewGuid().ToString("n").Take(4).ToArray());

            uid += user.fn.Value.Length >= 7
                ? string.Concat(user.fn.Value.AsSpan(0, 7), user.gn.Value.AsSpan(0, 1))
                : string.Concat(user.fn.Value.AsSpan(0, user.fn.Value.Length), user.gn.Value.AsSpan(0, 1));

            return uid;
        }
    }

    public class MadEntityGenerator : BaseEntityGenerator, IMadEntityGenerator
    {
        public List<LdapEntry> GetLdapEntries(LdapEntryOptions opts, TextWriter tw)
        {
            List<LdapEntry> entries = new();

            if (opts.CreateBaseOrganization)
                entries.Add(CreateBaseDn(opts));

            if (opts.CreateAdmin)
                entries.AddRange(CreateAdmin(opts));

            if (opts.CreateBaseOrganization)
                entries.Add(CreateRootOu(opts));

            if (opts.UserCount > 0)
            {
                var createdOUs = CreateOUs(opts);
                entries.AddRange(createdOUs);

                var users = CreateUsers(opts, tw);
                entries.AddRange(users);

                var groups = CreateGroups(opts, createdOUs, users);
                entries.AddRange(groups);
            }

            return entries;
        }

        private static List<LdapEntry> CreateGroups(LdapEntryOptions opts, IList<LdapEntry> ous, List<LdapEntry> users)
        {
            List<LdapEntry> groups = new();
            if (!opts.OrgUnits.Any()) return groups;

            for (var i = 0; i < opts.Groups.Length; i++)
            {
                var group = opts.Groups[i];
                var destOu = ous[Rnd.Next(ous.Count)];

                LdapEntry groupEntry = new(opts.BaseDomain)
                {
                    cn = { Value = { group } },
                    changetype = { Value = opts.ChangeType }
                };

                groupEntry.ou.Value.AddRange(destOu.ou.Value);

                groupEntry.objectClass.Value.Add(ObjectClass.top);
                groupEntry.objectClass.Value.Add(ObjectClass.group);

                groups.Add(groupEntry);
            }

            var userdnsByGroupDn = GetUserdnsByGroupDn(groups, users, 4);

            foreach (var group in groups)
                if (userdnsByGroupDn.TryGetValue(group.dn.Value, out var members))
                    group.member.Value.AddRange(members);

            return groups;
        }

        private static Dictionary<string, List<string>> GetUserdnsByGroupDn(List<LdapEntry> groups, List<LdapEntry> users, int groupMemberCount)
        {
            if (users.Count < groupMemberCount)
                throw new InvalidOperationException(nameof(GetUserdnsByGroupDn));

            Dictionary<string, List<string>> userdnsByGroupDn = new();
            Random rand = new();

            foreach (var group in groups)
            {
                List<string> userDns = new();

                while (userDns.Count < groupMemberCount)
                {
                    var u = users[rand.Next(users.Count)];

                    if (!userDns.Contains(u.dn.Value))
                        userDns.Add(u.dn.Value);
                }

                userdnsByGroupDn.Add(group.dn.Value, userDns);
            }

            return userdnsByGroupDn;
        }

        private static List<LdapEntry> CreateUsers(LdapEntryOptions opts, TextWriter tw)
        {
            List<LdapEntry> users = new();
            HashSet<string> dnLut = new();

            for (int x = 1; x <= opts.UserCount; x++)
            {
                if (x % 10 == 0)
                    tw.WriteLine($"Progress: {(x * 100.0 / opts.UserCount):F2} %");

                LdapEntry user = CreateUniqueUser(opts, dnLut);

                var userId = user.GenerateUserId();

                user.uid.Value = userId;
                user.sAMAccountName.Value = userId;
                user.HassAMAccountName = true;
                user.description.Value = $"This is {user.gn.Value} {user.fn.Value}'s description";
                user.mail.Value = $"{userId}@{opts.BaseDomain}";
                user.userAccountControl.Value = (int)opts.UserAccountControl;
                user.accountExpires.Value = Int64.MaxValue;
                user.userPrincipalName.Value = user.mail.Value;
                user.unicodePwd.Value = opts.Password.AsUnicodeBase64();

                var ac = user.ac.Value;
                user.telephoneNumber.Value = $"+1 {ac} {Rnd.Next(100, 999)}-{Rnd.Next(1000, 9999)}";
                user.mobile.Value = "+1 " + ac + " " + Rnd.Next(100, 999) + $"-" + Rnd.Next(1000, 9999);

                users.Add(user);
            }

            return users;
        }

        private static LdapEntry CreateUniqueUser(LdapEntryOptions opts, HashSet<string> dnLut)
        {
            LdapEntry? user = default;

            bool isUnique = false;

            while (!isUnique)
            {
                user = new LdapEntry(opts.BaseDomain)
                {
                    changetype = { Value = opts.ChangeType }
                };

                user.fn.Value = nameGen.GenerateRandomFirstName();
                user.gn.Value = nameGen.GenerateRandomLastName();

                user.ac.Value = (Rnd.Next(8999) + 1000).ToString();
                user.l.Value = new RandomNameGeneratorNG.PlaceNameGenerator().GenerateRandomPlaceName();

                user.objectClass.Value.Add(ObjectClass.top);
                user.objectClass.Value.Add(ObjectClass.person);
                user.objectClass.Value.Add(ObjectClass.organizationalPerson);
                user.objectClass.Value.Add(ObjectClass.user);

                user.ou.Value.Add(opts.RootOu);
                user.cn.Value.Add($"{user.fn.Value} {user.gn.Value}");

                isUnique = !dnLut.Contains(user.dn.Value);

                if (isUnique)
                    dnLut.Add(user.dn.Value);
            }

            if (user is null)
                throw new Exception("Unique user creation failed!");

            return user;
        }
    }
}
