using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NPC : NetworkBehaviour
{
   [SerializeField] private Image _timerImage;
   [SerializeField] private Transform _wantItemParent;
   [SerializeField] private NpcWantItem _originWantItem;
   
   [SerializeField] private Renderer _renderer;
   [SerializeField] private NavMeshAgent _navMeshAgent;
   [SerializeField] private Outline _outline;
   [SerializeField] private Image _faceImage;
   [SerializeField] private List<Sprite> _faceImageList;
   [SerializeField] private List<ParticleSystem> _resultParticleList;

   private List<NpcWantItem> _npcWantItems = new List<NpcWantItem>();

   private float _timerTime;
   private float _waitTime;

   public bool IsSuccess = false;
   public bool IsFail = false;

   public bool IsEnd => IsSuccess || IsFail;
   
   public Transform TargetSit { get; set; }
   // [Networked] private 

   public override void Spawned()
   {
      base.Spawned();
      
      var newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
      
      var colorChangeMaterial = new Material(_renderer.material);
      colorChangeMaterial.color = newColor;

      _renderer.material = colorChangeMaterial;

      SetWantItem();
      StartTimer();
      
      _faceImage.gameObject.SetActive(false);
      
      Global.Instance.IngameManager.Npcs.Add(this);
   }

   public void SetWantItem()
   {
      _npcWantItems.Clear();
      
      // Test
      var wantItem = Instantiate(_originWantItem, _wantItemParent);
      wantItem.gameObject.SetActive(true);
      var wantItemRecipe = CraftRecipeManager.GetRecipe(InventoryItemType.BonFire);
      wantItem.SetInfo(wantItemRecipe);
      
      _npcWantItems.Add(wantItem);
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

   public void Update()
   {
      if (!HasStateAuthority)
      {
         return;
      }
      
      if (TargetSit != null)
      {
         _navMeshAgent.SetDestination(TargetSit.position);
      }

      if (!IsEnd)
      {
         _waitTime += Time.deltaTime;

         if (_timerTime > _waitTime)
         {
            _timerImage.fillAmount = (_timerTime - _waitTime) / _timerTime;
         }
         else
         {
            _timerImage.fillAmount = 0f;

            if (!IsEnd)
            {
               _faceImage.gameObject.SetActive(true);
               _faceImage.sprite = _faceImageList[2];
               _resultParticleList[2].gameObject.SetActive(true);
               _resultParticleList[2].Play();
               IsFail = true;
            }
         }
      }
   }

   public void ReceiveItem(InventoryItemType type)
   {
      var wantItem = _npcWantItems.First(x => x.WantCraftRecipe.ResultItem == type && !x.IsSuccess && !x.IsFail);
      wantItem.SetSuccess();

      // 끝!
      if (!_npcWantItems.Exists(x => !x.IsSuccess && !x.IsFail))
      {
         _faceImage.gameObject.SetActive(true);
         _faceImage.sprite = _faceImageList[0];
         _resultParticleList[0].gameObject.SetActive(true);
         _resultParticleList[0].Play();

         IsSuccess = true;
         return;
      }
   }

   public List<InventoryItemType> GetWantItemList()
   {
      return _npcWantItems.Where(x => !x.IsSuccess && !x.IsFail).Select(x=> x.WantCraftRecipe.ResultItem).ToList();
   }
}
