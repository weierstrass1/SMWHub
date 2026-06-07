namespace Configs;

public sealed class FeatureDependency
{
    public required string Parent;
    public required string[] Children;
    public static bool CheckDependency(string? feature, FeatureDependency dependency, BoolOption[] BoolOptions)
    {
        ArgumentNullException.ThrowIfNull(feature);
        if (dependency.Children.FirstOrDefault(c => c.Equals(feature)) == default)
            return true;
        return BoolOptions.First(opt => opt.Name != null && opt.Name.Equals(dependency.Parent)).Value;
    }
}
