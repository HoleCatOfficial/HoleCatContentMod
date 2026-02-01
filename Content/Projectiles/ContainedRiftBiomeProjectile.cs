using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using Terraria.Audio;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.RiftBiome;
using ReLogic.Utilities;

namespace DestroyerTest.Content.Projectiles
{
    public class ContainedRiftBiomeProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360 * 4;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public bool InRange = false;

        SlotId LoopSlot;
        public SoundStyle Loop = new SoundStyle("DestroyerTest/Assets/Audio/AuraLoop/ElectricLoop1") 
        { 
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };
        

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Length() * 0.01f;
            Projectile.velocity *= 0.98f;

            if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound)) {
                var tracker = new ProjectileAudioTracker(Projectile);
                LoopSlot = SoundEngine.PlaySound(Loop, Projectile.Center, soundInstance => {
                    soundInstance.Position = Projectile.Center;
                    return tracker.IsActiveAndInGame();
                });
            }
            else
            {
                activeSound.Position = Projectile.Center;
            }

            Vector2[] DustPositions = Opus.GetEquidistantOrbitVectors(5, Projectile.Center, 0.1f, 1500f);

            foreach (Vector2 dPos in DustPositions)
            {
                Vector2 Inward = Projectile.Center - dPos;
                Inward.Normalize();
                Dust EdgeDust = Dust.NewDustPerfect(dPos, ModContent.DustType<ColorableNeonDust>(), Inward, 0, ColorLib.Rift, 1.2f);
            }

            foreach(Player player in Main.player)
            {
                if (player.active && Vector2.Distance(player.Center, Projectile.Center) < 150f)
                {
                    InRange = true;
                }
                else
                {
                    InRange = false;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Opus.DrawProjectileShadowsRotating(Projectile, 4, ColorLib.Rift);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}