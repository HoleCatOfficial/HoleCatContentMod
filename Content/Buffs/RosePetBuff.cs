using DestroyerTest.Content.Projectiles.Pets;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{

    public class RosePetBuff : ModBuff
    {

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
            Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
            Main.vanityPet[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        { // This method gets called every frame your buff is active on your player.
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<RosePet>());
        }
    }
}