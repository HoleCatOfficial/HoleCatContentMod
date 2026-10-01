using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.ClasslessItems;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.SummonItems;
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

namespace DestroyerTest.Content.Projectiles.Weapon.Classless
{
    public class BoLDHoldout : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D PT = TextureAssets.Projectile[Type].Value;

            Texture2D Cube = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/BoLDCube").Value;

            int frameHeight = PT.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                PT.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(PT.Width / 2f, frameHeight / 2f);

            SpriteEffects FX = SpriteEffects.None;

            float rot = Projectile.rotation;

            if (rot > MathHelper.PiOver2 || rot < -MathHelper.PiOver2)
            {
                FX = SpriteEffects.FlipVertically;
            }
            else
            {
                FX = SpriteEffects.None;
            }

            Main.EntitySpriteDraw(PT, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, FX);

            Main.EntitySpriteDraw(Cube, ShootPoint - Main.screenPosition, null, Color.White, CubeRot, Cube.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public Player Owner => Main.player[Projectile.owner];

        public SoundStyle FireSound = SoundID.Item20 with {  };

        Vector2 ShootPoint;
        float CubeRot = 0f;

        public override void AI()
        {
            Projectile.velocity *= 0;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Projectile.Center = Owner.Center + new Vector2(30 * Owner.direction, 0);

            Projectile.rotation = 0;
            ShootPoint = Projectile.Center + new Vector2(0, -20);

            PointGlowPreMultiplied glow = new();
            glow.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), new Vector2(0f, -7f), ColorLib.Soul3 * 0.5f, 1.25f, 30);
            ParticleEngine.Particles.Add(glow);


            PointGlowPreMultiplied glow2 = new();
            glow2.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), new Vector2(0f, -7f), ColorLib.Soul * 0.5f, 1f, 30);
            ParticleEngine.Particles.Add(glow2);

            CubeRot += 0.1f * Owner.direction;

            if (Owner.HeldItem.type == ModContent.ItemType<TheBookofLifeAndDeath>() && Owner.controlUseItem && !Owner.CCed && !Owner.dead)
            {
                Owner.SetDummyItemTime(2);
                Projectile.timeLeft = 60;
                Projectile.ai[0]++;

                if (Projectile.ai[0] >= 6)
                {
                    Projectile.ai[0] = 0;
                    SoundEngine.PlaySound(FireSound, Projectile.Center);
                    Fire();
                    Effects();
                }

            }
        }

        private void Effects()
        {
            for (int i = 0; i < 4; i++)
            {
                PixelParticle Pixel = new();
                Pixel.Initialize(ShootPoint, Main.rand.NextVector2Circular(5f, 5f), ColorLib.Soul, 2f);
                ParticleEngine.Particles.Add(Pixel);
            }
        }

        private void Fire()
        {
            Vector2 dir = ShootPoint.DirectionTo(Main.MouseWorld);

            int[] Types = [ModContent.ProjectileType<SoulTriangle>(), ModContent.ProjectileType<SoulDart>(), ModContent.ProjectileType<SoulStar>()];
            
            Projectile Shot1 = Projectile.NewProjectileDirect(Owner.GetSource_FromThis(), ShootPoint, dir.RotatedByRandom(0.1f) * Main.rand.NextFloat(10f, 18f), Types[Main.rand.Next(Types.Length)], Projectile.damage, Projectile.knockBack, Owner.whoAmI);



        }
    }
}
