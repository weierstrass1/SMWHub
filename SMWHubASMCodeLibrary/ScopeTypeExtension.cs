using System.ComponentModel;
using System.Reflection;

namespace SMWHubASMCodeLibrary;

public static class ScopeTypeExtension
{
    private static Dictionary<string, ScopeType> _types = [];
    public static ScopeType? GetFromDescription(string description)
    {
        if(_types.Count == 0)
            foreach (var type in Enum.GetValues<ScopeType>())
            {
                _types.Add(type.GetDescription().ToLower(), type);
            }
        return _types.TryGetValue(description
            .ToLower()
            .Replace(":",""), out ScopeType val) ? 
                val : 
                null;
    }
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
