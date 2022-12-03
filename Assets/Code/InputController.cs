using UnityEngine;
using UnityEngine.EventSystems;

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

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            UpdateMenuInput();
        }
        else
        {
            UpdateGameInput();
        }
    }

    private void UpdateGameInput()
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
            spinner.ToggleSpin(true);
        }
        else if (spinner.SpinningWithFixedSpeed && Input.GetButtonUp("Cancel"))
        {
            spinner.ToggleSpin(false);
        }
        else if (Input.GetButtonUp("Select Random Target"))
        {
            spinner.SelectRandomTarget();
        }
        else if (Input.GetButtonUp("Add Points"))
        {
            spinner.AddPoints(spinner.SelectedTarget, 1);
        }
        else if (Input.GetButtonUp("Open Menu"))
        {
            spinner.DefaultMenuElement.Select();
        }
    }

    private void UpdateMenuInput()
    {
        if (Input.GetButtonUp("Cancel"))
        {
            if (spinner.TargetCountDropdown.IsActive())
            {
                spinner.TargetCountDropdown.Hide();
            }

            EventSystem.current.SetSelectedGameObject(null);
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
