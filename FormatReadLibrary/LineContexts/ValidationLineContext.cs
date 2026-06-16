using Validations;
namespace FormatReadLibrary.LineContexts;

public sealed class ValidationLineContext(ValidationContext context) : LineContext
{
    private readonly ValidationContext _context = context;
    public override int LineIndex => _context.Line;
    public override string LineContent => _context.LineContent;
    public override string FilePath => _context.FilePath;
}
