namespace invenpro.auth.common.Decorators;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class FunctionalityAttribute : Attribute
{
    public string Type { get; }

    public FunctionalityAttribute(string type) => Type = type;
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class TypologyAttribute : Attribute
{
    public string Type { get; }

    public TypologyAttribute(string type) => Type = type;
}