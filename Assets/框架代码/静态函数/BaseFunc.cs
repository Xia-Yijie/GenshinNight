using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BaseFunc
{
    public const float eps = 1e-3f;
    public const float highOper_y = 0.25f;
    public const float operAnimFix_z = -0.2f;
    
    public static Vector2 xz(Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    public static float xz_Distance(Vector3 a, Vector3 b)
    {
        return Vector2.Distance(xz(a), xz(b));
    }
    
    public static float FixCoordinate(float x)
    {
        return x > 0 ? (int) x + 0.5f : (int) x - 0.5f;
    }
    
    public static Vector2 FixCoordinate(Vector2 vec)
    {
        vec.x = vec.x > 0 ? (int) vec.x + 0.5f : (int) vec.x - 0.5f;
        vec.y = vec.y > 0 ? (int) vec.y + 0.5f : (int) vec.y - 0.5f;
        return vec;
    }
    
    public static Vector2 FixCoordinate(Vector3 vec)
    {
        vec.x = vec.x > 0 ? (int) vec.x + 0.5f : (int) vec.x - 0.5f;
        vec.y = vec.z > 0 ? (int) vec.z + 0.5f : (int) vec.z - 0.5f;
        return (Vector2)vec;
    }

    public static Vector3 x0z(Vector2 vec)
    {
        return new Vector3(vec.x, 0, vec.y);
    }

    public static bool Equal(Vector2 x, Vector2 y)
    {
        return (Mathf.Abs(x.x - y.x) < eps) && (Mathf.Abs(x.y - y.y) < eps);
    }

    public static bool preEqual(Vector3 x, Vector3 y)
    {
        return (Mathf.Abs(x.x - y.x) < eps) && (Mathf.Abs(x.z - y.z) < eps);
    }
    
    public static bool almostZero(Vector3 a)
    {
        return Mathf.Abs(a.x) < 1e-3 && Mathf.Abs(a.y) < 1e-3 && Mathf.Abs(a.z) < 1e-3;
    }

    public static bool isFront(Vector3 pos, Vector3 tarPos, Vector3 directionVector)
    {// 判断tarPos是否在pos根据direction向量的正前方
        Vector3 tmp = tarPos - pos;
        if (directionVector.x == 0 && tmp.x != 0) return false;
        if (directionVector.x > 0 && tmp.x < 0) return false;
        if (directionVector.x < 0 && tmp.x > 0) return false;
        if (directionVector.z == 0 && tmp.z != 0) return false;
        if (directionVector.z > 0 && tmp.z < 0) return false;
        if (directionVector.z < 0 && tmp.z > 0) return false;
        return true;
    }
}
