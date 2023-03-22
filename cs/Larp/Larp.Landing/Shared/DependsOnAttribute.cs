namespace Larp.Landing.Shared;

[AttributeUsage(AttributeTargets.Property)]
public class DependsOnAttribute : Attribute
{
    public string[] PropertyNames { get; }
    public string UpdateMethod { get; }

    public DependsOnAttribute(string updateMethod, params string[] propertyNames)
    {
        UpdateMethod = updateMethod;
        PropertyNames = propertyNames;
    }
}