using System.Formats.Tar;
using System.Linq;
using System.Runtime.CompilerServices;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class CurseProjectile : ModProjectile, IHomingProjectile, IDrawPixelated
    {
        enum curseType
        {
            Hellfire,
            Shadowflame,
            SpiritDrift
        }

        curseType CurseType;

        public ref float DelayTimer => ref Projectile.ai[1];

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 4f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingMaxAccel => 0f;

        float IHomingProjectile.DetectRadius => 2000f;

        bool IHomingProjectile.CanHome => Timer > 120;

        int Timer = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 70;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.hide = true;
        }

        public PixelLayer PixelLayer => PixelLayer.AboveProjectiles;
        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            Texture2D texture = DTAssetLib.CurseSigilRing.Value;
            Texture2D SparkTex = DTAssetLib.MiscSparkle144.Value;
            Vector2 origin = texture.Size() / 2f;
            Vector2 SparkOrigin = SparkTex.Size() / 2f;

            Opus.StartSpriteBatchPixelated(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            DTTrail.DrawTrail(spriteBatch, BlendState.AlphaBlend, DTAssetLib.Streak(Trailtype()).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 16, Col() with { A = 0 }, 2f);

            spriteBatch.Draw(SparkTex, Projectile.Center - Main.screenPosition, null, Col() with { A = 0 }, rot, SparkOrigin, Projectile.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Col() with { A = 0 }, rot, origin, 0.2f * Projectile.scale, SpriteEffects.None, 0f);
            
            Opus.ReturnToDefaultDrawing(spriteBatch);
        }

        int Trailtype()
        {
            switch (CurseType)
            {
                case curseType.Hellfire:
                    return 4;
                case curseType.Shadowflame:
                    return 2;
                case curseType.SpiritDrift:
                    return 8;
                default:
                    return 8;
            }
        }
        
        Color Col()
        {
            switch (CurseType)
            {
                case curseType.Hellfire:
                    return Color.OrangeRed;
                case curseType.Shadowflame:
                    return Color.Purple;
                case curseType.SpiritDrift:
                    return Color.CadetBlue;
                default:
                    return Color.White;
            }
        }

        float rot = 0f;
        public override void AI()
        {
            Timer++;

            rot += 0.1f;

        }
	}
}