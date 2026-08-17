using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class HestiasBaneThrown : ModProjectile, IHomingProjectile
    {
        bool IHomingProjectile.TracksNPCs => !returning;

        bool IHomingProjectile.TracksPlayers => returning;

        float IHomingProjectile.HomingTurnSpeed => 5f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 50f;

        float IHomingProjectile.DetectRadius => 3200;

        bool IHomingProjectile.CanHome => (returning || !HitNPC) && flightTime  > 20;

        private bool returning = false;
        private int flightTime = 0;
        private int soundCooldown = 0;
        public bool HitNPC = false;
        private SoundStyle Woosh = DTAssetLib.SwordSounds.ColdSword with { Pitch = -0.7f, PitchVariance = 0.7f, MaxInstances = 0, Volume = 0.4f };
        private SoundStyle TileHit = DTAssetLib.Charge.Anvil;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 76;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;
        }

        public override void OnSpawn(IEntitySource source)
        {
        }


        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects FX = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            float ROff = Projectile.direction == 1 ? Projectile.rotation + MathHelper.PiOver4 : Projectile.rotation + MathHelper.PiOver4;
            Main.EntitySpriteDraw(DTAssetLib.CircularSwingThin.Value, Projectile.Center - Main.screenPosition, null, Color.SlateBlue, ROff, DTAssetLib.CircularSwingThin.Value.Size() / 2, 0.5f, SpriteEffects.None, 0);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                texture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);



            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, FX, 0f);
            return false;
        }

        public bool RangeOfPlayer = false;


        public override void AI()
        {

            if (soundCooldown > 0)
            {
                soundCooldown--;
            }

            if (soundCooldown <= 0)
            {
                SoundEngine.PlaySound(Woosh, Projectile.Center);
                soundCooldown = 30;
            }


            Player player = Main.player[Projectile.owner];

            RangeOfPlayer = Projectile.Center.Distance(player.Center) < 20;

            Projectile.rotation += 0.20f * Projectile.direction;

            
            Fire fire = new Fire();
            fire.PrepareFire(Projectile.Center + new Vector2(Projectile.width / 2, -(Projectile.width / 2)).RotatedBy(Projectile.rotation), Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), ColorLib.TenebrisMagenta, 0.6f, 100, FireDrawMode.Additive, PixelLayer.AboveTiles);
            ParticleEngine.BehindProjectiles.Add(fire);
            

            if (!DTOptimizationsConfig.instance.DisableExcessParticles)
            {

                PointGlowPreMultiplied Glow = new();
                Glow.Initialize(Projectile.Center + new Vector2(Projectile.width / 2, -(Projectile.width / 2)).RotatedBy(Projectile.rotation), Vector2.Zero, ColorLib.TenebrisMagenta, 1f);
                ParticleEngine.BehindProjectiles.Add(Glow);

                Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Projectile.width / 2, -(Projectile.width / 2)).RotatedBy(Projectile.rotation), DustID.FireworksRGB, new Vector2(4, 0).RotatedBy(Projectile.rotation + (Main.rand.NextFloat(0.25f) * Projectile.direction)), 0, ColorLib.TenebrisMagenta);
                d.noGravity = true;
            }

            if (!returning)
            {
                flightTime++;

                if (HitNPC)
                {
                    Projectile.velocity *= 0.9f;
                }

                if (flightTime >= 120)
                {
                    returning = true;
                }
            }

            if (returning)
            {
                Projectile.SmoothMoveToPoint(player.MountedCenter, 30f);
                Projectile.tileCollide = false;



                if (RangeOfPlayer)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return !HitNPC && Projectile.ManualCanHitFriendly(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            HitNPC = true;
            
            SoundStyle Hit = DTAssetLib.Impacts.DreamHit with
            {
                PitchVariance = 0.1f,
                Pitch = -0.5f
            };


            Player player = Main.player[Main.myPlayer];
            hit.Knockback = 4f;
            target.StrikeNPC(hit);
            SoundEngine.PlaySound(Hit, Projectile.position);

            BloomRingSharp Ring = new BloomRingSharp();
            Ring.Prepare(target.Center, Vector2.Zero, ColorLib.TenebrisMagenta, 0.05f, 0.01f, 1f, BlendState.Additive);
            ParticleEngine.ShaderParticles.Add(Ring);

            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<DarkRaptureExplosion>(), Projectile.damage, 10, Projectile.owner);

            returning = true;
            
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(TileHit, Projectile.Center);
            Projectile.penetrate--;

            BloomRingSharp Ring = new BloomRingSharp();
            Ring.Prepare(Projectile.Center, Vector2.Zero, ColorLib.TenebrisMagenta, 0.05f, 0.01f, 1f, BlendState.Additive);
            ParticleEngine.ShaderParticles.Add(Ring);


            returning = true;

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (!RangeOfPlayer)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }

        }

    }
}

