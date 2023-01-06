using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryManager : MonoBehaviour
{

    
    public int number_of_tab=0;
 //  public  List<GameObject> galleryModules;
   public  List<Gallery> galleryModules;
    public List< Transform >availableDocks;
    bool modulecreated = false;
    // Start is called before the first frame update
    int findMinRooms(int[] capacities, int numPeople)
    {
        int num_rooms = 0;
        int remaining = numPeople;
        while(remaining>0){ }
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
        
    }

    // Update is called once per frame
    void Update()
    {
        /* if(!modulecreated)
         CheckTabNum();*/
        InstantiateGallery();
    }
   public void CheckTabNum() 
    {
        modulecreated = true;
        
        if (number_of_tab <= 6) { Instantiate(galleryModules[6], availableDocks[Random.Range(0,availableDocks.Count-1)].position, transform.rotation * Quaternion.Euler(0f, 180f, 0f)); }
        else if (number_of_tab > 6 && number_of_tab <= 12) {Instantiate(galleryModules[12], availableDocks[Random.Range(0, availableDocks.Count - 1)].position, transform.rotation * Quaternion.Euler(0f, 180f, 0f));  }
   
        
        

    }
    void InstantiateGallery()
    {
        int oldnum = number_of_tab;
        //GameObject dock=new GameObject();
        Gallery temp;
      //  dock.transform.position=new Vector3(0, 0, 0);
        int dockstart=0;
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
                            availableDocks.Add( Dock);
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

                        temp = Instantiate(galleryModules[i+1], availableDocks[randPos].position, transform.rotation * Quaternion.Euler(0f, availableDocks[randPos].eulerAngles.y, 0f));
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
}
