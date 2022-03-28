namespace LdapEntityGenerator;

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
    public bool CreateAdmin { get; set; }
    public string Password { get; set; }
    public bool CreateBaseOrganization { get; set; }
    public bool CreateRootOu { get; set; }
    public string[] Groups { get; set; }
    public UserAccessControlFlags UserAccountControl { get; set; }
    public int GroupMemberCount { get; set; }
}
