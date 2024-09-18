using Fusion;
using TMPro;
using UnityEngine;

public class LobbyPlayerInfo : NetworkBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _typeText;
    [SerializeField] private GameObject _roomMasterText;
    [SerializeField] private GameObject _readyText;
    [SerializeField] private GameObject _notReadyText;

    [Networked, OnChangedRender(nameof(OnChangedMyName))] private string _myName { get; set; }
    [Networked, OnChangedRender(nameof(OnChangedPlayerType))] private PlayerType _playerType { get; set; }
    [Networked, OnChangedRender(nameof(OnChangedPlayerType))] private NetworkBool _playerTypeSelected { get; set; }
    [Networked, OnChangedRender(nameof(OnChangedReady))] public NetworkBool _readied { get; set; }
    [Networked, OnChangedRender(nameof(OnChangedReady))] public NetworkBool _roomMaster { get; set; } = false;

    private PlayerRef _playerRef;

    public void SetPlayerRef(PlayerRef playerRef)
    {
        _playerRef = playerRef;
    }
    
    public override void Spawned()
    {
        base.Spawned();

        OnChangedMyName();
        OnChangedPlayerType();
        OnChangedReady();
        
        this.transform.SetParent(Global.Instance.LobbyCanvas.PlayerListTransform);
        this.transform.localScale = Vector3.one;

        if (HasInputAuthority)
        {
            // Global.Instance.Selecter.SetActive(true);
            
            // var selectCanvas = Global.Instance.SelectCanvas;
            // selectCanvas.SetLobbyPlayerInfo(this);
            
            var lobbyCanvas = Global.Instance.LobbyCanvas;
            lobbyCanvas.SetLobbyPlayerInfo(this, Runner.IsServer);

            SetMyName(Global.Instance.MyName);
            SetRoomMaster(HasStateAuthority);
            
            Global.Instance.LobbyCanvas.gameObject.SetActive(true);
            Global.Instance.LobbyCanvas.TypeSelected(PlayerType.Farmer);

            // SetPlayerType(Global.Instance.SelectCanvas.SelectedPlayerType);
        }

        if (HasStateAuthority)
        {
            Global.Instance.LobbyCanvas.CheckReady();
        }
    }

    public void SetMyName(string myName)
    {
        RpcSetMyName(myName);
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcSetMyName(string myName)
    {
        _myName = myName;
        OnChangedMyName();
    }

    public void OnChangedMyName()
    {
        _nameText.text = _myName;
    }
    
    public void SetPlayerType(PlayerType playerType)
    {
        RpcSetPlayerType(playerType);
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcSetPlayerType(PlayerType playerType)
    {
        _playerType = playerType;
        OnChangedPlayerType();
    }

    public void OnChangedPlayerType()
    {
        if (!_playerTypeSelected)
        {
            _typeText.text = "미선택";
        }
        else
        {
            if (_playerType == PlayerType.Farmer)
            {
                _typeText.text = "농술사";
                _typeText.color = Global.Instance.FarmerColor;
            }   
            
            if (_playerType == PlayerType.Fisher)
            {
                _typeText.text = "수산술사";
                _typeText.color = Global.Instance.FisherColor;
            }   
        }
    }

    public void SetTypeSelected(bool select)
    {
        RpcSetTypeSelected(select);
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcSetTypeSelected(bool select)
    {
        _playerTypeSelected = select;
        OnChangedPlayerType();
    }

    public void SetReady(bool ready)
    {
        RpcSetReady(ready);
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcSetReady(bool ready)
    {
        _readied = ready;
        OnChangedReady();
        
        Global.Instance.LobbyCanvas.CheckReady();
    }

    public void OnChangedReady()
    {
        if (_roomMaster)
        {
            _notReadyText.SetActive(false);
            _readyText.SetActive(false);
            _roomMasterText.SetActive(true);
        }
        else
        {
            _roomMasterText.SetActive(false);
            
            if (_readied)
            {
                _notReadyText.SetActive(false);
                _readyText.SetActive(true);
            }
            else
            {
                _notReadyText.SetActive(true);
                _readyText.SetActive(false);
            }
        }
    }
    
    public void SetRoomMaster(bool master)
    {
        RpcSetMaster(master);
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcSetMaster(bool master)
    {
        _roomMaster = master;
        OnChangedReady();
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    public void RpcStart()
    {
        Global.Instance.LobbyCanvas.gameObject.SetActive(false);
    }
    //
    // public void SelectConfirm(PlayerType playerType)
    // {
    //     RpcSelectConfirm(Runner.LocalPlayer, playerType);
    // }
    //
    // [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    // public void RpcSelectConfirm(PlayerRef player, PlayerType playerType)
    // {
    //     if (HasStateAuthority)
    //     {
    //         Spawn(player, playerType);
    //     }
    // }

    public void Spawn()
    {
        RpcStart();
        Global.Instance.PrefabSpawner.SpawnPlayer(_playerRef, _playerType);
    }
}
