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

    Vector4[] spheres = new Vector4[50];
    float[] xoffsets = new float[50];
    float[] yoffsets = new float[50];
    float[] zoffsets = new float[50];
    float[] xmags = new float[50];
    float[] ymags = new float[50];
    float[] zmags = new float[50];
    float time = 0;

    private void Start()
    {
        material = new Material(shader);
        cam = GetComponent<Camera>();

        for (int i = 0; i < 50; i++) {
            spheres[i] = new Vector4(0,0,0,Random.Range(0.1f, 1.0f));
            xoffsets[i] = Random.Range(0, 360);
            yoffsets[i] = Random.Range(0, 360);
            zoffsets[i] = Random.Range(0, 360);
            xmags[i] = Random.Range(0, 10f);
            ymags[i] = Random.Range(0, 10f);
            zmags[i] = Random.Range(0, 10f);
        }
    }

    private void Update()
    {
        time += Time.deltaTime;
        float val = time / 60;
        for (int i = 0; i < spheres.Length; i++) {
            spheres[i].x = xmags[i] * Mathf.Sin(val - xoffsets[i]);
            spheres[i].y = ymags[i] * Mathf.Sin(val - yoffsets[i]);
            spheres[i].z = zmags[i] * Mathf.Sin(val - zoffsets[i]);
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //List<Vector4> spheres = new List<Vector4>();
        //spheres.Add(sphere1);
        //spheres.Add(sphere2);
        //spheres.Add(sphere3);
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
