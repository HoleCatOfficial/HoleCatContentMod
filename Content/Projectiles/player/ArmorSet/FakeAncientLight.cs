using System;
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using System.Linq;
using DestroyerTest.Common.Interfaces;
using BreadLibrary.Core.Utilities;

namespace DestroyerTest.Content.Projectiles.player.ArmorSet
{
    public class FakeAncientLight : ModProjectile, IHomingProjectile
    {
        public ref float DelayTimer => ref Projectile.ai[1];

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 12.5f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 1f;

        float IHomingProjectile.DetectRadius => 1400;

        bool IHomingProjectile.CanHome => DelayTimer >= 10;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 90;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 33;
            Projectile.height = 33;

            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
        }

        public float trailOffset = 0f;

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.SkyBlue;
            trailOffset += 0.04f;


            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(1, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 30, lightColor, trailOffset, 2f);

            spriteBatch.UseBlendState(BlendState.Additive);

            Opus.DrawGlowOnProj(Projectile, lightColor, true);

            Opus.ReturnToDefaultDrawing(spriteBatch);

            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, Color.SkyBlue, true, 0f, 0.9f, 0.9f);

            return false;
        }

        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();

            Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3() * 0.2f);

            if (DelayTimer < 10)
            {
                DelayTimer += 1;
                return;
            }


        }
    }
}