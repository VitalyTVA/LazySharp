using NUnit.Framework;
using Roslyn.Compilers.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazySharp.Roslyn.Tests {
    [TestFixture]
    public class LazyRewriterTests {
        #region property
        class TestPropertyRewriter : SyntaxRewriter {
            public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node) {
                return base.VisitPropertyDeclaration(TypeRewriter.RewritePropertyType(node, "X"));
            }
        }
        [Test]
        public void RewriteProperty() {
            AssertRewritedProperty("public X<int> Prop { get; set; }", "public int Prop { get; set; }");
            AssertRewritedProperty("protected X<Type> Prop { get; set; }", "protected Type Prop { get; set; }");
            AssertRewritedProperty("X<Type> Prop { get; set; }", "Type Prop { get; set; }");
            AssertRewritedProperty("protected virtual X<Func<int, String>> Prop { get; set; }", "protected virtual Func<int, String> Prop { get; set; }");
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
            var tree = SyntaxTree.ParseText(original);
            var rewritten = rewriter.Visit(tree.GetRoot());
            Assert.AreEqual(expected, rewritten.GetText().ToString());
        }
    }
}
