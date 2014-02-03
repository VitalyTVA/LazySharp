using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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
    class Program {
        static void Main(string[] args) {
            //var compilation = Compilation.Create("test.dll")
            //    .AddReferences(new MetadataFileReference(typeof(object).Assembly.Location))
            //    .WithOptions(new CompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            //    .AddSyntaxTrees(tree);

            //var rewriter = new LazyRewriter();
            //var newRoot = rewriter.Visit(root);
            //Debug.WriteLine(newRoot.GetText());

            //var result = compilation.Emit("test.dll");

        }
        //class LazyRewriter : SyntaxRewriter {
        //    public override SyntaxNode VisitParameter(ParameterSyntax node) {
        //        var trail = node.Type.GetTrailingTrivia().Single();
        //        var clearType = node.Type.ReplaceTrivia(trail, SyntaxTriviaList.Empty);
        //        var newType = Syntax.GenericName("L").AddTypeArgumentListArguments(clearType).WithTrailingTrivia(trail);
        //        return node.WithType(newType);
        //    }
        //    public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node) {
        //        var syntaxSeparatedList = Syntax.SeparatedList(Syntax.Argument(node.Right));
        //        var syntaxArgumentList = Syntax.ArgumentList(syntaxSeparatedList);
        //        return Syntax.InvocationExpression(Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, node.Left, Syntax.IdentifierName("Add")), syntaxArgumentList);
        //    }
        //}
    }
}
