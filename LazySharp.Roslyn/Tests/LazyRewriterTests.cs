using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

#if ROSLYN_NEW
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Syntax = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using SyntaxRewriter = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter;
#else
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.CSharp;
#endif

namespace LazySharp.Roslyn.Tests {
    [TestFixture]
    public class LazyRewriterTests {
        #region fields
        class TestFieldRewriter : SyntaxRewriter {
            public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node) {
                node = FieldRewriter.RewriteFieldDeclaration(node, "L");
                return base.VisitFieldDeclaration(node);
            }
        }
        [Test]
        public void RewriteField() {
            AssertRewritedField(
                @"public static readonly L<List<T>> Null = new L<List<T>>(default(List<T>));",
                @"public static readonly List<T> Null = default(List<T>);"
            );
            AssertRewritedField(
                @"private L<int> a = new L<int>(5 + 7),b = new L<int>(3 + (1 + 2));",
                @"private int a = 5 + 7, b = 3 + (1 + 2);"
            );
        }
        void AssertRewritedField(string expected, string original) {
            string context = @"using System; namespace Sample.From {{ 
    class Test {{
        {0}
    }}
}}";
            AssertRewrited(expected, original, new TestFieldRewriter(), context);
        }
        #endregion

        #region null checks
        class AddNullChecksRewriter : SyntaxRewriter {
            public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node) {
                node = node.WithBody(BodyRewriter.RewriteBody(node));
                return base.VisitConstructorDeclaration(node);
            }
        }
        [Test]
        public void AddNullChecks() {
            AssertNullChecks(
@"        public Test(T head, List<T> tail) {
            head.NotNull();
            tail.NotNull();
            Head = head;
            Tail = tail;
        }
        public Test(int x) { 
x.NotNull();
        }
",
@"        public Test(T head, List<T> tail) {
            Head = head;
            Tail = tail;
        }
        public Test(int x) { 
        }
");
        }
        void AssertNullChecks(string expected, string original) {
            string context = @"using System; namespace Sample.From {{ 
    class Test {{
        {0}
    }}
}}";
            AssertRewrited(expected, original, new AddNullChecksRewriter(), context);
        }       
        #endregion

        #region property
        class TestParameterRewriter : SyntaxRewriter {
            public override SyntaxNode VisitParameter(ParameterSyntax node) {
                return node.WithType(TypeRewriter.WrapType(node.Type, "X"));
            }
        }
        [Test]
        public void RewriteParamter() {
            AssertRewritedParameters("public int Method(X<int> a, X<List<int>> b) { }", "public int Method(int a, List<int> b) { }");
        }
        void AssertRewritedParameters(string expected, string original) {
            string context = @"using System; namespace Sample.From {{ 
    class Test {{
        {0}
    }}
}}";
            AssertRewrited(expected, original, new TestParameterRewriter(), context);
        }
        #endregion

        #region property
        class TestPropertyRewriter : SyntaxRewriter {
            public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node) {
                return node.WithType(TypeRewriter.WrapType(node.Type, "X"));
            }
        }
        [Test]
        public void RewriteProperty() {
            AssertRewritedProperty("public X<int> Prop { get; set; }", "public int Prop { get; set; }");
            AssertRewritedProperty("protected X<Type> Prop { get; set; }", "protected Type Prop { get; set; }");
            AssertRewritedProperty("X<Type> Prop { get; set; }", "Type Prop { get; set; }");
            AssertRewritedProperty("protected virtual X<Func<int, String>> Prop { get; set; }", "protected virtual Func<int, String> Prop { get; set; }");

            AssertRewritedProperty(@"   
  /* e */ X<Type> //todo 
  
    Prop     { get; set; }", @"   
  /* e */ Type //todo 
  
    Prop     { get; set; }");
        }
        void AssertRewritedProperty(string expected, string original) {
            string context = @"using System; namespace Sample.From {{ 
    class Test {{
        {0}
    }}
}}";
            AssertRewrited(expected, original, new TestPropertyRewriter(), context);
        }
        #endregion

        #region namespace
        class TestNamespaceRewriter : SyntaxRewriter {
            public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node) {
                return base.VisitNamespaceDeclaration(NamespaceRewriter.RewriteNamespace(node, "From", "To"));
            }
        }
        [Test]
        public void RewriteNamespace1() {
            string text = @"using System; namespace Sample.From { }";
            string expected = @"using System; namespace Sample.To { }";
            AssertRewritedNamespace(expected, text);
        }
        [Test]
        public void RewriteNamespace2() {
            string text = @"using System; 

  namespace

   Sample .   
 From   
   
   { 
    class x {}
 }

";
            string expected = @"using System; 

  namespace

   Sample .   
 To   
   
   { 
    class x {}
 }

";
            AssertRewritedNamespace(expected, text);
        }
        void AssertRewritedNamespace(string expected, string original) {
            AssertRewrited(expected, original, new TestNamespaceRewriter());
        }
        #endregion
        void AssertRewrited(string expected, string original, SyntaxRewriter rewriter, string context = null) {
            if(context != null) {
                expected = string.Format(context, expected);
                original = string.Format(context, original);
            }
            var tree = CSharpSyntaxTree.ParseText(original);
            var rewritten = rewriter.Visit(tree.GetRoot());
            Assert.AreEqual(expected, rewritten.GetText().ToString());
        }
    }
}
