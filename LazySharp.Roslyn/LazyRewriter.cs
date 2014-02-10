using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Syntax = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using SyntaxRewriter = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter;

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
        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) {
            node = node.WithBody(BodyRewriter.RewriteBody(node));
            return base.VisitMethodDeclaration(node);
        }
        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node) {
            node = FieldRewriter.RewriteFieldDeclaration(node, LazyTypeName);
            return base.VisitFieldDeclaration(node);
        }
        static TypeSyntax WrapType(TypeSyntax type, bool keepTrailingTrivia = true) {
            return TypeRewriter.WrapType(type, LazyTypeName, keepTrailingTrivia);
        }
    }
    static class FieldRewriter {
        public static FieldDeclarationSyntax RewriteFieldDeclaration(FieldDeclarationSyntax node, string wrapperClassName) {
            var variables = node.Declaration.Variables
                .Select(x => {
                    var argList = Syntax.ArgumentList(Syntax.SeparatedList(new[] { Syntax.Argument(x.Initializer.Value) }));
                    var newExpression = Syntax.ObjectCreationExpression(TypeRewriter.WrapType(node.Declaration.Type, wrapperClassName, false).WithLeadingTrivia(Syntax.Whitespace(" ")))
                        .WithArgumentList(argList)
                        .WithLeadingTrivia(Syntax.Whitespace(" "));
                    return x.WithInitializer(Syntax.EqualsValueClause(newExpression));
                });
            var variableDeclaration = Syntax.VariableDeclaration(TypeRewriter.WrapType(node.Declaration.Type, wrapperClassName))
                .AddVariables(variables.ToArray());
            node = node.WithDeclaration(variableDeclaration);
            return node;
        }

    }
    static class BodyRewriter {
        public static BlockSyntax RewriteBody(BaseMethodDeclarationSyntax node) {
            var leadingTrivia = (IEnumerable<SyntaxTrivia>)node.Body.Statements.Select(x => x.GetLeadingTrivia()).FirstOrDefault() ?? new [] { Syntax.Whitespace(string.Empty) };
            var nullChecks = node.ParameterList.Parameters
                .Select(x => x.Identifier.ValueText)
                .Select(x => {
                    var memberAccessExpression = Syntax.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Syntax.IdentifierName(x), Syntax.IdentifierName("NotNull"));
                    return Syntax.ExpressionStatement(Syntax.InvocationExpression(memberAccessExpression))
                        .WithLeadingTrivia(leadingTrivia)
                        .WithTrailingTrivia(Syntax.Whitespace("\r\n"));
                });
            var newBlock = node.Body.Update(node.Body.OpenBraceToken, node.Body.Statements.InsertRange(0, nullChecks.ToArray()), node.Body.CloseBraceToken)
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
            var clearType = type.ReplaceTrivia(leads.Concat(trails), (_, __) => Syntax.Whitespace(string.Empty));
            var result = Syntax.GenericName(wrapperClassName)
                .AddTypeArgumentListArguments(clearType)
                .WithLeadingTrivia(leads);
            if(keepTrailingTrivia)
                result = result.WithTrailingTrivia(trails);
            return result;
        }
    }
}
