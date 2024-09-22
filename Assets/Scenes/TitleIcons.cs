using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TitleIcons : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private List<Sprite> _itemIcons;

    private Vector2 _dir;
    private float _moveSpeed;
    private bool rotDir = false;
    
    public float bounceOffsetMin = 5f; // 경계에서 꺾일 최소 값
    public float bounceOffsetMax = 20f; // 경계에서 꺾일 최대 값

    private void Start()
    {
        _dir = GetDirectionFromAngle(Random.Range(0, 360));
        Set();
    }

    public void Set()
    {
        _image.sprite = _itemIcons[Random.Range(0, _itemIcons.Count)];
        _moveSpeed = Random.Range(100f, 200f);
        _dir = -_dir;
        
        // 꺾인 방향으로 랜덤 값 추가
        // _bounceOffset = Random.Range(bounceOffsetMin, bounceOffsetMax);
        rotDir = Random.Range(0, 2) == 0;
    }

    private void Update()
    {
        _rectTransform.anchoredPosition += (_dir * _moveSpeed * Time.deltaTime);
        _rectTransform.Rotate(new Vector3(0, 0, rotDir ? 2 : -2));
        
        if (IsUIElementOutOfCanvas(_rectTransform))
        {
            Set();
        }
    }

    Vector3 GetDirectionFromAngle(float angle)
    {
        // 각도를 라디안으로 변환
        float radians = angle * Mathf.Deg2Rad;

        // 방향 벡터 계산
        float x = Mathf.Cos(radians);
        float z = Mathf.Sin(radians);

        return new Vector3(x, 0, z).normalized; // 정규화된 방향 벡터 반환
    }
    
    bool IsUIElementOutOfCanvas(RectTransform rectTransform)
    {
        // 스크린 크기
        float screenWidth = Screen.width + 1000;
        float screenHeight = Screen.height + 1000;

        if (rectTransform.anchoredPosition.x < -screenWidth / 2 || rectTransform.anchoredPosition.x > screenWidth / 2)
        {
            return true;
        }
        
        if (rectTransform.anchoredPosition.y < -screenHeight / 2 || rectTransform.anchoredPosition.y > screenHeight / 2)
        {
            return true;
        }

        return false; // UI 요소가 스크린 안에 있음
    }
}
