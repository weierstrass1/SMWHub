namespace SMWHubASMCodeLibrary.Exceptions;

public class CircularIncludeException(CodeLine line) : Exception($"Circular include detected in {line}.")
{
}
