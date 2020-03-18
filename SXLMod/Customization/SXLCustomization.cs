using System;
using System.Collections.Generic;

using UnityEngine;

using Harmony12;

namespace SXLMod.Customization
{
    class SXLCustomization
    {
        public static readonly string MOD_PATH = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SkaterXL\\Skins\\";

        private static Transform[] _playerComponents;
        public static Transform[] playerComponents {
            get {
                if (_playerComponents == null)
                {
                    _playerComponents = GetPlayerInstanceComponents();
                }
                return _playerComponents;
            }
        }

        private static GameObject _skater;
        public static GameObject skater {
            get {
                if (_skater == null)
                {
                    _skater = GetSkater();
                }
                return _skater;
            }
        }

        private static CharacterCustomizer _characterCustomizer;
        public static CharacterCustomizer characterCustomizer
        {
            get {
                if (_characterCustomizer == null)
                {
                    _characterCustomizer = skater.GetComponent<CharacterCustomizer>();
                }
                return _characterCustomizer;
            }
        }

        public static Transform[] GetPlayerInstanceComponents()
        {
            return PlayerController.Instance.gameObject.GetComponentsInChildren<Transform>();
        }

        private static GameObject GetSkater()
        {
            for (int i=0; i < playerComponents.Length; i++)
            {
                if (playerComponents[i].gameObject.name.Equals("NewSkater"))
                {
                    if (playerComponents[i].Find("NewSteezeIK"))
                    {
                        return playerComponents[i].gameObject;
                    }
                }
            }
            return null;
        }

        public static List<Tuple<CharacterGear, GameObject>> GetGearList()
        {
            return Traverse.Create(characterCustomizer).Field("equippedGear").GetValue() as List<Tuple<CharacterGear, GameObject>>;
        }
    }
}
