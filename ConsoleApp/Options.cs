
using CommandLine;

using LdapEntityGenerator.Entities;

namespace App;

public class Options
{
    [Option('u', "users", Required = true, HelpText = "Set generated user count.")]
    public int UserCount { get; set; }

    [Option('g', "groups", Required = true, HelpText = "Set generated group count.")]
    public int GroupCount { get; set; }

    [Option('o', "ous", Required = true, HelpText = "Set generated organization unit count.")]
    public int OuCount { get; set; }

    [Option('p', "password", Required = false, HelpText = "Set user password.", Default = "AnExamplePassword1!")]
    public string Password { get; set; }

    [Option('a', "createAdmin", Required = false, HelpText = "Should create admin user.", Default = false)]
    public bool CreateAdmin { get; set; }

    [Option('e', "createBaseOu", Required = false, HelpText = "Should create base organization unit.", Default = false)]
    public bool CreateBaseOu { get; set; }

    [Option('b', "baseDomain", Required = false, HelpText = "Base domain name.", Default = "example.org")]
    public string BaseDomain { get; set; }

    [Option('y', "createRootOu", Required = false, HelpText = "Should create root organization unit.", Default = false)]
    public bool CreateRootOu { get; set; }

    [Option('r', "rootOu", Required = false, HelpText = "Should create root organization unit.", Default = "RootOU")]
    public string RootOu { get; set; }

    [Option('t', "type", Required = false, HelpText = "Generated ldif file system type [MSAD, generic].", Default = CbType.MAD)]
    public CbType LdifType { get; set; }

    [Option('c', "accessControl", Required = false, HelpText = "Set MSAD user access control.", Default = 0x0200)]
    public int UserAccessControl { get; set; }

    [Option('s', "fileSizeLimit", Required = false, HelpText = "Set generated file size limit.", Default = 2_000_000)]
    public int FileSizeLimit { get; set; }

    [Option('x', "outputPath", Required = false, HelpText = "Set output file path.", Default = ".")]
    public string OutputPath { get; set; }

    [Option('f', "fileName", Required = false, HelpText = "Set output file name.", Default = "output.ldif")]
    public string FileName { get; set; }

    [Option('m', "groupMemberCount", Required = false, HelpText = "Set member count of the groups.", Default = 1)]
    public int GroupMemberCount { get; set; }
}