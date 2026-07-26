using System.ComponentModel;
using System.Reflection;

namespace SMWHubASMCodeLibrary;

public static class ScopeTypeExtension
{
    public static string GetDescription(this ScopeType value)
    {
        Type tipo = value.GetType();
        MemberInfo[] infoMiembro = tipo.GetMember(value.ToString());

        if (infoMiembro.Length <= 0)
            return value.ToString();

        var attrs = infoMiembro[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attrs.Length > 0)
            return ((DescriptionAttribute)attrs[0]).Description;
        return value.ToString();
    }
}
