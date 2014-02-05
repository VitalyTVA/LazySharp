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
        const string LazyTypeName = "L";
        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node) {
            return base.VisitNamespaceDeclaration(NamespaceRewriter.RewriteNamespace(node, "Prototypes", "Generated"));
        }
        public override SyntaxNode VisitParameter(ParameterSyntax node) {
            return node.WithType(WrapType(node.Type));
        }
        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node) {
            return node.WithType(WrapType(node.Type));
        }
        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node) {
            node = node.WithBody(BodyRewriter.RewriteBody(node));
            return base.VisitConstructorDeclaration(node);
        }
        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node) {
            var varieables = node.Declaration.Variables
                .Select(x => {
                    var newExpression = Syntax.ObjectCreationExpression(WrapType(node.Declaration.Type, false).WithLeadingTrivia(Syntax.Whitespace(" ")))
                        .WithArgumentList(Syntax.ArgumentList(Syntax.SeparatedList(Syntax.Argument(x.Initializer.Value))));
                    return x.WithInitializer(Syntax.EqualsValueClause(newExpression.WithLeadingTrivia(Syntax.Whitespace(" "))));
                });
            var variableDeclaration = Syntax.VariableDeclaration(WrapType(node.Declaration.Type))
                .AddVariables(varieables.ToArray());
            node = node.WithDeclaration(variableDeclaration);
            return base.VisitFieldDeclaration(node);
        }
        static TypeSyntax WrapType(TypeSyntax type, bool keepTrailingTrivia = true) {
            return TypeRewriter.WrapType(type, LazyTypeName, keepTrailingTrivia);
        }
    }
    static class BodyRewriter {
        public static BlockSyntax RewriteBody(BaseMethodDeclarationSyntax node) {
#if ROSLYN_NEW
            const SyntaxKind simpleMemberAccessExpression = SyntaxKind.SimpleMemberAccessExpression;
#else
            const SyntaxKind simpleMemberAccessExpression = SyntaxKind.MemberAccessExpression;
#endif
            var leadingTrivia = node.Body.Statements.First().GetLeadingTrivia();
            var nullChecks = node.ParameterList.Parameters
                .Select(x => x.Identifier.ValueText)
                .Select(x => {
                    var memberAccessExpression = Syntax.MemberAccessExpression(simpleMemberAccessExpression, Syntax.IdentifierName(x), Syntax.IdentifierName("NotNull"));
                    return Syntax.ExpressionStatement(Syntax.InvocationExpression(memberAccessExpression))
                        .WithLeadingTrivia(leadingTrivia)
                        .WithTrailingTrivia(Syntax.Whitespace("\r\n"));
                });
            var newBlock = node.Body.Update(node.Body.OpenBraceToken, node.Body.Statements.Insert(0, nullChecks.ToArray()), node.Body.CloseBraceToken)
                .WithLeadingTrivia(node.Body.GetLeadingTrivia())
                .WithTrailingTrivia(node.Body.GetTrailingTrivia());
            return newBlock;
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
    static class TypeRewriter {
        public static TypeSyntax WrapType(TypeSyntax type, string wrapperClassName, bool keepTrailingTrivia = true) {
            var trails = type.GetTrailingTrivia();
            var leads = type.GetLeadingTrivia();
            var clearType = type.ReplaceTrivia(leads.Concat(trails), (_, __) => SyntaxTriviaList.Empty);
            var result = Syntax.GenericName(wrapperClassName)
                .AddTypeArgumentListArguments(clearType)
                .WithLeadingTrivia(leads);
            if(keepTrailingTrivia)
                result = result.WithTrailingTrivia(trails);
            return result;
        }
    }
}
