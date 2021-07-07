using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analyzers;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;

namespace Runner
{
    public static class Program
    {
        private static async Task Main(string[] args)
        {
            var solutionPath = args[0];

            RegisterMsBuild();
            var workspace = await LoadSolution(solutionPath);
            var analyzer = new AnalyzerFileReference(typeof(ExtensionsClassNameRule).Assembly.Location, AssemblyLoader.Instance);
            var solution = workspace.CurrentSolution.AddAnalyzerReference(analyzer);

            var dependencyGraph = solution.GetProjectDependencyGraph();

            foreach (var projectId in dependencyGraph.GetTopologicallySortedProjects())
            {
                var project = solution.GetProject(projectId);

                var compilation = await project.GetCompilationAsync().ConfigureAwait(false);
                var diagnostics = compilation.GetDiagnostics();
            }

            Console.WriteLine($"Solution loaded: {solution.FilePath}, Projects: {workspace.CurrentSolution.Projects.Count()}");
        }

        private static void RegisterMsBuild()
        {
            var msBuildInstance = MSBuildLocator.QueryVisualStudioInstances()
                                                .OrderByDescending(x => x.Version)
                                                .First();

            MSBuildLocator.RegisterInstance(msBuildInstance);

            Console.WriteLine($"MsBuild {msBuildInstance.Version} registered.");
        }

        private static async Task<MSBuildWorkspace> LoadSolution(string solutionPath)
        {
            var workspace = MSBuildWorkspace.Create();

            await workspace.OpenSolutionAsync(solutionPath).ConfigureAwait(false);

            foreach (var workspaceDiagnostic in GetFailures(workspace))
            {
                Console.WriteLine(workspaceDiagnostic.Message);
            }

            return workspace;
        }

        private static IEnumerable<WorkspaceDiagnostic> GetFailures(MSBuildWorkspace workspace) =>
            workspace.Diagnostics
                     .Where(workspaceDiagnostic => workspaceDiagnostic.Kind == WorkspaceDiagnosticKind.Failure);
    }
}