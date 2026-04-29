using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using GlowmaskHelper.Content;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;


namespace DestroyerTest.Content.Projectiles.Pets
{
    [AutoloadGlowmask]
    public class TenebrousConstructPet : ModProjectile
    {
        public Effect RedShader;
        public override void Load()
        {
            RedShader = ModContent.Request<Effect>("DestroyerTest/Assets/Effects/ColorShitRed").Value;
        }
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            Main.projFrames[Type] = 30;
        }
        public SoundStyle TP = new SoundStyle("DestroyerTest/Assets/Audio/TenebrousConstruct/Idle", 8)
        {
            PitchRange = (0.5f, 1f),
            MaxInstances = 0
        };

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2;
        }

        public float WingXScale = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Asset<Texture2D> WingLeft = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/TenebrousConstructPetWingLeft");
            Asset<Texture2D> WingRight = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/TenebrousConstructPetWingRight");

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, RedShader, Main.GameViewMatrix.TransformationMatrix);

            // Left wing: origin at RIGHT edge, middle vertically
            Vector2 originLeft = new Vector2(WingLeft.Width(), WingLeft.Height() / 2);
            Main.EntitySpriteDraw(
                WingLeft.Value,
                Projectile.Center - Main.screenPosition + new Vector2(-16, -20),
                null,
                Color.White,
                0f,
                originLeft,
                new Vector2(WingXScale * 2, 2f),
                SpriteEffects.None,
                0
            );

            // Right wing: origin at LEFT edge, middle vertically
            Vector2 originRight = new Vector2(0, WingRight.Height() / 2);
            Main.EntitySpriteDraw(
                WingRight.Value,
                Projectile.Center - Main.screenPosition + new Vector2(16, -20),
                null,
                Color.White,
                0f,
                originRight,
                new Vector2(WingXScale * 2, 2f),
                SpriteEffects.None,
                0
            );

            Opus.ReturnToDefaultDrawing(spriteBatch);
            return true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(TP, Projectile.Center);
            for (int f = 0; f < 8; f++)
            {
                Vector2 Outer = Projectile.Center + Main.rand.NextVector2CircularEdge(3, 3);
                Vector2 Dir = Projectile.Center - Outer;
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Projectile.Center, Dir, ColorLib.TenebrisGradient, Main.rand.NextFloat(0.15f, 1f));
            }
        }

        private void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }
        
        public float Randomizer => Projectile.localAI[0];
        private const int IdleModeSwitchTime = 480;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            WingXScale = 0.5f + 0.3f * (float)Math.Sin(Main.GameUpdateCount * 0.05f);
            Projectile.ai[0] += 0.5f;

            AnimateProjectile();

            if (player.dead || !player.active)
                player.ClearBuff(ModContent.BuffType<TenebrousConstructPetBuff>());

            if (player.HasBuff(ModContent.BuffType<TenebrousConstructPetBuff>()))
                Projectile.timeLeft = 2;

            int mode = (int)((Projectile.ai[0] / IdleModeSwitchTime) % 3);
            DoIdleMovement(player, mode);
            KeepUp(1200f, 2400f, player);
        }

        private void DoIdleMovement(Player player, int mode)
        {
            Vector2 idlePos = player.Center;
            float time = Projectile.ai[0] / 60f;
            float offsetRadius = 100f;

            Vector2 targetPos = idlePos;

            switch (mode)
            {
                case 0:
                    float sweep = (float)Math.Sin(time * 2f) * 120f;
                    float verticalOffset = -40f + (float)Math.Sin(time * 0.3f) * 20f;
                    targetPos = player.Center + new Vector2(-sweep, verticalOffset);
                    break;

                case 1:
                    float angle = time * MathHelper.TwoPi / 5f;
                    targetPos = player.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * offsetRadius;
                    break;

                case 2:
                    float figX = (float)Math.Sin(time * 1.5f) * 50f;
                    float figY = (float)Math.Sin(time * 3f) * 25f - 70f;
                    targetPos = player.Center + new Vector2(figX, figY);
                    break;
            }

            Projectile.spriteDirection = 1;

            Vector2 desiredVelocity = (targetPos - Projectile.Center) * 0.08f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.1f);

            float Ob = Projectile.velocity.ToRotation() * 0.05f;
            if (Projectile.velocity.LengthSquared() > 0.01f)
                Projectile.rotation = float.Lerp(Projectile.rotation, Ob, 0.15f);
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
                for (int f = 0; f < 8; f++)
                {
                    Vector2 Outer = Projectile.Center + Main.rand.NextVector2CircularEdge(3, 3);
                    Vector2 Dir = Projectile.Center - Outer;
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Projectile.Center, Dir, ColorLib.TenebrisGradient, Main.rand.NextFloat(0.15f, 1f));
                }
                SoundEngine.PlaySound(TP, Projectile.Center);
                Projectile.Center = master.Center;
                Projectile.velocity *= 0.1f;
            }
        }
    }
}
