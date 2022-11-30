using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public enum Colors
    {
        Red,
        Green,
        Blue,
        Yellow,
        Black,
        Pink,
        Lime,
        Cyan,
        Orange,
        White,
        Brown,
        DarkGreen,
        DarkBlue,
        Purple,
        Gray
    }

    public float angle;

    [SerializeField]
    private Colors color;

    [SerializeField]
    private SpriteRenderer colorSprite;

    [SerializeField]
    private SpriteRenderer edgeSprite;

    [SerializeField]
    private TextMesh label;
    
    [SerializeField]
    private TextMesh pointsText;

    private int number;

    public Colors TargetColor
    {
        get
        {
            return color;
        }

        set
        {
            color = value;

            if (colorSprite != null)
            {
                colorSprite.color = GetColorValue(color);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static Colors GetColorFromSet(int setIndex, int colorIndex)
    {
        return (Colors)(setIndex * 5 + colorIndex);
    }

    public static Colors[] GetColorMix(int colorCount)
    {
        if (colorCount <= 0)
        {
            return null;
        }

        Colors[] colorMix = new Colors[colorCount];

        if (colorCount <= 5)
        {
            colorMix[0] = Colors.Red;
            if (colorCount > 1)
                colorMix[1] = Colors.Green;
            if (colorCount > 2)
                colorMix[2] = Colors.Blue;
            if (colorCount > 3)
                colorMix[3] = Colors.Yellow;
            if (colorCount > 4)
                colorMix[4] = Colors.Black;

            return colorMix;
        }

        int setIndex = 0;
        int colorIndex = 0;

        for (int i = 0; i < colorCount; i++)
        {
            colorMix[i] = GetColorFromSet(setIndex, colorIndex);

            setIndex++;
            if (setIndex > 2)
            {
                setIndex = 0;
            }

            colorIndex++;
            if (colorIndex > 4)
            {
                colorIndex = 0;
            }
        }

        return colorMix;
    }

    public void Highlight(bool highlight)
    {
        if (edgeSprite != null)
        {
            edgeSprite.color = highlight ? Color.white : Color.black;
        }
    }

    public static Color GetColorValue(Colors color)
    {
        switch (color)
        {
            case Colors.Red:
                return Color.red;
            case Colors.Green:
                return Color.green;
            case Colors.Blue:
                return new Color((20f / 255f), (80f / 255f), 1);
            case Colors.Yellow:
                return Color.yellow;
            case Colors.Black:
                return Color.black;
            case Colors.Pink:
                return new Color(1, (135f / 255f), 1);
            case Colors.Lime:
                return new Color((128f / 255f), 1, (128f / 255f));
            case Colors.Cyan:
                return Color.cyan;
            case Colors.Orange:
                return new Color(1, (128f / 255f), 0);
            case Colors.White:
                return Color.white;
            case Colors.Purple:
                return new Color((170f / 255f), (50f / 255f), (220f / 255f));
            case Colors.DarkGreen:
                return new Color(0, (100f / 255f), 0);
            case Colors.DarkBlue:
                return new Color(0, 0, (140f / 255f));
            case Colors.Brown:
                return new Color((185f / 255f), (122f / 255f), (87f / 255f));
            case Colors.Gray:
                return new Color((100f / 255f), (100f / 255f), (100f / 255f));

            default:
                return Color.magenta;
        }
    }

    public void SetNumber(int number)
    {
        this.number = number;
        label.text = number.ToString();
    }

    public void SetPoints(int points)
    {
        pointsText.text = points.ToString();
    }

    public void SetPointsVisibility(bool visible)
    {
        pointsText.gameObject.SetActive(visible);
    }
}
