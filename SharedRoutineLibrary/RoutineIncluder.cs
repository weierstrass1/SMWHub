using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedRoutineLibrary
{
    public sealed class RoutineIncluder(string routinesDirectory)
    {
        private readonly string _routinesDirectory = routinesDirectory;
    }
}
