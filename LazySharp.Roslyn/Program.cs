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
        public const string CODE =
@"using System;
    public class X {
        public int Add( int  a,  int b ) {
            return a + b;
        }
    }
";
        public const string RESULT =
@"using System;
    public class X {
        public Lazy<int> Add(Lazy<int> a, Lazy<int> b) {
            return new Lazy<int>(() => a.Value + b.Value);
        }
    }
";


        static void Main(string[] args) {
            SyntaxTree tree = SyntaxTree.ParseText(CODE);
            SyntaxTree pattern = SyntaxTree.ParseText(RESULT);
            SyntaxNode root = tree.GetRoot();
            SyntaxNode rootPattern = pattern.GetRoot();

            var compilation = Compilation.Create("test.dll")
                .AddReferences(new MetadataFileReference(typeof(object).Assembly.Location))
                .WithOptions(new CompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(tree);

            var rewriter = new LazyRewriter();
            var newRoot = rewriter.Visit(root);
            Debug.WriteLine(newRoot.GetText());

            var result = compilation.Emit("test.dll");

        }
        class LazyRewriter : SyntaxRewriter {
            public override SyntaxNode VisitParameter(ParameterSyntax node) {
                var trail = node.Type.GetTrailingTrivia().Single();
                var clearType = node.Type.ReplaceTrivia(trail, SyntaxTriviaList.Empty);
                var newType = Syntax.GenericName("Lazy").AddTypeArgumentListArguments(clearType).WithTrailingTrivia(trail);
                return node.WithType(newType);
            }
        }
    }
}
