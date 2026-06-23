using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.BossBar;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


/// <summary>
/// This is the code from Consolaria's Arch Wyvern. I do not own any of this except for the textures I paint over it. This code will be replaced in the future, when I am capable of modding something so advanced. (Trust me. I tried many times with the example worm. It did not go well.)
/// </summary>


namespace DestroyerTest.Content.Entities
{
    public class WyvernCorpseBody1 : ModNPC
    {

        public void immunities()
        {
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
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);

            immunities();
            Main.npcFrameCount[Type] = 6;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.penetrate > 2 || projectile.penetrate < 0)
            {
                modifiers.FinalDamage *= 0.10f;
            }
            if (projectile.type == ProjectileID.LastPrism || projectile.type == ProjectileID.LastPrismLaser || projectile.type == ProjectileID.Meowmere || projectile.type == ProjectileID.PhantasmArrow)
            {
                modifiers.FinalDamage *= 0.65f;
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void SetDefaults()
        {
            int width = 32; int height = width;
            NPC.Size = new Vector2(width, height);

            NPC.aiStyle = NPCAIStyleID.Worm;

            NPC.damage = 70;
            NPC.defense = 65;
            NPC.lifeMax = 8000;

            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.HitSound = SoundID.Tink with { Pitch = -0.6f, PitchVariance = 0.4f };

            NPC.knockBackResist = 0.0f;

            NPC.netAlways = true;
            NPC.dontCountMe = true;
            NPC.hide = true;
            NPC.realLife = ModContent.NPCType<WyvernCorpseHead>();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => new bool?(false);

        public bool anyNodesAlive;
        public override void AI()
        {
            ModifyHitDustAmounts();

            anyNodesAlive = Main.npc.Any(n => n.active && n.type == ModContent.NPCType<IchorNode>());

            if (anyNodesAlive)
            {
                NPC.dontTakeDamage = true;
                NPC.immortal = true;
                NPC.life++;
            }
            else
            {
                NPC.dontTakeDamage = false;
                NPC.immortal = false;
            }

            if (!Main.npc[(int)NPC.ai[1]].active)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.active = false;
            }
            if (NPC.position.X > Main.npc[(int)NPC.ai[1]].position.X) NPC.spriteDirection = 1;
            if (NPC.position.X < Main.npc[(int)NPC.ai[1]].position.X) NPC.spriteDirection = -1;
        }

        int Frame = 0;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return;
            }

            float Progress = (float)Main.npc[NPC.realLife].life / (float)Main.npc[NPC.realLife].lifeMax;
            Frame = (int)MathHelper.Lerp(5, 0, Progress);

            NPC.frame.Y = Frame * frameHeight;
        }

        public bool flag = false;

        public Asset<Texture2D> texture;
        public Asset<Texture2D> Glowtexture;
        public void SetTex()
        {
            if (!flag)
            {
                if (DestroyerTestMod.MasochistIsActive)
                {
                    texture = NPC.GetMasoTexture("DestroyerTest/Content/Entities/MasoMode", "WyvernCorpseBody1");
                    Glowtexture = NPC.GetMasoTexture("DestroyerTest/Content/Entities/MasoMode", "WyvernCorpseBody1");
                }
                else
                {
                    texture = TextureAssets.Npc[Type];
                    Glowtexture = ModContent.Request<Texture2D>($"{Texture}_Glow");
                }
                flag = true;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return false;
            }

            SetTex();
            

            Vector2 origin = NPC.frame.Size() * 0.5f;
            Vector2 drawPos = new Vector2(NPC.position.X - Main.screenPosition.X + (NPC.width / 2) - texture.Value.Width * NPC.scale / 2f + origin.X * NPC.scale, NPC.position.Y - Main.screenPosition.Y + NPC.height - (texture.Value.Height / 6) * NPC.scale + 4f + origin.Y * NPC.scale + 56f);
            
            if (anyNodesAlive)
            {
                //Opus.DrawNPCShadowsRotating(NPC, 6, ColorLib.Ichor);
                float rotationOffset = 0.3f * (float)NPC.direction;
                WyvernCorpseHead.DrawHealingShadow(NPC, new Vector2(0f, 6), drawPos, ColorLib.Ichor, rotationOffset);
                WyvernCorpseHead.DrawHealingShadow(NPC, new Vector2(0f, 0f - 6), drawPos, ColorLib.Ichor, rotationOffset);
                WyvernCorpseHead.DrawHealingShadow(NPC, new Vector2(6, 0f), drawPos, ColorLib.Ichor, rotationOffset);
                WyvernCorpseHead.DrawHealingShadow(NPC, new Vector2(0f - 6, 0f), drawPos, ColorLib.Ichor, rotationOffset);
            }

            
            SpriteEffects effects = SpriteEffects.None;
            if (NPC.spriteDirection == 1) effects = SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture.Value, new Vector2(NPC.position.X - Main.screenPosition.X + (NPC.width / 2) - texture.Value.Width * NPC.scale / 2f + origin.X * NPC.scale, NPC.position.Y - Main.screenPosition.Y + NPC.height - (texture.Value.Height / 6) * NPC.scale + 4f + origin.Y * NPC.scale + 56f), NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
            spriteBatch.Draw(Glowtexture.Value, new Vector2(NPC.position.X - Main.screenPosition.X + (NPC.width / 2) - texture.Value.Width * NPC.scale / 2f + origin.X * NPC.scale, NPC.position.Y - Main.screenPosition.Y + NPC.height - (texture.Value.Height / 6) * NPC.scale + 4f + origin.Y * NPC.scale + 56f), NPC.frame, Color.White, NPC.rotation, origin, NPC.scale, effects, 0f);
            return false;
        }

        int NumCrimstoneDusts = 0;
        int NumSoulParticles = 0;
        int NumBoneDusts = 0;

        void ModifyHitDustAmounts()
        {
            float Progress = (float)Main.npc[NPC.realLife].life / (float)Main.npc[NPC.realLife].lifeMax;

            float FirstQuarterProgress = (float)NPC.life / (float)NPC.lifeMax / 4;
            float LastQuarterProgress = (float)NPC.life / (float)NPC.lifeMax * 0.75f;

            //Crimstone dusts stop appearing below 25% health.
            if (Progress > 0.25f)
            {
                NumCrimstoneDusts = (int)MathHelper.Lerp(3, 0, FirstQuarterProgress);
            }
            else
            {
                NumSoulParticles = (int)MathHelper.Lerp(8, 0, LastQuarterProgress);
            }

            //Bone dusts increase throughout the fight.
            NumBoneDusts = (int)MathHelper.Lerp(1, 3, Progress);

        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            float Progress = (float)Main.npc[NPC.realLife].life / (float)Main.npc[NPC.realLife].lifeMax;

            if (Progress > 0.5f)
            {
                SoundEngine.PlaySound(SoundID.Tink with { Pitch = -0.6f, PitchVariance = 0.4f }, NPC.Center);
                if (!DTOptimizationsConfig.instance.DisableExcessDusts)
                {
                    for (int i = 0; i < NumCrimstoneDusts; i++)
                    {
                        Dust.NewDust(Main.rand.NextVector2FromRectangle(NPC.Hitbox), 20, 20, DustID.Crimstone, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), 0, default, 2);
                    }
                }
            }
            if (Progress < 0.5f && Progress > 0.25f)
            {
                SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt with { Pitch = 0.6f, PitchVariance = 0.2f }, NPC.Center);
                if (!DTOptimizationsConfig.instance.DisableExcessDusts)
                {
                    for (int i = 0; i < NumBoneDusts; i++)
                    {
                        Dust.NewDust(Main.rand.NextVector2FromRectangle(NPC.Hitbox), 20, 20, DustID.Bone, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), 0, default, 2);
                    }
                }
            }
            if (Progress < 0.25f)
            {
                SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot with { Pitch = 0.6f, PitchVariance = 0.2f }, NPC.Center);

                if (!DTOptimizationsConfig.instance.DisableExcessDusts)
                {
                    for (int i = 0; i < NumSoulParticles; i++)
                    {
                        PointGlowPreMultiplied SoulParticle = new();
                        SoulParticle.Initialize(Main.rand.NextVector2FromRectangle(NPC.Hitbox), new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)), ColorLib.Soul, 1f, 120);
                        ParticleEngine.Particles.Add(SoulParticle);
                    }
                }
            }




            if (Progress <= 0.001f)
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.DreamHit, NPC.Center);

                for (int i = 0; i < 10; i++)
                {
                    PointGlowPreMultiplied SoulParticle = new();
                    SoulParticle.Initialize(Main.rand.NextVector2FromRectangle(NPC.Hitbox), new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)), ColorLib.Soul, 1f, 120);
                    ParticleEngine.Particles.Add(SoulParticle);
                }
            }
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }
    }
}