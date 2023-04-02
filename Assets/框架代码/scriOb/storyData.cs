using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New storyData",menuName = "myScript/storyData")]
public class storyData : ScriptableObject
{
    [Header("背景图片")] 
    public StringSpriteDictionary backgroundDictionary = new StringSpriteDictionary();
    [Header("角色立绘")] 
    public StringSpriteDictionary characterDictionary = new StringSpriteDictionary();
    [Header("声音字典")] 
    public StringAudioClipDictionary audioClipDictionary = new StringAudioClipDictionary();

    [Header("剧情文字")] 
    public TextAsset story;

}
