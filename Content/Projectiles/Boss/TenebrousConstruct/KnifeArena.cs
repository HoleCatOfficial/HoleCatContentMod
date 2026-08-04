using BreadLibrary.Core;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.TenebrousConstruct
{
    public class KnifeArena : ModProjectile, IDrawPixelated
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1400;
            //Projectile.timeLeft = 70000;
            Projectile.tileCollide = false;
        }


        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveTiles;

        float R;
        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            R += 0.04f;

            var Cap = spriteBatch.Capture();
            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.End();
            spriteBatch.Begin(Cap);

            //Main.EntitySpriteDraw(DTAssetLib.Square.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, 0f, DTAssetLib.Square.Value.Size() / 2, 1f, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(DTAssetLib.Circle.Value, Projectile.Center - Main.screenPosition, null, Color.Black, 0f, DTAssetLib.Circle.Value.Size() / 2, DTAssetLib.Circle.Value.ScaleRingTextureToMatchRadius(301f, 300), SpriteEffects.None, 0);

            Main.EntitySpriteDraw(DTAssetLib.BarrierRing.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, R, DTAssetLib.BarrierRing.Value.Size() / 2, DTAssetLib.BarrierRing.Value.ScaleRingTextureToMatchRadius(300f, 1300), SpriteEffects.None, 0);

            if (Main.GameUpdateCount % 10 == 0)
            {
                BloomRingSharp Ring = new();
                Ring.Prepare(Projectile.Center, Vector2.Zero, ColorLib.TenebrisGradient, 0.1f, 0.01f, 1.25f, BlendState.Additive);
                ParticleEngine.Particles.Add(Ring);
            }

            spriteBatch.ResetToDefault();
        }

        SlotId LoopSlot;
        public SoundStyle Loop = DTAssetLib.LoopedSounds.ShadesRevenge with
        {
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };

        bool F1 = false;
        public override void AI()
        {
            Player player = Main.player[(int)Projectile.ai[0]];
            if (!F1)
            {
                SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Activate, Projectile.Center);
                F1 = true;
            }

            if (player.Center.Distance(Projectile.Center) > 300 && player.Center.Distance(Projectile.Center) > 0.1f)
            {
                player.Center = Projectile.Center + new Vector2(290, 0).RotatedBy(player.DirectionFrom(Projectile.Center).ToRotation());
            }
            else
            {
                if (player.Center.Distance(Projectile.Center) > 0.1f)
                {
                    player.velocity += player.Center.DirectionTo(Projectile.Center) * 0.5f;
                }
            }

            if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound))
            {
                var tracker = new ProjectileAudioTracker(Projectile);
                LoopSlot = SoundEngine.PlaySound(Loop, Projectile.Center, soundInstance => {
                    //soundInstance.Position = Projectile.Center;
                    return tracker.IsActiveAndInGame();
                });

            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 P = Projectile.Center + Main.rand.NextVector2CircularEdge(200, 200);
                Dust.NewDustPerfect(P, DustID.FireworksRGB, P.DirectionTo(Projectile.Center) * 14, newColor: Color.White).noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item30);
            foreach (Dust dust in Opus.RingSpreadDust(DustID.FireworksRGB, 40, Projectile.Center, 300f, 0, Color.White, 1f, 8f))
            {
                dust.noGravity = true;
            }
        }
    }
}
