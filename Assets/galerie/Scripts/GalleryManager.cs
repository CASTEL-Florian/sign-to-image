using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryManager : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] AudioHandler audioHandler;
    [SerializeField] private ImageFileManager imageFileManager;
    [SerializeField] private GameObject restricted;

    private int PaintingsNumber = 0;

    [SerializeField]private Gallery reception;

    [SerializeField]private List<Gallery> galleryModules;
    [SerializeField] private List<Frame> frames;

    int AvailableModuleIndex = 1;
    int offset;
    int i;
    int currentRoom;
   
    Dictionary<int, Gallery> availableModules = new Dictionary<int, Gallery>();


    void Start()
    {
        //To play the gallery music when generated
        audioHandler.PlayGaleryleMusic();

        //Assigning the number of painting created by the player 
        PaintingsNumber = imageFileManager.PaintingsNumber;

        //adding the gallery room to the reception before starting the generation of the other room needed 
        availableModules.Add(0, reception);
        PaintingsNumber -= reception.capacity;
        frames.AddRange(reception.frames);
        //Calling the function responsable of the exhebition of the paintings in the gallery
        imageFileManager.ShowPaintings(reception.frames,offset );

        offset += reception.frames.Count;
    }

    // Update is called once per frame
    void Update()
    {
       
        //Keep track of the Current room that the player are in 
        currentRoom = FindCurrentRoom(availableModules, player);
   

        if (PaintingsNumber > 0)
        {
            SuccessiveGeneration(currentRoom);
        }
        //Function to hide the none connected rooms to the current room 
        hideModules(currentRoom);
        //Function to activate the connected rooms to the current room 
        showModule(currentRoom);
    }
  
   
    //The function searchs for the rooms that aren't connected to the current one and hides them 
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
    //The function searchs for the rooms that are connected to the current one and activate them 
    void showModule(int currentRoom)
    {
        int v;
        List<int> connectedModules = FindConnectedRooms(currentRoom);

        foreach (int key in connectedModules)
        {
            availableModules[key].gameObject.SetActive(true);
        }

    }

    //Generating succesivually the gallery depending on the player's current room  
    void SuccessiveGeneration(int CurrentRoomKey)
    {

        Gallery module = availableModules[CurrentRoomKey];

        Gallery temp = null ;


        List<int> connected = FindConnectedRooms(CurrentRoomKey);
        //Following the logic of FindConnectedRooms function it adds also the actual room to the list of connected room and the initial dock port used to connect the new module with the dock of the currentroom exept if current room is the reception it do not have a initial dock 
        int n = CurrentRoomKey == 0 ? 1 : 2;
        //To verifie we already generated rooms and connected them to the current one if so the number of the connected room will not be inferior 
        if (connected.Count < module.docks.Length + n)
        {

            //Connecting all the room's docks by generating a module for each dock  
            foreach (Transform Dock in module.docks)
            {
                
                if (galleryModules[i].capacity <= PaintingsNumber)
                {
                    if (i != 0|| PaintingsNumber - galleryModules[i].capacity <= 0)
                    {


                        temp = Instantiate(galleryModules[i], Dock.position, transform.rotation * Quaternion.Euler(0f, Dock.eulerAngles.y, 0f));
                        PaintingsNumber -= galleryModules[i].capacity;
                        availableModules.Add(AvailableModuleIndex, temp);
                        frames.AddRange(temp.frames);

                    }
             
                    else if (PaintingsNumber - galleryModules[i].capacity >= 0)
                    {


                        temp = Instantiate(galleryModules[i + 1], Dock.position, transform.rotation * Quaternion.Euler(0f, Dock.eulerAngles.y, 0f));

                        PaintingsNumber -= galleryModules[i + 1].capacity;
                        availableModules.Add(AvailableModuleIndex, temp);
                        frames.AddRange(temp.frames);

                    }
                    AvailableModuleIndex++;
                    i++;
                    //we have only 3 modules
                    if (i > 2) { i = 0; }

                    imageFileManager.ShowPaintings(temp.frames, offset);
                    offset += temp.frames.Count;
                }

                else
                {
                    if (PaintingsNumber > 0)
                    {
                        for (int j = 0; j < galleryModules.Count; j++)
                        {
                            if (galleryModules[j].capacity > PaintingsNumber)
                            {
                                PaintingsNumber -= galleryModules[j].capacity;
                                temp = Instantiate(galleryModules[i], Dock.position, transform.rotation * Quaternion.Euler(0f, Dock.eulerAngles.y, 0f));
                                availableModules.Add(AvailableModuleIndex,temp );
                                frames.AddRange(temp.frames);
                                AvailableModuleIndex++;
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

