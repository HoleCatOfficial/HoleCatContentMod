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
    public class WyvernSoul : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.maxStack = 1;
            Item.value = 1;
            Item.rare = ItemRarityID.Master;
            Item.consumable = true;
            Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/CorpseRoar2") with { PitchVariance = 1.0f, Volume = 4 };
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTime = 120;
            Item.useAnimation = 120;
        }

        public override bool CanUseItem(Player player)
        {
            var soulplayer = player.GetModPlayer<SoulEffectPlayer>();
            return soulplayer.RoseSoul == false || soulplayer.WyvernSoul == false;
        }

        public override bool? UseItem(Player player)
        {
            var soulplayer = player.GetModPlayer<SoulEffectPlayer>();
            soulplayer.WyvernSoul = true;
            return true;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, ColorLib.Soul.ToVector3() * 0.55f * Main.essScale);
            Vector2 OuterOffset = Main.rand.NextVector2CircularEdge(160, 160);
            Vector2 Inward = Item.Center - OuterOffset;
            Rectangle SpawnArea = Item.Hitbox;
            SpawnArea.Inflate(60, 60);
            PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Main.rand.NextVector2FromRectangle(SpawnArea), Vector2.Zero, ColorLib.Soul2, 0.25f);
            PRTLoader.NewParticle(PRTLoader.GetParticleID<StarParticle>(), OuterOffset, Inward * 0.1f, ColorLib.Soul3, 0.25f);
        }
    }

    public class SoulSceneWyvern : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player)
        {
            foreach (Item sl in Main.item)
            {
                if (sl.type == ModContent.ItemType<WyvernSoul>() && sl.active)
                {
                    if (player.Distance(sl.Center) < 1000)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/WyvernSoulAmbience");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override void SpecialVisuals(Player player, bool isActive)
        {

        }

    }
}
