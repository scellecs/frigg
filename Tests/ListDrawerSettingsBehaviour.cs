namespace Frigg.Tests {
    using UnityEngine;

    public class ListDrawerSettingsBehaviour : MonoBehaviour {
        [ListDrawerSettings(AllowDrag = false, HideAddButton = true, HideRemoveButton = true)]
        public int[] testArray;
        
        [ListDrawerSettings(AllowDrag = false, HideAddButton = false, HideRemoveButton = false)]
        public int[] testArrayTwo;

        [ListDrawerSettings(AllowDrag = true, HideAddButton = true, HideRemoveButton = true, HideHeader = true)]
        public int[] testArrayThree = new []{1,2,3,4,5};
    }
}