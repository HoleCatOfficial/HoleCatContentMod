using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.RiftBiome;
using InnoVault.PRT;
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

namespace DestroyerTest.Content.Entity
{
    [AutoloadHead]
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
            NPC.defense = 9;
            NPC.lifeMax = 160;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.immortal = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("A willful construct of the Corruption. It is made as a deirect upgrade to the cursed hammers that failed to stop you before."),
                new FlavorTextBestiaryInfoElement("The Corruption posesses more intellect than you thought."),
            });
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

            if (!Stunned)
            {
                AttackCharge++;
                if (Main.GameUpdateCount % 20 == 0)
                {
                    for (int a = 0; a < 3; a++)
                    {
                        Vector2 Edge = Main.rand.NextVector2CircularEdge(600, 600);
                        Vector2 Inward = NPC.Center - Edge;
                        Inward.Normalize();
                        PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Edge, Inward  * 0.01f, ColorLib.CursedFlames, 4.0f);
                    }
                }
            }
            

            if (NPC.justHit && AttackCharge > 20 && !Stunned)
            {
                AttackCharge -= 20;
            }

            if (NPC.justHit && AttackCharge > 150 && !Stunned)
            {
                SoundEngine.PlaySound(ChargeBreak, NPC.Center);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<Boom1>(), NPC.Center, Vector2.Zero, ColorLib.CursedFlames, 3.0f);
                CombatText.NewText(NPC.getRect(), ColorLib.CursedFlames, "Charge Broken!", true, false);
                AttackCharge = 0;
                Stunned = true;
            }

            if (Stunned)
            {
                NPC.velocity = Vector2.Zero;
                StunTimer--;
                if (StunTimer <= 0)
                {
                    Stunned = false;
                    StunTimer = 120;
                }
            }

            if (AttackCharge >= 300 && !Stunned)
                {
                    NPC.velocity = Vector2.Zero;

                    SoundEngine.PlaySound(WallShoot, NPC.Center);
                    Projectile.NewProjectile(Entity.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CursedFlameWallVertical>(), 25, 4);
                    Projectile.NewProjectile(Entity.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CursedFlameWallHorizontal>(), 25, 4);
                    
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