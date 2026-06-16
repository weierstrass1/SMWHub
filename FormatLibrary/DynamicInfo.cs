using FormatLibrary.Interfaces;
using FormatLibrary.ResourceManagement;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
namespace FormatLibrary;

public sealed partial class DynamicInfo
{
    public PoseGraphicResource? PoseGraphics { get; private set; }
    public IReadOnlyList<PaletteResource> Palettes => _palettes.AsReadOnly();
    public IReadOnlyList<GeneralResource> GeneralResources => _generalResources.AsReadOnly();
    public int PoseLength => _dynamicPoses.Count;
    public int PaletteLength => _palettes.Count;
    public int ResourcesLength => _generalResources.Count;
    private List<PaletteResource> _palettes = [];
    private List<GeneralResource> _generalResources = [];
    private List<DynamicPose> _dynamicPoses = [];
    public void SetPalettes(IEnumerable<(string, byte[])> palettes) =>
        setResource(_palettes, palettes);
    public void SetGeneralResources(IEnumerable<(string, byte[])> res) =>
        setResource(_generalResources, res);
    [MemberNotNull(nameof(PoseGraphics))]
    public void SetPoseGraphics(string name, IEnumerable<byte[]> graphics)
    {
        List<byte> data = [];
        foreach (var item in graphics)
            data.AddRange(item);
        PoseGraphics = new PoseGraphicResource(name, [.. data]);
    }
    public void SetDynamicPoses(IEnumerable<(int, int)> posechunksSizes)
    {
        _dynamicPoses.Clear();
        _dynamicPoses.AddRange(posechunksSizes.Select(pcs => new DynamicPose(pcs.Item1, pcs.Item2)));
    }
    public void SetDynamicPoses(IDictionary<int, string> numberOf16x16TilesPerPose)
    {
        _dynamicPoses.Clear();
        if (PoseGraphics == null)
            return;
        Regex modifier = tilesRegex();
        Match m;
        int posetiles;
        foreach (var kvp in numberOf16x16TilesPerPose.OrderBy(pair => pair.Key))
        {
            m = modifier.Match(kvp.Value);
            _dynamicPoses.Add(getDynamicPoseFrom16x16Tiles(m, out posetiles));
        }
        PoseGraphics.UseCurrentFormat(_dynamicPoses);
    }
    private static DynamicPose getDynamicPoseFrom16x16Tiles(Match m, out int posetiles)
    {
        posetiles = int.Parse(m.Groups["tiles"].Value);
        int v1 = posetiles / 8;
        v1 *= 32;
        int remainder = posetiles % 8;
        int v2 = remainder * 2;
        v1 += v2;
        if (!m.Groups["modifier"].Success)
            return new(v1, v2);

        string mod = m.Groups["modifier"].Value;
        posetiles++;
        v1 += mod switch
        {
            "q3" => 2,
            "h" => 2,
            "q" => 1,
            _ => 0
        };
        v2 += mod switch
        {
            "q3" => 1,
            _ => 0
        };

        return new(v1, v2);
    }

    private static void setResource<T>(List<T> targetList, IEnumerable<(string, byte[])> res) where T : IResourceFactory<T>
    {
        targetList.Clear();
        targetList.AddRange(res.Select(r => T.Create(r.Item1, r.Item2)));
    }
    [GeneratedRegex("(?<tiles>\\d+)(?<modifier>(q3|h|q))?")]
    private static partial Regex tilesRegex();
}
