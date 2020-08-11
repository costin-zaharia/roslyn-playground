namespace Analyzers

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open Microsoft.CodeAnalysis.Diagnostics
open System.Collections.Immutable

[<DiagnosticAnalyzer(Microsoft.CodeAnalysis.LanguageNames.CSharp)>]
type public ExtensionsClassNameRule() =
    inherit DiagnosticAnalyzer()

    let descriptor = DiagnosticDescriptor("Z00001",
                                          "Extensions class names should contain the extended type name concatenated with Extensions suffix",
                                          "{0} should be renamed to {1}." ,
                                          "Naming",
                                          DiagnosticSeverity.Warning,
                                          true,
                                          null,
                                          null)

    override rule.SupportedDiagnostics with get() = ImmutableArray.Create(descriptor)

    override rule.Initialize (context: AnalysisContext) =
        let getClassName (classDeclaration: ClassDeclarationSyntax) = classDeclaration.Identifier.ValueText

        let analyze (ctx: SyntaxNodeAnalysisContext) =
            match ctx.Node with
                |  z ->
                    let d = Diagnostic.Create(descriptor, z.GetLocation(), z)
                    ctx.ReportDiagnostic(d)

        context.RegisterSyntaxNodeAction(analyze, SyntaxKind.ClassDeclaration)
