using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Mono.CompilerServices.SymbolWriter;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Resources
{
    public class RoseSoul : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.maxStack = 1;
            Item.value = 1;
            Item.rare = ItemRarityID.Master;
            Item.consumable = true;
            Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/RoseSoulUse") with { PitchVariance = 1.0f, Volume = 4 };
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTime = 120;
            Item.useAnimation = 120;
        }

        public override bool CanResearch()
        {
            return false;
        }


        public override bool CanUseItem(Player player)
        {
            var soulplayer = player.GetModPlayer<SoulEffectPlayer>();
            return soulplayer.RoseSoul == false || soulplayer.WyvernSoul == false;
        }

        public override bool? UseItem(Player player)
        {
            var soulplayer = player.GetModPlayer<SoulEffectPlayer>();
            soulplayer.RoseSoul = true;
            return true;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, ColorLib.Soul.ToVector3() * 0.55f * Main.essScale);
            Vector2 OuterOffset = Main.rand.NextVector2CircularEdge(160, 160);
            Vector2 Inward = Item.Center - OuterOffset;
            Rectangle SpawnArea = Item.Hitbox;
            SpawnArea.Inflate(60, 60);
        }
    }

    public class SoulSceneRose : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player)
        {
            foreach (Item sl in Main.item)
            {
                if (sl.type == ModContent.ItemType<RoseSoul>() && sl.active)
                {
                    if (player.Distance(sl.Center) < 1000)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/RoseSoulAmbience");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (isActive)
            {
                
            }
            else
            {
               
            }
        }

    }
}
