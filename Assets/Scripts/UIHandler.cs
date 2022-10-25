using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject generationUI;
    [SerializeField] private TMP_InputField inputField;
    private Request frame;

    public void ToggleUI()
    {
        generationUI.SetActive(!generationUI.activeSelf);
        if (generationUI.activeSelf)
            inputField.ActivateInputField();
    }

    public void UpdatePrompt()
    {
        frame.SetPrompt(inputField.text);
    }

    public void Generate()
    {
        frame.Generate();
    }

    public void SetFrame(Request f)
    {
        frame = f;
    }
}
