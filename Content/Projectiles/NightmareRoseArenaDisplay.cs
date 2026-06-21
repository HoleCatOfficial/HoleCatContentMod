using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class NightmareRoseArenaDisplay : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1200;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var Tex = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/NightmareRoseArenaIndicator", AssetRequestMode.AsyncLoad).Value;

            if (Tex != null)
            {
                Main.EntitySpriteDraw(Tex, Projectile.Bottom - Main.screenPosition, null, (ColorLib.WretchedGradient() * 0.5f) * Projectile.Opacity, 0f, new Vector2(Tex.Width / 2, Tex.Height), 1f, SpriteEffects.None, 0f);
            }
            return false;
        }

        //Baby I'm not even here.... Im a hallucination...
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Projectile.Bottom = FindGround().ToWorldCoordinates();

            IndicateArena();

            if (Projectile.timeLeft < 60)
            {
                Projectile.Opacity = MathHelper.Lerp(1f, 0f, ((float)Projectile.timeLeft / 60f).Inverse());
            }    
        }

        private void IndicateArena()
        {
            float Rad = 1200f;
            Vector2 Head = Projectile.Center + new Vector2(0, -79);
            Vector2[] P = Opus.GetEquidistantOrbitVectors(24, Head, 0.003f, Rad);

            for (int i = 0; i < P.Length; i++)
            {
                Dust A = Dust.NewDustPerfect(P[i], DustID.CursedTorch, Vector2.Zero, 0, default, 1f);
                A.noGravity = true;
            }
        }

        public Point FindGround()
        {
            Vector2 pos = Projectile.Bottom;
            Point posCoord = pos.ToTileCoordinates();

            for(int i = posCoord.Y; i < Main.maxTilesY; i++)
{
                Tile check = Framing.GetTileSafely(posCoord.X, i);

                if (check.HasTile &&
                    check.HasUnactuatedTile &&
                    Main.tileSolid[check.TileType])
                {
                    return new Point(posCoord.X, i);
                }
            }

            return Point.Zero;
        }

    }
}
