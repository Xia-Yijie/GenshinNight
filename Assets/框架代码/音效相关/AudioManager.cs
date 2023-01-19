using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioClip mainSceneBGM;
    public AudioMixer Mixer;
    
    [Header("不会播放BGM")]
    public bool cannotPlayBGM;
    [Header("不会播放干员语音")]
    public bool cannotPlayTalk;

    private static AudioManager instance;
    
    public static AudioSource Operator;
    public static AudioSource BGM;
    public static AudioSource[] EFF = new AudioSource[5];
    private static int EFFPointer = 0;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        Operator = gameObject.AddComponent<AudioSource>();
        BGM = gameObject.AddComponent<AudioSource>();
        BGM.loop = true;
        for (int i = 0; i < 5; i++)
        {
            EFF[i] = gameObject.AddComponent<AudioSource>();
        }

        Operator.outputAudioMixerGroup = instance.Mixer.FindMatchingGroups("Voice")[0];
        BGM.outputAudioMixerGroup = instance.Mixer.FindMatchingGroups("BGM")[0];
        PlayBGM();
    }

    public static void OperatorTalk(AudioClip talk)
    {
        if (instance.cannotPlayTalk) return;
        Operator.clip = talk;
        Operator.Play();
    }

    public static void PlayEFF(AudioClip eff)
    {
        for (int i = 0; i < 5; i++)
        {
            if (!EFF[EFFPointer].isPlaying)
            {
                EFF[EFFPointer].clip = eff;
                EFF[EFFPointer].Play();
                break;
            }
            EFFPointer = (EFFPointer + 1) % 5;
        }
    }

    public static void PlayBGM(AudioClip bgm = null)
    {
        if (instance.cannotPlayBGM) return;
        BGM.clip = bgm == null ? instance.mainSceneBGM : bgm;
        BGM.Play();
    }
}
