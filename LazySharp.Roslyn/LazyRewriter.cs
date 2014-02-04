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
            return base.VisitNamespaceDeclaration(NamespaceRewriter.RewriteNamespace(node, "Prototypes", "Generated"));
        }
        //public override SyntaxNode VisitParameter(ParameterSyntax node) {
        //    var trail = node.Type.GetTrailingTrivia().Single();
        //    var clearType = node.Type.ReplaceTrivia(trail, SyntaxTriviaList.Empty);
        //    var newType = Syntax.GenericName("L").AddTypeArgumentListArguments(clearType).WithTrailingTrivia(node.Type.GetTrailingTrivia());
        //    return node.WithType(newType);
        //}
        //public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node) {
        //    var syntaxSeparatedList = Syntax.SeparatedList(Syntax.Argument(node.Right));
        //    var syntaxArgumentList = Syntax.ArgumentList(syntaxSeparatedList);
        //    return Syntax.InvocationExpression(Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, node.Left, Syntax.IdentifierName("Add")), syntaxArgumentList);
        //}
    }

    class NamespaceRewriter : SyntaxRewriter {
        public static NamespaceDeclarationSyntax RewriteNamespace(NamespaceDeclarationSyntax node, string from, string to) {
            var newName = (NameSyntax)new NamespaceRewriter(from, to).Visit(node.Name);
            return node.WithName(newName);
        }
        readonly string from;
        readonly string to;
        NamespaceRewriter(string from, string to) {
            this.from = from;
            this.to = to;
        }
        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node) {
            if((string)node.Identifier.Value == from) {
                var newNode = Syntax.Identifier(to)
                    .WithLeadingTrivia(node.Identifier.LeadingTrivia)
                    .WithTrailingTrivia(node.Identifier.TrailingTrivia);
                node = node.WithIdentifier(newNode);
            }
            return node;
        }
    }
    class TypeRewriter : SyntaxRewriter {
        public static PropertyDeclarationSyntax RewritePropertyType(PropertyDeclarationSyntax node, string wrapperClassName) {
            var trails = node.Type.GetTrailingTrivia();
            var leads = node.Type.GetLeadingTrivia();
            var clearType = node.Type
                .ReplaceTrivia(leads.Concat(trails), (_, __) => SyntaxTriviaList.Empty);
            var newType = Syntax.GenericName(wrapperClassName)
                .AddTypeArgumentListArguments(clearType)
                .WithTrailingTrivia(trails)
                .WithLeadingTrivia(leads);
            return node.WithType(newType);
        }
    }
}
