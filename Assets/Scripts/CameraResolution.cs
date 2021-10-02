using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Skrypt odpowiada za usatwienie rozdzielczosci kemerze
/// </summary>
public class CameraResolution : MonoBehaviour
{
    [SerializeField] private float multiplier = 4.0f;

    private void Update()
    {
        Camera.main.orthographicSize = (float)Screen.height / Screen.width * multiplier;
        Debug.Log(Screen.height );
        Debug.Log( Screen.width );
        Debug.Log(multiplier);
    }
}