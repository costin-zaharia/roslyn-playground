namespace Analyzers

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open Microsoft.CodeAnalysis.CSharp.Syntax
open Microsoft.CodeAnalysis.Diagnostics
open System.Collections.Immutable

[<DiagnosticAnalyzer(Microsoft.CodeAnalysis.LanguageNames.CSharp)>]
type public ExtensionsClassNameRule() =
    inherit DiagnosticAnalyzer()

    let descriptor = DiagnosticDescriptor("Z00001",
                                          "Extensions methods should be defined in classes with name composed from extended type name concatenated with \"Extensions\" suffix.",
                                          "{0} method should be declared in a class named {1}." ,
                                          "Naming",
                                          DiagnosticSeverity.Warning,
                                          true,
                                          null,
                                          null)

    override rule.SupportedDiagnostics with get() = ImmutableArray.Create(descriptor)

    override rule.Initialize (context: AnalysisContext) =
        // let getClassName (classDeclaration: ClassDeclarationSyntax) = classDeclaration.Identifier.ValueText

        let getMethodName (methodDeclaration: MethodDeclarationSyntax) = methodDeclaration.Identifier.ValueText

        let isExtension (methodDeclaration: MethodDeclarationSyntax) =
            methodDeclaration.Modifiers.IndexOf(SyntaxKind.StaticKeyword) <> 0 &&
            methodDeclaration.ParameterList.Parameters.Count > 0 &&
            methodDeclaration.ParameterList.Parameters.First().Modifiers.IndexOf(SyntaxKind.ThisExpression) <> 0

        let analyze (context: SyntaxNodeAnalysisContext) =
            match context.Node with
                |  :? MethodDeclarationSyntax as methodDeclaration when isExtension methodDeclaration ->
                    let methodName = getMethodName methodDeclaration

                    printfn "%s" methodName

                    let diagnostic = Diagnostic.Create(descriptor, methodDeclaration.GetLocation(), methodName, "zzz")
                    context.ReportDiagnostic(diagnostic)
                | _ -> ()

        context.RegisterSyntaxNodeAction(analyze, SyntaxKind.MethodDeclaration)
