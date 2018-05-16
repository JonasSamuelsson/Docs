using System;
using System.Threading;
using System.Threading.Tasks;

namespace Docs.Utils
{
   public interface IConsole
   {
      IDisposable CreateScope(string text = null);
      void WriteInfo(string text);
      void WriteError(string text);
   }

   public class Console : IConsole
   {
      private static readonly string Characters = @"-\|/";
      private int _index = 0;

      public IDisposable CreateScope(string text = null)
      {
         if (!string.IsNullOrEmpty(text))
         {
            System.Console.WriteLine(text);
         }

         var cts = new CancellationTokenSource();

         Task.Factory.StartNew(async () =>
         {
            while (!cts.Token.IsCancellationRequested)
            {
               lock (this)
               {
                  WriteProgressIndicator();
               }

               await Task.Delay(50);
            }
         }, TaskCreationOptions.LongRunning);

         return new Disposable(() => cts.Cancel());
      }

      private void WriteProgressIndicator()
      {
         System.Console.CursorLeft = 0;

         var character = Characters[_index];
         System.Console.Write(character);
         _index = _index++ % 4;
      }

      public void WriteInfo(string text)
      {
         Write(text, System.Console.ForegroundColor);
      }

      public void WriteError(string text)
      {
         Write(text, ConsoleColor.Red);
      }

      public void Write(string text, ConsoleColor color)
      {
         lock (this)
         {
            var backup = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.CursorLeft = 0;
            System.Console.WriteLine(text);
            System.Console.ForegroundColor = backup;
         }
      }

      class Disposable : IDisposable
      {
         private readonly Action _action;

         public Disposable(Action action)
         {
            _action = action;
         }

         public void Dispose()
         {
            _action();
         }
      }
   }
}