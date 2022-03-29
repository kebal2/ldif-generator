using LdapEntityGenerator.Entities;

namespace LdapEntityGenerator.Interfaces
{
    public interface IEntityGenerator
    {
        List<LdapEntry> GetLdapEntries(LdapEntryOptions opts);
    }
}
