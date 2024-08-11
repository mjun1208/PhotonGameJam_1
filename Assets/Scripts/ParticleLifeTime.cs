using Cysharp.Threading.Tasks;
using UnityEngine;

public class ParticleLifeTime : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 1.5f;
    
    // Start is called before the first frame update
     async void Start()
     {
         await UniTask.Delay((int) (_lifeTime * 1000));

         if (this == null)
         {
             return;
         }
         
         Destroy(this.gameObject);
     }
}
