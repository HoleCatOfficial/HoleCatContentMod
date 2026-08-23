using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Fargos;
using DestroyerTest.Content.Projectiles.Pets;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Fargos.EternityDrops
{
    public class LittleBuggy : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 34;
            Item.value = 1;
            Item.rare = ModContent.RarityType<WretchedRarity>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<Defilement>()] = true;
            player.GetModPlayer<BuggyPlayer>().Active = true;
        }
    }

    public class BuggyPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public Projectile[] Nodes = new Projectile[3];

        public override void PostUpdateMiscEffects()
        {
            if (Active)
            {
                if (Nodes != null) //For whatever reason. Better safe than nullreference.
                {
                    Vector2[] Positions = Opus.GetEquidistantOrbitVectors(Nodes.Length, Player.MountedCenter, 0.1f, 200);

                    for (int i = 0; i < Nodes.Length; i++)
                    {
                        if (Nodes[i] != null)
                        {
                            Nodes[i].Center = Positions[i];
                        }
                        else
                        {
                            Nodes[i] = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Positions[i], Vector2.Zero, ModContent.ProjectileType<BuggyNode>(), 0, 0, Player.whoAmI);
                        }

                        if (Nodes[i].ModProjectile is not BuggyNode)
                        {
                            Nodes[i] = null;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Nodes.Length; i++)
                {
                    if (Nodes[i] != null)
                    {
                        Nodes[i].Kill();
                        Nodes[i] = null;
                    }
                }
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            for (int i = 0; i < Nodes.Length; i++)
            { 
                if (Nodes[i] != null)
                {
                    Nodes[i].Kill();
                    Nodes[i] = null;
                }
            }
        }
    }
}
