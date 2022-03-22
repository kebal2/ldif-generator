using System.Text;

using LdapEntityGenerator.Entities;

namespace LdapEntityGenerator;

public class LdifFileRenderer
{
    public string[] RenderDiff(IList<LdapEntry> entries, int chunkSize = 1000)
    {
        HashSet<string> chunks = new HashSet<string>();
        
        var output = new StringBuilder();

        for(int offset = 0; offset < entries.Count; offset += chunkSize)
        {
            output.AppendLine("version: 1");
            output.AppendLine();

            var e = entries.Skip(offset).Take(chunkSize);
            
            foreach (IRenderableEntry entry in e)
            {
                output.AppendLine(entry.Render());
            }

            chunks.Add(output.ToString());
            output.Clear();
        }

        return chunks.ToArray();
    }
}