using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.Utilities;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Common;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.ItemDropRules;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using Terraria.GameContent;
using OpusLib;
using Terraria.Audio;
using System;
using Terraria.DataStructures;

namespace DestroyerTest.Content.Entities
{
    public class BlessedNodeFlyingKnife : ModNPC
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 34;
            NPC.damage = 20;
            NPC.defense = 15;
            NPC.lifeMax = 300;
            NPC.value = 70f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = NPCAIStyleID.Corite;
            NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
            NPC.DeathSound = SoundID.Item84;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BlueCrystalShard);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.PinkCrystalShard);
                }
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Circular(6, 6), 99);
                }
            }
        }
    }
}