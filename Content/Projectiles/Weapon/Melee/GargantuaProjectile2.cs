using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using Microsoft.Xna.Framework.Graphics;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Buffs;
using System;
using InnoVault.PRT;
using OpusLib;
using Terraria.GameContent;
using ReLogic.Content;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class GargantuaProjectile2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180; // persistent
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.netImportant = true;
            Projectile.scale = 0.75f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> Tex = TextureAssets.Projectile[Projectile.type];
            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Tex.Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public SoundStyle Throw = new SoundStyle("DestroyerTest/Assets/Audio/Constitution_Jab") { MaxInstances = 0, PitchVariance = 0.4f };
        
        public SoundStyle FullCharge = new SoundStyle("DestroyerTest/Assets/Audio/HopeScabbardTele") { MaxInstances = 0, PitchVariance = 0.4f };

        public int Charge = 0;
        public bool Sound1 = false;
        public bool Sound2 = false;
        public Vector2 swordTip;
        public Vector2 bladeBase;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float bladeLength = 60f * Projectile.scale; // tune this
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * bladeLength;

            Vector2 bladeDir = Projectile.rotation.ToRotationVector2();
            bladeBase = Projectile.Center - bladeDir * 20f;
            Vector2 bladeTip  = Projectile.Center + bladeDir * 60f;


            
            if (player.HeldItem.type == ModContent.ItemType<Gargantua>() && player.controlUseTile)
            {
                
                RotationFX(player);
                Projectile.timeLeft = 180;

                if (Projectile.scale < 1f)
                {
                    Projectile.scale += 0.01f;
                }
                if (Charge < 300)
                {
                    Charge++;
                    //Opus.RingDustInward(DustID.FireworksRGB, 5, swordTip, 50, 0, Color.Red, 1.75f, 5, true);
                }

                if (Charge >= 300)
                {
                    if (!Sound1)
                    {
                        //Opus.RingDustOutward(DustID.FireworksRGB, 20, swordTip, 50, 0, Color.Red, 1.75f, 10, true);
                        SoundEngine.PlaySound(FullCharge, Projectile.Center);
                        Sound1 = true;
                    }
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Vector2 toCursor = Main.MouseWorld - player.MountedCenter;
                Projectile.rotation = toCursor.ToRotation() + MathHelper.PiOver4;

                if (!player.controlUseTile)
                {
                    int Speed = 50;
                    Projectile.velocity = toCursor.ToRotation().ToRotationVector2() * Speed;
                    CheckForStick();
                }
                
                if (Charge >= 300)
                {
                    if (!Sound2)
                    {
                        SoundEngine.PlaySound(Throw, Projectile.Center);
                        Sound2 = true;
                        Projectile.netUpdate = true;
                    }

                    if (!player.controlUseTile)
                    {
                        FullChargeThrowEffects();
                    }
                }
            }
        }

        public void RotationFX(Player player)
        {
            float holdDistance = 100f;
            if (Charge > 150)
            {
                if (holdDistance > 50)
                {
                    holdDistance -= 1f;
                }
            }
            Vector2 mountedCenter = player.MountedCenter;
            Vector2 toCursor = Main.MouseWorld - mountedCenter;
            toCursor.Normalize();
            Vector2 desiredPos = mountedCenter + toCursor * holdDistance;

            Projectile.Center = desiredPos;

            
            Projectile.rotation = toCursor.ToRotation() + MathHelper.PiOver4;

            Vector2 PRTDir = toCursor * 5;
            
            PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], bladeBase, PRTDir, Color.Red, 1f, 60, ai2: 1);

            player.SetCompositeArmFront((player.controlUseTile && player.HeldItem.type == ModContent.ItemType<Gargantua>()), Player.CompositeArmStretchAmount.ThreeQuarters, toCursor.ToRotation() - MathHelper.PiOver2);
        }

        public void FullChargeThrowEffects()
        {
            PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], swordTip, Vector2.Zero, Color.Red, 0.5f, 60, ai2: 1);
            //PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), swordTip, new Vector2(-10, 0).RotatedBy(Projectile.rotation), Color.Red, 0.5f, 60, ai2: 1);
        }

        private NPC StickT;
        private Vector2 stuckOffset;
        private bool Flag1;
        private bool Stick;

        public void Sticking(NPC target)
        {
            if (!Stick)
            {
                StickT = target;
                stuckOffset = Projectile.Center - target.Center;
                Stick = true;
            }
        }

        public void CheckForStick()
        {
            if (Stick)
            {
                if (!Flag1)
                {
                    Projectile.timeLeft = 30;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.tileCollide = false;
                    Flag1 = true;
                    Projectile.netUpdate = true;
                }

                if (StickT != null && StickT.active)
                {
                    // Keep the projectile glued to the target
                    Projectile.Center = StickT.Center + stuckOffset;

                    // Optional: match target rotation if you want it to “move” with the enemy animation
                    // Projectile.rotation = StickT.rotation;

                    if (StickT.life <= 0)
                        Projectile.Kill();

                    return;
                }
                else
                {
                    // Target despawned or died
                    Stick = false;
                    Projectile.tileCollide = true;
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float bladeLength = 60f * Projectile.scale;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * bladeLength;

            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                start,
                end
            );
        }

		
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
            Sticking(target);
            Projectile.netUpdate = true;

            if (Charge < 300)
            {
                Projectile.Kill();
            }
            if (Charge >= 300)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = target.Center;
                Projectile.timeLeft = 30;
                Projectile.NewProjectile(Projectile.GetSource_Death(), target.Center, Vector2.Zero, ModContent.ProjectileType<GargantuaExplosion>(), (int)(Projectile.damage * 1.5f), 20, Projectile.owner);
                //SoundEngine.PlaySound(FullChargeHit, Projectile.Center);
                //hit.Damage = (int)(hit.Damage * 1.5f);
            }
		}

        public override void OnKill(int timeLeft)
        {
            Charge = 0;
            Sound1 = false;
            Sound2 = false;
        }

    }
}