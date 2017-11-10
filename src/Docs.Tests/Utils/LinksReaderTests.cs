using Docs.Utils;
using Shouldly;
using Xunit;

namespace Docs.Tests.Utils
{
   public class LinksReaderTests
   {
      [Fact]
      public void ShouldGetLinks()
      {
         var lines = new[]
         {
            "1[2](3)4",
            "a[b](c)d[e](f)g"
         };

         var links = new LinksReader().GetLinks(lines);

         links.Count.ShouldBe(3);
         links[0].Index.ShouldBe(5);
         links[0].Line.ShouldBe(0);
         links[0].Uri.ShouldBe("3");
         links[1].Index.ShouldBe(5);
         links[1].Line.ShouldBe(1);
         links[1].Uri.ShouldBe("c");
         links[2].Index.ShouldBe(12);
         links[2].Line.ShouldBe(1);
         links[2].Uri.ShouldBe("f");
      }
   }
}