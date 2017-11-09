using System;

namespace Docs
{
   public class AppException : Exception
   {
      public AppException(string message)
         : base(message)
      {
      }
   }
}