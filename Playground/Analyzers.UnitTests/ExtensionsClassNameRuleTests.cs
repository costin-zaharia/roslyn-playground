using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
        public async Task TestCases()
        {
            const string code = @"
public static class WrongName
{
    public static string ExtensionForString(this string str) => str;

    public static string OtherMethod(string str) => str;

    public static void Test(){}
}

public static class StringExtensions
{
    public static string Same(this string str) => str;
}
";
            var compilation = Compile(code);

            var analysisResult = await compilation.GetAnalysisResultAsync(new CancellationToken()).ConfigureAwait(false);
            Print(analysisResult);

            Assert.That(analysisResult.GetAllDiagnostics().Length, Is.EqualTo(1));

            var diagnostic = analysisResult.GetAllDiagnostics().Single();
            Assert.That(diagnostic.Id, Is.EqualTo("Z00001"));
            Assert.That(diagnostic.Location.GetLineSpan().StartLinePosition.Line, Is.EqualTo(3));
            Assert.That(diagnostic.GetMessage(), Is.EqualTo("ExtensionForString method should be declared in a class named StringExtensions instead of WrongName."));
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
