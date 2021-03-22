using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace EvE
{
    public class EvE : Mod
	{
        public static EvEConfig config;
        public static EvE Instance;
        public static string NewXXXState = "";
        public static int NewXXXSource = -1;
        public const int FakePlayer1 = 101;
        public const int FakePlayer2 = 102;
        public static int EnemyA = -1;
        public static int EnemyB = -1;
        public EvE()
        {
            Instance = this;
        }
        public override void Unload()
        {
            Instance = null;
            config = null;
            NewXXXSource = -1;
            NewXXXState = "";
            EnemyA = -1;
            EnemyB = -1;
            FakePlayer.Initialised1 = false;
            FakePlayer.Initialised2 = false;
            Main.player[FakePlayer1] = new Player(false);
            Main.player[FakePlayer2] = new Player(false);
        }
        public override void Load()
        {
            On.Terraria.Projectile.NewProjectile_float_float_float_float_int_int_float_int_float_float += new On.Terraria.Projectile.hook_NewProjectile_float_float_float_float_int_int_float_int_float_float(NewProjHook);
            On.Terraria.NPC.NewNPC += new On.Terraria.NPC.hook_NewNPC(NewNPCHook);
            On.Terraria.NPC.AI += new On.Terraria.NPC.hook_AI(NPCAIHook);
            On.Terraria.Projectile.AI += new On.Terraria.Projectile.hook_AI(ProjAIHook);
            On.Terraria.NPC.VanillaHitEffect += new On.Terraria.NPC.hook_VanillaHitEffect(HitEffectHook);
            On.Terraria.NPC.checkDead += new On.Terraria.NPC.hook_checkDead(CheckDeadHook);
            On.Terraria.NPC.SetDefaults += new On.Terraria.NPC.hook_SetDefaults(NPCSetDefHook);
            On.Terraria.NPC.TargetClosest += new On.Terraria.NPC.hook_TargetClosest(TargetClosestHook);
            On.Terraria.Utilities.NPCUtils.SearchForTarget_NPC_Vector2_TargetSearchFlag_SearchFilter1_SearchFilter1 += new On.Terraria.Utilities.NPCUtils.hook_SearchForTarget_NPC_Vector2_TargetSearchFlag_SearchFilter1_SearchFilter1(SearchForTargetHook);
            On.Terraria.Player.FindClosest += new On.Terraria.Player.hook_FindClosest(FindClosestHook);
            On.Terraria.Player.KillMe += new On.Terraria.Player.hook_KillMe(KillMeHook);
            On.Terraria.Player.KillMeForGood += new On.Terraria.Player.hook_KillMeForGood(KillMeForGoodHook);
            On.Terraria.Player.Ghost += new On.Terraria.Player.hook_Ghost(GhostHook);
            On.Terraria.Player.AddBuff += new On.Terraria.Player.hook_AddBuff(AddBuffHook);
        }



        public static int IsOrBelongsToNPCID(NPC npc)
        {
            if (npc.GetGlobalNPC<ProjectileOwnerGNPC>().OwnerWMI == -1) 
            {
                return npc.whoAmI; 
            }
            else
            {
                return npc.GetGlobalNPC<ProjectileOwnerGNPC>().OwnerWMI;
            }
        }

        
        public static void AnotherTargetSelect(NPC self, int target, bool faceTarget)
        {
            if (Main.player[target].active)
            {
                self.target = target;
                self.targetRect = new Rectangle((int)Main.player[target].position.X, (int)Main.player[target].position.Y, Main.player[target].width, Main.player[target].height);
                if (Main.player[self.target].dead)
                {
                    faceTarget = false;
                }
                /*
                if (Main.player[self.target].npcTypeNoAggro[self.type] && self.direction != 0)
                {
                    faceTarget = false;
                }
                */

                if (faceTarget)
                {
                    self.direction = 1;
                    if (Main.player[self.target].Center.X < self.Center.X)
                    {
                        self.direction = -1;
                    }
                    self.directionY = 1;
                    if (Main.player[self.target].Center.Y < self.Center.Y)
                    {
                        self.directionY = -1;
                    }
                }
                
                if (self.confused)
                {
                    self.direction *= -1;
                }
                
            }
        }
        public static NPCUtils.TargetSearchResults AnotherDD2TargetSelect(NPC searcher, Vector2 position, int Target, NPCUtils.TargetSearchFlag flags = NPCUtils.TargetSearchFlag.All, NPCUtils.SearchFilter<NPC> npcFilter = null)
        {
            float nearestNPCDistance = float.MaxValue;
            int nearestNPCIndex = -1;
            float nearestAdjustedTankDistance = float.MaxValue;
            float nearestTankDistance = float.MaxValue;
            int nearestTankIndex = -1;
            NPCUtils.TargetType tankType = NPCUtils.TargetType.Player;


            if (flags.HasFlag(NPCUtils.TargetSearchFlag.Players))
            {
                Player player = Main.player[Target];
                nearestTankIndex = Target;
                nearestTankDistance = searcher.Distance(player.Center);
                nearestAdjustedTankDistance = searcher.Distance(player.Center);
                tankType = NPCUtils.TargetType.Player;
            }
            else if (flags.HasFlag(NPCUtils.TargetSearchFlag.NPCs))
            {
                for (int i = 0; i < 200; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && (npcFilter == null || npcFilter(npc)))
                    {
                        float dist = Vector2.DistanceSquared(position, npc.Center);
                        if (dist < nearestNPCDistance)
                        {
                            nearestNPCIndex = i;
                            nearestNPCDistance = dist;
                        }
                    }
                }
            }


            return new NPCUtils.TargetSearchResults(searcher, nearestNPCIndex, (float)Math.Sqrt(nearestNPCDistance), nearestTankIndex, nearestTankDistance, nearestAdjustedTankDistance, tankType);
        }
        


        public static bool BelongsToDummy(Player player)
        {
            if (player.whoAmI == FakePlayer1 || player.whoAmI == FakePlayer2) return true;
            return false;   
        }

        public override void MidUpdatePlayerNPC()
        {
            
            if (EnemyA >= 0)
            {
                if (!Main.npc[EnemyA].active)
                {
                    EnemyA = -1;
                    EnemyB = -1;
                    Main.player[FakePlayer1].active = false;
                }
            }
            if (EnemyB >= 0)
            {
                if (!Main.npc[EnemyB].active)
                {
                    EnemyA = -1;
                    EnemyB = -1;
                    Main.player[FakePlayer2].active = false;
                }
            }

            if (EnemyA == -1) Main.player[FakePlayer1].active = false;
            if (EnemyB == -1) Main.player[FakePlayer2].active = false;

            if (EnemyA >= 0 && EnemyB >= 0)
            {
                FakePlayer.InitDummy1(Main.npc[EnemyA].Center);
                FakePlayer.InitDummy2(Main.npc[EnemyB].Center);
                Main.player[FakePlayer1].dead = false;
                Main.player[FakePlayer2].dead = false;
            }
        }

        
        public static void UpdateOwner(NPC npc)
        {
            if (npc.GetGlobalNPC<ProjectileOwnerGNPC>().OwnerWMI != -1)
            {
                if (!Main.npc[npc.GetGlobalNPC<ProjectileOwnerGNPC>().OwnerWMI].active)
                {
                    npc.GetGlobalNPC<ProjectileOwnerGNPC>().OwnerWMI = -1;
                }
            }
        }

        public static void UpdateOwner(Projectile proj)
        {
            if (proj.GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI != -1)
            {
                if (!Main.npc[proj.GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI].active)
                {
                    proj.GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI = -1;
                }
            }
        }



        #region Patch相关

        public static void TargetClosestHook(On.Terraria.NPC.orig_TargetClosest orig, NPC self, bool faceTarget)
        {
            if (IsOrBelongsToNPCID(self) == EnemyA)
            {
                if (EnemyB != -1)
                {
                    if (Main.npc[EnemyB].active && FakePlayer.Dummy2Available())
                    {
                        Main.player[FakePlayer2].dead = false;
                        Main.player[FakePlayer2].Center = Main.npc[EnemyB].Center;
                        AnotherTargetSelect(self, FakePlayer2, faceTarget);
                        return;
                    }
                }
            }

            if (IsOrBelongsToNPCID(self) == EnemyB)
            {
                if (EnemyA != -1)
                {
                    if (Main.npc[EnemyA].active && FakePlayer.Dummy1Available())
                    {
                        Main.player[FakePlayer1].dead = false;
                        Main.player[FakePlayer1].Center = Main.npc[EnemyA].Center;
                        AnotherTargetSelect(self, FakePlayer1, faceTarget);
                        return;
                    }
                }
            }
            orig.Invoke(self, faceTarget);
        }

        public static byte FindClosestHook(On.Terraria.Player.orig_FindClosest orig, Vector2 Position, int Width, int Height)
        {
            if (NewXXXState == "Projectile")
            {
                if (Main.projectile[NewXXXSource].GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI != -1)
                {
                    if (Main.projectile[NewXXXSource].GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI == EnemyA)
                    {
                        if (EnemyB != -1)
                        {
                            if (Main.npc[EnemyB].active && FakePlayer.Dummy2Available())
                            {
                                return FakePlayer2;
                            }
                        }
                    }

                    if (Main.projectile[NewXXXSource].GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI == EnemyB)
                    {
                        if (EnemyA != -1)
                        {
                            if (Main.npc[EnemyA].active && FakePlayer.Dummy1Available())
                            {
                                return FakePlayer1;
                            }
                        }
                    }
                }
            }
            return orig.Invoke(Position, Width, Height);
        }

        public static NPCUtils.TargetSearchResults SearchForTargetHook(On.Terraria.Utilities.NPCUtils.orig_SearchForTarget_NPC_Vector2_TargetSearchFlag_SearchFilter1_SearchFilter1 orig, NPC searcher, Vector2 position, NPCUtils.TargetSearchFlag flags = NPCUtils.TargetSearchFlag.All, NPCUtils.SearchFilter<Player> playerFilter = null, NPCUtils.SearchFilter<NPC> npcFilter = null)
        {
            if (IsOrBelongsToNPCID(searcher) == EnemyA)
            {
                if (EnemyB != -1)
                {
                    if (Main.npc[EnemyB].active && FakePlayer.Dummy2Available())
                    {
                        Main.player[FakePlayer2].dead = false;
                        Main.player[FakePlayer2].Center = Main.npc[EnemyB].Center;
                        return AnotherDD2TargetSelect(searcher, position, FakePlayer2, flags, npcFilter);

                    }
                }
            }

            if (IsOrBelongsToNPCID(searcher) == EnemyB)
            {
                if (EnemyA != -1)
                {
                    if (Main.npc[EnemyA].active && FakePlayer.Dummy1Available())
                    {
                        Main.player[FakePlayer1].dead = false;
                        Main.player[FakePlayer1].Center = Main.npc[EnemyA].Center;
                        return AnotherDD2TargetSelect(searcher, position, FakePlayer1, flags, npcFilter);

                    }
                }
            }
            return orig.Invoke(searcher, position, flags, playerFilter, npcFilter);
        }


        public static void AddBuffHook(On.Terraria.Player.orig_AddBuff orig,Player self,int type,int time1,bool quiet)
        {
            if (self.whoAmI == FakePlayer1)
            {
                if (EnemyA != -1)
                {
                    if (Main.npc[EnemyA].active)
                    {
                        Main.npc[EnemyA].AddBuff(type, time1, quiet);
                    }
                }
                return;
            }

            if (self.whoAmI == FakePlayer2)
            {
                if (EnemyB != -1)
                {
                    if (Main.npc[EnemyB].active)
                    {
                        Main.npc[EnemyB].AddBuff(type, time1, quiet);
                    }
                }
                return;
            }
            orig.Invoke(self, type, time1, quiet);
        }

        public static void GhostHook(On.Terraria.Player.orig_Ghost orig, Player self)
        {
            if (BelongsToDummy(self))
            {
                self.ghost = false;
                self.dead = false;
                return;
            }
            orig.Invoke(self);
        }
        public static void KillMeForGoodHook(On.Terraria.Player.orig_KillMeForGood orig, Player self)
        {
            if (BelongsToDummy(self))
            {
                self.ghost = false;
                self.dead = false;
                return;
            }
            orig.Invoke(self);
        }
        public static void KillMeHook(On.Terraria.Player.orig_KillMe orig, Player self, PlayerDeathReason damageSource, double dmg, int hitdirection, bool pvp = false)
        {
            if (BelongsToDummy(self))
            {
                self.statLife = self.statLifeMax2;
                if (self.statLifeMax < 99999) self.statLifeMax = 99999;
                if (self.statLifeMax2 < 99999) self.statLifeMax2 = 99999;
                self.dead = false;
                self.ghost = false;
                self.statLife = self.statLifeMax2;
                return;
            }
            orig.Invoke(self, damageSource, dmg, hitdirection, pvp);
        }

        public static void NPCSetDefHook(On.Terraria.NPC.orig_SetDefaults orig, NPC self, int Type, float scaleOverride = -1f)
        {
            orig.Invoke(self, Type, scaleOverride);
            if (NewXXXState == "NPC")
            {
                self.GetGlobalNPC<ProjectileOwnerGNPC>().OwnerWMI = GetSource(Main.npc[NewXXXSource]);
            }
            else if (NewXXXState == "Projectile")
            {
                self.GetGlobalNPC<ProjectileOwnerGNPC>().OwnerWMI = GetSource(Main.projectile[NewXXXSource]); //Main.projectile[NewXXXSource].GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI;
            }
        }

        public static void CheckDeadHook(On.Terraria.NPC.orig_checkDead orig,NPC self)
        {
            NewXXXState = "NPC";
            NewXXXSource = self.whoAmI;
            orig.Invoke(self);
            NewXXXState = "";
            NewXXXSource = -1;
        }

        public static void HitEffectHook(On.Terraria.NPC.orig_VanillaHitEffect orig, NPC self, int hitDirection, double dmg)
        {
            NewXXXState = "NPC";
            NewXXXSource = self.whoAmI;
            orig.Invoke(self, hitDirection, dmg);
            NewXXXState = "";
            NewXXXSource = -1;
        }

        public static void NPCAIHook(On.Terraria.NPC.orig_AI orig, NPC self)
        {
            NewXXXState = "NPC";
            NewXXXSource = self.whoAmI;
            orig.Invoke(self);
            NewXXXState = "";
            NewXXXSource = -1;
        }

        public static void ProjAIHook(On.Terraria.Projectile.orig_AI orig,Projectile self)
        {
            NewXXXState = "Projectile";
            NewXXXSource = self.whoAmI;
            orig.Invoke(self);
            NewXXXState = "";
            NewXXXSource = -1;
        }

        public static int NewNPCHook(On.Terraria.NPC.orig_NewNPC orig, int X, int Y, int Type, int Start, float ai0, float ai1, float ai2, float ai3, int Target)
        {
            //if (Type == NPCID.Probe) Main.NewText(NewXXXState);

            int wmi = orig.Invoke(X, Y, Type, Start, ai0, ai1, ai2, ai3, Target);
            if (wmi != -1)
            {
                if (NewXXXState == "NPC")
                {
                    Main.npc[wmi].GetGlobalNPC<ProjectileOwnerGNPC>().OwnerWMI = GetSource(Main.npc[NewXXXSource]);
                }
                else if (NewXXXState == "Projectile")
                {
                    Main.npc[wmi].GetGlobalNPC<ProjectileOwnerGNPC>().OwnerWMI = Main.projectile[NewXXXSource].GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI;
                }
            }
            return wmi;
        }

        public static int NewProjHook(On.Terraria.Projectile.orig_NewProjectile_float_float_float_float_int_int_float_int_float_float orig, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner = 255, float ai0 = 0f, float ai1 = 0f)
        {
            int wmi = orig.Invoke(X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1);
            if (wmi != -1)
            {
                if (NewXXXState == "NPC")
                {
                    Main.projectile[wmi].GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI = GetSource(Main.npc[NewXXXSource]);
                }
                else if (NewXXXState == "Projectile")
                {
                    Main.projectile[wmi].GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI = Main.projectile[NewXXXSource].GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI;// Main.projectile[NewXXXSource].GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI;
                }
            }
            return wmi;
        }


        private static int GetSource(Projectile proj)
        {
            int target = proj.GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI;
            if (target == -1) return -1;
            if (!Main.npc[target].active) return -1;
            return GetSource(Main.npc[target]);
        }

        private static int GetSource(NPC npc)
        {
            if (!npc.active) return -1;
            if (npc.type >= NPCID.EaterofWorldsHead && npc.type <= NPCID.EaterofWorldsTail) return -1;

            if (npc.modNPC != null)
            {
                if (npc.modNPC.mod.Name == "MABBossChallenge" || npc.modNPC.mod.Name == "MentalAIBoost")
                {
                    if (npc.friendly)
                    {
                        return -1;
                    }
                }
            }
            
            int target = npc.GetGlobalNPC<ProjectileOwnerGNPC>().OwnerWMI;
            if (target == -1 || target == npc.whoAmI) return npc.whoAmI;
            if(!Main.npc[target].active) return npc.whoAmI;
            if (Main.npc[target].GetGlobalNPC<ProjectileOwnerGNPC>().OwnerWMI != -1)
            {
                return GetSource(Main.npc[target]);
            }
            else
            {
                return target;
            }
        }

        #endregion
    }


    public class LensPlayer : ModPlayer
    {
        public bool Lens = false;
        public int WatcherID = -1;
        public override void ResetEffects()
        {
            Lens = false;
        }
        public override void UpdateDead()
        {
            Lens = false;
        }

        public override void ModifyScreenPosition()
        {
            if (WatcherID != -1)
            {
                if (Main.npc[WatcherID].active)
                {
                    Main.screenPosition = Main.npc[WatcherID].Center - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
                }
                else
                {
                    WatcherID = -1;
                }
            }
        }

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active) return;
            if (Main.LocalPlayer.GetModPlayer<LensPlayer>().Lens)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && !npc.friendly)
                    {
                        if (npc.target == player.whoAmI)
                        {
                            if (npc.Distance(player.Center) > 4)
                            {
                                for (int i = 0; i <= npc.Distance(player.Center); i += 2)
                                {
                                    Vector2 DrawPos = player.Center + Vector2.Normalize(npc.Center - player.Center) * i - Main.screenPosition;
                                    if (DrawPos.X > 0 && DrawPos.Y > 0 && DrawPos.X < Main.screenWidth && DrawPos.Y < Main.screenHeight)
                                    {
                                        Main.spriteBatch.Draw(Main.magicPixel, DrawPos, new Rectangle(0, 0, 2, 2), Color.DarkRed, 0, Vector2.Zero, 1, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
                                    }
                                }
                            }

                        }
                    }
                }

                if (EvE.EnemyA >= 0)
                {
                    if (player.whoAmI == Main.npc[EvE.EnemyA].target)
                    {
                        string str = Language.ActiveCulture == GameCulture.Chinese ? "A的目标" : "A Target";
                        Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, str, player.Center.X - Main.screenPosition.X, player.Top.Y - 20 - Main.screenPosition.Y, Color.Red, Color.Black, Vector2.Zero);
                    }
                }

                if (EvE.EnemyB >= 0)
                {
                    if (player.whoAmI == Main.npc[EvE.EnemyB].target)
                    {
                        string str = Language.ActiveCulture == GameCulture.Chinese ? "B的目标" : "B Target";
                        Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, str, player.Center.X - Main.screenPosition.X, player.Top.Y - 20 - Main.screenPosition.Y, Color.Cyan, Color.Black, Vector2.Zero);
                    }
                }
            }
        }
    }


}