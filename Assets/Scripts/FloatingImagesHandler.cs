using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class FloatingImage
{
    public string name;
    public Color color;
    public Sprite sprite;
}
public class FloatingImagesHandler : MonoBehaviour
{
    [SerializeField] private List<FloatingImage> images;
    [SerializeField] private SymbolsMovement floatingImagePrefab;
    [SerializeField] private Transform target;
    [SerializeField] private Transform centerEyeAnchor;
    private List<SymbolsMovement> activeSymbols;
    [SerializeField]  bool test = false;
    [SerializeField]  bool test2 = false;

    private void Start()
    {
        activeSymbols = new List<SymbolsMovement>();
    }

    private void Update()
    {
        if (activeSymbols.Count > 0 && activeSymbols[0].TargetReached())
        {
            DeleteSymbols();
            return;
        }
        foreach (SymbolsMovement symbol in activeSymbols)
        {
            symbol.transform.LookAt(centerEyeAnchor.transform.position);
        }
        if (test)
        {
            DeleteSymbols();
            test = false;
            CreateImage("fox", new Vector3(0, 1, 0));
        }
        if (test2)
        {
            test2 = false;
            SendImagesToTarget();
        }
    }

    public void CreateImage(string name, Vector3 position)
    {
        int i = 0;
        while (i < images.Count && images[i].name != name)
            i++;
        if (i >= images.Count)
            return;
        SymbolsMovement floationgImage = Instantiate(floatingImagePrefab, position, Quaternion.identity);
        floationgImage.GetComponentInChildren<Image>().sprite = images[i].sprite;
        activeSymbols.Add(floationgImage);
    }

    public void SendImagesToTarget()
    {
        foreach (SymbolsMovement symbol in activeSymbols)
        {
            symbol.GoToTarget(target.position);
        }
    }

    public void DeleteSymbols()
    {
        foreach (SymbolsMovement symbol in activeSymbols)
        {
            Destroy(symbol.gameObject);
        }
        activeSymbols.Clear();
    }
}
