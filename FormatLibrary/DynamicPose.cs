namespace FormatLibrary;

public sealed class DynamicPose
{
    public int ChunksSize1 { get; }
    public int ChunksSize2 { get; }
    public int LastRow { get; }
    public int Tiles { get; }
    public int Size { get; }
    public DynamicPose(int chunksSize1, int chunksSize2)
    {
        ChunksSize1 = chunksSize1;
        ChunksSize2 = chunksSize2;
        Size = (chunksSize1 + chunksSize2) * 32;

        LastRow = chunksSize1 / 32;
        LastRow *= 2;
        LastRow++;
        LastRow *= 16;

        Tiles = chunksSize1 / 32;
        Tiles *= 8;
        Tiles += (Math.Max(ChunksSize1 % 32, chunksSize2) + 1) / 2;
    }
}
