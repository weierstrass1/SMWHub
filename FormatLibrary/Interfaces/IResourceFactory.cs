namespace FormatLibrary.Interfaces;

public interface IResourceFactory<T>
{
    public static abstract T Create(string name, byte[] data);
}
