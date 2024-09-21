using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
    [SerializeField] private RectTransform _endTextObject;
    [SerializeField] private GameObject _successText;
    [SerializeField] private GameObject _failText;
    [SerializeField] private RectTransform _board;
    [SerializeField] private TMP_Text  _waveText;
    [SerializeField] private GameObject _confirmButton;
    
    [SerializeField] private GameObject _tutorialText;
    
    private void Start()
    {
        CheckErrorHelper.CheckError = false;
        
        End();

        CinemachineBrain.SoloCamera = null;
        
        Global.Instance.IngameActivingCursor = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private async void End()
    {
        if (Global.Instance.LastGameResult.Success)
        {
            _successText.gameObject.SetActive(true);
        }
        else
        {
            _failText.gameObject.SetActive(true);
        }

        await UniTask.Delay(500);

        _endTextObject.DOScale(new Vector3(0.45f, 0.45f, 0.45f), 1f);
        await _endTextObject.DOAnchorPos(new Vector2(0, 281), 1f);

        _board.gameObject.SetActive(true);
        await _board.DOAnchorPos(new Vector2(0, 0), 1f);
        
        await UniTask.Delay(1000);

        _waveText.text = $"라운드 : {Global.Instance.LastGameResult.Wave}";
        _waveText.gameObject.SetActive(true);

        if (Global.Instance.LastGameResult.Wave == 0)
        {
            await UniTask.Delay(1000);
            _tutorialText.gameObject.SetActive(true);
        }
            
        await UniTask.Delay(1000);
        
        _confirmButton.gameObject.SetActive(true);
    }
    
    public void SceneChange()
    {
        if (Global.Instance.PrefabSpawner != null)
        {
            Global.Instance.PrefabSpawner.GetComponent<NetworkRunner>().Shutdown();
        }
        
        CheckErrorHelper.CheckError = true;
        
        SceneManager.LoadScene("Title");
    }
}
