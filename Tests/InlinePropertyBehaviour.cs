namespace Frigg.Tests {
    using System;
    using UnityEngine;

    [HideMonoScript]
    public class InlinePropertyBehaviour : MonoBehaviour {

        [HideLabel]
        [InlineProperty(LabelWidth = 10)]
        public Player    player;

        [PropertySpace(SpaceBefore = 15)]
        [Title("Foldout")]
        public Player playerInsideFoldout;
        
        [PropertySpace(SpaceBefore = 15)]
        [Title("Foldout (Read only)")]
        [Readonly]
        public Player playerInsideFoldoutReadOnly;

        [Readonly]
        [PropertySpace(SpaceBefore = 15)]
        [Title("Inline property (ReadOnly)")]
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
            [Title("Some title inside")]
            public int someMore;
        }
    }
}