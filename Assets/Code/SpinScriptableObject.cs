using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spin", menuName = "Scriptable Objects/Spin", order = 1)]
public class SpinScriptableObject : ScriptableObject
{
    public const float SpinDuration = 3.00f;
    public const float StepDuration = 0.25f;
    public const float MinSpeedMultiplier = 1f;
    public const float MaxSpeedMultiplier = 2f;

    [Header("Spin Speed Curve")]
    public float step00;
    public float step01;
    public float step02;
    public float step03;
    public float step04;
    public float step05;
    public float step06;
    public float step07;
    public float step08;
    public float step09;
    public float step10;
    public float step11;

    private float[] steps;

    public float GetRandomSpeedMultiplier()
    {
        return Random.Range(MinSpeedMultiplier, MaxSpeedMultiplier);
    }

    public float GetSpeedAt(float time, float speedMultiplier = 1)
    {
        // TODO: Smooth transitions between steps

        float ratio = time / SpinDuration;
        int step = (int)(12 * ratio);

        if (step < 0)
        {
            step = 0;
        }
        else if (step >= 12)
        {
            return 0;
        }

        if (steps == null || steps.Length == 0)
        {
            steps = new float[]
            {
                step00,
                step01,
                step02,
                step03,
                step04,
                step05,
                step06,
                step07,
                step08,
                step09,
                step10,
                step11
            };
        }

        return steps[step] * speedMultiplier;
    }
}
