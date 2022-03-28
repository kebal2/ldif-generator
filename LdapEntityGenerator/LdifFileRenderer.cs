using System.Text;

using LdapEntityGenerator.Entities;

namespace LdapEntityGenerator;

public class LdifFileRenderer
{
    public string[] RenderDiff(IList<LdapEntry> entries, int chunkSize = 2_000_000)
    {
        HashSet<string> chunks = new();

        var output = NewChunk();

        foreach (IRenderableEntry entry in entries)
        {
            var render = entry.Render();

            if (output.Length + render.Length > chunkSize)
            {
                chunks.Add(output.ToString());
                output = NewChunk();
            }

            output.AppendLine(render);
        }

        chunks.Add(output.ToString());
        return chunks.ToArray();
    }
    private static StringBuilder NewChunk()
    {
        StringBuilder output = new();

        output.AppendLine("version: 1");
        output.AppendLine();


        return output;
    }
}