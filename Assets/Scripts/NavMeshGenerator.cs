using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NavMeshGenerator : MonoBehaviour
{
    //public NavMesh navMesh;
    /* public NavMeshData navMeshData;
     [SerializeField]
     private LayerMask BuildMask;
     private void Start()
     {
         // Create a NavMeshData object
         navMeshData = new NavMeshData();

         // Collect all the NavMeshBuildSources in the scene
         NavMeshBuildSettings settings = NavMesh.GetSettingsByID(0);
         List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
         NavMeshBuilder.CollectSources(transform, settings.agentTypeID, NavMeshCollectGeometry.PhysicsColliders, BuildMask, new List<NavMeshBuildMarkup>(), sources);
         // Build the NavMesh

         Bounds localBounds = new Bounds(Vector3.zero, Vector3.one * 999999);
         NavMeshBuilder.UpdateNavMeshData(navMeshData, settings, sources, localBounds);

         // Assign the NavMeshData to the NavMesh
         NavMesh.AddNavMeshData(navMeshData);
     }*/

    Vector3 BoundsCenter = Vector3.zero;
    Vector3 BoundsSize = new Vector3(512f, 4000f, 512f);

    LayerMask BuildMask;
    LayerMask NullMask;

    NavMeshData NavMeshData;
    NavMeshDataInstance NavMeshDataInstance;

    void Start()
    {
        AddNavMeshData();
        BuildMask = ~0;
        NullMask = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Build " + Time.realtimeSinceStartup.ToString());
            Build();
            Debug.Log("Build finished " + Time.realtimeSinceStartup.ToString());
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("Update " + Time.realtimeSinceStartup.ToString());
            UpdateNavmeshData();
        }
    }

    void AddNavMeshData()
    {
        if (NavMeshData != null)
        {
            if (NavMeshDataInstance.valid)
            {
                NavMesh.RemoveNavMeshData(NavMeshDataInstance);
            }
            NavMeshDataInstance = NavMesh.AddNavMeshData(NavMeshData);
        }
    }

    void UpdateNavmeshData()
    {
        StartCoroutine(UpdateNavmeshDataAsync());
    }

    IEnumerator UpdateNavmeshDataAsync()
    {
        AsyncOperation op = NavMeshBuilder.UpdateNavMeshDataAsync(
            NavMeshData,
            NavMesh.GetSettingsByID(0),
            GetBuildSources(BuildMask),
            new Bounds(BoundsCenter, BoundsSize));
        yield return op;

        AddNavMeshData();
        Debug.Log("Update finished " + Time.realtimeSinceStartup.ToString());
    }

    void Build()
    {
        NavMeshData = NavMeshBuilder.BuildNavMeshData(
            NavMesh.GetSettingsByID(0),
            GetBuildSources(NullMask),
            new Bounds(BoundsCenter, BoundsSize),
            Vector3.zero,
            Quaternion.identity);
        AddNavMeshData();
    }

    List<NavMeshBuildSource> GetBuildSources(LayerMask mask)
    {
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        NavMeshBuilder.CollectSources(
            new Bounds(BoundsCenter, BoundsSize),
            mask,
            NavMeshCollectGeometry.PhysicsColliders,
            0,
            new List<NavMeshBuildMarkup>(),
            sources);
        Debug.Log("Sources found: " + sources.Count.ToString());
        return sources;
    }

}

 
public class NavBuilder : MonoBehaviour
{

   
}