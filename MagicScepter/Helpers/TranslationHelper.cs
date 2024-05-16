namespace MagicScepter.Helpers
{
  public static class TranslationHelper
  {
    public static string Get(string key)
    {
      return ModUtility.Helper.Translation.Get(key);
    }
  }
}