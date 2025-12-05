using System.Reflection;

namespace invenpro.auth.repository;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}