using Handyman.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Docs.Utils
{
   public class DocsElementParser
   {
      private const string OpenPattern = @"^(?<indentation>\s*)\[\/\/]: # \(<docs-(?<name>[a-z]+)(\s+(?<attribute>[a-z]+=""[^""]*""))*\s*(?<selfclosing>/)?>\)\s*$";
      private const string AttributePattern = @"(?<name>[a-z]+)=""(?<value>[^""]*)""";
      private const string ClosePattern = @"^\s*\[\/\/]: # \(</docs-(?<name>[a-z]+)\s*>\)\s*$";
      private const RegexOptions RegexOptions = System.Text.RegularExpressions.RegexOptions.IgnoreCase;
      private const StringComparison StringComparison = System.StringComparison.OrdinalIgnoreCase;

      public IReadOnlyList<DocsElement> Parse(IReadOnlyList<string> lines, string elementName)
      {
         return Parse(lines, elementName, new AttributeOptions());
      }

      public IReadOnlyList<DocsElement> Parse(IReadOnlyList<string> lines, string elementName, AttributeOptions attributeOptions)
      {
         var elements = new List<DocsElement>();

         for (var i = 0; i < lines.Count; i++)
         {
            var line = lines[i];
            var match = Regex.Match(line, OpenPattern, RegexOptions);

            if (match.Success)
            {
               var name = match.Groups["name"].Value;

               if (!name.Equals(elementName))
                  continue;

               var attributes = new Dictionary<string, string>();

               foreach (var attribute in GetAttributes(match))
               {
                  var key = attribute.Key;

                  if (!attributeOptions.All.Contains(key, StringComparer.OrdinalIgnoreCase))
                     throw new AppException($"invalid attribute '{key}'.");

                  if (!attributes.TryAdd(key, attribute.Value))
                     throw new AppException($"duplicate attribute '{key}'.");
               }

               var missingAttributes = attributeOptions.Required
                  .Where(x => !attributes.ContainsKey(x))
                  .ToList();
               if (missingAttributes.Any())
                  throw new AppException($"missing attribute(s) {string.Join(", ", missingAttributes)}");

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
                  match = Regex.Match(line, ClosePattern, RegexOptions);

                  if (match.Success && match.Groups["name"].Value.Equals(element.Name, StringComparison))
                     break;
               }

               element.ElementLines = 1 + i - element.ElementLine;
               elements.Add(element);
            }
         }

         return elements;
      }

      private static IEnumerable<(string Key, string Value)> GetAttributes(Match match)
      {
         return match.Groups["attribute"].Captures.Cast<Capture>()
            .Select(x => Regex.Match(x.Value, AttributePattern, RegexOptions))
            .Select(m => (Key: m.Groups["name"].Value.ToLower(), Value: m.Groups["value"].Value));
      }

      public class AttributeOptions
      {
         public IEnumerable<string> Required { get; set; } = Enumerable.Empty<string>();
         public IEnumerable<string> Optional { get; set; } = Enumerable.Empty<string>();

         public IEnumerable<string> All => Required.Concat(Optional);
      }
   }
}