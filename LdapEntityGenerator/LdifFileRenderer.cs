using System.Text;

using LdapEntityGenerator.Entities;

namespace LdapEntityGenerator;

public class LdifFileRenderer
{
    public string RenderDiff(IList<LdapEntry> entries)
    {
        var output = new StringBuilder();

        output.AppendLine("version: 1");
        output.AppendLine();

        foreach (IRenderableEntry entry in entries)
        {
            output.AppendLine(entry.Render());
        }

        return output.ToString();
    }
}