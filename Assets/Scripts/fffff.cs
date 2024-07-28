using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class fffff : MonoBehaviour
{
    public GameObject _spawnWavePrefabFx;
    private float angle = 0;
    
    private void Start()
    {
        GOGO();
    }

    private async void GOGO()
    {
        for (int i = 0; i < 360; i+= 10)
        {
            var anglecvv = new Vector3(0, i, 0);
            Quaternion waveRotation = Quaternion.Euler(anglecvv);
        
            // 각도를 라디안으로 변환
            var angle_radians = math.radians(i);
        
            // 새로운 위치 계산
            var wow_x = 10 * math.cos(angle_radians);
            var wow_y = 10 * math.sin(angle_radians);

            var wavPos = this.transform.position + new Vector3(wow_x, 0, -wow_y);

            var wave = Instantiate(_spawnWavePrefabFx, wavPos, quaternion.identity);
            var rot = wave.transform.rotation;

            wave.transform.rotation = Quaternion.Euler(new Vector3(rot.eulerAngles.x, i, rot.eulerAngles.z));

            // angle++;   
        }
        
        await UniTask.Delay(1000);

        GOGO();
    }
}
