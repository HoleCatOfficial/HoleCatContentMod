using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Boss;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Common.NPC_Folder
{
    public class MoonlordWarnings : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public static SoundStyle LaserWarning = new SoundStyle("DestroyerTest/Assets/Audio/Moonlord/LaserCharge")
        {
            PitchVariance = 0.2f,
            MaxInstances = 0
        };

        public static SoundStyle CoreLaserWarning = new SoundStyle("DestroyerTest/Assets/Audio/Moonlord/CoreLaserCharge")
        {
            PitchVariance = 0.2f,
            MaxInstances = 0
        };

        public static SoundStyle CoreBoltWarning = new SoundStyle("DestroyerTest/Assets/Audio/Moonlord/CoreBoltCharge")
        {
            PitchVariance = 0.2f,
            MaxInstances = 0
        };

        public static SoundStyle LaserWarning2 = new SoundStyle("DestroyerTest/Assets/Audio/Moonlord/LaserCharge2")
        {
            Volume = 0.9f,
            Pitch = 0f,
            PitchVariance = 0.2f,
            MaxInstances = 0
        };

        public static SoundStyle LaserFire = new SoundStyle("DestroyerTest/Assets/Audio/Moonlord/LaserFire")
        {
            PitchVariance = 0.2f,
            MaxInstances = 0
        };

        public static SoundStyle BoltWarning = new SoundStyle("DestroyerTest/Assets/Audio/Moonlord/BoltCharge")
        {
            Volume = 0.6f,
            PitchVariance = 0.2f,
            MaxInstances = 0
        };

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MustAlwaysDraw[NPCID.MoonLordCore] = true;
            NPCID.Sets.MustAlwaysDraw[NPCID.MoonLordFreeEye] = true;
            NPCID.Sets.MustAlwaysDraw[NPCID.MoonLordHead] = true;
            NPCID.Sets.MustAlwaysDraw[NPCID.MoonLordHand] = true;
        }

        public override void SetDefaults(NPC entity)
        {
            if (entity.type == NPCID.MoonLordCore || entity.type == NPCID.MoonLordHead || entity.type == NPCID.MoonLordHand)
            {
                entity.takenDamageMultiplier = 0.92f;
            }
        }

        bool HeadFlag = false;
        bool HeadFlag2 = false;

        bool HeadBoltFlag = false;


        bool FreeEyeFlag = false;

        bool FreeEyeBoltFlag = false;

        bool HandBoltFlag = false;
        public override void AI(NPC npc)
        {
            if (DTVanillaChangesConfig.instance.BossRework == false)
            {
                return;
            }

            CoreAI(npc);
            HeadAI(npc);
            FreeEyeAI(npc);
            HandAI(npc);
        }

        int coreShieldDelay = 0;
        bool makeCoreShield = false;
        int DecideShieldCore = 0;
        void CoreAI(NPC npc)
        {
           
            if (npc.aiStyle == NPCAIStyleID.MoonLordCore)
            {
                Player target = Main.player[npc.target];
                if (npc.ai[0] == 1f)
                {
                    npc.localAI[0]++;

                    
                    if (npc.localAI[0] == 270)
                    {
                        if (npc.localAI[1] % 2 == 0)
                        {
                            SoundEngine.PlaySound(CoreBoltWarning);
                        }
                        else
                        {
                            SoundEngine.PlaySound(CoreLaserWarning);
                        }

                        npc.localAI[1]++;
                    }

                    if (npc.localAI[0] > 270 && npc.localAI[0] < 270 + 120 && npc.localAI[1] % 2 == 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            int DustType = 229;
                            float velocityMult = 0.8f;
                            if (i % 2 == 1)
                            {
                                DustType = 229;
                                velocityMult = 1.65f;
                            }

                            Vector2 vector239 = npc.Center + ((float)Main.rand.NextDouble() * ((float)Math.PI * 2f)).ToRotationVector2() * new Vector2(30f) / 2f;
                            int dustIdx = Dust.NewDust(vector239 - Vector2.One * 8f, 16, 16, DustType, npc.velocity.X / 2f, npc.velocity.Y / 2f);
                            Main.dust[dustIdx].velocity = Vector2.Normalize(npc.Center - vector239) * 3.5f * (10f - (float)i * 2f) / 10f;
                            Main.dust[dustIdx].noGravity = true;
                            Main.dust[dustIdx].scale = velocityMult;

                            HeatseekerSilohSpark spark = new();
                            Vector2 outer = npc.Center + Main.rand.NextVector2Circular(300, 200);
                            spark.PrepareSpark(outer, outer.DirectionTo(npc.Center) * 3f, outer.DirectionTo(npc.Center).ToRotation() + MathHelper.PiOver2, Color.Aquamarine, 0.5f, false, 40, SparkDrawMode.Additive, 3f);
                            ParticleEngine.Particles.Add(spark);
                        }
                    }

                    if (npc.localAI[0] >= 270 + 120)
                    {
                        if (npc.localAI[1] % 2 == 0)
                        {
                            if (npc.localAI[0] == 270 + 120)
                            {
                                SoundEngine.PlaySound(LaserFire with { Pitch = -0.5f }, npc.Center);
                                Opus.RadialSpreadProjectile(ModContent.ProjectileType<MoonlordFusionBolt>(), 24, npc.Center, 25, 3, 7);
                            }
                            if (npc.localAI[0] == 270 + 120 + 7)
                            {
                                SoundEngine.PlaySound(LaserFire with { Pitch = -0.25f }, npc.Center);
                                Opus.RadialSpreadProjectile(ModContent.ProjectileType<MoonlordFusionBolt>(), 28, npc.Center, 25, 3, 7);
                            }
                            if (npc.localAI[0] == 270 + 120 + 14)
                            {
                                SoundEngine.PlaySound(LaserFire with { Pitch = 0f }, npc.Center);
                                Opus.RadialSpreadProjectile(ModContent.ProjectileType<MoonlordFusionBolt>(), 32, npc.Center, 25, 3, 7);
                            }
                            if (npc.localAI[0] == 270 + 120 + 15)
                            {
                                if (Main.masterMode)
                                {
                                    DecideShieldCore++;

                                    makeCoreShield = true;
                                    
                                }

                                
                            }
                        }
                    }

                    if (npc.localAI[0] == 270 + 70)
                    {
                        if (npc.localAI[1] % 2 != 0)
                        {
                            Opus.RadialSpreadProjectile(ProjectileID.PhantasmalBolt, 18, npc.Center, 25, 3, 7);
                           
                        }
                    }

                    if (npc.localAI[0] >= 270 + 70 + 120 && npc.localAI[1] % 2 != 0)
                    {
                        npc.localAI[0] = 0;
                    }
                    if (npc.localAI[0] >= 270 + 120 + 15 + 120 && npc.localAI[1] % 2 == 0)
                    {
                        npc.localAI[0] = 0;
                    }

                    if (makeCoreShield)
                    {
                        coreShieldDelay++;
                        if (coreShieldDelay >= 60)
                        {
                            if (Main.netMode != 1)
                            {
                                SoundEngine.PlaySound(DTAssetLib.FrigidFenzim.Hit with { Pitch = -1f }, npc.Center);
                                BloomRingSharp ring = new();
                                ring.Prepare(npc.Center, Vector2.Zero, Color.Aquamarine, 0.1f, 0.05f, 1.7f, BlendState.Additive);
                                ParticleEngine.Particles.Add(ring);
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<MoonlordMasterShield>(), 0, 0f, Main.myPlayer, npc.whoAmI, target.whoAmI);
                            }
                            makeCoreShield = false;
                            coreShieldDelay = 0;
                        }
                        else
                        {
                            Vector2 dustPos = npc.Center + (new Vector2(150, 0).RotatedBy(npc.DirectionTo(target.Center).ToRotation()));
                            Rectangle DustRect = Utils.CenteredRectangle(dustPos, new Vector2(400, 400));
                            Vector2 spawnPos = Main.rand.NextVector2FromRectangle(DustRect);

                            for (int i = 0; i < 2; i++)
                            {
                                Dust.NewDustPerfect(spawnPos, DustID.Phantasmal, spawnPos.DirectionTo(dustPos) * 8f);
                            }
                        }
                    }
                }
            }
            
        }

        void HeadAI(NPC npc)
        {
            if (npc.aiStyle == NPCAIStyleID.MoonLordHead)
            {
                Player target = Main.player[npc.target];
                float mainTimer = 0f;
                float duration = 0f;
                if (npc.ai[0] >= 0f)
                {
                    float previous = npc.ai[0];
                    // npc.ai[1]++;
                    int attackPattern = (int)Main.npc[(int)npc.ai[3]].ai[2];
                    int set = 2;
                    int current = 0;
                    int elapsed = 0;
                    for (; current < 5; current++)
                    {
                        duration = NPC.MoonLordAttacksArray[attackPattern, set, 1, current];
                        if (!(duration + (float)elapsed <= npc.ai[1]))
                        {
                            break;
                        }
                        elapsed += (int)duration;
                    }
                    if (current == 5)
                    {
                        current = 0;
                        //npc.ai[1] = 0f;
                        duration = NPC.MoonLordAttacksArray[attackPattern, set, 1, current];
                        elapsed = 0;
                    }
                    //npc.ai[0] = NPC.MoonLordAttacksArray[attackPattern, set, 0, current];
                    mainTimer = (int)npc.ai[1] - elapsed;
                    /*
                    if (npc.ai[0] != previous)
                    {
                        npc.netUpdate = true;
                    }
                    */
                }

                //Deathray attack type
                if (npc.ai[0] == 1f)
                {
                    if (mainTimer < 180f)
                    {
                        if (mainTimer >= 100f)
                        {
                            if (!HeadFlag)
                            {
                                SoundEngine.PlaySound(LaserWarning2);
                                HeadFlag = true;
                            }
                        }
                        if (mainTimer >= 120f)
                        {
                            if (!HeadFlag2)
                            {
                                SoundEngine.PlaySound(LaserWarning);
                                HeadFlag2 = true;
                            }
                        }

                        HeatseekerSilohSpark spark = new();
                        Vector2 outer = npc.Center + Main.rand.NextVector2Circular(300, 200);
                        spark.PrepareSpark(outer, outer.DirectionTo(npc.Center) * 3f, outer.DirectionTo(npc.Center).ToRotation() + MathHelper.PiOver2, Color.Aquamarine, 0.5f, false, 40, SparkDrawMode.Additive, 3f);
                        ParticleEngine.Particles.Add(spark);
                    }

                    if (mainTimer == 180f && Main.netMode != 1)
                    {
                        SoundEngine.PlaySound(LaserFire, npc.Center);
                        BloomRingSharp ring = new();
                        ring.Prepare(npc.Center, Vector2.Zero, Color.Aquamarine, 0.1f, 0.05f, 3f, BlendState.Additive);
                        ParticleEngine.Particles.Add(ring);

                        if (Main.masterMode)
                        {
                            SoundEngine.PlaySound(SoundID.Item124, npc.Center);
                            Opus.RadialSpreadProjectile(ProjectileID.PhantasmalBolt, 16, npc.Center, 30, 4, 6f);
                        }


                    }
                }

                if (npc.ai[0] != 1f)
                {
                    HeadFlag = false;
                    HeadFlag2 = false;
                }

                if (npc.ai[0] == 3f)
                {

                    Vector2 toPrediction = target.Center + (target.velocity * 20f) - npc.Center;

                    if (mainTimer == duration - 90f)
                    {
                        if (!HeadBoltFlag)
                        {
                            SoundEngine.PlaySound(BoltWarning, npc.Center);
                            HeadBoltFlag = true;
                        }
                    }
                    if ((mainTimer == duration - 14f || mainTimer == duration - 7f || mainTimer == duration) && Main.netMode != 1)
                    {
                        HeadBoltFlag = false;
                    }
                }
                if (npc.ai[0] != 3f)
                {
                    HeadBoltFlag = false;
                }
            }
        }

        int handShieldDelay = 0;
        bool makeHandShield = false;
        int DecideShield = 0;
        void HandAI(NPC npc)
        {
            if (npc.aiStyle == NPCAIStyleID.MoonLordHand)
            {
                Player target = Main.player[npc.target];

                bool flag79 = npc.ai[2] == 0f;
                float num1218 = -flag79.ToDirectionInt();
                float mainTimer = 0f;
                float duration = 0f;
                bool flag80 = true;
                int num1221 = 0;
                if (npc.ai[0] != -2f)
                {
                    float num1222 = npc.ai[0];
                    int num1223 = (int)Main.npc[(int)npc.ai[3]].ai[2];
                    int num1224 = ((!flag79) ? 1 : 0);
                    int num1225 = 0;
                    int num1226 = 0;
                    for (; num1225 < 5; num1225++)
                    {
                        duration = NPC.MoonLordAttacksArray[num1223, num1224, 1, num1225];
                        if (!(duration + (float)num1226 <= npc.ai[1]))
                        {
                            break;
                        }
                        num1226 += (int)duration;
                    }
                    if (num1225 == 5)
                    {
                        num1225 = 0;
                        duration = NPC.MoonLordAttacksArray[num1223, num1224, 1, num1225];
                        num1226 = 0;
                    }

                    mainTimer = (int)npc.ai[1] - num1226;

                }

                if (npc.ai[0] == 3f)
                {

                    Vector2 toPrediction = target.Center + (target.velocity * 20f) - npc.Center;

                    if (mainTimer == duration - 90f)
                    {
                        if (!HandBoltFlag)
                        {
                            SoundEngine.PlaySound(BoltWarning with { Pitch = 0.4f }, npc.Center);
                            HandBoltFlag = true;
                        }
                    }
                    if ((mainTimer == duration - 14f || mainTimer == duration - 7f || mainTimer == duration) && Main.netMode != 1)
                    {
                        if (HandBoltFlag)
                        {
                            if (Main.masterMode)
                            {
                                DecideShield++;

                                if (DecideShield % 2 == 0)
                                {
                                    makeHandShield = true;
                                }
                            }

                            HandBoltFlag = false;
                        }
                        

                        
                    }
                }
                if (npc.ai[0] != 3f)
                {
                    HandBoltFlag = false;
                }

                if (makeHandShield)
                {
                    handShieldDelay++;
                    if (handShieldDelay >= 90)
                    {
                        if (Main.netMode != 1)
                        {
                            SoundEngine.PlaySound(DTAssetLib.FrigidFenzim.Hit with { Pitch = -1f }, npc.Center);
                            BloomRingSharp ring = new();
                            ring.Prepare(npc.Center, Vector2.Zero, Color.Aquamarine, 0.1f, 0.05f, 1.7f, BlendState.Additive);
                            ParticleEngine.Particles.Add(ring);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<MoonlordMasterShield>(), 0, 0f, Main.myPlayer, npc.whoAmI, target.whoAmI);
                        }
                        makeHandShield = false;
                        handShieldDelay = 0;
                    }
                    else
                    {
                        Vector2 dustPos = npc.Center + (new Vector2(150, 0).RotatedBy(npc.DirectionTo(target.Center).ToRotation()));
                        Rectangle DustRect = Utils.CenteredRectangle(dustPos, new Vector2(400, 400));
                        Vector2 spawnPos = Main.rand.NextVector2FromRectangle(DustRect);

                        for (int i = 0; i < 2; i++)
                        {
                            Dust.NewDustPerfect(spawnPos, DustID.Phantasmal, spawnPos.DirectionTo(dustPos) * 8f);
                        }
                    }
                }
            }
        }

        void FreeEyeAI(NPC npc)
        {
            if (npc.aiStyle == NPCAIStyleID.TrueEyeOfCthulhu)
            {
                Player target = Main.player[npc.target];
                float mainTimer = 0f;
                float duration = 0f;
                float previous = npc.ai[0];
                //npc.ai[1]++;
                int current = 0;
                int scale = 0;
                for (; current < 10; current++)
                {
                    duration = NPC.MoonLordAttacksArray2[1, current];
                    if (!(duration + (float)scale <= npc.ai[1]))
                    {
                        break;
                    }
                    scale += (int)duration;
                }
                if (current == 10)
                {
                    current = 0;
                    //npc.ai[1] = 0f;
                    duration = NPC.MoonLordAttacksArray2[1, current];
                    scale = 0;
                }
                //npc.ai[0] = NPC.MoonLordAttacksArray2[0, current];
                mainTimer = (int)npc.ai[1] - scale;
                /*
                if (npc.ai[0] != previous)
                {
                    npc.netUpdate = true;
                }
                */

                if (npc.ai[0] == 1f)
                {

                    Vector2 toPrediction = target.Center + target.velocity * 20f - npc.Center;

                    if (mainTimer == duration - 90f)
                    {
                        if (!FreeEyeBoltFlag)
                        {
                            SoundEngine.PlaySound(BoltWarning with { Pitch = 0.2f }, npc.Center);
                            FreeEyeBoltFlag = true;
                        }
                    }
                    if ((mainTimer == duration - 14f || mainTimer == duration - 7f || mainTimer == duration) && Main.netMode != 1)
                    {

                    }
                }


                if (npc.ai[0] != 1f)
                {
                    FreeEyeBoltFlag = false;
                }

                if (npc.ai[0] != 4f)
                {
                    return;
                }
                if (mainTimer == 0f)
                {

                }
                if (mainTimer < 180f)
                {
                    if (mainTimer >= 120f)
                    {
                        if (!FreeEyeFlag)
                        {
                            SoundEngine.PlaySound(LaserWarning with { Pitch = 0.2f }, npc.Center);
                            FreeEyeFlag = true;
                        }
                    }

                }
                if (mainTimer < duration - 15f)
                {
                    //This seems mostly verbatim what the head does, which is nice.
                    if (mainTimer == 180f && Main.netMode != 1)
                    {
                        SoundEngine.PlaySound(LaserFire with { Pitch = 0.2f }, npc.Center);
                        BloomRingSharp ring = new();
                        ring.Prepare(npc.Center, Vector2.Zero, Color.Aquamarine, 0.08f, 0.02f, 2f, BlendState.Additive);
                        ParticleEngine.Particles.Add(ring);

                        if (Main.masterMode)
                        {
                            SoundEngine.PlaySound(SoundID.Item124, npc.Center);
                            Opus.RadialSpreadProjectile(ProjectileID.PhantasmalBolt, 6, npc.Center, 30, 4, 6f);
                        }
                    }
                }
                else
                {
                    FreeEyeFlag = false;
                }
            }
        }

        float Opacity;
        int ScrollX = 0;
        int ScrollY = 0;

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.type == NPCID.MoonLordCore)
            {
                if (npc.active)
                {
                    Player target = npc.HasValidTarget ? Main.player[npc.target] : Main.LocalPlayer;
                    if (!target.dead)
                    {
                        if (Opacity < 1f)
                        {
                            Opacity += 0.01f;
                        }
                    }
                    else
                    {
                        Opacity = 0f;
                    }

                    ScrollX += 2;
                    ScrollY += 1;

                    var Cap = spriteBatch.Capture();
                    spriteBatch.End();

                    Cap.SamplerState = SamplerState.PointWrap;
                    Cap.BlendState = BlendState.Additive;

                    spriteBatch.Begin(Cap);

                    //Main.EntitySpriteDraw(DTAssetLib.TilableNoise(8).Value, Vector2.Zero, Utils.CenteredRectangle(npc.Center + new Vector2(ScrollX, ScrollY), DTAssetLib.TilableNoise(8).Value.Size() * 4f), Color.MediumSeaGreen, 0f, DTAssetLib.TilableNoise(8).Value.Size() / 2, 6f, SpriteEffects.None, 0f);
                    //  Main.EntitySpriteDraw(DTAssetLib.TilableNoise(8).Value, Vector2.Zero, Utils.CenteredRectangle(npc.Center + new Vector2(ScrollX * -1.3f, ScrollY * -0.5f), DTAssetLib.TilableNoise(8).Value.Size() * 4f), Color.SeaGreen, 0f, DTAssetLib.TilableNoise(8).Value.Size() / 2, 6f, SpriteEffects.None, 0f);

                    spriteBatch.ResetToDefault();
                    return true;
                }    
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        float HeadWarningMultiplier = 0f;
        float HeadBoltWarningMultiplier = 0f;

        float HeadOffScreenWarningMultiplier = 0f;
        float FreeEyeWarningMultiplier = 0f;

        float FreeEyeOffScreenWarningMultiplier = 0f;

        float FreeEyeBoltWarningMultiplier = 0f;
        float HandBoltWarningMultiplier = 0f;

        float BasicCoreBoltWarningMultiplier = 0f;
        float BigCoreBoltWarningMultiplier = 0f;
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (DTVanillaChangesConfig.instance.BossRework == false)
            {
                return;
            }

            HeadLaserTelegraph(npc);
            HeadBoltTelegraph(npc);
            FreeEyeLaserTelegraph(npc);
            HandBoltTelegraph(npc);
            FreeEyeBoltTelegraph(npc);
            CoreTelegraph(npc);
        }

        void HeadBoltTelegraph(NPC npc)
        {
            if (npc.aiStyle == NPCAIStyleID.MoonLordHead)
            {
                float mainTimer = 0f;
                float duration = 0f;
                if (npc.ai[0] >= 0f)
                {
                    float previous = npc.ai[0];
                    int attackPattern = (int)Main.npc[(int)npc.ai[3]].ai[2];
                    int set = 2;
                    int current = 0;
                    int elapsed = 0;
                    for (; current < 5; current++)
                    {
                        duration = NPC.MoonLordAttacksArray[attackPattern, set, 1, current];
                        if (!(duration + (float)elapsed <= npc.ai[1]))
                        {
                            break;
                        }
                        elapsed += (int)duration;
                    }
                    if (current == 5)
                    {
                        current = 0;
                        duration = NPC.MoonLordAttacksArray[attackPattern, set, 1, current];
                        elapsed = 0;
                    }
                    mainTimer = (int)npc.ai[1] - elapsed;
                }

                Player target = Main.player[npc.target];
                Vector2 toPrediction = target.Center + (target.velocity * 20f) - npc.Center;

                Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, npc.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, 0f, DTAssetLib.ShineFlare.Value.Size() / 2, Opus.Sine(0.35f, 0.7f, 0.75f) * HeadBoltWarningMultiplier, SpriteEffects.None);

                Main.EntitySpriteDraw(DTAssetLib.DirectionalTelegraph.Value, npc.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, toPrediction.ToRotation(), new Vector2(0f, DTAssetLib.DirectionalTelegraph.Value.Height / 2), new Vector2(1f, 0.35f) * HeadBoltWarningMultiplier, SpriteEffects.None);

                if (npc.ai[0] == 3f)
                {
                    if (mainTimer > duration - 90)
                    {
                        if (HeadBoltWarningMultiplier < 1f)
                        {
                            HeadBoltWarningMultiplier += 0.1f;
                        }


                    }

                }
                else
                {
                    if (HeadBoltWarningMultiplier > 0f)
                    {
                        HeadBoltWarningMultiplier -= 0.1f;

                        if (HeadBoltWarningMultiplier < 0.11f)
                        {
                            HeadBoltWarningMultiplier = 0;
                        }
                    }
                }
            }
        }

        float HeadLaserExtraRot = 0f;
        void HeadLaserTelegraph(NPC npc)
        {
            if (npc.aiStyle == NPCAIStyleID.MoonLordHead)
            {
                float mainTimer = 0f;
                float duration = 0f;
                if (npc.ai[0] >= 0f)
                {
                    float previous = npc.ai[0];
                    int attackPattern = (int)Main.npc[(int)npc.ai[3]].ai[2];
                    int set = 2;
                    int current = 0;
                    int elapsed = 0;
                    for (; current < 5; current++)
                    {
                        duration = NPC.MoonLordAttacksArray[attackPattern, set, 1, current];
                        if (!(duration + (float)elapsed <= npc.ai[1]))
                        {
                            break;
                        }
                        elapsed += (int)duration;
                    }
                    if (current == 5)
                    {
                        current = 0;
                        duration = NPC.MoonLordAttacksArray[attackPattern, set, 1, current];
                        elapsed = 0;
                    }
                    mainTimer = (int)npc.ai[1] - elapsed;
                }

                Player target = Main.player[npc.target];
                Vector2 toTarget = npc.Center.DirectionTo(target.Center);
                toTarget.Normalize();

                //default direction is to the left
                float direction = -1f;
                // but if the player is to the left of the eye, the direction will be to the right.
                if (toTarget.X < 0f)
                {
                    direction = 1f;
                }
                // the sweeping of the laser is incremented by 1/3rd of pi in the driection dictated by the player's relative position
                toTarget = toTarget.RotatedBy((0f - direction) * ((float)Math.PI * 2f) / 6f);

                HeadLaserExtraRot += 0.1f;

                Main.EntitySpriteDraw(DTAssetLib.Corona.Value, npc.Center - Main.screenPosition, null, Color.MediumAquamarine with { A = 0 } * 0.65f * HeadWarningMultiplier, HeadLaserExtraRot * 0.5f, DTAssetLib.Corona.Value.Size() / 2, 1f, SpriteEffects.None);
                Main.EntitySpriteDraw(DTAssetLib.Corona.Value, npc.Center - Main.screenPosition, null, Color.MediumAquamarine with { A = 0 } * 0.7f * HeadWarningMultiplier, HeadLaserExtraRot * 0.6f, DTAssetLib.Corona.Value.Size() / 2, 0.75f, SpriteEffects.None);
                Main.EntitySpriteDraw(DTAssetLib.Corona.Value, npc.Center - Main.screenPosition, null, Color.MediumAquamarine with { A = 0 } * 0.75f * HeadWarningMultiplier, HeadLaserExtraRot * 0.7f, DTAssetLib.Corona.Value.Size() / 2, 0.5f, SpriteEffects.None);

                Main.EntitySpriteDraw(DTAssetLib.CurseSigilRing.Value, npc.Center - Main.screenPosition, null, Color.MediumAquamarine with { A = 0 } * HeadWarningMultiplier, HeadLaserExtraRot * 1.5f, DTAssetLib.CurseSigilRing.Value.Size() / 2, 0.5f, SpriteEffects.None);
                Main.EntitySpriteDraw(DTAssetLib.CurseSigilRing.Value, npc.Center - Main.screenPosition, null, Color.MediumAquamarine with { A = 0 } * HeadWarningMultiplier, HeadLaserExtraRot * -1.5f, DTAssetLib.CurseSigilRing.Value.Size() / 2, 0.65f, SpriteEffects.None);

                Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, npc.Center - Main.screenPosition, null, Color.MediumAquamarine with { A = 0 }, HeadLaserExtraRot, DTAssetLib.ShineFlare.Value.Size() / 2, Opus.Sine(0.4f, 0.6f, 0.5f) * HeadWarningMultiplier, SpriteEffects.None);

                Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, npc.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, 0f, DTAssetLib.ShineFlare.Value.Size() / 2, Opus.Sine(0.5f, 1f, 0.75f) * HeadWarningMultiplier, SpriteEffects.None);

                Main.EntitySpriteDraw(DTAssetLib.DirectionalTelegraph.Value, npc.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, toTarget.ToRotation(), new Vector2(0f, DTAssetLib.DirectionalTelegraph.Value.Height / 2), new Vector2(2.5f, 0.35f) * HeadWarningMultiplier, SpriteEffects.None);

                Rectangle screen = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, (int)Main.screenWidth, (int)Main.screenHeight);
                if (!screen.Contains((int)npc.Center.X, (int)npc.Center.Y))
                {
                    if (mainTimer < 180f)
                    {
                        if (mainTimer >= 90f)
                        {
                            if (HeadOffScreenWarningMultiplier < 1f)
                            {
                                HeadOffScreenWarningMultiplier += 0.1f;
                            }
                        }

                    }
                    else
                    {
                        if (HeadOffScreenWarningMultiplier > 0f)
                        {
                            HeadOffScreenWarningMultiplier -= 0.1f;
                        }
                    }
                }
                else
                {
                    if (HeadOffScreenWarningMultiplier > 0f)
                    {
                        HeadOffScreenWarningMultiplier -= 0.1f;
                    }
                }

                Main.EntitySpriteDraw(DTAssetLib.DirectionalTelegraph.Value, target.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 } * HeadOffScreenWarningMultiplier, target.DirectionTo(npc.Center).ToRotation(), new Vector2(0f, DTAssetLib.DirectionalTelegraph.Value.Height / 2), new Vector2(3f, 0.5f) * HeadWarningMultiplier, SpriteEffects.None);

                if (npc.ai[0] == 1f)
                {
                    if (mainTimer < 180f)
                    {
                        if (mainTimer >= 90f)
                        {
                            if (HeadWarningMultiplier < 1f)
                            {
                                HeadWarningMultiplier += 0.04f;
                            }
                        }


                    }
                    else
                    {
                        if (HeadWarningMultiplier > 0f)
                        {
                            HeadWarningMultiplier -= 0.01f;

                            if (HeadWarningMultiplier < 0.02f)
                            {
                                HeadWarningMultiplier = 0;
                            }
                        }
                    }
                }
                else
                {
                    if (HeadWarningMultiplier > 0f)
                    {
                        HeadWarningMultiplier -= 0.01f;

                        if (HeadWarningMultiplier < 0.02f)
                        {
                            HeadWarningMultiplier = 0;
                        }
                    }
                }


            }
        }

        void HandBoltTelegraph(NPC npc)
        {
            if (npc.aiStyle == NPCAIStyleID.MoonLordHand)
            {
                Player target = Main.player[npc.target];

                bool flag79 = npc.ai[2] == 0f;
                float num1218 = -flag79.ToDirectionInt();
                float mainTimer = 0f;
                float duration = 0f;
                bool flag80 = true;
                int num1221 = 0;
                if (npc.ai[0] != -2f)
                {
                    float num1222 = npc.ai[0];
                    int num1223 = (int)Main.npc[(int)npc.ai[3]].ai[2];
                    int num1224 = ((!flag79) ? 1 : 0);
                    int num1225 = 0;
                    int num1226 = 0;
                    for (; num1225 < 5; num1225++)
                    {
                        duration = NPC.MoonLordAttacksArray[num1223, num1224, 1, num1225];
                        if (!(duration + (float)num1226 <= npc.ai[1]))
                        {
                            break;
                        }
                        num1226 += (int)duration;
                    }
                    if (num1225 == 5)
                    {
                        num1225 = 0;
                        duration = NPC.MoonLordAttacksArray[num1223, num1224, 1, num1225];
                        num1226 = 0;
                    }

                    mainTimer = (int)npc.ai[1] - num1226;

                }

                Vector2 toPrediction = target.Center + (target.velocity * 20f) - npc.Center;

                Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, npc.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, 0f, DTAssetLib.ShineFlare.Value.Size() / 2, Opus.Sine(0.5f, 1f, 0.75f) * HandBoltWarningMultiplier, SpriteEffects.None);

                Main.EntitySpriteDraw(DTAssetLib.DirectionalTelegraph.Value, npc.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, toPrediction.ToRotation(), new Vector2(0f, DTAssetLib.DirectionalTelegraph.Value.Height / 2), new Vector2(1f, 0.35f) * HandBoltWarningMultiplier, SpriteEffects.None);

                if (npc.ai[0] == 3f)
                {



                    if (mainTimer > duration - 90f)
                    {
                        if (HandBoltWarningMultiplier < 1f)
                        {
                            HandBoltWarningMultiplier += 0.1f;
                        }
                    }
                    else
                    {
                        if (HandBoltWarningMultiplier > 0f)
                        {
                            HandBoltWarningMultiplier -= 0.1f;

                            if (HandBoltWarningMultiplier < 0.11f)
                            {
                                HandBoltWarningMultiplier = 0;
                            }
                        }
                    }
                    if ((mainTimer == duration - 14f || mainTimer == duration - 7f || mainTimer == duration) && Main.netMode != 1)
                    {

                    }
                }
                if (npc.ai[0] != 3f)
                {
                    if (HandBoltWarningMultiplier > 0f)
                    {
                        HandBoltWarningMultiplier -= 0.1f;

                        if (HandBoltWarningMultiplier < 0.11f)
                        {
                            HandBoltWarningMultiplier = 0;
                        }
                    }
                }
            }
        }

        void FreeEyeBoltTelegraph(NPC npc)
        {
            if (npc.aiStyle == NPCAIStyleID.TrueEyeOfCthulhu)
            {
                Player target = Main.player[npc.target];
                float mainTimer = 0f;
                float duration = 0f;
                float previous = npc.ai[0];
                //npc.ai[1]++;
                int current = 0;
                int scale = 0;
                for (; current < 10; current++)
                {
                    duration = NPC.MoonLordAttacksArray2[1, current];
                    if (!(duration + (float)scale <= npc.ai[1]))
                    {
                        break;
                    }
                    scale += (int)duration;
                }
                if (current == 10)
                {
                    current = 0;
                    //npc.ai[1] = 0f;
                    duration = NPC.MoonLordAttacksArray2[1, current];
                    scale = 0;
                }
                //npc.ai[0] = NPC.MoonLordAttacksArray2[0, current];
                mainTimer = (int)npc.ai[1] - scale;
                /*
                if (npc.ai[0] != previous)
                {
                    npc.netUpdate = true;
                }
                */
                Vector2 toPrediction = target.Center + (target.velocity * 20f) - npc.Center;

                Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, npc.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, 0f, DTAssetLib.ShineFlare.Value.Size() / 2, Opus.Sine(0.35f, 0.7f, 0.75f) * FreeEyeBoltWarningMultiplier, SpriteEffects.None);

                Main.EntitySpriteDraw(DTAssetLib.DirectionalTelegraph.Value, npc.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, toPrediction.ToRotation(), new Vector2(0f, DTAssetLib.DirectionalTelegraph.Value.Height / 2), new Vector2(1f, 0.35f) * FreeEyeBoltWarningMultiplier, SpriteEffects.None);

                if (npc.ai[0] == 1f)
                {
                    if (mainTimer > duration - 90f)
                    {
                        if (FreeEyeBoltWarningMultiplier < 1f)
                        {
                            FreeEyeBoltWarningMultiplier += 0.1f;
                        }
                    }
                    if ((mainTimer == duration - 14f || mainTimer == duration - 7f || mainTimer == duration) && Main.netMode != 1)
                    {

                    }
                }
                else
                {
                    if (FreeEyeBoltWarningMultiplier > 0f)
                    {
                        FreeEyeBoltWarningMultiplier -= 0.1f;
                        if (FreeEyeBoltWarningMultiplier < 0.11f)
                        {
                            FreeEyeBoltWarningMultiplier = 0;
                        }
                    }
                }
            }
        }

            float FreeEyeLaserExtraRot = 0f;
        void FreeEyeLaserTelegraph(NPC npc)
        {
            if (npc.aiStyle == NPCAIStyleID.TrueEyeOfCthulhu)
            {
                float mainTimer = 0f;
                float duration = 0f;
                float previous = npc.ai[0];
                int current = 0;
                int scale = 0;
                for (; current < 10; current++)
                {
                    duration = NPC.MoonLordAttacksArray2[1, current];
                    if (!(duration + (float)scale <= npc.ai[1]))
                    {
                        break;
                    }
                    scale += (int)duration;
                }
                if (current == 10)
                {
                    current = 0;
                    duration = NPC.MoonLordAttacksArray2[1, current];
                    scale = 0;
                }
                mainTimer = (int)npc.ai[1] - scale;

                if (npc.ai[0] != 4f)
                {
                    return;
                }
                if (mainTimer == 0f)
                {

                }


                Player target = Main.player[npc.target];
                Vector2 toTarget = npc.Center.DirectionTo(target.Center);
                toTarget.Normalize();

                //default direction is to the left
                float direction = -1f;
                // but if the player is to the left of the eye, the direction will be to the right.
                if (toTarget.X < 0f)
                {
                    direction = 1f;
                }
                // the sweeping of the laser is incremented by 1/3rd of pi in the driection dictated by the player's relative position
                toTarget = toTarget.RotatedBy((0f - direction) * ((float)Math.PI * 2f) / 6f);

                FreeEyeLaserExtraRot += 0.1f;
                Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, npc.Center - Main.screenPosition, null, Color.MediumAquamarine with { A = 0 }, FreeEyeLaserExtraRot, DTAssetLib.ShineFlare.Value.Size() / 2, Opus.Sine(0.2f, 0.5f, 0.5f) * HeadWarningMultiplier, SpriteEffects.None);

                Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, npc.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, 0f, DTAssetLib.ShineFlare.Value.Size() / 2, Opus.Sine(0.35f, 0.7f, 0.75f) * FreeEyeWarningMultiplier, SpriteEffects.None);

                Main.EntitySpriteDraw(DTAssetLib.DirectionalTelegraph.Value, npc.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, toTarget.ToRotation(), new Vector2(0f, DTAssetLib.DirectionalTelegraph.Value.Height / 2), new Vector2(2f, 0.25f) * FreeEyeWarningMultiplier, SpriteEffects.None);

                Rectangle screen = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, (int)Main.screenWidth, (int)Main.screenHeight);
                if (!screen.Contains((int)npc.Center.X, (int)npc.Center.Y))
                {
                    if (mainTimer < 180f)
                    {
                        if (mainTimer >= 90f)
                        {
                            if (FreeEyeOffScreenWarningMultiplier < 1f)
                            {
                                FreeEyeOffScreenWarningMultiplier += 0.1f;
                            }
                        }

                    }

                    else
                    {
                        if (FreeEyeOffScreenWarningMultiplier > 0f)
                        {
                            FreeEyeOffScreenWarningMultiplier -= 0.1f;

                        }
                    }
                }
                else
                {
                    if (FreeEyeOffScreenWarningMultiplier > 0f)
                    {
                        FreeEyeOffScreenWarningMultiplier -= 0.1f;
                    }
                }

                Main.EntitySpriteDraw(DTAssetLib.DirectionalTelegraph.Value, target.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 } * FreeEyeOffScreenWarningMultiplier, target.DirectionTo(npc.Center).ToRotation(), new Vector2(0f, DTAssetLib.DirectionalTelegraph.Value.Height / 2), new Vector2(2f, 0.4f) * FreeEyeWarningMultiplier, SpriteEffects.None);

                if (mainTimer < 180f)
                {
                    if (mainTimer >= 90f)
                    {
                        if (FreeEyeWarningMultiplier < 1f)
                        {
                            FreeEyeWarningMultiplier += 0.01f;
                        }
                    }

                }
                else
                {
                    if (FreeEyeWarningMultiplier > 0f)
                    {
                        FreeEyeWarningMultiplier -= 0.01f;
                        if (FreeEyeWarningMultiplier < 0.02f)
                        {
                            FreeEyeWarningMultiplier = 0;
                        }
                    }

                }

                if (mainTimer < duration - 15f)
                {

                }
            }
        }


        float CoreBoltsExtraRot = 0f;
        void CoreTelegraph(NPC npc)
        {
            if (npc.aiStyle == NPCAIStyleID.MoonLordCore)
            {
                if (npc.ai[0] == 1f)
                {

                    bool basic = npc.localAI[1] % 2 != 0;

                    Vector2[] pos1 = Opus.GetEquidistantVectors(18, npc.Center, 4);
                    Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, npc.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, 0f, DTAssetLib.ShineFlare.Value.Size() / 2, Opus.Sine(0.5f, 1f, 0.75f) * BasicCoreBoltWarningMultiplier, SpriteEffects.None);

                    for (int i = 0; i < pos1.Length; i++)
                    {
                        Main.EntitySpriteDraw(DTAssetLib.DirectionalTelegraph.Value, npc.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, npc.Center.DirectionTo(pos1[i]).ToRotation(), new Vector2(0f, DTAssetLib.DirectionalTelegraph.Height() / 2), new Vector2(3f, 1f) * BasicCoreBoltWarningMultiplier, SpriteEffects.None);
                    }


                    if (npc.localAI[0] > 270 && npc.localAI[0] < 270 + 70 && basic)
                    {
                        
                        if (BasicCoreBoltWarningMultiplier < 1f)
                        {
                            BasicCoreBoltWarningMultiplier += 0.01f;
                        }

                        
                    }
                    else
                    {
                        if (BasicCoreBoltWarningMultiplier > 0f)
                        {
                            BasicCoreBoltWarningMultiplier -= 0.1f;

                            if (BasicCoreBoltWarningMultiplier < 0.11f)
                            {
                                BasicCoreBoltWarningMultiplier = 0;
                            }
                        }
                    }

                    if (npc.localAI[0] >= 270 + 70 && basic)
                    {
                        
                    }

                    CoreBoltsExtraRot += 0.1f;
                    Main.EntitySpriteDraw(DTAssetLib.Corona.Value, npc.Center - Main.screenPosition, null, ColorLib.CelestialGradient with { A = 0 } * 0.65f * BigCoreBoltWarningMultiplier, CoreBoltsExtraRot * 0.5f, DTAssetLib.Corona.Value.Size() / 2, 1f, SpriteEffects.None);
                    Main.EntitySpriteDraw(DTAssetLib.Corona.Value, npc.Center - Main.screenPosition, null, ColorLib.CelestialGradient with { A = 0 } * 0.7f * BigCoreBoltWarningMultiplier, CoreBoltsExtraRot * 0.6f, DTAssetLib.Corona.Value.Size() / 2, 0.75f, SpriteEffects.None);
                    Main.EntitySpriteDraw(DTAssetLib.Corona.Value, npc.Center - Main.screenPosition, null, ColorLib.CelestialGradient  with { A = 0 } * 0.75f * BigCoreBoltWarningMultiplier, CoreBoltsExtraRot * 0.7f, DTAssetLib.Corona.Value.Size() / 2, 0.5f, SpriteEffects.None);

                    Main.EntitySpriteDraw(DTAssetLib.CurseSigilRing.Value, npc.Center - Main.screenPosition, null, ColorLib.CelestialGradient with { A = 0 } * BigCoreBoltWarningMultiplier, CoreBoltsExtraRot * 1.5f, DTAssetLib.CurseSigilRing.Value.Size() / 2, 0.5f, SpriteEffects.None);
                    Main.EntitySpriteDraw(DTAssetLib.CurseSigilRing.Value, npc.Center - Main.screenPosition, null, ColorLib.CelestialGradient with { A = 0 } * BigCoreBoltWarningMultiplier, CoreBoltsExtraRot * -1.5f, DTAssetLib.CurseSigilRing.Value.Size() / 2, 0.65f, SpriteEffects.None);

                    Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, npc.Center - Main.screenPosition, null, Color.MediumAquamarine with { A = 0 }, CoreBoltsExtraRot, DTAssetLib.ShineFlare.Value.Size() / 2, Opus.Sine(0.8f, 1.6f, 0.5f) * BigCoreBoltWarningMultiplier, SpriteEffects.None);

                    if (npc.localAI[0] > 270 && npc.localAI[0] < 270 + 120 && npc.localAI[1] % 2 == 0)
                    {
                        if (BigCoreBoltWarningMultiplier < 1f)
                        {
                            BigCoreBoltWarningMultiplier += 0.05f;
                        }

                        

                        
                    }
                    else
                    {
                        if (BigCoreBoltWarningMultiplier > 0f)
                        {
                            BigCoreBoltWarningMultiplier -= 0.1f;

                            if (BigCoreBoltWarningMultiplier < 0.11f)
                            {
                                BigCoreBoltWarningMultiplier = 0;
                            }
                        }
                    }

                    if (npc.localAI[0] >= 270 + 180 && !basic)
                    {
                       
                    }

                }
            }
        }
        

    }
}

