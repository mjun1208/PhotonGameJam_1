using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class HitCanvas : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
        
    private CancellationTokenSource _clt;
    
    public void Hitted()
    {
        if (_clt != null)
        {
            _clt.Cancel();
            _clt = null;
        }

        _clt = new CancellationTokenSource();
        
        DoAnime(_clt.Token);
    }

    private async void DoAnime(CancellationToken cancellationToken)
    {
        _canvasGroup.alpha = 1f;
        
        _canvasGroup.DOFade(0, 1f).WithCancellation(cancellationToken);
    }
}
