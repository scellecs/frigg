namespace Frigg.Tests {
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ReorderableListBehaviour : MonoBehaviour {
        public List<TestClass> testClasses = new List<TestClass>();
        public List<TestClassSingle> testClassesSingles = new List<TestClassSingle>();
        
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

        [Serializable]
        public class TestClass {
            [HideLabel]
            public int testIntegerClass;

            [HideLabel]
            public string testStringClass;
        }
        
        [Serializable]
        public class TestClassSingle {
            [HideLabel]
            public int testIntegerSingle;
        }
    }
}