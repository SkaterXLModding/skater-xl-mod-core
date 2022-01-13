using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

using SXLMod.Customization;

public class SXLMultiplayerSpectator : MonoBehaviour
{
    private Camera camera;
    private HDCamera hdCam;
    private CommandBuffer m_buffer;
    private Mesh m_quad;

    private bool isEnabled = false;

    void Start()
    {
        camera = SXLMultiplayer.SpectateCamera;
        hdCam = HDCamera.GetOrCreate(camera);
        m_buffer = new CommandBuffer();

        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        m_quad = quad.GetComponent<MeshFilter>().sharedMesh;
        Destroy(quad);
    }

    void Update()
    {
        if (!isEnabled) return;

        if (!SXLMultiplayer.SpectateMat)
        {
           // m_buffer.DrawMesh(m_quad, Matrix4x4.identity, SXLMultiplayer.SpectateMat);
        }
    }

    public void EnableCamera(bool value)
    {
        camera.enabled = value;
        isEnabled = value;
    }

}
