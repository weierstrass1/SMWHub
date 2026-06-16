using FormatLibrary.ResourceManagement.ResourceTypes;
using ResourceManagement;
namespace FormatLibrary.ResourceManagement;

public class PoseGraphicResource : DataResource
{
    public long Tiles { get; private set; }
    public PoseGraphicResource(string name, byte[] data) : base(name, new PoseGraphic(), data)
    {
        Tiles = getNumberOfTilesPerGraphic();
    }
    public PoseGraphicResource(string filepath) : base(filepath, new PoseGraphic())
    {
        Tiles = getNumberOfTilesPerGraphic();
    }
    private long getNumberOfTilesPerGraphic()
    {
        long end = Data.Length/512;
        end *= 512;
        end -= 513;

        for (; end > 0 && Data[end] != 0; end--) 
        {
        }
        long tiles = end / 1024;
        tiles *= 8;
        long remainder = end % 512;
        remainder += 63;
        remainder /= 64;
        return tiles + remainder;
    }
    public void UseCurrentFormat(IEnumerable<DynamicPose> poses)
    {
        List<byte> result = [];
        byte[] bytes;
        int bytePointer;
        int rows;
        int endV1, endV2;
        int lastRowAdder;
        int grPointer = 0;
        int grPointerBytes = 0;
        int endTiles;
        foreach (var pose in poses)
        {

            bytes = new byte[pose.Size];
            bytePointer = 0;

            rows = pose.Tiles / 8;
            rows *= 8;
            endV1 = pose.ChunksSize1 * 32;
            endV2 = pose.Size;
            lastRowAdder = endV1;

            for (int i = 0; i < pose.Tiles; i++)
            {
                if (i < rows)
                {
                    Array.Copy(Data, grPointerBytes, bytes, bytePointer, 64);
                    Array.Copy(Data, grPointerBytes + 512, bytes, bytePointer + 512, 64);
                }
                else
                {
                    Array.Copy(Data, grPointerBytes, bytes, bytePointer, endV1 - bytePointer);
                    Array.Copy(Data, grPointerBytes + 512, bytes, lastRowAdder, endV2 - lastRowAdder);
                    endTiles = (63 + endV1 - bytePointer) / 64;
                    grPointer += endTiles;
                    for (int j = 0; j < endTiles; j++)
                    {
                        grPointerBytes += 64;
                        if (grPointerBytes % 512 == 0)
                            grPointerBytes += 512;
                    }
                    break;
                }
                bytePointer += 64;
                if (bytePointer % 512 == 0)
                    bytePointer += 512;
                grPointerBytes += 64;
                if (grPointerBytes % 512 == 0)
                    grPointerBytes += 512;
                grPointer++;
            }
            result.AddRange(bytes);
        }
        Data = [.. result];
    }
}
