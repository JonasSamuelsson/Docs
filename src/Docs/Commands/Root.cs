using Microsoft.Extensions.CommandLineUtils;

namespace Docs.Commands
{
   public class Root : CommandLineApplication
   {
      public Root()
      {
         Command("samples", Samples.Configure);
         Command("toc", TableOfContent.Configure);

         HelpOption("-?|-h|--help").ShowInHelpText = false;
      }
   }
}