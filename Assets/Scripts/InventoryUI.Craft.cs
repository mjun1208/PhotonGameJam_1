using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class InventoryUI : MonoBehaviour
{
    [SerializeField] private CraftInfo _craftInfo;
    [SerializeField] private Player _player;
    [SerializeField] private CraftEnd _craftEnd;
    [SerializeField] private Transform _recipeListParent;
    [SerializeField] private CraftRecipeListItem _originCraftRecipeListItem;

    private List<CraftRecipeListItem> _craftRecipeListItems = new List<CraftRecipeListItem>();

    private CraftRecipe _craftRecipe = null;

    public void OnClickCook()
    {
        _inventoryTab = InventoryTab.Craft;
        
        _craftTabButton.color = Color.white;
        _inventoryTabButton.color = Color.gray;
        
        _shopGroup.SetActive(false);
        _invenGroup.SetActive(true);
        
        _craftGroup.SetActive(true);
        _inventoryGroup.SetActive(false);
        
        SetRecipes(CraftRecipeManager.CookRecipes);
        
        OnClickRecipe(CraftRecipeManager.CookRecipes[0]);
    }
    
    public void OnClickCraftTab()
    {
        _inventoryTab = InventoryTab.Craft;
        
        _craftTabButton.color = Color.white;
        _inventoryTabButton.color = Color.gray;
        
        _shopGroup.SetActive(false);
        _invenGroup.SetActive(true);
        
        _craftGroup.SetActive(true);
        _inventoryGroup.SetActive(false);

        SetRecipes(CraftRecipeManager.CraftRecipes);

        OnClickRecipe(CraftRecipeManager.CraftRecipes[0]);
        
        // Tutorial
        Global.Instance.IngameManager.ServerOnlyGameManager.TutorialManager.SetTutorialIndex(3);
    }

    private void SetRecipes(List<CraftRecipe> craftRecipes)
    {
        _craftRecipeListItems.ForEach(x=> x.gameObject.SetActive(false));

        var craftRecipesByWave = craftRecipes
            .Where(x => x.OpenWave <= Global.Instance.IngameManager.ServerOnlyGameManager.CraftWave).ToList();
        
        for (int i = 0; i < craftRecipesByWave.Count; i++)
        {
            if (i >= _craftRecipeListItems.Count - 1)
            {
                var newRecipeListItem = Instantiate(_originCraftRecipeListItem, _recipeListParent);
                _craftRecipeListItems.Add(newRecipeListItem);
            }

            _craftRecipeListItems[i].SetInventoryUI(this, craftRecipesByWave[i], OnClickRecipe);
            _craftRecipeListItems[i].SetCraftAble(CraftAble(craftRecipesByWave[i]));
            _craftRecipeListItems[i].gameObject.SetActive(true);
        }
    }

    private void UpdateRecipes()
    {
        _craftRecipeListItems.ForEach(x=>
        {
            if (x.gameObject.activeSelf)
            {
                x.SetCraftAble(CraftAble(x.GetRecipe));
            }
        });
    }

    public int GetInventoryItemCount(InventoryItemType type)
    {
        var inventoryList = _inventoryBarListItems.ToList();
        inventoryList.AddRange(_inventoryListItems.ToList());

        int count = 0;
        
        foreach (var inventoryListItem in inventoryList)
        {
            if (!inventoryListItem.Empty)
            {
                if (type == inventoryListItem.GetInventoryItemType)
                {
                    count += inventoryListItem.ItemCount;
                }
            }
        }

        return count;
    }

    public bool CraftAble(CraftRecipe craftRecipe)
    {
        var inventoryList = _inventoryBarListItems.ToList();
        inventoryList.AddRange(_inventoryListItems.ToList());

        foreach(var material in craftRecipe.Material)
        {
            if (GetInventoryItemCount(material.Key) < material.Value)
            {
                return false;
            }
        }
        
        return true;
    }

    public void Craft()
    {
        if (!CraftAble(_craftRecipe))
        {
            _player.ShowNotice("재료가 부족합니다", Color.red);
            return;
        }
        
        foreach (var materialItem in _craftRecipe.Material)
        {
            RemoveItem(materialItem.Key, materialItem.Value);
        }
        
        AddItem(_craftRecipe.ResultItem, 1);
        
        _craftInfo.SetInfo(_craftRecipe);
        
        _craftEnd.Show(_craftRecipe);
        
        UpdateRecipes();

        if (_craftRecipe.ResultItem == InventoryItemType.Shovel)
        {
            // Tutorial
            Global.Instance.IngameManager.ServerOnlyGameManager.TutorialManager.SetTutorialIndex(4);
        }
        
        if (_craftRecipe.ResultItem == InventoryItemType.BonFire)
        {
            // Tutorial
            Global.Instance.IngameManager.ServerOnlyGameManager.TutorialManager.SetTutorialIndex(7);
        }
        
        if (_craftRecipe.ResultItem == InventoryItemType.CookedCorn)
        {
            // Tutorial
            Global.Instance.IngameManager.ServerOnlyGameManager.TutorialManager.SetTutorialIndex(8);
        }
        
        if (_craftRecipe.ResultItem == InventoryItemType.Table)
        {
            // Tutorial
            Global.Instance.IngameManager.ServerOnlyGameManager.TutorialManager.SetTutorialIndex(9);
        }
        
        // _player.ShowNotice("제작 완료", Color.green);
    }

    public void OnClickRecipe(CraftRecipe craftRecipe)
    {
        _craftRecipe = craftRecipe;
        _craftInfo.SetInfo(craftRecipe);
    }
}

public class CraftRecipe
{
    public InventoryItemType ResultItem;
    public string Name;
    public string Desc;
    public Dictionary<InventoryItemType, int> Material;

    public int OpenWave;

    // public bool CraftAble(InventoryUI inventoryUI)
    // {
    //     inventoryUI.GetFirstItemSlot()
    //     
    //     return false;
    // }
}

public static class CraftRecipeManager
{
    public static List<CraftRecipe> CraftRecipes = new List<CraftRecipe>()
    {
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.Axe,
            Name = "도끼",
            Desc = "이걸 사용하면!!\n나무를 자르거나,\n동물의 고기를 얻을 수 있다",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Log, 1},
            },
            OpenWave = 0,
        },
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.Shovel,
            Name = "삽",
            Desc = "땅을 파는데 사용하는 도구.",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Log, 1},
            },
            OpenWave = 0,
        },
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.BonFire,
            Name = "모닥불",
            Desc = "뜨끈뜨끈 하다.\n여기서 요리를 할 수 있을 것 같다",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Log, 5},
            },
            OpenWave = 0,
        },
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.Table,
            Name = "테이블",
            Desc = "손님 맞을 준비 OK?",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Log, 5},
            },
            OpenWave = 0,
        },
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.FishRod,
            Name = "낚시대",
            Desc = "긴 막대에 실을 달고 미끼를 달아서 물고기를 유인하고, 걸리면 당기는 도구. 물에서 사용해보자",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Log, 5},
            },
            OpenWave = 3,
        },
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.Cup,
            Name = "컵",
            Desc = "물 따위의 액체를 담는 잔.\n일회용인듯 하다",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Log, 1},
            },
            OpenWave = 3,
        },
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.Plate,
            Name = "그릇",
            Desc = "음식이나 물건 따위를 담는 도구.\n일회용인듯 하다",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Log, 1},
            },
            OpenWave = 5,
        },
        
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.Axe_1,
            Name = "좋은 도끼",
            Desc = "더 강력하다!",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Log, 10},
            },
            OpenWave = 5,
        },
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.Axe_2,
            Name = "더 좋은 도끼!!",
            Desc = "소가 두방에 죽음!!",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Log, 15},
            },
            OpenWave = 10,
        },
    };
    
    public static List<CraftRecipe> CookRecipes = new List<CraftRecipe>()
    {
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.CookedCorn,
            Name = "구운 옥수수",
            Desc = "여름 제철의 옥수수를 구웠다.\n여름이였다.",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Corn, 1},
            },
            OpenWave = 0,
        },
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.CookedChicken,
            Name = "구운 치킨",
            Desc = "닭을 구워버렸다.\n",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Chicken, 1},
            },
            OpenWave = 2,
        },
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.FishWater,
            Name = "물고기 즙",
            Desc = "물고기를 짜낸 즙.\n우엑.",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Fish, 1},
                {InventoryItemType.Cup, 1},
            },
            OpenWave = 3,
        },
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.AppleCarrotJuice,
            Name = "캐플",
            Desc = "당근과 사과를 합친 주스.",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Carrot, 1},
                {InventoryItemType.Apple, 1},
            },
            OpenWave = 4,
        },
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.CornPie,
            Name = "옥수수 파이",
            Desc = "옥수수로 만든 파이.",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Corn, 2},
                {InventoryItemType.Plate, 1},
            },
            OpenWave = 5,
        },
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.PigAndChicken,
            Name = "삼겹살 닭꼬치",
            Desc = "",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Pig, 1},
                {InventoryItemType.Chicken, 2},
                {InventoryItemType.Plate, 1},
            },
            OpenWave = 6,
        },
        
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.TurtleAndFish,
            Name = "바다거북 스프",
            Desc = "\"죄송합니다. 이거 정말로 바다거북 수프인가요\"\n\"네, 틀림없는 바다거북 수프 맞습니다\"",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Turtle, 1},
                {InventoryItemType.Fish, 2},
                {InventoryItemType.Plate, 1},
            },
            OpenWave = 8,
        },
        
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.VeganSet,
            Name = "야채샐러드",
            Desc = "채식을 하는 당신을 위한 메뉴.",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Carrot, 1},
                {InventoryItemType.Apple, 1},
                {InventoryItemType.Corn, 1},
                {InventoryItemType.Log, 1},
                {InventoryItemType.Plate, 1},
            },
            OpenWave = 10,
        },
        
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.BergurSet,
            Name = "햄버거 세트",
            Desc = "항상 보장된 맛.",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Cow, 1},
                {InventoryItemType.Corn, 3},
                {InventoryItemType.Cola, 1},
                {InventoryItemType.Plate, 1},
            },
            OpenWave = 12,
        },
        
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.SharkRamen,
            Name = "샥스핀 라멘",
            Desc = "고급스러워 보인다.\n콜라는 덤입니다.",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Fish_Shark, 1},
                {InventoryItemType.Corn, 3},
                {InventoryItemType.Pig, 1},
                {InventoryItemType.Cola, 1},
                {InventoryItemType.Plate, 1},
            },
            OpenWave = 14,
        },
    };

    public static CraftRecipe GetRecipe(InventoryItemType type)
    {
        var find = CraftRecipes.FirstOrDefault(x => x.ResultItem == type);
        
        if (find == null)
        {
            find = CookRecipes.FirstOrDefault(x => x.ResultItem == type);
        }
        
        return find;
    }

    public static CraftRecipe GetRandomCookRecipe()
    {
        var list = CookRecipes.Where(x => x.OpenWave <= Global.Instance.IngameManager.ServerOnlyGameManager.Wave).ToList();

        int randomIndex = Random.Range(0, list.Count);
        
        return list[randomIndex];
    }
    
    public static bool HasNewRecipes(int wave)
    {
        var newCraftRecipe = CraftRecipes.Where(x => x.OpenWave == wave).Any();
        var newCookRecipe = CookRecipes.Where(x => x.OpenWave == wave).Any();

        return newCraftRecipe || newCookRecipe;
    }
}