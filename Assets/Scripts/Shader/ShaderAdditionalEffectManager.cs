using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderAdditionalEffectManager : MonoBehaviour {
    private Renderer _renderer;

    public Shader shaderToModify;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void SetTransparency(float transparency)
    {
        if (_renderer.material.HasProperty("_Transparency"))
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
