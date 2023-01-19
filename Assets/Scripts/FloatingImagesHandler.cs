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
    [SerializeField] private FloatingImage unknownImage;
    [SerializeField] private FloatingSymbol floatingImagePrefab;
    [SerializeField] private Transform target;
    [SerializeField] private Transform centerEyeAnchor;
    private List<FloatingSymbol> activeSymbols;

    private void Start()
    {
        activeSymbols = new List<FloatingSymbol>();
    }

    private void Update()
    {
        if (activeSymbols.Count > 0 && activeSymbols[0].TargetReached())
        {
            DeleteSymbols();
            return;
        }
        foreach (FloatingSymbol symbol in activeSymbols)
        {
            symbol.transform.LookAt(centerEyeAnchor.transform.position);
        }
    }

    public void CreateImage(string name, Vector3 position, string type)
    {
        int i = 0;
        List<FloatingSymbol> symbolsToDelete = activeSymbols.FindAll(s => s.GetSymbolType() == type);
        foreach (FloatingSymbol symbol in symbolsToDelete)
        {
            symbol.FadeOutAndDestroy();
        }
        activeSymbols.RemoveAll(s => s.GetSymbolType() == type);
        while (i < images.Count && images[i].name != name)
            i++;
        if (i >= images.Count)
        {
            FloatingSymbol floatingImage = Instantiate(floatingImagePrefab, position, Quaternion.identity);
            floatingImage.SetSymbolImage(unknownImage.sprite);
            floatingImage.SetBackgroundColor(unknownImage.color);
            floatingImage.SetType(type);
            activeSymbols.Add(floatingImage);
            return;
        }
        else
        {
            FloatingSymbol floatingImage = Instantiate(floatingImagePrefab, position, Quaternion.identity);
            floatingImage.SetSymbolImage(images[i].sprite);
            floatingImage.SetBackgroundColor(images[i].color);
            floatingImage.SetType(type);
            activeSymbols.Add(floatingImage);
        }
        
    }

    public void SendImagesToTarget()
    {
        foreach (FloatingSymbol symbol in activeSymbols)
        {
            symbol.GoToTarget(target.position);
        }
    }

    public void DeleteSymbols()
    {
        foreach (FloatingSymbol symbol in activeSymbols)
        {
            Destroy(symbol.gameObject);
        }
        activeSymbols.Clear();
    }
}
