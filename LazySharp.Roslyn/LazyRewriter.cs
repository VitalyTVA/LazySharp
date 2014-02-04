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
        public override SyntaxNode VisitParameter(ParameterSyntax node) {
            return node.WithType(TypeRewriter.WrapType(node.Type, "L"));
        }
        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node) {
            return node.WithType(TypeRewriter.WrapType(node.Type, "L"));
        }
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
            return node.WithType(WrapType(node.Type, wrapperClassName));
        }
        public static TypeSyntax WrapType(TypeSyntax type, string wrapperClassName) {
            var trails = type.GetTrailingTrivia();
            var leads = type.GetLeadingTrivia();
            var clearType = type.ReplaceTrivia(leads.Concat(trails), (_, __) => SyntaxTriviaList.Empty);
            return Syntax.GenericName(wrapperClassName)
                .AddTypeArgumentListArguments(clearType)
                .WithTrailingTrivia(trails)
                .WithLeadingTrivia(leads);
        }
    }
}
