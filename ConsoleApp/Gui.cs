using Terminal.Gui;

namespace App;

internal static class Gui
{
    const string title = "Ldif generator";
    
    internal static void Start(Action<Options> post)
    {
        Application.Init();
        var top = Application.Top;

        var win = CreateWindow();

        top.Add(win);

        var menu = CreateMenu(top);
        top.Add(menu);

        _ = new ConfigFormHandler(win,post);

        var statusBar = CreateStatusBar(top);

        top.Add(statusBar);

        Application.Run(top);
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
        return new Window(title)
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