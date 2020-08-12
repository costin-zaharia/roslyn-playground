module Analyzers.MethodDeclarationSyntaxExtensions

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

open SyntaxNodeExtensions

let isExtension (methodDeclaration: MethodDeclarationSyntax) =
    methodDeclaration.Modifiers.IndexOf(SyntaxKind.StaticKeyword) <> 0 &&
    methodDeclaration.ParameterList.Parameters.Count > 0 &&
    methodDeclaration.ParameterList.Parameters.First().Modifiers.IndexOf(SyntaxKind.ThisKeyword) >= 0

let getParent (methodDeclaration: MethodDeclarationSyntax) =
    methodDeclaration.Ancestors()
    |> Seq.filter isParent
    |> Seq.head

let getParentName (methodDeclaration: MethodDeclarationSyntax) = methodDeclaration |> getParent |> getName
