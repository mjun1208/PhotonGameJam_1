using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnlockUI : MonoBehaviour
{
    [SerializeField] private Transform _unlockItmemParent;
    [SerializeField] private UnlockNewItem _origin;
    
    private List<UnlockNewItem> _newRecipeListItems = new List<UnlockNewItem>();

    public void SetNewRecipe(int wave)
    {
        _newRecipeListItems.ForEach(x=> x.gameObject.SetActive(false));
        
        var newRecipeList = CraftRecipeManager.CraftRecipes.Where(x => x.OpenWave == wave).ToList();

        if (newRecipeList.Any())
        {
            for (int i = 0; i < newRecipeList.Count; i++)
            {
                if (i >= newRecipeList.Count - 1)
                {
                    var newRecipeListItem = Instantiate(_origin, _unlockItmemParent);
                    _newRecipeListItems.Add(newRecipeListItem);
                }

                _newRecipeListItems[i].SetInfo(newRecipeList[i], false);
                _newRecipeListItems[i].gameObject.SetActive(true);
            }
        }
        
        // 
        
        var newCookList = CraftRecipeManager.CookRecipes.Where(x => x.OpenWave == wave).ToList();

        if (newCookList.Any())
        {
            for (int i = 0; i < newCookList.Count; i++)
            {
                if (i >= newCookList.Count - 1)
                {
                    var newRecipeListItem = Instantiate(_origin, _unlockItmemParent);
                    _newRecipeListItems.Add(newRecipeListItem);
                }

                _newRecipeListItems[i].SetInfo(newCookList[i], true);
                _newRecipeListItems[i].gameObject.SetActive(true);
            }
        }
    }
}
