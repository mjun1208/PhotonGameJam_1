using System;
using ExitGames.Client.Photon;
using Photon.Chat;
using UnityEngine;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    [SerializeField] private string appId = "4f81776a-3f71-466c-ad32-0a0e0178840e";
    
    public string UserName { get; set; }

    private ChatClient _chatClient;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _chatClient = new ChatClient(this);
        // Set your favourite region. "EU", "US", and "ASIA" are currently supported.
        _chatClient.ChatRegion = "ASIA";
        _chatClient.Connect(this.appId, "1.0", null);
    }

    private void Update()
    {
        _chatClient.Service();
    }

    public void DebugReturn(DebugLevel level, string message)
    {
    }

    public void OnDisconnected()
    {
    }

    public void OnConnected()
    {
        Debug.Log("Cow");
        
        _chatClient.Subscribe( new string[] { "channelA", "channelB" } );
    }

    public void OnChatStateChange(ChatState state)
    {
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string msgs = "";
        for (int i = 0; i < senders.Length; i++)
        {
            msgs += senders[i] + "=" + messages[i] + ", ";
        }

        Debug.Log("OnGetMessages: " + channelName + "(" + senders.Length + ") > " + msgs);
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        _chatClient.PublishMessage( "channelA", "So Long, and Thanks for All the Fish!" );
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
