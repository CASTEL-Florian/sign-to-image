using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryManager : MonoBehaviour
{
    [SerializeField] Transform player;
    
    static int moduleindex = 1;
    int offset;
    int i;
    int currentRoom;
    public int number_of_tab = 0;
    public Gallery reception;
    //  public  List<GameObject> galleryModules;
    [SerializeField] private ImageFileManager imageFileManager;
    [SerializeField] private GameObject restricted;
    public List<Gallery> galleryModules;
    public List<Transform> availableDocks;
    Dictionary<int, Gallery> availableModules = new Dictionary<int, Gallery>();
   public List<Frame> frames;
    bool modulecreated = false;
    // Start is called before the first frame update
    int findMinRooms(int[] capacities, int numPeople)
    {
        int num_rooms = 0;
        int remaining = numPeople;
        while (remaining > 0) { }
        for (int i = 0; i < capacities.Length; i++)
        {
            if (capacities[i] <= remaining)
            {
                num_rooms++;
                remaining -= capacities[i];
            }
            else if (i == 3)
            {

            }
            {
                break;
            }
        }
        return num_rooms;
    }

    void Start()
    {
       
        availableModules.Add(0, reception);
        number_of_tab -= reception.capacity;
        frames.AddRange(reception.frames);
        //imageFileManager.ShowPaintings(reception.frames, 0);


    }

    // Update is called once per frame
    void Update()
    {
        /* if(!modulecreated)
         CheckTabNum();*/
        //  InstantiateGallery3();

        currentRoom = FindCurrentRoom(availableModules, player);
   
        if (number_of_tab > 0) { SuccessiveGeneration(currentRoom); }
        else
        {
            foreach (Transform dock in availableModules[availableModules.Count - 1].docks) 
            {
                Instantiate(restricted, dock.position, Quaternion.identity);
            }
            
        }
        hideModules(currentRoom);
        showModule(currentRoom);
    }
    public void CheckTabNum()
    {
        modulecreated = true;

        if (number_of_tab <= 6) { Instantiate(galleryModules[6], availableDocks[Random.Range(0, availableDocks.Count - 1)].position, transform.rotation * Quaternion.Euler(0f, 180f, 0f)); }
        else if (number_of_tab > 6 && number_of_tab <= 12) { Instantiate(galleryModules[12], availableDocks[Random.Range(0, availableDocks.Count - 1)].position, transform.rotation * Quaternion.Euler(0f, 180f, 0f)); }




    }
    void InstantiateGallery()
    {
        int oldnum = number_of_tab;
        //GameObject dock=new GameObject();
        Gallery temp;
        //  dock.transform.position=new Vector3(0, 0, 0);
        int dockstart = 0;
        while (number_of_tab > 0)
        {
            if (oldnum == number_of_tab)
            {
                availableDocks.Add(this.transform);
            }

            for (int i = 0; i < galleryModules.Count; i++)
            {
                if (galleryModules[i].capacity <= number_of_tab)
                {
                    if (i != 0)
                    {
                        int randPos = Random.Range(dockstart, availableDocks.Count);

                        temp = Instantiate(galleryModules[i], availableDocks[randPos].position, transform.rotation * Quaternion.Euler(0f, availableDocks[randPos].eulerAngles.y, 0f));
                        availableDocks.RemoveAt(randPos);
                        foreach (Transform Dock in temp.docks)
                        {
                            availableDocks.Add(Dock);
                        }


                        number_of_tab -= galleryModules[i].capacity;
                    }
                    else if (number_of_tab - galleryModules[i].capacity <= 0)
                    {
                        int randPos = Random.Range(dockstart, availableDocks.Count);

                        temp = Instantiate(galleryModules[i], availableDocks[randPos].position, transform.rotation * Quaternion.Euler(0f, availableDocks[randPos].eulerAngles.y, 0f));
                        availableDocks.RemoveAt(randPos);

                        foreach (Transform Dock in temp.docks)
                        {
                            availableDocks.Add(Dock);
                        }
                        number_of_tab -= galleryModules[i].capacity;
                    }
                    else if (number_of_tab - galleryModules[i].capacity >= 0)
                    {
                        int randPos = Random.Range(dockstart, availableDocks.Count);

                        temp = Instantiate(galleryModules[i + 1], availableDocks[randPos].position, transform.rotation * Quaternion.Euler(0f, availableDocks[randPos].eulerAngles.y, 0f));
                        availableDocks.RemoveAt(randPos);
                        foreach (Transform Dock in temp.docks)
                        {
                            availableDocks.Add(Dock);
                        }
                        number_of_tab -= galleryModules[i + 1].capacity;

                    }
                }
                /*     else if (galleryModules[i].capacity == number_of_tab)
                     {

                         Instantiate(galleryModules[i], dock.position, transform.rotation * Quaternion.Euler(0f, 180f, 0f));
                         number_of_tab -= galleryModules[i].capacity;
                     }*/
                else
                {
                    if (number_of_tab > 0)
                    {
                        for (int j = 0; j < galleryModules.Count; j++)
                        {
                            if (galleryModules[j].capacity > number_of_tab)
                            {
                                int randPos = Random.Range(dockstart, availableDocks.Count);

                                Instantiate(galleryModules[i], availableDocks[randPos].position, transform.rotation * Quaternion.Euler(0f, availableDocks[randPos].eulerAngles.y, 0f));
                                availableDocks.RemoveAt(randPos);
                                number_of_tab -= galleryModules[j].capacity;

                            }
                            break;
                        }

                    }
                }
                Debug.Log(number_of_tab);
                //dockstart = 1;
            }

        }



    }




    void hideModules(int currentRoom)
    {
        int v;
        List<int> connectedModules = FindConnectedRooms(currentRoom);
 
        foreach (KeyValuePair<int, Gallery> module in availableModules)
        {
            if (!connectedModules.Contains(module.Key))
            {
                module.Value.gameObject.SetActive(false);
            }
        }

    }
    void showModule(int currentRoom)
    {
        int v;
        List<int> connectedModules = FindConnectedRooms(currentRoom);

        foreach (int key in connectedModules)
        {
            availableModules[key].gameObject.SetActive(true);
        }

    }
    void SuccessiveGeneration(int ModuleKey)
    {
        Gallery module = availableModules[ModuleKey];

        Gallery temp = null ;


        List<int> connected = FindConnectedRooms(ModuleKey);
        int n = ModuleKey == 0 ? 1 : 2;
        if (connected.Count < module.docks.Length + n)
        {


            foreach (Transform Dock in module.docks)
            {

                if (galleryModules[i].capacity <= number_of_tab)
                {
                    if (i != 0)
                    {


                        temp = Instantiate(galleryModules[i], Dock.position, transform.rotation * Quaternion.Euler(0f, Dock.eulerAngles.y, 0f));
                        number_of_tab -= galleryModules[i].capacity;
                        availableModules.Add(moduleindex, temp);
                        frames.AddRange(temp.frames);

                    }
                    else if (number_of_tab - galleryModules[i].capacity <= 0)
                    {

                        temp = Instantiate(galleryModules[i], Dock.position, transform.rotation * Quaternion.Euler(0f, Dock.eulerAngles.y, 0f));
                        number_of_tab -= galleryModules[i].capacity;
                        availableModules.Add(moduleindex, temp);
                        frames.AddRange(temp.frames);
                    }
                    else if (number_of_tab - galleryModules[i].capacity >= 0)
                    {


                        temp = Instantiate(galleryModules[i + 1], Dock.position, transform.rotation * Quaternion.Euler(0f, Dock.eulerAngles.y, 0f));

                        number_of_tab -= galleryModules[i + 1].capacity;
                        availableModules.Add(moduleindex, temp);
                        frames.AddRange(temp.frames);

                    }
                    moduleindex++;
                    i++;
                    if (i > 2) { i = 0; }
                    imageFileManager.ShowPaintings(temp.frames, offset);
                    offset += temp.frames.Count;
                }

                else
                {
                    if (number_of_tab > 0)
                    {
                        for (int j = 0; j < galleryModules.Count; j++)
                        {
                            if (galleryModules[j].capacity > number_of_tab)
                            {




                                number_of_tab -= galleryModules[j].capacity;
                                temp = Instantiate(galleryModules[i], Dock.position, transform.rotation * Quaternion.Euler(0f, Dock.eulerAngles.y, 0f));
                                availableModules.Add(moduleindex,temp );
                                frames.AddRange(temp.frames);
                                moduleindex++;
                                i++;
                                if (i > 3) { i = 0; }
                                imageFileManager.ShowPaintings(temp.frames, offset);
                                offset += temp.frames.Count;
                            }
                            break;
                        }
                        i++;
                        if (i > 2) { i = 0; }
                    }
                }


            }
        }
    }

    int FindCurrentRoom(Dictionary<int, Gallery> activeRooms, Transform player)
    {
        // Iterate through the active rooms dictionary and check if the player is inside any of the rooms
        foreach (KeyValuePair<int, Gallery> room in activeRooms)
        {
            if (room.Value.gameObject.GetComponent<Collider>().bounds.Contains(player.position))
            {
                return room.Key;
            }
        }
        // If the player is not in any of the rooms, return -1
        return -1;
    }


    List<int> FindConnectedRooms(int currentRoom)
    {
        List<int> connectedRooms = new List<int>();
        // Add the current room to the connected rooms list
        connectedRooms.Add(currentRoom);
        // Iterate through the active rooms dictionary and check if the room is connected to the current room
        if (availableModules.Count > 1)
        {
            foreach (KeyValuePair<int, Gallery> room in availableModules)
            {
                foreach (Transform previousdock in room.Value.docks)
                {
                    if (previousdock.position == availableModules[currentRoom].transform.position)
                    {
                        connectedRooms.Add(room.Key);
                        break;
                    }
                }
                foreach (Transform dock in availableModules[currentRoom].docks)
                {
                    if (room.Value.transform.position == dock.position)
                    {
                        connectedRooms.Add(room.Key);
                    }
                }

            }
        }
        return connectedRooms;
    }

}

