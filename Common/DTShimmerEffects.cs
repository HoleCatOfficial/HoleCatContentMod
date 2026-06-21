using Terraria;
using Terraria.ID;
using System;
using Terraria.ModLoader;
using DestroyerTest.Content.Resources.Cloths;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Rarity;
using System.Collections.Generic;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.RiftArsenal;
using Terraria.GameContent.ItemDropRules;
using System.Linq;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.Equips.ScepterAccessories;
using Terraria.DataStructures;
using DestroyerTest.Content.RiftBiomeSpread;

namespace DestroyerTest.Common
{
    public class DTShimmerEffects : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player)
        {
            return player.ZoneShimmer;
        }

        public override int Music => MusicLoader.GetMusicSlot("DestroyerTest/Assets/Music/DTShimmerMusic");

        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;

        public override float GetWeight(Player player)
        {
            return 0.9f;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {

        }
    }
}
