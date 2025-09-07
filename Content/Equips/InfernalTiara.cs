using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Common;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DestroyerTest.Content.Equips
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
    public class InfernalTiara : ModItem
    {
        
        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            // By passing this (the ModItem) into the item parameter we can reference it later in GetEquipSlot with just the item's name
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}_Highlight", EquipType.Head, null, $"{Name}_Head_Highlight");

            /* Here is example code for supporting a female-specifig legs equip texture. See SetMatch as well.
			EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}_Female", EquipType.Legs, this, Name + "_Female");
			*/
        }

        public override void SetStaticDefaults()
        {
            // If your head equipment should draw hair while drawn, use one of the following:
            //ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
            //ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
            // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22; // Width of the item
            Item.height = 10; // Height of the item
            Item.value = Item.sellPrice(gold: 8); // How many coins the item is worth
            Item.rare = ModContent.RarityType<ScepterArmorPHMRarity>(); // The rarity of the item
            Item.defense = 8; // The amount of defense the item will give when equipped
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<InfernalDress>();
        }

        public override void UpdateArmorSet(Player player)
        {
            if (player.TryGetModPlayer<InfernalShieldPlayer>(out InfernalShieldPlayer Shield))
            {
                Shield.Active = true;
            }
            ScepterClassStats.Range += 2;
            player.lavaImmune = true;
            player.setBonus = Language.GetText("Mods.DestroyerTest.Items.InfernalTiara.SetBonus").Value;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.HellstoneBar, 8)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }

    public class InfernalShieldPlayer : ModPlayer
    {
        public const int MaxDurability = 75;
        public int Durability = 75;
        public const int Radius = 100;
        public bool Active = false;
        public bool Absorb = false;
        public bool Recharge = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            Vector2 drawPos = Player.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            drawPos.Y -= 200;

            string text = $"{Durability.ToString()} / {MaxDurability.ToString()}";

            if (Active)
            {
                Utils.DrawBorderString(spriteBatch, text, drawPos, Color.OrangeRed, 2f, 0.5f, 0.5f);
            }
        }

        public override void PostUpdateEquips()
        {
            if (Active)
            {
                if (Durability == MaxDurability && !Recharge)
                {
                    Absorb = true;
                }
            }
            // --- Absorption phase ---
                if (Active && Absorb && !Recharge)
                {
                    foreach (Projectile p in Main.projectile)
                    {
                        if (p.active && p.hostile && p.Distance(Player.Center) <= Radius)
                        {
                            if (p.TryGetGlobalProjectile<InfernalShieldGlobal>(out InfernalShieldGlobal Hostile))
                            {
                                if (!Hostile.Blocked)
                                {
                                    SoundEngine.PlaySound(SoundID.Item96, Player.Center);
                                    for (int y = 0; y < 9; y++)
                                        {
                                            PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle>(), p.Center, new Vector2(Main.rand.NextFloat(-2f, 2.1f), Main.rand.NextFloat(-4f, -6.1f)), new Color(253, 62, 3), 0.4f);
                                        }
                                    p.Kill();
                                    Hostile.Blocked = true;
                                    Durability = Math.Max(Durability - p.damage, 0);
                                    if (p.damage > Durability)
                                    {
                                        SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/TO_Break"), Player.Center);
                                    }
                                }
                            }
                        }
                    }

                    if (Main.rand.NextBool(400))
                    {
                        SoundEngine.PlaySound(SoundID.Pixie with { Pitch = -2 }, Player.Center);
                    }

                    for (int r = 0; r < 3; r++)
                    {
                    BasePRT WallPRT = PRTLoader.NewParticle(
                        PRTLoader.GetParticleID<SimpleParticle>(),
                        Player.Center + Main.rand.NextVector2CircularEdge(Radius, Radius),
                        Vector2.Zero, Color.OrangeRed, 0.4f
                    );
                    WallPRT.Velocity += Player.velocity;
                    Dust WallDust = Dust.NewDustPerfect(
                        Player.Center + Main.rand.NextVector2CircularEdge(Radius, Radius),
                        DustID.TintableDustLighted, Vector2.Zero, 0, Color.OrangeRed, 1.0f
                    );
                    WallDust.velocity += Player.velocity;
                    }

                    if (Durability <= 0)
                    {
                        
                        Absorb = false;   // shield can’t block anymore
                        Recharge = true;  // enter recharge mode
                    }
                }

            // --- Recharge phase ---
            if (Recharge)
            {
                if (Main.GameUpdateCount % 20 == 0)
                {
                    NetworkText[] DeathMSGs = new NetworkText[]
                    {
                        NetworkText.FromLiteral($"{Player.name} sacrificed themselves to the inferno."),
                        NetworkText.FromLiteral($"{Player.name} gave a little too much in return for too little."),
                        NetworkText.FromLiteral($"{Player.name} succumbed under the burden of the inferno."),
                        NetworkText.FromLiteral($"{Player.name} didnt have it in them to sustain their shield.")
                    };
                    SoundEngine.PlaySound(SoundID.Unlock with { Pitch = -2 }, Player.Center);

                    Player.HurtInfo Steal = new Player.HurtInfo()
                    {
                        Damage = 1,
                        HitDirection = 0,
                        Dodgeable = false,
                        SoundDisabled = true,
                        Knockback = 0,
                        DamageSource = PlayerDeathReason.ByCustomReason(DeathMSGs[Main.rand.Next(DeathMSGs.Length)])
                    };

                    Player.Hurt(Steal, quiet: true);
                    Durability++;
                }


                if (Durability >= MaxDurability)
                {
                    SoundEngine.PlaySound(SoundID.Research, Player.Center);
                    Recharge = false;
                    Absorb = true; // shield comes back online
                }
            }
        }

    }

    public class InfernalShieldGlobal : GlobalProjectile
    {
        public bool Blocked;
        public override bool InstancePerEntity => true;
    }

}