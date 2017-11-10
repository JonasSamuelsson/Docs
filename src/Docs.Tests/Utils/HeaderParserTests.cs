using Docs.Utils;
using Shouldly;
using Xunit;

namespace Docs.Tests
{
   public class HeaderParserTests
   {
      [Fact]
      public void ShouldParseHeaders()
      {
         var lines = new[]
         {
            "# 1",
            "[text](#link)",
            "## 1.1",
            "",
            "# 2",
            "",
            "#"
         };

         var headers = new HeaderParser().Parse(lines);

         headers.Count.ShouldBe(3);
         headers[0].Level.ShouldBe(1);
         headers[0].Text.ShouldBe("1");
         headers[1].Level.ShouldBe(2);
         headers[1].Text.ShouldBe("1.1");
         headers[2].Level.ShouldBe(1);
         headers[2].Text.ShouldBe("2");
      }
   }
}