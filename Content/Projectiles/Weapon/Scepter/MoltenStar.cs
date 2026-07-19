using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class MoltenStar : ModProjectile, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 8f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.03f;

        float IHomingProjectile.HomingMaxAccel => 32f;

        float IHomingProjectile.DetectRadius => 15000;

        bool IHomingProjectile.CanHome => Timer >= 30;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }



        Color C;
        float SC = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            SC += 0.01f;
            Vector2[] AuraDrawPositions = Opus.GetEquidistantOrbitVectors(5, Projectile.Center, 0.04f, 10);
            Vector2 AuraOrigin = DTAssetLib.StarAura.Size() / 2 + new Vector2(-5f, 0f);

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < AuraDrawPositions.Length; i++)
            {
                Main.EntitySpriteDraw(DTAssetLib.StarAura.Value, AuraDrawPositions[i] - Main.screenPosition, null, OpusColorUtils.Pastel(Color.OrangeRed, 0.1f) * 0.5f, Projectile.rotation, AuraOrigin, Projectile.scale * 1.4f, SpriteEffects.None, 0f);
            }



            DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Streak(8, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 20, OpusColorUtils.Pastel(Color.OrangeRed, 0.1f), SC, Projectile.oldPos.Length);

            Main.spriteBatch.UseBlendState(BlendState.Additive);

            Main.EntitySpriteDraw(DTAssetLib.StarAura.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, AuraOrigin, Projectile.scale * 1f, SpriteEffects.None, 0f);

           

            Main.EntitySpriteDraw(DTAssetLib.ColorlessStar.Value, Projectile.Center - Main.screenPosition, null, Color.White, STRot, DTAssetLib.ColorlessStar.Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.ResetToDefault();
            return false;
        }

        int Timer = 0;
        float STRot = 0f;
        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            STRot += 0.2f;

            if (Main.rand.NextBool(3))
            {
                Dust D1 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                D1.velocity = Projectile.velocity * 0.15f;
                D1.fadeIn = 0f;
            }


            if (Timer % 30 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item9 with { Pitch = -0.4f, pitchVariance = 0.1f }, Projectile.Center);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            target.AddBuff(BuffID.OnFire, 120);
        }
    }
}
