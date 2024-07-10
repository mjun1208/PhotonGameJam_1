using System;
using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private const float InteractionRayCastDistance = 3f;
    
    [SerializeField] private Dirt _dirt;
    [SerializeField] private GameObject _dirt_ghost;
    [SerializeField] private Ball _prefabBall;
    [SerializeField] private Transform _playerCameraRootTransform;
    [SerializeField] private Transform _modelTransform;
    [SerializeField] private Animator _animator;
    [SerializeField] private SimpleKCC _simpleKCC;

    private Vector3 _forward = Vector3.forward;
    
    private Vector3 _shootPosition = Vector3.zero;
    private bool _shootAble = false;

    private Dirt _plantTargetDirt = null;
    private bool _plantAble = false;
    
    [Networked]
    private Vector3 _networkedMoveDirection { get; set; }

    private PlayerCamera _playerCamera;
    private Vector3 _cameraRotation;
    [Networked] private TickTimer delay { get; set; }

    private void Awake()
    {
    }

    public override void Spawned()
    {
        base.Spawned();

        if (!HasInputAuthority)
        {
            return;
        }
        
        _playerCamera = GameObject.Find("PlayerFollowCamera").GetComponent<PlayerCamera>();
        _playerCamera.CinemachineCamera.Follow = this._playerCameraRootTransform;
        _playerCamera.transform.position = this._playerCameraRootTransform.position;
            
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        if (!HasInputAuthority)
        {
            return;
        }
    }

    public override void FixedUpdateNetwork()
    {
        var inputData = GetInput<NetworkInputData>().GetValueOrDefault();

        _simpleKCC.AddLookRotation(inputData.lookDelta);
            
        Look();

        _networkedMoveDirection = inputData.direction;
        
        if (inputData.direction.sqrMagnitude > 0)
        {
            Move(inputData);
            _forward = inputData.direction;
        }

        // 슛.

        if (HasInputAuthority)
        {
            if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
            {
                RpcDoSomething();
            }
        }

        if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
        {
            if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
            {
                if (_shootAble)
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_dirt, _shootPosition, Quaternion.LookRotation(_forward), Object.InputAuthority);
                }
            }
        }
        
        if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
        {
            if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
            {
                delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                Runner.Spawn(_prefabBall,
                    transform.position + _forward, Quaternion.LookRotation(_forward),
                    Object.InputAuthority, (runner, o) =>
                    {
                        // Initialize the Ball before synchronizing it
                        o.GetComponent<Ball>().Init();
                    });
            }
        }
    }

    public override void Render()
    {
        base.Render();
        SetAnimation();
        
        if (HasInputAuthority)
        {
            GetPlantTarget();
            ShootGOGO();
        }
    }

    private void SetAnimation()
    {
        if (_networkedMoveDirection.sqrMagnitude > 0)
        {
            _animator.SetBool("Move", true);
                
            var aniamtorMoveX = _animator.GetFloat("MoveX");
            var aniamtorMoveZ = _animator.GetFloat("MoveZ");

            var lerpAnimatorMoveX = Mathf.Lerp(aniamtorMoveX, _networkedMoveDirection.x, 10f * Time.fixedDeltaTime);
            var lerpAnimatorMoveZ = Mathf.Lerp(aniamtorMoveZ, _networkedMoveDirection.z, 10f * Time.fixedDeltaTime);

            _animator.SetFloat("MoveX", lerpAnimatorMoveX);
            _animator.SetFloat("MoveZ", lerpAnimatorMoveZ);
        }
        else
        {
            _animator.SetBool("Move", false);
        }
    }

    private void Move(NetworkInputData networkInputData)
    {
        var inputDir = networkInputData.direction.normalized;

        float angle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg;
        var moveDir = Quaternion.Euler(0, -_cameraRotation.y + angle, 0) * Vector3.forward;
        //  float moveSpeed = _isSprint ? SprintSpeed : MoveSpeed;
        
        Vector3 jumpImpulse  = default;
        
        if (networkInputData.jump == true && _simpleKCC.IsGrounded == true)
        {
            // Set world space jump vector.
            jumpImpulse = Vector3.up * 10.0f;
        }
        
        Vector3 inputDirection = _simpleKCC.TransformRotation * new Vector3(inputDir.x, 0f, inputDir.z);

        _simpleKCC.Move(inputDirection * 2f, jumpImpulse.magnitude);
    }
    
    /// <summary>
    /// Camera Ratation
    /// </summary>
    private void Look()
    {
        var input = _simpleKCC.GetLookRotation(true, true);
        
        _cameraRotation = new Vector3(input.x, input.y);
        _playerCameraRootTransform.transform.rotation = Quaternion.Euler(_cameraRotation);
        _modelTransform.rotation = Quaternion.Euler(0, _cameraRotation.y, 0);
    }

    private void GetPlantTarget()
    {
        LayerMask groundMask = 1 << LayerMask.NameToLayer("Dirt");
        
        if (Physics.Raycast(_playerCameraRootTransform.transform.position, _playerCameraRootTransform.transform.forward, out RaycastHit hit,
                InteractionRayCastDistance, groundMask))
        {
            _plantAble = true;

            var dirt = hit.transform.GetComponent<Dirt>();
            if (dirt != null)
            {
                if (_plantTargetDirt != null && dirt != _plantTargetDirt)
                {
                    _plantTargetDirt.Looking(false);
                    _plantTargetDirt = null;
                }

                _plantTargetDirt = dirt;
                _plantTargetDirt.Looking(true);
            }
        }
        else
        {
            _plantAble = false;

            if (_plantTargetDirt != null)
            {
                _plantTargetDirt.Looking(false);
                _plantTargetDirt = null;
            }
        }
    }

    private void ShootGOGO()
    {
        LayerMask groundMask = 1 << LayerMask.NameToLayer("Ground");
        
        Debug.DrawRay(_playerCameraRootTransform.transform.position, _playerCameraRootTransform.transform.forward * InteractionRayCastDistance, Color.red);

        if (Physics.Raycast(_playerCameraRootTransform.transform.position, _playerCameraRootTransform.transform.forward, out RaycastHit hit,
                InteractionRayCastDistance, groundMask) && !_plantAble)
        {
                    
            Debug.Log(_shootPosition);
            
            _shootPosition = hit.point;
            _shootAble = true;

            DirtRender();
        }
        else
        {
            _shootPosition = Vector3.zero;
            _shootAble = false;
            
            if (_dirt_ghost.activeSelf)
            {
                _dirt_ghost.SetActive(false);
            }
        }
    }

    private void DirtRender()
    {
        if (!HasInputAuthority)
        {
            return; 
        }
        
        if (!_dirt_ghost.activeSelf)
        {
            _dirt_ghost.SetActive(true);
        }

        _dirt_ghost.transform.position = _shootPosition;
    }
    
    // RPC 함수 정의
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcDoSomething()
    {
        Debug.Log("RPC 호출됨!");
        // 원하는 작업 수행
    }

    private void GetInteractionTarget()
    {
        RaycastHit hit;

        LayerMask interactableObjectMask = 1 << LayerMask.NameToLayer("InteractableObject");

        if (Physics.SphereCast(_playerCamera.transform.position, 0.25f, _playerCamera.transform.forward, out hit,
                InteractionRayCastDistance, interactableObjectMask))
        {
            // var interactable = hit.transform.GetComponent<IInteractable>();
            // 
            // if (interactable != null && interactable.Interactable)
            // {
            //     if (_interactionTarget != null && interactable != _interactionTarget)
            //     {
            //         _interactionTarget.Looking(false);
            //         _interactionTarget = null;
            //     }
            //     
            //     _interactionTarget = interactable;
            //     
            //     _interactionTarget.Looking(true);
            // 
            //     Global.Instance.F.gameObject.SetActive(true);
            // }
        }
        else
        {
            // if (_interactionTarget != null)
            // {
            //     _interactionTarget.Looking(false);
            // }
            // 
            // _interactionTarget = null;
            // 
            // Global.Instance.F.gameObject.SetActive(false);
        }
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