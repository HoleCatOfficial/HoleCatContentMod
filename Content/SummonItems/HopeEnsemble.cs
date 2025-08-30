
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Steamworks;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.SummonItems
{
	// This file contains all the code necessary for a minion
	// - ModItem - the weapon which you use to summon the minion with
	// - ModBuff - the icon you can click on to despawn the minion
	// - ModProjectile - the minion itself

	// It is not recommended to put all these classes in the same file. For demonstrations sake they are all compacted together so you get a better overview.
	// To get a better understanding of how everything works together, and how to code minion AI, read the guide: https://github.com/tModLoader/tModLoader/wiki/Basic-Minion-Guide
	// This is NOT an in-depth guide to advanced minion AI
	public class HopeEnsemble_Buff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
			Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
		}

		public override void Update(Player player, ref int buffIndex) {
			// If the minions exist reset the buff time, otherwise remove the buff from the player
			if (player.ownedProjectileCounts[ModContent.ProjectileType<Copper_Broadsword>()] > 0) {
				player.buffTime[buffIndex] = 18000;
			}
			else {
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}

	[AutoloadEquip(EquipType.Waist)]
	public class Hope_Scabbard : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

			ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f; // The default value is 1, but other values are supported. See the docs for more guidance. 
		}

		public override void SetDefaults() {
			Item.damage = 40;
			Item.knockBack = 0f;
			Item.mana = 100; // mana cost
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.RaiseLamp; // how the player's arm moves when using the item
			Item.value = 18000;
			Item.rare = ItemRarityID.Expert;
			Item.UseSound = new SoundStyle($"DestroyerTest/Assets/Audio/HopeScabbardOpen") with {
				Volume = 1.0f, 
    			Pitch = 0.0f, 
    			PitchVariance = 0.5f, 
			}; // The sound when the weapon is being used.
			Item.accessory = true;

			// These below are needed for a minion weapon
			Item.noMelee = true; // this item doesn't do any melee damage
			Item.DamageType = DamageClass.Summon; // Makes the damage register as summon. If your item does not have any damage type, it becomes true damage (which means that damage scalars will not affect it). Be sure to have a damage type
			Item.buffType = ModContent.BuffType<HopeEnsemble_Buff>();
			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ModContent.ProjectileType<Copper_Broadsword>(); // This item creates the minion projectile
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			// Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position
			position = Main.MouseWorld;
		}

       
       // Define minionTypes as a class field so both methods can access it
    public static readonly List<int> minionTypes = new List<int>
    {
        ModContent.ProjectileType<Copper_Broadsword>(),
        ModContent.ProjectileType<Tin_Broadsword>(),
        ModContent.ProjectileType<Iron_Broadsword>(),
        ModContent.ProjectileType<Lead_Broadsword>(),
        ModContent.ProjectileType<Gold_Broadsword>(),
        ModContent.ProjectileType<Silver_Broadsword>(),
        ModContent.ProjectileType<Platinum_Broadsword>(),
        ModContent.ProjectileType<Tungsten_Broadsword>(),
        ModContent.ProjectileType<Blood_Butcherer>(),
        ModContent.ProjectileType<Lights_Bane>()
    };

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
        player.AddBuff(Item.buffType, 2);

        // Iterate through the list and spawn each minion
        foreach (int minionType in minionTypes)
        {
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, minionType, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;
        }

        // Since we spawned the projectile manually already, return false so the game doesn't spawn another one
        return false;
    }

    public void UpdateEquip(Player player, EntitySource_ItemUse_WithAmmo source, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        player.AddBuff(ModContent.BuffType<HopeEnsemble_Buff>(), 60);

        // Use the shared minionTypes list
        foreach (int minionType in minionTypes)
        {
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, minionType, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;
        }
    }


		
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.CopperShortsword, 1)
				.AddIngredient(ItemID.TinShortsword, 1)
				.AddIngredient(ItemID.IronShortsword, 1)
				.AddIngredient(ItemID.LeadShortsword, 1)
				.AddIngredient(ItemID.GoldShortsword, 1)
				.AddIngredient(ItemID.SilverShortsword, 1)
				.AddIngredient(ItemID.PlatinumShortsword, 1)
				.AddIngredient(ItemID.TungstenShortsword, 10)
				.AddIngredient(ItemID.BloodButcherer, 1)
				.AddIngredient(ItemID.LightsBane, 1)
				.AddIngredient(ItemID.Leather, 10)
				.AddIngredient(ItemID.Silk, 5)
				.AddIngredient(ItemID.PinkThread, 5)
				.AddIngredient(ItemID.Wood, 10)
				.AddCondition(Condition.InExpertMode)
				.AddDecraftCondition(Condition.Hardmode)
				.AddTile(TileID.DemonAltar)
				.Register();
		}

	}


	/*
	// This minion shows a few mandatory things that make it behave properly.
	// Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
	// If the player targets a certain NPC with right-click, it will fly through tiles to it
	// If it isn't attacking, it will float near the player with minimal movement
	public class Copper_Shortsword : ModProjectile
	{
		private void GenerateDust()
		{
			
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.Copper,
					0, 0, 254, Scale: 1.0f);
				dust.velocity += Projectile.velocity * 0.5f;
				dust.velocity *= 0.5f;
				dust.noGravity = true;
		
		}

		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
		}

		public sealed override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 28;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely

			// These below are needed for a minion weapon
			Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.minion = true; // Declares this as a minion (has many effects)
			Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
			Projectile.minionSlots = 0.4f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
			
		}

		public override bool PreDraw(ref Color lightColor) {
			// Draws an afterimage trail. See https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile#afterimage-trail for more information.

			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = Projectile.oldPos.Length - 1; k > 0; k--) {
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			return true;
		}


		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles() {
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage() {
			return true;
		}

		// The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
		public override void AI() {

			
			GenerateDust();
			
			Player owner = Main.player[Projectile.owner];

			
			if (!CheckActive(owner)) {
				return;
			}

			GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
			SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
			Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
			Visuals();
		}

		// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
		private bool CheckActive(Player owner) {
			if (owner.dead || !owner.active) {
				owner.ClearBuff(ModContent.BuffType<HopeEnsemble_Buff>());

				return false;
			}

			if (owner.HasBuff(ModContent.BuffType<HopeEnsemble_Buff>())) {
				Projectile.timeLeft = 2;
			}

            
			return true;
		}



		
		private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition) {
			
			GenerateDust();

			Vector2 idlePosition = owner.Center;
			idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

			
			// If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
			// The index is projectile.minionPos
			float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
			idlePosition.X += minionPositionOffsetX; // Go behind the player

			// All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

			// Teleport to player if distance is too big
			vectorToIdlePosition = idlePosition - Projectile.Center;
			distanceToIdlePosition = vectorToIdlePosition.Length();

			if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 1000f) {
				SoundEngine.PlaySound(SoundID.Item4);
				// Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
				// and then set netUpdate to true
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
				ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
				new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(Projectile.Hitbox) },
				Projectile.owner);
			}

			// If your minion is flying, you want to do this independently of any conditions
			float overlapVelocity = 0.04f;

			// Fix overlap with other minions
			foreach (var other in Main.ActiveProjectiles) {
				if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width) {
					if (Projectile.position.X < other.position.X) {
						Projectile.velocity.X -= overlapVelocity;
					}
					else {
						Projectile.velocity.X += overlapVelocity;
					}

					if (Projectile.position.Y < other.position.Y) {
						Projectile.velocity.Y -= overlapVelocity;
					}
					else {
						Projectile.velocity.Y += overlapVelocity;
					}
				}
			}
		}

		private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter) {
			// Starting search distance
			distanceFromTarget = 700f;
			targetCenter = Projectile.position;
			foundTarget = false;

			GenerateDust();
			
			// This code is required if your minion weapon has the targeting feature
			if (owner.HasMinionAttackTargetNPC) {
				NPC npc = Main.npc[owner.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, Projectile.Center);

				// Reasonable distance away so it doesn't target across multiple screens
				if (between < 2000f) {
					distanceFromTarget = between;
					targetCenter = npc.Center;
					foundTarget = true;
				}
			}

			if (!foundTarget) {
				// This code is required either way, used for finding a target
				foreach (var npc in Main.ActiveNPCs) {
					if (npc.CanBeChasedBy()) {
						float between = Vector2.Distance(npc.Center, Projectile.Center);
						bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;
						bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
						// Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
						// The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
						bool closeThroughWall = between < 100f;

						if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall)) {
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}
			
			// friendly needs to be set to true so the minion can deal contact damage
			// friendly needs to be set to false so it doesn't damage things like target dummies while idling
			// Both things depend on if it has a target or not, so it's just one assignment here
			// You don't need this assignment if your minion is shooting things instead of dealing contact damage
			Projectile.friendly = foundTarget;
		}

		private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition) {
			float speed = 50f;
			float inertia = 140f;
			
			GenerateDust();

			if (foundTarget) {
				if (distanceFromTarget > 40f) {
					// If not in "strike-through" mode, home in
					if (Projectile.ai[1] == 0) {
						Vector2 direction = targetCenter - Projectile.Center;
						direction.Normalize();
						direction *= speed;

						float targetAngle = Projectile.AngleTo(targetCenter * MathHelper.ToRadians(360));
						Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;

						// If close enough, enter "strike-through" mode
						if (distanceFromTarget < 50f) {
							SoundEngine.PlaySound(SoundID.Item66);
							Projectile.ai[1] = 1; // Enter strike-through phase
							Projectile.ai[0] = 0; // Reset timer
						}
						Projectile.rotation = targetAngle;
					}
				}
			}

			// If in "strike-through" mode, keep moving forward without changing direction
			if (Projectile.ai[1] == 1) {
				Projectile.ai[0]++; // Increment timer

				if (Projectile.ai[0] < 20) {
					// Keep moving in the same direction for a bit
					Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
				} else {
					// Exit "strike-through" mode after 20 ticks (~1/3 of a second)
					Projectile.ai[1] = 0;
				}
			}

			if (!foundTarget) {
				// Reset "strike-through" state when there's no target
				Projectile.ai[1] = 0;

				if (distanceToIdlePosition > 600f) {
					speed = 12f;
					inertia = 60f;
				}
				else {
					speed = 4f;
					inertia = 80f;
				}

				if (distanceToIdlePosition > 20f) {
					vectorToIdlePosition.Normalize();
					vectorToIdlePosition *= speed;
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
				}
				else if (Projectile.velocity == Vector2.Zero) {
					Projectile.velocity.X = -0.15f;
					Projectile.velocity.Y = -0.05f;
				}
			}
		}


		private void Visuals() {
			// So it will lean slightly towards the direction it's moving
			Projectile.rotation = Projectile.velocity.X * 0.5f;

			GenerateDust();

			// This is a simple "loop through all frames from top to bottom" animation
			//int frameSpeed = 5;

			//Projectile.frameCounter++;

			//if (Projectile.frameCounter >= frameSpeed) {
				//Projectile.frameCounter = 0;
				//Projectile.frame++;

				//if (Projectile.frame >= Main.projFrames[Projectile.type]) {
					//Projectile.frame = 0;
				//}
			//}

			// Some visuals here
			Lighting.AddLight(Projectile.Center, Color.DarkOrange.ToVector3() * 0.78f);
		}
	}
	*/

	public class Copper_Broadsword : SwordMinionTemplate
	{
		public SoundStyle Tele = new SoundStyle("DestroyerTest/Assets/Audio/HopeScabbardTele") with { PitchVariance = 1 };
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.width = 36;
			Projectile.height = 36;
			ThemeColor = Color.OrangeRed;
			TintColor = Color.White;
			IdleDustType = DustID.Copper;
			DashDustType = DustID.Torch;
			TeleDustType = DustID.Torch;
			TeleSound = Tele;
			DashSound = SoundID.Item66;
			AfterImageColorless = true;
			AfterImageTinted = false;
			AfterImage = true;
			DefaultDraw = true;
			TickSpeed = 3;
			UsesParticleOrchestratorOnTele = false;
			TelePRTID = PRTLoader.GetParticleID<Boom1>();
			UsesPRTOnTele = true;
			TeleDist = 2000;
			Range = 2000;
			Style = IdleStyle.Chevron;
			ActiveBuff = ModContent.BuffType<HopeEnsemble_Buff>();
			Projectile.minionSlots = 0.5f;
			Group = Hope_Scabbard.minionTypes;
			UsesGroup = true;
		}
	}
	public class Tin_Broadsword : SwordMinionTemplate
	{
		public SoundStyle Tele = new SoundStyle("DestroyerTest/Assets/Audio/HopeScabbardTele") with { PitchVariance = 1 };
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.width = 36;
			Projectile.height = 36;
			ThemeColor = Color.Wheat;
			TintColor = Color.White;
			IdleDustType = DustID.Tin;
			DashDustType = DustID.Torch;
			TeleDustType = DustID.Torch;
			TeleSound = Tele;
			DashSound = SoundID.Item66;
			AfterImageColorless = true;
			AfterImageTinted = false;
			AfterImage = true;
			DefaultDraw = true;
			TickSpeed = 3;
			UsesParticleOrchestratorOnTele = false;
			TelePRTID = PRTLoader.GetParticleID<Boom1>();
			UsesPRTOnTele = true;
			TeleDist = 2000;
			Range = 2000;
			Style = IdleStyle.Chevron;
			ActiveBuff = ModContent.BuffType<HopeEnsemble_Buff>();
			Projectile.minionSlots = 0.5f;
			Group = Hope_Scabbard.minionTypes;
			UsesGroup = true;
		}

	}
	public class Lead_Broadsword : SwordMinionTemplate
	{
		public SoundStyle Tele = new SoundStyle("DestroyerTest/Assets/Audio/HopeScabbardTele") with { PitchVariance = 1 };
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.width = 36;
			Projectile.height = 36;
			ThemeColor = Color.Navy;
			TintColor = Color.White;
			IdleDustType = DustID.Lead;
			DashDustType = DustID.Torch;
			TeleDustType = DustID.Torch;
			TeleSound = Tele;
			DashSound = SoundID.Item66;
			AfterImageColorless = true;
			AfterImageTinted = false;
			AfterImage = true;
			DefaultDraw = true;
			TickSpeed = 3;
			UsesParticleOrchestratorOnTele = false;
			TelePRTID = PRTLoader.GetParticleID<Boom1>();
			UsesPRTOnTele = true;
			TeleDist = 2000;
			Range = 2000;
			Style = IdleStyle.Chevron;
			ActiveBuff = ModContent.BuffType<HopeEnsemble_Buff>();
			Projectile.minionSlots = 0.5f;
			Group = Hope_Scabbard.minionTypes;
			UsesGroup = true;
		}
	}
	public class Iron_Broadsword : SwordMinionTemplate
	{
		public SoundStyle Tele = new SoundStyle("DestroyerTest/Assets/Audio/HopeScabbardTele") with { PitchVariance = 1 };
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.width = 36;
			Projectile.height = 36;
			ThemeColor = Color.Wheat;
			TintColor = Color.White;
			IdleDustType = DustID.Iron;
			DashDustType = DustID.Torch;
			TeleDustType = DustID.Torch;
			TeleSound = Tele;
			DashSound = SoundID.Item66;
			AfterImageColorless = true;
			AfterImageTinted = false;
			AfterImage = true;
			DefaultDraw = true;
			TickSpeed = 3;
			UsesParticleOrchestratorOnTele = false;
			TelePRTID = PRTLoader.GetParticleID<Boom1>();
			UsesPRTOnTele = true;
			TeleDist = 2000;
			Range = 2000;
			Style = IdleStyle.Chevron;
			ActiveBuff = ModContent.BuffType<HopeEnsemble_Buff>();
			Projectile.minionSlots = 0.5f;
			Group = Hope_Scabbard.minionTypes;
			UsesGroup = true;
		}
	}
	public class Gold_Broadsword : SwordMinionTemplate
	{
		public SoundStyle Tele = new SoundStyle("DestroyerTest/Assets/Audio/HopeScabbardTele") with { PitchVariance = 1 };
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.width = 40;
			Projectile.height = 40;
			ThemeColor = Color.Gold;
			TintColor = Color.White;
			IdleDustType = DustID.GoldCoin;
			DashDustType = DustID.Torch;
			TeleDustType = DustID.Torch;
			TeleSound = Tele;
			DashSound = SoundID.Item66;
			AfterImageColorless = true;
			AfterImageTinted = false;
			AfterImage = true;
			DefaultDraw = true;
			TickSpeed = 3;
			UsesParticleOrchestratorOnTele = false;
			TelePRTID = PRTLoader.GetParticleID<Boom1>();
			UsesPRTOnTele = true;
			TeleDist = 2000;
			Range = 2000;
			Style = IdleStyle.Chevron;
			ActiveBuff = ModContent.BuffType<HopeEnsemble_Buff>();
			Projectile.minionSlots = 0.5f;
			Group = Hope_Scabbard.minionTypes;
			UsesGroup = true;
		}

	}
	public class Silver_Broadsword : SwordMinionTemplate
	{
		public SoundStyle Tele = new SoundStyle("DestroyerTest/Assets/Audio/HopeScabbardTele") with { PitchVariance = 1 };
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.width = 38;
			Projectile.height = 38;
			ThemeColor = Color.White;
			TintColor = Color.White;
			IdleDustType = DustID.Silver;
			DashDustType = DustID.Torch;
			TeleDustType = DustID.Torch;
			TeleSound = Tele;
			DashSound = SoundID.Item66;
			AfterImageColorless = true;
			AfterImageTinted = false;
			AfterImage = true;
			DefaultDraw = true;
			TickSpeed = 3;
			UsesParticleOrchestratorOnTele = false;
			TelePRTID = PRTLoader.GetParticleID<Boom1>();
			UsesPRTOnTele = true;
			TeleDist = 2000;
			Range = 2000;
			Style = IdleStyle.Chevron;
			ActiveBuff = ModContent.BuffType<HopeEnsemble_Buff>();
			Projectile.minionSlots = 0.5f;
			Group = Hope_Scabbard.minionTypes;
			UsesGroup = true;
		}
	}
	public class Platinum_Broadsword : SwordMinionTemplate
	{
		public SoundStyle Tele = new SoundStyle("DestroyerTest/Assets/Audio/HopeScabbardTele") with { PitchVariance = 1 };
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.width = 40;
			Projectile.height = 40;
			ThemeColor = Color.GhostWhite;
			TintColor = Color.White;
			IdleDustType = DustID.Platinum;
			DashDustType = DustID.Torch;
			TeleDustType = DustID.Torch;
			TeleSound = Tele;
			DashSound = SoundID.Item66;
			AfterImageColorless = true;
			AfterImageTinted = false;
			AfterImage = true;
			DefaultDraw = true;
			TickSpeed = 3;
			UsesParticleOrchestratorOnTele = false;
			TelePRTID = PRTLoader.GetParticleID<Boom1>();
			UsesPRTOnTele = true;
			TeleDist = 2000;
			Range = 2000;
			Style = IdleStyle.Chevron;
			ActiveBuff = ModContent.BuffType<HopeEnsemble_Buff>();
			Projectile.minionSlots = 0.5f;
			Group = Hope_Scabbard.minionTypes;
			UsesGroup = true;
		}
	}
	public class Tungsten_Broadsword : SwordMinionTemplate
	{
		public SoundStyle Tele = new SoundStyle("DestroyerTest/Assets/Audio/HopeScabbardTele") with { PitchVariance = 1 };
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.width = 38;
			Projectile.height = 38;
			ThemeColor = Color.LightGreen;
			TintColor = Color.White;
			IdleDustType = DustID.Tungsten;
			DashDustType = DustID.Torch;
			TeleDustType = DustID.Torch;
			TeleSound = Tele;
			DashSound = SoundID.Item66;
			AfterImageColorless = true;
			AfterImageTinted = false;
			AfterImage = true;
			DefaultDraw = true;
			TickSpeed = 3;
			UsesParticleOrchestratorOnTele = false;
			TelePRTID = PRTLoader.GetParticleID<Boom1>();
			UsesPRTOnTele = true;
			TeleDist = 2000;
			Range = 2000;
			Style = IdleStyle.Chevron;
			ActiveBuff = ModContent.BuffType<HopeEnsemble_Buff>();
			Projectile.minionSlots = 0.5f;
			Group = Hope_Scabbard.minionTypes;
			UsesGroup = true;
		}
	}
	public class Blood_Butcherer : SwordMinionTemplate
	{
		public SoundStyle Tele = new SoundStyle("DestroyerTest/Assets/Audio/HopeScabbardTele") with { PitchVariance = 1 };
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.width = 50;
			Projectile.height = 58;
			ThemeColor = Color.Red;
			TintColor = Color.White;
			IdleDustType = DustID.RedTorch;
			DashDustType = DustID.Torch;
			TeleDustType = DustID.Torch;
			TeleSound = Tele;
			DashSound = SoundID.Item66;
			AfterImageColorless = true;
			AfterImageTinted = false;
			AfterImage = true;
			DefaultDraw = true;
			TickSpeed = 3;
			UsesParticleOrchestratorOnTele = false;
			TelePRTID = PRTLoader.GetParticleID<Boom1>();
			UsesPRTOnTele = true;
			TeleDist = 2000;
			Range = 2000;
			Style = IdleStyle.Chevron;
			ActiveBuff = ModContent.BuffType<HopeEnsemble_Buff>();
			Projectile.minionSlots = 0.5f;
			noDrawTint = true;
			Group = Hope_Scabbard.minionTypes;
			UsesGroup = true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			//lightColor = ThemeColor;
			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

			Texture2D pixel = TextureAssets.MagicPixel.Value;

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
				Color color = ThemeColor * alpha;

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


			return base.PreDraw(ref lightColor); // Let the default system handle the base projectile drawing
		}

		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 30;
		public int ShootTimer = 0;

		public override void AI()
		{
			TrailPositions.Insert(0, Projectile.Center);
			TrailRotations.Insert(0, Projectile.rotation);

			// Cap trail
			while (TrailPositions.Count > TrailLength)
				TrailPositions.RemoveAt(TrailPositions.Count - 1);
			while (TrailRotations.Count > TrailLength)
				TrailRotations.RemoveAt(TrailRotations.Count - 1);

			
				if (TargFlag && ShootTimer <= 0)
				{
					float rotation = MathHelper.ToRadians(45);

					Vector2 ShootOrig = Projectile.Center;
					Vector2 Velocity = Projectile.velocity * 2;

					ShootOrig += Vector2.Normalize(Velocity) * 5f;

					for (int i = 0; i < 6; i++)
					{
						Vector2 perturbedSpeed = Velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (6 - 1))); // Watch out for dividing by 0 if there is only 1 projectile.
						Projectile.NewProjectile(Entity.GetSource_FromThis(), ShootOrig, perturbedSpeed, ModContent.ProjectileType<EnchantedBlood>(), Projectile.damage / 3, Projectile.knockBack);
					}
					ShootTimer = 240;
				}
				if (TargFlag && ShootTimer > 0)
				{

				}
				if (ShootTimer > 0)
				{
					ShootTimer--;
				}
			base.AI();
		}
	}
    public class Lights_Bane : SwordMinionTemplate
	{
		public SoundStyle Tele = new SoundStyle("DestroyerTest/Assets/Audio/HopeScabbardTele") with { PitchVariance = 1 };
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.width = 50;
			Projectile.height = 50;
			ThemeColor = Color.Purple;
			TintColor = Color.White;
			IdleDustType = DustID.Shadowflame;
			DashDustType = DustID.Torch;
			TeleDustType = DustID.Torch;
			TeleSound = Tele;
			DashSound = SoundID.Item66;
			AfterImageColorless = true;
			AfterImageTinted = false;
			AfterImage = true;
			DefaultDraw = true;
			TickSpeed = 3;
			UsesParticleOrchestratorOnTele = false;
			TelePRTID = PRTLoader.GetParticleID<Boom1>();
			UsesPRTOnTele = true;
			TeleDist = 2000;
			Range = 2000;
			Style = IdleStyle.Chevron;
			ActiveBuff = ModContent.BuffType<HopeEnsemble_Buff>();
			Projectile.minionSlots = 0.5f;
			noDrawTint = true;
			Group = Hope_Scabbard.minionTypes;
			UsesGroup = true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			//lightColor = ThemeColor;
			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

			Texture2D pixel = TextureAssets.MagicPixel.Value;

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
				Color color = ThemeColor * alpha;

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
			return base.PreDraw(ref lightColor); // Let the default system handle the base projectile drawing
		}

		public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
		private const int TrailLength = 30;
		public int ShootTimer = 0;

		public override void AI()
		{
			TrailPositions.Insert(0, Projectile.Center);
			TrailRotations.Insert(0, Projectile.rotation);

			// Cap trail
			while (TrailPositions.Count > TrailLength)
				TrailPositions.RemoveAt(TrailPositions.Count - 1);
			while (TrailRotations.Count > TrailLength)
				TrailRotations.RemoveAt(TrailRotations.Count - 1);

			
				if (TargFlag && ShootTimer <= 0)
				{
					float rotation = MathHelper.ToRadians(45);

					Vector2 ShootOrig = Projectile.Center;
					Vector2 Velocity = Projectile.velocity *  2;

					ShootOrig += Vector2.Normalize(Velocity) * 5f;

					for (int i = 0; i < 6; i++)
					{
						Vector2 perturbedSpeed = Velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (6 - 1))); // Watch out for dividing by 0 if there is only 1 projectile.
						Projectile.NewProjectile(Entity.GetSource_FromThis(), ShootOrig, perturbedSpeed, ModContent.ProjectileType<EnchantedShadowflame>(), Projectile.damage / 3, Projectile.knockBack);
					}
					ShootTimer = 240;
				}
				if (TargFlag && ShootTimer > 0)
				{

				}
				if (ShootTimer > 0)
				{
					ShootTimer--;
				}
			
			base.AI();
		}
	}
}
