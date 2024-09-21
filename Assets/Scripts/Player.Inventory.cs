using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

public partial class Player
{
    [SerializeField] private InventoryBar _inventoryBar;
    [SerializeField] private InventoryUI _inventoryUI;
    [SerializeField] private ChestUI _chestUI;
    [SerializeField] private UnlockUI _unlockUI;
    private InventoryItemType _inventoryItemType { get; set; }
    
    [Networked] private NetworkBool _isInventoryOpen { get; set; } = false;
    [Networked] private NetworkBool _isChestOpen { get; set; } = false;
    [Networked] private NetworkBool IsUnlockOpen { get; set; } = false;

    private Chest _lookingChest;

    // public void OnChangedEquipItem()
    // {
    //     Equip(_inventoryItemType);
    // }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcOpenUnlockUI(bool isOpen)
    {
        IsUnlockOpen = isOpen;
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcOpenChestUI(bool isOpen)
    {
        _isChestOpen = isOpen;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcOpenInventoryUI(bool isOpen)
    {
        _isInventoryOpen = isOpen;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcEquipToServer(InventoryItemType inventoryItemType, bool isEmpty)
    {
        Equip(inventoryItemType, isEmpty);
        
        RpcEquipToAll(inventoryItemType, isEmpty);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcEquipToAll(InventoryItemType inventoryItemType, bool isEmpty)
    {
        Equip(inventoryItemType, isEmpty);
    }
    
    public void Equip(InventoryItemType inventoryItemType, bool isEmpty)
    {
        if (_inventoryItemType != inventoryItemType)
        {
            _inventoryItemType = inventoryItemType;
        }

        if (inventoryItemType == InventoryItemType.Table)
        {
            if (HasInputAuthority)
            {
                Global.Instance.IngameManager.IsTabling = true;
                Global.Instance.IngameManager.Tables.ForEach(x=> x.ShowSizeObject(true));
            }
        }
        else
        {
            if (HasInputAuthority)
            {
                Global.Instance.IngameManager.IsTabling = false;
                Global.Instance.IngameManager.Tables.ForEach(x=> x.ShowSizeObject(false));
            }
        }

        SetToolFalse();

        if (isEmpty || inventoryItemType == InventoryItemType.None)
        {
            _inventoryItemType = InventoryItemType.None;
            return;
        }

        switch (_inventoryItemType)
        {
            case InventoryItemType.Shovel:
            {
                _shovel.SetActive(true);
                break;
            }
            case InventoryItemType.SeedBag_Corn:
            {
                _seedBag.SetActive(true);
                break;
            }
            case InventoryItemType.FishRod:
            {
                _fishRod.SetActive(true);
                break;
            }
            case InventoryItemType.Fish:
            {
                // _fishWeapon.gameObject.SetActive(true);
                // _fishWeapon2222.gameObject.SetActive(true);
                break;
            }
            case InventoryItemType.Axe:
            {
                _axe.gameObject.SetActive(true);   
                break;
            }
            case InventoryItemType.BonFire:
            {
                break;
            }
            case InventoryItemType.Log:
            {
                break;
            }
            case InventoryItemType.Table:
            {
                break;
            }
            default:
            {
                break;
            }
        }
    }
    
    private void GetChest()
    {
        LayerMask chestLayer = 1 << LayerMask.NameToLayer("Chest");
        
        if (Physics.Raycast(_playerCameraRootTransform.transform.position, _playerCameraRootTransform.transform.forward,
                out RaycastHit hit, InteractionRayCastDistance))
        {
            var chest = hit.transform.GetComponent<Chest>();
            if (chest)
            {
                if (_lookingChest != null && _lookingChest != chest)
                {
                    _lookingChest.Look(false);
                    _lookingChest = null;
                }
                
                chest.Look(true);
                _lookingChest = chest;
            }
            else
            {
                if (_lookingChest != null)
                {
                    _lookingChest.Look(false);
                    _lookingChest = null;
                }
            }
        }
        else
        {
            if (_lookingChest != null)
            {
                _lookingChest.Look(false);
                _lookingChest = null;
            }
        }

        if (_lookingChest != null)
        {
            _interactionText.text = "클릭 - 상자 열기";
            _interactionText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            DisableInteractionText();
        }
    }

    public async void OpenUnlockUI(int wave)
    {
        if (!CraftRecipeManager.HasNewRecipes(wave))
        {
            return;
        }
        
        _unlockUI.gameObject.SetActive(true);
        _unlockUI.SetNewRecipe(wave);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Global.Instance.IngameActivingCursor = true;

        await UniTask.NextFrame();

        RpcOpenUnlockUI(true);
    }
}
