namespace SharedCodeLibrary
{
    public sealed class Macro(string name, SharedCode sharedCode, int line)
    {
        public readonly string Name = name;
        public readonly SharedCode SharedCode = sharedCode;
        public readonly int Line = line;
        public override string ToString()
        {
            return $"{SharedCode} {Name} at line {Line}";
        }
    }
}
