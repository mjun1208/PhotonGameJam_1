using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    public List<Table> Tables;
    public List<NPC> Npcs;
    public NPC Npc;
    public MapManager MapManager;
    public Transform NpcSpawnPosition;
    public Transform NpcReturnPosition;
    public ServerOnlyGameManager ServerOnlyGameManager { get; set; }
    
    public int MaxTable = 10;
    public bool IsTabling = false;

    private void Start()
    {
        Global.Instance.IngameManager = this;
    }

    public void UpdateMap()
    {
        MapManager.UpdateMap();
    }

    public Transform GetTableSit()
    {
        var tableSit = Tables.Select(x => x.GetEmptySit()).FirstOrDefault(x => x != null);
        return tableSit;
    }
}
