
using Terraria.Localization;
using Terraria.ModLoader;

namespace EvE
{
    public static class TranslationUtils
    {
        public static void AddTranslation(string En, string Zh)
        {
            string temp = En.Replace(" ", "_");
            ModTranslation CustomText = EvE.Instance.CreateTranslation(temp);
            CustomText.SetDefault(En);
            CustomText.AddTranslation(GameCulture.Chinese, Zh);
            EvE.Instance.AddTranslation(CustomText);
        }
        public static void AddTranslation(string key ,string En, string Zh)
        {
            ModTranslation CustomText = EvE.Instance.CreateTranslation(key);
            CustomText.SetDefault(En);
            CustomText.AddTranslation(GameCulture.Chinese, Zh);
            EvE.Instance.AddTranslation(CustomText);
        }
        public static string GetTranslation(string key)
        {
            return Language.GetTextValue("Mods.EvE." + key);
        }

        public static string GetTranslationConfig(string key)
        {
            return Language.GetTextValue("$Mods.EvE." + key);
        }
    }
}