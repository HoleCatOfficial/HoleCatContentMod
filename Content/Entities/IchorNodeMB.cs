
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Policy;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.BossBar;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.ConstitutionBoss;
using DestroyerTest.Content.Projectiles.NightmareRose;
using InnoVault;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Entities
{

    [AutoloadBossHead]
    public class IchorNodeMB : ModNPC
    {
        public override string BossHeadTexture => "DestroyerTest/Content/Entities/IchorNode_Head_Boss";
        public override void SetStaticDefaults()
        {
            NPCID.Sets.CanHitPastShimmer[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.TrailCacheLength[Type] = 20;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { // Influences how the NPC looks in the Bestiary
                CustomTexturePath = "DestroyerTest/Content/Entities/NodesBestiary", // If the NPC is multiple parts like a worm, a custom texture for the Bestiary is encouraged.
                Position = Vector2.Zero,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 100;
            NPC.aiStyle = -1;
            NPC.damage = 25;
            NPC.defense = 24;
            NPC.lifeMax = 22000;
            NPC.HitSound = new SoundStyle("DestroyerTest/Assets/Audio/NodeHit");
            NPC.DeathSound = new SoundStyle("DestroyerTest/Assets/Audio/NodeExplode");
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.timeLeft = 150000;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.netUpdate = true;
            NPC.netID = ModContent.NPCType<IchorNodeMB>();
            NPC.BossBar = ModContent.GetInstance<CrimsonBossBar>();
            NPC.alpha = 255;
            NPC.friendly = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("Elemental Constructs that strengthen the potency of Cursed Flames and Ichor."),
                new FlavorTextBestiaryInfoElement("They are often times found idly floating above the ground. Though the nodes will become retaliatory if provoked."),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCrimson == true && spawnInfo.Player.ZoneOverworldHeight == true && DownedBossSystem.downedPlanteraBoss == true)
            {
                return 0.01f;
            }
            return 0f;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return !DTUtils.NodeCharmEquipped;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.friendly)
                return !DTUtils.NodeCharmEquipped; // prevent friendly damage when charm is equipped

            // hostile projectiles behave normally
            return null;
        }

        public enum AttackState
        {
            Dormant,
            Idle,
            BloodRain,
            IchorSpiral,
            ToothBombs,
            GroundSlam
        }

        public AttackState CurrentAttack;

        public int DormantPulseTimer = 60;
        public int IdleTimer = 60;
        public int BloodRainSpawnTimer = 180;
        public int BloodRainWaitTimer = 240;
        public int IchorSpiralTimer = 240;
        public int IchorSpiralCooldownTimer = 120;
        public float IchorSpiralRotationOffset = 0;
        public int MineInterval = 0;
        public int MineCount = 0;
        public int MineCooldown = 240;
        public int SlamCharge = 120;
        public int SlamCount = 0;
        public int WaveTimer = 0;
        public int WaveIndex = 0;
        public bool SoundFlag1 = false;
        public SoundStyle StarShoot = new SoundStyle("DestroyerTest/Assets/Audio/NodeAttackTS") with { MaxInstances = 0, PitchVariance = 1, Volume = 2 };
        public SoundStyle Wallwarn = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/CursedFlamesWarn") with { MaxInstances = 0, PitchVariance = 1 };
        public SoundStyle WallShoot1 = new SoundStyle("DestroyerTest/Assets/Audio/FlameWall") with { MaxInstances = 0, PitchVariance = 1, Volume = 2 };
        public SoundStyle NapalmShoot = new SoundStyle("DestroyerTest/Assets/Audio/NodeAttackNapalm") with { MaxInstances = 0, PitchVariance = 1 };
        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            DTUtils Utility = new DTUtils();
            DTConfig cfg = ModContent.GetInstance<DTConfig>();

            if (NPC.alpha > 0)
            {
                NPC.immortal = true;
                NPC.alpha--;
            }
            else
            {
                NPC.immortal = false;
            }

            if (player.active == false || player.dead == true)
            {
                CurrentAttack = AttackState.Dormant;
            }

            if (CurrentAttack != AttackState.GroundSlam)
            {
                TryFindTileBelow();
            }

            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/NodeBoss");
            }
            if (!Main.dedServ && CurrentAttack == AttackState.Dormant && cfg.NodeIdleMusic == true)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/NodeIdle");
            }
            if (!Main.dedServ && CurrentAttack == AttackState.Dormant && cfg.NodeIdleMusic == false)
            {
                Music = MusicID.Crimson;
            }

            Vector2 PRTPos;
            PRTPos = NPC.Center;

            switch (CurrentAttack)
            {
                case AttackState.Dormant:
                    {
                        DormantPulseTimer--;
                        if (DormantPulseTimer <= 0)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse, NPC.Center);
                            PRTLoader.NewParticle(PRTLoader.GetParticleID<BloomRingSharp>(), NPC.Center, Vector2.Zero, ColorLib.Ichor, 1f);
                            DormantPulseTimer = 120;
                        }

                        float bobSpeed = 0.03f;
                        float bobHeight = 16f;
                        NPC.velocity.Y = (float)Math.Sin(Main.GameUpdateCount * bobSpeed) * 0.5f;
                        NPC.position.Y += (float)Math.Sin(Main.GameUpdateCount * bobSpeed) * (bobHeight / 100f);

                        foreach (NPC npc in Main.npc)
                        {
                            if (npc.Center.Distance(NPC.Center) < 1000
                            && npc.type != ModContent.NPCType<IchorNodeMB>()
                            && npc.type != ModContent.NPCType<CursedFlameNodeMB>()
                            && npc.type != ModContent.NPCType<IchorNode>()
                            && npc.type != ModContent.NPCType<CursedFlameNode>())
                            {
                                npc.AddBuff(ModContent.BuffType<NodePower>(), 60);
                            }
                        }

                        foreach (Player p in Main.player)
                        {
                            if (p.Center.Distance(NPC.Center) < 1000 && DTUtils.NodeCharmEquipped)
                            {
                                p.AddBuff(ModContent.BuffType<NodePower>(), 60);
                            }
                        }
                        
                        if (NPC.justHit && !DTUtils.NodeCharmEquipped)
                        {
                            CurrentAttack = AttackState.Idle;
                        }
                        break;
                    }
                case AttackState.Idle:
                    {
                        KeepToPlayer(player.Center + new Vector2(0, -200));
                        if (IdleTimer > 0)
                        {
                            IdleTimer--;
                        }

                        if (IdleTimer <= 0)
                        {
                            CurrentAttack = AttackState.BloodRain;
                            IdleTimer = 60;
                        }
                        break;
                    }
                case AttackState.BloodRain:
                    {
                        KeepToPlayer(player.Center + new Vector2(0, -200));
                        if (BloodRainSpawnTimer > 0)
                        {
                            if (BloodRainSpawnTimer % 8 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item66, NPC.Center);
                                Vector2 Position = new Vector2(
                                    player.Center.X + Main.rand.Next(-200, 200),
                                    player.Center.Y - 400f // blanket above
                                );
                                Projectile.NewProjectile(Entity.GetSource_FromThis(), Position, Vector2.Zero, ModContent.ProjectileType<BloodCloud>(), 16, 8);
                            }
                            BloodRainSpawnTimer--;
                        }
                        if (BloodRainSpawnTimer <= 0 && BloodRainWaitTimer > 0)
                        {
                            BloodRainWaitTimer--;
                        }
                        if (BloodRainSpawnTimer <= 0 && BloodRainWaitTimer <= 0)
                        {
                            CurrentAttack = AttackState.IchorSpiral;
                            BloodRainSpawnTimer = 180;
                            BloodRainWaitTimer = 240;
                        }
                        break;
                    }
                case AttackState.IchorSpiral:
                    {
                        NPC.velocity = Vector2.Zero;
                        if (IchorSpiralTimer > 0)
                        {
                            IchorSpiralRotationOffset += 1f;
                            //var launchVelocity = new Vector2(-8, 0);
                            NPC.rotation = IchorSpiralRotationOffset;

                            if (IchorSpiralTimer % 4 == 0)
                            {
                                SoundEngine.PlaySound(StarShoot, NPC.Center);

                                for (int i = 0; i < 6; i++)
                                {
                                    var angle = IchorSpiralRotationOffset + (i * MathHelper.TwoPi / 6f);
                                    var launchVelocity = new Vector2(8, 0).RotatedBy(angle);
                                    Projectile Crys = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), NPC.Center, launchVelocity, ModContent.ProjectileType<IchorNodeCrystal2>(), 15, 4);
                                    Crys.timeLeft = 120;
                                }

                                IchorSpiralRotationOffset += 1f; // spiral effect
                            }
                            IchorSpiralTimer--;
                        }
                        if (IchorSpiralTimer <= 0 && IchorSpiralCooldownTimer > 0)
                        {
                            IchorSpiralCooldownTimer--;
                        }
                        if (IchorSpiralTimer <= 0 && IchorSpiralCooldownTimer <= 0)
                        {
                            CurrentAttack = AttackState.ToothBombs;
                            IchorSpiralTimer = 240;
                            IchorSpiralCooldownTimer = 120;
                            NPC.rotation = 0f;
                        }
                        break;
                    }
                case AttackState.ToothBombs:
                    {
                        KeepToPlayer(player.Center + new Vector2(0, -200));

                        if (MineInterval > 0)
                        {
                            MineInterval--;
                        }

                        if (MineInterval <= 0)
                        {
                            for (int q = 0; q < 6; q++)
                            {
                                Vector2 Position = NPC.Center + new Vector2(Main.rand.Next(-400, 400), Main.rand.Next(-400, 400));
                                Vector2 Velocity = Position - NPC.Center;
                                Projectile Mine = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), NPC.Center, Velocity * 0.03f, ModContent.ProjectileType<CrystalBomb>(), 30, 5);
                                Mine.timeLeft = 120;
                            }
                            MineInterval = 120;
                            MineCount += 1;
                        }

                        if (MineCount >= 3)
                        {
                            MineCooldown--;
                        }

                        if (MineCount >= 3 && MineCooldown <= 0)
                        {
                            CurrentAttack = AttackState.GroundSlam;
                            MineCount = 0;
                            MineCooldown = 240;
                        }
                        break;
                    }
                case AttackState.GroundSlam:
                    {
                        if (SlamCharge > 0)
                        {
                            NPC.noTileCollide = true;
                            Vector2 toTarget = new Vector2(player.Center.X, player.Center.Y - 180f) - NPC.Center;
                            float speed = 10f;
                            NPC.velocity = toTarget.SafeNormalize(Vector2.Zero) * speed;
                            SlamCharge--;
                        }
                        if (SlamCharge <= 0)
                        {
                            if (SoundFlag1 == false)
                            {
                                SoundEngine.PlaySound(SoundID.Item63, NPC.Center);
                                SoundFlag1 = true;
                            }
                            Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y + NPC.height / 2), 2, 2, DustID.Ichor, 2f, -1.5f, 0, ColorLib.Ichor, 2f);
                            Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y + NPC.height / 2), 2, 2, DustID.Ichor, -2f, -1.5f, 0, ColorLib.Ichor, 2f);
                            PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), new Vector2(NPC.Center.X, NPC.Center.Y + NPC.height / 2), new Vector2(2, 1.5f), ColorLib.Ichor, 1.0f);
                            PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), new Vector2(NPC.Center.X, NPC.Center.Y + NPC.height / 2), new Vector2(-2, 1.5f), ColorLib.Ichor, 1.0f);
                            NPC.noTileCollide = false;
                            NPC.velocity.Y = 0f;
                            NPC.velocity.Y = 24f;
                            NPC.velocity.X = 0f;
                        }
                        if (NPC.collideY && NPC.velocity.Y >= 0f)
                        {
                            NPC.velocity = Vector2.Zero;
                            SoundEngine.PlaySound(SoundID.Item88, NPC.Center);
                            player.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 10;
                            player.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = 2;
                            SlamWave();
                            SlamSpray();
                            SlamCharge = 120;
                            SlamCount += 1;
                        }
                        if (SlamCount > 5)
                        {
                            CurrentAttack = AttackState.Idle;
                            SlamCharge = 120;
                            SlamCount = 0;
                        }
                        break;
                    }

                
            }
        }

        public void TryFindTileBelow()
        {
            Vector2 Probe = NPC.Center + new Vector2(0, 400);

            int left = (int)(Probe.X / 16);
            int right = (int)((Probe.X + NPC.width) / 16);
            int top = (int)(Probe.Y / 16);
            int bottom = (int)((Probe.Y + NPC.height) / 16);

            bool surrounded =
                Collision.SolidTiles(left - 1, right + 1, top - 1, bottom + 1) &&
                Collision.SolidTiles(left, right, top - 1, top - 1) && // Above
                Collision.SolidTiles(left, right, bottom + 1, bottom + 1) && // Below
                Collision.SolidTiles(left - 1, left - 1, top, bottom) && // Left
                Collision.SolidTiles(right + 1, right + 1, top, bottom); // Right

            if (surrounded)
            {
                NPC.velocity.Y = -6f;
                NPC.Center += new Vector2(0, -16);
            }
        }

        public bool TryFindAirTile(Vector2 Probe, out bool surrounded)
        {
            int left = (int)(Probe.X / 16);
            int right = (int)((Probe.X + 32) / 16);
            int top = (int)(Probe.Y / 16);
            int bottom = (int)((Probe.Y + 32) / 16);

            surrounded =
                Collision.SolidTiles(left - 1, right + 1, top - 1, bottom + 1) &&
                Collision.SolidTiles(left, right, top - 1, top - 1) && // Above
                Collision.SolidTiles(left, right, bottom + 1, bottom + 1) && // Below
                Collision.SolidTiles(left - 1, left - 1, top, bottom) && // Left
                Collision.SolidTiles(right + 1, right + 1, top, bottom); // Right
            if (surrounded)
            {
                return false;
            }
            return true;
        }

        public void KeepToPlayer(Vector2 CTR)
        {
            // Calculate vector from NPC to target
            Vector2 toTarget = CTR - NPC.Center;

            // If distance is too small, slow it down
            float distance = toTarget.Length();
            if (distance > 32f)
            {
                // Move half-way towards target
                NPC.velocity = toTarget * 0.25f;
            }
            else
            {
                // Move slower if close
                NPC.velocity = toTarget * 0.25f;
            }
        }


        public void SlamWave()
        {
            int left = (int)(NPC.position.X / 16);
            int right = (int)((NPC.position.X + NPC.width) / 16);
            int bottom = (int)((NPC.position.Y + NPC.height) / 16);

            WaveTimer++;
            if (WaveTimer % 10 == 0) // spawn interval
            {
                int x = left + WaveIndex;
                if (x <= right && WorldGen.SolidTile(x, bottom))
                {
                    Vector2 spawnPos = new Vector2(x * 16 + 8, bottom * 16);
                    Projectile.NewProjectile(
                        Entity.GetSource_FromThis(),
                        spawnPos,
                        Vector2.Zero,
                        ModContent.ProjectileType<NodeSlam>(),
                        25,
                        4f
                    );
                }

                WaveIndex++; // move to next tile column

                if (x > right)
                {
                    // finished wave
                    WaveIndex = 0;
                    WaveTimer = 0;
                }
            }
        }

        public void SlamSpray()
        {
            for (int f = 0; f < 7; f++)
            {
                Vector2 velo = new Vector2(Main.rand.Next(-10, 10), -12);
                Projectile.NewProjectile(Entity.GetSource_FromThis(), NPC.Center, velo, ProjectileID.GoldenShowerHostile, 15, 4);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CursedNodeLootBag>()));
        }
    }
}