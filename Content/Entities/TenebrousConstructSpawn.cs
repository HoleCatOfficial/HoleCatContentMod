using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Boss;
using DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss;
using DestroyerTest.Content.Projectiles.Boss.TenebrousConstruct;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.SHADEMANAGEMENT;
using DestroyerTest.Content.Tools;
using log4net.Repository.Hierarchy;
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
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using UtfUnknown.Core.Models.SingleByte.Finnish;
using static DestroyerTest.Core.AssetReferences.Assets.Audio.TenebrousConstruct;

namespace DestroyerTest.Content.Entities
{
    public class TCTitle : BossTitle
    {
        public override int Time { get; set; } = 180;
        public override bool Flip { get; set; } = false;

        public override Color BackColor()
        {
            return ColorLib.TenebrisGradient;
        }

        public override string MusicArtist()
        {
            return Language.GetTextValue("Mods.DestroyerTest.Music.Author1");
        }

        public override string MusicTitle()
        {
            return Language.GetTextValue("Mods.DestroyerTest.Music.TenebrousConstruct");
        
        }

        public override string Name()
        {
            return Language.GetTextValue("Mods.DestroyerTest.NPCs.TenebrousConstruct.Fables.Name");
        }

        public override Color TextAbberationColor1()
        {
            return ColorLib.TenebrisGradient;
        }

        public override Color TextAbberationColor2()
        {
            return ColorLib.TenebrisGradient;
        }

        public override Color TextColor()
        {
            return Color.White;
        }

        public override string Title()
        {
            return Language.GetTextValue("Mods.DestroyerTest.NPCs.TenebrousConstruct.Fables.Title");
        }
    }
    public class TenebrousConstructSpawn : ModNPC
    {
        public override string Texture => "DestroyerTest/Content/Entities/TenebrousConstruct";
        public static List<string> Dialogue;
        public readonly int NumDialogueLines = 6;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[Type] = 55;
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 55;
            NPC.defense = 999999;
            NPC.lifeMax = 10;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            // Sets the above
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0.0f;

            Dialogue = new List<string>();

            for (int i = 0; i < 5; i++)
            {
                Dialogue.Add(Language.GetTextValue($"Mods.DestroyerTest.NPCs.TenebrousConstruct.SpawnDialogue{i}"));
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

        public float WingXScale = 0f;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return true;
            }

            Asset<Texture2D> WingLeft = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/TenebrousConstructWingLeft");
            Asset<Texture2D> WingRight = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/TenebrousConstructWingRight");

            Vector2 originLeft = new Vector2(WingLeft.Width(), WingLeft.Height() / 2);
            Main.EntitySpriteDraw(
                WingLeft.Value,
                NPC.Center - screenPos + new Vector2(-30, -30),
                null,
                Color.White,
                0f,
                originLeft,
                new Vector2(WingXScale * 2, 2f),
                SpriteEffects.None,
                0
            );


            Vector2 originRight = new Vector2(0, WingRight.Height() / 2);
            Main.EntitySpriteDraw(
                WingRight.Value,
                NPC.Center - screenPos + new Vector2(30, -30),
                null,
                Color.White,
                0f,
                originRight,
                new Vector2(WingXScale * 2, 2f),
                SpriteEffects.None,
                0
            );
            return true;
        }


        bool[] DisplayedDialogue = new bool[21];
        int CurrentDialogue = -1;
        bool FinishedDialogue = false;
        void ControlDialogue()
        {
            try
            {
                NPC.ai[0]++;

                if (NPC.ai[0] % 120 == 0 && CurrentDialogue < 5)
                {
                    CurrentDialogue++;
                }

                if (CurrentDialogue >= 0)
                {
                    if (!DisplayedDialogue[CurrentDialogue])
                    {
                        Mod.Logger.Info(CurrentDialogue);
                        Main.NewText(Dialogue[CurrentDialogue]);
                        DisplayedDialogue[CurrentDialogue] = true;
                        if (CurrentDialogue == 4)
                        {
                            FinishedDialogue = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mod.Logger.Error(ex.ToString());
                throw;
            }
        }

        SoundStyle Roar = new SoundStyle("DestroyerTest/Assets/Audio/TenebrousConstruct/Stun", 5)
        {
            PitchVariance = 0.2f,
            MaxInstances = 0,

        };

        bool Roared = false;
        public override void AI()
        {
            ControlDialogue();

            NPC.SmoothMoveToPoint(Main.LocalPlayer.Center + new Vector2(300 * Main.LocalPlayer.direction, -300), 16f);

            if (FinishedDialogue)
            {
                if (WingXScale < 1f)
                {
                    WingXScale += 0.025f;
                }

                if (WingXScale > 0.5f && WingXScale < 0.75f)
                {
                    WingXScale += 0.01f;
                }

                if (WingXScale > 0.75f && WingXScale < 1f)
                {
                    WingXScale += 0.005f;
                }

                if( WingXScale >= 1f )
                {
                    if (!Roared)
                    {
                        SoundEngine.PlaySound(Roar);
                        FablesTitleCardSystem.RegisterFablesBossIntro(new TCTitle());
                        Roared = true;
                    }
                    else
                    {
                        Main.musicVolume = 1f;
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<TenebrousConstruct>());
                        NPC.StrikeInstantKill();
                    }
                }
            }
        }
    }
}
