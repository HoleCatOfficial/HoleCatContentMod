
using System.Collections.Generic;
using System.Security.Permissions;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.ConstitutionBoss;
using DestroyerTest.Content.Projectiles.HellWeapons;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib.Content.Helpers;
using System;
using System.Linq;
using Terraria.GameContent.ItemDropRules;
using DestroyerTest.Content.Scepter;
using OpusLib;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class Scroll : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.value = Item.buyPrice(10);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public virtual void RegisterScroll(Player player)
        {
            if (player.TryGetModPlayer<ScrollScepterUsePlayer>(out ScrollScepterUsePlayer Scptr))
            {
            }
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            RegisterScroll(player);
        }
    }

    public class ScrollScepterUsePlayer : ModPlayer
    {
        public bool CurseScroll = false;
        public bool EtherScroll = false;
        public bool FrigidScroll1 = false;
        public bool FrigidScroll2 = false;
        public bool HandScroll = false;
        public bool HellfireScroll1 = false;
        public bool PurityScroll = false;
        public bool SandScroll = false;
        public bool StarScroll = false;
        public bool TreasonScroll = false;
        public bool TurbulenceScroll = false;
        public bool GalantineScroll = false;
        public bool IncendiaryScroll = false;
        public bool SharkronPendant = false;
        public bool TempestScroll = false;
        public override void ResetEffects()
        {
            CurseScroll = false;
            EtherScroll = false;
            FrigidScroll1 = false;
            FrigidScroll2 = false;
            HandScroll = false;
            HellfireScroll1 = false;
            PurityScroll = false;
            SandScroll = false;
            StarScroll = false;
            TreasonScroll = false;
            TurbulenceScroll = false;
            GalantineScroll = false;
            SharkronPendant = false;
            TempestScroll = false;
        }
        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (CurseScroll)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        for (int t = 0; t < 3; t++)
                        {
                            Vector2 outer = Player.Center + Main.rand.NextVector2CircularEdge(10, 10);
                            Vector2 motion = outer - position;

                            Projectile.NewProjectile(
                                Player.GetSource_ItemUse(item),
                                Player.Center,
                                motion,
                                ModContent.ProjectileType<CurseProjectile>(),
                                damage / 3,
                                knockback,
                                Player.whoAmI
                            );
                        }
                    }
                }
            }
            if (EtherScroll)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        for (int t = 0; t < 5; t++)
                        {
                            Projectile.NewProjectile(
                                Player.GetSource_ItemUse(item),
                                Player.Center,
                                velocity.RotatedByRandom(4),
                                ProjectileID.DD2PhoenixBowShot,
                                damage,
                                knockback,
                                Player.whoAmI
                            );
                        }
                    }
                }
            }
            if (FrigidScroll1)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        for (int t = 0; t < 5; t++)
                        {
                            Projectile.NewProjectile(
                                Player.GetSource_ItemUse(item),
                                Player.Center,
                                velocity.RotatedByRandom(4),
                                ProjectileID.NorthPoleSpear,
                                damage,
                                knockback,
                                Player.whoAmI
                            );
                        }
                    }
                }
            }
            if (FrigidScroll2)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        for (int t = 0; t < 3; t++)
                        {
                            Projectile.NewProjectile(
                                Player.GetSource_ItemUse(item),
                                Player.Center,
                                velocity.RotatedByRandom(0.5f),
                                ModContent.ProjectileType<SnowStormProjectile>(),
                                damage,
                                knockback,
                                Player.whoAmI
                            );
                        }
                    }
                }
            }
            if (SandScroll)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        for (int t = 0; t < 3; t++)
                        {
                            Projectile.NewProjectile(
                                Player.GetSource_ItemUse(item),
                                Player.Center,
                                velocity.RotatedByRandom(0.5f),
                                ModContent.ProjectileType<SandStormProjectile>(),
                                damage,
                                knockback,
                                Player.whoAmI
                            );
                        }
                    }
                }
            }
            if (StarScroll)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Vector2 target = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                        float ceilingLimit = target.Y;
                        if (ceilingLimit > Player.Center.Y - 200f)
                        {
                            ceilingLimit = Player.Center.Y - 200f;
                        }
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 position2 = Player.Center - new Vector2(Main.rand.NextFloat(401) * Player.direction, 600f);
                            position2.Y -= 100 * i;
                            Vector2 heading = target - position2;

                            if (heading.Y < 0f)
                            {
                                heading.Y *= -1f;
                            }

                            if (heading.Y < 40f)
                            {
                                heading.Y = 40f;
                            }

                            heading.Normalize();
                            heading *= velocity.Length();
                            heading.Y += Main.rand.Next(-40, 41) * 0.02f;
                            Projectile Star = Projectile.NewProjectileDirect(Player.GetSource_ItemUse(item), position2, heading, ProjectileID.StarWrath, damage / 2, knockback, Player.whoAmI, 0f, ceilingLimit);
                            Star.timeLeft = 600;
                        }
                    }
                }
            }
            if (TreasonScroll)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        for (int t = 0; t < 5; t++)
                        {
                            Vector2 outer = Player.Center + Main.rand.NextVector2CircularEdge(5, 5);
                            Vector2 motion = outer - position;

                            Projectile.NewProjectile(
                                Player.GetSource_ItemUse(item),
                                Player.Center,
                                motion,
                                ModContent.ProjectileType<TreasonScrollBomb>(),
                                damage,
                                knockback,
                                Player.whoAmI
                            );
                        }
                    }
                }
            }
            if (TurbulenceScroll)
                {
                    if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            for (int t = 0; t < 5; t++)
                            {
                                Projectile.NewProjectile(
                                    Player.GetSource_ItemUse(item),
                                    Player.Center,
                                    velocity,
                                    ProjectileID.WeatherPainShot,
                                    damage / 2,
                                    knockback,
                                    Player.whoAmI
                                );
                            }
                        }
                    }
                }
            if (GalantineScroll)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        for (int t = 0; t < 5; t++)
                        {
                            Projectile.NewProjectile(
                                Player.GetSource_ItemUse(item),
                                Player.Center,
                                velocity.RotatedByRandom(0.5f),
                                ModContent.ProjectileType<ConstitutionStar>(),
                                damage,
                                knockback,
                                Player.whoAmI,
                                ai2: 1
                            );
                        }
                    }
                }
            }
            if (IncendiaryScroll)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Vector2 target = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                        float screenBottom = Main.screenPosition.Y + Main.screenHeight - 32f;
                        SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/SwordSounds/HellSword", 3));
                        for (int i = 0; i < 5; i++)
                        {
                            float spread = MathHelper.Lerp(0f, Main.screenWidth, i / 4f);
                            Vector2 position2 = new Vector2(Main.screenPosition.X + spread, screenBottom);

                            Vector2 heading = target - position2;
                            if (heading.Length() < 80f)
                                heading = heading.SafeNormalize(Vector2.UnitY) * 80f;
                            else
                                heading = heading.SafeNormalize(Vector2.UnitY) * velocity.Length();

                            int[] types = new int[]
                            {
                                ModContent.ProjectileType<HellHalberd>(),
                                ModContent.ProjectileType<HellScimitar>(),
                                ModContent.ProjectileType<HellSickle>(),
                                ModContent.ProjectileType<HellTrident>()
                            };

                            Projectile.NewProjectileDirect(Player.GetSource_ItemUse(item), position2, heading, types[Main.rand.Next(types.Length)], damage / 2, knockback, Player.whoAmI);
                        }
                    }
                }
            }
            if (SharkronPendant)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Opus.RadialSpreadProjectile(ModContent.ProjectileType<SharkronNecklaceMinion>(), 8, position, damage / 2, 3, 4);
                    }
                }
            }
            if (TempestScroll)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Vector2 outer = Player.Center + Main.rand.NextVector2CircularEdge(5, 5);
                        Vector2 motion = outer - position;

                        Projectile.NewProjectile(
                            Player.GetSource_ItemUse(item),
                            Player.Center,
                            motion,
                            ModContent.ProjectileType<TempestProj>(),
                            damage,
                            knockback,
                            Player.whoAmI
                        );
                    }
                }
            }
        }
    }

    public class ScrollScepterProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool IsAThrownScepter = false;
        public bool IsScepterClassButNotThrown = false;
        public bool ChristmasScroll1 = false;
        public bool ChristmasScroll2 = false;
        public bool ChristmasScroll3 = false;
        public bool IchorScroll = false;
        public bool CursedFlameScroll = false;
        public bool TemporalGlove = false;
        public bool DiabolicScroll = false;
        public bool SpookyScroll1 = false;
        public bool SpookyScroll2 = false;
        public bool SpookyScroll3 = false;
        public bool SpookyScroll4 = false;
        public override void SetDefaults(Projectile entity)
        {
            if (entity.DamageType == ModContent.GetInstance<ScepterClass>() && entity.Name.Contains("Thrown"))
            {
                IsAThrownScepter = true;
            }
            if (entity.DamageType == ModContent.GetInstance<ScepterClass>() && !entity.Name.Contains("Thrown"))
            {
                IsScepterClassButNotThrown = true;
            }
        }

        public int[] OrnamentVariants = new int[]
        {
            ProjectileID.OrnamentFriendly,
            ProjectileID.OrnamentStar
        };

        public int[] FireVariants = new int[]
        {
            ProjectileID.GreekFire1,
            ProjectileID.GreekFire2,
            ProjectileID.GreekFire3
        };

        public bool Flag1;
        public override void AI(Projectile projectile)
        {
            if (ChristmasScroll1 && IsAThrownScepter)
            {
                if (Main.rand.NextBool(4))
                {
                    for (int t = 0; t < 9; t++)
                    {
                        Vector2 outer = projectile.Center + Main.rand.NextVector2CircularEdge(12, 12);
                        Vector2 motion = outer - projectile.Center;

                        Projectile.NewProjectile(
                            projectile.GetSource_FromAI(),
                            projectile.Center,
                            motion,
                            ProjectileID.PineNeedleFriendly,
                            projectile.damage / 4,
                            projectile.knockBack / 4,
                            projectile.owner
                        );
                    }
                }
            }
            if (ChristmasScroll2 && IsAThrownScepter)
            {
                if (Main.rand.NextBool(12))
                {
                    for (int t = 0; t < 7; t++)
                    {
                        Vector2 outer = projectile.Center + Main.rand.NextVector2CircularEdge(12, 12);
                        Vector2 motion = outer - projectile.Center;

                        Projectile.NewProjectile(
                            projectile.GetSource_FromAI(),
                            projectile.Center,
                            motion,
                            OrnamentVariants[Main.rand.Next(OrnamentVariants.Length)],
                            projectile.damage / 2,
                            projectile.knockBack / 4,
                            projectile.owner
                        );
                    }
                }
            }
            if (ChristmasScroll3 && IsAThrownScepter)
            {
                if (Main.rand.NextBool(13))
                {
                    Opus.RadialSpreadProjectile(ProjectileID.Blizzard, 4, projectile.Center, (int)(projectile.damage * 1.75f), (int)projectile.knockBack, 12);
                }
            }
            if (IchorScroll && IsAThrownScepter)
            {
                for (int y = 0; y < 26; y++)
                {
                    Vector2 Outer = projectile.Center + Main.rand.NextVector2CircularEdge(400, 400);
                    Dust.NewDustPerfect(Outer, DustID.IchorTorch, projectile.velocity, 0, default, 1.5f);
                }

                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && !npc.friendly && npc.Distance(projectile.Center) < 400)
                    {
                        npc.AddBuff(BuffID.Ichor, 600);
                    }
                }
            }
            if (CursedFlameScroll && IsAThrownScepter)
            {
                for (int y = 0; y < 26; y++)
                {
                    Vector2 Outer = projectile.Center + Main.rand.NextVector2CircularEdge(400, 400);
                    Dust.NewDustPerfect(Outer, DustID.CursedTorch, projectile.velocity, 0, default, 1.5f);
                }

                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && !npc.friendly && npc.Distance(projectile.Center) < 400)
                    {
                        npc.AddBuff(BuffID.CursedInferno, 600);
                    }
                }
            }
            if (TemporalGlove && IsAThrownScepter)
            {
                for (int y = 0; y < 26; y++)
                {
                    Vector2 Outer = projectile.Center + Main.rand.NextVector2CircularEdge(300, 300);
                    Dust.NewDustPerfect(Outer, DustID.MagicMirror, projectile.velocity, 0, default, 1.5f);
                }

                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && !npc.friendly && npc.Distance(projectile.Center) < 400)
                    {
                        npc.velocity *= 0.90f;
                    }
                }
            }
            if (SpookyScroll1 && IsAThrownScepter)
            {
                for (int y = 0; y < 26; y++)
                {
                    Vector2 Outer = projectile.Center + Main.rand.NextVector2CircularEdge(300, 300);
                    Dust.NewDustPerfect(Outer, DustID.Torch, projectile.velocity, 0, default, 1.5f);
                }

                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && !npc.friendly && npc.Distance(projectile.Center) < 400)
                    {
                        npc.AddBuff(BuffID.OnFire3, 600);
                    }
                }

                if (!Flag1)
                {
                    for (int f = 0; f < 6; f++)
                    {
                        Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, projectile.velocity, ModContent.ProjectileType<SpookyFirewood>(), (int)(projectile.damage * 0.65f), 4, projectile.owner, projectile.whoAmI);
                    }
                    Flag1 = true;
                }
                
            }
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(projectile, target, hit, damageDone);
            if (IsScepterClassButNotThrown && DiabolicScroll)
            {
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), projectile.Center, Vector2.Zero, ProjectileID.InfernoFriendlyBlast, (int)(projectile.damage * 0.75f), 2, projectile.owner);
            }
        }

    }

    public class ScrollDrops : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);
            OpusNPCDropHelper DH = new OpusNPCDropHelper();


            if (DH.MoltenLegionEnemiesExclusive.Contains(npc.type))
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IncendiaryScroll>(), 8, 1, 1));
            }
            if (DH.DiabolicFactionEnemiesExclusive.Contains(npc.type))
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DiabolicScroll>(), 8, 1, 1));
            }
            if (DH.RustedCompanyEnemiesExclusive.Contains(npc.type))
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustyPendant>(), 8, 1, 1));
            }
            if (DH.MarchingBonesFactionEnemies.Contains(npc.type))
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TemporalGlove>(), 8, 1, 1));
            }
            if (DH.NecromanticFactionEnemies.Contains(npc.type))
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CurseScroll>(), 8, 1, 1));
            }
        }

        
    }
}