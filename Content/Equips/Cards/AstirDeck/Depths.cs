using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.Cards.AstirDeck
{
    public class Depths : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 24;
            Item.maxStack = 1;
            Item.value = 1;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            if (player.TryGetModPlayer<DepthsPlayer>(out var deep))
            {
                deep.Active = true;
            }
        }
    }

    public class DepthsPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;

        }

        public int Heat = 0;
        public int MaxHeat = 400;

        Color HeatTextColor()
        {
            float Progress = (float)Heat / (float)MaxHeat;

            return Color.Lerp(Color.IndianRed, Color.Orange, Progress);
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Active && drawInfo.shadow == 0)
            {
                Utils.DrawBorderString(Main.spriteBatch, Heat.ToString(), (drawInfo.drawPlayer.Center - Main.screenPosition) + new Vector2(0, -40), HeatTextColor(), 0.8f, 0.5f, 0.5f);
            }
        }

        bool CanPlaySound = false;
        public override void PostUpdateEquips()
        {
            if (Active)
            {
                int ty = ModContent.ProjectileType<DepthsAudioProjectile>();
                if (Player.ownedProjectileCounts[ty] < 1)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ty, 0, 0, Player.whoAmI);
                }

                if (Heat >= MaxHeat)
                {
                    SoundStyle Ping = new SoundStyle("DestroyerTest/Assets/Audio/DepthsPing") { Volume = 0.6f };
                    if (CanPlaySound)
                    {
                        SoundEngine.PlaySound(Ping);
                        CanPlaySound = false;
                    }
                }


                bool Below = Heat < MaxHeat;

                if (Below)
                {
                    CanPlaySound = true;
                }
                if (Player.adjLava)
                {
                    if (Player.miscCounter % 5 == 0 && Below)
                    {
                        Heat++;
                    }
                }
                else if (Player.lavaWet)
                {
                    if (Player.miscCounter % 2 == 0 && Below)
                    {
                        Heat++;
                    }
                }
                else if (Player.wet || Player.ZoneSnow)
                {
                    if (Player.miscCounter % 5 == 0 && Heat > 0)
                    {
                        Heat--;
                    }
                }
                else
                {
                    if (Player.miscCounter % 60 == 0 && Heat > 0)
                    {
                        Heat--;
                    }
                }

                curDMGBonus = MathHelper.Lerp(0f, MaxDMGBonus, (float)Heat / (float)MaxHeat);
                DefenseLoss = (int)MathHelper.Lerp(0, MaxDefenseLoss, (float)Heat / (float)MaxHeat);

                Player.statDefense -= DefenseLoss;
            }

            
        }

        int DefenseLoss = 0;
        int MaxDefenseLoss = 15;

        float MaxDMGBonus = 0.6f;
        float curDMGBonus = 0f;
        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (Active)
            {
                damage += curDMGBonus;
            }
        }
    }

    public class DepthsAudioProjectile : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.timeLeft = 60;
        }

        public override bool? CanHitNPC(NPC target) => false;
        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
        public override bool CanHitPlayer(Player target) => false;
        public override bool CanHitPvp(Player target) => false;


        public SlotId LoopSlot;
        public SoundStyle Loop = new SoundStyle("DestroyerTest/Assets/Audio/AuraLoop/SpiritAura", 4)
        {
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };

        public float PitchVal = -1f;
        public float Vol = 0f;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.TryGetModPlayer<DepthsPlayer>(out var depths))
            {
                if (depths.Active)
                {
                    Projectile.timeLeft = 60;
                    Projectile.Center = player.Center;

                    float Progress = (float)depths.Heat / (float)depths.MaxHeat;

                    PitchVal = MathHelper.Lerp(-1f, 0f, Progress);
                    Vol = MathHelper.Lerp(0f, 0.5f, Progress);

                    if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound))
                    {
                        var tracker = new ProjectileAudioTracker(Projectile);
                        LoopSlot = SoundEngine.PlaySound(Loop, Projectile.Center, soundInstance =>
                        {
                            soundInstance.Position = Projectile.Center;
                            soundInstance.Pitch = PitchVal;
                            soundInstance.Volume = Vol;
                            return tracker.IsActiveAndInGame();
                        });
                    }
                    else
                    {
                        activeSound.Position = Projectile.Center;
                        activeSound.Pitch = PitchVal;
                        activeSound.Volume = Vol;
                    }
                }
            }
        }
    }

    public class DepthsGlobal : GlobalTile
    {
        List<int> HotTiles = new()
        {
            TileID.Hellstone,
            TileID.HellstoneBrick,
            TileID.AncientHellstoneBrick
        };
        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            if (HotTiles.Contains(type))
            {
                closer = false;

                if (Main.LocalPlayer.TryGetModPlayer<DepthsPlayer>(out var Deep))
                {
                    if (Main.LocalPlayer.miscCounter % 60 == 0)
                    {
                        Deep.Heat += 1;
                    }
                }
            }
        }
    }
}
