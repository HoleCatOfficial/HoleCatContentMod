
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{

    public class Endemy2Swing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 140;
            Projectile.height = 142;
            SweepColor = Color.Goldenrod;
            SweepHighlightColor = Color.Bisque;
            UsesDefaultSweepFX = true;
            SweepScale = 2.6f;

            WaitTimeMultiplier = 1.3f;
            SwingSpeed = 0.15f;


            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Highlight");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.BigBasicSwing;

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            npc.AddBuff(BuffID.BrokenArmor, 300);
            SoundEngine.PlaySound(DTAssetLib.Impacts.IceImpact with { MaxInstances = 0, PitchVariance = 0.4f, Pitch = -0.7f }, npc.Center);
            BloomRingSharp Ring = new();
            Ring.Prepare(npc.Center, Vector2.Zero, Color.Goldenrod, 0.07f, 0.01f, 1f, BlendState.Additive);
            ParticleEngine.ShaderParticles.Add(Ring);
        }


        public override void DrawUnderBlade()
        {

        }
        public override void DrawOverBlade()
        {

        }

        public override void ExtraEffects()
        {



        }
    }
}
