using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
    [SerializeField] Shader shader;
    [SerializeField] Vector3 fogColor;
    [SerializeField] float ambientOcclusion;
    Material material;
    Camera cam;

    Vector4 sphere1 = new Vector4(0,0,0,1);
    Vector4 sphere2 = new Vector4(0,0,3,1.5f);
    Vector4 sphere3 = new Vector4(0, 0, -2, 0.75f);
    float time = 0;

    private void Start()
    {
        material = new Material(shader);
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        time += Time.deltaTime;
        sphere1.x = Mathf.Cos(time/3);
        sphere1.y = Mathf.Cos(time/3);
        sphere2.x = Mathf.Sin(time/3);
        sphere2.y = Mathf.Sin(time/3);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        List<Vector4> spheres = new List<Vector4>();
        spheres.Add(sphere1);
        spheres.Add(sphere2);
        spheres.Add(sphere3);
        Vector2 cameraSettings = getRenderPlaneWidthHeight();
        material.SetVector("cameraSettings", new Vector3(cameraSettings.x, cameraSettings.y, cam.nearClipPlane));
        material.SetMatrix("localToWorldMatrix", this.transform.localToWorldMatrix);
        material.SetVector("cameraPosition", this.transform.position);
        material.SetFloat("ambientOcclusion", ambientOcclusion);
        material.SetVectorArray("spheres", spheres);
        Graphics.Blit(null, destination, material);
    }

    private Vector2 getRenderPlaneWidthHeight()
    {
        float height = 2 * cam.nearClipPlane * Mathf.Tan(cam.fieldOfView / 2 / 180 * Mathf.PI);
        float width = height * cam.aspect;
        return new Vector2(width, height);
    }
}
