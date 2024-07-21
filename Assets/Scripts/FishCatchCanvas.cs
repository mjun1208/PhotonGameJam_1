using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FishCatchCanvas : MonoBehaviour
{
    [SerializeField] private RectTransform _fishTransform;
    [SerializeField] private RectTransform _catcherImage;
    [SerializeField] private Image _gaugeImage;
    [SerializeField] private GameObject _fx;
    [SerializeField] private Player _player;

    private readonly Vector2 offset = new Vector2(300f, 200f);
    private Vector2 _newFishTarget;
    private float _newTargetCoolTime;
    private float _fishSpeed;
    private float _catchGauge = 0f;
    private float _catchSpeed = 1f;

    private bool _catchComplete = false;
    
    private Vector2 _catcherPosition = Vector2.zero;

    public void StartGOGO()
    {
        _fishTransform.anchoredPosition = new Vector2(0, 0);
        _catcherPosition = Vector2.zero;
        _gaugeImage.fillAmount = 0f;
        _catchGauge = 0f;
        _catchSpeed = 1f;
        _newTargetCoolTime = 0f;
        _catchComplete = false;
        
        _catchSpeed = Random.Range(0.3f, 0.5f);
    }

    private void Update()
    {
        _newTargetCoolTime -= Time.deltaTime;
        
        if (_newTargetCoolTime <= 0f)
        {
            _newFishTarget = new Vector2(Random.Range(-offset.x, offset.x), Random.Range(-offset.y, offset.y));
            _newTargetCoolTime = Random.Range(1.5f, 3f);
            _fishSpeed = Random.Range(0.05f, 0.2f);
        }

        _fishTransform.anchoredPosition = Vector2.Lerp(_fishTransform.anchoredPosition, _newFishTarget, _fishSpeed);

        Catcher();
        if (IsAABBColliding(_fishTransform, _catcherImage))
        {
            _catchGauge += Time.deltaTime * _catchSpeed;
            _gaugeImage.fillAmount = _catchGauge;
            _fx.SetActive(true);
            _fx.transform.position = _catcherImage.transform.position;

            if (_gaugeImage.fillAmount >= 1f)
            {
                if (!_catchComplete)
                {
                    _catchComplete = true;
                    _player.CatchComplete();
                }
            }
        }
        else
        {
            _fx.SetActive(false);
        }
    }

    private void Catcher()
    {
        _catcherPosition += new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * 3f;
        _catcherPosition = MousePositionClamp(_catcherPosition);

        _catcherImage.anchoredPosition = _catcherPosition;
    }

    private Vector2 MousePositionClamp(Vector2 posss)
    {
        posss.x = Mathf.Clamp(posss.x, -offset.x, offset.x);
        posss.y = Mathf.Clamp(posss.y, -offset.y, offset.y);

        return posss;
    }

    // 충돌 췍
    bool IsAABBColliding(RectTransform rect1, RectTransform rect2)
    {
        Rect rect1World = GetWorldRect(rect1);
        Rect rect2World = GetWorldRect(rect2);

        return rect1World.Overlaps(rect2World);
    }

    Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector3 bottomLeft = corners[0];
        Vector3 topRight = corners[2];

        return new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
    }
}
