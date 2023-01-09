using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitch : MonoBehaviour
{
    public static SceneSwitch instance;

    public gradualChange transitionCanvas;
    public GameObject levelInformation;
    public Text levelNum;
    public Text levelName;
    public Slider loadSlider;
    public Text downTip;

    private const float MinSwitchTime = 0.5f;
    private float maxProgress;
    
    private AsyncOperation async = null;        // 异步加载场景时的变量
    
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

    public static void LoadScene(string sceneName, levelData LD = null)
    {
        instance.transitionCanvas.Show();
        instance.maxProgress = 0;
        instance.StartCoroutine(nameof(LoadLeaver), sceneName);
        
        // 改变BGM
        AudioManager.PlayBGM(LD == null ? null : LD.BGM);
    }

    IEnumerator LoadLeaver(string sceneName)
    {
        async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;
        while (!async.isDone)
        {
            float progressValue = async.progress < 0.9f ? async.progress : 1f;
            maxProgress += Time.deltaTime / MinSwitchTime;
            progressValue = Mathf.Min(progressValue, maxProgress);
            loadSlider.value = progressValue;

            if (progressValue >= 1f) {
                transitionCanvas.Hide();
                async.allowSceneActivation = true;
                if (sceneName == "主界面")
                {
                    InitManager.Clear();
                    BuffManager.Clear();
                    PoolManager.Clear();
                }
            }
            yield return null;
        }
    }
    
}
