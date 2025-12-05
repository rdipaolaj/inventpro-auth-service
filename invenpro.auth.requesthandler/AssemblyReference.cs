using System.Reflection;

namespace invenpro.auth.requesthandler;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}