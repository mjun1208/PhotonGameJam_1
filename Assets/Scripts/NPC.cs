using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Fusion;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Newtonsoft.Json;
using UnityEngine.Rendering.UI;

public class NPC : NetworkBehaviour
{
   [SerializeField] private Image _timerImage;
   [SerializeField] private Transform _wantItemParent;
   [SerializeField] private NpcWantItem _originWantItem;
   [SerializeField] private GameObject _timerObject;
   
   [SerializeField] private Renderer _renderer;
   [SerializeField] private Animator _animator;
   [SerializeField] private GameObject _chair;
   [SerializeField] private NavMeshAgent _navMeshAgent;
   [SerializeField] private Outline _outline;
   [SerializeField] private Image _faceImage;
   [SerializeField] private List<Sprite> _faceImageList;
   [SerializeField] private List<ParticleSystem> _resultParticleList;

   private List<NpcWantItem> _npcWantItems = new List<NpcWantItem>();

   [Networked, OnChangedRender(nameof(SetColor))] private Color _myColor { get; set; }

   [Networked, OnChangedRender(nameof(SetTimer))] private float _fillAmount { get; set; } = 1f;
   [Networked, OnChangedRender(nameof(StartOrder_Networked))] private bool _isStartOrder_Networked { get; set; }
   [Networked, OnChangedRender(nameof(SetResting))] private bool _isResting { get; set; }
   [Networked, OnChangedRender(nameof(SetWantItem_Networked)), Capacity(200)] private string _npcWantItems_Networked { get; set; } = "";

   private float _timerTime;
   private float _waitTime;
   private bool _isStartOrder = false;

   public bool IsSuccess = false;
   public bool IsFail = false;

   public bool IsEnd => IsSuccess || IsFail;
   public bool IsStart => _isStartOrder_Networked;
   public bool IsResting => _isResting;
   
   public Transform TargetSit { get; set; }
   public Table TargetTable { get; set; }
   // [Networked] private 
   
   public override void Spawned()
   {
      base.Spawned();

      if (HasStateAuthority)
      {
         _myColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
      }
      
      SetColor();

      _faceImage.gameObject.SetActive(false);
      Global.Instance.IngameManager.Npcs.Add(this);
   }

   public void SetColor()
   {
      var newColor = _myColor;
      
      var colorChangeMaterial = new Material(_renderer.material);
      colorChangeMaterial.color = newColor;

      _renderer.material = colorChangeMaterial;
   }

   public void StartOrder()
   {
      _animator.SetBool("Sit", true);
      _isStartOrder_Networked = true;
      _isStartOrder = true;
      SetWantItem();
      StartTimer();

      this.transform.position = TargetSit.transform.position;
      this.transform.rotation = TargetSit.transform.rotation;
      this.transform.rotation = TargetSit.transform.rotation;
      
      _timerObject.gameObject.SetActive(true);
   }

   public void StartOrder_Networked()
   {
      _timerObject.gameObject.SetActive(true);
   }

   public void SetWantItem_Networked()
   {
      if (HasStateAuthority)
      {
         return;
      }

      if (string.IsNullOrWhiteSpace(_npcWantItems_Networked))
      {
         return;
      }
      
      List<NpcWantItem_Networked> deserializedList = JsonConvert.DeserializeObject<List<NpcWantItem_Networked>>(_npcWantItems_Networked);

      for (int i = 0; i < _npcWantItems.Count; i++)
      {
         Destroy(_npcWantItems[i].gameObject);
      }
      
      _npcWantItems.Clear();
      
      foreach (var npcWantItemNetworked in deserializedList)
      {
         var wantItem = Instantiate(_originWantItem, _wantItemParent);
         wantItem.gameObject.SetActive(true);
         var wantItemRecipe = CraftRecipeManager.GetRecipe((InventoryItemType)npcWantItemNetworked.R);
         wantItem.SetInfo(wantItemRecipe);

         if (npcWantItemNetworked.S)
         {
            wantItem.SetSuccess();
         }
         
         if (npcWantItemNetworked.F)
         {
            wantItem.SetFail();
         }

         _npcWantItems.Add(wantItem);
      }
      
      // 끝!
      if (_npcWantItems.All(x => x.IsSuccess))
      {
         _faceImage.gameObject.SetActive(true);
         _faceImage.sprite = _faceImageList[0];
         _resultParticleList[0].gameObject.SetActive(true);
         _resultParticleList[0].Play();

         IsSuccess = true;
         
         _animator.SetTrigger("Clap");
      }
   }

   public void SetWantItem()
   {
      for (int i = 0; i < _npcWantItems.Count; i++)
      {
         Destroy(_npcWantItems[i].gameObject);
      }
      
      _npcWantItems.Clear();

      for (int i = 0; i < 4; i++)
      {
         // Test
         var wantItem = Instantiate(_originWantItem, _wantItemParent);
         wantItem.gameObject.SetActive(true);
         var wantItemRecipe = CraftRecipeManager.GetRecipe(InventoryItemType.BonFire);
         wantItem.SetInfo(wantItemRecipe);

         _npcWantItems.Add(wantItem);
      }

      SendNetworkNpcWantItems();
   }

   public void SendNetworkNpcWantItems()
   {
      var networkedList = _npcWantItems.Select(x => x.ToNetworked()).ToList();
      string jsonString = JsonConvert.SerializeObject(networkedList);
      _npcWantItems_Networked = jsonString;
   }

   public void Look(bool look)
   {
      _outline.enabled = look;
   }

   public void StartTimer()
   {
      _timerImage.fillAmount = 1f;
      _timerTime = 60f;
      _waitTime = 0f;
   }

   private void LateUpdate()
   {
      if (HasStateAuthority)
      {
         if (TargetSit != null)
         {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Move"))
            {
               _navMeshAgent.SetDestination(TargetSit.position);
            }

            if (Vector3.Distance(TargetSit.position, this.transform.position) < 2f)
            {
               if (Global.Instance.IngameManager.NpcReturnPosition == TargetSit)
               {
                  _isResting = true;
                  _faceImage.gameObject.SetActive(false);

                  _resultParticleList[0].gameObject.SetActive(false);
                  _resultParticleList[1].gameObject.SetActive(false);
                  _resultParticleList[2].gameObject.SetActive(false);

                  this.gameObject.SetActive(false);

                  Global.Instance.IngameManager.ServerOnlyGameManager.EndNpcCount++;
               }
            }

            if (Vector3.Distance(TargetSit.position, this.transform.position) < 1f)
            {
               if (!_isStartOrder)
               {
                  StartOrder();
               }

               // if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
               // {
               //    // 목표 지점에 도착한 경우
               //    if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
               //    {
               //    }
               // }
            }
         }

         if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Sit"))
         {
            if (HasStateAuthority)
            {
               if (TargetTable != null)
               {
                  this.transform.LookAt(TargetTable.transform);
               }
            }
         }
      }
   }

   public void Update()
   {
      if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Sit"))
      {
         if (!_chair.activeSelf)
         {
            _chair.SetActive(true);
         }
      }
      else
      {
         if (_chair.activeSelf)
         {
            _chair.SetActive(false);
         }
      }
      
      if (!HasStateAuthority)
      {
         if (_isStartOrder_Networked)
         {
            if (_fillAmount == 0f)
            {
               if (!IsEnd)
               {
                  _faceImage.gameObject.SetActive(true);
                  _faceImage.sprite = _faceImageList[2];
                  _resultParticleList[2].gameObject.SetActive(true);
                  _resultParticleList[2].Play();
                  IsFail = true;
                  
                  _animator.SetBool("Sit", false);
               }
            }
         }
         return;
      }
      else
      {
         if (!IsEnd && _isStartOrder)
         {
            _waitTime += Time.deltaTime;

            if (_timerTime > _waitTime)
            {
               _fillAmount = (_timerTime - _waitTime) / _timerTime;
               _timerImage.fillAmount = _fillAmount;
            }
            else
            {
               _fillAmount = 0f;
               _timerImage.fillAmount = _fillAmount;

               if (!IsEnd)
               {
                  _faceImage.gameObject.SetActive(true);
                  _faceImage.sprite = _faceImageList[2];
                  _resultParticleList[2].gameObject.SetActive(true);
                  _resultParticleList[2].Play();
                  IsFail = true;
                  _animator.SetBool("Sit", false);

                  var wantItems = _npcWantItems.Where(x => !x.IsSuccess && !x.IsFail);
                  foreach (var npcWantItem in wantItems)
                  {
                     npcWantItem.SetFail();
                  }

                  SendNetworkNpcWantItems();

                  if (TargetTable != null && TargetSit != null)
                  {
                     TargetTable.SetSitEmpty(TargetSit);
                  }
                  
                  TargetTable = null;
                  TargetSit = Global.Instance.IngameManager.NpcReturnPosition;
               }
            }
         }
      }
   }

   public void SetResting()
   {
      if (HasStateAuthority)
      {
         return;
      }

      if (IsResting)
      {
         _resultParticleList[0].gameObject.SetActive(false);
         _resultParticleList[1].gameObject.SetActive(false);
         _resultParticleList[2].gameObject.SetActive(false);
         
         _faceImage.gameObject.SetActive(false);
         gameObject.SetActive(false);
      }
   }

   public void SetTimer()
   {
      if (HasStateAuthority)
      {
         return;
      }
      
      _timerImage.fillAmount = _fillAmount;
   }

   public void ReceiveItem(InventoryItemType type)
   {
      var wantItem = _npcWantItems.First(x => x.WantCraftRecipe.ResultItem == type && !x.IsSuccess && !x.IsFail);
      wantItem.SetSuccess();

      SendNetworkNpcWantItems();

      // 끝!
      if (!_npcWantItems.Exists(x => !x.IsSuccess && !x.IsFail))
      {
         _faceImage.gameObject.SetActive(true);
         _faceImage.sprite = _faceImageList[0];
         _resultParticleList[0].gameObject.SetActive(true);
         _resultParticleList[0].Play();

         if (TargetTable != null)
         {
            TargetTable.SetReward(TargetTable.RewardCount + 1000);
         }

         IsSuccess = true;
         _animator.SetBool("Sit", false);
         _animator.SetTrigger("Clap");

         if (TargetTable != null && TargetSit != null)
         {
            TargetTable.SetSitEmpty(TargetSit);
         }

         TargetTable = null;
         TargetSit = Global.Instance.IngameManager.NpcReturnPosition;

         return;
      }
   }

   public List<InventoryItemType> GetWantItemList()
   {
      return _npcWantItems.Where(x => !x.IsSuccess && !x.IsFail).Select(x=> x.WantCraftRecipe.ResultItem).ToList();
   }
}
