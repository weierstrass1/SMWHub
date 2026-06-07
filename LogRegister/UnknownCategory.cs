using LogRegister.Interfaces;

namespace LogRegister;

public class UnknownCategory : ILogCategory
{
    public const string KEY = "UNKNOWN LOG TYPE";
}
