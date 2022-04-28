using LdapEntityGenerator.Entities;

using Terminal.Gui;

namespace App;

public class ConfigFormHandler
{
    private readonly ConfigForm form;

    public ConfigFormHandler(View parent, Action<Options> post)
    {
        form = FormFactory.Get<ConfigForm>(parent);
        form.SetPost(() => { post(GetOptionSet()); });
    }

    private Options GetOptionSet()
    {
        return new()
        {
            Password = form.UserPassword.Text.ToString(),
            BaseDomain = form.BaseDomain.Text.ToString(),
            CreateAdmin = form.CreateAdminUser.Checked,
            FileName = form.FileName.Text.ToString(),
            GroupCount = int.Parse(form.GroupCount.Text.ToString()),
            OuCount = int.Parse(form.OrganizationUnitCount.Text.ToString()),
            OutputPath = form.OutputPath.Text.ToString(),
            RootOu = form.RootOU.Text.ToString(),
            UserCount = int.Parse(form.UserCount.Text.ToString()),
            CreateBaseOu = form.CreateBaseOU.Checked,
            CreateRootOu = form.CreateRootOU.Checked,
            FileSizeLimit = int.Parse(form.FileSizeLimit.Text.ToString().Replace("_", "")),
            GroupMemberCount = int.Parse(form.GroupMemberCount.Text.ToString()),
            UserAccessControl = int.Parse(form.UserAccessControl.Text.ToString()),
            LdifType = (CbType)form.Type.SelectedItem
        };
    }
}