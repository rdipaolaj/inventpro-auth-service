namespace invenpro.auth.common.Decorators;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class CaptureHeadersAttribute : Attribute
{
}