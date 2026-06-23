using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{

    public class MemoriamSwing : BaseBroadswordProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
           
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 112;
            Projectile.height = 112;
            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.MemoriamSwing with { MaxInstances = 0, PitchVariance = 0.3f };

        

        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);
            Vector2[] pt = SwordLine.GetPointsAlongLine(30);

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(pt[Main.rand.Next(30)], ModContent.DustType<ColorableNeonDust>(), SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, Color.GhostWhite, 1);
            }


            if (player.HeldItem.ModItem is Memoriam Memory)
            {

                foreach (Projectile proj in Main.projectile)
                {
                    if ((proj.hostile) && proj.active)
                    {
                        Vector2 start = player.MountedCenter;
                        Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
                        float collisionPoint = 0f;
                        Rectangle H = proj.Hitbox;

                        if (Collision.CheckAABBvLineCollision(H.TopLeft(), H.Size(), start, end, 15f * Projectile.scale, ref collisionPoint))
                        {
                            if (Memory.CanParry)
                            {
                                Parry(proj.Center);
                                Memory.ParryCooldown = Memoriam.MaxParryCooldown;

                                proj.velocity = -proj.velocity;
                                proj.friendly = true;

                                //Added after the initial demo video
                                Projectile.NewProjectile(Projectile.GetSource_Misc("MemoraiamParry"), proj.position, proj.velocity.RotatedByRandom(0.2f), ModContent.ProjectileType<SoulOfLight_Projectile>(), Projectile.damage / 2, 7, player.whoAmI);
                                Projectile.NewProjectile(Projectile.GetSource_Misc("MemoraiamParry"), proj.position, proj.velocity.RotatedByRandom(0.2f), ModContent.ProjectileType<SoulOfNight_Projectile>(), Projectile.damage / 2, 7, player.whoAmI);
                            }
                        }
                    }
                }
            }

            SparkEdge(Main.player[Projectile.owner], 1f, Color.NavajoWhite);
        }

        public void Parry(Vector2 Position)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.SpiritOfJusticeParry with { MaxInstances = 0, PitchVariance = 0.1f }, Projectile.Center);
            ParticleOrchestrator.RequestParticleSpawn(false, ParticleOrchestraType.Excalibur, new ParticleOrchestraSettings() { IndexOfPlayerWhoInvokedThis = (byte)Projectile.owner, PositionInWorld = Position });
        }

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            ParticleOrchestrator.RequestParticleSpawn(false, ParticleOrchestraType.PaladinsHammer, new ParticleOrchestraSettings() { IndexOfPlayerWhoInvokedThis = (byte)Projectile.owner, PositionInWorld = Main.rand.NextVector2FromRectangle(npc.Hitbox) });
            ParticleOrchestrator.RequestParticleSpawn(false, ParticleOrchestraType.Excalibur, new ParticleOrchestraSettings() { IndexOfPlayerWhoInvokedThis = (byte)Projectile.owner, PositionInWorld = Main.rand.NextVector2FromRectangle(npc.Hitbox) });
            SoundEngine.PlaySound(DTAssetLib.Impacts.FleshHit with { MaxInstances = 0, PitchVariance = 0.1f }, Projectile.Center);
        }
    }
}