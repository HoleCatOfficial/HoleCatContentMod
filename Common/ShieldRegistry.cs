using System;
using System.Collections.Generic;
using Terraria;

namespace DestroyerTest.Common
{
    public static class ShieldRegistry
    {
        private static readonly Dictionary<int, Type> _registeredShields = new();
        private static readonly Dictionary<Type, int> _reverseLookup = new();
        private static int _nextID = 0;

        /// <summary>
        /// Registers a ShieldPlayer subclass with an auto-assigned ID.
        /// </summary>
        public static int Register<T>() where T : ShieldPlayer, new()
        {
            Type type = typeof(T);

            if (_reverseLookup.ContainsKey(type))
                return _reverseLookup[type]; // Already registered

            int id = _nextID++;
            _registeredShields[id] = type;
            _reverseLookup[type] = id;
            return id;
        }

        /// <summary>
        /// Gets a ShieldPlayer subclass by its ID.
        /// </summary>
        public static Type GetShieldType(int id)
        {
            return _registeredShields.TryGetValue(id, out var type) ? type : null;
        }

        /// <summary>
        /// Gets a Shield ID from its subclass type.
        /// </summary>
        public static int GetShieldID(Type type)
        {
            return _reverseLookup.TryGetValue(type, out var id) ? id : -1;
        }

        /// <summary>
        /// Gets the Shield ID for a given instance.
        /// </summary>
        public static int GetShieldID(ShieldPlayer shield)
        {
            return shield == null ? -1 : GetShieldID(shield.GetType());
        }

        /// <summary>
        /// Instantiates a new ShieldPlayer for a given ID.
        /// </summary>
        public static ShieldPlayer CreateInstance(int id)
        {
            if (!_registeredShields.TryGetValue(id, out var type))
                return null;

            return (ShieldPlayer)Activator.CreateInstance(type);
        }

        /// <summary>
        /// Clears all registered shield entries.
        /// </summary>
        public static void Clear()
        {
            _registeredShields.Clear();
            _reverseLookup.Clear();
            _nextID = 0;
        }
    }
}
