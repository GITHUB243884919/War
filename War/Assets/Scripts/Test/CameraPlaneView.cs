using UnityEngine;
using System.Collections;
using System;

public class CameraPlaneView : MonoBehaviour
{
    #region for debug
    public Camera viewCamera;

    void Start()
    {
        Debug.Log(Vector3.up);
    }

    //void Update()
    void OnDrawGizmos()
    {
        Vector3[] corners;
        if (GetPlaneCorners(Vector3.up, Vector3.zero, viewCamera, out corners))
        {
            Debug.Log("有交点");
            Debug.DrawLine(corners[0], corners[1], Color.red);    // bottom
            Debug.DrawLine(corners[1], corners[2], Color.red);    // right
            Debug.DrawLine(corners[2], corners[3], Color.red);    // top
            Debug.DrawLine(corners[3], corners[0], Color.red);    // left
        }
        else
        {
            Debug.Log("没有交点");
        }
    }
    #endregion


    /// <summary>
    /// 获取摄像机在平面上的视野范围的4个角的顶点
    /// </summary>
    /// <param name="normal">平面法线</param>
    /// <param name="planePoint">平面上的一点</param>
    /// <param name="camera">摄像机</param>
    /// <param name="corners">返回4个角的顺序：左下、右下、右上、左上</param>
    /// <returns>摄像机与平面是否完全相交</returns>
    public static bool GetPlaneCorners(Vector3 normal, Vector3 planePoint, Camera camera, out Vector3[] corners)
    {
        Plane plane = new Plane(normal, planePoint);
        return GetPlaneCorners(ref plane, camera, out corners);
    }

    /// <summary>
    /// 获取摄像机在平面上的视野范围的4个角的顶点
    /// </summary>
    /// <param name="plane">平面结构体</param>
    /// <param name="camera">摄像机</param>
    /// <param name="corners">返回4个角的顺序：左下、右下、右上、左上</param>
    /// <returns>摄像机与平面是否完全相交</returns>
    public static bool GetPlaneCorners(ref Plane plane, Camera camera, out Vector3[] corners)
    {
        Ray rayBL = camera.ViewportPointToRay(new Vector3(0, 0, 1));     // bottom left
        Ray rayBR = camera.ViewportPointToRay(new Vector3(1, 0, 1));     // bottom right
        Ray rayTL = camera.ViewportPointToRay(new Vector3(0, 1, 1));     // top left
        Ray rayTR = camera.ViewportPointToRay(new Vector3(1, 1, 1));     // top right

        corners = new Vector3[4];
        if (!GetRayPlaneIntersection(ref plane, rayBL, out corners[0])
            || !GetRayPlaneIntersection(ref plane, rayBR, out corners[1])
            || !GetRayPlaneIntersection(ref plane, rayTR, out corners[2])
            || !GetRayPlaneIntersection(ref plane, rayTL, out corners[3]))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 获取平面与射线的交点
    /// </summary>
    /// <param name="plane">平面结构体</param>
    /// <param name="ray">射线</param>
    /// <param name="intersection">返回交点</param>
    /// <returns>是否相交</returns>
    public static bool GetRayPlaneIntersection(ref Plane plane, Ray ray, out Vector3 intersection)
    {
        float enter;
        if (!plane.Raycast(ray, out enter))
        {
            intersection = Vector3.zero;
            return false;
        }

        // 下面是获取t的公式
        // 注意，你需要先判断射线与平面是否平行，如果平面和射线平行，那么平面法线和射线方向的点积为0，即除数为0.
        //float t = (Vector3.Dot(normal, planePoint) - Vector3.Dot(normal, ray.origin)) / Vector3.Dot(normal, ray.direction.normalized);
        if (enter >= 0)
        {
            intersection = ray.origin + enter * ray.direction.normalized;
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

    /// <summary>
    /// 获取平面与射线的交点
    /// </summary>
    /// <param name="normal">平面法线</param>
    /// <param name="planePoint">平面上的一点</param>
    /// <param name="ray">射线</param>
    /// <param name="intersection">返回交点</param>
    /// <returns>是否相交</returns>
    public static bool GetRayPlaneIntersection(Vector3 normal, Vector3 planePoint, Ray ray, out Vector3 intersection)
    {
        Plane plane = new Plane(normal, planePoint);
        return GetRayPlaneIntersection(ref plane, ray, out intersection);
    }
}