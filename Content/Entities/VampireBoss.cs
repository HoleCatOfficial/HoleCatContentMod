using DestroyerTest.Common;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.Buffs;
using log4net.Repository.Hierarchy;
using Microsoft.Build.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Steamworks;
using System;
using System.Collections;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using System.Collections.Generic;
using System.Linq;
using ReLogic.Content;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;

namespace DestroyerTest.Content.Entities
{
    [AutoloadBossHead]
    public class VampireBoss : ModNPC
    {
        public override string BossHeadTexture => "DestroyerTest/Content/Entities/VampireBoss_Head_Boss";
        public override void SetStaticDefaults()
        {
            NPCID.Sets.CanHitPastShimmer[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Burning] = false;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = false;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Oiled] = false;
            NPCID.Sets.TrailCacheLength[Type] = 20;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { // Influences how the NPC looks in the Bestiary
                CustomTexturePath = "DestroyerTest/Content/Entities/VampireBossBestiary", // If the NPC is multiple parts like a worm, a custom texture for the Bestiary is encouraged.
                Position = Vector2.Zero,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SetDefaults()
        {
            NPC.width = 70;
            NPC.height = 64;
            NPC.aiStyle = -1;
            NPC.damage = 24;
            NPC.defense = 17;
            NPC.lifeMax = 600;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = false;
            NPC.knockBackResist = 0f;
            NPC.timeLeft = 150000;
            NPC.boss = true;
            NPC.npcSlots = 10f;
            
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface
            });
        }

        public override bool CheckActive()
        {
            return false;
        }

        public enum AttackState
        {
            Idle,
            ShootBlood,
            BrethrenBats,
            GoldenShots,
            ChargeNoBite,
            ChargeAndBite,
            DarkHands,
            GetHelpFromMommyEimvur
        }

        public AttackState currentState = AttackState.Idle;
        public Vector2 PlayerCenter = Vector2.Zero;
        public Vector2 DirectionToPlayerCenter = Vector2.Zero;
        public int BloodShotAmount = 4;
        public int BloodShotTimer = 0;
        public int BloodShotCount = 0;
        public int BatTimer = 0;
        public int BatCount = 0;

        public override void OnSpawn(IEntitySource source)
        {
            currentState = AttackState.Idle;
        }


        public override void AI()
        {

            Player player = Main.LocalPlayer;

            DirectionToPlayerCenter = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);

           

            PlayerCenter = player.Center;

            if (!Main.dedServ) {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/UnfinishedBoss");
            }

            switch (currentState)
            {
                case AttackState.Idle:
                    if (NPC.type == ModContent.NPCType<VampireBoss>())
                    {
                        NPC.aiStyle = 10;

                    }
                    break;

            }
        }


        public void ShootBlood(NPC npc, Player player)
        {
            if (npc.type == ModContent.NPCType<VampireBoss>())
            {
                for (int i = 0; i < BloodShotAmount; i++)
                {
                    Projectile Blood = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), npc.Center, DirectionToPlayerCenter * 28f, ModContent.ProjectileType<BloodProjectile>(), 4, 1);
                }
            }
        }

        public void BreathrenBatSpawn(NPC npc, Player player)
        {
            if (npc.type == ModContent.NPCType<VampireBoss>())
            {
                Vector2 PlayerLeft = player.Center + new Vector2(-50, 0);
                Vector2 PlayerRight = player.Center + new Vector2(50, 0);

                Dust.NewDust(PlayerLeft, 5, 5, DustID.Blood, 0f, 0f, 0, default, 1.2f);
                Dust.NewDust(PlayerRight, 5, 5, DustID.Blood, 0f, 0f, 0, default, 1.2f);

                Projectile BatLeft = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), PlayerLeft, DirectionToPlayerCenter, ProjectileID.BatOfLight, 8, 1);
                Projectile BatRight = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(), PlayerLeft, DirectionToPlayerCenter, ProjectileID.BatOfLight, 8, 1);

            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (currentState == AttackState.ChargeAndBite)
            {
            target.AddBuff(BuffID.Bleeding, 480);
            }
        }

    }
}
