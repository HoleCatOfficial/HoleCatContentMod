using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.Audio;
using Terraria.ModLoader.UI;
using Terraria.ID;
using System.Collections.Generic;
using DestroyerTest.Content.UI;
using System;
public class PrayerUISystem : ModSystem
{
    internal PrayerUI pUI;
    private UserInterface pInterface;

    public override void Load()
    {
        if (!Main.dedServ)
        {
            pUI = new PrayerUI();
            pUI.Activate();
            pInterface = new UserInterface();
            pInterface.SetState(pUI);
        }
    }

    public void Open()
    {
        if (!Main.dedServ)
        {
            pInterface?.SetState(pUI);
        }
    }

    public void Close()
    {
        if (!Main.dedServ)
        {
            pInterface?.SetState(null);
        }
    }
    public override void UpdateUI(GameTime gameTime)
    {
        if (!Main.dedServ)
        {
            if (PrayerUI.Visible)
            {
                pInterface?.Update(gameTime);
            }
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
        if (inventoryIndex != -1 && PrayerUI.Visible)
        {
            layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
                "DestroyerTest: Prayer UI",
                () => { pInterface.Draw(Main.spriteBatch, new GameTime()); return true; },
                InterfaceScaleType.UI)
            );
        }
    }
}
