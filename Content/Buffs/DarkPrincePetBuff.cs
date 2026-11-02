using DestroyerTest.Content.Projectiles.Pets;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{

    public class DarkPrincePetBuff : ModBuff
    {

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<DarkPrincePet>());
        }
    }
}