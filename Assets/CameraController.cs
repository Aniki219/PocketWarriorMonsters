using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    PixelPerfectCamera ppcam;
    Camera cam;
    Vector3 targetLocation;
    float targetZoom;
    float ppuZoom;
    Vector3 moveVelocity;
    float zoomVel;

    private void Start()
    {
        cam = GetComponent<Camera>();
        ppcam = GetComponent<PixelPerfectCamera>();
        targetZoom = ppcam.assetsPPU;
        ppuZoom = targetZoom;
    }

    private void Update()
    {
        float maxX = 16.0f - cam.orthographicSize * 16.0f/9.0f;
        float maxY = 9.0f - cam.orthographicSize;

        float minX = -16.0f + cam.orthographicSize * 16.0f / 9.0f;
        float minY = -9.0f + cam.orthographicSize;

        float clampedX = Mathf.Clamp(targetLocation.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetLocation.y, minY, maxY);
        float clampedZ = -10;

        Vector3 clampedTargetLocation = new Vector3(clampedX, clampedY, clampedZ);

        transform.position = Vector3.SmoothDamp(transform.position, clampedTargetLocation, ref moveVelocity, 0.25f);

        ppuZoom = Mathf.Lerp(ppuZoom, targetZoom, 7*Time.deltaTime);
        ppcam.assetsPPU = Mathf.RoundToInt(ppuZoom);
    }

    public void SetTarget(Vector3 target)
    {
        targetLocation = target;
        targetZoom = 48;
    }

    public void Reset()
    {
        targetZoom = 32;
        targetLocation = Vector3.zero;
    }
}
