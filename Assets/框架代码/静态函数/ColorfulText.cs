using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CT
{
    public static string normalBlue = "#87CEFA";
    public static string normalRed = "#FF6464";
    public static string normalGreen = "#00FF00";
    public static string PyroRed = "#FF3232";
    public static string GeoYellow = "#FFC800";
    public static string ElectroPurple = "#C832FF";
    public static string DendroGreen = "#00AF00";
    public static string HydroBlue = "#00A0FF";
    public static string CryoWhite = "#AFFFFF";
    public static string AnemoGreen = "#00FFC8";
    public static string NoneGray = "#AAAAAA";


    public static string ChangeToPercentage(float x)
    {
        x *= 100;
        return x.ToString("f0") + "%";
    }

    public static string GetColorfulText(string text, string color = "#87CEFA")
    {
        return "<color=\"" + color + "\">" + text + "</color>";
    }
    
    public static string ChangeToColorfulPercentage(float x, string color)
    {
        x *= 100;
        return GetColorfulText(x.ToString("f0") + "%", color);
    }
    
    public static string ChangeToColorfulPercentage(float x)
    {
        x *= 100;
        return GetColorfulText(x.ToString("f0") + "%", normalBlue);
    }
    
    public static string ChangeToColorfulPercentage(float x, int count)
    {
        x *= 100;
        string s = count.ToString();
        return GetColorfulText(x.ToString("f" + s) + "%", normalBlue);
    }
    
}
