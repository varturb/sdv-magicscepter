using MagicScepter.Helpers;

namespace MagicScepter.Constants
{
  public static class TranslatedKeys
  {
    public static string Title => Get("Title");
    public static string Cancel => Get("Cancel");
    public static string Save => Get("Save");
    public static string ResetToDefault => Get("ResetToDefault");
    public static string Rename => Get("Rename");
    public static string Hide => Get("Hide");
    public static string Show => Get("Show");
    public static string MoveUp => Get("MoveUp");
    public static string MoveDown => Get("MoveDown");
    public static string Configuration => Get("Configuration");
    public static string Settings => Get("Settings");

    private static string Get(string key)
    {
      return TranslationHelper.Get(key);
    }
  }
}