using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryListItemDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public InventoryListItem InventoryListItem;

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryListItem.Dragging = true;
        InventoryListItem.SetEmpty(true);

        InventoryListItem.InventoryUI.OnBeginDrag(eventData, InventoryListItem, InventoryListItem.GetInventoryItemType);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryListItem.Dragging = false;
        
        GameObject hoveredObject = eventData.pointerCurrentRaycast.gameObject;

        if (hoveredObject != null)
        {
            // 드래그 중인 슬롯 은 현재

            // 마우스 아래 슬롯
            InventoryListItem targetSlot = hoveredObject.GetComponent<InventoryListItem>();

            if (targetSlot != null)
            {
                InventoryListItem.InventoryUI.OnDrop(eventData, targetSlot.GetInventoryItemType, targetSlot.Empty);

                targetSlot.SetInventoryItemType(InventoryListItem.InventoryUI.GetDraggingItemType);

                if (targetSlot.InventoryBar != null)
                {
                    targetSlot.InventoryBar.InventoryListItems[targetSlot.InventoryBarIndex]
                        .SetInventoryItemType(targetSlot.GetInventoryItemType);
                    if (targetSlot.InventoryBar.CurrentIndex == targetSlot.InventoryBarIndex)
                    {
                        targetSlot.InventoryBar.SelectItem(targetSlot.InventoryBarIndex);
                    }
                }
            }
            else
            {
                InventoryListItemDrag targetSlot2 = hoveredObject.GetComponent<InventoryListItemDrag>();
                
                if (targetSlot2 != null)
                {
                    InventoryListItem.InventoryUI.OnDrop(eventData, targetSlot2.InventoryListItem.GetInventoryItemType, targetSlot2.InventoryListItem.Empty);

                    targetSlot2.InventoryListItem.SetInventoryItemType(InventoryListItem.InventoryUI.GetDraggingItemType);

                    if (targetSlot2.InventoryListItem.InventoryBar != null)
                    {
                        targetSlot2.InventoryListItem.InventoryBar.InventoryListItems[targetSlot2.InventoryListItem.InventoryBarIndex]
                            .SetInventoryItemType(targetSlot2.InventoryListItem.GetInventoryItemType);
                        if (targetSlot2.InventoryListItem.InventoryBar.CurrentIndex == targetSlot2.InventoryListItem.InventoryBarIndex)
                        {
                            targetSlot2.InventoryListItem.InventoryBar.SelectItem(targetSlot2.InventoryListItem.InventoryBarIndex);
                        }
                    }
                }
                else
                {
                    InventoryListItem.InventoryUI.OnDrop(); 
                }
            }
        }
        else
        {
            InventoryListItem.InventoryUI.OnDrop(); 
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        InventoryListItem.Dragging = true;
    }
}
