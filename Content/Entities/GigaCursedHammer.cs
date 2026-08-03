using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.RiftBiome;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using GlowmaskHelper.Content;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.CursedFlame;
using OpusLib;
using OpusLib.Content.Particles;

namespace DestroyerTest.Content.Entities
{
    [AutoloadHead]
    [AutoloadGlowmask]
    public class GigaCursedHammer : ModNPC
    {
        public override void SetStaticDefaults()
        {
            
        }

        public override void SetDefaults()
        {
            NPC.width = 164;
            NPC.height = 164;
            NPC.aiStyle = 23;
            NPC.damage = 100;
            NPC.defense = 999;
            NPC.lifeMax = 160;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.dontTakeDamage = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("A willful construct of the Corruption. It is made as a deirect upgrade to the cursed hammers that failed to stop you before."),
                new FlavorTextBestiaryInfoElement("The Corruption posesses more intellect than you thought."),
            });
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return false;
        }

        public override bool CanBeHitByNPC(NPC attacker)
        {
            return false;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return false;
        }




        public int AttackCharge = 0;
        public int FlameShootTimer = 240;
        public bool Stunned = false;
        public int StunTimer = 120;
        public float rotationOffset = 0f;
        public SoundStyle WallShoot = new SoundStyle("DestroyerTest/Assets/Audio/FlameWall") with { MaxInstances = 0, PitchVariance = 1, Volume = 2 };
        public SoundStyle ChargeBreak = new SoundStyle("DestroyerTest/Assets/Audio/ChargeBreak") with { MaxInstances = 0, PitchVariance = 1, Volume = 2 };

        public override void AI()
        {
            bool ParentAlive = Main.npc.Any(n => n.active && n.type == ModContent.NPCType<NightmareRoseBoss>());

            if (ParentAlive)
            {
                NPC.active = true;
            }
            else
            {
                NPC.StrikeInstantKill();
                NPC.active = false;
            }

            if (AttackCharge < 0)
            {
                AttackCharge = 0;
            }

            AttackCharge++;
            if (Main.GameUpdateCount % 20 == 0)
            {
                var Ring = Opus.RingRadialVectorRandom(8, NPC.Center, 200, 0.1f);
                for (int a = 0; a < 8; a++)
                {
                    Fire F = new Fire();
                    F.PrepareFire(Ring.Item1[a], Ring.Item2[a], DTUtils.RandomDirection(2), ColorLib.CursedFlames, 0.7f, 20, FireDrawMode.Additive, BreadLibrary.Core.Graphics.Pixelation.PixelLayer.AboveNPCs);
                }
            }

            if (AttackCharge >= 300)
            {
                NPC.velocity = Vector2.Zero;

                SoundEngine.PlaySound(WallShoot, NPC.Center);
                
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<CursedFlameVortex>(), 4, NPC.Center, 20, 5, 10, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                
                AttackCharge = 0;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.PostDraw(spriteBatch, screenPos, drawColor);

            Vector2 drawPos = NPC.Center - screenPos;
            drawPos.Y -= 200;

            string text = AttackCharge.ToString();

            Utils.DrawBorderString(spriteBatch, text, drawPos, ColorLib.CursedFlames, 2f, 0.5f, 0.5f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.CursedInferno, 300);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Cursed, 600);
            }
        }
    }
}