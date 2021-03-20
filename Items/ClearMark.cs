using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EvE.Items
{
	public class ClearMark : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Clear All Marks");
			DisplayName.AddTranslation(GameCulture.Chinese, "清除标记");
			Tooltip.SetDefault("Left click to remove all the marks on the creature\n" +
				"Right click to remove all the marked creatures and their minions and projectiles\n" +
				"When your game get stucked, try using this");
			Tooltip.AddTranslation(GameCulture.Chinese, "左键清除掉所有生物的标记\n" +
				"右键清除标记的生物及其衍生仆从和弹幕\n" +
				"如果你游戏出现了问题，用一下这个试试");
		}

		public override void SetDefaults() 
		{
			item.width = 32;
			item.height = 32;
			item.useTime = 30;
			item.useAnimation = 30;
			item.noMelee = true;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.value = 0;
			item.rare = ItemRarityID.Expert;
			item.UseSound = SoundID.Item44;
			item.autoReuse = true;
			item.shoot = ProjectileID.WoodenArrowFriendly;
		}
		public override bool AltFunctionUse(Player player) => true;
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			if (player.altFunctionUse == 2)
			{
				if (EvE.EnemyA != -1) 
				{
					foreach (Projectile proj in Main.projectile)
					{
						if (proj.active)
						{
							if (proj.GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI == EvE.EnemyA)
                            {
								proj.active = false;
                            }
						}
					}
					foreach (NPC npc in Main.npc)
					{
                        if (npc.active)
                        {
                            if (EvE.IsOrBelongsToNPCID(npc) == EvE.EnemyA)
                            {
								npc.active = false;
                            }
                        }
					}
					EvE.EnemyA = -1;
				}
				if (EvE.EnemyB != -1)
				{
					foreach (Projectile proj in Main.projectile)
					{
						if (proj.active)
						{
							if (proj.GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI == EvE.EnemyB)
							{
								proj.active = false;
							}
						}
					}
					foreach (NPC npc in Main.npc)
					{
						if (npc.active)
						{
							if (EvE.IsOrBelongsToNPCID(npc) == EvE.EnemyB)
							{
								npc.active = false;
							}
						}
					}
					EvE.EnemyB = -1;
				}
				string str = Language.ActiveCulture == GameCulture.Chinese ? "目标全部清除！" : "Target all clear!";
				CombatText.NewText(player.Hitbox, Color.Green, str);
			}
			else
			{
				EvE.EnemyA = -1;
				EvE.EnemyB = -1;
				string str = Language.ActiveCulture == GameCulture.Chinese ? "标记全部清除！" : "Mark all clear!";
				CombatText.NewText(player.Hitbox, Color.Green, str);
			}
			return false;
        }
        public override void AddRecipes() 
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}