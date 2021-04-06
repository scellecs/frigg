namespace Assets.Scripts.Tests {
    using System;
    using Attributes.Custom;
    using Attributes.Meta;
    using UnityEngine;

    [HideMonoScript]
    public class InlinePropertyBehaviour : MonoBehaviour {

        [HideLabel]
        [InlineProperty(LabelWidth = 10)]
        public Player    player;
        
        public PlayerTwo playerTwo;

        [Serializable]
        public struct Player {
            public int health;
            public int armor;
            public int damage;
        }
        
        [Serializable]
        [InlineProperty(LabelWidth = 10)]
        public class PlayerTwo {
            public int healthTwo;
            public int armorTwo;
            public int damageTwo;
        }
    }
}