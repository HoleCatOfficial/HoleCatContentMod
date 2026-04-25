
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.BossBar;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.CursedFlame;
using GlowmaskHelper.Content;
using InnoVault;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Entities
{

    [AutoloadBossHead]
    [AutoloadGlowmask]
    public class BlessedNodeMB : ModNPC
    {
        public override string BossHeadTexture => "DestroyerTest/Content/Entities/BlessedNodeMB_Head_Boss";
        public override void SetStaticDefaults()
        {
            NPCID.Sets.CanHitPastShimmer[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
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
            NPC.lifeMax = 60000;
            NPC.HitSound = new SoundStyle("DestroyerTest/Assets/Audio/NodeHit");
            NPC.DeathSound = new SoundStyle("DestroyerTest/Assets/Audio/NodeExplode");
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.timeLeft = 150000;
            NPC.boss = false;
            NPC.npcSlots = 1f;
            NPC.netUpdate = true;
            NPC.netID = ModContent.NPCType<BlessedNodeMB>();
            NPC.alpha = 255;
            NPC.friendly = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),

                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            bool NodeAlive = false;
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type == Type)
                {
                    NodeAlive = true;
                }
            }
            if (spawnInfo.Player.ZoneHallow == true && spawnInfo.Player.ZoneOverworldHeight == true && DownedBossSystem.downedPlanteraBoss == true && !NodeAlive)
            {
                return 0.1f;
            }
            return 0f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            ResetData();
        }

        public void ResetData()
        {
            DormantNPCKillTally = 0;
            BNGlobal.WaveNPCCount = 0;
        }

        public override bool CheckActive()
        {
            return true;
        }

        public float ShieldOpacity = 0f;
        public float ShieldScale = 1f;

        public float LaserWarnOpacity = 0f;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (CurrentAttack == AttackState.Dormant)
            {
                DTUtils.DrawChargeBar(2f, (NPC.Center + new Vector2(0, 100)) - Main.screenPosition, (float)DormantNPCKillTally / (float)DormantNPCKillRequirement, Color.SkyBlue);
            }

            if (CurrentAttack == AttackState.Lasers)
            {
                if (LaserWarnTimer > 0)
                {
                    Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
                    Main.EntitySpriteDraw(DTAssetLib.BlessedNodeLaserTelegraph.Value, NPC.Center - Main.screenPosition, null, Main.DiscoColor * LaserWarnOpacity, LaserRotOffset - 12f, DTAssetLib.BlessedNodeLaserTelegraph.Value.Size() / 2, 1f, SpriteEffects.None);
                    Main.EntitySpriteDraw(DTAssetLib.BlessedNodeLaserTelegraph.Value, NPC.Center - Main.screenPosition, null, Color.White * LaserWarnOpacity, LaserRotOffset - 12f, DTAssetLib.BlessedNodeLaserTelegraph.Value.Size() / 2, 0.65f, SpriteEffects.None);
                    Opus.ReturnToDefaultDrawing(spriteBatch);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            var v = DTAssetLib.BloomRingSharp.Value;

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(v, NPC.Center - screenPos, null, Color.SkyBlue * ShieldOpacity, 0f, v.Size() / 2, ShieldScale, SpriteEffects.None);
            Utils.DrawBorderString(spriteBatch, $"{DormantNPCKillTally} / {DormantNPCKillRequirement}", (NPC.Center + new Vector2(0, -90)) - screenPos, Color.SkyBlue * ShieldOpacity, 3f, 0.5f, 0.5f);
            Opus.ReturnToDefaultDrawing(spriteBatch);
            return true;
        }
        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return !DTFlags.NodeCharmEquipped && !(DormantNPCKillTally < DormantNPCKillRequirement);
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.friendly)
                return !DTFlags.NodeCharmEquipped && !(DormantNPCKillTally < DormantNPCKillRequirement); ; // prevent friendly damage when charm is equipped

            // hostile projectiles behave normally
            return null;
        }




        public enum AttackState
        {
            Dormant,
            Idle,
            CrystalCross,
            SummonKnives,
            Lasers,
            HallowBolts,
            None
        }

        public AttackState CurrentAttack;

        public int DormantPulseTimer = 60;
        public bool HasBuffed = false;
        public bool HasDebuffed = false;
        public int IdleTimer = 60;
        public int DormantNPCKillTally = 0;
        public const int DormantNPCKillRequirement = 50;
        public bool Flag2 = false;

        public int CrossCount = 0;
        public int KnifeCount = 0;
        public float LaserRotOffset = 0;
        public int LaserWarnTimer = 120;
        public int LaserCount = 0;
        public int BoltCount = 0;
        public int DespawnTimer = 60;
        
        public override void AI()
        {

            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            DTUtils Utility = new DTUtils();
            DTMusicConfig muscfg = ModContent.GetInstance<DTMusicConfig>();

            if (NPC.alpha > 0 && CurrentAttack != AttackState.None)
            {
                NPC.dontTakeDamage = true;
                NPC.alpha--;
            }
            else
            {
                NPC.dontTakeDamage = false;
            }

            if (player.active == false || player.dead == true || !NPC.HasValidTarget)
            {
                if (DespawnTimer > 0)
                {
                    DespawnTimer--;
                }
                else
                {
                    CurrentAttack = AttackState.None;
                }
            }


            TryFindTileBelow();

            
            if (!Main.dedServ && CurrentAttack != AttackState.Dormant)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/BlessedNode");
            }
             

            LaserRotOffset += 0.03f;

            if (LaserBurstCol != null)
            {
                if (LaserBurstCol.Length != 0)
                {
                    for (int p = 0; p < LaserBurstCol.Length; p++)
                    {
                        LaserBurstCol[p].ai[1] = LaserRotOffset;
                        LaserBurstCol[p].netUpdate = true;
                    }
                }
            }

            Vector2 PRTPos;
            PRTPos = NPC.Center;

            if ((DormantNPCKillTally < DormantNPCKillRequirement))
            {
                ManageShieldIn();
            }

            if (DormantNPCKillTally >= DormantNPCKillRequirement)
            {
                if (!Flag2)
                {
                    SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Activate, NPC.Center);
                    Flag2 = true;
                }
                ManageShieldOut();
            }

            switch (CurrentAttack)
            {
                case AttackState.Dormant:
                    {

                        DormantAI();
                        if ((DormantNPCKillTally < DormantNPCKillRequirement))
                        {
                            NPC.dontTakeDamage = true;
                        }
                        else
                        {
                            NPC.dontTakeDamage = false;
                        }

                        if (NPC.justHit && !DTFlags.NodeCharmEquipped && !(DormantNPCKillTally < DormantNPCKillRequirement))
                        {
                            CurrentAttack = AttackState.Idle;
                        }
                        break;
                    }
                case AttackState.Idle:
                    {
                        NPC.boss = true;
                        NPC.npcSlots = 10f;
                        KeepToPlayer(player.Center + new Vector2(0, -200));
                        if (IdleTimer > 0)
                        {
                            IdleTimer--;
                        }

                        if (IdleTimer <= 0)
                        {
                            CurrentAttack = AttackState.CrystalCross;
                            IdleTimer = 60;
                        }
                        break;
                    }
                case AttackState.CrystalCross:
                    {
                        NPC.velocity *= 0.8f;
                        if (CrossCount < 6)
                        {
                            if (Main.GameUpdateCount % 60 == 0)
                            {
                                CrystalCross(player);
                                CrossCount++;
                            }
                        }
                        else
                        {
                            CurrentAttack = AttackState.SummonKnives;
                            CrossCount = 0;
                        }
                        break;
                    }
                case AttackState.SummonKnives:
                    {
                        KeepToPlayer(player.Center + new Vector2(0, -300));

                        if (KnifeCount < 6)
                        {
                            if (Main.GameUpdateCount % 240 == 0)
                            {
                                for (int o = 0; o < 3; o++)
                                {
                                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BlessedNodeFlyingKnife>());
                                }
                                KnifeCount++;
                            }
                        }
                        else
                        {
                            CurrentAttack = AttackState.Lasers;
                            KnifeCount = 0;
                        }
                        break;
                    }
                case AttackState.Lasers:
                    {
                        
                        NPC.velocity *= 0.8f;

                        

                        if (LaserCount < 3)
                        {
                            if (LaserWarnTimer == 119)
                            {
                                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/BlessedNodeLasersCharge"), NPC.Center);
                            }
                            if (LaserWarnTimer > 0)
                            {
                                float t = Utilities.Convert01To010((LaserWarnTimer / 120f));
                                LaserWarnOpacity = MathHelper.Lerp(0f, 1f, t);

                                Opus.RadialSpreadDust(DustID.AncientLight, 6, NPC.Center, 0, Main.DiscoColor, 1f, 5f, offset: LaserRotOffset);
                                LaserWarnTimer--;
                            }
                            else
                            {
                                LaserBurst();
                                LaserCount++;
                                LaserWarnTimer = 120;
                            }
                        }
                        else
                        {
                            CurrentAttack = AttackState.HallowBolts;
                            LaserCount = 0;
                            LaserWarnTimer = 120;
                        }
                        break;
                    }
                case AttackState.HallowBolts:
                    {
                        KeepToPlayer(player.Center + new Vector2(0, -300));

                        Vector2 toPlayer = player.Center - NPC.Center;
                        toPlayer.Normalize();
                        if (BoltCount < 4)
                        {
                            if (Main.GameUpdateCount % 240 == 0)
                            {
                                SoundEngine.PlaySound(DTAssetLib.Impacts.AmbitionChargeBurst with { PitchRange = (-0.4f, -0.1f), MaxInstances = 0 }, NPC.Center);
                                int amt = 3;
                                if (Main.expertMode && !Main.masterMode)
                                {
                                    amt = 5;
                                }
                                if (Main.masterMode)
                                {
                                    amt = 7;
                                }
                                for (int o = 0; o < amt; o++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, toPlayer.RotatedByRandom(0.5f) * 10, ModContent.ProjectileType<HallowBolt>(), 30, 15);
                                }
                                BoltCount++;
                            }
                        }
                        else
                        {
                            CurrentAttack = AttackState.Idle;
                            BoltCount = 0;
                        }
                        break;
                    }
                case AttackState.None:
                    {
                        NPC.velocity *= 0.8f;

                        if (NPC.alpha < 255)
                        {
                            NPC.dontTakeDamage = true;
                            NPC.alpha++;
                        }
                        else
                        {
                            NPC.active = false;
                        }
                        break;
                    }
                
            }
        }

        public void ManageShieldIn()
        {
            if (ShieldScale > 0.1f)
            {
                ShieldScale -= 0.01f;
            }
            if (ShieldOpacity < 1)
            {
                ShieldOpacity += 0.05f;
            }
        }

        public void ManageShieldOut()
        {
            if (ShieldScale < 1f)
            {
                ShieldScale += 0.05f;
            }
            if (ShieldOpacity > 0)
            {
                ShieldOpacity -= 0.1f;
            }
        }

        public void DormantAI()
        {
            DormantPulseTimer--;
            if (DormantPulseTimer <= 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse, NPC.Center);
                Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), NPC.Center, Vector2.Zero, Color.SkyBlue, 0.01f, 1f);
                DormantPulseTimer = 120;
            }

            NPC.velocity.Y = Opus.Sine(1f, -1f, 0.01f);


            foreach (NPC npc in Main.npc)
            {
                if (npc.Center.Distance(NPC.Center) < 1000
                && npc.type != ModContent.NPCType<IchorNodeMB>()
                && npc.type != ModContent.NPCType<CursedFlameNodeMB>()
                && npc.type != ModContent.NPCType<IchorNode>()
                && npc.type != ModContent.NPCType<CursedFlameNode>()
                && npc.type != ModContent.NPCType<BlessedNodeMB>())
                {
                    npc.AddBuff(ModContent.BuffType<NodePower>(), 60);
                }
            }

            Vector2[] P = Opus.GetEquidistantOrbitVectors(16, NPC.Center, 0.1f, 1200);

            for (int i = 0; i < P.Length; i++)
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), P[i], Vector2.Zero, Color.SkyBlue, 1f);
            }

            foreach (Player p in Main.player)
            {
                if (p.Center.Distance(NPC.Center) < 1200)
                {
                    if (DTFlags.NodeCharmEquipped)
                    {
                        p.AddBuff(ModContent.BuffType<NodePower>(), 60);
                    }

                    if (DormantNPCKillTally < DormantNPCKillRequirement)
                    {
                        SpawnNPCWave();
                    }
                }
            }
        }

        public int SpawnNPCTimer = 0;
        public static string NPCIdentifierContext = "BlessedNodeWaveEnemy";

        public static List<int> BlessedNodeWaveEnemies = new List<int>
        {
            NPCID.Pixie,
            NPCID.Unicorn,
            NPCID.RainbowSlime,
            NPCID.Gastropod
        };

        public int WaveTimeout = 0;
        public int ClickCooldown = 60;
        public void SpawnNPCWave()
        {
            SpawnNPCTimer++;
            WaveTimeout++;

            if (ClickCooldown > 0)
            {
                ClickCooldown--;
            }

            Vector2[] SpawnPositions = Opus.GetEquidistantVectors(5, NPC.Center, 250);

            if ((SpawnNPCTimer % 300 == 0 && BNGlobal.WaveNPCCount == 0) || WaveTimeout > 1800)
            {
                if (Main.netMode == NetmodeID.MultiplayerClient || Main.netMode == NetmodeID.SinglePlayer)
                {
                    if (Main.MouseWorld.Distance(NPC.Center) < 20 && ClickCooldown <= 0)
                    {
                        if (Main.LocalPlayer.controlUseTile)
                        {
                            SoundEngine.PlaySound(SoundID.Item129);
                            WaveTimeout = 1801;
                            ClickCooldown = 60;
                        }
                    }
                }

                if (WaveTimeout > 1800)
                {
                    CombatText.NewText(NPC.Hitbox, Color.Red, "Wave failsafe intiated.");
                    Main.NewText("TALID: Wave failsafe intiated.", Color.Red);

                    foreach (NPC child in Main.npc)
                    {
                        if (!child.active) continue;

                        var g = child.GetGlobalNPC<BNGlobal>();

                        if (g.IsNodeSpawned && g.Node == this)
                        {
                            child.StrikeInstantKill();
                        }
                    }

                    DormantNPCKillTally = ((DormantNPCKillTally + 9) / 10) * 10;
                }
                WaveTimeout = 0;

                

                SoundEngine.PlaySound(DTAssetLib.Impacts.DarkMagicImpact);
                for (int i = 0; i < SpawnPositions.Length; i++)
                {
                    Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), SpawnPositions[i], Vector2.Zero, Color.SkyBlue, 0.01f, 0.4f);
                    NPC wavenpc = NPC.NewNPCDirect(NPC.GetSource_FromAI(NPCIdentifierContext), SpawnPositions[i], BlessedNodeWaveEnemies[Main.rand.Next(BlessedNodeWaveEnemies.Count)]);
                    wavenpc.scale = 1.5f;
                    wavenpc.knockBackResist = 0f;
                    var g = wavenpc.GetGlobalNPC<BNGlobal>();
                    g.Node = this;
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

        public void TryFindAirTile(Vector2 Probe, out bool surrounded)
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

        public void CrystalCross(Player target)
        {
            SoundEngine.PlaySound(SoundID.Item9, target.Center);
            if (!Main.masterMode)
            {
                Opus.RingProjectileInward(ModContent.ProjectileType<BlessedNodeCrystal2>(), 4, target.Center, 200, 30, 5, 1, RandomOffset: true);
            }
            else
            {
                Opus.RingProjectileInward(ModContent.ProjectileType<BlessedNodeCrystal2>(), 8, target.Center, 360, 30, 5, 1, RandomOffset: true);
            }
        }

        Projectile[] LaserBurstCol;
        public void LaserBurst()
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/BlessedNodeLasers"), NPC.Center);
            LaserBurstCol = Opus.RadialSpreadProjectile(ModContent.ProjectileType<BlessedLaser>(), 6, NPC.Center, 80, 1, 0.005f, offset: LaserRotOffset);

            if (Main.expertMode && !Main.masterMode)
            {
                Opus.RingProjectileOutward(ModContent.ProjectileType<BlessedNodeCrystal2>(), 16, NPC.Center, 10, 20, 4, 1, RandomOffset: true);
            }
            if (Main.masterMode)
            {
                Opus.RingProjectileOutward(ModContent.ProjectileType<BlessedNodeCrystal2>(), 24, NPC.Center, 10, 20, 4, 1, RandomOffset: true);
            }
        }
        

        public override void OnKill()
        {
            BNGlobal.WaveNPCCount = 0;
            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BlessedNodeDeathProjectile>(), 100, 0);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BlessedNodeLootBag>()));
        }
    }

    public class BNGlobal : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public static int WaveNPCCount = 0;

        public bool IsNodeSpawned = false;

        public BlessedNodeMB Node = null;

        public override void Unload()
        {
            WaveNPCCount = 0;
            IsNodeSpawned = false;
            Node = null;
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Context == BlessedNodeMB.NPCIdentifierContext)
            {
                WaveNPCCount += 1;
                IsNodeSpawned = true;
            }

        }

        public int TexOffset = 0;
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (IsNodeSpawned)
            {
                Line L = new Line(npc.Center, Node.NPC.Center);
                TexOffset += 10;
                DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(10), Color.SkyBlue, spriteBatch, BlendState.Additive, TexOffset, 0.5f);
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public float rOff = 0f;
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            rOff += 0.05f;
            if (IsNodeSpawned)
            {
                Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
                Main.EntitySpriteDraw(DTAssetLib.HallowedSigil.Value, npc.Center - screenPos, null, Color.SkyBlue * 0.5f, rOff, DTAssetLib.HallowedSigil.Value.Size() / 2, 0.5f, SpriteEffects.None, 0f);
                Opus.ReturnToDefaultDrawing(spriteBatch);
            }
        }

        public override void AI(NPC npc)
        {


        }

        public override bool CheckActive(NPC npc)
        {
            if (IsNodeSpawned)
            {
                return false;
            }
            return true;
        }

        public override void OnKill(NPC npc)
        {
            if (IsNodeSpawned)
            {
                if (Node != null)
                {
                    Node.DormantNPCKillTally += 1;
                }
                WaveNPCCount--;
            }

            if (npc.type == ModContent.NPCType<BlessedNodeMB>())
            {
                if (Node != null)
                {
                    Node.DormantNPCKillTally = 0;
                }
                WaveNPCCount = 0;
            }
        }
    }
}