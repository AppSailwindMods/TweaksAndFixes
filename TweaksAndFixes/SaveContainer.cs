using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweaksAndFixes
{
    [System.Serializable]
    internal class SaveContainer
    {
        public List<HookItemSaveable> hookItems = new List<HookItemSaveable>();
        public List<InInventorySaveable> inInventoryItems = new List<InInventorySaveable>();
        public List<ItemDataSaveable> itemDatas = new List<ItemDataSaveable>();
    }

    [System.Serializable]
    public class HookItemSaveable
    {
        public int prefabIndex;
        public int hookItemIndex;

        public HookItemSaveable(int prefabIndex, int hookItemIndex)
        {
            this.prefabIndex = prefabIndex;
            this.hookItemIndex = hookItemIndex;
        }
    }

    [System.Serializable]
    public class InInventorySaveable
    {
        public int prefabIndex;
        public bool inInventory;

        public InInventorySaveable(int prefabIndex, bool inInventory)
        {
            this.prefabIndex = prefabIndex;
            this.inInventory = inInventory;
        }
    }

    [System.Serializable]
    public class ItemDataSaveable
    {
        public int prefabIndex;
        public string dataName;
        public object data;

        public ItemDataSaveable(int prefabIndex, string dataName, object data)
        {
            this.prefabIndex = prefabIndex;
            this.dataName = dataName;
            this.data = data;
        }
    }
}
