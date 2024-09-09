using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Addons.SimpleKCC;
using Photon.Voice.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public enum PlayerType
{
    Farmer,
    Fisher
}

public partial class Player : NetworkBehaviour
{
    private const float InteractionRayCastDistance = 3f;
    
    [SerializeField] private Dirt _dirt;
    [SerializeField] private GameObject _dirt_ghost;
    [SerializeField] private GameObject _fish_ghost;
    [SerializeField] private Ball _prefabBall;
    [SerializeField] private Transform _playerCameraRootTransform;
    [SerializeField] private Transform _modelTransform;
    [SerializeField] private Animator _animator;
    [SerializeField] private SimpleKCC _simpleKCC;
    [SerializeField] private GameObject _speakingIcon;
    [SerializeField] private Speaker _speaker;
    [SerializeField] private AudioSource _speakerAudioSource;
    [SerializeField] private Transform _headPivot;
    [SerializeField] private FishingRodLine _fishingRodLine;
    [SerializeField] private GameObject _fishingFX;
    [SerializeField] private ParticleSystem _fishingFIFIFIFX;
    [SerializeField] private FishCatchCanvas _fishCatchCanvas;
    [SerializeField] private HitCanvas _HitCanvas;
    [SerializeField] private FishWeapon _spawnFish;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private ParticleSystem _equipFx;
    
    [SerializeField] private Renderer _pants;
    [SerializeField] private List<Renderer> _hideBody;
    [SerializeField] private List<Renderer> _customizeRenderers;

    [SerializeField] private List<Collider> _ragDollCollider;
    [SerializeField] private List<Rigidbody> _ragDollRigidbody;
    
    [Space(5)]
    [Header("Tools")]
    [SerializeField] private GameObject _shovel;
    [SerializeField] private GameObject _seedBag;
    [SerializeField] private GameObject _fishRod;
    [SerializeField] private Weapon _fishWeapon;
    [SerializeField] private GameObject _fishWeapon2222;
    [SerializeField] private Axe _axe;
    
    private const float Gravity = 9.81f; // 중력 가속도

    private int _hp = 1000;
    [Networked] private NetworkBool _dead { get; set; } = false;
    [Networked, OnChangedRender(nameof(OnChangedPlayerType))] private PlayerType _playerType { get; set; }

    public enum ShootType
    {
        Dirt,
        Fishing,
        Bonfire,
    }

    private Vector3 _forward = Vector3.forward;
    
    private Vector3 _shootPosition = Vector3.zero;
    private bool _shootAble = false;
    private ShootType _shootType = ShootType.Dirt;

    private bool _isDigging = false;
    // , OnChangedRender(nameof(OnChangePlated))
    [Networked] private NetworkBool _isFishing { get; set; } = false;
    [Networked] private int _fishingState { get; set; } = 0;
    private Vector3 _fishingPosition { get; set; }
    private float _fishingCatchingTimer = 0;
    private bool _fishCatchStart = false;
    private bool _fishCatchComplete = false;

    private Dirt _plantTargetDirt = null;
    private bool _plantAble = false;
    
    private bool _isEquipWeapon = false;

    [Networked]
    private Vector3 _networkedMoveDirection { get; set; }
    
    [Networked]
    private Quaternion _networkedLookRotation { get; set; }

    private PlayerCamera _playerCamera;
    private Vector3 _cameraRotation;
    [Networked] private TickTimer _mouse0delay { get; set; }
    [Networked] private TickTimer _mouse1delay { get; set; }

    [Networked] public PlayerRef MyPlayerRef { get; set; }
    
    [Networked,OnChangedRender(nameof(OnChangedMyName))] public string MyName { get; set; }

    // ServerOnly
    public void SetPlayerType(PlayerType playerType)
    {
        _playerType = playerType;
        RpcSetPlayerType(playerType);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcSetPlayerType(PlayerType playerType)
    {
        _playerType = playerType;
        OnChangedPlayerType();
    }
    
    // ServerOnly
    public void SetPlayerRef(PlayerRef playerRef)
    {
        MyPlayerRef = playerRef;
    }

    public void OnChangedPlayerType()
    {
        if (_playerType == PlayerType.Farmer)
        {
            _shovel.SetActive(true);
            _fishRod.SetActive(false);
        }
        
        if (_playerType == PlayerType.Fisher)
        {
            _fishRod.SetActive(true);
            _shovel.SetActive(false);
        }
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcSetMyName(string myName)
    {
        MyName = myName;
        OnChangedPlayerType();
    }

    public void OnChangedMyName()
    {
        _nameText.text = MyName;
    }
    
    public override void Spawned()
    {
        base.Spawned();

        SetColor();

        SetRagDollMode(false);

        OnChangedPlayerType();
        OnChangedMyName();

        if (!HasInputAuthority)
        {
            return;
        }

        _inventoryBar.SetPlayer(this);
        
        _nameText.gameObject.SetActive(false);
        RpcSetMyName(Global.Instance.MyName);

        _playerCamera = GameObject.Find("PlayerFollowCamera").GetComponent<PlayerCamera>();
        _playerCamera.CinemachineCamera.Follow = this._playerCameraRootTransform;
        _playerCamera.transform.position = this._playerCameraRootTransform.position;
            
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        HideBody();
        
        _HitCanvas.gameObject.SetActive(true);
    }
    
    private void HideBody()
    {
        _hideBody.ForEach(x => x.shadowCastingMode = ShadowCastingMode.ShadowsOnly);
    }
    
    private void SetColor()
    {
        var newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        foreach (var renderer in _customizeRenderers)
        {
            var origin = renderer.material;
            var newMaterial = new Material(origin);

            newMaterial.color = newColor;
            renderer.material = newMaterial;
        }
        
        var origina = _pants.material;
        var newMateriala = new Material(origina);

        newMateriala.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        _pants.material = newMateriala;
    }

    private void Update()
    {
        if (!HasInputAuthority)
        { 
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _inventoryBar.SelectItem(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _inventoryBar.SelectItem(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _inventoryBar.SelectItem(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _inventoryBar.SelectItem(3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _inventoryBar.SelectItem(4);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _inventoryBar.SelectItem(5);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _inventoryBar.SelectItem(6);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _inventoryBar.SelectItem(7);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            _inventoryBar.SelectItem(8);
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            _inventoryBar.SelectNext();
        }
        
        if (Input.mouseScrollDelta.y > 0)
        {
            _inventoryBar.SelectPrev();
        }
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Global.Instance.ChatManager.IsFocus)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
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
        
        if (!_isFishing)
        {
            Look();
        }
        else
        {
            if (HasStateAuthority)
            {
                _fishingCatchingTimer -= Time.deltaTime;

                if (_fishingCatchingTimer <= 0f && !_fishCatchStart)
                {
                    _fishCatchStart = true; 
                    RpcFishNext();
                    RpcFishCatchingGOGO();
                }

                if (_fishCatchStart && _fishCatchComplete)
                {
                    RpcStopFishing(true);
                    _fishCatchStart = false;
                    _fishCatchComplete = false;
                    
                    if (HasStateAuthority)
                    {
                        var spawnedFish = Runner.Spawn(_spawnFish, _fishingFIFIFIFX.transform.position, Quaternion.LookRotation(Vector3.up), Object.InputAuthority);
                        spawnedFish.Fished();
                    }
                }
            }
        }

        if (_dead)
        {
            return;
        }

        _networkedMoveDirection = inputData.direction;
        
        if (inputData.direction.sqrMagnitude > 0 || inputData.jump)
        {
            Move(inputData);
            _forward = inputData.direction;
        }
        else
        {
            // 중력 적용
            if (!_simpleKCC.IsGrounded)
            {
                _simpleKCC.Move(Vector3.down * Gravity * Runner.DeltaTime);
            }
        }

        TreeUpdate(inputData);

        // if (_playerType == PlayerType.Fisher)
        if (_inventoryItemType == InventoryItemType.Fish || _inventoryItemType == InventoryItemType.FishRod)
        {
            FishWeaponUpdate(inputData);
        }

        if (_isEquipWeapon)
        {
            return;
        }

        if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
        {
            if (HasInputAuthority && _mouse0delay.ExpiredOrNotRunning(Runner))
            {
                if (_plantTargetDirt != null)
                {
                    RpcDoSomething(_plantTargetDirt);
                    RpcTriggerFeedingAnimeInput();
                }
            }

            if (HasStateAuthority && _mouse0delay.ExpiredOrNotRunning(Runner))
            {
                if (_shootAble)
                {
                    if (_shootType == ShootType.Dirt)
                    {
                        _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                        RpcTriggerShovelAnime(_shootPosition);
                    }

                    if (_shootType == ShootType.Fishing)
                    {
                        _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                        RpcTriggerFishingAnime(_shootPosition);

                        _fishingCatchingTimer = Random.Range(2f, 4f);
                        
                        _fishCatchStart = false;
                        _fishCatchComplete = false;
                    }
                }
            }
        }

        if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON1))
        {
            if (HasStateAuthority && _mouse1delay.ExpiredOrNotRunning(Runner))
            {
                if (_isFishing)
                {
                    _mouse1delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    RpcStopFishing(false);
                }
            }
        }
    }

    public override void Render()
    {
        base.Render();
        SetAnimation();

        // if (_playerType == PlayerType.Farmer)
        if (_inventoryItemType == InventoryItemType.SeedBag)
        {
            GetPlantTarget();
        }

        ShootGOGO();

        // if (_playerType == PlayerType.Fisher)
        if (_inventoryItemType == InventoryItemType.FishRod)
        {
            GetFish();
        }

        GetTree();
        GetLog();
        GetBonFire();

        ShowSpeakingIcon();
        
        if (!HasInputAuthority)
        {
            _playerCameraRootTransform.transform.rotation = _networkedLookRotation;
            _playerCameraRootTransform.transform.position = _headPivot.transform.position;
        }
        
        if (_isFishing)
        {
            _fishingRodLine.DrawBezierCurve();
            _fishingFX.transform.position = _fishingPosition;
            _fishingFIFIFIFX.transform.position = _fishingPosition;
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Villager@Shovel-Working01"))
        {
            _isDigging = true;
        }
        else
        {
            _isDigging = false;
        }
    }

    private void ShowSpeakingIcon()
    {
        _speakingIcon.SetActive(_speaker.IsPlaying && GetCurrentVolume() > 0.001f);
    }
    
    private float GetCurrentVolume()
    {
        float[] samples = new float[256];
        _speakerAudioSource.GetOutputData(samples, 0); // 오디오 샘플 데이터 가져오기
        float sum = 0f;

        foreach (float sample in samples)
        {
            sum += sample * sample;
        }

        return Mathf.Sqrt(sum / samples.Length); // RMS 값 계산
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
        if (_isFishing)
        {
            return;
        }
        
        if (_isDigging)
        {
            return;
        }
        
        var inputDir = networkInputData.direction.normalized;

        float angle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg;
        var moveDir = Quaternion.Euler(0, -_cameraRotation.y + angle, 0) * Vector3.forward;
        //  float moveSpeed = _isSprint ? SprintSpeed : MoveSpeed;
        
        Vector3 jumpImpulse  = default;
        
        if (networkInputData.jump == true && _simpleKCC.IsGrounded == true)
        {
            // Set world space jump vector.
            jumpImpulse = Vector3.up * 4.0f;
        }
        
        Vector3 inputDirection = _simpleKCC.TransformRotation * new Vector3(inputDir.x, 0f, inputDir.z);

        _simpleKCC.Move(inputDirection * 5f, jumpImpulse.magnitude);
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

        if (!_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            _playerCameraRootTransform.transform.position = _headPivot.transform.position;
        }

        if (HasStateAuthority)
        {
            _networkedLookRotation = Quaternion.Euler(_cameraRotation);
        }
    }

    private void GetPlantTarget()
    {
        LayerMask dirtMask = 1 << LayerMask.NameToLayer("Dirt");
        
        if (Physics.Raycast(_playerCameraRootTransform.transform.position, _playerCameraRootTransform.transform.forward, out RaycastHit hit,
                InteractionRayCastDistance, dirtMask) && !_isDigging)
        {
            _plantAble = true;

            if (HasInputAuthority)
            {
                var dirt = hit.transform.GetComponent<Dirt>();
                if (dirt != null && !dirt.Planted)
                {
                    if (_plantTargetDirt != null && (dirt != _plantTargetDirt || _plantTargetDirt.Planted))
                    {
                        _plantTargetDirt.Looking(false);
                        _plantTargetDirt = null;
                    }

                    _plantTargetDirt = dirt;
                    if (HasInputAuthority)
                    {
                        _plantTargetDirt.Looking(true);
                    }
                }
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
        if (_isFishing || _lookingFishWeapon != null || _isEquipWeapon)
        {
            NotShowingGround();
            return;
        }
        
        LayerMask groundMask = 1 << LayerMask.NameToLayer("Ground");
        LayerMask dirtMask = 1 << LayerMask.NameToLayer("Dirt");
        LayerMask waterMask = 1 << LayerMask.NameToLayer("Water");

        int layer = groundMask | dirtMask | waterMask;
        
        if (Physics.Raycast(_playerCameraRootTransform.transform.position, _playerCameraRootTransform.transform.forward, out RaycastHit hit,
                InteractionRayCastDistance) && !_plantAble)
        {
            if (1 << hit.transform.gameObject.layer == dirtMask.value)
            {
                NotShowingGround();
                return;
            }

            // if (_playerType == PlayerType.Farmer)
            if (_inventoryItemType == InventoryItemType.Shovel || _inventoryItemType == InventoryItemType.Log)
            {
                if (_fish_ghost.activeSelf)
                {
                    _fish_ghost.SetActive(false);
                }
                
                if (1 << hit.transform.gameObject.layer == groundMask.value)
                {
                    _shootPosition = hit.point;
                    _shootAble = true;

                    if (_inventoryItemType == InventoryItemType.Shovel)
                    {
                        DirtRender();
                        _shootType = ShootType.Dirt;
                    }
                    else
                    {
                        if (_dirt_ghost.activeSelf)
                        {
                            _dirt_ghost.SetActive(false);
                        }
                    }
           
                    if (_inventoryItemType == InventoryItemType.Log)
                    {
                        BonFireRender();
                        _shootType = ShootType.Bonfire;
                    }
                    else
                    {
                        if (_bonFire_Ghost.activeSelf)
                        {
                            _bonFire_Ghost.SetActive(false);
                        }
                    }
                }
            }

            // if (_playerType == PlayerType.Fisher)
            if (_inventoryItemType == InventoryItemType.FishRod)
            {
                if (_dirt_ghost.activeSelf)
                {
                    _dirt_ghost.SetActive(false);
                }
                if (_bonFire_Ghost.activeSelf)
                {
                    _bonFire_Ghost.SetActive(false);
                }
                
                if (1 << hit.transform.gameObject.layer == waterMask.value)
                {
                    _shootPosition = hit.point;
                    _shootAble = true;

                    FishingRender();
                    
                    _shootType = ShootType.Fishing;
                }
            }
        }
        else
        {
            NotShowingGround();
        }

        void NotShowingGround()
        {
            _shootPosition = Vector3.zero;
            _shootAble = false;
            
            if (_dirt_ghost.activeSelf)
            {
                _dirt_ghost.SetActive(false);
            }
            if (_fish_ghost.activeSelf)
            {
                _fish_ghost.SetActive(false);
            }
            if (_bonFire_Ghost.activeSelf)
            {
                _bonFire_Ghost.SetActive(false);
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
    
    private void FishingRender()
    {
        if (!HasInputAuthority)
        {
            return; 
        }
        
        if (!_fish_ghost.activeSelf)
        {
            _fish_ghost.SetActive(true);
        }

        _fish_ghost.transform.position = _shootPosition;
    }
    
    // RPC 함수 정의
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcDoSomething(Dirt dirt)
    {
        dirt.Planted = true;
    }
    
    public void DoDig()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        if (_shootAble && _shootType == ShootType.Dirt)
        {
            Runner.Spawn(_dirt, _shootPosition, Quaternion.LookRotation(_forward), Object.InputAuthority);
        }

        _isDigging = false;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcTriggerShovelAnime(Vector3 shootPosition)
    {
        _animator.SetTrigger("Shovel");
        SetToolFalse();
        _shovel.SetActive(true);
        _shootPosition = shootPosition;
        _isDigging = true;
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcTriggerFeedingAnimeInput()
    {
        // Input -> State -> All
        RpcTriggerFeedingAnime();
        // _animator.SetTrigger("Feed");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcTriggerFeedingAnime()
    {
        _animator.SetTrigger("Feed");

        SetToolFalse();

        _seedBag.SetActive(true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcTriggerFishingAnime(Vector3 shootPosition)
    {
        _fishingPosition = shootPosition;
        _isFishing = true;
        _fishingState = 0;

        SetToolFalse();
        
        _fishRod.SetActive(true);

        _animator.SetBool("IsFishing", true);
        _animator.SetInteger("FishingState", _fishingState);

        _fishingRodLine.gameObject.SetActive(true);

        _fishingRodLine.SetFinalPosition(shootPosition);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcFishNext()
    {
        _fishingState = 1;
        _animator.SetInteger("FishingState", _fishingState);
        _fishingFX.SetActive(true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    public void RpcFishCatchingGOGO()
    {
        _fishCatchCanvas.gameObject.SetActive(true);
        _fishCatchCanvas.StartGOGO();
    }

    public void CatchComplete()
    {
        _fishCatchCanvas.gameObject.SetActive(false);
        _fishCatchComplete = true;

        RpcFishCatchComplete();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcFishCatchComplete()
    {
        _fishCatchComplete = true;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcStopFishing(bool success)
    {
        _isFishing = false;
        _fishingState = 0;

        _animator.SetBool("IsFishing", false);
        _animator.SetInteger("FishingState", _fishingState);
            
        _fishingRodLine.gameObject.SetActive(false);
        _fishingFX.SetActive(false);

        if (success)
        {
            _fishingFIFIFIFX.gameObject.SetActive(true);
            _fishingFIFIFIFX.Play();
        }
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

    private void Dead()
    {
        if (!_dead)
        {
            RpcInputToStateSetRagDollMode(true);
            _HitCanvas.Dead();
            RpcSetDead(true);
            Respawn();
        }
    }

    private async void Respawn()
    {
        await UniTask.Delay(3000);
        
        _hp = 1000;
        RpcSetDead(false);
        _HitCanvas.Respawn();
        
        RpcInputToStateSetRagDollMode(false);
    }

    private void OnDamaged(int damage)
    {
        if (!HasInputAuthority)
        {
            return;
        }

        _hp -= damage;

        if (_hp <= 0)
        {
            _hp = 0;
            Dead();
        }
        
        _HitCanvas.Hitted(_hp);
    }

    private void OnParticleCollision(GameObject other)
    {
        var collParticle = other.transform.GetComponent<CollParticle>();
        if (collParticle == null)
        {
            return;
        }

        OnDamaged(collParticle.Damage);
    }

    private void OnTriggerEnter(Collider other)
    {
        var collParticle = other.transform.GetComponent<CollParticle>();
        if (collParticle == null)
        {
            return;
        }

        OnDamaged(collParticle.Damage);
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcSetDead(bool dead)
    {
        _dead = dead;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcInputToStateSetRagDollMode(bool enable)
    {
        RpcStateToAllSetRagDollMode(enable);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcStateToAllSetRagDollMode(bool enable)
    {
        SetRagDollMode(enable);
    }
    
    private void SetRagDollMode(bool enable)
    {
        _animator.enabled = !enable;
        _ragDollCollider.ForEach(x => x.enabled = enable);
        _ragDollRigidbody.ForEach(x => x.isKinematic = !enable);
    }

    private void SetToolFalse()
    {
        _shovel.SetActive(false);
        _fishRod.SetActive(false);
        _seedBag.SetActive(false);
        _axe.gameObject.SetActive(false);   
    }
}