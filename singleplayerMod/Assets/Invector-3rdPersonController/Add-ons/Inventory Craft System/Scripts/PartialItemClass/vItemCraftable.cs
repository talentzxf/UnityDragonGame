using System.Collections.Generic;
using UnityEngine;

namespace Invector.vItemManager
{
    /// <summary>
    /// Used to Craft an item
    /// </summary>
    [System.Serializable]
    public class vItemCraft
    {
        public vItem item;
        public int amount;
    }
    public partial class vItem
    {
        /// <summary>
        /// List of materials necessary to craft a new item
        /// </summary>
        [HideInInspector]
        public List<vItemCraft> itemsToCraft = new List<vItemCraft>();
        //[HideInInspector]
        //public bool canCraft = true;
        [HideInInspector]
        public int price = 0;
    }
}