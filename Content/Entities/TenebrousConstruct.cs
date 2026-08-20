using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Boss;
using DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss;
using DestroyerTest.Content.Projectiles.Boss.TenebrousConstruct;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.RogueItems;
using DestroyerTest.Content.SHADEMANAGEMENT;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tools;
using Humanizer;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using OpusLib;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Cinematics;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using UtfUnknown.Core.Models.SingleByte.Finnish;

namespace DestroyerTest.Content.Entities
{
    [AutoloadBossHead]
    public class TenebrousConstruct : ModNPC, IDrawPixelated
    {

        public override void SetStaticDefaults()
        {
            immunities();
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f,
                Direction = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            Main.npcFrameCount[Type] = 55;

            NPCID.Sets.TrailCacheLength[Type] = 100;
            NPCID.Sets.TrailingMode[Type] = 3;
        }
        public void immunities()
        {
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<ShimmeringFlames>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<HaepiensBlizzard>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<HaepiensInferno>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Bleeding] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Dazed] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Electrified] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frozen] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Oiled] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Slimed] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.SoulDrain] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("An inanimate creation of unknown origin. Despite being composed of shade matter, it is not related to anything in the shade world."),
                ModContent.GetInstance<ShadeWorldBestiary>().ModBiomeBestiaryInfoElement,
            });
        }

        SoundStyle Stun = new SoundStyle("DestroyerTest/Assets/Audio/TenebrousConstruct/Stun", 5)
        {
            PitchVariance = 0.2f,
            MaxInstances = 0,

        };

        SoundStyle Laugh = new SoundStyle("DestroyerTest/Assets/Audio/TenebrousConstruct/Laugh")
        {
            PitchVariance = 0.5f,
            MaxInstances = 0,

        };


        SoundStyle Idle = new SoundStyle("DestroyerTest/Assets/Audio/TenebrousConstruct/Idle", 8)
        {
            PitchVariance = 0.2f,
            MaxInstances = 0,
        };

        SoundStyle Kill = new SoundStyle("DestroyerTest/Assets/Audio/TenebrousConstruct/Kill", 3)
        {
            PitchVariance = 0.2f,
            MaxInstances = 0
        };

        SoundStyle Hit = DTAssetLib.Impacts.Malevolence with { PitchVariance = 0.6f };

        public static List<string> FightDialogue;
        public readonly int NumFightDialogueLines = 21;

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 55;
            NPC.defense = 140;
            NPC.lifeMax = ModLoader.HasMod("CalamityMod") ? 800000 : 100000;
            NPC.HitSound = Hit;
            NPC.DeathSound = Kill;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            // Sets the above
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0.0f;
            NPC.boss = true;
            NPC.takenDamageMultiplier = ModLoader.HasMod("CalamityMod") ? 0.85f : 0.95f;


            FightDialogue = new List<string>();

            for (int i = 0; i < NumFightDialogueLines - 1; i++)
            {
                FightDialogue.Add(Language.GetTextValue($"Mods.DestroyerTest.NPCs.TenebrousConstruct.FightDialogue{i}"));
            }

        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            int startFrame = 0;
            int finalFrame = 54;
            int frameSpeed = 1;
            NPC.frameCounter += 0.5f;
            NPC.frameCounter += NPC.velocity.Length() / 10f;
            if (NPC.frameCounter > frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y > finalFrame * frameHeight)
                {
                    NPC.frame.Y = startFrame * frameHeight;
                }
            }
        }

        public float WingXScale = 1f;
        public float OrbitBarrierOpacity = 0f;

        float[] RingRotAmt = new float[16];
        float[] RingRot = new float[16];

        float R = 0f;

        float BGScale = 0f;

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveTiles;
        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return;
            }

            R += 0.04f;

            if (BGScale < 100f)
            {
                BGScale += 0.1f;
            }

            //Main.EntitySpriteDraw(DTAssetLib.Circle.Value, NPC.Center - Main.screenPosition, null, Color.Black, 0f, DTAssetLib.Circle.Value.Size() / 2, BGScale, SpriteEffects.None, 0f);


            for (int i = 0; i < RingRotAmt.Length; i++)
            {
                if (RingRotAmt[i] == 0f)
                {
                    RingRotAmt[i] = Main.rand.NextFloat(-0.3f, 0.3f);
                }
            }


            for (int i2 = 0; i2 < RingRot.Length; i2++)
            {
                RingRot[i2] += RingRotAmt[i2];
            }

            if (PlayerShouldBeTrapped)
            {
                if (OrbitBarrierOpacity < 1f)
                {
                    OrbitBarrierOpacity += 0.025f;
                }
            }
            else
            {
                if (OrbitBarrierOpacity > 0f)
                {
                    OrbitBarrierOpacity -= 0.025f;
                }
            }

            Vector2 screenPos = Main.screenPosition;



            var Cap = spriteBatch.Capture();
            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.End();
            spriteBatch.Begin(Cap);

            /*
            //Inner Ring
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient with { A = 0 } * 0.25f) * OrbitBarrierOpacity, RingRot[0], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(200f, 34), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient with { A = 0 } * 0.25f) * OrbitBarrierOpacity, RingRot[1], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(200f, 34), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient with { A = 0 } * 0.25f) * OrbitBarrierOpacity, RingRot[2], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(200f, 34), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient with { A = 0 } * 0.25f) * OrbitBarrierOpacity, RingRot[3], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(200f, 34), SpriteEffects.None, 0);

            //Second ring
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient with { A = 0 } * 0.25f) * OrbitBarrierOpacity, RingRot[4], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(500f, 34), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient with { A = 0 } * 0.25f) * OrbitBarrierOpacity, RingRot[5], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(500f, 34), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient with { A = 0 } * 0.25f) * OrbitBarrierOpacity, RingRot[6], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(500f, 34), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient with { A = 0 } * 0.25f) * OrbitBarrierOpacity, RingRot[7], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(500f, 34), SpriteEffects.None, 0);

            //third
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient with { A = 0 } * 0.25f) * OrbitBarrierOpacity, RingRot[8], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(800f, 34), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient with { A = 0 } * 0.25f) * OrbitBarrierOpacity, RingRot[9], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(800f, 34), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient with { A = 0 } * 0.25f) * OrbitBarrierOpacity, RingRot[10], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(800f, 34), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient with { A = 0 } * 0.25f) * OrbitBarrierOpacity, RingRot[11], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(800f, 34), SpriteEffects.None, 0);

            //outer
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient with { A = 0 } * 0.25f) * OrbitBarrierOpacity, RingRot[12], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(1100f, 34), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient with { A = 0 } * 0.25f) * OrbitBarrierOpacity, RingRot[13], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(1100f, 34), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient with { A = 0 } * 0.25f) * OrbitBarrierOpacity, RingRot[14], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(1100f, 34), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, NPC.Center - screenPos, null, (ColorLib.TenebrisGradient with { A = 0 } * 0.25f) * OrbitBarrierOpacity, RingRot[15], DTAssetLib.AuraRing.Value.Size() / 2, DTAssetLib.AuraRing.Value.ScaleRingTextureToMatchRadius(1100f, 34), SpriteEffects.None, 0);
            */

            Main.EntitySpriteDraw(DTAssetLib.BarrierRing.Value, NPC.Center - screenPos, null, Color.White with { A = 0 } * OrbitBarrierOpacity, R, DTAssetLib.BarrierRing.Value.Size() / 2, DTAssetLib.BarrierRing.Value.ScaleRingTextureToMatchRadius(1300f, 1300), SpriteEffects.None, 0);


            spriteBatch.UseBlendState(BlendState.Additive);
            Main.EntitySpriteDraw(DTAssetLib.BlessedNodeLaserTelegraph.Value, NPC.Center - Main.screenPosition, null, ColorLib.TenebrisGradient * LaserWarnOpacity, LaserRotOffset - 12f, DTAssetLib.BlessedNodeLaserTelegraph.Value.Size() / 2, 1f, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.BlessedNodeLaserTelegraph.Value, NPC.Center - Main.screenPosition, null, Color.White * LaserWarnOpacity, LaserRotOffset - 12f, DTAssetLib.BlessedNodeLaserTelegraph.Value.Size() / 2, 0.65f, SpriteEffects.None);
            spriteBatch.UseBlendState(BlendState.AlphaBlend);

            if (ShouldDrawVingette)
            {
                DrawVingette();
            }

            spriteBatch.ResetToDefault();

        }

        public float VingetteOpacity = 0f;
        public float vOpacity = 0f;

        public float VingetteScale = 0.5f;
        public float vScale = 0.5f;

        public bool ShouldDrawVingette = false;
        public void DrawVingette()
        {
            Main.EntitySpriteDraw(DTAssetLib.Vingette.Value, NPC.Center - Main.screenPosition, null, Color.Black * vOpacity, 0f, DTAssetLib.Vingette.Value.Size() / 2, vScale, SpriteEffects.None);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return true;
            }

            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                float Alpha = MathHelper.Lerp(0.2f, 0f, (float)i / (float)NPC.oldPos.Length);
                Main.EntitySpriteDraw(TextureAssets.Npc[Type].Value, (NPC.oldPos[i] + new Vector2(NPC.width / 2, (NPC.height / 2) + 2)) - screenPos, NPC.frame, drawColor * Alpha, NPC.oldRot[i], new Vector2(NPC.width / 2, (NPC.frame.Height) / 2), NPC.scale, SpriteEffects.None);
            }


            Utils.DrawBorderString(spriteBatch, InternalTimer.ToString(), (NPC.Center + new Vector2(0, -40)) - Main.screenPosition, Color.Red, 1f, 0.5f, 0.5f);

            Asset<Texture2D> WingLeft = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/TenebrousConstructWingLeft");
            Asset<Texture2D> WingRight = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/TenebrousConstructWingRight");

         

            // Left wing: origin at RIGHT edge, middle vertically
            Vector2 originLeft = new Vector2(WingLeft.Width(), WingLeft.Height() / 2);
            Main.EntitySpriteDraw(
                WingLeft.Value,
                NPC.Center - screenPos + new Vector2(-30, -30),
                null,
                Color.White with { A = 0 } * 0.5f * NPC.Opacity,
                0f,
                originLeft,
                new Vector2(WingXScale * 2.15f, 2.15f),
                SpriteEffects.None,
                0
            );

            Main.EntitySpriteDraw(
                WingLeft.Value,
                NPC.Center - screenPos + new Vector2(-30, -30),
                null,
                Color.White with { A = 0 } * NPC.Opacity,
                0f,
                originLeft,
                new Vector2(WingXScale * 2, 2f),
                SpriteEffects.None,
                0
            );

            // Right wing: origin at LEFT edge, middle vertically
            Vector2 originRight = new Vector2(0, WingRight.Height() / 2);

            Main.EntitySpriteDraw(
               WingRight.Value,
               NPC.Center - screenPos + new Vector2(30, -30),
               null,
               Color.White with { A = 0 } * 0.5f * NPC.Opacity,
               0f,
               originRight,
               new Vector2(WingXScale * 2.15f, 2.15f),
               SpriteEffects.None,
               0
           );

            Main.EntitySpriteDraw(
                WingRight.Value,
                NPC.Center - screenPos + new Vector2(30, -30),
                null,
                Color.White with { A = 0 } * NPC.Opacity,
                0f,
                originRight,
                new Vector2(WingXScale * 2f, 2f),
                SpriteEffects.None,
                0
            );


            return true;
        }

        public bool HasCalamity => ModLoader.TryGetMod("CalamityMod", out Mod CalamityMod);
        public enum State
        {
            IdleChase,
            LanceCross,
            Orbit,
            StarShoot,
            Knives,
            Suck,
            Lasers,

            Calamity_TeleportBurst
        }

        public State CurrentState;
        public int InternalTimer = 0;
        public int LanceCount = 0;
        public List<Vector2[]> Rings = new List<Vector2[]>();
        public List<Projectile[]> RingProjectiles = new List<Projectile[]>();
        public int KnifeCount = 0;
        public Vector2[] KnifePositions;

        public bool Knife_GetPlayerCenter = false;
        public Vector2 Knife_PlayerCenter;

        public int TeleportBurstTimer = 0;
        public bool TeleportBurstHasPosition = false;
        public Vector2 TeleportBurstTarget;

        bool[] DisplayedDialogue = new bool[21];
        int CurrentDialogue = 0;

        public bool PlayerShouldBeTrapped = false;

        public int Consumed = 0;
        public int ConsumptionThreshold = 100;
        float LaserWarnOpacity = 0f;
        float LaserRotOffset = 0f;
        public float ConsumptionProgress => (float)Consumed / (float)ConsumptionThreshold;

        void ControlDialogue()
        {
            float Prog = ((float)NPC.life / (float)NPC.lifeMax).Inverse();
            Prog = Utils.Clamp(Prog, 0f, 1f);
            CurrentDialogue = (int)MathHelper.Lerp(0, NumFightDialogueLines - 1, Prog);
            CurrentDialogue = Utils.Clamp(CurrentDialogue, 0, 20);

            if (!DisplayedDialogue[CurrentDialogue])
            {
                Main.NewText(FightDialogue[CurrentDialogue]);
                DisplayedDialogue[CurrentDialogue] = true;
            }
        }

        int DeathTimer = 0;
        private void HandleDeath()
        {
            DeathTimer++;
            NPC.aiStyle = 0;
            NPC.dontTakeDamage = true;

            if (DeathTimer == 60)
            {
                SoundEngine.PlaySound(Laugh);
                string key = NPC.life > NPC.life / 2 ? "Mods.DestroyerTest.NPCs.TenebrousConstruct.KilledPlayer" : "Mods.DestroyerTest.NPCs.TenebrousConstruct.KilledPlayer_SecondHalf";
                int Range = NPC.life > NPC.life / 2 ? 5 : 4;
                int Selection = Main.rand.Next(Range);
                AdvancedPopupRequest MSG = new() { Text = Language.GetTextValue(key + Selection.ToString()), Color = Color.Lime, DurationInFrames = 120, Velocity = new Vector2(0f, -3f) };
                PopupText.NewText(MSG, NPC.Center);
            }
            if (DeathTimer > 90)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(7f * DTUtils.RandomDirection(2), -15f), 0.02f);
            }

            if (DeathTimer >= 500)
            {
                NPC.active = false;
            }
        }

        public int IdleChaseTime = 300;
        public int LanceCrossTime = 2000;
        public int OrbitTime = 3500;
        public int StarShootTime = 1160;
        public int KnivesTime = 1460;
        public int SuckTime = 1800;
        public int LasersTime = 1320;
        public int CalamityTeleportBurstTime = 1200;

        public int IdleChaseEnd => IdleChaseTime;

        public int LanceCrossEnd => IdleChaseEnd + LanceCrossTime;

        public int OrbitEnd => LanceCrossEnd + OrbitTime;

        public int StarShootEnd => OrbitEnd + StarShootTime;

        public int KnivesEnd => StarShootEnd + KnivesTime;

        public int SuckEnd => KnivesEnd + (HasCalamity ? CalamityTeleportBurstTime : 0) + SuckTime;

        public int LasersEnd => SuckEnd + LasersTime;

        public override void AI()
        {
            NPC.TargetClosest(faceTarget: true);
            Player player;
            player = Main.player[NPC.target];

            InternalTimer++;

            NPC.rotation = 0.05f * NPC.velocity.Length();
            Vector2 direction = player.Center - NPC.Center;
            direction.Normalize();

            WingXScale = Opus.Sine(0f, 0.8f, 0.08f);

            ControlDialogue();

            if (!Main.projectile.Any(n => n.active && n.type == ModContent.ProjectileType<TenebrousConstructBG>()))
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TenebrousConstructBG>(), 0, 0);
            }

            if (vOpacity < VingetteOpacity)
            {
                vOpacity += 0.05f;
            }
            if (vScale < VingetteScale)
            {
                vScale += 0.05f;
            }

            if (vOpacity > VingetteOpacity)
            {
                vOpacity -= 0.05f;
            }
            if (vScale > VingetteScale)
            {
                vScale -= 0.05f;
            }

            if (Main.rand.NextBool(12))
            {
                Dust.NewDust(NPC.Center, NPC.width, NPC.height, ModContent.DustType<TenebrisDarkmatterDust>(), 0, 0, 0, default, 1.0f);
            }

            if (player.dead)
            {
                HandleDeath();
            }
            else
            {
                if (PlayerShouldBeTrapped)
                {
                    if (player.Center.Distance(NPC.Center) > 1300)
                    {
                        player.Center = NPC.Center + new Vector2(1280, 0).RotatedBy(player.DirectionFrom(NPC.Center).ToRotation());
                    }
                }



                //Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/UnfinishedBoss");
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/tc4");
                //Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/TenebrousConstruct");


                switch (CurrentState)
                {
                    case State.IdleChase:
                        {
                            {
                                PlayerShouldBeTrapped = false;

                                ShouldDrawVingette = true;
                                VingetteScale = 5f;
                                VingetteOpacity = 0f;

                                NPC.velocity = Vector2.Lerp(NPC.velocity, direction * 4f, 0.025f);

                                if (Main.rand.NextBool(32) && Main.GameUpdateCount % 60 == 0)
                                {
                                    SoundEngine.PlaySound(Idle, NPC.Center);
                                }

                                if (InternalTimer >= IdleChaseEnd)
                                {
                                    NPC.velocity = Vector2.Zero;
                                    SoundEngine.PlaySound(Stun);
                                    CurrentState = State.LanceCross;
                                }
                            }
                            break;
                        }
                    case State.LanceCross:
                        {

                            ShouldDrawVingette = false;

                            NPC.aiStyle = NPCAIStyleID.AncientVision;
                            if (InternalTimer % 300 == 0)
                            {
                                LanceCount++;
                                SoundEngine.PlaySound(SoundID.Item84);
                                Opus.RingSpreadProjectile(ModContent.ProjectileType<TenebrisLance>(), 4, player.Center, 1200, 40, 3, -24f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                            }

                            if (InternalTimer > LanceCrossEnd)
                            {
                                LanceCount = 0;
                                SoundEngine.PlaySound(Stun);
                                CurrentState = State.Orbit;
                            }
                            break;
                        }
                    case State.Orbit:
                        {
                            NPC.SmoothMoveToPoint(player.Center, 1f);
                            PlayerShouldBeTrapped = true;
                            ShouldDrawVingette = true;
                            VingetteScale = 2.75f;
                            VingetteOpacity = 1f;


                            if (Rings.Count == 0)
                            {
                                Rings.Add(Opus.GetEquidistantOrbitVectors(6, NPC.Center, 0.04f, 200));
                                RingProjectiles.Add(new Projectile[6]);

                                Rings.Add(Opus.GetEquidistantOrbitVectors(12, NPC.Center, 0.02f, 500));
                                RingProjectiles.Add(new Projectile[12]);

                                Rings.Add(Opus.GetEquidistantOrbitVectors(24, NPC.Center, 0.01f, 800));
                                RingProjectiles.Add(new Projectile[24]);

                                Rings.Add(Opus.GetEquidistantOrbitVectors(48, NPC.Center, 0.005f, 1100));
                                RingProjectiles.Add(new Projectile[48]);
                            }
                            else
                            {

                                Rings[0] = Opus.GetEquidistantOrbitVectors(6, NPC.Center, 0.04f, 200);
                                Rings[1] = Opus.GetEquidistantOrbitVectors(12, NPC.Center, 0.02f, 500);
                                Rings[2] = Opus.GetEquidistantOrbitVectors(24, NPC.Center, 0.01f, 800);
                                Rings[3] = Opus.GetEquidistantOrbitVectors(48, NPC.Center, 0.005f, 1100);
                            }


                            for (int o = 0; o < 4; o++)
                            {

                                Vector2[] ringPositions = Rings[o];
                                Projectile[] projectiles = RingProjectiles[o];

                                for (int i = 0; i < ringPositions.Length; i++)
                                {
                                    if (projectiles[i] == null || !projectiles[i].active)
                                    {
                                        projectiles[i] = Projectile.NewProjectileDirect(
                                            NPC.GetSource_FromAI(),
                                            ringPositions[i],
                                            Vector2.Zero,
                                            ModContent.ProjectileType<DarkEnergyOrb>(),
                                            40,
                                            3
                                        );
                                    }
                                    else
                                    {
                                        projectiles[i].Center = ringPositions[i];
                                        projectiles[i].timeLeft = 60;
                                    }
                                }
                            }

                            if (InternalTimer % 150 == 0)
                            {
                                Projectile Mine = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextVector2Circular(10, 10), ModContent.ProjectileType<DarkLaserMine>(), 100, 2);
                                Mine.timeLeft = 180;
                            }

                            if (InternalTimer > OrbitEnd)
                            {

                                for (int i = 0; i < RingProjectiles.Count; i++)
                                {
                                    for (int j = 0; j < RingProjectiles[i].Length; j++)
                                    {
                                        RingProjectiles[i][j].Kill();
                                    }
                                }

                                for (int i = 0; i < Rings.Count; i++)
                                {
                                    for (int j = 0; j < Rings[i].Length; j++)
                                    {
                                        Rings[i][j] = NPC.Center;
                                    }
                                }

                                RingProjectiles.Clear();
                                Rings.Clear();
                                SoundEngine.PlaySound(Stun);
                                CurrentState = State.StarShoot;
                            }

                            break;
                        }
                    case State.StarShoot:
                        {
                            Vector2[] PossibleShootPositions = Opus.GetEquidistantVectors(12, NPC.Center, 50f);
                            PlayerShouldBeTrapped = false;
                            ShouldDrawVingette = true;
                            VingetteScale = 5f;
                            VingetteOpacity = 0f;


                            if (InternalTimer % 4 == 0 && InternalTimer > 3560)
                            {
                                Vector2 ShootPosition = PossibleShootPositions[Main.rand.Next(PossibleShootPositions.Length)];

                                Vector2 PlayerPrediction = player.Center + (player.velocity * 20);
                                Dust.NewDustPerfect(PlayerPrediction, DustID.RedTorch).noGravity = true;
                                Vector2 ToPlayer = ShootPosition.DirectionTo(PlayerPrediction);
                                ToPlayer.Normalize();

                                SoundEngine.PlaySound(SoundID.Item28 with { MaxInstances = 0 }, ShootPosition);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPosition, ToPlayer * 18f, ModContent.ProjectileType<TenebrisStarHostile_NoHoming>(), 20, 5);

                                if (Main.rand.NextBool(60))
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPosition, ToPlayer * 18f, ModContent.ProjectileType<TenebrisLance>(), 20, 5);
                                }
                            }

                            if (InternalTimer > StarShootEnd)
                            {
                                SoundEngine.PlaySound(Stun);
                                CurrentState = State.Knives;
                            }
                            break;
                        }
                    case State.Knives:
                        {
                            NPC.aiStyle = -1;
                            NPC.velocity = new Vector2(0f, Opus.Sine(2f, -2f));



                            if (!Knife_GetPlayerCenter)
                            {
                                Knife_PlayerCenter = player.Center;
                                KnifePositions = Opus.GetEquidistantVectors(8, Knife_PlayerCenter, 400f);

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<KnifeArena>(), 0, 0, ai0: player.whoAmI);

                                Knife_GetPlayerCenter = true;
                            }

                            if (InternalTimer % 15 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item80, NPC.Center);

                                Vector2 P = KnifePositions[Main.rand.Next(KnifePositions.Length)];
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.Center.DirectionTo(P), ModContent.ProjectileType<BlackKnife>(), 30, 5, ai0: player.whoAmI, ai1: Main.rand.Next(KnifePositions.Length));

                                if (Main.rand.NextBool(16) && Main.masterMode)
                                {

                                    Vector2 vel = KnifePositions[Main.rand.Next(KnifePositions.Length)].DirectionTo(Knife_PlayerCenter);

                                    //Projectile.NewProjectile(NPC.GetSource_FromAI(), Knife_PlayerCenter + (vel * -400f), vel * 10f, ModContent.ProjectileType<TenebrisLance>(), 20, 5);

                                }
                            }



                            if (InternalTimer > KnivesEnd)
                            {
                                if (!HasCalamity)
                                {
                                    Knife_GetPlayerCenter = false;
                                    SoundEngine.PlaySound(Stun);
                                    CurrentState = State.Suck;
                                }
                                else
                                {
                                    Knife_GetPlayerCenter = false;
                                    SoundEngine.PlaySound(Stun);

                                    NPC.velocity = Vector2.Zero;
                                    for (int i = 0; i < 12; i++)
                                    {
                                        StarParticle Star = new();
                                        Star.Initialize(NPC.Center, Main.rand.NextVector2Circular(4, 4), ColorLib.TenebrisGradient, 2.5f);
                                        ParticleEngine.BehindProjectiles.Add(Star);
                                    }
                                    NPC.Opacity = 0f;
                                    CurrentState = State.Calamity_TeleportBurst;
                                }
                            }

                            break;
                        }
                    case State.Suck:
                        {
                            PlayerShouldBeTrapped = true;

                            ShouldDrawVingette = true;
                            VingetteScale = MathHelper.Lerp(2.75f, 0.5f, ConsumptionProgress);
                            VingetteOpacity = 1f;

                            NPC.aiStyle = -1;
                            NPC.velocity *= 0.99f;

                            player.velocity += player.Center.DirectionTo(NPC.Center) * 0.7f;

                            if (Consumed >= ConsumptionThreshold)
                            {
                                SoundEngine.PlaySound(Laugh);
                                InternalTimer = SuckEnd;
                                CurrentState = State.Lasers;
                            }
                            else
                            {
                                if (InternalTimer >= SuckEnd)
                                {
                                    SoundEngine.PlaySound(Stun);
                                    CurrentState = State.IdleChase;
                                    Consumed = 0;
                                    foreach (NPC npc in Main.npc)
                                    {
                                        if (npc.active && npc.type == ModContent.NPCType<KillableChargeSpirit>())
                                        {
                                            npc.StrikeInstantKill();
                                        }
                                    }

                                    InternalTimer = 0;
                                }
                                else
                                {
                                    if (InternalTimer % 10 == 0)
                                    {
                                        Vector2 Spawn = NPC.Center + Main.rand.NextVector2CircularEdge(1200f, 1200f);
                                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)Spawn.X, (int)Spawn.Y, ModContent.NPCType<KillableChargeSpirit>(), ai0: NPC.whoAmI);
                                    }

                                    if (InternalTimer % 120 == 0)
                                    {
                                        Opus.RingSpreadProjectile(ModContent.ProjectileType<TenebrisStarHostile_NoHoming>(), 3, NPC.Center, 1200f, 50, 4, -12f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                                    }
                                }
                            }
                            break;
                        }
                    case State.Lasers:
                        {
                            PlayerShouldBeTrapped = true;

                            ShouldDrawVingette = true;
                            VingetteScale = 2.75f;
                            VingetteOpacity = 1f;

                            if (InternalTimer < (SuckEnd + 120))
                            {
                                LaserRotOffset += 0.01f;

                                LaserWarnOpacity = MathHelper.Lerp(0f, 1f, Utilities.Convert01To010((float)(InternalTimer - SuckEnd) / 120f));

                            }
                            else
                            {
                                if (InternalTimer == SuckEnd + 120)
                                {
                                    SoundEngine.PlaySound(DTAssetLib.Impacts.MagicHit);

                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<LaserAudioController>(), 0, 0);
                                    foreach (Projectile projectile in Opus.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisLaser2>(), 6, NPC.Center, 100, 4, 0.001f, ai1: 1))
                                    {

                                    }
                                }

                                if (InternalTimer % 90 == 0)
                                {
                                    Opus.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisStarHostile_NoHoming>(), 20, NPC.Center, 100, 4, 6f);
                                }

                                if (InternalTimer >= (LasersEnd + 120))
                                {
                                    SoundEngine.PlaySound(Stun);
                                    CurrentState = State.IdleChase;
                                    Consumed = 0;
                                    InternalTimer = 0;

                                }
                            }
                            break;

                        }

                    case State.Calamity_TeleportBurst:
                        {
                            ShouldDrawVingette = false;


                            if (InternalTimer % 120 == 0)
                            {
                                SoundEngine.PlaySound(Laugh);

                                NPC.Opacity -= 0.02f;

                                TeleportBurstTarget = player.Center + new Vector2(Main.rand.Next(-300, 300), Main.rand.Next(-300, 300));
                                TeleportBurstHasPosition = true;
                            }

                            if (TeleportBurstHasPosition)
                            {
                                if (TeleportBurstTimer < 90)
                                {
                                    Vector2[] P = Opus.GetEquidistantOrbitVectors(8, TeleportBurstTarget, 0.1f, 100);

                                    for (int i = 0; i < P.Length; i++)
                                    {
                                        StarParticle Star = new();
                                        Star.Initialize(P[i], P[i].DirectionTo(TeleportBurstTarget) * 1.5f, ColorLib.TenebrisGradient, 0.5f);
                                        ParticleEngine.BehindProjectiles.Add(Star);
                                    }

                                    NPC.velocity *= 0.9f;
                                    TeleportBurstTimer++;
                                }
                                else
                                {
                                    SoundEngine.PlaySound(DTAssetLib.Impacts.DarkShatter);

                                    NPC.Opacity = 1f;
                                    NPC.Center = TeleportBurstTarget;

                                    for (int i = 0; i < 12; i++)
                                    {
                                        StarParticle Star = new();
                                        Star.Initialize(NPC.Center, Main.rand.NextVector2Circular(4, 4), ColorLib.TenebrisGradient, 2.5f);
                                        ParticleEngine.BehindProjectiles.Add(Star);
                                    }

                                    NPC.velocity = NPC.Center.DirectionTo(player.Center) * 22f;
                                    Opus.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisFlamesHostile_NoHoming>(), 8, NPC.Center, 30, 4, 10);
                                    Opus.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisStarHostile>(), 16, NPC.Center, 30, 4, 16);

                                    TeleportBurstTimer = 0;
                                    TeleportBurstHasPosition = false;
                                }
                            }

                            if (InternalTimer > (KnivesEnd + CalamityTeleportBurstTime))
                            {

                                TeleportBurstHasPosition = false;
                                TeleportBurstTimer = 0;
                                SoundEngine.PlaySound(Stun);
                                CurrentState = State.Suck;
                            }
                            break;
                        }

                }
            }

        }



        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 120, true, false);
        }

        List<int> FiftyPercentDamageProjectiles = new()
        {
            ProjectileID.LastPrismLaser,
            ProjectileID.RainbowWhip,
            ModContent.ProjectileType<WyvernTailProjectile>(),
        };

        List<int> TwentyFivePercentDamageProjectiles = new()
        {
            ProjectileID.EmpressBlade,
        };

        

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (FiftyPercentDamageProjectiles.Contains(projectile.type))
            {
                hit.Damage = (int)(hit.Damage * 0.5f);
            }
            if (TwentyFivePercentDamageProjectiles.Contains(projectile.type))
            {
                hit.Damage = (int)(hit.Damage * 0.25f);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MiniConstruct>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<RingFromBeyond>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HestiasBane>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Item_TenebrousConstructRelic>()));
        }

        public override bool ModifyDeathMessage(ref NetworkText customText, ref Color color)
        {
            customText = NetworkText.FromLiteral(Language.GetTextValue("Mods.DestroyerTest.NPCs.TenebrousConstruct.Death"));
            color = ColorLib.TenebrisGradient;
            return false;
        }

        public override void OnKill()
        {
            int Gore1 = Mod.Find<ModGore>("TenebrousConstructGore1").Type;
            int Gore2 = Mod.Find<ModGore>("TenebrousConstructGore2").Type;
            int Gore3 = Mod.Find<ModGore>("TenebrousConstructGore3").Type;
            int Gore4 = Mod.Find<ModGore>("TenebrousConstructGore4").Type;
            int Gore5 = Mod.Find<ModGore>("TenebrousConstructGore5").Type;

            var entitySource = NPC.GetSource_Death();
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(0, 10)), Gore1);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(0, 10)), Gore2);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(0, 10)), Gore3);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(0, 10)), Gore4);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(0, 10)), Gore5);

            AdvancedPopupRequest T = new AdvancedPopupRequest() with { Text = "YOU BRAT!!!", Color = Color.Red, DurationInFrames = 180, Velocity = new Vector2(0, -4) };
            PopupText.NewText(T, NPC.Center);
        }


    }

    public class TenebrousConstructBG : ModProjectile, IDrawPixelated
    {

        public override string Texture => DTUtils.NoTexture;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.timeLeft = 80000;
        }

        public override void AI()
        {
            Projectile.Center = Main.screenPosition + new Vector2(Main.screenWidth / 2, (Main.screenHeight / 2) + 200);
            if (!Active)
            {
                if (Opacity <= 0)
                {
                    Projectile.Kill();
                }
            }
        }

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.BehindTiles;

        bool IDrawPixelated.ShouldDrawPixelated => true;

        public static bool Active => Main.npc.Any(n => n.active && n.type == ModContent.NPCType<TenebrousConstruct>());
        public static NPC Subject => Main.npc.First(n => n.active && n.type == ModContent.NPCType<TenebrousConstruct>());
        float Opacity;
        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {

            Texture2D T = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/TenebrousConstructBGGlow").Value;

            if (Active)
            {
                Opacity = ((float)Subject.life / (float)Subject.lifeMax).Inverse();
            }
            else
            {
                Opacity -= 0.05f;
            }


            var Cap = spriteBatch.Capture();
            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.End();
            spriteBatch.Begin(Cap);

            spriteBatch.Draw(T, Projectile.Center - Main.screenPosition, null, ColorLib.TenebrisGradient * Opacity, 0f, T.Size() / 2, new Vector2(200f, 0.8f), SpriteEffects.None, 0f);

            spriteBatch.ResetToDefault();

            
            PointGlowPreMultiplied Particle = new();
            Particle.Initialize(Main.screenPosition + new Vector2(Main.rand.NextFloat(Main.screenWidth), Main.screenHeight), new Vector2(0f, Main.rand.NextFloat(-9f, -2f)), ColorLib.TenebrisGradient * Opacity, 1f);
            Particle.PixelLayer = PixelLayer.BehindTiles;
            Particle.color = ColorLib.TenebrisGradient * Opacity;
            ParticleEngine.BehindProjectiles.Add(Particle);
            
        }
    }
}