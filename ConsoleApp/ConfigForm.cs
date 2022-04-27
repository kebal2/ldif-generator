using LdapEntityGenerator.Entities;

using NStack;

using Terminal.Gui;

namespace App;

public class ConfigForm
{
    private Label UserCountLabel { get; }
    private TextField UserCount { get; }

    private Label GroupCountLabel { get; }
    private TextField GroupCount { get; }
    private Label OrganizationUnitLabel { get; }
    private TextField OrganizationUnit { get; }
    private Label UserPasswordLabel { get; }
    private TextField UserPassword { get; }
    private Label BaseDomainLabel { get; }
    private TextField BaseDomain { get; }
    private Label RootOULabel { get; }
    private TextField RootOU { get; }
    private CheckBox CreateAdminUser { get; }
    private CheckBox CreateBaseOU { get; }
    private CheckBox CreateRootOU { get; }
    private RadioGroup Type { get; }
    private FrameView FileTypeFrame { get; }
    private Label UserAccessControlLabel { get; }
    private TextField UserAccessControl { get; }
    private Label FileSizeLimitLabel { get; }
    private TextField FileSizeLimit { get; }
    private Label OutputPathLabel { get; }
    private TextField OutputPath { get; }
    private Label FileNameLabel { get; }
    private TextField FileName { get; }
    private Label GroupMemberCountLabel { get; }
    private TextField GroupMemberCount { get; }

    private Button Post { get; }

    public ConfigForm(Action<Options> post)
    {
        int startPositionY = 1;
        int startPositionX = 3;

        var userCount = ElemFactory.CreateTextField("User count: ", startPositionX, startPositionY, "1000");
        var groupCount = ElemFactory.CreateTextField("Group count: ", userCount.label, "10");
        var organizationUnit = ElemFactory.CreateTextField("Organization unit count: ", groupCount.label, "10");
        var groupMemberCount = ElemFactory.CreateTextField("Group member count: ", organizationUnit.label, "1");
        var userPassword = ElemFactory.CreateTextField("User password: ", groupMemberCount.label, "AnExamplePassword1");
        var baseDomain = ElemFactory.CreateTextField("Base domain name: ", userPassword.label, "example.com");
        var rootOu = ElemFactory.CreateTextField("Root organization unit: ", baseDomain.label, "ADC");

        var createAdminUser = ElemFactory.CreateCheckBox("Should create administrative user", rootOu.label);
        var createBaseOrganizationUnit = ElemFactory.CreateCheckBox("Should create base organization unit", createAdminUser);
        var createRootOrganizationUnit = ElemFactory.CreateCheckBox("Should create root organization unit", createBaseOrganizationUnit);

        var typeSelector = new RadioGroup(new ustring[] { "G_ENERIC", "_MAD" }, 1);

        var frameTitle = "Ldif file type";
        var fileTypeFrame = new FrameView(frameTitle)
        {
            X = Pos.Left(createRootOrganizationUnit), Y = Pos.Bottom(createRootOrganizationUnit) + 1,
            Width = frameTitle.Length + 6, Height = 4
        };
        fileTypeFrame.Add(typeSelector);

        var userAccessControl = ElemFactory.CreateTextField("User access control: ", fileTypeFrame, 0x0200.ToString(), yOffset: 1);

        var fileSizeLimit = ElemFactory.CreateTextField("File size limit: ", userAccessControl.label, "2_000_000");
        var outputPath = ElemFactory.CreateTextField("Output path: ", fileSizeLimit.label, ".");
        var fileName = ElemFactory.CreateTextField("filename: ", outputPath.label, "output.ldif");

        var postButton = new Button("Generate")
        {
            X = Pos.Left(fileName.label),
            Y = Pos.Bottom(fileName.label) + 2,
        };

        postButton.Clicked += () =>
        {
            try
            {
                post(GetOptionSet());
                MessageBox.Query(50, 7, "Success", "File generation finished!", "OK");
            }
            catch (Exception e)
            {
                MessageBox.ErrorQuery(50, 7, "Fail!", e.Message, "OK");
            }
        };

        UserCountLabel = userCount.label;
        UserCount = userCount.field;
        GroupCountLabel = groupCount.label;
        GroupCount = groupCount.field;
        OrganizationUnitLabel = organizationUnit.label;
        OrganizationUnit = organizationUnit.field;
        UserPasswordLabel = userPassword.label;
        UserPassword = userPassword.field;
        BaseDomainLabel = baseDomain.label;
        BaseDomain = baseDomain.field;
        RootOULabel = rootOu.label;
        RootOU = rootOu.field;
        CreateAdminUser = createAdminUser;
        CreateBaseOU = createBaseOrganizationUnit;
        CreateRootOU = createRootOrganizationUnit;
        Type = typeSelector;
        FileTypeFrame = fileTypeFrame;
        UserAccessControlLabel = userAccessControl.label;
        UserAccessControl = userAccessControl.field;
        FileSizeLimitLabel = fileSizeLimit.label;
        FileSizeLimit = fileSizeLimit.field;
        OutputPathLabel = outputPath.label;
        OutputPath = outputPath.field;
        FileNameLabel = fileName.label;
        FileName = fileName.field;
        GroupMemberCountLabel = groupMemberCount.label;
        GroupMemberCount = groupMemberCount.field;

        Post = postButton;
    }

    private Options GetOptionSet() => new()
    {
        Password = UserPassword.Text.ToString(),
        BaseDomain = BaseDomain.Text.ToString(),
        CreateAdmin = CreateAdminUser.Checked,
        FileName = FileName.Text.ToString(),
        GroupCount = int.Parse(GroupCount.Text.ToString()),
        OuCount = int.Parse(OrganizationUnit.Text.ToString()),
        OutputPath = OutputPath.Text.ToString(),
        RootOu = RootOU.Text.ToString(),
        UserCount = int.Parse(UserCount.Text.ToString()),
        CreateBaseOu = CreateBaseOU.Checked,
        CreateRootOu = CreateRootOU.Checked,
        FileSizeLimit = int.Parse(FileSizeLimit.Text.ToString().Replace("_", "")),
        GroupMemberCount = int.Parse(GroupMemberCount.Text.ToString()),
        UserAccessControl = int.Parse(UserAccessControl.Text.ToString()),
        LdifType = (CbType)Type.SelectedItem
    };

    public void SetArrangement(Window parent)
    {
        parent.Add(
            UserCountLabel, UserCount,
            GroupCountLabel, GroupCount,
            OrganizationUnitLabel, OrganizationUnit,
            GroupMemberCountLabel, GroupMemberCount,
            UserPasswordLabel, UserPassword,
            BaseDomainLabel, BaseDomain,
            RootOULabel, RootOU,
            CreateAdminUser,
            CreateBaseOU,
            CreateRootOU,
            FileTypeFrame,
            UserAccessControlLabel, UserAccessControl,
            FileSizeLimitLabel, FileSizeLimit,
            OutputPathLabel, OutputPath,
            FileNameLabel, FileName,
            Post
        );
    }
}