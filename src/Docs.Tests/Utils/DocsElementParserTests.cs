using System;
using System.Linq;
using Docs.Utils;
using Shouldly;
using Xunit;

namespace Docs.Tests.Utils
{
   public class DocsElementParserTests
   {
      [Fact]
      public void ShouldFindSelfClosingElement()
      {
         var lines = new[]
         {
            "<!--<docs:foo />-->",
            "<!--<docs:bar />-->",
         };

         var element = new DocsElementParser().Parse(lines, "foo").Single();

         element.Name.ShouldBe("foo");
         element.ElementLine.ShouldBe(0);
         element.ElementLines.ShouldBe(1);
      }

      [Fact]
      public void ShouldFindElementWithOpenCloseTags()
      {
         var lines = new[]
         {
            "<!--<docs:foo>-->",
            "<!--</docs:foo>-->",
            "<!--<docs:bar>-->",
            "<!--</docs:bar>-->"
         };

         var element = new DocsElementParser().Parse(lines, "foo").Single();

         element.Name.ShouldBe("foo");
         element.ElementLine.ShouldBe(0);
         element.ElementLines.ShouldBe(2);
      }

      [Fact]
      public void ShouldThrowIfElementIsMissingClosingTag()
      {
         var lines = new[] { "<!--<docs:foo>-->" };

         Should.Throw<Exception>(() => new DocsElementParser().Parse(lines, "foo"));
      }

      [Fact]
      public void ShouldGetElementAttributes()
      {
         var lines = new[]
         {
            "<!--<docs:foo a=\"b\" c=\"d\" />-->"
         };

         var element = new DocsElementParser().Parse(lines, "foo").Single();

         element.Attributes["a"].ShouldBe("b");
         element.Attributes["c"].ShouldBe("d");
      }

      [Fact]
      public void ShouldFindElementWithRequiredAttribute()
      {
         var lines = new[]
         {
            "<!--<docs:foo />-->",
            "<!--<docs:foo a=\"b\" />-->",
            "<!--<docs:foo b=\"c\" />-->"
         };

         new DocsElementParser().Parse(lines, "foo", "a").Single().Attributes["a"].ShouldBe("b");
      }
   }
}
