
using System;
using System.Collections.Generic;
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
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class CursedHammerBolt : ModProjectile, IHomingProjectile
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
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }

        public int variant;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.DamageType = DamageClass.Throwing;
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

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float prog = (float)i / (float)Projectile.oldPos.Length;
                float opacity = MathHelper.Lerp(1f, 0f, prog);
                float scale = MathHelper.Lerp(Projectile.scale, 0f, prog);
                Main.EntitySpriteDraw(projectileTexture, Projectile.OldCenter()[i] - Main.screenPosition, null, OpusColorUtils.MultiLerp(prog, ColorLib.WretchedColorMap) with { A = 0 } * opacity, Projectile.oldRot[i], projectileTexture.Size() / 2, scale, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, null, ColorLib.Wretched1 with { A = 0 }, Projectile.rotation, projectileTexture.Size() / 2, Projectile.scale, SpriteEffects.None);

            return false;
        }

        public override void AI()
        {
           
            DelayTimer++;
           

            Projectile.rotation = Projectile.velocity.ToRotation();

   

            Lighting.AddLight(Projectile.Center, ColorLib.Wretched2.ToVector3());

           
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
            SoundEngine.PlaySound(SoundID.Item74, target.Center);
            target.AddBuff(ModContent.BuffType<Defilement>(), 300);
        }

    }

}