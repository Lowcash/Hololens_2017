using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPulsingEffect : MonoBehaviour {
    public enum ScaleDirection { FORWARD, BACKWARD }

    [Header("Pulse object")]
    public GameObject objectToScale;

    [Header("Scale range")]
    public float fromScale = 0.0f;
    public float toScale = 1.0f;

    public ScaleDirection scaleDirection;

    [Header("Other properties")]
    public float speedOfTransition = 0.1f;
    
    public int countOfWaves = 5;

    private Vector3 _defaultScale;

    private float _scaleBetweenWaves;

    private List<float> _actualScales = new List<float>();
    private List<GameObject> _scaleObjects = new List<GameObject>();

    private void Start()
    {
        _defaultScale = objectToScale.transform.localScale;

        _scaleBetweenWaves = toScale / countOfWaves;

        for (int i = 0; i < countOfWaves; i++)
        {
            var scale = fromScale + (i * _scaleBetweenWaves);

            var generatedObject = Instantiate(objectToScale, transform);
            generatedObject.transform.localScale *= scale;

            _scaleObjects.Add(generatedObject);
            _actualScales.Add(scale);
        }
    }

    private void Update()
    {
        var direction = scaleDirection == ScaleDirection.FORWARD ? 1 : -1;

        for (int i = 0; i < countOfWaves; i++)
        {
            _actualScales[i] += direction * (Time.deltaTime * speedOfTransition);

            switch (scaleDirection)
            {
                case ScaleDirection.BACKWARD:
                    _actualScales[i] = _actualScales[i] < fromScale ? toScale : _actualScales[i];

                    break;

                case ScaleDirection.FORWARD:
                    _actualScales[i] = _actualScales[i] > toScale ? fromScale : _actualScales[i];

                    break;
            }

            Vector3 objectScale = _defaultScale * _actualScales[i];

            _scaleObjects[i].transform.localScale = objectScale;
        }
    }
}
