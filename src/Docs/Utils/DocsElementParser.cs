using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Docs.Utils
{
   public class DocsElementParser
   {
      public IReadOnlyList<DocsElement> Parse(IReadOnlyList<string> lines, string elementName)
      {
         return Parse(lines, elementName, null);
      }

      public IReadOnlyList<DocsElement> Parse(IReadOnlyList<string> lines, string elementName, string attributeName)
      {
         var openPattern = @"^(?<indentation>\s*)<!--\s*<docs:(?<name>[a-z]+)(\s+(?<attribute>[a-z]+=""[^""]*""))*\s*(?<selfclosing>/)?>\s*-->\s*$";
         var attributePattern = @"(?<name>[a-z]+)=""(?<value>[^""]*)""";
         var closePattern = @"^\s*<!--\s*</docs:(?<name>[a-z]+)\s*>\s*-->\s*$";
         var options = RegexOptions.IgnoreCase;
         var comparison = StringComparison.OrdinalIgnoreCase;

         var elements = new List<DocsElement>();

         for (var i = 0; i < lines.Count; i++)
         {
            var line = lines[i];
            var match = Regex.Match(line, openPattern, options);

            if (match.Success)
            {
               var name = match.Groups["name"].Value;

               if (!name.Equals(elementName))
                  continue;

               var attributes = match.Groups["attribute"].Captures.Cast<Capture>()
                  .Select(x => Regex.Match(x.Value, attributePattern, options))
                  .ToDictionary(m => m.Groups["name"].Value, m => m.Groups["value"].Value);

               if (attributeName != null && !attributes.ContainsKey(attributeName))
                  continue;

               var element = new DocsElement
               {
                  Attributes = attributes,
                  Indentation = match.Groups["indentation"].Value,
                  ElementLine = i,
                  Name = name
               };

               if (match.Groups["selfclosing"].Success)
               {
                  element.ElementLines = 1;
                  elements.Add(element);
                  continue;
               }

               while (true)
               {
                  if (++i == lines.Count)
                     throw new AppException($"element {element.Name}@{element.ElementLine} : closing tag not found");

                  line = lines[i];
                  match = Regex.Match(line, closePattern, options);

                  if (match.Success && match.Groups["name"].Value.Equals(element.Name, comparison))
                     break;
               }

               element.ElementLines = 1 + i - element.ElementLine;
               elements.Add(element);
            }
         }

         return elements;
      }
   }
}