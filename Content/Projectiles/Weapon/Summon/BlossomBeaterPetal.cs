using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Summon
{
    public class BlossomBeaterPetal : ModProjectile
    {
        private NPC HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public ref float DelayTimer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 120;
            Projectile.frame = 0;
            Projectile.ArmorPenetration = 12;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                projectileTexture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(projectileTexture.Width / 2f, frameHeight / 2f);

            Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        private bool willHome;

        public override void OnSpawn(IEntitySource source)
        {
            willHome = Main.rand.NextBool(4);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            AnimateProjectile();

            for (int g = 0; g < 3; g++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, default, 0.8f);
            }

            float maxDetectRadius = 800f;

            if (DelayTimer < 30)
            {
                DelayTimer += 1;
                return;
            }

            if (willHome == false && DelayTimer >= 30)
            {
                Projectile.velocity *= 0.999f;
            }

            if (willHome == true)
            {
                if (HomingTarget == null)
                {
                    HomingTarget = FindClosestNPC(maxDetectRadius);
                }

                if (HomingTarget != null && !IsValidTarget(HomingTarget))
                {
                    HomingTarget = null;
                }

                if (HomingTarget == null)
                    return;

                float length = Projectile.velocity.Length();
                float targetAngle = Projectile.AngleTo(HomingTarget.Center);
                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(3)).ToRotationVector2() * length;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            

        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.Center);
            for (int g = 0; g < 10; g++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<NightmareRosePetalDust>(), Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, default, 1.2f);
            }
        }

        private void AnimateProjectile()
        {

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (var target in Main.ActiveNPCs)
            {
                if (IsValidTarget(target))
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public bool IsValidTarget(NPC target)
        {
            return target.CanBeChasedBy();
        }
    }
}