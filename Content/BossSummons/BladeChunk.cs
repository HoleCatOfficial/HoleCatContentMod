
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.BossSummons
{
    [AutoloadHead]
    public class BladeChunk : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12; 
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 8;
            Item.maxStack = 99;
            Item.value = 100;
            Item.rare = ModContent.RarityType<StellarRarity>();
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            

            Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Item.Center - Main.screenPosition, null, ColorLib.StellarFireGradientLooping(), 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, Opus.Sine(1f, 1.7f), SpriteEffects.None);
            return true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Main.rand.NextBool(5))
            {
                StellarPointGlow Glow = new();
                Glow.Prepare(Main.rand.NextVector2FromRectangle(Item.Hitbox), new Vector2(0f, -1f));
                ParticleEngine.BehindProjectiles.Add(Glow);
            }

            Lighting.AddLight(Item.Center, ColorLib.StellarFireGradientLooping().ToVector3());
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<ConstitutionBoss>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = ModContent.NPCType<ConstitutionBoss>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                }
                else
                {
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                }
            }

            return true;
        }
    }
}