using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Docs.Tests
{
   public class DocsElementTests
   {
      [Fact]
      public void ShouldFindSelfClosingElement()
      {
         var lines = new[]
         {
            "<!-- <docs:foo /> -->",
            "<!-- <docs:bar /> -->",
         };

         var element = DocsElement.Find(lines, "foo").Single();

         element.Name.ShouldBe("foo");
         element.Line.ShouldBe(0);
         element.Lines.ShouldBe(1);
      }

      [Fact]
      public void ShouldFindElementWithOpenCloseTags()
      {
         var lines = new[]
         {
            "<!-- <docs:foo> -->",
            "<!-- </docs:foo> -->",
            "<!-- <docs:bar> -->",
            "<!-- </docs:bar> -->"
         };

         var element = DocsElement.Find(lines, "foo").Single();

         element.Name.ShouldBe("foo");
         element.Line.ShouldBe(0);
         element.Lines.ShouldBe(2);
      }

      [Fact]
      public void ShouldThrowIfElementIsMissingClosingTag()
      {
         var lines = new[] { "<!-- <docs:foo> -->" };

         Should.Throw<Exception>(() => DocsElement.Find(lines, "foo"));
      }

      [Fact]
      public void ShouldFindElementWithAttributes()
      {
         var lines = new[]
         {
            "<!-- <docs:foo a=\"b\" c=\"d\" /> -->"
         };

         var element = DocsElement.Find(lines, "foo").Single();

         element.Attributes["a"].ShouldBe("b");
         element.Attributes["c"].ShouldBe("d");
      }

      [Fact]
      public void ShouldFindElementWithRequiredAttribute()
      {
         var lines = new[]
         {
            "<!-- <docs:foo /> -->",
            "<!-- <docs:foo a=\"b\" /> -->",
            "<!-- <docs:foo b=\"c\" /> -->"
         };

         DocsElement.Find(lines, "foo", "a").Single().Attributes["a"].ShouldBe("b");
      }
   }
}
