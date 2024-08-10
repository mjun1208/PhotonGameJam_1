using Photon.Voice.Fusion.Demo;
using UnityEngine;
using Object = UnityEngine.Object;

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

    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public string MyName { get; set; }
    public string RoomName { get; set; }
    
    public BasicSpawner BasicSpawner { get; set; }

    public GameObject Selecter;
    public SelectCanvas SelectCanvas;
    public LobbyCanvas LobbyCanvas;

    public PrefabSpawner PrefabSpawner { get; set; }
    public ChatManager ChatManager { get; set; }

    public Color FarmerColor = new Color(0f, 32f / 255f, 115f / 255f);
    public Color FisherColor = new Color(0f, 32f / 255f, 115f / 255f);

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
        ChatManager.SetName(MyName);
    }
}
