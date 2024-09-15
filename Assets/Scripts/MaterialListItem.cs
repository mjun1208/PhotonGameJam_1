using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialListItem : MonoBehaviour
{
    [SerializeField] private InventoryListItem _icon;
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private Image _countImage;

    [SerializeField] private Color _yesColor;
    [SerializeField] private Color _noColor;

    private InventoryItemType _itemType;
    private int _itemCount;

    public void SetInfo(InventoryItemType itemType, int needCount, int hasCount)
    {
        _icon.SetInventoryItemType(itemType, needCount);
        CheckItemCount(needCount, hasCount);
    }
    
    public void CheckItemCount(int needCount, int hasCount)
    {
        _countText.text = $"{hasCount} / {needCount}";
        _countImage.color = needCount <= hasCount ? _yesColor : _noColor;
    }
}
