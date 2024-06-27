using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private Ball _prefabBall;
    [SerializeField] private Transform _playerCameraRootTransform;

    private Vector3 _forward = Vector3.forward;

    private NetworkCharacterController _cc;
    private PlayerCamera _playerCamera;
    private Vector3 _cameraRotation;
    [Networked] private TickTimer delay { get; set; }

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        _playerCamera = GameObject.Find("PlayerFollowCamera").GetComponent<PlayerCamera>();
        _playerCamera.CinemachineCamera.Follow = this._playerCameraRootTransform;
        _playerCamera.transform.position = this._playerCameraRootTransform.position;
    }

    private void FixedUpdate()
    {
        Look(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData inputData))
        {
            if (inputData.direction.sqrMagnitude > 0)
            {
                Move(inputData);
                _forward = inputData.direction;
            }

            // 슛.
            // if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
            // {
            //     if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
            //     {
            //         delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                       // Runner.Spawn(_prefabBall, transform.position + _forward, Quaternion.LookRotation(_forward), Object.InputAuthority);
                       //         
                       //         Runner.Spawn(_prefabBall,
                       //             transform.position+_forward, Quaternion.LookRotation(_forward),
                       //             Object.InputAuthority, (runner, o) =>
                       //             {
                       //                 // Initialize the Ball before synchronizing it
                       //                 o.GetComponent<Ball>().Init();
                       //             });
                       //     }
            // }
        }
    }

    private void Move(NetworkInputData networkInputData)
    {
        var inputDir = networkInputData.direction.normalized;

        float angle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg;
        var moveDir = Quaternion.Euler(0, -_cameraRotation.y + angle, 0) * Vector3.forward;
        // float moveSpeed = _isSprint ? SprintSpeed : MoveSpeed;
           
        Debug.Log(moveDir);
        // this.transform.position += 5 * moveDir * Runner.DeltaTime;
        _cc.Move(5 * moveDir * Runner.DeltaTime);

        // _rigidbody.AddForce(dir * moveSpeed, ForceMode.Force);
        // velocity.x = dir.x * MoveSpeed;
        // velocity.z = dir.z * MoveSpeed;
    }
    
    /// <summary>
    /// Camera Ratation
    /// </summary>
    private void Look(Vector2 input)
    {
        if (input == Vector2.zero)
        {
            return;
        }
        
        _cameraRotation += new Vector3(-input.y * 0.8f, -input.x * 5f, 0);
        _cameraRotation = CameraRotationClamp(_cameraRotation);
        _playerCameraRootTransform.transform.rotation = Quaternion.Euler(_cameraRotation.x, -_cameraRotation.y, 0);
        
        // _modelTransform.rotation = Quaternion.Euler(0, -_cameraRotation.y, 0);
    }
    
    private Vector3 CameraRotationClamp(Vector3 rotation)
    {
        if (rotation.y > 360)
        {
            rotation.y %= 360;
        }
        
        if (rotation.y < 0)
        {
            rotation.y %= 360;
        }
        
        rotation.y = Mathf.Clamp(rotation.y, float.MinValue, float.MaxValue);
        rotation.x = Mathf.Clamp(rotation.x, -80f, 60f);

        return rotation;
    }
}