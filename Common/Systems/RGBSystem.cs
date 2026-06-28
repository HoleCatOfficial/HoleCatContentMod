using DestroyerTest.Common;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Lorebooks;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Altar;
using DestroyerTest.Content.Tiles.RoseGarden;
using InnoVault;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using SteelSeries.GameSense;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.WorldBuilding;
using OpenRGB.NET;
using OpenRGB.NET.Utils;
using DestroyerTest.Content.RiftBiome;
using System.Threading.Tasks;
using System.Threading;

/*
namespace DestroyerTest.Common.Systems
{
    
    public class RGBManager : IDisposable
    {
        private OpenRgbClient _client = new();
        private Device[] _devices = [];

        private readonly CancellationTokenSource _cts = new();
        private Task? _task;

        private RGBFrame _currentFrame;
        private readonly object _lock = new();

        public void Initialize()
        {
            _client.Connect();
            _devices = _client.GetAllControllerData();
        }

        public void SetFrame(RGBFrame frame)
        {
            lock (_lock)
            {
                _currentFrame = frame;
            }
        }

        public void Start()
        {
            _task = Task.Run(UpdateLoop);
        }

        private void UpdateLoop()
        {
            while (!_cts.IsCancellationRequested)
            {
                RGBFrame frame;

                lock (_lock)
                {
                    frame = _currentFrame;
                }

                if (frame.DeviceColors != null)
                {
                    for (int i = 0; i < _devices.Length; i++)
                    {
                        if (i < frame.DeviceColors.Length)
                            _client.UpdateLeds(i, frame.DeviceColors[i]);
                    }
                }

                Thread.Sleep(16); // ~60fps
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _task?.Wait();
            _client.Dispose();
        }
    }

    public struct RGBFrame
    {
        public OpenRGB.NET.Color[][] DeviceColors;
    }

    public class RGBSystem : ModSystem
    {
        RGBManager RGB;
        public override void Load()
        {
            RGB = new RGBManager();

            RGB.Initialize();
            RGB.Start();
        }

        public override void PostUpdatePlayers()
        {
            var frame = new RGBFrame();

            var keyboard = new OpenRGB.NET.Color[1][];


            if (Main.LocalPlayer.InModBiome<RiftSurface>())
            {
                keyboard[0] = Enumerable.Repeat(
                    new OpenRGB.NET.Color(255, 155, 0),
                    100
                ).ToArray();


            }

            frame.DeviceColors = keyboard;

            RGB.SetFrame(frame);

        }

        public override void Unload()
        {
            RGB?.Dispose();
        }
    }

}
*/
