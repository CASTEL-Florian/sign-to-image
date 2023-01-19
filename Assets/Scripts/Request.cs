using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.IO;

public class Request : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] int steps = 15;
    [SerializeField] private string imageName = "image";
    [SerializeField] private Frame frame;
    [SerializeField] private bool saveImage = false;
    [SerializeField] private bool test = false;

    private string prompt = "A cat";
    private string negativePrompt = "bad art, low quality, ugly, blurry, pixelated, morbid, mutation, gross proportions, duplicate, mutilated, deformed, signature";
    private Texture2D tex;
    private int width = 512;
    private int height = 768;
    public bool generationEnded = true;
    public string url = "http://c376-35-188-156-72.ngrok.io";


    private void Update()
    {
        if (test)
        {
            test = false;
            Generate();
        }
    }
    private void Start()
    {
        tex = new Texture2D(2,2);
        LoadImage();
        url = PlayerPrefs.GetString("url");
    }
    public class Parameters
    {
        public bool enable_hr;
        public float denoising_strength;
        public int firstphase_width;
        public int firstphase_height;
        public string prompt;
        public string styles;
        public int seed;
        public int subseed;
        public float subseed_strength;
        public int seed_resize_from_h;
        public int seed_resize_from_w;
        public int batch_size;
        public int n_iter;
        public int steps;
        public float cfg_scale;
        public int width;
        public int height;
        public bool restore_faces;
        public bool tiling;
        public string negative_prompt;
        public float eta;
        public float s_churn;
        public float s_tmax;
        public float s_tmin;
        public float s_noise;
        public string sampler_index;
    }

    public class Response
    {
        public List<string> images;
        public Parameters param;
        public string info;
    }

    public class ProgressResponse
    {
        public string current_image;
    }

    public void SetPrompt(string _prompt)
    {
        prompt = _prompt;
    }
    public void Generate()
    {
        StartCoroutine(GetTexture());
    }
    IEnumerator GetTexture()
    {
        if (url.Length == 0)
            Debug.Log("url vide");
        if (url[url.Length - 1] == '/')
            url = url.Substring(0, url.Length - 1);
        float t = Time.time;

        string json = "{\"prompt\": \"" + prompt + "\", \"negative_prompt\": \"" + negativePrompt + "\", \"steps\": " + steps.ToString() + ", \"width\": " + width.ToString() + ", \"height\": " + height.ToString() + ", \"sampler_index\": \"Euler a\"}";
        print(json);
        var jsonBinary = System.Text.Encoding.UTF8.GetBytes(json);

        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();

        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
        uploadHandlerRaw.contentType = "application/json";
        UnityWebRequest www =
            new UnityWebRequest(url + "/sdapi/v1/txt2img", "POST", downloadHandlerBuffer, uploadHandlerRaw);
        www.SetRequestHeader("ngrok-skip-browser-warning", "69420");
        generationEnded = false;
        Coroutine progressRoutine = StartCoroutine(CheckProgress());
        yield return www.SendWebRequest();
        generationEnded = true;
        if (www.isNetworkError)
            Debug.LogError(string.Format("{0}: {1}", www.url, www.error));
        else
        {
            Debug.Log(string.Format("Response: {0}", www.downloadHandler.text));
            Response response = JsonUtility.FromJson<Response>(www.downloadHandler.text);
            byte[] imageBytes = Convert.FromBase64String(response.images[0]);
            tex.LoadImage(imageBytes);
            if (img)
                img.sprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2());
            Painting painting = new Painting();
            painting.date = DateTime.Now;
            painting.name = prompt;
            if (frame)
                frame.SetPainting(painting, tex);
            print("Time:" + (Time.time - t).ToString());
            if (saveImage)
                SaveImage();
        }
        www.Dispose();
    }

    IEnumerator CheckProgress()
    {
        float t = Time.time;
        do
        {
            yield return new WaitForSeconds(0.5f);
            string json = "{\"prompt\": \"" + prompt + "\", \"steps\": " + steps.ToString() + ", \"steps\": 10, \"sampler_index\": \"Euler a\"}";
            var jsonBinary = System.Text.Encoding.UTF8.GetBytes(json);

            DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();

            UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
            uploadHandlerRaw.contentType = "application/json";

            UnityWebRequest www =
                new UnityWebRequest(url + "/sdapi/v1/progress", "GET", downloadHandlerBuffer, uploadHandlerRaw);
            www.SetRequestHeader("ngrok-skip-browser-warning", "69420");
            yield return www.SendWebRequest();

            if (www.isNetworkError)
                Debug.LogError(string.Format("{0}: {1}", www.url, www.error));
            else
            {
                if (!generationEnded)
                {
                    print("progress time:" + (Time.time - t).ToString());
                    t = Time.time;
                    if (www.downloadHandler.text[0] == '{')
                    {
                        ProgressResponse response = JsonUtility.FromJson<ProgressResponse>(www.downloadHandler.text);
                        if (response.current_image.Length > 0)
                        {
                            byte[] imageBytes = Convert.FromBase64String(response.current_image);
                            Debug.Log(response.current_image);
                            tex.LoadImage(imageBytes);
                            if (img)
                                img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2());
                            Painting painting = new Painting();
                            painting.date = DateTime.Now;
                            painting.name = prompt;
                            if (frame)
                                frame.SetPainting(painting, tex);
                        }
                        else
                        {
                            Debug.Log("empty image");
                        }
                    }
                }
            }
            www.Dispose();
        } while (!generationEnded);
    }

    private void LoadImage()
    {
        string path = Application.dataPath + "/SavedPictures/" + imageName;
        if (File.Exists(path))
        {
            byte[] byteArray = File.ReadAllBytes(path);
            tex.LoadImage(byteArray);
            img.sprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2());
        }
    }
    private void SaveImage()
    {
        if (!ImageFileManager.Instance)
            return;
        ImageFileManager.Instance.AddPainting(prompt, tex, false);
    }
}
