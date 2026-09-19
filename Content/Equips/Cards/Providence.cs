using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.player.Accessory;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;


namespace DestroyerTest.Content.Equips.Cards
{
    public class Providence : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 24;
            Item.maxStack = 1;
            Item.value = 1;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<VesperRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<ProvidencePlayer>(out var prov))
            {
                prov.Active = true;
            }
        }
    }

    public class ProvidencePlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;

        }


        public override void PostUpdateEquips()
        {
            if (Active)
            {
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<ProvidenceRadiance>()] < 1)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<ProvidenceRadiance>(), (int)Player.GetTotalDamage(DamageClass.Generic).ApplyTo(14), 2f, Player.whoAmI);
                }
            }
        }
    }
}
