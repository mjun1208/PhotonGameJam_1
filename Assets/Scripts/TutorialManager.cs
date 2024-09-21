using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private ServerOnlyGameManager ServerOnlyGameManager;
    [SerializeField] private GameObject _tutorialObject;
    [SerializeField] private TMP_Text _tutorialText;
    
    // Global.Instance.IngameManager.ServerOnlyGameManager.TutorialManager.SetTutorialIndex(1);
    
    private List<string> _tutorialTextList = new List<string>()
    {
        "인벤토리 1번에 있는 도끼로 나무를 자르세요!", // Player.Tree - SpawnLog 1
        "떨어진 나무를 먹으세요!", // Player.Tree - TreeUpdate 2
        "E를 눌러 인벤토리를 열고, 제작창으로 가세요!", // InventoryUI.Craft - OnClickCraftTab 3
        "삽을 제작하세요!", // InventoryUI.Craft - Craft 4
        "삽을 이용하여 땅을 파고 옥수수를 심으세요!", // Player - RpcDoSomething 5
        "옥수수가 자라면 수확하세요!", // Player - FixedUpdateNetwork 6
        "모닥불을 제작하세요!", // InventoryUI.Craft - Craft 7
        "모닥불을 설치하여 옥수수 죽을 만드세요!", // InventoryUI.Craft - Craft 8
        "테이블을 제작하세요!", // InventoryUI.Craft - Craft 9
        "테이블을 설치하여 손님을 받으세요! (기다리면 손님이 옴)", // Npc - StartOrder 10
        "손님이 원하는 아이템을 건네세요!", // Npc - ReceiveItem 11
        "3초 후 1라운드가 시작됩니다!", // End Game Start
    };

    public void SetTutorialIndex(int index)
    {
        // 직전 Index만 적용
        if (ServerOnlyGameManager.TutorialIndex == index - 1)
        {
            ServerOnlyGameManager.TutorialIndex = index;
        }
    }

    public void OnChangedTutorialIndex(out bool isFinal)
    {
        if (_tutorialTextList.Count > ServerOnlyGameManager.TutorialIndex)
        {
            _tutorialText.text = _tutorialTextList[ServerOnlyGameManager.TutorialIndex];
            isFinal = false;

            if (_tutorialTextList.Count - 1 == ServerOnlyGameManager.TutorialIndex)
            {
                ServerOnlyGameManager.TutorialIndex++;
            }
        }
        else
        {
            isFinal = true;
        }
    }

    public void HideTutorialUI()
    {
        _tutorialObject.SetActive(false);
    }
}
