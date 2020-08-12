module Analyzers.SyntaxNodeExtensions

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp.Syntax

let getName(node: SyntaxNode) =
    match node with
    | :? MethodDeclarationSyntax as methodDeclaration -> methodDeclaration.Identifier.ValueText
    | :? ClassDeclarationSyntax as classDeclaration -> classDeclaration.Identifier.ValueText
    | :? TypeSyntax as typeSyntax -> typeSyntax.GetText().ToString()
    | _ -> ""

let isParent(node: SyntaxNode) =
    match node with
    | :? ClassDeclarationSyntax | :? StructDeclarationSyntax-> true
    | _ -> false
