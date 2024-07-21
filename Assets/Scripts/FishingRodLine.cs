using UnityEngine;

public class FishingRodLine : MonoBehaviour
{
    public Transform point0, point1;
    public Vector3 FinalPosition;
    public int numPoints = 50;
    public float curveHeight = 2.0f; // 곡선의 높이를 조절하는 변수
    private LineRenderer lineRenderer;
    private Vector3[] positions;

    private Vector3 lastPo;

    private bool _isInit = false;
    
    private bool _gofinal = false;

    void Init()
    {
        // LineRenderer 컴포넌트를 가져옴
        lineRenderer = GetComponent<LineRenderer>();

        // positions 배열을 초기화
        positions = new Vector3[numPoints];
        lineRenderer.positionCount = 2;
    }

    public void SetFinalPosition(Vector3 possss)
    {
        FinalPosition = possss;
        _gofinal = false;
    }

    public void DrawBezierCurve()
    {
        if (!_isInit)
        {
            Init();
            _isInit = true; 
        }

        // point0과 point1의 위치를 가져옴
        Vector3 p0 = point0.position;
        Vector3 p1 = point1.position;

        if (_gofinal)
        {
            lastPo = Vector3.Lerp(lastPo, FinalPosition, 0.5f);
        }
        else
        {
            lastPo = p1;
        }

        p1 = lastPo;
        
        // // point0과 point1의 상대적인 위치에 따라 제어점 방향 결정
        // Vector3 midPoint = (p0 + p1) / 2;
        // Vector3 direction = (p1 - p0).normalized;
        // Vector3 perpendicular;
        //
        // // 위아래로 배치된 경우
        // perpendicular = Vector3.right; // 왼쪽으로 설정
        //
        // // p0와 p1 사이의 거리의 일정 비율만큼 아래로 내림
        // Vector3 p2 = midPoint + perpendicular * curveHeight;
        //
        // // Bezier 곡선을 계산하여 positions 배열에 저장
        // for (int i = 0; i < numPoints; i++)
        // {
        //     float t = i / (numPoints - 1f);
        //     positions[i] = CalculateBezierPoint(t, p0, p2, p1);
        // }
        //
        // // LineRenderer에 positions 배열을 설정
        // lineRenderer.SetPositions(positions);

        lineRenderer.SetPositions(new[] {p0, p1});
    }

    // Bezier 곡선의 점을 계산하는 메서드
    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0; // (1-t)^2 * P0
        p += 2 * u * t * p1; // 2(1-t)t * P1
        p += tt * p2; // t^2 * P2

        return p;
    }

    public void GOFinalPosition()
    {
        _gofinal = true;
    }
}
