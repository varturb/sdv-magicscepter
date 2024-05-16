using MagicScepter.Helpers;

namespace MagicScepter.Constants
{
  public static class TranslatedKeys
  {
    public static string Title => Get("Title");
    public static string Cancel => Get("Cancel");
    public static string Save => Get("Save");
    public static string ResetToDefault => Get("ResetToDefault");
    public static string ChangeName => Get("ChangeName");
    public static string Hidden => Get("Hidden");
    public static string Visible => Get("Visible");
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