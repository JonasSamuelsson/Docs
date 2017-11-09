using System.Collections.Generic;
using System.Linq;

namespace Docs
{
   public class DocsElementWriter
   {
      public IEnumerable<string> Write(DocsElement element, IEnumerable<string> content)
      {
         var result = new List<string>();

         var open = $"{element.Indentation}<!--<docs:{element.Name}";

         if (element.Attributes.Any())
         {
            foreach (var attribute in element.Attributes)
            {
               open += $" {attribute.Key}=\"{attribute.Value}\"";
            }
         }

         open += ">-->";
         result.Add(open);

         result.AddRange(content.Select(s => element.Indentation + s));

         result.Add($"{element.Indentation}<!--</docs:{element.Name}>-->");

         return result;
      }
   }
}