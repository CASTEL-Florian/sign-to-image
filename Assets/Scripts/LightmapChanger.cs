
using UnityEngine;
using UnityEngine.Rendering;
public class LightmapChanger : MonoBehaviour
{
    public Texture2D[] newLightmapsGlobal;
    public Texture2D[] newLightmapsCanvas;
    public Texture2D[] newLightmapsQuest;
    public Texture2D[] newLightmapsGrimoire;
    public GameObject canvasCandles;
    public GameObject GrimoireCandles;
    public GameObject QuestCandles;
    public GameObject GlobalCandles;
    public LightmapData[] currentLightmaps;
    public bool IsGlobal;
    public bool IsCanvas;
    public bool IsQuest;
    public bool IsGrimoire;
    public LightmapData[] newLightmapData;
    void Start()
    {
        // Get the current lightmaps used in the scene
         currentLightmaps = LightmapSettings.lightmaps;
      //  LightmapSettings.lightmaps = newLightmapData;

    }
    private void Update()
    {
        if (IsGlobal) 
        {
            // Modify the lightmap data in the array
            for (int i = 0; i < currentLightmaps.Length; i++)
            {
                currentLightmaps[i].lightmapColor = newLightmapsGlobal[i];
            }

            // Update the lightmaps used in the scene
            LightmapSettings.lightmaps = currentLightmaps;
            GlobalCandles.SetActive(true);
            canvasCandles.SetActive(true);
            GrimoireCandles.SetActive(true);
            QuestCandles.SetActive(true);
        }  if (IsCanvas) 
        {
            // Modify the lightmap data in the array
            for (int i = 0; i < currentLightmaps.Length; i++)
            {
                currentLightmaps[i].lightmapColor = newLightmapsCanvas[i];
            }
            GlobalCandles.SetActive(false);
            canvasCandles.SetActive(true);
            GrimoireCandles.SetActive(false);
            QuestCandles.SetActive(false);
            // Update the lightmaps used in the scene
            LightmapSettings.lightmaps = currentLightmaps;
        }  if (IsGrimoire) 
        {
            // Modify the lightmap data in the array
            for (int i = 0; i < currentLightmaps.Length; i++)
            {
                currentLightmaps[i].lightmapColor = newLightmapsGrimoire[i];
            }
            GlobalCandles.SetActive(false);
            canvasCandles.SetActive(false);
            GrimoireCandles.SetActive(true);
            QuestCandles.SetActive(false);
            // Update the lightmaps used in the scene
            LightmapSettings.lightmaps = currentLightmaps;
        }  if (IsQuest) 
        {
            // Modify the lightmap data in the array
            for (int i = 0; i < currentLightmaps.Length; i++)
            {
                currentLightmaps[i].lightmapColor = newLightmapsQuest[i];
            }
            GlobalCandles.SetActive(false);
            canvasCandles.SetActive(false);
            GrimoireCandles.SetActive(false);
            QuestCandles.SetActive(true);
            // Update the lightmaps used in the scene
            LightmapSettings.lightmaps = currentLightmaps;
        }
    }
}
