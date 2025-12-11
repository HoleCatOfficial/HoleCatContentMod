using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using UtfUnknown.Core.Models.SingleByte.Finnish;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class FlameNode : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public int LineProjectileDurability = 10;
        public int LineNPCDurability = 5;
        public int HitCooldown = 40;
        private bool refresh = false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.ownedProjectileCounts[ModContent.ProjectileType<FlameNode>()] > 2)
            {
                Projectile.Kill();
                return;
            }

            Projectile.velocity = Vector2.Zero;
            Projectile.rotation -= 0.3f;


            PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Projectile.Center, new Vector2(0, -1f), new Color(253, 62, 3) * 0.9f, 0.4f, 60, 2);

            foreach (Projectile node in Main.projectile)
            {
                if (node.active && node.type == ModContent.ProjectileType<FlameNode>() && node.owner == Projectile.owner && node.whoAmI != Projectile.whoAmI)
                {
                    float distance = Vector2.Distance(Projectile.Center, node.Center);
                    int pointnumber = (int)(distance / 50f);

                    if (distance < 700f)
                    {
                        Vector2[] connectionPoints = new Vector2[pointnumber];
                        float[] randomTs = new float[pointnumber];
                        for (int i = 0; i < pointnumber; i++) randomTs[i] = Main.rand.NextFloat();
                        Array.Sort(randomTs);

                        for (int i = 0; i < pointnumber; i++)
                        {
                            float t = randomTs[i];
                            Vector2 point = Vector2.Lerp(node.Center, Projectile.Center, t);
                            connectionPoints[i] = point;

                            Dust.NewDustPerfect(point, DustID.TintableDustLighted, Vector2.Zero, 0, new Color(253, 62, 3), 1f);
                            PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), point, Vector2.Zero, new Color(253, 62, 3) * 0.4f, 0.4f);
                        }

                        foreach (Projectile hostile in Main.projectile)
                        {
                            if (hostile.active && (hostile.hostile || !hostile.friendly) && hostile.type != ModContent.ProjectileType<FlameNode>())
                            {
                                Rectangle hitbox = hostile.Hitbox;
                                float collisionPoint = 0f; // required "ref" variable
                                bool hit = Collision.CheckAABBvLineCollision(
                                    hitbox.TopLeft(), hitbox.Size(),
                                    Projectile.Center, node.Center,
                                    4f, // line width in pixels
                                    ref collisionPoint
                                );

                                if (hit)
                                {
                                    for (int y = 0; y < 9; y++)
                                    {
                                        PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle>(), hostile.Center, new Vector2(Main.rand.NextFloat(-2f, 2.1f), Main.rand.NextFloat(-4f, -6.1f)), new Color(253, 62, 3), 0.4f);
                                    }
                                    hostile.Kill();

                                    SoundEngine.PlaySound(SoundID.Item96, hostile.Center);
                                    LineProjectileDurability -= 1;
                                }
                            }
                        }

                        if (HitCooldown >= 40)
                        {
                            foreach (NPC enemy in Main.npc)
                            {
                                if (enemy.active && !enemy.friendly)
                                {
                                    Rectangle hitbox = enemy.Hitbox;
                                    float collisionPoint = 0f; // required "ref" variable
                                    bool hit = Collision.CheckAABBvLineCollision(
                                        hitbox.TopLeft(), hitbox.Size(),
                                        Projectile.Center, node.Center,
                                        4f, // line width in pixels
                                        ref collisionPoint
                                    );

                                    if (hit)
                                    {
                                        for (int y = 0; y < 9; y++)
                                        {
                                            PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle>(), enemy.Center, new Vector2(Main.rand.NextFloat(-2f, 2.1f), Main.rand.NextFloat(-4f, -6.1f)), new Color(253, 62, 3), 0.4f);
                                        }
                                        var Strike = new NPC.HitInfo() with { DamageType = ModContent.GetInstance<ScepterClass>(), Crit = Main.rand.NextBool(3), Damage = Projectile.damage, InstantKill = false, HideCombatText = false, HitDirection = 0, Knockback = 0, SourceDamage = Projectile.damage };
                                        enemy.StrikeNPC(Strike, false, false);
                                        SoundEngine.PlaySound(SoundID.Item96, enemy.Center);
                                        LineNPCDurability -= 1;
                                        HitCooldown = 0; // reset cooldown after hitting
                                        break; // stop after hitting one enemy to respect cooldown
                                    }
                                }
                            }
                        }
                        else
                        {
                            HitCooldown++;
                        }
                    }
                    else
                    {
                        node.Kill();
                        Main.NewText("Distance is too large!", new Color(253, 62, 3));
                    }
                }
                else
                {
                    if (refresh == false)
                    {
                        Projectile.timeLeft = 600;
                        refresh = true;
                    }
                }
            }

            if (LineProjectileDurability <= 0 || LineNPCDurability <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item100, Projectile.Center);
                foreach (Projectile node in Main.projectile)
                {
                    if (node.active && node.type == ModContent.ProjectileType<FlameNode>() && node.owner == Projectile.owner && node.whoAmI != Projectile.whoAmI)
                    {
                        node.Kill();
                    }
                }
                Projectile.Kill();
            }

            if (Projectile.timeLeft == 10)
            {
                for (int y = 0; y < 9; y++)
                {
                    Vector2 Pos = Projectile.Center + Main.rand.NextVector2CircularEdge(80, 100);
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle>(), Pos, (Projectile.Center - Pos) * 0.06f, new Color(253, 62, 3), 0.2f);
                }
            }

        }
    }
}