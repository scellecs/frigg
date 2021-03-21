namespace Assets.Scripts.Tests {
    using System.Collections.Generic;
    using Attributes;
    using UnityEngine;

    public class IListNoAttrBehaviour : MonoBehaviour {
        
        public int[] intArrayNoAttr;
        
        public List<int> intListNoAttr;
        
        public float[] floatArrayNoAttr;
        
        public List<float> floatListNoAttr;
        
        public Vector3[] vectorArray;
        
        public List<Vector3> vectorList;
    }
}