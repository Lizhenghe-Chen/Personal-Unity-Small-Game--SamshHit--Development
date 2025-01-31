using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
/* Copyright (c) [2023] [Lizhneghe.Chen https://github.com/Lizhenghe-Chen]
* Please do not use these code directly without permission.
*/
public class AudioPeer : MonoBehaviour
{
    public int filterTest, filterTestValue;
    private AudioSource audioSource;
    public Transform FollowTarget, playerBrain;
    public Transform Audiovisualparent;
    public CharacterCtrl characterCtrl;
    public GameObject cubePrefab;
    //========== variables for the FFT analysis ==========
    [Header("Aduio Visualizer Settings")]
    public int num_Samples = 512;
    public FFTWindowType fftWindowType;
    public enum FFTWindowType { Blackman, Triangle, Hamming, Hanning, BlackmanHarris }
    public int channels = 1, buffer = 10;

    [Header("Below for Audio Tuple Test")]
    public Transform AuidioTestParent;
    public int multiplier = 500;

    [Header("Below for Audio Circle")]
    public bool showCircle;
    public bool isVertical;

    [Tooltip("0 < begainSameplIndex < num_Samples - endSampleMinus < num_Samples")]
    public int begainSampleIndex, endSampleMinus;
    public float cubeScale = 0.05f;
    public float radius = 5f;
    public float rangeTimer = 100;


    public float[] testSample;
    public int rangeA, rangeB;
    public float TEST;



    //============================================================================
    [Header("\n")]
    public float[] samples;
    public float[] averageSamples = new float[8];
    [SerializeField] Transform[] _cubesSamples;
    // public float BaseValue;

    void Start()
    {
        if (!characterCtrl.AudioTail) { return; }
        audioSource = GetComponent<AudioSource>();
        samples = new float[num_Samples];
        Physics.IgnoreLayerCollision(GlobalRules.instance.IgnoreHoldObjectID, GlobalRules.instance.IgnoreHoldObjectID);
        InstantiateCubes();
        // StartCoroutine(SimpleSnake());
    }

    // Update is called once per frame
    void Update()
    {
        GetAverageSpectrum();
        if (!characterCtrl.AudioTail) { return; }
        samples = GetSpectrumAudioSource(samples);
        SimpleAudioVisualize();
        if (showCircle) { SimpleCircle(); } else { SimpleFollowSnake(); }
        //AudioVisualizeRepeat();
        if (AuidioTestParent) { GetAverageSpectrum(); AudioVisualize(); }
        if (Input.GetKeyDown(KeyCode.T)) { showCircle = !showCircle; }

        //samples = GetSpectrumAudioSource(samples);
        //GetAverageSpectrum();
        //if (showCircle) { SimpleCircle(); }
        // Buffer();
    }
    // private void FixedUpdate()
    // {

    //     //else
    //     //{
    //     //    SimpleSnake();
    //     //}
    //     //TEST = GetRhythm();



    // }

    float[] GetSpectrumAudioSource(float[] samples)    //get spectrum audio source
    {
        //get spectrum data
        switch (fftWindowType)
        {
            case FFTWindowType.Blackman:
                AudioListener.GetSpectrumData(samples, channels, FFTWindow.Blackman);
                break;
            case FFTWindowType.Triangle:
                AudioListener.GetSpectrumData(samples, channels, FFTWindow.Triangle);
                break;
            case FFTWindowType.Hamming:
                AudioListener.GetSpectrumData(samples, channels, FFTWindow.Hamming);
                break;
            case FFTWindowType.Hanning:
                AudioListener.GetSpectrumData(samples, channels, FFTWindow.Hanning);
                break;
            case FFTWindowType.BlackmanHarris:
                AudioListener.GetSpectrumData(samples, channels, FFTWindow.BlackmanHarris);
                break;

        }
        return samples;

    }

    float GetRhythm()//get rhythm
    {
        testSample = GetSpectrumAudioSource(testSample);
        float average = 0;
        int count = 0;
        for (int j = rangeA; j < rangeB; j++)
        {
            average += testSample[j];
            count++;
        }
        return average /= count;
    }
    void GetAverageSpectrum()
    {
        int sampleCount = samples.Length / averageSamples.Length;
        for (int i = 0; i < averageSamples.Length - begainSampleIndex; i++)
        {
            float average = 0f;

            int count = 0;
            for (int j = sampleCount * i; j < (i + 1) * sampleCount; j++)
            {
                average += samples[j];
                count++;
            }
            average /= count;
            averageSamples[i] = average * multiplier;
        }
    }
    int AudioVisualizeCounter;
    public void AudioVisualize()
    {
        AudioVisualizeCounter = 0;
        foreach (Transform child in AuidioTestParent)
        {
            child.localScale = new Vector3(1, Mathf.Lerp(child.localScale.y, averageSamples[AudioVisualizeCounter], Time.deltaTime * buffer), 1);
            AudioVisualizeCounter++;
        }
    }

    int SimpleAudioVisualizeCounter;
    float range;
    int rangeCounter;
    public void SimpleAudioVisualize()
    {
        SimpleAudioVisualizeCounter = 0;
        if (samples[begainSampleIndex] > 3) { rangeTimer = 0.01f; } else { rangeTimer = 10; }
        foreach (Transform child in _cubesSamples)
        {
            rangeCounter = SimpleAudioVisualizeCounter + begainSampleIndex;
            range = samples[rangeCounter] *PlayerBrain.instance.shootEnergy * rangeTimer;
            //if (rangeCounter > samples.Length / 3 && range < 0.5f) { range *= rangeTimer; }
            if (range >= 3) { range *= 0.01f; }
            // if (range <= 0.1) { range *= 5; }
            range = Mathf.Clamp(range, 0.01f, 3f);
            //else if (range < 1) { range *= 10; }
            if (isVertical) { child.localScale = new Vector3(cubeScale, Mathf.Lerp(child.localScale.y, range, Time.deltaTime * buffer), cubeScale); }
            else { child.localScale = new Vector3(cubeScale, cubeScale, Mathf.Lerp(child.localScale.z, range, Time.deltaTime * buffer)); }
            SimpleAudioVisualizeCounter++;
        }
    }
    public void AudioVisualizeRepeat()
    {
        int counter = 3;
        float range;
        foreach (Transform child in _cubesSamples)
        {
            if (counter >= averageSamples.Length) { counter = 3; }
            range = averageSamples[counter] * multiplier *PlayerBrain.instance.shootEnergy;
            if (isVertical) { child.localScale = new Vector3(cubeScale, Mathf.Lerp(child.localScale.y, range, Time.deltaTime * buffer), cubeScale); }
            else { child.localScale = new Vector3(cubeScale, cubeScale, Mathf.Lerp(child.localScale.z, range, Time.deltaTime * buffer)); }
            counter++;
        }

    }
    GameObject Ins_cube;
    void InstantiateCubes()
    {
        _cubesSamples = new Transform[samples.Length - endSampleMinus - begainSampleIndex];

        float averangeEulerAng = (float)360 / (_cubesSamples.Length);
        Debug.Log(averangeEulerAng);
        for (int i = 0; i < _cubesSamples.Length; i++)
        {
            //  GameObject Ins_cube;
            Ins_cube = Instantiate(cubePrefab, this.transform.position, Quaternion.Euler(0, averangeEulerAng * i, 0));

            Ins_cube.transform.parent = Audiovisualparent;
            // if (i == 0)
            // {
            //     Ins_cube.GetComponent<Renderer>().material.color = Color.blue;
            //     Ins_cube.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.blue);

            // }
            // Ins_cube.transform.position = this.transform.position;
            // this.transform.eulerAngles = new Vector3(0, averangeEulerAng * i, 0);

            Ins_cube.name = "Cube" + i;

            _cubesSamples[i] = Ins_cube.transform;
        }
    }
    private float speed_Distance;
    void SimpleCircle()
    {
        foreach (Transform child in _cubesSamples)
        {
            //child.position = child.forward * radius + this.transform.position;
            speed_Distance = 10 / Vector3.Distance(child.position, transform.position);
            //speed_Distance = speed_Distance < 1 ? 10 : speed_Distance;
            child.position = Vector3.Lerp(
                child.position
                , (child.forward * radius + this.transform.position)
                , (speed_Distance < 1 ? 10 : speed_Distance) * Time.deltaTime);
            child.RotateAround(playerBrain.position, playerBrain.up, 20 * Time.deltaTime);
        }
    }
    private int SimpleSnakeCounter;
    IEnumerator SimpleSnake()
    {
        SimpleSnakeCounter = 0;
        while (true)
        {
            if (SimpleSnakeCounter >= _cubesSamples.Length) { SimpleSnakeCounter = 0; }
            _cubesSamples[SimpleSnakeCounter].position = FollowTarget.position;
            //if (count == 0) { _cubesSamples[count].position = FollowTarget.position; }
            //else { _cubesSamples[count].position = _cubesSamples[count - 1].position; }
            SimpleSnakeCounter++;
            yield return new WaitForSeconds(0.01f);
        }
    }
    void SimpleFollowSnake()
    {
        for (int i = 0; i < _cubesSamples.Length; i++)
        {
            //_cubesSamples[i].position = FollowTarget.position;
            if (i == 0) { _cubesSamples[i].position = Vector3.Lerp(_cubesSamples[i].position, FollowTarget.position, 20 * Time.deltaTime); }
            else { _cubesSamples[i].position = Vector3.Lerp(_cubesSamples[i].position, _cubesSamples[i - 1].position, 20 * Time.deltaTime); }
        }
    }
}