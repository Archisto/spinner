using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spinner : MonoBehaviour
{
    public const int MaxTargetCount = 15;

    private enum AspectRatio
    {
        Aspect16_9,
        Aspect16_10,
        Aspect5_4,
        Aspect4_3,
        Unknown
    }

    [SerializeField]
    private Target targetPrefab;

    [SerializeField]
    private Dropdown targetCountDropdown;

    [SerializeField]
    private Dropdown aspectRatioDropdown;

    [SerializeField]
    private Slider speedSlider;

    [SerializeField]
    private Toggle pointsVisibilityToggle;

    [SerializeField]
    private float targetRingRadius = 3;

    [SerializeField]
    private float minSpeed = 0;

    [SerializeField]
    private float maxSpeed = 1200;

    [SerializeField]
    private Target highlightedTarget;

    [SerializeField]
    private SpinScriptableObject[] spins;

    private SpinnerArrow spinnerArrow;
    private PointsBank pointsBank;
    private List<Target> targets;
    private float targetSectorAngle;

    public bool controlledSpinActive;
    private SpinScriptableObject currentSpin;
    private float spinSpeedMultiplier;
    private float elapsedTime;

    private AspectRatio aspectRatio;

    public int TargetCount { get; private set; }

    public Dropdown TargetCountDropdown => targetCountDropdown;

    public Selectable DefaultMenuElement => targetCountDropdown;

    public bool SpinningWithFixedSpeed =>
        spinnerArrow.active && !controlledSpinActive && spinnerArrow.rotationSpeed > 0;

    public Target SelectedTarget
    {
        get
        {
            return !controlledSpinActive && (!spinnerArrow.active || spinnerArrow.rotationSpeed <= 0)
                ? highlightedTarget
                : null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Resolution resolution = Screen.currentResolution;
        aspectRatio = resolution.width == 1920 && resolution.height == 1080
            ? AspectRatio.Aspect16_9
            : resolution.width == 1280 && resolution.height == 800
              ? AspectRatio.Aspect16_10
              : AspectRatio.Unknown;

        Debug.Log("Resolution: " + resolution.ToString() + "\n"
                  + "Aspect ratio: " + aspectRatio.ToString());

        spinnerArrow = FindObjectOfType<SpinnerArrow>();
        pointsBank = FindObjectOfType<PointsBank>();

        if (targetCountDropdown != null)
        {
            targetCountDropdown.SetValueWithoutNotify(8);
            SetTargetCountFromDropdown(targetCountDropdown.value);
        }

        if (speedSlider != null)
        {
            speedSlider.SetValueWithoutNotify(0.5f);
            //SetSpeed(speedSlider.value);
        }

        if (pointsVisibilityToggle != null)
        {
            SetPointsVisibility(pointsVisibilityToggle.isOn);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (controlledSpinActive)
        {
            if (elapsedTime >= SpinScriptableObject.SpinDuration)
            {
                elapsedTime = 0;
                controlledSpinActive = false;
                spinnerArrow.active = false;
                SelectTarget(highlightedTarget, false);
            }
            else
            {
                elapsedTime += Time.deltaTime;
                SetSpeed();
            }
        }
    }

    public void SetAspectRatioFromDropdown(int dropdownId)
    {
        Resolution resolution = new Resolution();
        resolution.refreshRate = Screen.currentResolution.refreshRate;

        switch (dropdownId)
        {
            case 0:
            default:
                if (aspectRatio == AspectRatio.Aspect16_9)
                {
                    return;
                }

                aspectRatio = AspectRatio.Aspect16_9;
                resolution.width = 1920;
                resolution.height = 1080;
                break;
            case 1:
                if (aspectRatio == AspectRatio.Aspect16_10)
                {
                    return;
                }

                aspectRatio = AspectRatio.Aspect16_10;
                resolution.width = 1280;
                resolution.height = 800;
                break;
            case 2:
                if (aspectRatio == AspectRatio.Aspect5_4)
                {
                    return;
                }

                aspectRatio = AspectRatio.Aspect5_4;
                resolution.width = 1000;
                resolution.height = 800;
                break;
            case 3:
                if (aspectRatio == AspectRatio.Aspect4_3)
                {
                    return;
                }

                aspectRatio = AspectRatio.Aspect4_3;
                resolution.width = 800;
                resolution.height = 600;
                break;
        }

        Debug.Log("New resolution: " + resolution.ToString() + "\n"
                  + "New aspect ratio: " + aspectRatio.ToString());

        Screen.SetResolution(resolution.width, resolution.height, true);
    }

    public void SetTargetCountFromDropdown(int dropdownTargetCount)
    {
        CreateTargets(dropdownTargetCount + 2);
    }

    public void SetSpeed(float speedRatio = 0)
    {
        if (controlledSpinActive)
        {
            spinnerArrow.rotationSpeed = currentSpin.GetSpeedAt(elapsedTime, spinSpeedMultiplier);
            return;
        }

        spinnerArrow.rotationSpeed = minSpeed + speedRatio * (maxSpeed - minSpeed);

        if (spinnerArrow.active && spinnerArrow.rotationSpeed == 0)
        {
            SelectTarget(highlightedTarget, false);
        }
    }

    public void SetPointsVisibility(bool visible)
    {
        foreach (Target target in targets)
        {
            target.SetPointsVisibility(visible);
        }
    }

    public void CreateTargets(int targetCount = 0)
    {
        if (targetPrefab == null)
        {
            return;
        }

        DestroyTargets();

        if (targetCount <= 0)
        {
            TargetCount = 0;
            return;
        }
        
        if (targetCount > 15)
        {
            targetCount = 15;
        }

        TargetCount = targetCount;
        targets = new List<Target>(targetCount);
        targetSectorAngle = 360 / targetCount;
        Target.Colors[] colors = Target.GetColorsInOrder(targetCount);

        for (int i = 0; i < targetCount; i++)
        {
            Target newTarget = Instantiate(targetPrefab);
            Target.Colors color = colors[i];
            newTarget.TargetColor = color;
            newTarget.name = "Target (" + color.ToString() + ")";
            newTarget.SetNumber(i + 1);
            newTarget.SetPoints(pointsBank.GetPoints(color));
            newTarget.SetPointsVisibility(pointsVisibilityToggle.isOn);

            float radians = (i * 2 * Mathf.PI) / targetCount;
            float x = targetRingRadius * Mathf.Sin(radians);
            float y = targetRingRadius * Mathf.Cos(radians);
            newTarget.transform.position = new Vector2(x, y);
            newTarget.angle = i * targetSectorAngle;

            targets.Add(newTarget);
        }
    }

    private void DestroyTargets()
    {
        if (targets == null)
        {
            return;
        }

        for (int i = targets.Count - 1; i >= 0; i--)
        {
            Destroy(targets[i].gameObject);
        }

        targets = null;
    }

    public void HighlightTarget(float angle)
    {
        if (targets == null || TargetCount <= 0)
        {
            return;
        }

        int targetIndex = 0;

        if (angle < 0)
        {
            float angleFromFirstIndex = (-1 * angle) - (targetSectorAngle / 2);
            if (angleFromFirstIndex >= 0)
            {
                targetIndex = (int)(angleFromFirstIndex / targetSectorAngle);
            }
        }
        else
        {
            if (angle > (targetSectorAngle / 2))
            {
                float angleFromLastIndex = angle - (targetSectorAngle / 2);
                targetIndex = TargetCount - (int)(angleFromLastIndex / targetSectorAngle) - 1;
            }
        }

        if (targetIndex < 0 || targetIndex >= TargetCount)
        {
            Debug.LogError("Invalid target index: " + targetIndex);
            return;
        }

        Target targetPointedAt = targets[targetIndex];
        if (targetPointedAt != highlightedTarget)
        {
            HighlightTarget(targetPointedAt);
        }
    }

    private void HighlightTarget(Target target)
    {
        if (highlightedTarget != null)
        {
            highlightedTarget.Highlight(false);
        }

        highlightedTarget = target;
        highlightedTarget.Highlight(true);
    }

    public void SelectTarget(Target target, bool snapToTarget, bool spinnerArrowHighlightsTarget = false)
    {
        if (snapToTarget)
        {
            spinnerArrow.SnapTo(target.angle, spinnerArrowHighlightsTarget);
        }

        AddPoints(target, 1);

        Debug.Log($"Target selected: {target.TargetColor.ToString()}, {pointsBank.GetPoints(target.TargetColor)} pts");
    }

    public void SelectRandomTarget()
    {
        if (controlledSpinActive)
        {
            return;
        }

        Target randomTarget = targets[Random.Range(0, targets.Count)];
        HighlightTarget(randomTarget);
        SelectTarget(randomTarget, true);
    }

    public void StartSpin()
    {
        if (controlledSpinActive)
        {
            return;
        }

        if (spinnerArrow.active)
        {
            ToggleSpin(true);
        }
        else
        {
            StartControlledSpin();
        }
    }

    private void StartControlledSpin()
    {
        controlledSpinActive = true;
        currentSpin = spins[Random.Range(0, spins.Length)];
        spinSpeedMultiplier = currentSpin.GetRandomSpeedMultiplier();
        spinnerArrow.rotationSpeed = currentSpin.GetSpeedAt(0);
        spinnerArrow.active = true;
    }

    public void ToggleSpin(bool selectHighlightedTarget)
    {
        if (controlledSpinActive)
        {
            return;
        }

        if (spinnerArrow.rotationSpeed <= 0)
        {
            ResetSpeedSlider();
            spinnerArrow.active = true;
        }
        else if (spinnerArrow.active)
        {
            if (selectHighlightedTarget)
            {
                SelectTarget(highlightedTarget, true);
            }
            else
            {
                spinnerArrow.active = false;
            }
        }
        else
        {
            spinnerArrow.active = true;
        }
    }

    public void AddPoints(Target target, int points)
    {
        if (target == null)
        {
            return;
        }

        target.SetPoints(pointsBank.AddPoints(target.TargetColor, points));
    }

    public void ResetPoints()
    {
        pointsBank.ResetPoints();

        foreach (Target target in targets)
        {
            target.SetPoints(pointsBank.GetPoints(target.TargetColor));
        }
    }

    public void ResetSpeedSlider()
    {
        spinnerArrow.rotationSpeed = minSpeed + 0.5f * (maxSpeed - minSpeed);
        speedSlider.SetValueWithoutNotify(0.5f);
    }
}
