using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Vanilla
{

    public class BeamSwordSwing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 52;
            Projectile.height = 52;
            SweepColor = Color.Blue;
            SweepHighlightColor = Color.Teal;
            WaitTimeMultiplier = 0.6f;
            SwingSpeed = 0.2f;
            UsesDefaultSweepFX = true;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.MediumHeavySwing with { PitchVariance = 0.6f, Pitch = 0.4f };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            
        }

        public override void DrawUnderBlade()
        {

        }
        public override void DrawOverBlade()
        {

        }

        public override void OnStartSwing()
        {
            Vector2 toMouse = Main.MouseWorld - Projectile.Center;
            toMouse.Normalize();

            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (toMouse * (22 * Owner.GetTotalAttackSpeed(DamageClass.Melee))), ModContent.ProjectileType<SwordBeam>(), Projectile.damage, 5, Projectile.owner);
        }

        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {

            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);
            Vector2[] pt = SwordLine.GetPointsAlongLine(30);
            Vector2[] ppt = pt[10..30];

            if (Main.rand.NextBool(2))
            {
                for (int i = 0; i < 2; i++)
                {
                    PixelParticle pixel = new();
                    pixel.Initialize(ppt[Main.rand.Next(15)], (SwordLine.GetLineRotation - MathHelper.PiOver2 * Owner.direction).ToRotationVector2(), Color.Goldenrod, 2f);
                    ParticleEngine.Particles.Add(pixel);
                }
            }

            ScaleMult = 1.6f;
        }
    }
}