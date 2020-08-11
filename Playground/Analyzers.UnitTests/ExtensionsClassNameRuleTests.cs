using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace Analyzers.UnitTests
{
    public class ExtensionsClassNameRuleTests
    {
        [Test]
        public async Task Test1()
        {
            const string code = @"
public static class WrongName
{
    public static string Extension(this string str) => str;
}
";
            var compilation = Compile(code);

            var analysisResult = await compilation.GetAnalysisResultAsync(new CancellationToken()).ConfigureAwait(false);

            Print(analysisResult);

            Assert.That(analysisResult.GetAllDiagnostics().Length, Is.Zero);
        }

        private static void Print(AnalysisResult analysisResult)
        {
            foreach (var diagnostic in analysisResult.GetAllDiagnostics())
            {
                Console.WriteLine(diagnostic);
            }
        }

        private static CompilationWithAnalyzers Compile(string code) =>
            CSharpCompilation.Create("Analyzers.Temp.dll")
                             .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                             .AddSyntaxTrees(SyntaxFactory.ParseSyntaxTree(code))
                             .AddReferences(MetadataReference.CreateFromFile(typeof(string).Assembly.Location))
                             .WithAnalyzers(new List<DiagnosticAnalyzer> {new ExtensionsClassNameRule()}.ToImmutableArray());
    }
}
