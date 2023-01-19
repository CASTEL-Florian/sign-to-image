using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

[Serializable]
public struct JsonDateTime
{
    public long value;
    public static implicit operator DateTime(JsonDateTime jdt)
    {
        return DateTime.FromFileTimeUtc(jdt.value);
    }
    public static implicit operator JsonDateTime(DateTime dt)
    {
        JsonDateTime jdt = new JsonDateTime();
        jdt.value = dt.ToFileTimeUtc();
        return jdt;
    }
}

[Serializable]
public class Painting
{
    public int id;
    public string name;
    public string description;
    public bool favorite;
    public JsonDateTime date;
}

[Serializable]
public class PaintingList
{
    public List<Painting> paintingList;
}

public enum Order
{
    Random,
    ID,
    Name,
    Date
}

public class ImageFileManager : MonoBehaviour
{
    private string directoryPath;

    private List<Painting> paintings;

    private List<int> availableIDs = new List<int>();
    private int maxID = 0;
    public static ImageFileManager Instance;
    private List<int> paintingOrder = new List<int>();
    [SerializeField] private List<Frame> frames;
    [SerializeField] private string querry = "";
    [SerializeField] private bool favorite = false;
    [SerializeField] private Order order = Order.Random;
    [SerializeField] private int offset = 0;
    [SerializeField] private bool reset = false;
    [SerializeField] private Gallery reception ;
    [SerializeField] private GalleryManager galleryManager ;
    public int PaintingsNumber { get; set; }

    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
        //frames = reception.frames;
    }


    void Start()
    {
        directoryPath = Application.dataPath + "/SavedPictures/";
        LoadData();
        PaintingsNumber = paintings.Count;
        if (paintings.Count > 0)
            maxID = paintings[paintings.Count - 1].id;
        else
            maxID = -1;

        // Find free IDs
        int k = 0;
        for (int i = 0; i < maxID; i++)
        {
            if (i != paintings[k].id)
            {
                availableIDs.Add(i);
            }
            else
            {
                k += 1;
            }
        }
        paintingOrder = Enumerable.Range(0, paintings.Count).ToList();

        OrderPaintings(order, querry, favorite);
       // ShowPaintings(frames, offset);
        /*
        AddPainting("A fox in the forest", new Texture2D(2, 2));
        AddPainting("A fox in the snow", new Texture2D(2, 2));
        AddPainting("A fox in the forest", new Texture2D(2, 2), true);
        AddPainting("A cat on the beach", new Texture2D(2, 2));
        AddPainting("A cat in the forest", new Texture2D(2, 2), true);
        AddPainting("A fox in the forest", new Texture2D(2, 2), true);
        AddPainting("A fox in the forest", new Texture2D(2, 2));
        AddPainting("A fox on the beach", new Texture2D(2, 2));
        AddPainting("A goat on the beach", new Texture2D(2, 2), true);
        AddPainting("A beach with a goat", new Texture2D(2, 2));
        AddPainting("A forest with a fox", new Texture2D(2, 2));
        AddPainting("A rabbit in space", new Texture2D(2, 2), true);
        AddPainting("A forest", new Texture2D(2, 2));
        AddPainting("A fox", new Texture2D(2, 2));
        AddPainting("A cat", new Texture2D(2, 2));
        AddPainting("A beach", new Texture2D(2, 2));
        AddPainting("A cat in the snow", new Texture2D(2, 2), true);
        AddPainting("A goat in the snow", new Texture2D(2, 2));
        AddPainting("A fox in the forest", new Texture2D(2, 2), true);
        AddPainting("A fox in the forest", new Texture2D(2, 2));
        AddPainting("A cat in the forest", new Texture2D(2, 2));
        AddPainting("fox in forest", new Texture2D(2, 2));
        AddPainting("fox in the forest", new Texture2D(2, 2));
        AddPainting("A dog in the snow", new Texture2D(2, 2));
        AddPainting("A dog", new Texture2D(2, 2));
        AddPainting("A cat", new Texture2D(2, 2));
        AddPainting("fox", new Texture2D(2, 2));
        AddPainting("A goat", new Texture2D(2, 2));
        AddPainting("cat", new Texture2D(2, 2));
        AddPainting("A dog", new Texture2D(2, 2));
        */
    }

    private void Update()
    {

        if (reset)
        {
            reset = false;
            OrderPaintings(order, querry, favorite);
            ShowPaintings(frames, offset);
        }
       // frames = galleryManager.frames;
    }
    public void LoadData()
    {
        if (File.Exists(directoryPath + "data.json"))
        {
            string json = File.ReadAllText(directoryPath + "data.json");
            paintings = JsonUtility.FromJson<PaintingList>(json).paintingList;
            paintings = paintings.OrderBy(p => p.id).ToList();
        }
        else
            paintings = new List<Painting>();
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(new PaintingList { paintingList = paintings });
        File.WriteAllText(directoryPath + "data.json", json);
    }

    public void AddPainting(string title,Texture2D tex, bool favorite = false)
    {
        Painting painting = new Painting();
        painting.name = title;
        painting.favorite = favorite;
        if (availableIDs.Count > 0) {
            painting.id = availableIDs[0];
            availableIDs.RemoveAt(0);
        }
        else
        {
            painting.id = ++maxID;
        }
        painting.date = DateTime.Now;
        paintings.Add(painting);
        paintings = paintings.OrderBy(p => p.id).ToList();
        byte[] byteArray = tex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/SavedPictures/" + painting.name + " " + painting.id.ToString() + ".png", byteArray);
        Debug.Log("creating the painting : " + painting.name + " " + painting.id.ToString());
        SaveData();
    }

    public void DeletePainting(Painting painting)
    {
        paintings.Remove(painting);
        if (painting.id != maxID)
        {
            availableIDs.Add(painting.id);
            maxID = paintings[paintings.Count - 1].id;
        }
        File.Delete(Application.dataPath + "/SavedPictures/" + painting.name + " " + painting.id.ToString() + ".png");
        SaveData();
    }

    private List<int> Search(string query, bool onlyFavorite = false)
    {
        // Split the query into individual words
        string[] queryWords = query.Split(' ');

        // Filter the list of file names using the Where method
        List<int> filteredPaintings = Enumerable.Range(0, paintings.Count).Where(i =>
        {

            // Check if the file name without the extension contains all the words in the query
            foreach (string queryWord in queryWords)
            {
                if (!paintings[i].name.Contains(queryWord) || (onlyFavorite && !paintings[i].favorite))
                {
                    return false;
                }
            }
            return true;
        }).ToList();
        return filteredPaintings;
    }


    public void OrderPaintings(Order order, string querry = "", bool onlyFavorite = false)
    {
        if (querry != "" || onlyFavorite)
            paintingOrder = Search(querry, onlyFavorite);
        else
            paintingOrder = Enumerable.Range(0, paintings.Count).ToList();
        switch (order)
        {
            case Order.Random:
                // Shuffle the list of paintings randomly
                paintingOrder = paintingOrder.OrderBy(x => Guid.NewGuid()).ToList();
                break;
            case Order.ID:
                // Order the list of paintings by their IDs
                paintingOrder = paintingOrder.OrderBy(x => paintings[x].id).ToList();
                break;
            case Order.Name:
                // Order the list of paintings by their names
                paintingOrder = paintingOrder.OrderBy(x => paintings[x].name).ToList();
                break;
            case Order.Date:
                // Order the list of paintings by their dates
                paintingOrder = paintingOrder.OrderBy(x => paintings[x].date.value).ToList();
                break;
            default:
                break;
        }
    }

    public void ShowPaintings(List<Frame> frames, int offset)
    {
        for (int i = offset; i < offset + frames.Count; i++)
        {
            if (i >= paintingOrder.Count)
            {
                frames[i - offset].Reset();
                continue;
            }
            Texture2D tex = new Texture2D(2, 2);
            string path = Application.dataPath + "/SavedPictures/" + paintings[paintingOrder[i]].name + " " + paintings[paintingOrder[i]].id.ToString() + ".png";
            if (File.Exists(path))
            {
                byte[] byteArray = File.ReadAllBytes(path);
                tex.LoadImage(byteArray);
            }
            frames[i - offset].SetPainting(paintings[paintingOrder[i]], tex);
        }
    }

}
