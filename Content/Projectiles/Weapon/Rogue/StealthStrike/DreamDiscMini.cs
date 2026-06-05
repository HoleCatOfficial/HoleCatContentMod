using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue.StealthStrike
{
    public class DreamDiscMini : ModProjectile, IHomingProjectile
    {

        public ref float DelayTimer => ref Projectile.ai[1];

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 7f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 1f;

        float IHomingProjectile.DetectRadius => 1200;

        bool IHomingProjectile.CanHome => DelayTimer >= 10;

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 10;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;

            Projectile.DamageType = ModContent.GetInstance<DTRogueClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D value = ModContent.Request<Texture2D>(Texture + "_Glow", ReLogic.Content.AssetRequestMode.AsyncLoad).Value;
            float num = Opus.Sine(0f, 0.6f, 1f);
            SpriteBatch spriteBatch = Main.spriteBatch;
            SpriteEffects effects = SpriteEffects.None;
            if (Projectile.direction == -1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Vector2 vector = new Vector2((float)value.Width * 0.5f, (float)Projectile.height * 0.5f);
            for (int num2 = Projectile.oldPos.Length - 1; num2 > 0; num2--)
            {
                Vector2 position = Projectile.oldPos[num2] - Main.screenPosition + vector + new Vector2(0f, Projectile.gfxOffY);
                Color color = Main.DiscoColor * ((float)(Projectile.oldPos.Length - num2) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(value, position, null, color, Projectile.rotation, vector, Projectile.scale, effects);
            }

            Opus.ReturnToDefaultDrawing(spriteBatch);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2f, Projectile.scale, effects);
            return false;
        }

        public override void AI()
        {
            Projectile.rotation += 0.55f * (float)Projectile.direction;
            Lighting.AddLight(Projectile.Center, Main.DiscoColor.ToVector3() * 0.05f);

            if (DelayTimer < 10)
            {
                DelayTimer += 1;
                return;
            }

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.ShortShine, target.Center);
        }
    }
}
