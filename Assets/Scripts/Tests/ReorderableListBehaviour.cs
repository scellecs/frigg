namespace Assets.Scripts.Tests {
    using System.Collections.Generic;
    using Attributes.Custom;
    using Attributes.Meta;
    using UnityEngine;

    public class ReorderableListBehaviour : MonoBehaviour{
        
        [ReorderableList]
        public int[] intArray;

        [ReorderableList]
        public List<int> intlist;
        
        [ReorderableList]
        public float[] floatArray;
        
        [ReorderableList]
        public List<float> floatlist;
        
        [ReorderableList]
        public Vector3[] vectorArray;
        
        [ReorderableList]
        public List<Vector3> vectorlist;
    }
}