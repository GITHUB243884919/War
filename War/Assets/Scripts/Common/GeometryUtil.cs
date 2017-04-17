/// <summary>
/// 几何工具库
/// author: fanzhengyong
/// date: 2017-03-31
/// </summary>
using UnityEngine;
using System.Collections;
namespace UF_FrameWork
{
public static class GeometryUtil
{
    public static float TOW_PI = Mathf.PI * 2f;
    public static float TwoPointAngleDeg2D(Vector3 from, Vector3 to)
    {
        float angle = 0f;

        from.y = 0f;
        to.y = 0f;
        Vector3 toTarget = to - from;
        float rad = Mathf.Acos(Mathf.Abs(toTarget.x) / toTarget.magnitude);
        angle = Mathf.Rad2Deg * rad;
        if ((toTarget.x >= 0) && (toTarget.z >= 0))
        {
        }
        else if ((toTarget.x >= 0) && (toTarget.z < 0))
        {
            angle = 360f - angle;
        }
        else if ((toTarget.x < 0) && (toTarget.z < 0))
        {
            angle = 180f + angle;
        }
        else if ((toTarget.x < 0) && (toTarget.z > 0))
        {
            angle = 180f - angle;
        }

        return angle;
    }

    /// <summary>
    /// 两个点形成的线段和世界坐标的弧度
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static float TwoPointAngleRad2D(Vector3 from, Vector3 to)
    {
        float angle = 0f;

        from.y = 0f;
        to.y = 0f;
        Vector3 toTarget = to - from;
        float rad = Mathf.Acos(Mathf.Abs(toTarget.x) / toTarget.magnitude);
        angle = rad;
        if ((toTarget.x >= 0) && (toTarget.z >= 0))
        {
        }
        else if ((toTarget.x >= 0) && (toTarget.z < 0))
        {
            angle = TOW_PI - angle;
        }
        else if ((toTarget.x < 0) && (toTarget.z < 0))
        {
            angle = Mathf.PI + angle;
        }
        else if ((toTarget.x < 0) && (toTarget.z > 0))
        {
            angle = Mathf.PI - angle;
        }

        return angle;
    }

    /// <summary>
    /// 在圆上指定一个角度的坐标
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="deg"></param>
    /// <returns></returns>
    public static Vector3 PositionInCycleByAngleDeg2D(
        Vector3 center, float radius, float deg)
    {
        Vector3 position = Vector3.zero;
        float rad = Mathf.Deg2Rad * deg;
        position = new Vector3(radius * Mathf.Cos(rad), 0f,
            radius * Mathf.Sin(rad));
        position += center;

        return position;
    }

    public static Vector3 PositionInCycleByAngleRad2D(
        Vector3 center, float radius, float rad)
    {
        Vector3 position = Vector3.zero;
        //float rad = Mathf.Deg2Rad * deg;
        position = new Vector3(radius * Mathf.Cos(rad), 0f,
            radius * Mathf.Sin(rad));
        position += center;

        return position;
    }

}

}
