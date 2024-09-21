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
        for (int i = 0; i < _newRecipeListItems.Count; i++)
        {
            Destroy(_newRecipeListItems[i]);
        }

        _newRecipeListItems.Clear();

        var newRecipeList = CraftRecipeManager.CraftRecipes.Where(x => x.OpenWave == wave).ToList();

        if (newRecipeList.Any())
        {
            foreach (var craftRecipe in newRecipeList)
            {
                var newRecipeListItem = Instantiate(_origin, _unlockItmemParent);
                newRecipeListItem.SetInfo(craftRecipe, false); 
                newRecipeListItem .gameObject.SetActive(true);       
                
                _newRecipeListItems.Add(newRecipeListItem);
            }
        }
        
        // 
        
        var newCookList = CraftRecipeManager.CookRecipes.Where(x => x.OpenWave == wave).ToList();

        if (newCookList.Any())
        {
            foreach (var craftRecipe in newCookList)
            {
                var newRecipeListItem = Instantiate(_origin, _unlockItmemParent);
                newRecipeListItem.SetInfo(craftRecipe, false); 
                newRecipeListItem .gameObject.SetActive(true);       
                
                _newRecipeListItems.Add(newRecipeListItem);
            }
        }
    }
}
