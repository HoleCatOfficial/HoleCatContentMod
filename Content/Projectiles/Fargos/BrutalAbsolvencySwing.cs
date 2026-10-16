using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Fargos
{
    public class BrutalAbsolvencySwing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 104;
            Projectile.height = 104;
            SweepColor = Color.DeepSkyBlue;
            SweepHighlightColor = Color.SkyBlue;
            WaitTimeMultiplier = 3f;
            SwingSpeed = 0.17f;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.BigBasicSwing;

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(DTAssetLib.Charge.Anvil with { Pitch = -2.2f, PitchVariance = 0.2f, MaxInstances = 0, Volume = 1.5f }, npc.Center);
            SoundEngine.PlaySound(DTAssetLib.EnergyWoosh with { Pitch = -0.6f, PitchVariance = 0.2f, MaxInstances = 0 }, npc.Center);

            BloomRingSharp Ring = new();
            Ring.Prepare(npc.Center, Vector2.Zero, Color.DeepSkyBlue, 0.1f, 0.01f, 0.8f, BlendState.Additive);
            ParticleEngine.Particles.Add(Ring);
        }

        
        public override void DrawUnderBlade()
        {

        }
        public override void DrawOverBlade()
        {

        }

        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);

            //SparkEdge(Main.player[Projectile.owner], 1f, ColorLib.Wretched3);
        }
    }
}
