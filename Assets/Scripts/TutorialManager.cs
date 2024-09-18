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
    
    private List<string> _tutorialTextList = new List<string>()
    {
        "인벤토리 1번에 있는 도끼로 나무를 자르세요!",
        "떨어진 나무를 먹으세요!",
        "E를 눌러 인벤토리를 열고, 제작창으로 가세요!",
        "삽을 제작하세요!",
        "삽을 이용하여 땅을 파고 옥수수를 심으세요!",
        "옥수수가 자라면 수확하세요!",
        "모닥불을 제작하세요!",
        "모닥불을 설치하여 옥수수 죽을 만드세요!",
        "테이블을 제작하세요!",
        "테이블을 설치하여 손님을 받으세요!",
        "손님에게 옥수수 죽을 건네세요!",
        "3초 후 1라운드가 시작됩니다!",
    };

    public void OnChangedTutorialIndex()
    {
        if (_tutorialTextList.Count > ServerOnlyGameManager.TutorialIndex)
        {
            _tutorialText.text = _tutorialTextList[ServerOnlyGameManager.TutorialIndex];
        }
    }
}
