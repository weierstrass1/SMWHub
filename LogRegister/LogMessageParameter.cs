using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogRegister;

public sealed class LogMessageParameter(string name, object value)
{
    public string Name { get; private set; } = name;
    public object Value { get; private set; } = value;
}
