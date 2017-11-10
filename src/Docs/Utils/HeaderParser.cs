using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Docs.Utils
{
   public class HeaderParser
   {
      public IReadOnlyList<Header> Parse(IEnumerable<string> lines)
      {
         var pattern = @"^\s*(?<level>#+)\s*(?<text>.+)\s*$";
         var headers = new List<Header>();

         foreach (var line in lines)
         {
            var match = Regex.Match(line, pattern);

            if (!match.Success)
               continue;

            headers.Add(new Header
            {
               Level = match.Groups["level"].Value.Length,
               Text = match.Groups["text"].Value
            });
         }

         return headers;
      }
   }
}