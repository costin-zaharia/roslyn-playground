using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Runner
{
    internal class AssemblyLoader : IAnalyzerAssemblyLoader
    {
        public static AssemblyLoader Instance = new AssemblyLoader();

        public void AddDependencyLocation(string fullPath) => Assembly.LoadFrom(fullPath);

        public Assembly LoadFromPath(string fullPath) => Assembly.LoadFrom(fullPath);
    }
}