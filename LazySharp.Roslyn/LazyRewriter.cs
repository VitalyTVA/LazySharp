using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

#if ROSLYN_NEW
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Syntax = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
#else
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.CSharp;
#endif

namespace LazySharp.Roslyn {
    class LazyRewriter : SyntaxRewriter {
        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node) {
            QualifiedNameSyntax namespaceSyntax = Syntax.QualifiedName(Syntax.IdentifierName("LazySharp"), Syntax.IdentifierName("Generated")).WithTrailingTrivia(node.Name.GetTrailingTrivia());
            return Syntax.NamespaceDeclaration(namespaceSyntax.WithLeadingTrivia(node.NamespaceKeyword.TrailingTrivia), node.Externs, node.Usings, node.Members)
                .WithOpenBraceToken(node.OpenBraceToken);
        }
        //public override SyntaxNode VisitParameter(ParameterSyntax node) {
        //    var trail = node.Type.GetTrailingTrivia().Single();
        //    var clearType = node.Type.ReplaceTrivia(trail, SyntaxTriviaList.Empty);
        //    var newType = Syntax.GenericName("L").AddTypeArgumentListArguments(clearType).WithTrailingTrivia(trail);
        //    return node.WithType(newType);
        //}
        //public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node) {
        //    var syntaxSeparatedList = Syntax.SeparatedList(Syntax.Argument(node.Right));
        //    var syntaxArgumentList = Syntax.ArgumentList(syntaxSeparatedList);
        //    return Syntax.InvocationExpression(Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, node.Left, Syntax.IdentifierName("Add")), syntaxArgumentList);
        //}
    }
}
