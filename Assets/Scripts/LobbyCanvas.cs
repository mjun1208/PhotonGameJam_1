using System;
using TMPro;
using UnityEngine;

public class LobbyCanvas : MonoBehaviour
{
    [SerializeField] private GameObject _readyButton;
    [SerializeField] private GameObject _readyCancelButton;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _mustEveryReady;
    [SerializeField] private TMP_Text _codeText;
    public Transform PlayerListTransform;

    private bool _ready = false;
    private bool _host = false;
    
    private LobbyPlayerInfo _lobbyPlayerInfo;

    public void OnEnable()
    {
        if (_host)
        {
            _readyButton.SetActive(false);
            _readyCancelButton.SetActive(false);
        }
        else
        {
            _readyButton.SetActive(true);
            _readyCancelButton.SetActive(false);
        }

        _lobbyPlayerInfo.SetReady(false);
        
        _mustEveryReady.SetActive(false);
        _startButton.SetActive(false);
    }

    public void SetLobbyPlayerInfo(LobbyPlayerInfo lobbyPlayerInfo, bool host)
    {
        _lobbyPlayerInfo = lobbyPlayerInfo;
        _host = host;
        
        if (_host)
        {
            _readyButton.SetActive(false);
            _readyCancelButton.SetActive(false);
        }
        else
        {
            _readyButton.SetActive(true);
        }

        _codeText.text = $"방 코드 : {Global.Instance.RoomName}";
    }
    
    public void Ready()
    {
        _ready = !_ready;
        
        _readyButton.SetActive(!_ready);
        _readyCancelButton.SetActive(_ready);
        
        _lobbyPlayerInfo.SetReady(_ready);
    }

    public void CheckReady()
    {
        int childCount = PlayerListTransform.childCount;
        bool everyReady = true;

        for (int i = 0; i < childCount; i++)
        {
            var lobbyPlayerInfo = PlayerListTransform.GetChild(i).GetComponent<LobbyPlayerInfo>();

            if (!lobbyPlayerInfo._readied && !lobbyPlayerInfo._roomMaster)
            {
                everyReady = false;
                break;
            }
        }

        _mustEveryReady.SetActive(!everyReady);
        _startButton.SetActive(everyReady);
    }

    public void ClickStart()
    {
        Global.Instance.BasicSpawner.LockRoom();
        
        int childCount = PlayerListTransform.childCount;
        
        for (int i = 0; i < childCount; i++)
        {
            var lobbyPlayerInfo = PlayerListTransform.GetChild(i).GetComponent<LobbyPlayerInfo>();
            lobbyPlayerInfo.Spawn();
        }
    }

    public void TypeSelected(PlayerType playerType)
    {
        _lobbyPlayerInfo.SetPlayerType(playerType);
        _lobbyPlayerInfo.SetTypeSelected(true);
    }
}
