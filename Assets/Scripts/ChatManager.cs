using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Chat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    [SerializeField] private string appId = "4f81776a-3f71-466c-ad32-0a0e0178840e";
    [SerializeField] private TMP_InputField _inputField;
    
    [SerializeField] private Transform _chatListTransform;
    [SerializeField] private ChatItem _chatItem;
    
    [SerializeField] private List<Image> _uiList;
    [SerializeField] private GameObject _inputGameObject;

    public bool IsFocus { get; set; } = false;

    public string UserName { get; set; } = "name";
    public string Server { get; set; } = "Dummy";

    private ChatClient _chatClient;

    private void Awake()
    {
        Global.Instance.SetChatManager(this);
    }

    public void SetName(string serverName, string userName)
    {
        Server = serverName;
        UserName = userName;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _chatClient = new ChatClient(this);
        // Set your favourite region. "EU", "US", and "ASIA" are currently supported.
        _chatClient.ChatRegion = "ASIA";
        _chatClient.Connect(this.appId, "1.0", new Photon.Chat.AuthenticationValues(UserName));
        
        _uiList.ForEach(x=> x.enabled = false);
        _inputGameObject.SetActive(false);
    }

    private void Update()
    {
        _chatClient.Service();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (IsFocus)
            {
                if (!string.IsNullOrWhiteSpace(_inputField.text))
                {
                    _chatClient.PublishMessage(Server, _inputField.text);
                    _inputField.text = string.Empty;
                }

                _uiList.ForEach(x=> x.enabled = false);
                
                _inputGameObject.SetActive(false);
            }
            else
            {
                _uiList.ForEach(x=> x.enabled = true);
                
                _inputGameObject.SetActive(true);
                
                _inputField.ActivateInputField();
            }

            IsFocus = !IsFocus;
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {
    }

    public void OnDisconnected()
    {
    }

    public void OnConnected()
    {
        ServerSubscribe();
    }

    private async void ServerSubscribe()
    {
        await UniTask.WaitWhile(() => string.IsNullOrWhiteSpace(Server));
        _chatClient.Subscribe(new string[] {Server});
    }

    public void OnChatStateChange(ChatState state)
    {
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            var chatItem = Instantiate(_chatItem, _chatListTransform);
            chatItem.Set(senders[i], (string)messages[i]);
            chatItem.gameObject.SetActive(true);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        var chatItem = Instantiate(_chatItem, _chatListTransform);
        chatItem.Set("SERVER", "엔터를 눌러서 채팅을 치세요");
        chatItem.gameObject.SetActive(true);
        
        _chatClient.PublishMessage(Server, $"{UserName}님이 들어오셨습니다");
    }

    public void OnUnsubscribed(string[] channels)
    {
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }
}
