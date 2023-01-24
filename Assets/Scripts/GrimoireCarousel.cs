using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GrimoireCarousel : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Image SignSpriteImageFirstPage;
    [SerializeField] private TextMeshProUGUI SignNameTextFirstPage;
    [SerializeField] private TextMeshProUGUI SignDescriptionTextFirstPage;
    [SerializeField] private Image SignSpriteImageSecondPage;
    [SerializeField] private TextMeshProUGUI SignNameTextSecondPage;
    [SerializeField] private TextMeshProUGUI SignDescriptionTextSecondPage;
    [SerializeField] private Button retourButon;
    private SignsData[] Signs;
    private int currentSignIndex = 1;


   

    // Start is called before the first frame update
    private void Start()
    {
        nextButton.onClick.AddListener(NextSign);
        previousButton.onClick.AddListener(PreviousSign);
      
        // retourButon.onClick.AddListener(retour);

        // Recuperation des donnees des signes
        Signs = Resources.LoadAll<SignsData>("ScriptableObjects");

        UpdateGrimoireUI();
    }

   /* private void retour()
    {
       
    }*/
 
    public void NextSign()
    {
        currentSignIndex+=2;
        if (currentSignIndex >= Signs.Length)
        {
            currentSignIndex = 1;
        }

        UpdateGrimoireUI();
    }

    public void PreviousSign()
    {
        currentSignIndex-=2;
        if (currentSignIndex <= 0)
        {
            currentSignIndex = Signs.Length - 1;
        }

        UpdateGrimoireUI();
    }

    private void UpdateGrimoireUI()
    {
        
        SignsData currentSign = Signs[currentSignIndex-1];
        SignNameTextFirstPage.SetText(currentSign.SignNameText);
        SignSpriteImageFirstPage.sprite = currentSign.SignSprite;
        SignDescriptionTextFirstPage.SetText(currentSign.SignDescriptionText.ToString());

        SignsData currentSignPage2 = Signs[currentSignIndex];
        SignNameTextSecondPage.SetText(currentSignPage2.SignNameText);
        SignSpriteImageSecondPage.sprite = currentSignPage2.SignSprite;
        SignDescriptionTextSecondPage.SetText(currentSignPage2.SignDescriptionText.ToString());

  


       
    }
}