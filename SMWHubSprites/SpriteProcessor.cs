using FormatLibrary.Entries;
using System.Text;

namespace SMWHubSprites;

public static class SpriteProcessor
{
    public static string GenerateExtraByteTable(IEnumerable<NormalSpriteConfigEntry> sprites)
    {
        Dictionary<int, NormalSpriteConfigEntry> sps = sprites.ToDictionary(s => s.ID, s => s);
        StringBuilder exByteClear = new("ExtraBytesWithClearExtraBit:\n");
        StringBuilder exByteSet = new("ExtraBytesWithSetExtraBit:\n");
        int i = 0;
        IEnumerable<(string, string)> exbytes;
        for (; i < 0x100; i += 16)
        {
            exbytes = Enumerable.Range(i, 16)
                .Select(s => sps.TryGetValue(s, out NormalSpriteConfigEntry? sp) ?
                    ($"${3 + sp.ExtraBytesWithClearExtraBit:X2}", $"${3 + sp.ExtraBytesWithSetExtraBit:X2}") :
                    ("$03", "$03"));
            exByteClear.AppendLine($"db {string.Join(',', [.. exbytes.Select(eb => eb.Item1)])}");
            exByteSet.AppendLine($"db {string.Join(',', [.. exbytes.Select(eb => eb.Item2)])}");
        }
        return $"{exByteClear}\n{exByteSet}";
    }
}
