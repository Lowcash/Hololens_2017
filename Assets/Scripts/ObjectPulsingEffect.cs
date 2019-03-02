using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPulsingEffect : MonoBehaviour {
    public enum ScaleDirection { FORWARD, BACKWARD }

    [Header("Pulse object")]
    public GameObject objectToScale;
    public GameObject objectsStencil;

    [Header("Scale settings")]
    public float fromScale = 0.0f;
    public float toScale = 1.0f;
    public float scaleDifferenceToFullTransparency = 0.2f;

    public ScaleDirection scaleDirection;

    [Header("Other properties")]
    public float speedOfTransition = 0.1f;
    
    public int countOfWaves = 5;

    private Vector3 _defaultScale;

    private float _cameraDistanceScale;
    private float _scaleBetweenWaves;

    private int _previousCountOfWaves = 5;

    private List<float> _actualScales = new List<float>();

    private List<GameObject> _scaleStencils = new List<GameObject>();
    private List<GameObject> _scaleObjects = new List<GameObject>();

    private List<ShaderAdditionalEffectManager> _objectsShaderManagers = new List<ShaderAdditionalEffectManager>();
    private List<ShaderAdditionalEffectManager> _stencilsShaderManagers = new List<ShaderAdditionalEffectManager>();

    private void Start()
    {
        GenerateObjects();
    }

    private void Update()
    {
        // check if count of waves was changed
        if (_previousCountOfWaves != countOfWaves)
        {
            GenerateObjects();

            _previousCountOfWaves = countOfWaves;
        }

        RecalculateCameraDistance();

        UpdateScale(scaleDirection);
        SetWaveTransparency(_objectsShaderManagers, scaleDirection);
        SetWaveTransparency(_stencilsShaderManagers, scaleDirection);
    }

    private void RecalculateCameraDistance()
    {
        _cameraDistanceScale = Vector3.Distance(Camera.main.transform.position, transform.position);
    }

    private void UpdateScale(ScaleDirection direction)
    {
        for (int i = 0; i < countOfWaves; i++)
        {
            _actualScales[i] += (Time.deltaTime * speedOfTransition) * (direction == ScaleDirection.FORWARD ? -1 : 1);

            // reset wave position
            switch (direction)
            {
                case ScaleDirection.FORWARD:
                    _actualScales[i] = _actualScales[i] < fromScale ? toScale : _actualScales[i];

                    break;

                case ScaleDirection.BACKWARD:
                    _actualScales[i] = _actualScales[i] > toScale ? fromScale : _actualScales[i];

                    break;
            }

            Vector3 scale = _defaultScale * _actualScales[i] * _cameraDistanceScale; ;

            _scaleObjects[i].transform.localScale = scale;
            _scaleStencils[i].transform.localScale = scale;
        }
    }

    private void SetWaveTransparency(List<ShaderAdditionalEffectManager> objectsShaderManagers, ScaleDirection direction)
    {
        for (int i = 0; i < countOfWaves; i++)
        {
            if (_actualScales[i] < fromScale + scaleDifferenceToFullTransparency)
            {
                objectsShaderManagers[i].SetTransparency(GetInterpolatedValueFromRange(fromScale, fromScale + scaleDifferenceToFullTransparency, _actualScales[i]));
            }
            if (_actualScales[i] > toScale - scaleDifferenceToFullTransparency)
            {
                objectsShaderManagers[i].SetTransparency(GetInterpolatedValueFromRange(toScale, toScale - scaleDifferenceToFullTransparency, _actualScales[i]));
            }
        }
    }

    private float GetInterpolatedValueFromRange(float minValue, float maxValue, float actualValue)
    {
        float hundredPercent = maxValue - minValue;
        float actualPercent = actualValue - minValue;

        return actualPercent / hundredPercent;
    }

    private void GenerateObjects()
    {
        DestroyGeneratedObjects();

        _defaultScale = objectToScale.transform.localScale;

        _scaleBetweenWaves = toScale / countOfWaves;

        for (int i = 0; i < countOfWaves; i++)
        {
            var scale = fromScale + (i * _scaleBetweenWaves);

            var generatedObject = Instantiate(objectToScale, transform);
            generatedObject.transform.localScale *= scale;

            var generatedStencil = Instantiate(objectsStencil, transform);
            generatedObject.transform.localScale *= scale;

            _scaleObjects.Add(generatedObject);
            _scaleStencils.Add(generatedStencil);
            _actualScales.Add(scale);
            _objectsShaderManagers.Add(generatedObject.GetComponent<ShaderAdditionalEffectManager>());
            _stencilsShaderManagers.Add(generatedStencil.GetComponent<ShaderAdditionalEffectManager>());
        }
    }

    private void DestroyGeneratedObjects()
    {
        _scaleObjects.ForEach(o => GameObject.Destroy(o));
        _scaleStencils.ForEach(o => GameObject.Destroy(o));

        _scaleObjects.Clear();
        _scaleStencils.Clear();
        _actualScales.Clear();
        _objectsShaderManagers.Clear();
        _stencilsShaderManagers.Clear();
    }
}
