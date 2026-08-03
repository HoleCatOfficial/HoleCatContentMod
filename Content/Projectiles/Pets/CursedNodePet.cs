using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System.Collections.Generic;
using OpusLib;


namespace DestroyerTest.Content.Projectiles.Pets
{
    [AutoloadGlowmask]
    public class CursedNodePet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
        }
        public SoundStyle TP = new SoundStyle("DestroyerTest/Assets/Audio/ChargeBreak")
        {
            PitchRange = (0.25f, 0.75f),
            MaxInstances = 0
        };

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(TP, Projectile.Center);
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            for (int i = 0; i < TrailPositions.Count - 1; i++)
            {
                Vector2 start = TrailPositions[i] - Main.screenPosition;
                Vector2 end = TrailPositions[i + 1] - Main.screenPosition;
                Vector2 diff = end - start;

                float length = diff.Length();
                if (length < 0.5f)
                    continue; // skip tiny wiggle segments

                float rotation = diff.ToRotation();

                float width = MathHelper.Lerp(0.005f, 0.0007f, i / (float)TrailLength);
                float alpha = MathHelper.Lerp(1f, 0f, i / (float)TrailLength);
                Color color = ColorLib.CursedFlames * alpha;

                Main.spriteBatch.Draw(
                    DTAssetLib.Square.Value,
                    start,
                    null,
                    color,
                    rotation,
                    new Vector2(DTAssetLib.Square.Value.Width / 2, DTAssetLib.Square.Value.Height / 2),
                    new Vector2(length, width),
                    SpriteEffects.None,
                    0f
                );
            }
            Opus.ReturnToDefaultDrawing(spriteBatch);
            return true;
        }

        public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
        private const int TrailLength = 40;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;

            if (player.dead || !player.active)
                player.ClearBuff(ModContent.BuffType<NodesPetBuff>());

            if (player.HasBuff(ModContent.BuffType<NodesPetBuff>()))
                Projectile.timeLeft = 2;

            TrailPositions.Insert(0, Projectile.Center);
            TrailRotations.Insert(0, Projectile.rotation);

            // Cap trail
            while (TrailPositions.Count > TrailLength)
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            while (TrailRotations.Count > TrailLength)
                TrailRotations.RemoveAt(TrailRotations.Count - 1);

            DoIdleMovement(player);
            KeepUp(1200f, 2400f, player);
        }

        public int UpdateChecks = 3;
        private void DoIdleMovement(Player player)
        {
            Vector2 idlePos = player.Center;
            float time = Projectile.ai[0] / 60f;
            float offsetRadius = 100f;

            Vector2 targetPos = idlePos;
            float angle = time * MathHelper.TwoPi / 5f;
            targetPos = player.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * offsetRadius;

            Projectile.spriteDirection = 1;

            Vector2 desiredVelocity = (targetPos - Projectile.Center) * 0.08f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.1f);

            float Ob = Projectile.velocity.ToRotation() * 0.05f;
            if (Projectile.velocity.LengthSquared() > 0.01f)
                Projectile.rotation = float.Lerp(Projectile.rotation, Ob, 0.15f);

            int ichorType = ModContent.ProjectileType<IchorNodePet>();
            Projectile other = null;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == Projectile.owner && p.type == ichorType && p.whoAmI != Projectile.whoAmI)
                {
                    other = p;
                    break;
                }
            }
            if (other != null && UpdateChecks > 0)
            {
                float otherAngle = (other.Center - player.Center).ToRotation();
                angle = otherAngle + MathHelper.Pi;
                targetPos = player.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * offsetRadius;
                Vector2 desiredVelocity2 = (targetPos - Projectile.Center) * 0.12f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity2, 0.18f);
                if (Projectile.velocity.LengthSquared() > 0.01f)
                    Projectile.rotation = float.Lerp(Projectile.rotation, Projectile.velocity.ToRotation() * 0.05f, 0.15f);
                UpdateChecks--;
            }
        }

       

        private void KeepUp(float distSpeed, float distTeleport, Player master)
        {
            float dist = Projectile.Distance(master.Center);

            if (dist < distSpeed) return;

            if (dist < distTeleport)
            {
                int maxSpeed = 35;
                Vector2 toPlayer = master.Center - Projectile.Center;
                float length = toPlayer.Length();
                if (length > 0)
                {
                    toPlayer /= length;
                    float speed = MathHelper.Clamp(length / 12f, 8f, maxSpeed);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, toPlayer * speed, 0.1f);
                }
                return;
            }

            if (dist > distTeleport)
            {
                SoundEngine.PlaySound(TP);
                Projectile.Center = master.Center;
                Projectile.velocity *= 0.1f;
            }
        }
    }
}
