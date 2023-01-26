
using UnityEngine;
using UnityEngine.Rendering;
public class LightmapChanger : MonoBehaviour
{
    public Texture2D[] newLightmaps;
    LightmapData[] currentLightmaps;
    public bool change;

    void Start()
    {
        // Get the current lightmaps used in the scene
         currentLightmaps = LightmapSettings.lightmaps;

      
    }
    private void Update()
    {
        if (change) 
        {
            // Modify the lightmap data in the array
            for (int i = 0; i < currentLightmaps.Length; i++)
            {
                currentLightmaps[i].lightmapColor = newLightmaps[i];
            }

            // Update the lightmaps used in the scene
            LightmapSettings.lightmaps = currentLightmaps;
        }
    }
}
