
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Common;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Tools;

using Terraria.Localization;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Tiles.RiftConfigurator;

namespace DestroyerTest.Content.RiftArsenal
{
	// This file contains all the code necessary for a minion
	// - ModItem - the weapon which you use to summon the minion with
	// - ModBuff - the icon you can click on to despawn the minion
	// - ModProjectile - the minion itself

	// It is not recommended to put all these classes in the same file. For demonstrations sake they are all compacted together so you get a better overview.
	// To get a better understanding of how everything works together, and how to code minion AI, read the guide: https://github.com/tModLoader/tModLoader/wiki/Basic-Minion-Guide
	// This is NOT an in-depth guide to advanced minion AI
	public class RiftSwordBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
			Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
		}

		public override void Update(Player player, ref int buffIndex) {
			// If the minions exist reset the buff time, otherwise remove the buff from the player
			if (player.ownedProjectileCounts[ModContent.ProjectileType<RiftSwordMinion>()] > 0) {
				player.buffTime[buffIndex] = 18000;
			}
			else {
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}

	[AutoloadEquip(EquipType.Waist)]
	public class RiftScabbard : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

			ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f; // The default value is 1, but other values are supported. See the docs for more guidance. 
		}

		public override void SetDefaults() {
			Item.damage = 55;
			Item.knockBack = 0f;
			Item.mana = 40; // mana cost
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.RaiseLamp; // how the player's arm moves when using the item
			Item.value = 18000;
			Item.rare = ModContent.RarityType<RiftRarity1>(); // The rarity of the item
			Item.UseSound = new SoundStyle($"DestroyerTest/Assets/Audio/Rift_Katana_Hold") with {
				Volume = 1.0f, 
    			Pitch = 0.0f, 
    			PitchVariance = 0.5f, 
			};
			Item.accessory = true;

			// These below are needed for a minion weapon
			Item.noMelee = true; // this item doesn't do any melee damage
			Item.DamageType = DamageClass.Summon; // Makes the damage register as summon. If your item does not have any damage type, it becomes true damage (which means that damage scalars will not affect it). Be sure to have a damage type
			Item.buffType = ModContent.BuffType<RiftSwordBuff>();
			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ModContent.ProjectileType<RiftSwordMinion>(); // This item creates the minion projectile
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			// Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position
			position = Main.MouseWorld;
		}

       
       // Define minionTypes as a class field so both methods can access it

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// Apply the buff to the player to keep the minion alive
			player.AddBuff(Item.buffType, 2);

			// Spawn the minion projectile
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<RiftSwordMinion>(), damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;

			// Prevent the game from spawning another projectile automatically
			return false;
		}


		public void UpdateEquip(Player player, EntitySource_ItemUse_WithAmmo source, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			player.AddBuff(ModContent.BuffType<RiftSwordBuff>(), 60);

			if (player.slotsMinions < player.maxMinions)
			{
				Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, velocity, ModContent.ProjectileType<RiftSwordMinion>(), damage, knockback, Main.myPlayer);
			}
		}

		
		
		public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RiftData>(16)
                .AddIngredient<ShadowCircuitry>(18)
                .AddIngredient<Item_Riftplate>(16)
                .AddTile<Tile_RiftConfiguratorWeaponry>()
			.Register();
        }
	}

	

	// This minion shows a few mandatory things that make it behave properly.
	// Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
	// If the player targets a certain NPC with right-click, it will fly through tiles to it
	// If it isn't attacking, it will float near the player with minimal movement
	public class RiftSwordMinion : SwordMinionTemplate
	{
		
        public override string Texture => "DestroyerTest/Content/RiftArsenal/RiftBroadsword";

		public bool PlayedSound = false;

		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = ColorLib.Rift;
			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

			Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;
			
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			for (int i = 0; i < TrailPositions.Count - 1; i++)
			{
				Vector2 start = TrailPositions[i] - Main.screenPosition;
				Vector2 end = TrailPositions[i + 1] - Main.screenPosition;
				Vector2 diff = end - start;

				float length = diff.Length();
				if (length < 0.5f)
					continue; // skip tiny wiggle segments

				float rotation = diff.ToRotation();

				Vector2 DimensionMeasurement = Projectile.Hitbox.BottomLeft() - Projectile.Hitbox.TopRight();

				float Width = DimensionMeasurement.Length();

				float width = MathHelper.Lerp(0.01f, 0.0007f, i / (float)TrailLength);
				float alpha = MathHelper.Lerp(1f, 0f, i / (float)TrailLength);
				Color color = ColorLib.Rift * alpha;

				// Instead of stepping pixel by pixel, just draw one scaled pixel segment:
				Main.spriteBatch.Draw(
					pixel,
					start,
					null,
					color,
					rotation,
					new Vector2(pixel.Width / 2, pixel.Height / 2), // Origin is at the left-middle of the scaled pixel
					new Vector2(length, width),
					SpriteEffects.None,
					0f
				);
			}

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


				// Draw the base projectile using the default drawing system (Deferred)
				Main.EntitySpriteDraw(
					projectileTexture,
					Projectile.Center - Main.screenPosition,
					null,
					lightColor,
					Projectile.rotation,
					projectileTexture.Size() / 2,
					Projectile.scale,
					SpriteEffects.None,
					0
				);

				// Glow effect (Immediate drawing with Additive blending)
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

				Texture2D glowTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/RiftSwordGlowmaskColor").Value;
				Main.EntitySpriteDraw(
					glowTexture,
					Projectile.Center - Main.screenPosition,
					null,
					lightColor,
					Projectile.rotation,
					glowTexture.Size() / 2,
					0.1f * Projectile.scale,
					SpriteEffects.None,
					0
				);

				// Restore the deferred mode (for the next drawing of things)
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

				return false; // Let the default system handle the base projectile drawing
			}

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }


		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.width = 68;
			Projectile.height = 68;
			Projectile.minionSlots = 0.5f;
			ThemeColor = ColorLib.Rift;
			TintColor = Color.White;
			IdleDustType = ModContent.DustType<RiftDust>();
			DashDustType = ModContent.DustType<RiftDust>();
			TeleDustType = ModContent.DustType<RiftDust>();
			TeleSound = new SoundStyle("DestroyerTest/Assets/Audio/RiftSwordMinionTeleport") with { MaxInstances = 0, PitchVariance = 2 };
			DashSound = new SoundStyle("DestroyerTest/Assets/Audio/RSDash") with { MaxInstances = 0, PitchVariance = 2 };
			AfterImageColorless = true;
			AfterImageTinted = false;
			AfterImage = true;
			DefaultDraw = true;
			TickSpeed = 3;
			UsesParticleOrchestratorOnTele = false;
			TelePRTID = PRTLoader.GetParticleID<Boom1>();
			UsesPRTOnTele = true;
			TeleDist = 8000;
			Range = 8000;
			Style = IdleStyle.Chevron;
			ActiveBuff = ModContent.BuffType<RiftSwordBuff>();
		}

		private void IdlePRT()
		{
			//PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.25f, DTColorUtils.WithAlpha(ColorLib.Rift, 0.25f), 0.5f);
		}


		private bool hasReplaced = false;
		public int ChargeCheckTimer = 0;
		public int SearchTimer = 240; // Unrelated to actually searching for targets. 
		
		public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
		private const int TrailLength = 30;

		public override void AI()
		{
			SearchTimer--;

			ChargeCheckTimer++;

			TrailPositions.Insert(0, Projectile.Center);
			TrailRotations.Insert(0, Projectile.rotation);

			// Cap trail
			while (TrailPositions.Count > TrailLength)
				TrailPositions.RemoveAt(TrailPositions.Count - 1);
			while (TrailRotations.Count > TrailLength)
				TrailRotations.RemoveAt(TrailRotations.Count - 1);

			IdlePRT();

			if (ChargeCheckTimer >= 60)
			{
				ChargeCheckTimer = 0;
				CheckForZeroCharge();
			}


			if (DestroyerTestMod.Config.MinionExtrasToggle == true && TargFlag == false)
			{
				Commentary(Main.player[Projectile.owner], null, false); // Call commentary for the minion, if needed. Pass in null for NPC to skip it.
			}
			base.AI();
		}



		
		public override void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
		{
			// Call base method to set out parameters
			base.GeneralBehavior(owner, out vectorToIdlePosition, out distanceToIdlePosition);
		}
		private HashSet<int> soundPlayedForNPCs = new HashSet<int>(); // Track NPCs that triggered the sound

		private void PlayRadioINSound() {
			SoundStyle RadioIN = new SoundStyle("DestroyerTest/Assets/Audio/RadioIN") {
				Volume = 0.15f
			};
			SoundEngine.PlaySound(RadioIN, Projectile.Center);
		}

		public int CommentaryTimer = 0;
		public void Commentary(Player player, NPC npc, bool foundtarget)
		{
			CommentaryTimer++;
			
			if (player.HeldItem.type == ModContent.ItemType<RiftBroadsword>() && CommentaryTimer >= 360) 
			{
				CombatText.NewText(Projectile.Hitbox, ColorLib.Rift, "Hah! It's me but dumber!", true);
				PlayRadioINSound();
				CommentaryTimer = 0;
			}

			if (player.HeldItem.type == ModContent.ItemType<Living_Shadow>() && CommentaryTimer >= 360) 
			{
				CombatText.NewText(Projectile.Hitbox, ColorLib.Rift, "Time for a recharge already? I'm bursting with energy though!", true);
				PlayRadioINSound();
				CommentaryTimer = 0;
			}

			if (player.HeldItem.type == ModContent.ItemType<Hope_Scabbard>() && CommentaryTimer >= 360) 
			{
				CombatText.NewText(Projectile.Hitbox, ColorLib.Rift, "Why use the magic-based shortswords when you can use me, the AI-Powered Broadsword?", true);
				PlayRadioINSound();
				CommentaryTimer = 0;
			}
			
			if (player.HeldItem.type == ModContent.ItemType<Goliath>() && CommentaryTimer >= 360) 
			{
				CombatText.NewText(Projectile.Hitbox, ColorLib.Rift, "Woah, so Regal!", true);
				PlayRadioINSound();
				CommentaryTimer = 0;
			}

			if (player.HeldItem.type == ModContent.ItemType<Gargantua>() && CommentaryTimer >= 360) 
			{
				CombatText.NewText(Projectile.Hitbox, ColorLib.Rift, "Woah, so Regal!", true);
				PlayRadioINSound();
				CommentaryTimer = 0;
			}

			if (player.HeldItem.type == ModContent.ItemType<Laevateinn>() && CommentaryTimer >= 360) 
			{
				CombatText.NewText(Projectile.Hitbox, ColorLib.Rift, "I can tell already you're a michevous type...", true);
				PlayRadioINSound();
				CommentaryTimer = 0;
			}

			if (player.HeldItem.type == ModContent.ItemType<TrueLaevateinn>() && CommentaryTimer >= 360) 
			{
				CombatText.NewText(Projectile.Hitbox, ColorLib.Rift, "I can tell already you're a michevous type...", true);
				PlayRadioINSound();
				CommentaryTimer = 0;
			}

			if (player.HeldItem.type == ModContent.ItemType<SoulEdge>() && CommentaryTimer >= 360) 
			{
				CombatText.NewText(Projectile.Hitbox, ColorLib.Rift, "Spooky! I like your style!", true);
				PlayRadioINSound();
				CommentaryTimer = 0;
			}

			if (player.HeldItem.type == ModContent.ItemType<TrueSoulEdge>() && CommentaryTimer >= 360) 
			{
				CombatText.NewText(Projectile.Hitbox, ColorLib.Rift, "Spooky! I like your style!", true);
				PlayRadioINSound();
				CommentaryTimer = 0;
			}

			if (player.HeldItem.type == ItemID.TerraBlade && CommentaryTimer >= 360) 
			{
				CombatText.NewText(Projectile.Hitbox, ColorLib.Rift, "Such great power you hold. Why still have me here?", true);
				PlayRadioINSound();
				CommentaryTimer = 0;
			}

			if (player.InModBiome<RiftSurface>() == true && CommentaryTimer >= 1200) 
			{
				CombatText.NewText(Projectile.Hitbox, ColorLib.Rift, "Hah! I feel empowered here!", true);
				PlayRadioINSound();
				CommentaryTimer = 0;
			}

			if (npc != null && npc.boss && CommentaryTimer >= 360) 
			{
				CombatText.NewText(Projectile.Hitbox, ColorLib.Rift, "Hah Hah! Time for combat!", true);
				PlayRadioINSound();
				CommentaryTimer = 0;
			}

			if (player.HasBuff(BuffID.Darkness) && CommentaryTimer >= 360) 
			{
				CombatText.NewText(Projectile.Hitbox, ColorLib.Rift, "Don't worry! I'll keep the way lit!", true);
				PlayRadioINSound();
				CommentaryTimer = 0;
			}



			// Past here is "Ambient Dialogue", which more  or less is just the stuff they say when nothing is happening for extended periods of time.

			if (!foundtarget && CommentaryTimer >= 1200) 
			{
				string[] ambientDialogue = new string[]
				{
					"I wonder if the other rift swords are aware of me... Maybe we're all connected somehow?",
					"Cha Cha Real Smooth...",
					"Knock Knock! Who's there? Nobody.",
					"I could use a sharpening... oh wait, I'm energy-based.",
					"The air is notably dry today...",
					"The void feels oddly quiet today...",
					"Blah Blah Blah Blah...",
					"Radio Silent. Do you copy?",
					"Damn, I kinda want some popcorn right now..."
				};

				string randomDialogue = ambientDialogue[Main.rand.Next(ambientDialogue.Length)];

				// Display the randomly selected dialogue
				CombatText.NewText(Projectile.Hitbox, ColorLib.Rift, randomDialogue, true);
				PlayRadioINSound();
				CommentaryTimer = 0;
			}
			
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			base.OnHitNPC(target, hit, damageDone);
			target.AddBuff(ModContent.BuffType<HeliouricShock>(), 600);
			SoundEngine.PlaySound(SoundID.NPCHit43, target.Center);
			DTOptimizationsConfig optcfg = ModContent.GetInstance<DTOptimizationsConfig>();
			if (optcfg.DisableExcessParticles)
			{
				for (int c = 0; c < 15; c++)
				{
					float offset = Main.rand.NextFloat(0.5f, -0.5f);
					Vector2 velocity = (Projectile.velocity * 0.5f).RotatedBy(offset);
					PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle>(), target.Center, velocity, ColorLib.Rift, 1f, 2, 2);
				}
			}
        }


		private void CheckForZeroCharge()
		{
			if (hasReplaced || Projectile.owner != Main.myPlayer)
				return;

			Player player = Main.LocalPlayer;
			var modPlayer = player.GetModPlayer<LivingShadowPlayer>();

			if (modPlayer.LivingShadowCurrent <= 0)
			{
				hasReplaced = true;

				// Convert into husk and remove this one
				Projectile.Kill(); // Triggers OnKill
			}
		}

		public override void OnKill(int timeLeft)
		{
			if (!hasReplaced || Projectile.owner != Main.myPlayer)
				return;

			Player player = Main.LocalPlayer;
			var modPlayer = player.GetModPlayer<LivingShadowPlayer>();

			if (modPlayer.LivingShadowCurrent <= 0)
			{
				Projectile.NewProjectileDirect(
					Projectile.GetSource_Death(),
					Projectile.position,
					Projectile.oldVelocity,
					ModContent.ProjectileType<RiftSwordMinionHusk>(),
					0,
					0
				);
			}
		}
	}
}
