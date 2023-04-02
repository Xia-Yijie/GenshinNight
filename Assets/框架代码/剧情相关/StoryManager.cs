using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    public static StoryManager instance;

    public const int MaxTangleCharacter = 2;        // 最多同台展示的角色立绘
    
    public storyData sd_;
    public gradualChange selfChange;
    public Image BackGroundImage;
    public Sprite defaultBackGround;
    public List<GameObject> ImageObjList = new List<GameObject>();
    public List<Image> ImageList = new List<Image>();
    public Text UpText;
    public Text DownText;

    private static Color32 darkColor = new Color32(100, 100, 100, 255);
    private static int instructID;
    private static bool instructRunning;
    private static bool textRunning;
    private const float  ImageSpeed = 400;
    private const float JumpSpeed = 400;
    private const float BackgroundSpeed = 200;
    
    
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
        Show(sd_);
    }


    public void Show(storyData sd)
    {
        sd_ = sd;
        instructID = 0;
        BackGroundImage.sprite = defaultBackGround;
        Color32 light = new Color(255, 255, 255, 0);
        Color32 dark = darkColor;
        dark.a = 0;
        for (int i = 0; i < ImageObjList.Count; i++)
        {
            ImageObjList[i].SetActive(false);
            ImageList[i].color = i == 0 ? light : dark;
        }

        UpText.text = "";
        DownText.text = "";
        
        StoryInterpreter.Interpret(sd_);
        StartCoroutine(StoryCoroutine());
    }

    IEnumerator StoryCoroutine()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            if (instructID >= StoryInterpreter.instructsList.Count)
            {
                if (StoryInterpreter.InterpreterCompleted) break;
                else continue;
            }

            textRunning = false;
            instructRunning = false;
            foreach (var instruct in StoryInterpreter.instructsList[instructID])
            {// 先激活指定先激活的文字指令
                if (instruct.instruct == InstructEnum.text && instruct.arg_meanwhile)
                    StartCoroutine(Txt(instruct));
                if (instruct.instruct == InstructEnum.top && instruct.arg_meanwhile)
                    StartCoroutine(Top(instruct));
            }

            foreach (var instruct in StoryInterpreter.instructsList[instructID])
            {
                if (instruct.instruct == InstructEnum.add)
                {
                    StartCoroutine(Add(instruct));
                    while (instructRunning) yield return null;
                }
                else if (instruct.instruct == InstructEnum.light)
                {
                    StartCoroutine(Light(instruct));
                    while (instructRunning) yield return null;
                }
                else if (instruct.instruct == InstructEnum.addl)
                {
                    StartCoroutine(Addl(instruct));
                    while (instructRunning) yield return null;
                }
                else if (instruct.instruct == InstructEnum.del)
                {
                    StartCoroutine(Del(instruct));
                    while (instructRunning) yield return null;
                }
                else if (instruct.instruct == InstructEnum.swap)
                {
                    StartCoroutine(Swap(instruct));
                    while (instructRunning) yield return null;
                }
                else if (instruct.instruct == InstructEnum.rol)
                {
                    StartCoroutine(Rol(instruct));
                    while (instructRunning) yield return null;
                }
                else if (instruct.instruct == InstructEnum.jump)
                {
                    StartCoroutine(Jump(instruct));
                    while (instructRunning) yield return null;
                }
                else if (instruct.instruct == InstructEnum.back)
                {
                    StartCoroutine(Back(instruct));
                    while (instructRunning) yield return null;
                }
                else if (instruct.instruct == InstructEnum.audio)
                {
                    StartCoroutine(Audio(instruct));
                    while (instructRunning) yield return null;
                }
                else if (instruct.instruct == InstructEnum.top && !instruct.arg_meanwhile)
                {
                    StartCoroutine(Top(instruct));
                    while (instructRunning) yield return null;
                }
                else if (instruct.instruct == InstructEnum.text && !instruct.arg_meanwhile)
                {
                    StartCoroutine(Txt(instruct));
                }
            }


            while (textRunning) yield return null;
            while (!Input.GetMouseButtonDown(0)) yield return null;
            
            instructID++;
            yield return null;
        }
    }
    
    private static IEnumerator Add(StoryInstruct instruct)
    {
        instructRunning = !instruct.arg_meanwhile;
        yield return new WaitForSeconds(instruct.arg_delay);
        
        int id = instruct.arg_int_1 - 1;
        List<GameObject> objList = instance.ImageObjList;
        List<Image> imageList = instance.ImageList;
        
        // 先透明消失，换图片，再渐变出现
        objList[id].SetActive(true);
        Image image = imageList[id];
        Color32 preColor = image.color;
        preColor.a = 255;
        Color32 transparentColor = image.color;
        transparentColor.a = 0;
        while (!instruct.arg_immediately && image.color.a > 0)
        {
            image.color = ChangeColor(image.color, transparentColor, ImageSpeed * 1.5f);
            yield return null;
        }

        image.sprite = instruct.arg_sprite;
        
        while (!instruct.arg_immediately && image.color.a < 1)
        {
            image.color = ChangeColor(image.color, preColor, ImageSpeed * 1.5f);
            yield return null;
        }
        image.color = preColor;

        if (!instruct.arg_meanwhile) instructRunning = false;
    }

    private static IEnumerator Light(StoryInstruct instruct)
    {
        instructRunning = !instruct.arg_meanwhile;
        yield return new WaitForSeconds(instruct.arg_delay);
        
        int id = instruct.arg_int_1 - 1;
        List<GameObject> objList = instance.ImageObjList;
        List<Image> imageList = instance.ImageList;

        while (!instruct.arg_immediately && imageList[id].color.r < 1)
        {
            
            for (int i = 0; i < imageList.Count; i++)
            {
                Color white= Color.white;
                Color dark = darkColor;
                white.a = dark.a = imageList[i].color.a;
                imageList[i].color = ChangeColor(imageList[i].color, i == id ? white : dark, ImageSpeed);
            }
                
            yield return null;
        }
        for (int i = 0; i < imageList.Count; i++)
        {
            Color white= Color.white;
            Color dark = darkColor;
            white.a = dark.a = imageList[i].color.a;
            imageList[i].color = i == id ? white : dark;
        }

        if (!instruct.arg_meanwhile) instructRunning = false;
    }

    private static IEnumerator Addl(StoryInstruct instruct)
    {
        instructRunning = !instruct.arg_meanwhile;
        yield return new WaitForSeconds(instruct.arg_delay);
        
        int id = instruct.arg_int_1 - 1;
        List<GameObject> objList = instance.ImageObjList;
        List<Image> imageList = instance.ImageList;
        
        objList[id].SetActive(true);
        Color32 transparentColor = imageList[id].color;
        transparentColor.a = 0;
        while (!instruct.arg_immediately && imageList[id].color.a > 0)
        {
            for (int i = 0; i < imageList.Count; i++)
            {
                imageList[i].color = ChangeColor(imageList[i].color, i == id ? transparentColor : darkColor, ImageSpeed * 1.5f);
            }
            
            yield return null;
        }

        imageList[id].sprite = instruct.arg_sprite;
            
        while (!instruct.arg_immediately && imageList[id].color.a < 1)
        {
            for (int i = 0; i < imageList.Count; i++)
            {
                Color white= Color.white;
                Color dark = darkColor;
                dark.a = imageList[i].color.a;
                imageList[i].color = ChangeColor(imageList[i].color, i == id ? white : dark, ImageSpeed * 1.5f);
            }
            yield return null;
        }
        for (int i = 0; i < imageList.Count; i++)
        {
            Color white= Color.white;
            Color dark = darkColor;
            dark.a = imageList[i].color.a;
            imageList[i].color = i == id ? white : dark;
        }

        if (!instruct.arg_meanwhile) instructRunning = false;
    }

    private static IEnumerator Del(StoryInstruct instruct)
    {
        instructRunning = !instruct.arg_meanwhile;
        yield return new WaitForSeconds(instruct.arg_delay);

        int id = instruct.arg_int_1 - 1;
        List<GameObject> objList = instance.ImageObjList;
        List<Image> imageList = instance.ImageList;
        if (id >= 0 && id < instance.ImageObjList.Count)
        {
            Color32 tarColor = imageList[id].color;
            tarColor.a = 0;
            while (!instruct.arg_immediately && imageList[id].color.a > 0)
            {
                imageList[id].color = ChangeColor(imageList[id].color, tarColor, ImageSpeed);
                yield return null;
            }
            imageList[id].color = tarColor;

            objList[id].SetActive(false);
        }
        else
        {
            while (!instruct.arg_immediately && (imageList[0].color.a > 0 || imageList[1].color.a > 0)) 
            {
                foreach (var image in imageList)
                {
                    Color32 tarColor = image.color;
                    tarColor.a = 0;
                    image.color = ChangeColor(image.color, tarColor, ImageSpeed);
                }
                yield return null;
            }
            foreach (var image in imageList)
            {
                Color32 tarColor = image.color;
                tarColor.a = 0;
                image.color = tarColor;
            }

            foreach (var obj in objList)
                obj.SetActive(false);
        }

        if(!instruct.arg_meanwhile) instructRunning = false;
    }

    private static IEnumerator Swap(StoryInstruct instruct)
    {
        instructRunning = !instruct.arg_meanwhile;
        yield return new WaitForSeconds(instruct.arg_delay);
        
        List<Image> imageList = instance.ImageList;
        
        // 先将两个位置都隐去
        while (!instruct.arg_immediately && (imageList[0].color.a > 0 || imageList[1].color.a > 0))
        {
            foreach (var image in imageList)
            {
                Color32 tarColor = image.color;
                tarColor.a = 0;
                image.color = ChangeColor(image.color, tarColor, ImageSpeed);
            }
            yield return null;
        }
        
        // 交换两个位置的图像
        Sprite tmp = imageList[0].sprite;
        imageList[0].sprite = imageList[1].sprite;
        imageList[1].sprite = tmp;
        
        // 将两个位置都显示
        while (!instruct.arg_immediately && (imageList[0].color.a < 1 || imageList[1].color.a < 1))
        {
            foreach (var image in imageList)
            {
                Color32 tarColor = image.color;
                tarColor.a = 255;
                image.color = ChangeColor(image.color, tarColor, ImageSpeed);
            }
            yield return null;
        }
        
        // 如果某个位置是关闭的，将其透明度设置为0
        for (int i = 0; i < imageList.Count; i++)
        {
            if (instance.ImageObjList[i].activeSelf) continue;
            Color tarColor = imageList[i].color;
            tarColor.a = 0;
            imageList[i].color = tarColor;
        }

        if(!instruct.arg_meanwhile) instructRunning = false;
    }
    
    private static IEnumerator Rol(StoryInstruct instruct)
    {// 瞬间完成
        yield return new WaitForSeconds(instruct.arg_delay);
        int id = instruct.arg_int_1 - 1;
        Vector3 scale = instance.ImageObjList[id].transform.localScale;
        scale.x *= -1;
        instance.ImageObjList[id].transform.localScale = scale;
    }

    private static IEnumerator Jump(StoryInstruct instruct)
    {
        instructRunning = !instruct.arg_meanwhile;
        yield return new WaitForSeconds(instruct.arg_delay);
        
        int id = instruct.arg_int_1 - 1;
        List<Image> imageList = instance.ImageList;
        float tarY = 50;
        float speed = JumpSpeed;

        if (id >= 0 && id < instance.ImageObjList.Count)
        {
            Image image = imageList[id];
            while (image.transform.localPosition.y < tarY)
            {
                float y = image.transform.localPosition.y + speed * Time.deltaTime > tarY
                    ? tarY : image.transform.localPosition.y + speed * Time.deltaTime;
                Vector3 pos = image.transform.localPosition;
                pos.y = y;
                image.transform.localPosition = pos;
                yield return null;
            }
            while (image.transform.localPosition.y > 0)
            {
                float y = image.transform.localPosition.y - speed * Time.deltaTime < 0
                    ? 0 : image.transform.localPosition.y - speed * Time.deltaTime;
                Vector3 pos = image.transform.localPosition;
                pos.y = y;
                image.transform.localPosition = pos;
                yield return null;
            }
        }
        else
        {
            while (imageList[0].transform.localPosition.y < tarY)
            {
                foreach (var image in imageList)
                {
                    float y = image.transform.localPosition.y + speed * Time.deltaTime > tarY
                        ? tarY : image.transform.localPosition.y + speed * Time.deltaTime;
                    Vector3 pos = image.transform.localPosition;
                    pos.y = y;
                    image.transform.localPosition = pos;
                    
                }
                yield return null;
            }
            while (imageList[0].transform.localPosition.y > 0)
            {
                foreach (var image in imageList)
                {
                    float y = image.transform.localPosition.y - speed * Time.deltaTime < 0
                        ? 0 : image.transform.localPosition.y - speed * Time.deltaTime;
                    Vector3 pos = image.transform.localPosition;
                    pos.y = y;
                    image.transform.localPosition = pos;
                    
                }
                yield return null;
            }
        }
        
        if(!instruct.arg_meanwhile) instructRunning = false;
    }

    private static IEnumerator Back(StoryInstruct instruct)
    {
        instructRunning = !instruct.arg_meanwhile;
        yield return new WaitForSeconds(instruct.arg_delay);
        
        //先变黑，更换，再换回来
        Color32 tarColor = Color.black;
        Image image = instance.BackGroundImage;
        while (!instruct.arg_immediately && image.color.r > 0)
        {
            image.color = ChangeColor(image.color, tarColor, BackgroundSpeed);
            yield return null;
        }

        image.sprite = instruct.arg_sprite;
        
        tarColor = Color.white;
        while (!instruct.arg_immediately && image.color.r < 1)
        {
            image.color = ChangeColor(image.color, tarColor, BackgroundSpeed);
            yield return null;
        }
        image.color = tarColor;
        
        if(!instruct.arg_meanwhile) instructRunning = false;
    }

    private static IEnumerator Audio(StoryInstruct instruct)
    {
        yield return new WaitForSeconds(instruct.arg_delay);
        AudioManager.PlayEFF(instruct.arg_audio);
    }

    private static IEnumerator Top(StoryInstruct instruct)
    {
        yield return new WaitForSeconds(instruct.arg_delay);
        instance.UpText.text = instruct.arg_string_1;
    }

    
    private static IEnumerator Txt(StoryInstruct instruct)
    {
        yield return new WaitForSeconds(instruct.arg_delay);
        instance.StartCoroutine(ShowTextCoroutine(instruct.arg_string_1));
    }
    
    
    private static Color32 ChangeColor(Color32 color, Color32 tarColor, float speed)
    {
        byte tmpFunc(float x, float y)
        {
            float z;
            if (y - x > 0)
                z = x + speed * Time.deltaTime >= y
                    ? y
                    : x + speed * Time.deltaTime;
            else 
                z = x - speed * Time.deltaTime <= y
                    ? y
                    : x - speed * Time.deltaTime;
            return (byte) z;
        }

        Color32 res = new Color32();
        res.a = 1;
        res.a = tmpFunc(color.a, tarColor.a);
        res.r = tmpFunc(color.r, tarColor.r);
        res.g = tmpFunc(color.g, tarColor.g);
        res.b = tmpFunc(color.b, tarColor.b);

        return res;
    }
    
    
    
    static IEnumerator ShowTextCoroutine(string text)
    {
        textRunning = true;
        float maxDur = 0.04f;
        float t = 0;
        int i = 0;
        instance.DownText.text = "";

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                instance.DownText.text = text;
                break;
            }
            
            if (i >= text.Length) break;
            if (t <= 0)
            {
                t += maxDur;
                instance.DownText.text += text[i];
                i++;
            }

            t -= Time.deltaTime;
            yield return null;
        }

        textRunning = false;
    }
}