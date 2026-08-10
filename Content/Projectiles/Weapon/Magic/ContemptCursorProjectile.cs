using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Magic;
 
using DestroyerTest.Content.Particles;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Utilities;
using System.Text;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class ContemptCursorProjectile : ModProjectile, IDrawPixelated
    {



        public override void SetStaticDefaults()
        {
            
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.penetrate = -1; // Infinite pierce
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180; // 10 seconds max lifespan
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;

            

            return false;
        }

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveProjectiles;

        bool IDrawPixelated.ShouldDrawPixelated => true;

        float r = 0;

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            r += 0.2f;
            Texture2D glowTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CorruptSigil").Value;

            Main.EntitySpriteDraw(
                glowTexture,
                Projectile.Center - Main.screenPosition,
                null,
                ColorLib.CursedFlames with { A = 0 } * Projectile.Opacity,
                Projectile.rotation,
                glowTexture.Size() / 2,
                Projectile.scale * 0.4f,
                SpriteEffects.None,
                0
            );

            float RingScale =  DTAssetLib.NightmareRoseArenaBorder.Value.ScaleRingTextureToMatchRadius(rad, 1327);
            Main.EntitySpriteDraw(DTAssetLib.NightmareRoseArenaBorder.Value, Projectile.Center - Main.screenPosition, null, ColorLib.CursedFlames with { A = 0 } * Projectile.Opacity, r, DTAssetLib.NightmareRoseArenaBorder.Value.Size() / 2, RingScale, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(DTAssetLib.NightmareRoseArenaBorder.Value, Projectile.Center - Main.screenPosition, null, OpusColorUtils.Pastel(ColorLib.CursedFlames, 0.75f) with { A = 0 } * Projectile.Opacity, r, DTAssetLib.NightmareRoseArenaBorder.Value.Size() / 2, RingScale, SpriteEffects.FlipHorizontally, 0);
        }



        public bool Good = false;

        SlotId LoopSlot;
        public SoundStyle Loop = new SoundStyle("DestroyerTest/Assets/Audio/AuraLoop/HateLaser")
        {
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };
        float P = 0f;

        float rad = 500f;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound))
            {
                var tracker = new ProjectileAudioTracker(Projectile);
                LoopSlot = SoundEngine.PlaySound(Loop, Projectile.Center, soundInstance => {
                    soundInstance.Position = Projectile.Center;
                    return tracker.IsActiveAndInGame();
                });
            }
            else
            {
                activeSound.Position = Projectile.Center;
                activeSound.Pitch = P;
            }

            Projectile.Center = Main.MouseWorld;


            if (player.HeldItem.type == ModContent.ItemType<Contempt>() && player.controlUseItem)
            {
                Good = true;
                
                


                



            }
            else
            {
                Good = false;
            }


            if (Good)
            {
                Projectile.timeLeft = 120;
                for (int i = 0; i < 5; i++)
                {
                    Vector2 P = Projectile.Center + Main.rand.NextVector2CircularEdge(rad, rad);

                    PointGlowPreMultiplied Border = new();
                    Border.Initialize(P, Vector2.Zero, ColorLib.Wretched3, 0.6f);
                    ParticleEngine.BehindProjectiles.Add(Border);
                }
            }
            else
            {
                Projectile.Opacity = MathHelper.Lerp(1f, 0f, ((float)Projectile.timeLeft / 120f).Inverse());

                P = MathHelper.Lerp(0f, -0.8f, ((float)Projectile.timeLeft / 120f).Inverse());
            }


        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utilities.CircularHitboxCollision(Projectile.Center, rad, targetHitbox);
        }




        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Defilement>(), 180);
           
        }

       

    }

}

