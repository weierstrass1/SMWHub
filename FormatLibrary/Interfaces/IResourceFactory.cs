using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatLibrary.Interfaces;

public interface IResourceFactory<T>
{
    public static abstract T Create(string name, byte[] data);
}
