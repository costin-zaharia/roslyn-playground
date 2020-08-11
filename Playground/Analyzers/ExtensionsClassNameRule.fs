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
                                          "Extensions methods should be defined in classes with name composed from extended type name concatenated with \"Extensions\" suffix.",
                                          "{0} method should be declared in a class named {1} instead of {2}." ,
                                          "Naming",
                                          DiagnosticSeverity.Warning,
                                          true,
                                          null,
                                          null)

    override rule.SupportedDiagnostics with get() = ImmutableArray.Create(descriptor)

    override rule.Initialize (context: AnalysisContext) =
        let isExtension (methodDeclaration: MethodDeclarationSyntax) =
            methodDeclaration.Modifiers.IndexOf(SyntaxKind.StaticKeyword) <> 0 &&
            methodDeclaration.ParameterList.Parameters.Count > 0 &&
            methodDeclaration.ParameterList.Parameters.First().Modifiers.IndexOf(SyntaxKind.ThisExpression) <> 0

        let getName(node: SyntaxNode) =
            match node with
                | :? MethodDeclarationSyntax as methodDeclaration -> methodDeclaration.Identifier.ValueText
                | :? ClassDeclarationSyntax as classDeclaration -> classDeclaration.Identifier.ValueText
                | _ -> ""

        let isParent(node: SyntaxNode) =
            match node with
                | :? ClassDeclarationSyntax -> true
                | :? StructDeclarationSyntax -> true
                | _ -> false

        let getParent (methodDeclaration: MethodDeclarationSyntax) =
            methodDeclaration.Ancestors()
                |> Seq.filter isParent
                |> Seq.head

        let analyze (context: SyntaxNodeAnalysisContext) =
            match context.Node with
                |  :? MethodDeclarationSyntax as methodDeclaration when isExtension methodDeclaration ->
                    let methodName = getName methodDeclaration
                    let parentName = methodDeclaration
                                        |> getParent
                                        |> getName

                    printfn "%s" (parentName.ToString())

                    let diagnostic = Diagnostic.Create(descriptor, methodDeclaration.GetLocation(), methodName, "zzz", parentName)
                    context.ReportDiagnostic(diagnostic)
                | _ -> ()

        context.RegisterSyntaxNodeAction(analyze, SyntaxKind.MethodDeclaration)
