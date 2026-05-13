using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.Cards.AstirDeck
{
    public class Instinct : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 24;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<InstinctPlayer>(out var instinct))
            {
                instinct.Active = true;
            }
        }
    }

    public class  InstinctPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void DrawPlayer(Camera camera)
        {
            if (Active)
            {
                float T = (float)dodgeCooldown / 1200f;

                float Off = MathHelper.Lerp(0, 20, T.Inverse());
                Vector2 offset1 = new Vector2(Off, 0);
                Vector2 offset2 = new Vector2(-Off, 0);

                Main.PlayerRenderer.DrawPlayer(camera, Player, Player.position + offset1, 0f, Player.position, 0.6f, 1f);
                Main.PlayerRenderer.DrawPlayer(camera, Player, Player.position + offset2, 0f, Player.position, 0.6f, 1f);
            }    
        }

        int dodgeCooldown = 0;
        public override void PostUpdateEquips()
        {
            if (Active)
            {
                if (dodgeCooldown > 0)
                {
                    dodgeCooldown--;
                }

                if (dodgeCooldown == 1)
                {
                    SoundEngine.PlaySound(SoundID.Item117, Player.Center);
                }
            }
        }

        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (dodgeCooldown <= 0 && Active)
            {
                SoundEngine.PlaySound(DTAssetLib.ChargeBreak, Player.Center);
                dodgeCooldown = 1200;
            }
            return dodgeCooldown <= 0 && Active;
        }
    }
}
