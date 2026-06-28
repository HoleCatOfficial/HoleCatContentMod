using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using OpusLib;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    class TEBit
    {
        public Vector2 Position;
        public readonly Texture2D Texture;
        public readonly Vector2 Dimensions = new(10, 14);
        public readonly NPC Master;

        public TEBit(NPC master)
        {
            Master = master;
            Texture = ModContent.Request<Texture2D>(
                "DestroyerTest/Content/Entities/TenebrisElementalBit"
            ).Value;
        }
    }

    public class TenebrisElemental : ModNPC
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
                new FlavorTextBestiaryInfoElement("Originating from the Shade World, this Crystal is home to enough energy to gain basic sentience."),
                new FlavorTextBestiaryInfoElement("In addition to freeing the moon lord from imprisonment, breaking the seal also tore open holes across space, allowing enemies from the shade world to enter yours."),
            });

            bestiaryEntry.Info.AddRange([
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson
            ]);
        }

        public int Variety = 0;

        public override void SetDefaults()
        {
            NPC.width = 42;
            NPC.height = 42;
            NPC.damage = 55;
            NPC.defense = 36;
            NPC.lifeMax = 1700;
            NPC.HitSound = SoundID.DD2_WitherBeastHurt;
            NPC.DeathSound = SoundID.DD2_WitherBeastDeath;
            NPC.noGravity = true;
            NPC.aiStyle = NPCAIStyleID.FlyingFish;
            // Sets the above
            NPC.lavaImmune = false;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0.25f;
            Variety = Main.rand.Next(3);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CorruptionModificationSystem.TenebrisSpawnRequirements(spawnInfo))
            {
                return 0.4f;
            }
            return 0f;
        }

        void DrawBits(ref Vector2 screenPos)
        {
            if (OwnedBits == null)
            {
                return;
            }

            for(int i = 0; i < OwnedBits.Count; i++)
            {
                TEBit bit = OwnedBits[i];
                Rectangle Frame = new Rectangle(0, (int)bit.Dimensions.Y * Variety, (int)bit.Dimensions.X, (int)bit.Dimensions.Y);


                Main.EntitySpriteDraw(bit.Texture, bit.Position - screenPos, Frame, Color.White, 0f, bit.Dimensions / 2, 1f, SpriteEffects.None, 0f);
            }
        }

        Asset<Texture2D> GetTexFromVariant()
        {
            switch (Variety)
            {
                case 0:
                    {
                        return ModContent.Request<Texture2D>("DestroyerTest/Content/Entities/TenebrisElementalMagenta");
                    }
                case 1:
                    {
                        return ModContent.Request<Texture2D>("DestroyerTest/Content/Entities/TenebrisElementalBlue");
                    }
                case 2:
                    {
                        return ModContent.Request<Texture2D>("DestroyerTest/Content/Entities/TenebrisElementalBeige");
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return false;
            }

            

            if (GetTexFromVariant() != null)
            {
                Texture2D Tex = GetTexFromVariant().Value;

                DrawBits(ref screenPos);

                Main.EntitySpriteDraw(Tex, NPC.Center - screenPos, null, Color.White, NPC.rotation, Tex.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
            }

            return false;
        }


        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            NPC.rotation = 0.05f * NPC.velocity.Length() * NPC.direction;

            

            Color GetColorFromVariant()
            {
                switch (Variety)
                {
                    case 0:
                        {
                            return ColorLib.TenebrisMagenta;
                        }
                    case 1:
                        {
                            return ColorLib.TenebrisBlue;
                        }
                    case 2:
                        {
                            return ColorLib.TenebrisBeige;
                        }
                    default:
                        {
                            return Color.White;
                        }
                }
            }

            Lighting.AddLight(NPC.Center, (GetColorFromVariant() * 0.3f).ToVector3());

            ManageBits();
        }

        List<TEBit> OwnedBits;

        private void ManageBits()
        {
            int Amt()
            {
                if (!DestroyerTestMod.EternityIsActive)
                {
                    if (Main.expertMode && !Main.masterMode)
                    {
                        return 6;
                    }
                    if (Main.masterMode)
                    {
                        return 7;
                    }
                }
                else
                {
                    return 10;
                }

                return 5;
            }

            if (OwnedBits == null)
            {
                OwnedBits = new List<TEBit>();
            }

            Vector2[] OrbitalPositions = Opus.GetEquidistantOrbitVectors( Amt(), NPC.Center, Math.Abs(0.05f) * NPC.direction, 50);

            for (int i = 0; i < Amt(); i++)
            {
                if (OwnedBits.Count <= i)
                    OwnedBits.Add(new TEBit(NPC));

                OwnedBits[i].Position = OrbitalPositions[i];
            }

            if (Main.GameUpdateCount % 240 == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ChargeBreak") with { PitchVariance = 1f, Volume = 3f });

                for (int i = 0; i < Amt(); i++)
                {
                    Vector2 Outward = OrbitalPositions[i] - NPC.Center;
                    Outward.Normalize();

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), OrbitalPositions[i], Outward * 5, ModContent.ProjectileType<TenebrisFlamesHostile>(), 100, 5);
                }
            }

        }

        
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadeParticle>(), 3, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShimmeringShards>(), 4, 12, 13));
        }
    }
}