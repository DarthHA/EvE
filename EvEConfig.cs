using System.ComponentModel;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace EvE
{
    public class EvEConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;


        [Label("$Mods.EvE.EvEMeleeDamage")]
        [Increment(1)]
        [Range(1, 100)]
        [DefaultValue(1)]
        [Slider]
        public int MeleeDamageMultiplier;


        [Label("$Mods.EvE.EvEProjDamage")]
        [Increment(1)]
        [Range(1, 100)]
        [DefaultValue(1)]
        [Slider]
        public int ProjDamageMultiplier;


        [Label("$Mods.EvE.EvEDotDamage")]
        [Increment(1)]
        [Range(1, 300)]
        [DefaultValue(1)]
        [Slider]
        public int DotMultiplier;

        public override ModConfig Clone()
        {
            var clone = (EvEConfig)base.Clone();
            return clone;
        }

        public override void OnLoaded()
        {
            EvE.config = this;
            TranslationUtils.AddTranslation("EvEDotDamage", "Debuff damage multifier dealt during the Super Smash.", "大乱斗中负面效果伤害系数。");
            TranslationUtils.AddTranslation("EvEMeleeDamage", "Melee damage multiplier dealt during the Super Smash.", "大乱斗中近战伤害系数。");
            TranslationUtils.AddTranslation("EvEProjDamage", "Projectile damage multifier dealt during the Super Smash.", "大乱斗中射弹伤害系数。");
        }


        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string messageline)
        {
            string message = "";
            string messagech = "";

            if (Language.ActiveCulture == GameCulture.Chinese)
            {
                messageline = messagech;
            }
            else
            {
                messageline = message;
            }

            if (whoAmI == 0)
            {
                //message = "Changes accepted!";
                //messagech = "设置改动成功!";
                return true;
            }
            if (whoAmI != 0)
            {
                //message = "You have no rights to change config.";
                //messagech = "你没有设置改动权限.";
                return false;
            }
            return false;
        }
    }
}