using Docs.Commands;
using System;

namespace Docs
{
   public class Program
   {
      public static int Main(string[] args)
      {
         try
         {
            return new Root().Execute(args);
         }
         catch (Exception exception)
         {
            var message = exception is AppException
               ? exception.Message
               : exception.ToString();

            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = color;

            return 1;
         }
      }
   }
}
