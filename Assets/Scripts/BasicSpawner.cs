using System;
using Cysharp.Threading.Tasks;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class BasicSpawner : MonoBehaviour 
{
    [SerializeField]
    private NetworkRunner _runnerPrefab;
    private NetworkRunner _runner;
    
    [SerializeField]
    private GameObject _hostInfo;
    
    [SerializeField]
    private GameObject _clientInfo;
    
    [SerializeField]
    private GameObject _clickBlock;
    
    [SerializeField]
    private TMP_InputField _hostInput;
    [SerializeField]
    private TMP_InputField _clientNameInput;
    [SerializeField]
    private TMP_InputField _clientRoomInput;
    
    [SerializeField]
    private GameObject _loading;

    public string RoomName { get; set; }

    async void StartGame(GameMode mode)
    {
        _loading.SetActive(true);
        
        Global.Instance.SetBasicSpawner(this);
        await SceneManager.LoadSceneAsync("SampleScene2");
        
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = Instantiate(_runnerPrefab);
        _runner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = RoomName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 4,
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            LockRoom(_runner);
        }
    }

    public void LockRoom(NetworkRunner runner)
    {
        if (runner.IsRunning)
        {
            // 방의 속성을 업데이트하여 방을 잠그기
            runner.SessionInfo.IsOpen = false;
        }
    }

    public void OpenHostInfo()
    {
        _clickBlock.SetActive(true);
        _hostInfo.SetActive(true);
    }
    
    public void OpenClientInfo()
    {
        _clickBlock.SetActive(true);
        _clientInfo.SetActive(true);
    }
    
    public void ClosePopup()
    {
        _clickBlock.SetActive(false);
        
        _hostInfo.SetActive(false);
        _clientInfo.SetActive(false);
    }

    public void SetHostInfo()
    {
        Global.Instance.MyName = _hostInput.text;
        RoomName = GetRandomString();

        StartHost();
    }
    
    public void SetClientInfo()
    {
        Global.Instance.MyName = _clientNameInput.text;
        RoomName = _clientRoomInput.text;

        StartClient();
    }

    public void StartHost()
    {
        StartGame(GameMode.Host);
    }

    public void StartClient()
    {
        StartGame(GameMode.Client);
    }

    private const int ASCII_A = 65;  // 'A'의 아스키 코드 값
    private const int ASCII_Z = 90;  // 'Z'의 아스키 코드 값

    // 5글자 랜덤 문자열을 반환하는 함수 (대문자만)
    private string GetRandomString()
    {
        char[] stringChars = new char[5];
        for (int i = 0; i < stringChars.Length; i++)
        {
            // 'A'에서 'Z' 사이의 랜덤한 아스키 코드 값을 생성하고 문자로 변환
            stringChars[i] = (char)Random.Range(ASCII_A, ASCII_Z + 1);
        }
        return new string(stringChars);
    }
}
