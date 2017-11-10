using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Docs.Utils
{
   public class LinksReader
   {
      public List<Link> GetLinks(IEnumerable<string> lines)
      {
         var linkPattern = @"\[[^]]+]\((?<uri>[^)]+)\)";

         return lines
            .SelectMany((s, i) => Regex.Matches(s, linkPattern).Cast<Match>().Select(m => new { i, m }))
            .Select(x => new Link
            {
               Index = x.m.Groups["uri"].Index,
               Line = x.i,
               Uri = x.m.Groups["uri"].Value
            })
            .ToList();
      }
   }
}