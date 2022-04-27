using NStack;

using Terminal.Gui;

namespace App;

public static class Gui
{
    internal static void Start<T>(Action<T> parsed)
    {
        Application.Init();
        var top = Application.Top;

        var win = CreateWindow();

        top.Add(win);

        var menu = CreateMenu(top);
        top.Add(menu);

        var form = CreateForm(parsed);
        
        // Add some controls, 
        win.Add(form);

        var statusBar = CreateStatusBar(top);
        
        top.Add(statusBar);
        
        Application.Run(top);
    }

    const int DefaultWidth = 40;

    private static View[] CreateForm<T>(Action<T> post)
    {
        int startPositionY = 1;
        int startPositionX = 3;
        
        var userCount = CreateTextField("User count: ", startPositionX, startPositionY, "1000");
        var groupCount = CreateTextField("Group count: ", userCount.label, "10");
        var organizationUnit = CreateTextField("Organization unit count: ", groupCount.label, "10");
        var userPassword = CreateTextField("User password: ", organizationUnit.label, "AnExamplePassword1");
        var baseDomain = CreateTextField("Base domain name: ", userPassword.label, "example.com");
        var rootOU  = CreateTextField("Root organization unit: ", baseDomain.label, "ADC");


        var shouldCreateAdminUser = new CheckBox("Should create administrative user") { X = Pos.Left(rootOU.label), Y = Pos.Bottom(rootOU.label) };
        var shouldCreateBaseOrganizationUnit = new CheckBox("Should create base organization unit") { X = Pos.Left(shouldCreateAdminUser), Y = Pos.Bottom(shouldCreateAdminUser) };
        var shouldCreateBRootOrganizationUnit = new CheckBox("Should create root organization unit") { X = Pos.Left(shouldCreateBaseOrganizationUnit), Y = Pos.Bottom(shouldCreateBaseOrganizationUnit) };

        var typeSelector = new RadioGroup(new ustring[] { "_GENERIC", "_MAD" }, 0)
        {
            X = Pos.Left(shouldCreateBRootOrganizationUnit) + 1, Y = Pos.Bottom(shouldCreateBRootOrganizationUnit) + 1 
        };
        
        return new View[]
        {
            userCount.label, userCount.field,
            groupCount.label, groupCount.field,
            organizationUnit.label, organizationUnit.field,
            userPassword.label, userPassword.field,
            baseDomain.label, baseDomain.field,
            rootOU.label, rootOU.field,
// The ones laid out like an australopithecus, with Absolute positions:
             shouldCreateAdminUser,
             shouldCreateBaseOrganizationUnit,
             shouldCreateBRootOrganizationUnit,
             typeSelector,
            // new CheckBox(startPositionX, startPositionY + 8, "Should create base organization unit"),
            // new CheckBox(startPositionX, startPositionY + 9, "Should create root organization unit"),
            // new RadioGroup(startPositionX, startPositionY + 10, new ustring[] { "_GENERIC", "_MAD" }, 0),
            new Button(startPositionX, 21, "Generate"),
            // new Button(10, 14, "Cancel"),
            // new Label(startPositionX, 22, "Press F9 or ESC plus 9 to activate the menubar")
        };
    }

    private static (Label label, TextField field) CreateTextField(string labelText, View relativeTo, string defaultValue = "")
    {
        var yPos = Pos.Top(relativeTo) + 1;
        var label = new Label(labelText) { X = Pos.Left(relativeTo), Y = yPos };
        var field = new TextField(defaultValue)
        {
            X = Pos.Right(label),
            Y = yPos,
            Width = DefaultWidth
        };

        return (label, field);
    }

    private static (Label label, TextField field) CreateTextField(string labelText, Pos x, Pos y, string defaultValue = "")
    {
        var label = new Label(labelText) { X = x, Y = y };
        var field = new TextField(defaultValue)
        {
            X = Pos.Right(label),
            Y = Pos.Top(label),
            Width = DefaultWidth
        };

        return (label, field);
    }

    static bool Quit()
    {
        var n = MessageBox.Query(50, 7, "Quit generator", "Are you sure you want to quit?", "Yes", "No");
        return n == 0;
    }

    private static MenuBar CreateMenu(Toplevel top)
    {
        return new MenuBar(new MenuBarItem[]
        {
            new MenuBarItem("_File", new MenuItem[]
            {
                new MenuItem("_Quit", "", () =>
                {
                    if (Quit()) top.Running = false;
                }, shortcut: Key.CtrlMask | Key.Q)
            })
        });
    }

    private static StatusBar CreateStatusBar(Toplevel top)
    {
        return new StatusBar(new StatusItem[]
        {
            new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () =>
            {
                if (Quit())
                {
                    top.Running = false;
                }
            })
        });
    }

    private static Window CreateWindow()
    {
        return new Window("Ldif generator")
        {
            X = 0,
            Y = 1, // Leave one row for the toplevel menu

            // By using Dim.Fill(), it will automatically resize without manual intervention
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            LayoutStyle = LayoutStyle.Computed
        };
    }
}