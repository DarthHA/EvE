using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.GameContent.Events;

namespace EvE
{
    public class FakePlayer : ModPlayer
    {
        public static bool Initialised1 = false;
        public static bool Initialised2 = false;



        public static bool Dummy1Available()
        {
            if (!Initialised1) return false;
            return Main.player[EvE.FakePlayer1].active;
        }

        public static bool Dummy2Available()
        {
            if (!Initialised2) return false;
            return Main.player[EvE.FakePlayer2].active;
        }

        public static void InitDummy1(Vector2 Pos)
        {
            if (!Initialised1)
            {
                Initialised1 = true;
                Main.player[EvE.FakePlayer1] = new Player(true)
                {
                    name = "",
                    difficulty = 2,
                    statLifeMax2 = 99999,
                    statLifeMax = 99999,
                    statLife = 99999,
                    Center = Pos,
                    active = true,
                    immuneAlpha = 255,
                    immune = true,
                    immuneTime = 60,
                };
                Main.player[EvE.FakePlayer1].PlayerFrame();
            }
            else
            {
                if (!Main.player[EvE.FakePlayer1].active || Main.player[EvE.FakePlayer1].dead)
                {
                    Main.player[EvE.FakePlayer1].active = true;
                    Main.player[EvE.FakePlayer1].dead = false;
                    Main.player[EvE.FakePlayer1].statLife = 99999;
                    Main.player[EvE.FakePlayer1].statLifeMax = 99999;
                    Main.player[EvE.FakePlayer1].statLifeMax2 = 99999;
                    Main.player[EvE.FakePlayer1].immuneAlpha = 255;
                    Main.player[EvE.FakePlayer1].immune = true;
                    Main.player[EvE.FakePlayer1].immuneTime = 60;
                }
            }
        }

        public static void InitDummy2(Vector2 Pos)
        {
            if (!Initialised2)
            {
                Initialised2 = true;
                Main.player[EvE.FakePlayer2] = new Player(true)
                {
                    name = "",
                    difficulty = 2,
                    statLifeMax2 = 99999,
                    statLifeMax = 99999,
                    statLife = 99999,
                    Center = Pos,
                    active = true,
                    immuneAlpha = 255,
                    immune = true,
                    immuneTime = 60,
                };
                Main.player[EvE.FakePlayer2].PlayerFrame();
            }
            else
            {
                if (!Main.player[EvE.FakePlayer2].active || Main.player[EvE.FakePlayer2].dead)
                {
                    Main.player[EvE.FakePlayer2].active = true;
                    Main.player[EvE.FakePlayer2].dead = false;
                    Main.player[EvE.FakePlayer2].statLife = 99999;
                    Main.player[EvE.FakePlayer2].statLifeMax = 99999;
                    Main.player[EvE.FakePlayer2].statLifeMax2 = 99999;
                    Main.player[EvE.FakePlayer2].immuneAlpha = 255;
                    Main.player[EvE.FakePlayer2].immune = true;
                    Main.player[EvE.FakePlayer2].immuneTime = 60;
                }
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (EvE.BelongsToDummy(player))
            {
                //player.UpdateBiomes();
                UpdateBiomesAlt(player);
                //player.ZoneJungle = true;
                //player.ZoneCorrupt = true;
                //player.ZoneCrimson = true;
                player.aggro -= 1000000;
                player.gravity = 0;
                player.velocity = Vector2.Zero;
                if (player.whoAmI == EvE.FakePlayer1 && EvE.EnemyA != -1)
                {
                    if (Main.npc[EvE.EnemyA].active)
                    {
                        player.Center = Main.npc[EvE.EnemyA].Center;
                    }
                }

                if (player.whoAmI == EvE.FakePlayer2 && EvE.EnemyB != -1)
                {
                    if (Main.npc[EvE.EnemyB].active)
                    {
                        player.Center = Main.npc[EvE.EnemyB].Center;
                    }
                }
            }
        }
        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            if (EvE.BelongsToDummy(player))
            {
                return false;
            }
            return true;
        }
        public override bool CanBeHitByProjectile(Projectile proj)
        {
            if (EvE.BelongsToDummy(player))
            {
                return false;
            }
            return true;
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (EvE.BelongsToDummy(player))
            {
                return false;
            }
            return true;
        }
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (EvE.BelongsToDummy(player))
            {
                return false;
            }
            return true;
        }
        public override void PreUpdateBuffs()
        {
            if (EvE.BelongsToDummy(player))
            {
                for (int i = 0; i < player.buffImmune.Length; i++)
                {
                    player.buffImmune[i] = true;
                }

                for (int i = 0; i < player.buffTime.Length; i++)
                {
                    if (player.buffTime[i] != 0)
                    {
                        player.buffTime[i] = 0;
                    }
                }
            }
        }
        
        public override void ModifyDrawHeadLayers(List<PlayerHeadLayer> layers)
        {
            if (EvE.BelongsToDummy(player)) //dont draw player
            {
                while (layers.Count > 0)
                    layers.RemoveAt(0);
            }
        }
        
        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            if (EvE.BelongsToDummy(player)) //dont draw player
            {
                while (layers.Count > 0)
                    layers.RemoveAt(0);
            }
        }
        
        public override void PostUpdate()
        {
            if (player.whoAmI == EvE.FakePlayer1)
            {
                if (EvE.EnemyA < 0 || EvE.EnemyA > 200)
                {
                    player.active = false;
                }
                else if (!Main.npc[EvE.EnemyA].active)
                {
                    player.active = false;
                }
            }

            if (player.whoAmI == EvE.FakePlayer2)
            {
                if (EvE.EnemyB < 0 || EvE.EnemyB > 200)
                {
                    player.active = false;
                }
                else if (!Main.npc[EvE.EnemyB].active)
                {
                    player.active = false;
                }
            }
        }

		public void UpdateBiomesAlt(Player player)
		{
			Point point = player.Center.ToTileCoordinates();
			player.ZoneDungeon = false;
			if (Main.dungeonTiles >= 250 && player.Center.Y > Main.worldSurface * 16.0)
			{
				int num = (int)player.Center.X / 16;
				int num2 = (int)player.Center.Y / 16;
				if (Main.wallDungeon[(int)Main.tile[num, num2].wall])
				{
					player.ZoneDungeon = true;
				}
			}
			Tile tileSafely = Framing.GetTileSafely(player.Center);
			if (tileSafely != null)
			{
				player.behindBackWall = tileSafely.wall > 0;
			}
			if (Main.sandTiles > 1000 && player.Center.Y > 3200f)
			{
				if (WallID.Sets.Conversion.Sandstone[tileSafely.wall] || WallID.Sets.Conversion.HardenedSand[tileSafely.wall])
				{
					player.ZoneUndergroundDesert = true;
				}
			}
			else
			{
				player.ZoneUndergroundDesert = false;
			}
			player.ZoneCorrupt = Main.evilTiles >= 200;
			player.ZoneHoly = Main.holyTiles >= 100;
			player.ZoneMeteor = Main.meteorTiles >= 50;
			player.ZoneJungle = Main.jungleTiles >= 80;
			player.ZoneSnow = Main.snowTiles >= 300;
			player.ZoneCrimson = Main.bloodTiles >= 200;
			player.ZoneWaterCandle = Main.waterCandles > 0;
			player.ZonePeaceCandle = Main.peaceCandles > 0;
			player.ZoneDesert = Main.sandTiles > 1000;
			player.ZoneGlowshroom = Main.shroomTiles > 100;
			player.ZoneUnderworldHeight = point.Y > Main.maxTilesY - 200;
			player.ZoneRockLayerHeight = point.Y <= Main.maxTilesY - 200 && point.Y > Main.rockLayer;
			player.ZoneDirtLayerHeight = point.Y <= Main.rockLayer && point.Y > Main.worldSurface;
			player.ZoneOverworldHeight = point.Y <= Main.worldSurface && point.Y > Main.worldSurface * 0.34999999403953552;
			player.ZoneSkyHeight = point.Y <= Main.worldSurface * 0.34999999403953552;
			player.ZoneBeach = player.ZoneOverworldHeight && (point.X < 380 || point.X > Main.maxTilesX - 380);
			player.ZoneRain = Main.raining && point.Y <= Main.worldSurface;
			player.ZoneSandstorm = point.Y <= Main.worldSurface && player.ZoneDesert && !player.ZoneBeach && Sandstorm.Happening;
			player.ZoneTowerSolar = player.ZoneTowerVortex = (player.ZoneTowerNebula = (player.ZoneTowerStardust = false));
			player.ZoneOldOneArmy = false;
			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].active)
				{
					if (Main.npc[i].type == NPCID.LunarTowerStardust)
					{
						if (player.Distance(Main.npc[i].Center) <= 4000f)
						{
							player.ZoneTowerStardust = true;
						}
					}
					else if (Main.npc[i].type == NPCID.LunarTowerNebula)
					{
						if (player.Distance(Main.npc[i].Center) <= 4000f)
						{
							player.ZoneTowerNebula = true;
						}
					}
					else if (Main.npc[i].type == NPCID.LunarTowerVortex)
					{
						if (player.Distance(Main.npc[i].Center) <= 4000f)
						{
							player.ZoneTowerVortex = true;
						}
					}
					else if (Main.npc[i].type == NPCID.LunarTowerSolar)
					{
						if (player.Distance(Main.npc[i].Center) <= 4000f)
						{
							player.ZoneTowerSolar = true;
						}
					}
					else if (Main.npc[i].type == NPCID.DD2LanePortal && player.Distance(Main.npc[i].Center) <= 4000f)
					{
						player.ZoneOldOneArmy = true;
					}
				}
			}
			PlayerHooks.UpdateBiomes(player);

			//PlayerHooks.UpdateBiomeVisuals(player);
            /*
			if (!player.dead)
			{
				if (player._funkytownCheckCD > 0)
				{
					player._funkytownCheckCD--;
				}
			}
			else
			{
				player._funkytownCheckCD = 100;
			}
            */
		}


	}
}