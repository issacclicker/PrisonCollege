using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : PersistentSingleton<InventorySystem>
{
    private List<PassiveItem> passiveItemList;


    public List<PassiveItem> PassiveItemList => passiveItemList;
}
