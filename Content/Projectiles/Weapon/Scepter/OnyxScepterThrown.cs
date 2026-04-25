using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using Terraria.GameContent.Drawing;
using System.IO;
using DestroyerTest.Content.Projectiles.ParentClasses;
using OpusLib;
using ReLogic.Utilities;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class OnyxScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = new Color(179, 54, 201);
            WidthDim = 40;
            HeightDim = 40;
            DustType = DustID.WaterCandle;
            base.SetDefaults();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Opus.DrawTextureOnProj(DTAssetLib.Swirl, Projectile,  new Color(179, 54, 201), true, Projectile.rotation, 0.5f, 0.5f);
            Opus.DrawTextureOnProj(DTAssetLib.FireRing, Projectile,  new Color(204, 121, 219) * 0.85f, false, -Projectile.rotation, 0.0625f, 0.0625f);
            Opus.DrawTextureOnProj(DTAssetLib.FireRing, Projectile,  new Color(179, 54, 201) * 0.85f, false, Projectile.rotation, 0.05f, 0.05f);
            Opus.ReturnToDefaultDrawing(spriteBatch);
            return base.PreDraw(ref lightColor);
        }
        public SlotId LoopSlot;
        public SoundStyle Loop = new SoundStyle("DestroyerTest/Assets/Audio/AuraLoop/ShadowflameAuraLoop") 
        { 
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };
        
        public float Vol = 0;
        public override void PostAI()
        {
            if (Vol < 1)
            {
                Vol += 0.1f;
            }
            if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound)) {
                var tracker = new ProjectileAudioTracker(Projectile);
                LoopSlot = SoundEngine.PlaySound(Loop, Projectile.position, soundInstance => {
                    soundInstance.Position = Projectile.position;
                    soundInstance.Volume = Vol;
                    return tracker.IsActiveAndInGame();
                });
            }

            //Opus.RingParticleOutward(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], 12, Projectile.Center, 20, 0.4f, new Color(179, 54, 201), 0.75f, 0.3f, 30, ai2: 2, RandomOffset: true);
            base.PostAI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 240);
            SoundEngine.PlaySound(SoundID.Item175, Projectile.position);
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.NightsEdge,
				new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) },
				Projectile.owner);
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}

