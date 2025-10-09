using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using Terraria.Audio;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using Terraria.DataStructures;

namespace DestroyerTest.Content.RangedItems
{
    public class StellarBowHoldout : ModProjectile
    {
        private int aiState = 0; // 0 = Lances, 1 = Stars
        private int stateTimer = 0; // Generic timer used in both states
        public override string Texture => "DestroyerTest/Content/RangedItems/StellarBow";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 200; // persistent
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D whiteOutline = ModContent.Request<Texture2D>("DestroyerTest/Content/RangedItems/StellarBowOutline").Value;

            Vector2 origin = new(texture.Width * 0.5f, texture.Height * 0.5f);
            SpriteEffects effects = Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float outlineRotation = Projectile.rotation;
            if (Projectile.direction == -1)
            {
                outlineRotation += MathHelper.Pi; // 180 degrees
            }

            // Draw the outline first
            Main.EntitySpriteDraw(
                whiteOutline,
                Projectile.Center - Main.screenPosition,
                null,
                ColorLib.StellarColor,
                outlineRotation,
                origin,
                Projectile.scale,
                effects,
                0
            );

            return true; // Let the base projectile texture draw as usual
        }


       

        SoundStyle ShootRegular = new SoundStyle($"DestroyerTest/Assets/Audio/StellarBow/StellarBowShoot", 3) with
        {
            PitchVariance = 0.2f,
            MaxInstances = 0
        };

        SoundStyle ShootEmpowered = new SoundStyle($"DestroyerTest/Assets/Audio/StellarBow/StellarBowEmpoweredShoot", 3) with
        {
            PitchVariance = 0.2f,
            MaxInstances = 0
        };

        public int ShotCount = 0;
        public int type = -1;
        public SoundStyle Shot;
        public enum State
        {
            Default,
            Empowered
        }
        public State state;

        public override void OnSpawn(IEntitySource source)
        {
            state = State.Default;
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.HeldItem.type == ModContent.ItemType<StellarBow>() && player.channel)
            {
                // Lock projectile to player/cursor
                float holdDistance = 15f;
                Vector2 mountedCenter = player.MountedCenter;
                Vector2 toCursor = Main.MouseWorld - mountedCenter;
                toCursor.Normalize();
                Vector2 desiredPos = mountedCenter + toCursor * holdDistance;

                Projectile.Center = desiredPos;
                Projectile.rotation = toCursor.ToRotation();
                Projectile.direction = toCursor.X > 0 ? 1 : -1;

                if (Main.GameUpdateCount % player.HeldItem.useTime == 0)
                {
                    SoundEngine.PlaySound(Shot);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.rotation.ToRotationVector2() * 12, type, Projectile.damage, Projectile.knockBack, player.whoAmI);
                    ShotCount++;
                }

                if (ShotCount >= 10)
                {
                    state = State.Empowered;
                }

                switch (state)
                    {
                        case State.Default:
                            {
                                type = ModContent.ProjectileType<GalantineArrow>();
                                Shot = ShootRegular;
                                break;
                            }
                        case State.Empowered:
                            {
                                Shot = ShootEmpowered;
                                type = ModContent.ProjectileType<GalantineLanceFriendly>();
                                break;
                            }
                    }
            }
            else
            {
                Projectile.Kill();
            }
        }


    }
}