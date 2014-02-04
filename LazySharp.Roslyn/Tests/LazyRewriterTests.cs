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
            var tree = SyntaxTree.ParseText(original);
            var rewritten = new TestNamespaceRewriter().Visit(tree.GetRoot());
            Assert.AreEqual(expected, rewritten.GetText().ToString());
        }
        #endregion
    }
}
