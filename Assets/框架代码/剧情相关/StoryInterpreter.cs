using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEngine;

public class StoryInterpreter
{// 剧情txt指令解析器
    
    public static List<List<StoryInstruct>> instructsList = new List<List<StoryInstruct>>();
    
    private static Coroutine interpretCoroutine;   // 解析协程，如果跳过剧情则中断此协程
    private static int pointer;
    public static int line { get; private set; }
    private static storyData sd_;
    private static string story;
    private static string[] pos_name = new string[StoryManager.MaxTangleCharacter + 1];
    private static Dictionary<string, int> name_pos = new Dictionary<string, int>();
    public static bool InterpreterCompleted;


    /// <summary>
    /// 解析剧情txt，将结果放入 instructsList 中
    /// </summary>
    public static void Interpret(storyData sd)
    {
        instructsList.Clear();
        name_pos.Clear();
        pointer = 0;
        line = 1;
        sd_ = sd;
        story = sd.story.ToString();
        interpretCoroutine = StoryManager.instance.StartCoroutine(Analysis());
    }


    private static IEnumerator Analysis()
    {// 解析协程，每一帧解析一组指令

        InterpreterCompleted = false;
        while (pointer < story.Length)
        {
            List<StoryInstruct> list = new List<StoryInstruct>();
            instructsList.Add(list);
            AnalysisGroup(list);
            yield return null;
        }
        InterpreterCompleted = true;
    }

    private static bool AnalysisGroup(List<StoryInstruct> list)
    {
        bool haveText = false;
        for (; pointer < story.Length; pointer++)
        {
            if (story[pointer] == '\r' || story[pointer] == '\n')
            {
                EndLine();
                line++;
                continue;
            }

            StoryInstruct slot = new StoryInstruct();
            list.Add(slot);

            if (story.Substring(pointer, 3) == "#\r\n")
            {// 单独一行 #
                EndLine();
                pointer++;
                line++;
                return false;
            }

            // 处理前置delay，不影响后面的指令处理
            if (pointer + 6 < story.Length && story.Substring(pointer, 6) == "delay ")
                Delay_Analysis(slot);
            if (slot.error != null)
            {
                PrintError(slot.error);
                return true;
            }
            
            // 处理-i参数，不影响后面的指令
            if (pointer + 3 < story.Length && story.Substring(pointer, 3) == "-i ")
            {
                slot.arg_immediately = true;
                pointer += 3;
            }
            
            // 处理-m参数，不影响后面的指令
            if (pointer + 3 < story.Length && story.Substring(pointer, 3) == "-m ")
            {
                slot.arg_meanwhile = true;
                pointer += 3;
            }
            
            if (pointer + 4 < story.Length && story.Substring(pointer, 4) == "add ")
                Add_Analysis(slot);
            else if (pointer + 6 < story.Length && story.Substring(pointer, 6) == "light ") 
                Light_Analysis(slot);
            else if (pointer + 5 < story.Length && story.Substring(pointer, 5) == "addl ") 
                Addl_Analysis(slot);
            else if (pointer + 3 <= story.Length && story.Substring(pointer, 3) == "del")
                Del_Analysis(slot);
            else if (pointer + 4 <= story.Length && story.Substring(pointer, 4) == "swap")
                Swap_Analysis(slot);
            else if (pointer + 4 < story.Length && story.Substring(pointer, 4) == "rol ")
                Rol_Analysis(slot);
            else if (pointer + 4 <= story.Length && story.Substring(pointer, 4) == "jump")
                Jump_Analysis(slot);
            else if (pointer + 5 < story.Length && story.Substring(pointer, 5) == "back ")
                Back_Analysis(slot);
            else if (pointer + 6 < story.Length && story.Substring(pointer, 6) == "audio ")
                Aduio_Analysis(slot);
            else if (pointer + 4 < story.Length && story.Substring(pointer, 4) == "top ") 
                Top_Analysis(slot);
            else
            {
                if (haveText) slot.error = new StoryInstructError("额外的文本指令");
                haveText = true;
                Text_Analysis(slot);
            }
            EndLine();
            
            if (slot.error != null)
            {
                PrintError(slot.error);
                return true;
            }
            if (pointer >= story.Length) return false;
            line++;
        }

        return false;
    }

    private static void Add_Analysis(StoryInstruct slot)
    {
        slot.instruct = InstructEnum.add;
        if (story[pointer + 4] < '0' || story[pointer + 4] > '0' + StoryManager.MaxTangleCharacter)
        {
            slot.error = new StoryInstructError("add指令第一参数数字有误");
            return;
        }
        
        slot.arg_int_1 = (int) (story[pointer + 4] - '0');
        string name = "";
        for (pointer += 6; condition(); pointer++)
        {
            name += story[pointer];
        }

        if (!sd_.characterDictionary.ContainsKey(name))
        {
            slot.error = new StoryInstructError("add指令第二参数指定的sprite不存在：" + name);
            return;
        }
        
        slot.arg_sprite = sd_.characterDictionary[name];
        AddPos(slot.arg_int_1, name);
        EndLine();
    }

    private static void Light_Analysis(StoryInstruct slot)
    {
        slot.instruct = InstructEnum.light;
        
        if (story[pointer + 6] >= '0' && story[pointer + 6] <= '9')
        {// 跟了数字参数
            if (story[pointer + 6] > '0' + StoryManager.MaxTangleCharacter) 
            {
                slot.error = new StoryInstructError("light指令第一参数数字有误");
                return;
            }
            slot.arg_int_1 = (int) (story[pointer + 6] - '0');
        }
        else
        {// 跟了字符串参数
            string name = "";
            for (pointer += 6; condition(); pointer++)
            {
                name += story[pointer];
            }

            if (!name_pos.ContainsKey(name))
            {
                slot.error = new StoryInstructError("light指令用字符串指定的位置不存在");
                return;
            }
            slot.arg_int_1 = name_pos[name];
        }
        EndLine();
    }

    private static void Addl_Analysis(StoryInstruct slot)
    {
        slot.instruct = InstructEnum.addl;
        
        if (story[pointer + 5] < '0' || story[pointer + 5] > '0' + StoryManager.MaxTangleCharacter)
        {
            slot.error = new StoryInstructError("addl指令第一参数数字有误");
            return;
        }
        
        slot.arg_int_1 = (int) (story[pointer + 5] - '0');
        string name = "";
        for (pointer += 7; condition(); pointer++)
        {
            name += story[pointer];
        }

        if (!sd_.characterDictionary.ContainsKey(name))
        {
            slot.error = new StoryInstructError("addl指令第二参数指定的sprite不存在：" + name);
            return;
        }
        
        slot.arg_sprite = sd_.characterDictionary[name];
        AddPos(slot.arg_int_1, name);
        EndLine();
    }
    
    private static void Del_Analysis(StoryInstruct slot)
    {
        slot.instruct = InstructEnum.del;
        
        if (pointer + 3 >= story.Length || story[pointer + 3] == '\r') 
        {// 不跟任何参数，删除所有
            
            slot.arg_int_1 = -1;
            return;
        }
        
        if (story[pointer + 4] >= '0' && story[pointer + 4] <= '9')
        {// 跟了数字参数
            if (story[pointer + 4] > '0' + StoryManager.MaxTangleCharacter) 
            {
                slot.error = new StoryInstructError("del指令第一参数数字有误");
                return;
            }
            slot.arg_int_1 = (int) (story[pointer + 4] - '0');
        }
        else
        {// 跟了字符串参数
            string name = "";
            for (pointer += 4; condition(); pointer++)
            {
                name += story[pointer];
            }
           
            if (!name_pos.ContainsKey(name))
            {
                slot.error = new StoryInstructError("del指令用字符串指定的位置不存在");
                return;
            }
            slot.arg_int_1 = name_pos[name];
        }

        DelPos(slot.arg_int_1, pos_name[slot.arg_int_1]);
        EndLine();
    }

    private static void Swap_Analysis(StoryInstruct slot)
    {
        slot.instruct = InstructEnum.swap;
        SwapPos();
        EndLine();
    }

    private static void Rol_Analysis(StoryInstruct slot)
    {
        slot.instruct = InstructEnum.rol;
        
        if (story[pointer + 4] >= '0' && story[pointer + 4] <= '9')
        {// 跟了数字参数
            if (story[pointer + 4] > '0' + StoryManager.MaxTangleCharacter) 
            {
                slot.error = new StoryInstructError("rol指令第一参数数字有误");
                return;
            }
            slot.arg_int_1 = (int) (story[pointer + 4] - '0');
        }
        else
        {// 跟了字符串参数
            string name = "";
            for (pointer += 4; condition(); pointer++)
            {
                name += story[pointer];
            }

            if (!name_pos.ContainsKey(name))
            {
                slot.error = new StoryInstructError("rol指令用字符串指定的位置不存在");
                return;
            }
            slot.arg_int_1 = name_pos[name];
        }
        EndLine();
    }

    private static void Jump_Analysis(StoryInstruct slot)
    {
        slot.instruct = InstructEnum.jump;
        
        if (pointer + 4 >= story.Length || story[pointer + 4] == '\r') 
        {// 不跟任何参数，删除所有
            slot.arg_int_1 = -1;
            return;
        }
        
        if (story[pointer + 5] >= '0' && story[pointer + 5] <= '9')
        {// 跟了数字参数
            if (story[pointer + 5] > '0' + StoryManager.MaxTangleCharacter) 
            {
                slot.error = new StoryInstructError("jump指令第一参数数字有误");
                return;
            }
            slot.arg_int_1 = (int) (story[pointer + 5] - '0');
        }
        else
        {// 跟了字符串参数
            string name = "";
            for (pointer += 5; condition(); pointer++)
            {
                name += story[pointer];
            }

            if (!name_pos.ContainsKey(name))
            {
                slot.error = new StoryInstructError("jump指令用字符串指定的位置不存在");
                return;
            }
            slot.arg_int_1 = name_pos[name];
        }
        EndLine();
    }

    private static void Back_Analysis(StoryInstruct slot)
    {
        slot.instruct = InstructEnum.back;
        string name = "";
        for (pointer += 5; condition(); pointer++)
        {
            name += story[pointer];
        }

        if (name == "default")
        {
            slot.arg_sprite = StoryManager.instance.defaultBackGround;
        }
        else if (!sd_.backgroundDictionary.ContainsKey(name))
        {
            slot.error = new StoryInstructError("back指令用字符串指定的位置不存在");
            return;
        }

        slot.arg_sprite = sd_.backgroundDictionary[name];
        EndLine();
    }
    
    private static void Aduio_Analysis(StoryInstruct slot)
    {
        slot.instruct = InstructEnum.audio;
        string name = "";
        for (pointer += 6; condition(); pointer++)
        {
            name += story[pointer];
        }

        if (!sd_.audioClipDictionary.ContainsKey(name))
        {
            slot.error = new StoryInstructError("audio指令用字符串指定的位置不存在");
            return;
        }

        slot.arg_audio = sd_.audioClipDictionary[name];
        EndLine();
    }

    private static void Text_Analysis(StoryInstruct slot)
    {
        slot.instruct = InstructEnum.text;
        slot.arg_string_1 = "";
        for (; condition(); pointer++)
        {
            slot.arg_string_1 += story[pointer];
        }
        EndLine();
    }

    private static void Top_Analysis(StoryInstruct slot)
    {
        slot.instruct = InstructEnum.top;
        slot.arg_string_1 = "";
        for (pointer += 4; condition(); pointer++)
        {
            slot.arg_string_1 += story[pointer];
        }
        EndLine();
    }

    private static void Delay_Analysis(StoryInstruct slot)
    {
        pointer += 6;   // 指向后面的数字
        float delay = 0;
        bool decimals = false;
        float k = 1;

        for (; pointer < story.Length && story[pointer] != ' '; pointer++)
        {
            char ch = story[pointer];
            if (ch == '.')
            {
                decimals = true;
                continue;
            }
            
            if (ch < '0' || ch > '9')
            {
                slot.error = new StoryInstructError("delay后的数字参数错误");
                return;
            }

            if (!decimals)
            {
                delay = delay * 10 + (ch - '0');
            }
            else
            {
                k *= 0.1f;
                delay += k * (ch - '0');
            }
        }

        if (pointer >= story.Length || story[pointer] != ' ')
        {
            slot.error = new StoryInstructError("delay后未正确接上其他指令");
            return;
        }

        pointer++;
        slot.arg_delay = delay;
    }
    

    private static void AddPos(int pos, string name)
    {
        if (name_pos.ContainsKey(name)) name_pos[name] = pos;
        else name_pos.Add(name, pos);
        pos_name[pos] = name;
    }

    private static void DelPos(int pos, string name)
    {
        name_pos.Remove(name);
    }
    
    private static void SwapPos()
    {// 1号和2号换
        name_pos[pos_name[1]] = 2;
        name_pos[pos_name[2]] = 1;
        string n = pos_name[1];
        pos_name[1] = pos_name[2];
        pos_name[2] = n;
    }

    private static void PrintError(StoryInstructError error)
    {
        Debug.LogError("剧情文本解析错误：第" + error.line + "行 " + error.message);
    }

    private static bool condition()
    {
        return pointer < story.Length && story[pointer] != '\n' && story[pointer] != '\r';
    }

    private static void EndLine()
    {
        while (pointer < story.Length && story[pointer] != '\n') pointer++;
    }
    
}

public class StoryInstruct
{// 剧情指令
    public InstructEnum instruct;           // 指令
    public int arg_int_1;                   // int类型参数
    public string arg_string_1;             // string类型参数
    public Sprite arg_sprite;               // sprite类型参数
    public AudioClip arg_audio;             // AudioClip类型参数
    public float arg_delay;                 // 延迟执行参数
    public bool arg_meanwhile = false;      // 不阻塞执行参数
    public bool arg_immediately = false;    // 不执行渐变动画

    public StoryInstructError error;        // 不为null表示本条指令错误
}

public enum InstructEnum
{// 剧情指令类型
    error,
    add,
    light,
    addl,
    del,
    swap,
    rol,
    jump,
    back,
    audio,
    top,
    text,
}

public class StoryInstructError
{// 指令错误
    public int line;
    public string message;

    public StoryInstructError(string Message)
    {
        line = StoryInterpreter.line;
        message = Message;
    }
    
}