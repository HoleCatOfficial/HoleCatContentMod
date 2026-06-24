using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using FargowiltasSouls.Content.Patreon.DanielTheRobot;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class HeliosBeam : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        float WidthScl = 0f;
        Line L;
        int oF = 0;
        float Rot = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            oF -= 30;
            L = new Line(Projectile.Center, Projectile.Center + new Vector2(2000, 0).RotatedBy(Projectile.rotation));

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(3), ColorLib.Rift, Main.spriteBatch, BlendState.Additive, oF, WidthScl * 2);
            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, ColorLib.Rift, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl * 0.75f), SpriteEffects.None);
            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(5), Color.White, Main.spriteBatch, BlendState.Additive, oF, WidthScl, 5f);
            //Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl * 0.15f), SpriteEffects.None);

            Rot += 0.3f;

            Main.EntitySpriteDraw(DTAssetLib.Star(3).Value, Projectile.Center - Main.screenPosition, null, ColorLib.Rift with { A = 0 }, Rot, DTAssetLib.Star(3).Value.Size() / 2, 1.4f, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.Star(3).Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Rot, DTAssetLib.Star(3).Value.Size() / 2, 1f, SpriteEffects.None);

            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.ai[0];
        }

        public bool GoodBeam = false;

        SlotId LoopSlot;

        public SoundStyle Loop = new SoundStyle("DestroyerTest/Assets/Audio/AuraLoop/ElectricHum")
        {
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };

        public override void AI()
        {
            Projectile.rotation = Projectile.ai[0];
            UpdateSound();

            float length = 2000f; // however long your laser should be

            Vector2 start = Projectile.Center;

            Vector2 end = start + new Vector2(Main.rand.NextFloat(length), 0).RotatedBy(Projectile.rotation);
            if (Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<HeliciteStaffHoldout>()] < 1)
            {
                GoodBeam = false;
            }

            if (GoodBeam)
            {
                if (WidthScl < 0.5f)
                {
                    WidthScl += 0.04f;
                }
                Projectile.timeLeft = 180;
            }
            else
            {
                if (WidthScl > 0)
                {
                    WidthScl -= 0.04f;
                }
                else
                {
                    Projectile.timeLeft = 0;
                }
            }
        }

        void UpdateSound()
        {
            if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound))
            {
                var tracker = new ProjectileAudioTracker(Projectile);
                LoopSlot = SoundEngine.PlaySound(Loop, Projectile.Center, soundInstance => {
                    soundInstance.Position = Projectile.Center;
                    soundInstance.Pitch = WidthScl;
                    return tracker.IsActiveAndInGame();
                });
            }
            else
            {
                activeSound.Position = Projectile.Center;
                activeSound.Pitch = WidthScl * 0.15f;
            }
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float length = 2000f; // however long your laser should be

            Vector2 start = Projectile.Center;

            Vector2 S = Projectile.velocity;
            Vector2 end = start + new Vector2(length, 0).RotatedBy(Projectile.rotation);

            float collisionPoint = 0f;

            float beamWidth = 30f * WidthScl; // scale this how you want

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, beamWidth, ref collisionPoint) && GoodBeam;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<DaylightOverload>(), 240);
        }
    }

}
