using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioClip mainSceneBGM;
    public AudioMixer Mixer;
    public GameObject Carrier;
    private static GameObject CarriersPrt;
    
    [Header("不会播放BGM")]
    public bool cannotPlayBGM;
    [Header("不会播放干员语音")]
    public bool cannotPlayTalk;

    private static AudioManager instance;
    
    public static AudioSource Operator;
    public static AudioSource BGM;
    public static List<AudioCarrier> EFF_List = new List<AudioCarrier>();
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

        CarriersPrt = new GameObject("音效载体队列");

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

    public static void PlayEFF(AudioClip clip, float volume = 1)
    {
        AudioCarrier carrier = null;
        if (EFF_List.Count == 0)
        {
            carrier = Instantiate(instance.Carrier, CarriersPrt.transform).GetComponent<AudioCarrier>();
        }
        else
        {
            carrier = EFF_List[EFF_List.Count - 1];
            carrier.gameObject.SetActive(true);
            EFF_List.RemoveAt(EFF_List.Count - 1);
        }

        carrier.PlayAudio(clip, volume);
    }

    public static void PlayBGM(AudioClip bgm = null)
    {
        if (instance.cannotPlayBGM) return;
        BGM.clip = bgm == null ? instance.mainSceneBGM : bgm;
        BGM.Play();
    }
}
