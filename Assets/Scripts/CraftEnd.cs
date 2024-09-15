using TMPro;
using UnityEngine;

public class CraftEnd : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private InventoryListItem _icon;

    public void Show(CraftRecipe craftRecipe)
    {
        this.gameObject.SetActive(true);
        _icon.SetInventoryItemType(craftRecipe.ResultItem, 1);
        _nameText.text = craftRecipe.Name;
    }

    public void Confirm()
    {
        this.gameObject.SetActive(false);
    }
}
