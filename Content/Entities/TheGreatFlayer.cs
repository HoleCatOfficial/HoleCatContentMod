using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.RiftBiome;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using ReLogic.Content;
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
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;

namespace DestroyerTest.Content.Entities
{
    [AutoloadHead]
    [AutoloadGlowmask]
    public class TheGreatFlayer : ModNPC
    {



        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            NPC.width = 160;
            NPC.height = 160;
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

        public override void Load()
        {
            if (!Main.dedServ)
            {
                // This gives you a valid texture index
                int headSlot = Mod.AddBossHeadTexture("DestroyerTest/Content/Entities/TheGreatFlayer_Head", ModContent.NPCType<TheGreatFlayer>());

                // Assign it to the NPC type
                NPCID.Sets.BossHeadTextures[ModContent.NPCType<TheGreatFlayer>()] = headSlot;
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("A gruesome construct of the crimson. It is made to functionally and visually resemble a sword you yourself would wield."),
                new FlavorTextBestiaryInfoElement("The crimson has greater learning capabilities than you previously thought."),
            });
        }

        public int AttackCharge = 0;
        public int ToothShootTimer = 240;
        public bool Stunned = false;
        public int StunTimer = 120;
        public float rotationOffset = 0f;
        public SoundStyle StarShoot = new SoundStyle("DestroyerTest/Assets/Audio/NodeAttackTS") with { MaxInstances = 0, PitchVariance = 1, Volume = 2 };
        public SoundStyle ChargeBreak = new SoundStyle("DestroyerTest/Assets/Audio/ChargeBreak") with { MaxInstances = 0, PitchVariance = 1, Volume = 2 };

        public override void AI()
        {
            bool ParentAlive = Main.npc.Any(n => n.active && n.type == ModContent.NPCType<WyvernCorpseHead>());

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
                        //PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Edge, Inward  * 0.1f, ColorLib.Ichor, 1.0f);
                    }
                }
            }
            

            if (NPC.justHit && AttackCharge > 20 && !Stunned)
            {
                AttackCharge -= 20;
            }

            if (NPC.justHit && AttackCharge > 300 && !Stunned)
            {
                SoundEngine.PlaySound(ChargeBreak, NPC.Center);
                CombatText.NewText(NPC.getRect(), ColorLib.Ichor, "Charge Broken!", true, false);
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

            if (AttackCharge >= 600 && !Stunned)
                {
                    if (ToothShootTimer > 0)
                    {
                        rotationOffset += 1f;
                        NPC.velocity = Vector2.Zero;

                        if (ToothShootTimer % 4 == 0)
                        {
                            SoundEngine.PlaySound(StarShoot, NPC.Center);

                            for (int i = 0; i < 6; i++)
                            {
                                var angle = rotationOffset + (i * MathHelper.TwoPi / 6f);
                                var launchVelocity = new Vector2(8, 0).RotatedBy(angle);
                                Projectile.NewProjectile(Entity.GetSource_FromThis(), NPC.Center, launchVelocity, ModContent.ProjectileType<Tooth>(), 25, 4);
                            }

                            rotationOffset += 1f;
                        }
                        ToothShootTimer--;
                    }
                    if (ToothShootTimer <= 0)
                    {
                        AttackCharge = 0;
                        ToothShootTimer = 120;
                    }
                }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.PostDraw(spriteBatch, screenPos, drawColor);

            Vector2 drawPos = NPC.Center - screenPos;
            drawPos.Y -= 200;

            string text = AttackCharge.ToString();

            Utils.DrawBorderString(spriteBatch, text, drawPos, ColorLib.Ichor, 2f, 0.5f, 0.5f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Ichor, 300);
            target.AddBuff(BuffID.Bleeding, 300);
        }

        
    }
}