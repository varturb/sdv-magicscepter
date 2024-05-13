using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MagicScepter.Helpers
{
  public static class FileHelper
  {
    public static string ReadFileData<TSource>(string embeddedFileName) where TSource : class
    {
      var assembly = typeof(TSource).GetTypeInfo().Assembly;
      var resourceName = assembly.GetManifestResourceNames().First(s => s.EndsWith(embeddedFileName, StringComparison.CurrentCultureIgnoreCase));

      using (var stream = assembly.GetManifestResourceStream(resourceName))
      {
        if (stream == null)
        {
          throw new InvalidOperationException("Could not load manifest resource stream.");
        }
        using (var reader = new StreamReader(stream))
        {
          return reader.ReadToEnd();
        }
      }
    }
  }
}