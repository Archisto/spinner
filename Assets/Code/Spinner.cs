using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spinner : MonoBehaviour
{
    public const int MaxTargetCount = 15;

    [SerializeField]
    private Target targetPrefab;

    [SerializeField]
    private Dropdown targetCountDropdown;

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

    private SpinnerArrow spinnerArrow;
    private PointsBank pointsBank;
    private List<Target> targets;
    private int targetCount;
    private float targetSectorAngle;

    // Start is called before the first frame update
    void Start()
    {
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
            SetSpeed(speedSlider.value);
        }

        if (pointsVisibilityToggle != null)
        {
            SetPointsVisibility(pointsVisibilityToggle.isOn);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            CreateTargets(targetCount);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            if (spinnerArrow.active)
            {
                SelectTarget(highlightedTarget, true);
            }
            else
            {
                spinnerArrow.active = true;
            }
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            SelectRandomTarget();
        }
    }

    public void SetTargetCountFromDropdown(int dropdownTargetCount)
    {
        CreateTargets(dropdownTargetCount + 2);
    }

    public void SetSpeed(float speedRatio)
    {
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

    public void ResetPoints()
    {
        pointsBank.ResetPoints();

        foreach (Target target in targets)
        {
            target.SetPoints(pointsBank.GetPoints(target.TargetColor));
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
            this.targetCount = 0;
            return;
        }
        
        if (targetCount > 15)
        {
            targetCount = 15;
        }

        this.targetCount = targetCount;
        targets = new List<Target>(targetCount);
        targetSectorAngle = 360 / targetCount;
        Target.Colors[] colors = Target.GetColorMix(targetCount);

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
        if (targets == null || targetCount <= 0)
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
                targetIndex = targetCount - (int)(angleFromLastIndex / targetSectorAngle) - 1;
            }
        }

        if (targetIndex < 0 || targetIndex >= targetCount)
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

    private void SelectTarget(Target target, bool snapToTarget, bool spinnerArrowHighlightsTarget = false)
    {
        if (snapToTarget)
        {
            spinnerArrow.SnapTo(target.angle, spinnerArrowHighlightsTarget);
        }

        target.SetPoints(pointsBank.AddPoints(target.TargetColor, 1));

        Debug.Log($"Target selected: {target.TargetColor.ToString()}, {pointsBank.GetPoints(target.TargetColor)} pts");
    }

    private void SelectRandomTarget()
    {
        Target randomTarget = targets[Random.Range(0, targets.Count)];
        HighlightTarget(randomTarget);
        SelectTarget(randomTarget, true);
    }
}