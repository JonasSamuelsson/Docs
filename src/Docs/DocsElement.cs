using System.Collections.Generic;

namespace Docs
{
   public class DocsElement
   {
      public IDictionary<string, string> Attributes { get; set; }
      public string Indentation { get; set; }
      public int ElementLine { get; set; }
      public int ElementLines { get; set; }
      public string Name { get; set; }

      public int ContentLine => ElementLines > 2 ? ElementLine + 1 : ElementLine;
      public int ContentLines => ElementLines > 2 ? ElementLines - 2 : 0;
   }
}
