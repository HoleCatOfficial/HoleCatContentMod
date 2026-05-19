using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Equips;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class ToxicOrb : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = Projectile.oldPos.Length - 1; k > 0; k--) {
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}
            return true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!Validate(owner))
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 120;

            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ToxicBubble, 0f, 0f, 100, default, 1.2f);
        }

        private bool Validate(Player owner)
        {
            foreach(Item i in owner.armor)
            {
                if (i.type == ModContent.ItemType<ToxicCanister>())
                {
                    return true;
                }
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.Hitbox.Inflate(10, 10);
            Opus.RadialSpreadDustRandom(DustID.ToxicBubble, 16, Projectile.Center, 100, default, 2f, 2);
        }
    }
}