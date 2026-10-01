
using System;
using System.Collections.Generic;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Classless
{
    public class SoulTriangle : ModProjectile, IHomingProjectile
    {

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 7f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.07f;

        float IHomingProjectile.HomingMaxAccel => 40f;

        float IHomingProjectile.DetectRadius => 4800;

        bool IHomingProjectile.CanHome => DelayTimer >= 30;

        public float DelayTimer;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            DTUtils.ThrowerProjectilesThatCantTriggerEquipEffects[Type] = true;
        }

        public int variant;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;

        }
        public override bool PreDraw(ref Color lightColor)
        {

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            DTUtils Utility = new DTUtils();

            Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, projectileTexture.Size() / 2, Projectile.scale, SpriteEffects.None);

            return false;
        }

        public override void AI()
        {

            DelayTimer++;


            Projectile.rotation += (0.07f) * Projectile.direction;

            if (Projectile.timeLeft % 15 == 0)
            {
                SoulTriangleTrail trail = new();
                trail.Initiate(Projectile.Center, Projectile.rotation, Color.White, Projectile.scale, 30);
                ParticleEngine.BehindProjectiles.Add(trail);
            }


            Lighting.AddLight(Projectile.Center, ColorLib.Soul.ToVector3());


            if (DelayTimer < 30)
            {
                Projectile.velocity *= 0.9f;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 30;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SoulInferno>(), 300);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                PointGlowPreMultiplied glow = new();
                glow.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Main.rand.NextVector2Circular(8f, 8f), ColorLib.Soul, 2f);
                ParticleEngine.Particles.Add(glow);
            }
        }
    }

    public class SoulTriangleTrail : BaseParticle<SoulTriangleTrail>
    {
        public int maxLifetime = 120;
        public int Lifetime = 0;
        public Color col;
        public float scale = 1f;
        private float InitScale = 1f;
        public Vector2 position;
        public float rotation;

        private float Opacity = 1f;

        public void Initiate(Vector2 Position, float Rotation, Color color, float Scale, int MaxLifetime)
        {
            this.position = Position;
            this.rotation = Rotation;
            this.scale = Scale;
            this.InitScale = Scale;
            this.maxLifetime = MaxLifetime;
            this.Lifetime = MaxLifetime;
            this.col = color;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            Lifetime--;


            float progress = (float)Lifetime / (float)(maxLifetime);
            Opacity = MathHelper.Lerp(0f, 1f, progress);
            //scale = MathHelper.Lerp(InitScale, InitScale * 0.5f, progress);


            if (Lifetime <= 0)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public Tuple<Texture2D, Rectangle, Vector2> GetTextureProperties()
        {
            Texture2D TexValue = ModContent.Request<Texture2D>($"DestroyerTest/Content/Projectiles/Weapon/Classless/SoulTriangle_Trail").Value;
            Rectangle frameRect = new Rectangle(0, 0, TexValue.Width, TexValue.Height);

            Vector2 origin = new Vector2(TexValue.Width / 2f, TexValue.Height - 8);

            return new Tuple<Texture2D, Rectangle, Vector2>(TexValue, frameRect, origin);
        }

        public override PixelLayer DefaultPixelLayer => PixelLayer.AbovePlayer;

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            Opus.StartSpriteBatchWithBlending(spritebatch, BlendState.AlphaBlend, SpriteSortMode.Deferred);
            spritebatch.Draw(GetTextureProperties().Item1, position - Main.screenPosition, GetTextureProperties().Item2, col with { A = 0 } * Opacity, rotation, GetTextureProperties().Item3, new Vector2(scale, scale), SpriteEffects.None, 0f);
            Opus.ReturnToDefaultDrawing(spritebatch);
        }
    }
}