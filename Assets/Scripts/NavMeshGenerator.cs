using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
public class NavMeshGenerator : MonoBehaviour
{
    //public NavMesh navMesh;
    public NavMeshData navMeshData;
    private void Start()
    {
        // Create a NavMeshData object
        navMeshData = new NavMeshData();

        // Collect all the NavMeshBuildSources in the scene
        NavMeshBuildSettings settings = NavMesh.GetSettingsByID(0);
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        NavMeshBuilder.CollectSources(transform, settings.agentTypeID, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>(), sources);
        // Build the NavMesh
      
        Bounds localBounds = new Bounds(Vector3.zero, Vector3.one * 999999);
        NavMeshBuilder.UpdateNavMeshData(navMeshData, settings, sources, localBounds);

        // Assign the NavMeshData to the NavMesh
        NavMesh.AddNavMeshData(navMeshData);
    }
}
