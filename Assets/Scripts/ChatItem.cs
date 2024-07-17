using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ChatItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private string _name;
    private string _message;

    public void Set(string name, string message)
    {
        _name = name;
        _message = message;

        _text.text = $"{name} : {message}";
        DestroySelf();
    }

    private async void DestroySelf()
    {
        await UniTask.Delay(30000);
        await _text.DOFade(0f, 1f);
        
        Destroy(this.gameObject);
    }
}
