using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioClip mainSceneBGM;
    
    private static AudioManager instance;
    public AudioMixer Mixer;
    private static float randomTalkCoolTime = 0;
    
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
    private void Update()
    {
        if (randomTalkCoolTime > 0)
        {
            randomTalkCoolTime -= Time.deltaTime;
        }
    }

    public static void OperatorTalk(AudioClip talk)
    {
        Operator.clip = talk;
        Operator.Play();
    }
    
    public static void OperatorTalkAndClod(AudioClip talk)
    {
        Operator.clip = talk;
        Operator.Play();
        randomTalkCoolTime = 5f;
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
        BGM.clip = bgm == null ? instance.mainSceneBGM : bgm;
        BGM.Play();
    }
}
