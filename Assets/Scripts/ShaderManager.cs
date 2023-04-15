using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
    [SerializeField] Shader shader;
    Material material;
    Camera cam;

    private void Start()
    {
        material = new Material(shader);
        cam = GetComponent<Camera>();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        
        Vector2 cameraSettings = getRenderPlaneWidthHeight();
        material.SetVector("cameraSettings", new Vector3(cameraSettings.x, cameraSettings.y, cam.nearClipPlane));
        material.SetMatrix("localToWorldMatrix", this.transform.localToWorldMatrix);
        material.SetVector("cameraPosition", this.transform.position);
        Graphics.Blit(null, destination, material);
    }

    private Vector2 getRenderPlaneWidthHeight()
    {
        float height = 2 * cam.nearClipPlane * Mathf.Tan(cam.fieldOfView / 2 / 180 * Mathf.PI);
        float width = height * cam.aspect;
        return new Vector2(width, height);
    }
}
