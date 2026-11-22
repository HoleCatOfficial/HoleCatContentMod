using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class CallOfVoidThrowerItem : GlobalItem
    {
        public override void SetDefaults(Item entity)
        {
            if (DTCrossMod.CallOfVoidIsLoaded)
            {
                Mod Mod = DTCrossMod.CallOfVoidMod;
                List<string> ItemNames = new List<string>
                {
                    "Apeirokyklos",
                    "AzafureSwirlblade",
                    "BlazingSwirlblade",
                    "BlazingSwirlblade",
                    "BrillianceSwirlblade",
                    "ExergySwirlblade",
                    "FlamingSwirlblade",
                    "GlacierSwirlblade",
                    "RunicSwirlblade",
                    "SawofMacrocosm",
                    "AbyssalPiercer",
                    "TheBeginingAndTheEnd",
                    "ProphecyFlyingKnife",
                    "ArbitratorII",
                    "TheDeadCut",
                    "Aquashard",
                    "MewMewBomb",
                    "LightDarkDragonDagger",
                    "AmethystFrisbee",
                    "LunarPlank",
                    "AzafureAirReaper",
                    "Revelation",
                    "AzafureSurgeRod",
                    "ScorchingChakram",
                    "SpearOfRadiance",
                    "ShadewindLance",
                    "AzureOfFirmament",
                };

                List<int> Items = new List<int>();
                for (int i = 0; i < ItemNames.Count; i++)
                {
                    ModItem item = DTCrossMod.GetItem(Mod, ItemNames[i]);
                    if (item == null)
                    {
                        throw new Exception(ItemNames[i]);
                    }

                    Items.Add(item.Type);
                }

                if (Items.Contains(entity.type))
                {
                    entity.DamageType = DamageClass.Throwing;
                }
            }

        
        }
    }

    public class CallOfVoidThrowerProjectile : GlobalProjectile
    {
        public override void SetDefaults(Projectile entity)
        {
            Mod Mod = DTCrossMod.CallOfVoidMod;
            List<string> ProjectileNames = new List<string>
            {
                "AzafureSwirlbladeProj",
                "ApeirokyklosProj",
                "AzafureSwirlbladeSaw",
                "AzafureSwirlbladeMissile",
                "ApeirokyklosSpike",
                "BlazingSwirlbladeProj",
                "BlazingSwirlbladeFlame",
                "BrillianceSwirlbladeProj",
                "ExergySwirlbladeProj",
                "ExergySwirlbladeSaw",
                "FlamingSwirlbladeProj",
                "FlamingSwirlbladeFlame",
                "GlacierSwirlbladeProj",
                "GlacierSwirlbladeSaw",
                "RunicSwirlbladeProj",
                "RunicSwirlbladeBullet",
                "SawofMacrocosmProj",
                "SawofMacrocosmSplit",
                "MacrocosmVoidBolt",
                "MacrocosmSawDash",
                "Echo",
                "SoulOfEcho",
                "SoulOfEclipse",
                "TheBeginning",
                "TheEnd",
                "AbyssTentacle",
                "AbyssalPiercerThrow",
                "AmethystFrisbeeProjectile",
                "AmethystExplosion",
                "AquashardSplit",
                "AquashardThrow",
                "ArbitratorIIThrow",
                "ArbitratorStealthImpactSpawner",
                "ArbitratorStealthImpact",
                "AzureOfFirmamentThrow",
                "FutureKnife",
                "RadianceSpearThrow",
                "ShadewindLanceThrow",
            };

            List<int> Projectiles = new List<int>();
            for (int i = 0; i < ProjectileNames.Count; i++)
            {
                ModProjectile projectile = DTCrossMod.GetProjectile(Mod, ProjectileNames[i]);
                if (projectile == null)
                {
                    throw new Exception(ProjectileNames[i]);
                }

                Projectiles.Add(projectile.Type);
            }


            if (Projectiles.Count == ProjectileNames.Count)
            {
                for (int i = 0; i < Projectiles.Count - 1; i++)
                {
                    if (entity.type == Projectiles[i])
                    {
                        entity.DamageType = DamageClass.Throwing;
                    }
                }
            }
        }
    }
}
