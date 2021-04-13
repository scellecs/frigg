namespace Frigg.Tests {
    using System.Collections.Generic;
    using UnityEngine;

    public class ReorderableListBehaviour : MonoBehaviour{
        
        /*[ReorderableList]
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
        public List<Vector3> vectorlist;*/
        
        [ReorderableList]
        public List<int> intlist;

        public List<string> stringTest;

        [ReorderableList]
        [ShowInInspector]
        [ListDrawerSettings(AllowDrag = false, HideAddButton = true, HideHeader = false, HideRemoveButton = true)]
        private List<int> privateListTestWithAttr;
        
        [ReorderableList]
        [ShowInInspector]
        private List<int> privateListTestWithAttrDummy = new List<int>{1,2,3,4};

        [ReorderableList]
        [ShowInInspector]
        private List<int> PrivatePropertyTestWithAttr => new List<int>{1,2,3,4,5};
        
        [ShowInInspector]
        private List<int> privateListTestWith;
        
        [ShowInInspector]
        private List<int> PrivatePropertyTestWith => new List<int>{1,2,3,4,5};
    }
}