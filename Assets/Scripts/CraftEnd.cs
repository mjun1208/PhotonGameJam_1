using TMPro;
using UnityEngine;

public class CraftEnd : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private InventoryListItem _icon;
    [SerializeField] private TMP_Text _title;

    public void Show(CraftRecipe craftRecipe)
    {
        _title.text = "제작 완료";
        this.gameObject.SetActive(true);
        _icon.SetInventoryItemType(craftRecipe.ResultItem, 1);
        _nameText.text = craftRecipe.Name;
    }
    
    public void Show(SellItem sellItem)
    {
        _title.text = "구매 완료";
        this.gameObject.SetActive(true);
        _icon.SetInventoryItemType(sellItem.InventoryItemType, 1);
        _nameText.text = sellItem.Name;
    }

    public void Confirm()
    {
        this.gameObject.SetActive(false);
    }
}
