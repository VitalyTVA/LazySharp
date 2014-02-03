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
    class Program {
        const string path = @"..\..\";
        static void Main(string[] args) {
            const string prototypesDllName = "LazySharp.Prototypes.dll";
            var compilation = Compilation.Create(prototypesDllName)
                .AddReferences(
                    new MetadataFileReference(typeof(object).Assembly.Location),
                    new MetadataFileReference(typeof(Enumerable).Assembly.Location)
                 ).WithOptions(new CompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(
                    GetTree(@"Utils\Argument"), 
                    GetTree(@"L"),
                    GetTree(@"Prototypes\List.Generic")
                );
            EmitAndLog(compilation, prototypesDllName);
            return;
        }
        static void EmitAndLog(Compilation compilation, string dllName) {
            var result = compilation.Emit(dllName);
            if(result.Success)
                Console.WriteLine("Emit success: " + dllName);
            else {
                Console.WriteLine("EMIT ERROR: " + dllName);
                foreach(var item in result.Diagnostics) {
                    Console.WriteLine(item.ToString());
                    
                }
            }
        }
        static SyntaxTree GetTree(string fileName) {
            return SyntaxTree.ParseFile(GetFilePath(fileName));
        }
        static string GetFilePath(string fileName) {
            return Path.Combine(path, fileName + ".cs");
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
