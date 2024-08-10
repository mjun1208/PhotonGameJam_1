using DG.Tweening;
using Fusion;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SelectCanvas : MonoBehaviour
{
    [SerializeField] private GameObject _farmerObject;
    [SerializeField] private GameObject _fisherObject;
    [SerializeField] private Image _farmerButton;
    [SerializeField] private Image _fisherButton;
    
    [SerializeField] private GameObject _farmerInfo;
    [SerializeField] private GameObject _fisherInfo;
    
    [SerializeField] private Camera _selectCamera;
    
    private PlayerType _selectedPlayerType = PlayerType.Farmer;
    private bool _selected = false;

    private LobbyPlayerInfo _lobbyPlayerInfo;

    public PlayerType SelectedPlayerType => _selectedPlayerType;

    public void SetLobbyPlayerInfo(LobbyPlayerInfo lobbyPlayerInfo)
    {
        _lobbyPlayerInfo = lobbyPlayerInfo;

        var mainCamera = Camera.main.GetUniversalAdditionalCameraData();
        mainCamera.cameraStack.Add(_selectCamera);
    }
    
    public void SelectFarmer()
    {
        if (_selected)
        {
            Cancel();
            return;
        }
        
        _selectedPlayerType = PlayerType.Farmer;

        _farmerButton.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
        _fisherObject.SetActive(false);
        _farmerInfo.SetActive(true);

        _selected = true;
    }
    
    public void SelectFisher()
    {
        if (_selected)
        {
            Cancel();
            return;
        }
        
        _selectedPlayerType = PlayerType.Fisher;
        
        _fisherButton.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
        _farmerObject.SetActive(false);
        _fisherInfo.SetActive(true);

        _selected = true;
    }

    public void Confirm()
    {
        Global.Instance.LobbyCanvas.gameObject.SetActive(true);
        Global.Instance.LobbyCanvas.TypeSelected(_selectedPlayerType);
        Global.Instance.Selecter.SetActive(false);
    }

    public void Cancel()
    {
        _farmerButton.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f);
        _fisherButton.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f);
        
        _farmerObject.SetActive(true);
        _fisherObject.SetActive(true);
        
        _farmerInfo.SetActive(false);
        _fisherInfo.SetActive(false);

        _selected = false;
    }
}
