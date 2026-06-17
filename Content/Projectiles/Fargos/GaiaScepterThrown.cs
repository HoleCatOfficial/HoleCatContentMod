using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using System.IO;
using DestroyerTest.Content.Projectiles.ParentClasses;
using OpusLib;
using DestroyerTest.Content.Projectiles.Weapon.Melee;

namespace DestroyerTest.Content.Projectiles.Fargos
{
    [JITWhenModsEnabled(DTCrossMod.FargosSoulsName)]
    public class GaiaScepterThrown : ModProjectile
    {
        
        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60 * 32;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, projectileTexture.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public int AITimer = 0;
        public override void AI()
        {
            Projectile.rotation += (Projectile.velocity.Length() * 0.03f) * Projectile.direction;
            ThrownScepter s = ModContent.GetInstance<ThrownScepter>();

            if (Main.rand.NextBool(3) && !s.ArmorSetHelper_AetherianShimmerEffects)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Projectile.velocity * 0.2f, 100, default, 1.2f);
                dust.noGravity = true;
            }
            else if (Main.rand.NextBool(3) && s.ArmorSetHelper_AetherianShimmerEffects)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.TintableDustLighted, Projectile.velocity * 0.2f, 100, DTColorUtils.Pastel(Main.DiscoColor, 0.4f), 1.2f);
                dust.noGravity = true;
            }

            AITimer++;

            var Positions = Opus.GetEquidistantOrbitVectors(7, Projectile.Center, 0.5f, 400);

            for (int i = 0; i < Positions.Length; i++)
            {
                Dust.NewDustPerfect(Positions[i], ModContent.DustType<ColorableNeonDust>(), Vector2.Zero, 0, Main.DiscoColor, 1f);
                Dust.NewDustPerfect(Positions[i], ModContent.DustType<ColorableNeonDust>(), Vector2.Zero, 0, Main.DiscoColor * 0.25f, 2f);
            }

            if (AITimer < 80)
            {
                Projectile.timeLeft = (60 * 32) - 40;
            }
            if (AITimer >= 80)
            {
                Projectile.velocity *= 0.82f;
                Projectile.rotation += 0.01f * Projectile.direction;

                if (AITimer % 20 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item30, Projectile.Center);
                    Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<GaiaOrb>(), 1, Projectile.Center, 100, 20, 10);
                }
            }
            EnchantmentVisuals();
        }

        public virtual Rectangle EnchantmentVisuals(int Width = 16, int Height = 16)
        {
            Rectangle hitbox = Projectile.Hitbox;
            Vector2 localOffset = new Vector2(
                (hitbox.Width / 2f) - (Width / 2f),
                -(hitbox.Height / 2f) + (Height / 2f)
            );
            Vector2 rotatedOffset = localOffset.RotatedBy(Projectile.rotation);

            Vector2 rectCenter = Projectile.Center + rotatedOffset;

            return new Rectangle(
                (int)(rectCenter.X - Width / 2f),
                (int)(rectCenter.Y - Height / 2f),
                Width,
                Height
            );
        }
    }

    public class GaiaScepterAura : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player)
        {
            bool t = false;
            foreach (Projectile scepter in Main.projectile)
            {
                if (scepter.active && player.Center.Distance(scepter.Center) < 400 && scepter.type == ModContent.ProjectileType<GaiaScepterThrown>())
                {
                    t = true;
                }
            }
            return t;
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override int Music => MusicLoader.GetMusicSlot("DestroyerTest/Assets/Music/GaiaAmbience");
    }
}

