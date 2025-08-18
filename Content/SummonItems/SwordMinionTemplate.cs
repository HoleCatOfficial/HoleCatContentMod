using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Formats.Tar;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
namespace DestroyerTest.Content.SummonItems
{
    /// <summary>
    /// Covers basic behaviour for you. Parameters such as dimensions and scale are not covered.
    /// </summary>
    public class SwordMinionTemplate : ModProjectile
    {
        /// <summary>
        /// Color used for dust, lighting, and other colored special effects.
        /// </summary>
        public Color ThemeColor = Color.White;

        /// <summary>
        /// Color used for Drawing if you want your sword to be tinted a certain color at any point.
        /// </summary>
        public Color TintColor = Color.White;

        /// <summary>
        /// The Dust type usede during death, teleportation, and idling.
        /// </summary>
        public int IdleDustType = DustID.Copper;

        /// <summary>
        /// The Dust type usede during death, teleportation, and idling.
        /// </summary>
        public int DashDustType = DustID.CursedTorch;

        /// <summary>
        /// The Dust type usede during death, teleportation, and idling.
        /// </summary>
        public int TeleDustType = DustID.Copper;

        /// <summary>
        /// The sound the minion will make when it teleports back to you.
        /// </summary>
        public SoundStyle TeleSound = SoundID.AbigailSummon;

        /// <summary>
        /// The sound the minion will make when dashing towards an enemy.
        /// </summary>
        public SoundStyle DashSound = SoundID.AbigailCry;

        /// <summary>
        /// Whether or not to draw an afterimage in the color of the original projectile sprite.
        /// <para/> You must override AfterImage to True for this to work. If not, this will automatically be set to false during loading.
        /// </summary>
        public bool AfterImageColorless = true;

        /// <summary>
        /// Whether or not to draw an afterimage in the tint color of the projectile.
        /// <para/> You must override AfterImage to True for this to work. If not, this will automatically be set to false during loading.
        /// </summary>
        public bool AfterImageTinted = false;

        /// <summary>
        /// Whether or not to even draw the afterimage at all.
        /// </summary>
        public bool AfterImage = true;

        /// <summary>
        /// Whether or not to return true in PreDraw (AKA let terraria draw the sprite normally.)
        /// </summary>
        public bool DefaultDraw = true;

        /// <summary>
        /// Shorthand for how many extraupdates to add. 3 is recommended, 1 is the minimum.
        /// </summary>
        public int TickSpeed = 1;

        /// <summary>
        /// This one is self explanatory.
        /// <para/> Buuuuuuuuut, For those who need a refresher, enabling this will spawn a particleorchestrator particle at the projectile's position when it teleports back to you.
        /// <para/> This can be used in conjunction with the PRT system using UsesPRTOnTele.
        /// </summary>
        public bool UsesParticleOrchestratorOnTele = true;

        /// <summary>
        /// This one is self explanatory.
        /// <para/> Buuuuuuuuut, For those who need a refresher, enabling this will spawn an Innovault PRT particle at the projectile's position when it teleports back to you.
        /// <para/> This can be used in conjunction with ParticleOrchestrator using UsesParticleOrchestratorOnTele.
        /// <para/> Word of Caution: Innovault Particles do not yet have layering controls. So they do not draw behind projectiles.
        /// </summary>
        public bool UsesPRTOnTele = false;

        /// <summary>
        /// How far away the projectile has to be to automatically teleport back. Bigger numbers allow it to dash further.
        /// </summary>
        public int TeleDist = 700;

        /// <summary>
        /// How far away an enemy can be to be spotted and chased.
        /// <para/> For Best Performance, set this less than or equal to TeleDist.
        /// </summary>
        public int Range = 2000;

        /// <summary>
        /// THe ID of the buff that is required to keep the projectile alive.
        /// </summary>
        public int ActiveBuff = -1;

        /// <summary>
        /// What particle ID to use when teleporting. Should be used in tandem with UsesPRTOnTele.
        /// </summary>
        public int TelePRTID = PRTLoader.GetParticleID<Boom1>();

        /// <summary>
        /// What ParticleOrchestrator type to use when teleporting. Should be used in tandem with UsesParticleOrchestratorOnTele.
        /// </summary>
        public ParticleOrchestraType TeleParticleOrchestraType = ParticleOrchestraType.Excalibur;

        /// <summary>
        /// How the blade(s) will idle near the player.
        /// <para/> LineUp = All blades form a relatively neat line behind the player. This option uses example mod code.
        /// <para/> Chevron = All Blades pointing up, evenly spaced, and slightly disinclined with each pair further out.
        /// <para/> Defensive = All Blades face out, are positioned equidistantly in a circle, and rotate in a circle around the player.
        /// </summary>
        public enum IdleStyle
        {
            LineUp,
            Chevron,
            Defensive
        };

        private void IdleDust()
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, IdleDustType, 0, 0, 254, Scale: 1.0f);
            dust.velocity += Projectile.velocity * 0.5f;
            dust.velocity *= 0.5f;
            dust.noGravity = true;
        }

        private void DashDust()
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DashDustType, 0, 0, 254, Scale: 1.0f);
            dust.velocity += Projectile.velocity * 0.5f;
            dust.velocity *= 0.5f;
            dust.noGravity = true;
        }

        private void TeleDust()
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, TeleDustType, 0, 0, 254, Scale: 1.0f);
            dust.velocity += Projectile.velocity * 0.5f;
            dust.velocity *= 0.5f;
            dust.noGravity = true;
        }

        public override void Load()
        {
            if ((AfterImageColorless || AfterImageTinted) && !AfterImage)
            {
                Mod.Logger.Warn("Failed to enable AfterImage effect with reason: AfterImage bool was not set to true, but AfterImage Colorless or Tinted was. Set AfterImage to true to use its variants.");
                AfterImageColorless = false;
                AfterImageTinted = false;
            }
            if (!UsesParticleOrchestratorOnTele && !UsesPRTOnTele)
            {
                Mod.Logger.Warn("Projectile is invalid. Reason: Sorry pal, but you need to use an effect on teleport. Dont worry though, I'll enable one of them for you.");
                UsesPRTOnTele = true;
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.extraUpdates = TickSpeed;
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            if (AfterImage && AfterImageColorless)
            {
                for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            if (AfterImage && AfterImageTinted)
            {
                for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = TintColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            return DefaultDraw;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            UpdateAtkCooldown();
            if (!CheckActive(owner))
            {
                return;
            }
            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
            Visuals();
        }

        // Attack cooldown timer. When > 0, minion cannot attack.
        public int AtkCooldownTimer = 0;

        // Returns true if minion can attack (cooldown expired).
        public bool CanAttack => AtkCooldownTimer <= 0;

        // Call this when the minion attacks to start cooldown.
        public void StartAtkCooldown(int timer)
        {
            AtkCooldownTimer = timer;
        }

        // Call this in AI() every tick to decrement cooldown.
        public void UpdateAtkCooldown()
        {
            if (AtkCooldownTimer > 0)
                AtkCooldownTimer--;
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ActiveBuff);

                return false;
            }
            bool hasScabbard = false;
            for (int j = 0; j < owner.armor.Length; j++)
            {
                if (owner.armor[j].type == ModContent.ItemType<Hope_Scabbard>())
                {
                    hasScabbard = true;
                    break;
                }
            }
            if (owner.HasBuff(ActiveBuff) || hasScabbard)
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        public IdleStyle Style;
        public bool TargFlag = false;
        public virtual void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            IdleDust();

            Vector2 idlePosition = owner.Center;
            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Projectile.Distance(owner.Center) > TeleDist)
            {
                Projectile.Center = idlePosition + new Vector2(0, -20).RotatedByRandom(MathHelper.Pi);
            }

            if (TargFlag)
                return;

            switch (Style)
            {
                case IdleStyle.LineUp:
                    {
                        idlePosition.Y -= 64f;
                        float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
                        idlePosition.X += minionPositionOffsetX;

                        if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > TeleDist)
                        {
                            SoundEngine.PlaySound(TeleSound);
                            if (UsesPRTOnTele)
                            {
                                TeleParticle_PRT(TelePRTID, Color.AliceBlue, 1.0f);
                            }
                            if (UsesParticleOrchestratorOnTele)
                            {
                                TeleParticle_ParticleOrchestrator((int)TeleParticleOrchestraType);
                            }
                            Projectile.position = idlePosition;
                            Projectile.velocity *= 0.1f;
                            Projectile.netUpdate = true;

                        }

                        float overlapVelocity = 0.04f;

                        foreach (var other in Main.ActiveProjectiles)
                        {
                            if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                            {
                                if (Projectile.position.X < other.position.X)
                                {
                                    Projectile.velocity.X -= overlapVelocity;
                                }
                                else
                                {
                                    Projectile.velocity.X += overlapVelocity;
                                }

                                if (Projectile.position.Y < other.position.Y)
                                {
                                    Projectile.velocity.Y -= overlapVelocity;
                                }
                                else
                                {
                                    Projectile.velocity.Y += overlapVelocity;
                                }
                            }
                        }
                        break;
                    }
                case IdleStyle.Chevron:
                    {

                        Vector2 MainPos = idlePosition + new Vector2(0, -100);
                        int totalMinions = owner.ownedProjectileCounts[Projectile.type];
                        int centerIndex = totalMinions / 2;

                        float chevronSpacing = 40f;
                        float chevronHeight = 32f;
                        int pos = Projectile.minionPos;

                        Vector2 chevronOffset = Vector2.Zero;

                        if (totalMinions % 2 == 1 && pos == 0)
                        {
                            chevronOffset.X = -chevronSpacing * (centerIndex + 1);
                            chevronOffset.Y = chevronHeight * (centerIndex + 1);
                        }
                        else
                        {
                            int side = (pos <= centerIndex) ? -1 : 1;
                            int offsetIndex = side == -1 ? centerIndex - pos : pos - centerIndex;
                            chevronOffset.X = side * chevronSpacing * (offsetIndex + 1);
                            chevronOffset.Y = chevronHeight * (offsetIndex + 1);
                        }

                        idlePosition += chevronOffset;

                        vectorToIdlePosition = idlePosition - Projectile.Center;
                        distanceToIdlePosition = vectorToIdlePosition.Length();

                        if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > TeleDist)
                        {
                            SoundEngine.PlaySound(TeleSound);
                            if (UsesPRTOnTele)
                            {
                                TeleParticle_PRT(TelePRTID, Color.AliceBlue, 1.0f);
                            }
                            if (UsesParticleOrchestratorOnTele)
                            {
                                TeleParticle_ParticleOrchestrator((int)TeleParticleOrchestraType);
                            }
                            Projectile.position = idlePosition;
                            Projectile.velocity *= 0.1f;
                            Projectile.netUpdate = true;
                        }

                        float overlapVelocity = 0.04f;

                        foreach (var other in Main.ActiveProjectiles)
                        {
                            if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                            {
                                if (Projectile.position.X < other.position.X)
                                {
                                    Projectile.velocity.X -= overlapVelocity;
                                }
                                else
                                {
                                    Projectile.velocity.X += overlapVelocity;
                                }

                                if (Projectile.position.Y < other.position.Y)
                                {
                                    Projectile.velocity.Y -= overlapVelocity;
                                }
                                else
                                {
                                    Projectile.velocity.Y += overlapVelocity;
                                }
                            }
                        }

                        Projectile.rotation = -MathHelper.PiOver4;

                        break;
                    }
                case IdleStyle.Defensive:
                    {
                        // Collect all active minions of this type for this owner
                        List<Projectile> minions = new List<Projectile>();
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner &&
                                Main.projectile[i].type == Projectile.type)
                            {
                                minions.Add(Main.projectile[i]);
                            }
                        }
                        int totalMinions = minions.Count;
                        int myIndex = minions.FindIndex(p => p.whoAmI == Projectile.whoAmI);

                        if (totalMinions == 0) totalMinions = 1; // Prevent division by zero

                        float orbitRadius = 120f;
                        float orbitSpeed = 0.05f; // Radians per tick
                        float angleOffset = MathHelper.TwoPi / totalMinions * myIndex;

                        float angle = Main.GameUpdateCount * orbitSpeed + angleOffset;
                        Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * orbitRadius;

                        Vector2 desiredPosition = owner.Center + offset;

                        Vector2 toPosition = desiredPosition - Projectile.Center;
                        float speed = 8f;
                        float inertia = 10f;

                        Vector2 desiredVelocity = toPosition.SafeNormalize(Vector2.Zero) * speed;
                        Projectile.velocity = (Projectile.velocity * (inertia - 1) + desiredVelocity) / inertia;
                        Projectile.rotation = offset.ToRotation() + MathHelper.PiOver2;

                        vectorToIdlePosition = idlePosition - Projectile.Center;
                        distanceToIdlePosition = vectorToIdlePosition.Length();

                        if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > TeleDist)
                        {
                            SoundEngine.PlaySound(TeleSound);
                            if (UsesPRTOnTele)
                            {
                                TeleParticle_PRT(TelePRTID, Color.AliceBlue, 1.0f);
                            }
                            if (UsesParticleOrchestratorOnTele)
                            {
                                TeleParticle_ParticleOrchestrator((int)TeleParticleOrchestraType);
                            }
                            Projectile.position = idlePosition;
                            Projectile.velocity *= 0.1f;
                            Projectile.netUpdate = true;
                        }
                        break;
                    }
            }
        }
        public void TeleParticle_ParticleOrchestrator(int particleType)
        {
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, (ParticleOrchestraType)particleType, new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(Projectile.Hitbox) }, Projectile.owner);
        }
        public void TeleParticle_PRT(int PRTID, Color color, float Scale)
        {
            PRTLoader.NewParticle(PRTID, Projectile.Center, new Vector2(0, 0.001f), color, Scale);
        }
        private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
        {
            // Starting search distance
            distanceFromTarget = 700f;
            targetCenter = Projectile.position;
            foundTarget = false;

            IdleDust();

            if (Projectile.Distance(owner.Center) > TeleDist)
            {
                Projectile.Center = owner.Center + new Vector2(0, -20).RotatedByRandom(MathHelper.Pi);
            }

            if (!CanAttack)
            {
                return;
            }

            // This code is required if your minion weapon has the targeting feature
                if (owner.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                    float between = Vector2.Distance(npc.Center, Projectile.Center);

                    // Reasonable distance away so it doesn't target across multiple screens
                    if (between < Range)
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        foundTarget = true;
                    }
                }

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;

                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            TargFlag = foundTarget;

            Projectile.friendly = true;
        }

        private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition)
        {
            float speed = 50f;
            float inertia = 140f;
            Player owner = Main.player[Projectile.owner];

            if (Projectile.Distance(owner.Center) > TeleDist)
            {
                Projectile.Center = owner.Center + new Vector2(0, -20).RotatedByRandom(MathHelper.Pi);
            }

            DashDust();

            if (foundTarget)
            {
                if (distanceFromTarget > 40f)
                {
                    if (Projectile.ai[1] == 0)
                    {
                        Vector2 direction = targetCenter - Projectile.Center;
                        direction.Normalize();
                        direction *= speed;
                        float targetAngle = Projectile.AngleTo(targetCenter * MathHelper.ToRadians(360));
                        Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                        if (distanceFromTarget < 50f)
                        {
                            SoundEngine.PlaySound(SoundID.Item66);
                            Projectile.ai[1] = 1; // Enter strike-through phase
                            Projectile.ai[0] = 0; // Reset timer
                        }
                        Projectile.rotation = targetAngle;
                    }
                }
            }
            if (Projectile.ai[1] == 1)
            {
                Projectile.ai[0]++;

                if (Projectile.ai[0] < 20)
                {
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
                }
                else
                {
                    Projectile.ai[1] = 0;
                }
            }

            if (!foundTarget)
            {
                Projectile.ai[1] = 0;
                if (distanceToIdlePosition > 600f)
                {
                    speed = 12f;
                    inertia = 60f;
                }
                else
                {
                    speed = 4f;
                    inertia = 80f;
                }
                if (distanceToIdlePosition > 20f)
                {
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
                else if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }

            if (Projectile.Distance(owner.Center) > TeleDist)
            {
                Projectile.Center = owner.Center + new Vector2(0, -20).RotatedByRandom(MathHelper.Pi);
            }
        }

        private void Visuals()
        {
            Projectile.rotation = Projectile.velocity.X * 0.5f;
            Lighting.AddLight(Projectile.Center, ThemeColor.ToVector3() * 0.7f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            StartAtkCooldown(80);
        }

    }
}