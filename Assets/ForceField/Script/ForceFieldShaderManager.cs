using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldShaderManager : MonoBehaviour {
    private Renderer _renderer;

    public Shader forceFieldShader;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void SetTransparency(float transparency)
    {
        if (forceFieldShader.name.Contains("ForceField"))
        {
            _renderer.material.SetFloat("_Transparency", transparency);
        }
        else
        {

#if UNITY_EDITOR
            Debug.Log("Wrong shader was set!");
#endif
        }
    }
}
