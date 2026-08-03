using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
 
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
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class TrueSoulEdgeCursorProjectile : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
        }

        public static int HitCooldownGlobal = 20;
        private int HitCooldown = 0;

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.penetrate = -1; // Infinite pierce
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240; // 10 seconds max lifespan
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.tileCollide = false;
            
        }

        public int trailLength = 10;
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.DeepSkyBlue;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            SpriteEffects FX = SpriteEffects.None;

            if (Projectile.direction < 0)
            {
                FX = SpriteEffects.FlipHorizontally;
            }
            else
            {

                FX = SpriteEffects.None;
            }

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Main.EntitySpriteDraw(DTAssetLib.CutSwing.Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, DTAssetLib.CutSwing.Value.Size() / 2, Projectile.scale * 0.55f, FX, 0);

            Opus.ReturnToDefaultDrawing(spriteBatch);

            Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, projectileTexture.Size() / 2, Projectile.scale, FX, 0);

            return false;
        }

        public float DamageMult = 0.1f;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale *= 0.1f;
            DamageMult = 0.1f;
        }

        public Vector2 toMouse;
        public bool kill = false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (HitCooldown > 0)
            {
                HitCooldown--;
            }
            if (DamageMult < 1)
            {
                DamageMult += 0.05f;
            }

            if (player.HeldItem.type == ModContent.ItemType<TrueSoulEdge>() && player.controlUseItem && !kill)
            {
                if (Main.GameUpdateCount % 5 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                }
                player.SetDummyItemTime(6);
                Projectile.timeLeft = 120;
                toMouse = Main.MouseWorld - Projectile.Center;
                toMouse.Normalize();

                float d = Projectile.Center.Distance(Main.MouseWorld) / 2;

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, toMouse * d, 0.05f);
            }
            else
            {
                kill = true;
            }

            if (Projectile.scale < (player.GetAdjustedItemScale(player.HeldItem) * 3) && !kill && HitCooldown <= 0)
            {
                Projectile.scale += 0.01f;
                if (Main.rand.NextBool(3))
                {
                    Opus.RingSpreadDustRandom(DustID.DungeonSpirit, 10, Projectile.Center, 180, 20, default, -0.000002f, 2f);
                }
                if (Main.rand.NextBool(4))
                {
                    //PRTLoader.NewParticle(PRTLoader.GetParticleID<DungeonSpiritParticle>(), Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Main.rand.NextVector2Circular(3, 3), new Color(184, 228, 242), 1f);
                    //Opus.RingParticleInwardRandomDir(PRTLoader.GetParticleID<DungeonSpiritParticle>(), 5, Projectile.Center, 180, 0.8f, Color.White, 0.02f, 2f, ai2: 1);
                }
            }

            if (Projectile.scale < 0.1f)
            {
                Projectile.Kill();
            }

            if (Projectile.timeLeft <= 60 && kill)
            {
                Projectile.velocity *= 0.88f;
                Projectile.Opacity *= 0.9f;
            }

            

            

            Projectile.rotation += (Projectile.velocity.Length() * 0.2f) * Projectile.direction;

        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Width = (int)(Projectile.width * Projectile.scale);
            hitbox.Height = (int)(Projectile.height * Projectile.scale);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return HitCooldown <= 0 && !target.friendly;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitCooldown = HitCooldownGlobal;
            Projectile.scale *= 0.75f;
            DamageMult *= 0.8f;
            SoundEngine.PlaySound(DTAssetLib.SwordSounds.ThinSlice);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= DamageMult;
        }
    }
}
