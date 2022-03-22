using LdapEntityGenerator;
using LdapEntityGenerator.Entities;

var g = new EntityGenerator();

var orgUnits = new[] { "Account", "Home", "Apple" };

var groups = new[] { "" };

var p = new EntityGenerator.LdapEntryOptions("example.com",  "RootDomain",  10)
{
    OrgUnits = orgUnits,
    CreateAdmin = true,
    Password = "Password1",
    CreateBaseOrganization = true,
    CreateRootOu = true
};

var lDif = g.GetLdapEntries(p);

LdifFileRenderer renderer = new LdifFileRenderer();

File.WriteAllText("output.ldif", renderer.RenderDiff(lDif));