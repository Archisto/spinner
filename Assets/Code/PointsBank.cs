using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsBank : MonoBehaviour
{
    [Serializable]
    public struct ColorPoints
    {
        public Target.Colors Color;
        public int Points;
    }

    public ColorPoints[] points;

    // Start is called before the first frame update
    void Start()
    {
        points = new ColorPoints[Spinner.MaxTargetCount];

        for (int i = 0; i < points.Length; i++)
        {
            points[i].Color = (Target.Colors)i;
        }
    }

    public int GetPoints(Target.Colors color)
    {
        return points[(int)color].Points;
    }

    public int AddPoints(Target.Colors color, int pointChange)
    {
        points[(int)color].Points += pointChange;
        return points[(int)color].Points;
    }

    public void ResetPoints()
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].Points = 0;
        }
    }
}
