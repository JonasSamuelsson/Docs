using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Docs
{
   public class DocsElement
   {
      public string Name { get; private set; }
      public int Line { get; private set; }
      public int Lines { get; private set; }
      public IReadOnlyDictionary<string, string> Attributes { get; private set; }

      public static IReadOnlyList<DocsElement> Find(IReadOnlyList<string> lines, string elementName, string attributeName = null)
      {
         var prefixPattern = @"^\s*<!--\s*";
         var suffixPattern = @"\s*-->\s*$";
         var elementNamePattern = "(?<elementName>[a-z]+)";
         var attributesPattern = "(\\s+(?<attribute>[a-z]+=\".*\"))*";
         var openPattern = $@"{prefixPattern}<docs:{elementNamePattern}\s*(?<selfclosing>/)?{suffixPattern}";
         var closePattern = $@"{prefixPattern}</docs:{elementNamePattern}\s*>{suffixPattern}";
         var options = RegexOptions.IgnoreCase;

         var elements = new List<DocsElement>();
         var element = new DocsElement();

         for (var i = 0; i < lines.Count; i++)
         {
            var line = lines[i];
            var match = Regex.Match(line, openPattern, options);
            if (match.Success)
            {
               //if (match.Captures)
            }
         }
         foreach (var line in lines)
         {
            var match = Regex.Match(line, openPattern, options);
            if (match.Success)
            {
               //if (mat)
            }

            //elements.Add(new DocsElement());
         }

         return elements;
      }
   }
}
