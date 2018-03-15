using Shouldly;
using System.IO;
using Xunit;

namespace Docs.Tests
{
   public class TestFileSystemTests
   {
      [Fact]
      public void ShouldReadFile()
      {
         var fileSystem = new TestFileSystem
         {
            Files =
            {
               {@"x:\file.txt", new []{"one", "two"} }
            }
         };

         fileSystem.ReadFile(@"x:\file.txt").ShouldBe(new[] { "one", "two" });
      }

      [Fact]
      public void ShouldWriteFile()
      {
         var fileSystem = new TestFileSystem();

         fileSystem.WriteFile(@"x:\file.txt", new[] { "one", "two" });

         fileSystem.Files[@"x:\file.txt"].ShouldBe(new[] { "one", "two" });
      }

      [Fact]
      public void FileExists()
      {
         var fileSystem = new TestFileSystem();

         fileSystem.FileExists(@"x:\file.txt").ShouldBeFalse();

         fileSystem.WriteFile(@"x:\file.txt", new[] { "" });

         fileSystem.FileExists(@"x:\file.txt").ShouldBeTrue();
      }

      [Fact]
      public void DirectoryExists()
      {
         var fileSystem = new TestFileSystem();

         fileSystem.DirectoryExists(@"x:\dir").ShouldBeFalse();

         fileSystem.WriteFile(@"x:\directory\file.txt", new[] { "" });

         fileSystem.DirectoryExists(@"x:\dir").ShouldBeFalse();

         fileSystem.WriteFile(@"x:\dir\file.txt", new[] { "" });

         fileSystem.DirectoryExists(@"x:\dir").ShouldBeTrue();
      }

      [Fact]
      public void GetFiles()
      {
         var fileSystem = new TestFileSystem
         {
            Files =
            {
               [@"x:\1\one.txt"] = new[] {""},
               [@"x:\1\two.xml"] = new[] {""},
               [@"x:\1\dir\three.txt"] = new[] {""},
               [@"x:\2\four.txt"] = new[] {""}
            }
         };

         fileSystem
            .GetFiles(@"x:\1", "*.txt", SearchOption.AllDirectories)
            .ShouldBe(new[] { @"x:\1\one.txt", @"x:\1\dir\three.txt" });

         fileSystem
            .GetFiles(@"x:\1", "*.txt", SearchOption.TopDirectoryOnly)
            .ShouldBe(new[] { @"x:\1\one.txt" });
      }
   }
}