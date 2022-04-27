using System.Reflection;

using App;

using CommandLine;

using LdapEntityGenerator;
using LdapEntityGenerator.Interfaces;

Gui.Start(o =>
{
    
    var offset = 1;
    var p = new LdapEntryOptions(o.BaseDomain, o.RootOu, o.UserCount)
    {
        OrgUnits = Enumerable.Range(offset, o.OuCount + offset).Select(i => $"organizationUnit{i:0000}").ToArray(),
        Groups = Enumerable.Range(offset, o.GroupCount + offset).Select(i => $"group{i:0000}").ToArray(),

        CreateAdmin = o.CreateAdmin,
        Password = @$"""{o.Password}""",
        CreateBaseOrganization = o.CreateBaseOu,
        CreateRootOu = o.CreateRootOu,
        UserAccountControl = (UserAccessControlFlags)o.UserAccessControl,
        GroupMemberCount = o.GroupMemberCount
    };

    IEntityGenerator? g = default;

    g = o.LdifType switch
    {
        LdapEntityGenerator.Entities.CbType.MAD => new MadEntityGenerator(Console.Out),
        LdapEntityGenerator.Entities.CbType.GENERIC => new GenericEntityGenerator(Console.Out),
        _ => throw new Exception($"Unknown {nameof(o.LdifType)}: {o.LdifType}"),
    };
    var lDif = g.GetLdapEntries(p);

    LdifFileRenderer renderer = new();

    var ldifChunks = renderer.RenderDiff(lDif, o.FileSizeLimit);

    var filePath = o.OutputPath == "." ? (new FileInfo(Assembly.GetExecutingAssembly().Location)).Directory.FullName : o.OutputPath;

    var fileParts = o.FileName.Split(".").ToList();
    fileParts.Insert(fileParts.Count - 1, "{0}");
    var fileFormat = string.Join(".", fileParts);

    for (var i = 0; i < ldifChunks.Length; i++)
    {
        var fileName = string.Format(fileFormat, i);

        var path = Path.Combine(filePath, fileName);

        File.WriteAllText(path, ldifChunks[i]);
    }
});
