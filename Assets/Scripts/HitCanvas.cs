using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HitCanvas : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _hpGauge;
    [SerializeField] private GameObject _deadImage;
    [SerializeField] private TMP_Text _respawnTime;
    
    [SerializeField] private GameObject _mp;
    [SerializeField] private Image _mpGauge;
        
    private CancellationTokenSource _clt;

    public void Hitted(int leftHp)
    {
        if (_clt != null)
        {
            _clt.Cancel();
            _clt = null;
        }

        _clt = new CancellationTokenSource();

        DoAnime(_clt.Token);

        if (leftHp <= 0)
        {
            _hpGauge.fillAmount = 0f;
        }
        else
        {
            _hpGauge.fillAmount = leftHp / 1000f;
        }
    }

    public async void Dead()
    {
        _deadImage.SetActive(true);
        
        for (int i = 3; i >= 1; i--)
        {
            _respawnTime.text = $"{i}초 뒤 부활";
            await UniTask.Delay(1000);
        }
        
        _deadImage.SetActive(false);
    }

    public void Respawn()
    {
        _hpGauge.fillAmount = 1f;
    }

    private async void DoAnime(CancellationToken cancellationToken)
    {
        _canvasGroup.alpha = 1f;
        
        _canvasGroup.DOFade(0, 1f).WithCancellation(cancellationToken);
    }

    public void ShowMP(int mp)
    {
        _mp.SetActive(true);
        SetMp(mp);
    }

    public void HideMp()
    {
        _mp.SetActive(false);
    }

    public void SetMp(int mp)
    {
        if (mp <= 0)
        {
            _mpGauge.fillAmount = 0f;
        }
        else
        {
            _mpGauge.fillAmount = mp / 15f;
        }
    }
}
