using System;
using System.IO;
using System.Linq;
using System.Reflection;
using MagicScepter.Constants;
using Microsoft.Xna.Framework.Graphics;

namespace MagicScepter.Helpers
{
  public static class FileHelper
  {
    private static Texture2D texture = ModUtility.Helper.ModContent.Load<Texture2D>(ModConstants.SpritesheetTexturePath);

    public static void ReloadTexture()
    {
      texture = ModUtility.Helper.ModContent.Load<Texture2D>(ModConstants.SpritesheetTexturePath);
    }

    public static Texture2D GetSpritesheetTexture()
    {
      return texture;
    }

    public static Texture2D GetScrollsTexture()
    {
      return ModUtility.Helper.ModContent.Load<Texture2D>(ModConstants.ScrollsTexturePath);
    }

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