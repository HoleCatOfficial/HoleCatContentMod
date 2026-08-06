using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.Utilities;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Common;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.ItemDropRules;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using Terraria.GameContent;
using OpusLib;
using Terraria.Audio;
using System;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.EntitiesProjectiles;
using DestroyerTest.Content.MeleeWeapons;

namespace DestroyerTest.Content.Entities
{
    public class SunscorchedDjinn : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            Banner = Type;
            BannerItem = Mod.Find<ModItem>("Item_SunscorchedDjinnBanner").Type;
        }
        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;
            NPC.damage = 20;
            NPC.defense = 50;
            NPC.lifeMax = 600;
            NPC.value = 1670f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.HitSound = DTAssetLib.Djinn.Hit with { PitchVariance = 0.2f };
            NPC.DeathSound = DTAssetLib.Djinn.Kill with { PitchVariance = 0.2f };
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.scale = 1.6f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),
            });
        }

        int CurrentFrame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.frameCounter % 3 == 0)
            {
                CurrentFrame++;

                if (CurrentFrame > 5)
                {
                    CurrentFrame = 0;
                }
            }

            NPC.frame.Y = CurrentFrame * frameHeight;
        }

        //Ignore this.
        bool AfterImages = false;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (AfterImages)
            {
                
            }

            Texture2D Tex = TextureAssets.Npc[Type].Value;
            Vector2 Origin = new Vector2(NPC.frame.Width / 2, Tex.Height / Main.npcFrameCount[NPC.type] / 2);
            
            SpriteEffects FX = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(Tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, Origin, 1f, FX, 0f);
            return false;
        }


        public override void AI()
        {
            Player player = Main.LocalPlayer;
            NPC.TargetClosest();
            NPC.aiStyle = NPCAIStyleID.Firefly;

            if (Math.Abs(NPC.velocity.X) > 6f)
            {
                NPC.spriteDirection = Math.Sign(NPC.velocity.X);
            }

            if (NPC.HasValidTarget)
            {
                 player = Main.player[NPC.target];
            }

            NPC.rotation = NPC.velocity.ToRotation() * 0.1f;
            Lighting.AddLight(NPC.Center, ColorLib.Rift.ToVector3() * 0.6f);
            
            if (NPC.HasValidTarget || NPC.Distance(player.Center) > 500f)
            {

                AI_HoverNear(player);
                return;
            }
            else
            {
                AI_Idle(player);
                return;
            }
            
        }

        public void AI_Idle(Player player)
        {
            NPC.aiStyle = NPCAIStyleID.Firefly;
            AI_ValidateTarget(player);
            
        }

        public void AI_ValidateTarget(Player target)
        {
            if(target.dead || !target.active || Vector2.Distance(NPC.Center, target.Center) > 2000f)
            {
                NPC.TargetClosest();
            }
        }

        int HoverTimer = 0;
        Vector2 DirectionToPlayer;
        Vector2 StoredCenter;
        Vector2 EndPosition;
        float StoredLength = 0f;

        int DashCount = 0;

        public void AI_HoverNear(Player target)
        {
            HoverTimer++;
            bool CanUpdateTargetDirection = HoverTimer % 120 == 0 || NPC.Distance(EndPosition) < 0.5f;

            //Get Direction to player when allowed
            if (CanUpdateTargetDirection)
            {
                StoredCenter = target.Center;
                EndPosition = StoredCenter + new Vector2(Main.rand.Next(100, 300), 0).RotatedBy(DirectionToPlayer.ToRotation());
            }

            DirectionToPlayer = StoredCenter - NPC.Center;

            //Overshoot the end position past the stored location
           
            Vector2 IdealVelocity = EndPosition - NPC.Center;
            IdealVelocity.Normalize();

            float distance = NPC.Center.Distance(EndPosition);

            if (CanUpdateTargetDirection)
            {
                StoredLength = distance;
                SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, NPC.Center);
                DashCount++;

                if (DashCount % 3 == 0)
                {
                    SoundEngine.PlaySound(DTAssetLib.Djinn.Laugh with { PitchVariance = 0.4f }, NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, IdealVelocity * 6f, ModContent.ProjectileType<SunscorchedDjinnBomb>(), 40, 80);
                }
                else
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, IdealVelocity * 6f, ModContent.ProjectileType<RiftSparkHostile_NoHoming>(), 40, 80);
                }
            }

            float progress = MathHelper.Clamp(distance / StoredLength, 0f, 1f);

            NPC.velocity = IdealVelocity * MathHelper.SmoothStep(0f, 12f, progress);

            if (distance <= 0.01f)
            {
                CanUpdateTargetDirection = true;
            }

            //Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.RedTorch);
            //Dust.NewDustPerfect(EndPosition, DustID.GreenTorch);
            

        }

     

        public void AI_Attack()
        {
            
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
            bool v = ModContent.GetInstance<RiftDesertUnderground>().IsBiomeActive(spawnInfo.Player);
			if (v)
			{
				return 0.08f;
			}
			return 0f;
		}

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Wraith);
                }
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Circular(6, 6), 99);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SunscorchedCinder>(), 2, 2, 10));
        }
    }
}