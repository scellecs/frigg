namespace Assets.Scripts.Tests {
    using System;
    using Attributes.Custom;
    using UnityEngine;

    public class InlinePropertyBehaviour : MonoBehaviour {

        [InlineProperty(LabelWitdh = 10)]
        public Player    player;
        
        public PlayerTwo playerTwo;

        [Serializable]
        public struct Player {
            public int health;
            public int armor;
            public int damage;
        }
        
        [Serializable]
        [InlineProperty(LabelWitdh = 10)]
        public class PlayerTwo {
            public int healthTwo;
            public int armorTwo;
            public int damageTwo;
        }
    }
}