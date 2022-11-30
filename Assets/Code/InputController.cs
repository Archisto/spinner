using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private SpinButton spinButton;

    private Spinner spinner;

    // Start is called before the first frame update
    void Start()
    {
        spinner = FindObjectOfType<Spinner>();

        PrintControllerInfo();
    }

    private void PrintControllerInfo()
    {
        string controllerStr = Input.GetJoystickNames().ArrayToString();

        if (controllerStr.Length > 0)
        {
            Debug.Log("Controllers connected: " + controllerStr);
        }
        else
        {
            Debug.Log("Controllers connected: None");
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDebugInput();
        UpdateInput();
    }

    private void UpdateInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UpdateMouseClick();
        }
        else if (Input.GetButtonUp("Start Spin"))
        {
            spinner.StartSpin();
        }
        else if (Input.GetButtonUp("Toggle Spin"))
        {
            spinner.ToggleSpin();
        }
        else if (Input.GetButtonUp("Select Random Target"))
        {
            spinner.SelectRandomTarget();
        }
        else if (Input.GetButtonUp("Add Points"))
        {
            spinner.AddPoints(spinner.SelectedTarget, 1);
        }
    }

    private void UpdateMouseClick()
    {
        if (spinner.controlledSpinActive)
        {
            return;
        }

        Vector3 mouseInWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Rect rect = new Rect(spinButton.transform.position - spinButton.transform.localScale / 2,
                             spinButton.transform.localScale);

        if (rect.Contains(mouseInWorldPos))
        {
            spinner.StartSpin();
        }
    }

    private void UpdateDebugInput()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            spinner.CreateTargets(spinner.TargetCount);
        }
    }
}
