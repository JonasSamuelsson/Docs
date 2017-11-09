using System.Collections.Generic;

namespace Docs
{
   public class DocsElement
   {
      public IDictionary<string, string> Attributes { get; set; }
      public string Indentation { get; set; }
      public int Line { get; set; }
      public int Lines { get; set; }
      public string Name { get; set; }
   }
}
