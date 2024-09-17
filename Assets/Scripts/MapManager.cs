using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;
    [SerializeField] private List<NavMeshSurface> _navMeshSurfaceList;

    private int index = 0;

    public void UpdateMap()
    {
        // _navMeshSurfaceList[index].BuildNavMesh();
        // SetNextIndex();
    }

    private void SetNextIndex()
    {
        if (index == _navMeshSurfaceList.Count - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }
    }
}
