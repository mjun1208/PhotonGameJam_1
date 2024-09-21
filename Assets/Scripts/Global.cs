using System;
using Photon.Voice.Fusion.Demo;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameResult
{
    public int Wave;
    public bool Success;
    public int Gold;
    public string WhoNpcGive;
    public string WhoWalk;
    public string WhoJump;
    public string WhoTreeCut;
    public string WhoCraft;
    public string WhoCook;
    public string WhoFishing;
}

public class Global : MonoBehaviour
{
    public static Global Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindObjectOfType<Global>();
            }
            
            return _instance;
        }
    }

    private static Global _instance = null;

    private void OnDestroy()
    {
        _instance = null;
    }

    public void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }
        
        DontDestroyOnLoad(this);

        Application.targetFrameRate = 60;
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public string MyName { get; set; }
    public string RoomName { get; set; }
    
    public BasicSpawner BasicSpawner { get; set; }

    public GameObject Selecter;
    // public SelectCanvas SelectCanvas;
    public LobbyCanvas LobbyCanvas;

    public PrefabSpawner PrefabSpawner { get; set; }
    public ChatManager ChatManager { get; set; }
    public IngameManager IngameManager { get; set; }
    
    public KingDino KingDino { get; set; }

    public Color FarmerColor = new Color(0f, 115f / 255f, 25f / 255f);
    public Color FisherColor = new Color(0f, 32f / 255f, 115f / 255f);

    public bool IngameActivingCursor = true;

    public Player MyPlayer = null;

    public GameResult LastGameResult = null;

    public void SetBasicSpawner(BasicSpawner basicSpawner)
    {
        if (BasicSpawner != null)
        {
            Destroy(BasicSpawner.gameObject);
            BasicSpawner = null;
        }
        
        BasicSpawner = basicSpawner;
        BasicSpawner.transform.SetParent(this.transform);

        RoomName = BasicSpawner.RoomName;
    }

    public void SetChatManager(ChatManager chatManager)
    {
        if (ChatManager != null)
        {
            Destroy(ChatManager.gameObject);
            ChatManager = null;
        }

        ChatManager = chatManager;
        ChatManager.SetName(RoomName,MyName);
    }

    [SerializeField] private GameObject FailCanvas;
    public GameObject MenuCanvas;

    public void RoomEnterFail()
    {
        FailCanvas.SetActive(true);
    }
    
    public void RoomDisconnected()
    {
        FailCanvas.SetActive(true);
    }
    
    public void CloseRoomEnterFailPopup()
    {
        FailCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (MyPlayer != null && MyPlayer.IsInvenOpen() && !MenuCanvas.activeSelf)
            {
                return;
            }
            
            if (MyPlayer != null && MyPlayer.IsChestOpen() && !MenuCanvas.activeSelf)
            {
                return;
            }

            MenuCanvas.SetActive(!MenuCanvas.activeSelf);

            if (!MenuCanvas.activeSelf)
            {
                if (!IngameActivingCursor)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    public void Continue()
    {
        MenuCanvas.SetActive(false);
        
        if (!IngameActivingCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
