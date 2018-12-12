using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPulsingEffect : MonoBehaviour {
    public GameObject objectToScale;

    public float fromScale = 5.0f;
    public float toScale = 0.0f;

    public float speedOfTransition = 0.1f;
    
    public int countOfWaves = 5;

    private Vector3 defaultScale;

    private List<float> _actualScales = new List<float>();
    private List<GameObject> _scaleObjects = new List<GameObject>();

    private void Start()
    {
        float scaleBetween = fromScale / countOfWaves;

        for (int i = 0; i < countOfWaves; i++)
        {
            _scaleObjects.Add(Instantiate(objectToScale, transform));

            _actualScales.Add(fromScale - (i * scaleBetween));
        }

        defaultScale = objectToScale.transform.localScale;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < countOfWaves; i++)
        {
            _actualScales[i] -= Time.deltaTime * speedOfTransition;

            if (_actualScales[i] < toScale)
            {
                _actualScales[i] = fromScale;
            }

            Vector3 objectScale = defaultScale * _actualScales[i];

            _scaleObjects[i].transform.localScale = objectScale;
        }
    }
}
