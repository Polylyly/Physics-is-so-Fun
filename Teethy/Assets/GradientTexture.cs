using UnityEngine;
using System.Collections;

public class GradientTexture : MonoBehaviour
{
    [Header("Type of Gradient")]
    [Tooltip("0 = downward to black")]
    public int type = 0;
    public Color bloodColour, toothColour;

    void Start()
    {
        float width = transform.lossyScale.x;
        float height = transform.lossyScale.y;

        Gradient gradient = new Gradient();
        Texture2D texture = new Texture2D(Mathf.CeilToInt(width), Mathf.CeilToInt(height));
        texture.alphaIsTransparency = true;



        switch (type)
        {
            case (0):
                {
                    gradient.SetKeys();

                    float yStep = 1F / height;

                    print("Height: " + height);
                    print("yStep: " + yStep);

                    for (int y = 0; y < Mathf.CeilToInt(height); y++)
                    {
                        Color color = gradient.Evaluate(y * yStep);
                        print("Y: " + y + " | R: " + color.r + " G: " + color.g + " B: " + color.b + " A: " + color.a);

                        for (int x = 0; x < Mathf.CeilToInt(width); x++)
                        {
                            texture.SetPixel(Mathf.CeilToInt(x), Mathf.CeilToInt(y), color);
                        }
                    }

                    break;
                }
        }

        GetComponent<Renderer>().material.mainTexture = texture;
    }
}