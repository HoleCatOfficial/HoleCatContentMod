using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Projectiles.EntitiesProjectiles;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.Scepter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace DestroyerTest.Content.Entities
{

    public class ProjectileShootingDummy : ModNPC
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.aiStyle = -1;
            NPC.defense = 10;
            NPC.lifeMax = 100;
            NPC.DeathSound = SoundID.NPCDeath43;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.damage = 20;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Chases you and shoots projectiles. Used for testing.")
            });
        }

        public Line Dir;
        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            Dir = new Line(NPC.Center, player.Center);

            NPC.ai[0]++;

            int t = (int)NPC.ai[0];

            if (t < 300)
            {
                NPC.velocity = Dir.GetLineRotation.ToRotationVector2() * Opus.Sine(1f, 2f);
                if (t % 60 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Zombie103, NPC.Center);
                    Shoot();
                }
            }
        }

        private void Shoot()
        {
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Dir.GetLineRotation.ToRotationVector2() * 10, ProjectileID.WoodenArrowHostile, NPC.damage, 2);
        }
    }
}