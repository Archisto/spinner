using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerArrow : MonoBehaviour
{
    public float rotationSpeed = 20;
    public bool active = true;

    private Spinner spinner;
    private float angle;

    // Start is called before the first frame update
    void Start()
    {
        spinner = FindObjectOfType<Spinner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            UpdateRotation();
        }
    }

    private void UpdateRotation()
    {
        Vector3 newRotation = transform.rotation.eulerAngles;
        newRotation.z -= rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(newRotation);
        angle = newRotation.z;
        spinner.HighlightTarget(angle);
    }

    public void SnapTo(float angle, bool highlightTarget)
    {
        active = false;

        Vector3 newRotation = transform.rotation.eulerAngles;
        newRotation.z = -1 * angle;
        transform.rotation = Quaternion.Euler(newRotation);
        this.angle = transform.rotation.eulerAngles.z;

        if (highlightTarget)
        {
            spinner.HighlightTarget(this.angle);
        }
    }
}
