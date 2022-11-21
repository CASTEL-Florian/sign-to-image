using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Keyboard : MonoBehaviour
{
    public TMP_InputField currentTextField;
    public TMP_InputField URLInputField;
    public TMP_InputField inputField;
    public GameObject normalButtons;
    public GameObject capsButtons;
    private bool isCaps;
    private bool isURL = false;

    // Start is called before the first frame update
    void Start()
    {
        isCaps = false;
        inputField = currentTextField;
    }

    public void InsertChar(string c)
    {
        inputField.text += c;
    }

    public void DeleteChar()
    {
        if(inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }

    public void InsertSpace()
    {
        inputField.text += " ";
    }
    public void CapsPressed()
    {
        if(!isCaps)
        {
            normalButtons.SetActive(false);
            capsButtons.SetActive(true);
            isCaps = true;
        }
        else
        {
            capsButtons.SetActive(false);
            normalButtons.SetActive(true);
            isCaps = false;
        }
    }

    public void SwitchToUrl()
    {
        if(isURL)
        {
            inputField = currentTextField;
            isURL = false;
        }
        else
        {
            inputField = URLInputField;
            isURL = true;
        }
    }
}
