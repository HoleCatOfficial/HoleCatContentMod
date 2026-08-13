using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Pets;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Fargos.EternityDrops
{
    [AutoloadEquip(EquipType.Neck)]
    public class ConstellationWeaverScarf : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 36;
            Item.value = 1000;
            Item.rare = ModContent.RarityType<StellarRarity>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ConstellationScarfPlayer>().Active = true;
        }
    }

    public class ConstellationScarfPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }


        public float StarRadius = 0;
        public Color RingColor = Color.Transparent;

        public int currentDodgeTime = 0;
        public int maxDodgeTime = 600;

        public int remainingStarTime = 0;
        public int maxStarTime = 300;

        bool ActivateStars = false;
        bool f1 = false;
        public override void PostUpdateEquips()
        {
            RingColor = OpusColorUtils.MultiLerp(StarRadius / 150f, ColorLib.StellarFireColormap);

            if (Active)
            {
                foreach(NPC n in Main.npc)
                {
                    if (n.active && n.Distance(Player.Center) < 1000 && n.lifeMax > 200)
                    {
                        currentDodgeTime++;
                    }
                }

                if (currentDodgeTime >= maxDodgeTime)
                {
                    if (!f1)
                    {
                        SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/ConstellationScarfReady"));
                        f1 = true;
                    }
                    

                    if (ActivateStars)
                    {
                        if (StarRadius < 150f)
                        {
                            StarRadius += 1.2f;
                        }

                        if (remainingStarTime > 0)
                        {
                            if (currentDodgeTime % 20 == 0)
                            {
                                Vector2 off = Player.Center + Main.rand.NextVector2Circular(StarRadius, StarRadius);
                                Projectile.NewProjectile(Projectile.GetSource_None(), off, (off).DirectionFrom(Player.Center) * 6f, ModContent.ProjectileType<ConstitutionStarFriendly>(), (int)Player.GetDamage(DamageClass.Generic).ApplyTo(15), 5, Player.whoAmI);
                            }
                            remainingStarTime--;
                        }
                        else
                        {
                            currentDodgeTime = 0;
                            ActivateStars = false;
                            f1 = false;
                        }
                    }
                    else
                    {
                        if (DestroyerTestMod.DeadlyBlossomKeybind.JustPressed)
                        {
                            SoundEngine.PlaySound(DTAssetLib.Impacts.KCrystalConsume);
                            remainingStarTime = 480;
                            ActivateStars = true;
                        }
                    }
                }
                else
                {
                    if (StarRadius > 0f)
                    {
                        StarRadius -= 1.2f;
                    }
                }
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                currentDodgeTime /= 2;
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                currentDodgeTime /= 2;
            }
        }
    }

    public class ConstellationScarfDrawLayer : IPlayerPixelatedDrawer
    {
        PixelLayer IPlayerPixelatedDrawer.PixelLayer => PixelLayer.AboveTiles;

        float R = 0f;
        void IPlayerPixelatedDrawer.DrawPixelated(Player player, SpriteBatch spriteBatch)
        {
            R += 0.18f;
            var Cap = spriteBatch.Capture();
            //Cap.TransformMatrix = PixelationSystem.PixelationMatrix;
            spriteBatch.End();
            spriteBatch.Begin(Cap);

            if (player.TryGetModPlayer<ConstellationScarfPlayer>(out var scarf) && scarf.Active)
            {
                spriteBatch.Draw(DTAssetLib.BarrierRing.Value, player.MountedCenter - Main.screenPosition, null, scarf.RingColor with { A = 0 }, R, DTAssetLib.BarrierRing.Value.Size() / 2f, DTAssetLib.BarrierRing.Value.ScaleRingTextureToMatchRadius(scarf.StarRadius, 1300), SpriteEffects.None, 0f);
            }

            spriteBatch.ResetToDefault();
        }

        bool IPlayerPixelatedDrawer.IsActive(Player player)
        {
            return player.GetModPlayer<ConstellationScarfPlayer>().Active;
        }
    }

    [Autoload(Side = ModSide.Client)]
    internal sealed class ConstellationScarfDrawLayerLoader : ModSystem
    {
        private static ConstellationScarfDrawLayer drawer;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            drawer = new ConstellationScarfDrawLayer();
            PlayerPixelRegistry.Register(drawer);
        }

        public override void Unload()
        {
            if (!Main.dedServ && drawer is not null)
                PlayerPixelRegistry.Unregister(drawer);

            drawer = null;
        }
    }
}
