using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if ROSLYN_NEW
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

    namespace HelloWorld
    {
        public class TestClass
        {
            public string TestField;
            public string TestProperty { get; set; }
            public string TestMethod() { return null; }
            public string TestMethod2(int k, string p) { return null; }
            public TestClass ChainTest;
        }
    }";


        static void Main(string[] args) {
            SyntaxTree tree = SyntaxTree.ParseText(CODE);
            //SyntaxNode root = tree.GetRoot();

            var compilation = Compilation.Create("test.dll")
                .AddReferences(new MetadataFileReference(typeof(object).Assembly.Location))
                .WithOptions(new CompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(tree);

            var result = compilation.Emit("test.dll");

        }
    }
}
