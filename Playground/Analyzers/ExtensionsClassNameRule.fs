namespace Analyzers

open Analyzers
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open Microsoft.CodeAnalysis.Diagnostics
open System.Collections.Immutable
open SyntaxNodeExtensions
open MethodDeclarationSyntaxExtensions

[<DiagnosticAnalyzer(Microsoft.CodeAnalysis.LanguageNames.CSharp)>]
type public ExtensionsClassNameRule() =
    inherit DiagnosticAnalyzer()

    let descriptor = DiagnosticDescriptor("Z00001",
                                          "Extensions methods should be defined in classes with name composed from extended type name concatenated with \"Extensions\" suffix.",
                                          "{0} method should be declared in a class named {1} instead of {2}." ,
                                          "Naming",
                                          DiagnosticSeverity.Warning,
                                          true,
                                          null,
                                          null)

    override rule.SupportedDiagnostics with get() = ImmutableArray.Create(descriptor)

    override rule.Initialize (context: AnalysisContext) =
        let analyze (context: SyntaxNodeAnalysisContext) =
            let getExpectedName (methodDeclaration: MethodDeclarationSyntax) =
                methodDeclaration.ParameterList.Parameters.First()
                |> context.SemanticModel.GetDeclaredSymbol
                |> fun s -> s.Type.Name + "Extensions"

            match context.Node with
            |  :? MethodDeclarationSyntax as methodDeclaration when isExtension methodDeclaration ->
                let methodName = getName methodDeclaration
                let parentName = methodDeclaration |> getParent |> getName
                let expectedTypeName = getExpectedName methodDeclaration
                let diagnostic =
                    if parentName = expectedTypeName then None
                    else Some(Diagnostic.Create(descriptor, methodDeclaration.GetLocation(), methodName, expectedTypeName, parentName))

                match diagnostic with
                | Some diag -> context.ReportDiagnostic(diag)
                | None -> ()
            | _ -> ()

        context.RegisterSyntaxNodeAction(analyze, SyntaxKind.MethodDeclaration)
