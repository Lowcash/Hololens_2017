using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpreadWaveShaderManager : MonoBehaviour
{
    private Vector4[] _contactPoints;
    private float[] _timers;
    private bool[] _enableTimers;

    private Renderer _renderer;

    private const int numberOfWaves = 10;

    public float waveLifeTime = 5.0f;
    public float timeBetweenGenerationOfWave = 1.0f;

    private void Awake()
    {
        _renderer = gameObject.GetComponent<Renderer>();

        _contactPoints = new Vector4[numberOfWaves];
        _timers = new float[numberOfWaves];
        _enableTimers = new bool[numberOfWaves];

        _enableTimers[0] = true;
    }

    void FixedUpdate()
    {
        for (int i = 0; i < numberOfWaves; i++)
        {
            if (_timers[i] > waveLifeTime)
            {
                RestartTimer(i);
            }

            if (_timers[i] > timeBetweenGenerationOfWave)
            {
                if (i + 1 < numberOfWaves)
                {
                    _enableTimers[i + 1] = true;
                }
                else
                {
                    _enableTimers[0] = true;
                }
            }

            if (_enableTimers[i])
            {
                _timers[i] += Time.deltaTime;
            }
        }

        if (_renderer != null)
        {
            _renderer.sharedMaterial.SetVectorArray("_ContactPoints", _contactPoints);
            _renderer.sharedMaterial.SetFloatArray("_Timers", _timers);
        }
    }

    private void RestartTimer(int timerIdx)
    {
        _timers[timerIdx] = 0.0f;
        _enableTimers[timerIdx] = false;
    }
}
