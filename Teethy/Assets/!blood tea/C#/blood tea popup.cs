using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad] // This ensures that the script is loaded when the editor starts
public class BloodTeaPopup : EditorWindow
{
    private Texture2D headerImage;
    private const string headerImagePath = "Assets/!blood tea/blood tea.png";
    private const int bannerHeight = 305; // Keep a fixed height for the banner

    private bool showCloseConfirmation = false;
    private bool showTools = false; // Toggle for tools visibility

    private static int _goCount;
    private static int _componentsCount;
    private static int _missingCount;

    // Path to the MP3 file
    private const string audioFilePath = "Assets/!blood tea/audio menu.mp3"; // Change this to your audio file path

    // Audio Source reference
    private AudioSource audioSource;

    // Static constructor to hook into the editor startup
    static BloodTeaPopup()
    {
        // Subscribe to the EditorApplication update event
        EditorApplication.update += ShowPopupOnStart;
    }

    private static void ShowPopupOnStart()
    {
        // Open the popup the first time the editor updates (i.e., on project load)
        EditorApplication.update -= ShowPopupOnStart; // Ensure it only runs once
        OpenPopup();
    }

    [MenuItem("Blood Tea Menu/public")]
    public static void OpenPopup()
    {
        BloodTeaPopup window = ScriptableObject.CreateInstance<BloodTeaPopup>();
        window.LoadImage();
        window.titleContent = new GUIContent("Tea Menu, or... Tea-mu!");
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 1080, bannerHeight + 600); // Increased Y-axis for more space
        window.Show();
    }

    private void LoadImage()
    {
        headerImage = LoadImageFromFile(headerImagePath);
    }

    private Texture2D LoadImageFromFile(string path)
    {
        Texture2D texture = new Texture2D(2, 2);

        if (File.Exists(path))
        {
            byte[] fileData = File.ReadAllBytes(path);
            if (texture.LoadImage(fileData))
            {
                Debug.Log("Successfully loaded image: " + path);
                return texture;
            }
        }

        Debug.LogError("Failed to load image from: " + path);
        return null;
    }

    void OnGUI()
    {
        // Set the dark background color
        GUI.color = new Color(0.1f, 0.1f, 0.1f); // Dark gray background
        GUI.DrawTexture(new Rect(0, 0, position.width, position.height), Texture2D.whiteTexture); // Background
        GUI.color = Color.white; // Reset GUI color for other elements

        // Header Image
        GUILayout.BeginVertical(GUI.skin.box);

        if (headerImage != null)
        {
            // Scale the header image based on the window's width
            float aspectRatio = (float)headerImage.height / headerImage.width;
            float scaledHeight = position.width * aspectRatio;
            GUILayout.Label(headerImage, GUILayout.Width(position.width), GUILayout.Height(scaledHeight));
        }
        else
        {
            EditorGUILayout.HelpBox("Failed to load header image.", MessageType.Warning);
        }

        // Play Audio Button at the top right
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // Push the button to the right
        
if (GUILayout.Button("Public 1.2.7", GUILayout.Width(100), GUILayout.Height(40)))
        {
            Play2DAudio(audioFilePath);
        }
        GUILayout.EndHorizontal();

        // Text
        GUILayout.Space(10);
        GUILayout.Label("Hey, I'm Blood Tea, I make avatars, assets, C# and more!", EditorStyles.boldLabel);
        GUILayout.Label("I try to keep as much of my assets free as I can, so any support really helps.");
        GUILayout.Label("Feel free to visit my socials!");

        GUILayout.Space(10);

        if (GUILayout.Button("Blood Tea Website", GUILayout.ExpandWidth(true))) // Expand width to fit
        {
            Application.OpenURL("https://guns.lol/blood_tea");
        }

        if (GUILayout.Button("Join my discord for more or community help!", GUILayout.ExpandWidth(true))) // Expand width to fit
        {
            Application.OpenURL("https://discord.gg/e8TAHEx2NY");
        }

        // Close Button
        GUILayout.Space(20);
        GUI.backgroundColor = Color.magenta;
        if (GUILayout.Button("OK Blood Tea, I get it, get the fuck off my screen!", GUILayout.Height(40), GUILayout.ExpandWidth(true))) // Expand width to fit
        {
            showCloseConfirmation = true;
        }
        GUI.backgroundColor = Color.white;

        GUILayout.EndVertical();

        // Tools Section
        GUILayout.Space(20);
        if (GUILayout.Button("Tools", GUILayout.Height(30), GUILayout.ExpandWidth(true))) // Toggle button for Tools
        {
            showTools = !showTools; // Toggle visibility of tools
        }

        if (showTools) // Only show tools if toggled on
        {
            GUILayout.BeginVertical(GUI.skin.box); // Vertical box for tools
            GUILayout.Label("Available Tools", EditorStyles.boldLabel);
            GUILayout.Space(10); // Give some space before the buttons

            GUILayout.BeginHorizontal();

            // Calculate button width based on the window's width and number of buttons
            float buttonWidth = (position.width - 30) / 4; // Adjusting for spacing

            // Button 1: Material Shader Swap
            if (GUILayout.Button("Force Swap to Standard Shader", GUILayout.Width(buttonWidth), GUILayout.Height(40)))
            {
                SwapShaderToStandard();
            }

            // Button 2: Remove All Scripts
            if (GUILayout.Button("Remove All Scripts", GUILayout.Width(buttonWidth), GUILayout.Height(40)))
            {
                RemoveAllScripts();
            }

            // Button 3: Remove Missing (Mono Script)
            if (GUILayout.Button("Remove Missing Scripts (MIP)", GUILayout.Width(buttonWidth), GUILayout.Height(40)))
            {
                RemoveMissingMonoScripts();
            }

            // New Button: Private
            if (GUILayout.Button("Private stuff", GUILayout.Width(buttonWidth), GUILayout.Height(40)))
            {
                // for the Private menu 
            }


            GUILayout.EndHorizontal();

            GUILayout.EndVertical(); // End vertical layout for tools
        }

        if (showCloseConfirmation)
        {
            ShowCloseConfirmation();
        }
    }

    // Method for playing 2D audio
    private void Play2DAudio(string path)
    {
        // Check if the audio file exists
        if (File.Exists(path))
        {
            // Load the audio clip from the file
            var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (audioClip != null)
            {
                // Create an AudioSource and set it up as 2D
                GameObject audioObject = new GameObject("TempAudio");
                audioSource = audioObject.AddComponent<AudioSource>();
                audioSource.clip = audioClip;
                audioSource.spatialBlend = 0f; // Set to 2D audio
                audioSource.Play();

                Debug.Log("Playing 2D audio: " + path);
            }
            else
            {
                Debug.LogError("Failed to load audio clip: " + path);
            }
        }
        else
        {
            Debug.LogError("Audio file not found: " + path);
        }
    }

    private void SwapShaderToStandard()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject obj in selectedObjects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null && renderer.sharedMaterial != null)
            {
                Material mat = renderer.sharedMaterial;
                mat.shader = Shader.Find("Standard"); // Swap shader to Standard
                Debug.Log($"Swapped shader to Standard on {obj.name}");
            }
        }
    }

    private void RemoveAllScripts()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject obj in selectedObjects)
        {
            Component[] components = obj.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (!(component is Transform))
                {
                    DestroyImmediate(component);
                }
            }
        }
    }

    private void RemoveMissingMonoScripts()
    {
        GameObject[] goArray = Selection.gameObjects;
        _goCount = 0;
        _componentsCount = 0;
        _missingCount = 0;
        foreach (GameObject g in goArray)
        {
            _goCount++;
            Component[] components = g.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                _componentsCount++;
                if (components[i] == null)
                {
                    _missingCount++;
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(g);
                    Debug.Log($"Removed missing script from {g.name}");
                }
            }
        }

        Debug.Log($"Searched {_goCount} GameObjects, {_componentsCount} components, found {_missingCount} missing");
    }

    private void ShowCloseConfirmation()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("Well... that was rude... TwT", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("good! now fuck off.."))
        {
            Close();
        }

        if (GUILayout.Button("im sorry! I wanna stay on the menu"))
        {
            showCloseConfirmation = false;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
}
