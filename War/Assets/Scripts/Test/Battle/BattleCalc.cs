using UnityEngine;
using System.Collections;

public class BattleCalc
{
    public static float Vector3AngleDeg(Vector3 from, Vector3 to)
    {
        float angle = 0f;
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

    public static float Vector3AngleRad(Vector3 from, Vector3 to)
    {
        float angle = 0f;
        Vector3 toTarget = to - from;
        float rad = Mathf.Acos(Mathf.Abs(toTarget.x) / toTarget.magnitude);
        angle = rad;
        if ((toTarget.x >= 0) && (toTarget.z >= 0))
        {
        }
        else if ((toTarget.x >= 0) && (toTarget.z < 0))
        {
            angle = Mathf.PI * 2 - angle;
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
}
