using BreadLibrary.Core.Graphics.PixelationShit;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;
using ReLogic.Peripherals.RGB;
using System;
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
	public class ConstantineScytheProjectile : BaseBroadswordProjectile
	{
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 94;
            Projectile.height = 102;
            SweepColor = Color.Magenta;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
            SwingSpeed = 0.23f;
        }

        public override SoundStyle Swing => new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionT3Slash") with { MaxInstances = 0, PitchVariance = 0.6f };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.ShortShine with { MaxInstances = 0, PitchVariance = 0.4f }, npc.Center);
            if (Owner.direction > 0)
            {
                if (LastSwing == -1)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), npc.Center, new Vector2(0, Main.rand.NextFloat(2f, 6f)).RotatedByRandom(0.1f), Color.Red * Main.rand.NextFloat(0.01f, 0.3f), 1f);
                    }
                }
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), npc.Center, new Vector2(0, Main.rand.NextFloat(-6f, -2f)).RotatedByRandom(0.1f), Color.Red * Main.rand.NextFloat(0.01f, 0.3f), 1f);
                    }
                }
            }
            else
            {
                if (LastSwing == -1)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), npc.Center, new Vector2(0, Main.rand.NextFloat(-6f, -2f)).RotatedByRandom(0.1f), Color.Red * Main.rand.NextFloat(0.01f, 0.3f), 1f);
                    }
                }
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), npc.Center, new Vector2(0, Main.rand.NextFloat(2f, 6f)).RotatedByRandom(0.1f), Color.Red * Main.rand.NextFloat(0.01f, 0.3f), 1f);
                    }
                }
            }
        }

        public override void OnStartSwing()
        {
            Vector2 dir = Main.MouseWorld - Projectile.Center;
            dir.Normalize();

            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, dir * 24, ModContent.ProjectileType<ConstantineScytheClone>(), (int)(Projectile.damage *  0.75f), 3, Owner.whoAmI);
        }
        public override void DrawOverBlade()
        {
        }

        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length() - 20f) * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);

            ScaleMult = 1f;

            if (CurrentState == State.SwingDown)
            {
                
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticlePlayerLock>(), swordTip, new Vector2(1, 0).RotatedBy(SwordLine.GetLineRotation + MathHelper.PiOver2), Color.NavajoWhite, 1f, ai1: 2, ai2: Owner.whoAmI);
                
            }
            if (CurrentState == State.SwingUp)
            {
                
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticlePlayerLock>(), swordTip, new Vector2(1, 0).RotatedBy(SwordLine.GetLineRotation - MathHelper.PiOver2), Color.NavajoWhite, 1f, ai1: 2, ai2: Owner.whoAmI);
                
            }
        }
    }
}