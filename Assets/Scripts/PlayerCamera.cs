using Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public CinemachineVirtualCamera CinemachineCamera;
    public Camera Camera;

    public void SetSolo()
    {
        CinemachineBrain.SoloCamera = CinemachineCamera;
    }
}
