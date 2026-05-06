using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.CompilerServices.SymbolWriter;
using OpusLib;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class HallowedPall : ModItem
	{


		public override void SetStaticDefaults() 
        {


		}

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 26;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<WineRarity>();
			Item.defense = 16;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ItemID.HallowedPlateMail && legs.type == ItemID.HallowedGreaves;
		}

        public override void UpdateEquip(Player player)
        {
            ScepterClassStats.Range += 25;
        }	

		public override void UpdateArmorSet(Player player) 
		{
			player.DefaultSetBonusText(player.armor[0]);
			player.GetDamage<ScepterClass>() += 0.12f;

			if (player.TryGetModPlayer<HallowedPallPlayer>(out var Pall))
			{
				Pall.Active = true;
			}
		}

		public override void AddRecipes() {
			CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 12)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}

	public class HallowedPallPlayer : ModPlayer
	{
		public bool Active = false;

		public int Cooldown = 7200;
        public int currentCooldown = 0;
		public float BarScale = 0f;
		public float TextScale = 0f;
		public float BarOpacity = 0f;
		public int TimeDisplay = 0;
		
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
			float progress = (float)currentCooldown / (float)Cooldown;
            if (Active && currentCooldown > 0)
			{
				if (drawInfo.shadow == 0)
				{
					DTUtils.DrawHallowChargeBar(BarScale, (drawInfo.drawPlayer.Center + new Vector2(0, 40)) - Main.screenPosition, progress, BarOpacity);
					Utils.DrawBorderString(Main.spriteBatch, TimeDisplay.ToString(), (drawInfo.drawPlayer.Center + new Vector2(0, 58)) - Main.screenPosition, Color.Red * BarOpacity, TextScale, 0.5f, 0.5f);
				}
			}
        }
        
        public override void PostUpdateEquips()
        {
			if (Active)
			{
				if (currentCooldown > 0)
				{
                	currentCooldown--;
					if (BarScale < 1f)
					{
						BarScale += 0.05f;
					}
					if (BarOpacity < 1f)
					{
						BarOpacity += 0.05f;
					}

					if (TextScale < 0.5f)
					{
						TextScale += 0.025f;
					}

					if (currentCooldown % 60 == 0)
					{
						TimeDisplay -= 1;
					}
				}


				if (currentCooldown == 1)
				{
					SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Charge/QuixotismCharge"), Player.position);
					

				}

				if (currentCooldown <= 0)
				{
					if (BarScale > 0f)
					{
						BarScale -= 0.05f;
					}
					if (BarOpacity > 0f)
					{
						BarOpacity -= 0.05f;
					}

					if (TextScale > 0f)
					{
						TextScale -= 0.025f;
					}
				}
			}
        }

		private void TrySurviveFatalHit(Player.HurtInfo hurtInfo)
        {
            if (!Active || currentCooldown > 0)
            return;

            if (hurtInfo.Damage > Player.statLife)
            {
                Player.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 5;
                Player.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = 16;
                BloomRingSharp ring = new();
				ring.Prepare(Player.MountedCenter, Vector2.Zero, Main.DiscoColor, 0.1f, 0.05f, BlendState.Additive);

                Vector2[] Vels = Opus.RadialVectorOutwardRandom(10, Player.MountedCenter, 3f);

                for (int i = 0; i < 10; i++)
                {
                    HallowedPallStar Star = new();
                    Star.Initialize(Player.MountedCenter, Vels[i], Color.White, 1f);
                    ParticleEngine.ShaderParticles.Add(Star);
                }

                SoundEngine.PlaySound(DTAssetLib.Impacts.BrightBell with { Volume = 1.50f }, Player.position);
                Player.statLife = Player.statLifeMax2 / 2;
                CombatText.NewText(Player.getRect(), Main.DiscoColor, "Death Evaded!", true);
                currentCooldown = Cooldown;
				TimeDisplay = (Cooldown / 60);
                hurtInfo.Damage = 0;
                Player.NinjaDodge();
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            TrySurviveFatalHit(hurtInfo);
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            TrySurviveFatalHit(hurtInfo);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            currentCooldown = 0;
			TimeDisplay = 0;
			BarScale = 0f;
			TextScale = 0f;
			BarOpacity = 0f;
        }
	}
}