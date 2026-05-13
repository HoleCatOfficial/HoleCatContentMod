using DestroyerTest.Common;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class GloryOrbHoldout : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2000;
            Projectile.netImportant = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects FX = SpriteEffects.None;
            if ((Projectile.direction == -1 || Projectile.spriteDirection == -1) && player.direction == 1)
            {
                FX = SpriteEffects.FlipVertically;
            }
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, FX, 0);
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

       
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.HeldItem.type == ModContent.ItemType<GloryOrb>() && player.controlUseItem == true)
            {
                Vector2 toCursor = Main.MouseWorld - Projectile.Center;
                toCursor.Normalize();
                Projectile.Center = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.ThreeQuarters, toCursor.ToRotation() - MathHelper.PiOver2);
                Projectile.rotation = toCursor.ToRotation() + MathHelper.PiOver2;

                if (player.direction == -1)
                {
                    Projectile.spriteDirection = -1;
                }
                else
                {
                    Projectile.spriteDirection = 1;
                }
                Projectile.direction = toCursor.X > 0 ? 1 : -1;

                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.Pi);

                Projectile.ai[0]++;

                if (Projectile.ai[0] == 61)
                {
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/BlessedNodeLasersCharge"), Projectile.Center);
                }

                if (Projectile.ai[0] > 120)
                {
                    if (player.CheckMana(200, true))
                    {
                        Rectangle R = Utils.CenteredRectangle(Projectile.Center, new Vector2(100, 100));
                        Vector2 SP = Main.rand.NextVector2FromRectangle(R);

                        for (int i = 0; i < 12; i++)
                        {
                            Dust Fire = Dust.NewDustPerfect(SP, DustID.AncientLight, toCursor * Main.rand.NextFloat(2f, 32f), 0, Main.DiscoColor, Main.rand.NextFloat(1f, 4f));
                            Fire.noGravity = true;
                        }

                        SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/BlessedNodeLasers") with { PitchVariance = 0.5f }, Projectile.Center);
                        Projectile.ai[0] = 0;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, toCursor * 0.1f, ModContent.ProjectileType<BlessedLaserFriendly>(), Projectile.damage, 10, player.whoAmI, 0f, 0f);
                    }
                }
                else
                {
                    Rectangle R = Utils.CenteredRectangle(Projectile.Center, new Vector2(100, 100));
                    Vector2 SP = Main.rand.NextVector2FromRectangle(R);
                    Vector2 Dir = Projectile.Center - SP;
                    Dir.Normalize();


                    Dust Charge = Dust.NewDustPerfect(SP, DustID.AncientLight, Dir, 0, Main.DiscoColor, Main.rand.NextFloat(0.1f, 1.1f));
                    Charge.noGravity = true;
                    //Opus.RingSpreadDustRandom(DustID.AncientLight, 7, Projectile.Center + new Vector2(0, -20).RotatedBy(Projectile.rotation), 25, 50, Main.DiscoColor, 0f, 0.5f);
                }
            }
            else
            {
                Projectile.Kill();
            }
        }

    }
}