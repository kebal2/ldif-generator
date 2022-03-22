using System.Reflection;

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

var lDif = g.GetLdapEntries(p, Console.Out);

LdifFileRenderer renderer = new LdifFileRenderer();

var ldifChunks = renderer.RenderDiff(lDif, 5000);

for (var i = 0; i < ldifChunks.Length; i++)
{
    var fileName = $"output{i}.ldif";
    File.WriteAllText(fileName, ldifChunks[i]);
    
    Console.Out.WriteLine(Path.Combine((new FileInfo(Assembly.GetExecutingAssembly().Location)).Directory.FullName, fileName));
}