using UnityEngine;

public class CraftRecipeListItem : MonoBehaviour
{
    private CraftRecipe _craftRecipe;
    private InventoryUI _inventoryUI;

    public void SetInventoryUI(InventoryUI inventoryUI, CraftRecipe craftRecipe)
    {
        _inventoryUI = inventoryUI;
        _craftRecipe = craftRecipe;
    }

    public void SetCraftAble()
    {
        
    }
}
