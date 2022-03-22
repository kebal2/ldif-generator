using ldifgen;

var g = new Generator();

var userObjectClasses = new[]
{
    "top",
    "person",
    "organizationalPerson",
    "inetOrgPerson"
};

var orgUnits = new[] { "Account", "Home", "Apple" };

var groups = new[] { "" };

var lDif = g.GenLDif("dc=example,dc=com", true, "RootDomain", orgUnits, "add", 10, userObjectClasses, Generator.cbType.GENERIC);

File.WriteAllText("output.ldif", lDif);