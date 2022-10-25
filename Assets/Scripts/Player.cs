using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private StarterAssets.FirstPersonController controller;
    [SerializeField] private StarterAssets.StarterAssetsInputs inputs;
    private Camera cam;
    [SerializeField] private float range = float.PositiveInfinity;
    private UIHandler ui;
    private Keyboard keyboard;
    // Start is called before the first frame update
    void Start()
    {
        ui = FindObjectOfType<UIHandler>();
        cam = Camera.main;
        keyboard = InputSystem.GetDevice<Keyboard>();
    }

    private void Update()
    {
        if (keyboard.ctrlKey.wasPressedThisFrame)
        {
            SelectFrame();
        }
        if (keyboard.enterKey.wasPressedThisFrame)
        {
            SelectFrame();
            ui.Generate();
        }
    }

    private void SelectFrame()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            if (hit.transform.tag == "Frame")
            {
                inputs.cursorInputForLook = !inputs.cursorInputForLook;
                inputs.cursorLocked = !inputs.cursorLocked;
                controller.enabled = !controller.enabled;
                ui.SetFrame(hit.transform.GetComponent<Request>());
                ui.ToggleUI();
            }
        }
        
    }
}
