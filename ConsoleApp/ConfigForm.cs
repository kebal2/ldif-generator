using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using LdapEntityGenerator.Entities;

using Terminal.Gui;

namespace App;

public class ConfigForm
{
    [DisplayName("                 User count: ")] [DefaultValue(100)]
    internal TextField UserCount { get; set; }

    [DisplayName("                Group count: ")] [DefaultValue(10)]
    internal TextField GroupCount { get; set; }

    [DisplayName("    Organization unit count: ")] [DefaultValue(10)]
    internal TextField OrganizationUnitCount { get; set; }

    [DisplayName("         Group member count: ")] [DefaultValue(4)]
    internal TextField GroupMemberCount { get; set; }

    [DisplayName("              User password: ")] [DefaultValue("AnExamplePassword1")]
    internal TextField UserPassword { get; set; }

    [DisplayName("           Base domain name: ")] [DefaultValue("example.com")]
    internal TextField BaseDomain { get; set; }

    [DisplayName("Root organization unit name: ")] [DefaultValue("corp")]
    internal TextField RootOU { get; set; }

    [DisplayName("Should create admin user")] [DefaultValue(false)]
    internal CheckBox CreateAdminUser { get; set; }

    [DisplayName("Should create base organization unit")] [DefaultValue(false)]
    internal CheckBox CreateBaseOU { get; set; }

    [DisplayName("Should create root organization unit")] [DefaultValue(false)]
    internal CheckBox CreateRootOU { get; set; }

    [DisplayName("File Type to generate")] [DefaultValue(1)] [EnumDataType(typeof(CbType))]
    internal RadioGroup Type { get; set; }

    [DisplayName("User access control: ")] [DefaultValue(0x0200)]
    internal TextField UserAccessControl { get; set; }

    [DisplayName("        Output path: ")] [DefaultValue(".")]
    internal TextField OutputPath { get; set; }

    [DisplayName("          File name: ")] [DefaultValue("output.ldif")]
    internal TextField FileName { get; set; }

    [DisplayName("    File size limit: ")] [DefaultValue(2_000_000)]
    internal TextField FileSizeLimit { get; set; }

    [DisplayName("Generate")]
    private Button Post { get; set; }

    internal void SetPost(Action post)
    {
        Post.Clicked += post;
    }
}