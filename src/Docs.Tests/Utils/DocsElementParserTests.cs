using Docs.Utils;
using Shouldly;
using System;
using System.Linq;
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
            "[//]: # (<docs-foo />)",
            "[//]: # (<docs-bar />)",
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
            "[//]: # (<docs-foo>)",
            "[//]: # (</docs-foo>)",
            "[//]: # (<docs-bar>)",
            "[//]: # (</docs-bar>)"
         };

         var element = new DocsElementParser().Parse(lines, "foo").Single();

         element.Name.ShouldBe("foo");
         element.ElementLine.ShouldBe(0);
         element.ElementLines.ShouldBe(2);
      }

      [Fact]
      public void ShouldThrowIfElementIsMissingClosingTag()
      {
         var lines = new[] { "[//]: # (<docs-foo>)" };

         Should.Throw<Exception>(() => new DocsElementParser().Parse(lines, "foo"));
      }

      [Fact]
      public void ShouldGetElementAttributes()
      {
         var lines = new[]
         {
            "[//]: # (<docs-foo a=\"b\" c=\"d\" />)"
         };

         var element = new DocsElementParser().Parse(lines, "foo", new DocsElementParser.AttributeOptions
         {
            Required = new[] { "a" },
            Optional = new[] { "c" }
         }).Single();

         element.Attributes["a"].ShouldBe("b");
         element.Attributes["c"].ShouldBe("d");
      }

      [Fact]
      public void ShouldThrowIfRequiredAttributeIsMissing()
      {
         var lines = new[]
         {
            "[//]: # (<docs-foo />)"
         };

         Should.Throw<AppException>(() => new DocsElementParser().Parse(lines, "foo", new DocsElementParser.AttributeOptions { Required = new[] { "a" } }));
      }

      [Fact]
      public void ShouldThrowIfSameAttributeIsDefinedTwice()
      {
         var lines = new[]
         {
            "[//]: # (<docs-foo a=\"b\" a=\"c\" />)"
         };

         Should.Throw<AppException>(() => new DocsElementParser().Parse(lines, "foo", new DocsElementParser.AttributeOptions { Required = new[] { "a" } }));
      }

      [Fact]
      public void ShouldThrowIfUndefinedAttributeIsFound()
      {
         var lines = new[]
         {
            "[//]: # (<docs-foo a=\"b\" />)"
         };

         Should.Throw<AppException>(() => new DocsElementParser().Parse(lines, "foo"));
      }
   }
}
