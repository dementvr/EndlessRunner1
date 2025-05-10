using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
