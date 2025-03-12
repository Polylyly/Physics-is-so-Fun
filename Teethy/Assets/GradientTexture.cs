using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GradientTexture : MonoBehaviour
{
    [Header("Type of Gradient")]
    [Tooltip("0 = downward to black")]
    public int type = 0;
    public Color bloodColour, toothColour;

    public void ColourTooth()
    {
        Debug.Log("chomp");

        float width = transform.lossyScale.x;
        float height = transform.lossyScale.y;

        Gradient gradient = new Gradient();
        Texture2D texture = new Texture2D(Mathf.CeilToInt(width), Mathf.CeilToInt(height));
        texture.alphaIsTransparency = true;

        List<GradientColorKey> colourList = new List<GradientColorKey>();
        GradientColorKey bloodKey = new GradientColorKey(Color.white, 0); //Change 0 to slider from bite
        colourList.Add(bloodKey);
        GradientColorKey toothKey = new GradientColorKey(Color.black, 1); //Change 1 to slider from bite?
        colourList.Add(toothKey);
        GradientColorKey[] colours = colourList.ToArray();

        List<GradientAlphaKey> alphaList = new List<GradientAlphaKey>();
        GradientAlphaKey alphaKey = new GradientAlphaKey(1, 0);
        alphaList.Add(alphaKey);
        GradientAlphaKey alphaKey2 = new GradientAlphaKey(0, 1);
        alphaList.Add(alphaKey2);
        GradientAlphaKey[] alphas = alphaList.ToArray();

        List<Color> colorsList = new List<Color>();
        colorsList.Add(bloodColour);
        colorsList.Add(toothColour);
        Color[] colors = colorsList.ToArray();

        switch (type)
        {
            case (0):
                {
                    gradient.SetKeys(colours, alphas);

                    float yStep = 1F / height;

                    print("Height: " + height);
                    print("yStep: " + yStep);

                    texture.SetPixels(colors);

                    break;
                }
        }

        GetComponent<Renderer>().material.SetTexture("_Gradient", texture);
        texture.Apply();
    }

}