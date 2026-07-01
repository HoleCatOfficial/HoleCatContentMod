
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Lorebooks;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
using DestroyerTest.Content.Projectiles.EntitiesProjectiles;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.SummonItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace DestroyerTest.Content.Entities
{
    public class SoulOrb : ModNPC
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 200;
            NPC.lifeMax = 20000;
            NPC.HitSound = SoundID.Item49;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0.5f;
            
        }

        bool b1 = false;
        public override bool CheckDead()
        {

            return false;
        }


        float rOff = 0;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Color Out = Color.Lerp(ColorLib.Soul3, ColorLib.Soul, ChargeProgress);

            Vector2[] Directions = Opus.GetEquidistantVectors(12, NPC.Center, 80);
            Texture2D Telegraph = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/DirectionalTelegraph").Value;
            Vector2 Orig = new Vector2(0, Telegraph.Width / 2);
            Vector2 Scl = new Vector2(MathHelper.Lerp(0f, 1f, ChargeProgress), 1f);

            if (Math.Abs(Scale - MainScale) < 0.1f)
            {
                for (int i = 0; i < Directions.Length; i++)
                {
                    Vector2 Outward = Directions[i] - NPC.Center;
                    Outward.Normalize();


                    Main.EntitySpriteDraw(Telegraph, Directions[i] - screenPos, null, Out with { A = 0 }, Outward.ToRotation(), Orig, Scl, SpriteEffects.None, 0f);

                }
            }



            rOff -= 0.1f;
            
            DTUtils.DrawCrystalCore(spriteBatch, NPC.Center, Color.White, Out, rOff, Scale);
            return false;
        }

        public int Charge = 0;
        public const int MaxCharge = 1000;
        public float ChargeProgress;

        bool KillHostile = false;
        bool dying = false;
        Player target => Main.player[(int)NPC.ai[0]];

        SlotId LoopSlot;
        public SoundStyle Loop = new SoundStyle("DestroyerTest/Assets/Audio/AuraLoop/LaserLoop1")
        {
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };
        public float PitchVal = -2f;

        float MainScale = 5f;
        float Scale = 0f;

        public override void OnSpawn(IEntitySource source)
        {

        }

        public override void AI()
        {
            Charge++;

            ChargeProgress = (float)Charge / (float)MaxCharge;

            

            if (dying)
            {
                if (!b1)
                {
                    if (KillHostile)
                    {
                        SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Break);
                        Opus.RadialSpreadProjectile(ModContent.ProjectileType<SoulBeam>(), 12, NPC.Center, 1200, 0f, 0.001f);
                    }

                    b1 = true;
                }

                NPC.velocity = Vector2.Zero;

                if (PitchVal > -2f)
                {
                    PitchVal -= 0.01f;
                }
                if (Scale > 0)
                {
                    Scale -= 0.05f;
                }
                else
                {
                    NPC.active = false;
                }
            }
            else
            {
                if (Scale < MainScale)
                {
                    Scale += 0.05f;
                }

                PitchVal = MathHelper.Lerp(-2f, 0f, ChargeProgress);

                NPC.SmoothMoveToPoint(target.MountedCenter, 12f);
            }

            if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound))
            {
                var tracker = new NPCAudioTracker(NPC);
                LoopSlot = SoundEngine.PlaySound(Loop, NPC.Center, soundInstance => {
                    soundInstance.Position = NPC.Center;
                    soundInstance.Pitch = PitchVal;
                    return tracker.IsActiveAndInGame();
                });

            }

            if (NPC.life < 1)
            {
                dying = true;
            }

            if (Charge >= MaxCharge)
            {

                KillHostile = true;
                dying = true;
                
            }


        }


    }
}
