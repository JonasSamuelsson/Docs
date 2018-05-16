using Docs.Utils;
using System;

namespace Docs.Tests
{
   public class TestConsole : IConsole
   {
      public static IConsole Instance = new TestConsole();

      public IDisposable CreateScope(string text = null)
      {
         return new Disposable();
      }

      public void WriteInfo(string text)
      {
      }

      public void WriteError(string text)
      {
      }

      private class Disposable : IDisposable
      {
         public void Dispose()
         {
         }
      }
   }
}