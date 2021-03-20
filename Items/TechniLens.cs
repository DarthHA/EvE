using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EvE.Items
{
	public class TechniLens : ModItem
	{
        public override bool Autoload(ref string name)
        {
			return false;
        }
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("仇恨晶状体");
			Tooltip.SetDefault("显示怪物仇恨情况\n测试用,怪物较多时可能会产生卡顿，甚至是游戏崩溃");
		}

		public override void SetDefaults() 
		{
			item.width = 32;
			item.height = 32;
			item.accessory = true;
			item.value = 0;
			item.rare = ItemRarityID.Expert;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<LensPlayer>().Lens = true;
        }
        public override void AddRecipes() 
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}