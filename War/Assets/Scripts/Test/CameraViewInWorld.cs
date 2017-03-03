/// <summary>
/// 画出屏幕4个点的世界坐标，用红线连接
/// 画出屏幕4个点在平面的坐标，用蓝色连接
/// 
/// fanzhengyong
/// 2017-03-02
/// </summary>

using UnityEngine;
using System.Collections;
public class CameraViewInWorld : MonoBehaviour 
{
    Vector3 m_viewInWorld_L_D = Vector3.zero;
    Vector3 m_viewInWorld_R_U = Vector3.zero;
    Vector3 m_viewInWorld_L_U = Vector3.zero;
    Vector3 m_viewInWorld_R_D = Vector3.zero;

    Vector3 m_plane_L_D       = Vector3.zero;
    Vector3 m_plane_R_U       = Vector3.zero;
    Vector3 m_plane_L_U       = Vector3.zero;
    Vector3 m_plane_R_D       = Vector3.zero;

	void Start () 
    {
        GetScreen4ToWorld();
        WorldProjectToPlane();
    }

    void GetScreen4ToWorld()
    {
        //1、World Space（世界坐标）：我们在场景中添加物体（如：Cube），他们都是以世界坐标显示在场景中的。
        //transform.position可以获得该位置坐标。

        //2、Screen Space（屏幕坐标）:以像素来定义的，以屏幕的左下角为（0，0）点，
        // 右上角为（Screen.width，Screen.height），Z的位置是以相机的世界单位来衡量的。
        //注：鼠标位置坐标属于屏幕坐标，Input.mousePosition可以获得该位置坐标，
        //手指触摸屏幕也为屏幕坐标，Input.GetTouch(0).position可以获得单个手指触摸屏幕坐标。

        //  Screen.width = Camera.pixelWidth

        //  Screen.height = Camera.pixelHeigth

        //3、ViewPort Space（视口坐标）:视口坐标是标准的和相对于相机的。相机的左下角为（0，0）点，右上角为（1，1）点，Z的位置是以相机的世界单位来衡量的。

        //4、绘制GUI界面的坐标系：这个坐标系与屏幕坐标系相似，不同的是该坐标系以屏幕的左上角为（0，0）点，右下角为（Screen.width，Screen.height）。

        //【四种坐标系的转换】

        //1、世界坐标→屏幕坐标：camera.WorldToScreenPoint(transform.position);这样可以将世界坐标转换为屏幕坐标。其中camera为场景中的camera对象。

        //2、屏幕坐标→视口坐标：camera.ScreenToViewportPoint(Input.GetTouch(0).position);这样可以将屏幕坐标转换为视口坐标。其中camera为场景中的camera对象。

        //3、视口坐标→屏幕坐标：camera.ViewportToScreenPoint();

        //4、视口坐标→世界坐标：camera.ViewportToWorldPoint();

        //左下 (0，0)
        Vector3 view_L_D = Vector3.zero;
        view_L_D.z = Mathf.Abs(Camera.main.transform.position.z);
        Debug.Log("view_L_D " + view_L_D);
        m_viewInWorld_L_D = Camera.main.ViewportToWorldPoint(view_L_D);
        Debug.Log("m_viewInWorld_L_D " + m_viewInWorld_L_D);
        GameObject obj_L_D = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj_L_D.transform.position = m_viewInWorld_L_D;

        //右上 (1, 1)
        Vector3 view_R_U = Vector3.one;
        view_R_U.z = Mathf.Abs(Camera.main.transform.position.z);
        Debug.Log("view_R_U " + view_R_U);
        m_viewInWorld_R_U = Camera.main.ViewportToWorldPoint(view_R_U);
        Debug.Log("m_viewInWorld_R_U " + m_viewInWorld_R_U);
        GameObject obj_R_U = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj_R_U.transform.position = m_viewInWorld_R_U;

        //左上 (0, 1)
        Vector3 view_L_U = Vector3.zero;
        view_L_U.y = 1f;
        view_L_U.z = Mathf.Abs(Camera.main.transform.position.z);
        Debug.Log("view_L_U " + view_L_U);
        m_viewInWorld_L_U = Camera.main.ViewportToWorldPoint(view_L_U);
        Debug.Log("m_viewInWorld_L_U " + m_viewInWorld_L_U);
        GameObject obj_L_U = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj_L_U.transform.position = m_viewInWorld_L_U;

        //左下 (1, 0)
        Vector3 view_R_D = Vector3.zero;
        view_R_D.x = 1f;
        view_R_D.z = Mathf.Abs(Camera.main.transform.position.z);
        Debug.Log("view_R_D " + view_R_D);
        m_viewInWorld_R_D = Camera.main.ViewportToWorldPoint(view_R_D);
        Debug.Log("m_viewInWorld_R_D " + m_viewInWorld_R_D);
        GameObject obj_R_D = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj_R_D.transform.position = m_viewInWorld_R_D;

    }

    void WorldProjectToPlane()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float distance = 0;

        Ray ray_L_D = Camera.main.GetComponent<Camera>().
            ScreenPointToRay(new Vector3(0f, 0f, 0f));
        if (plane.Raycast(ray_L_D, out distance))
        {
            m_plane_L_D = ray_L_D.GetPoint(distance);
            Debug.Log("Math L_D " + ray_L_D.GetPoint(distance));
        }

        Ray ray_R_U = Camera.main.GetComponent<Camera>().
            ScreenPointToRay(new Vector3(
                Camera.main.GetComponent<Camera>().pixelWidth ,
                Camera.main.GetComponent<Camera>().pixelHeight , 0f));
        if (plane.Raycast(ray_R_U, out distance))
        {
            m_plane_R_U = ray_R_U.GetPoint(distance);
            Debug.Log("Math R_U " + ray_R_U.GetPoint(distance));
        }

        Ray ray_L_U = Camera.main.GetComponent<Camera>().
            ScreenPointToRay(new Vector3(
                0,
                Camera.main.GetComponent<Camera>().pixelHeight, 0f));
        if (plane.Raycast(ray_L_U, out distance))
        {
            m_plane_L_U = ray_L_U.GetPoint(distance);
            Debug.Log("Math L_U " + ray_L_U.GetPoint(distance));
        }

        Ray ray_R_D = Camera.main.GetComponent<Camera>().
            ScreenPointToRay(new Vector3(
            Camera.main.GetComponent<Camera>().pixelWidth,
            0f, 0f));
        if (plane.Raycast(ray_R_D, out distance))
        {
            m_plane_R_D = ray_R_D.GetPoint(distance);
            Debug.Log("Math L_U " + ray_R_D.GetPoint(distance));
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_viewInWorld_L_D, 1f);
        Gizmos.DrawWireSphere(m_viewInWorld_R_U, 1f);
        Gizmos.DrawWireSphere(m_viewInWorld_L_U, 1f);
        Gizmos.DrawWireSphere(m_viewInWorld_R_D, 1f);

        Debug.DrawLine(m_viewInWorld_L_D, m_viewInWorld_L_U, Color.red);
        Debug.DrawLine(m_viewInWorld_L_U, m_viewInWorld_R_U, Color.red);
        Debug.DrawLine(m_viewInWorld_R_U, m_viewInWorld_R_D, Color.red);
        Debug.DrawLine(m_viewInWorld_R_D, m_viewInWorld_L_D, Color.red);

        Debug.DrawLine(m_plane_L_D, m_plane_L_U, Color.cyan);
        Debug.DrawLine(m_plane_L_U, m_plane_R_U, Color.cyan);
        Debug.DrawLine(m_plane_R_U, m_plane_R_D, Color.cyan);
        Debug.DrawLine(m_plane_R_D, m_plane_L_D, Color.cyan);

        //Debug.DrawLine(m_plane_L_D, m_plane_R_U, Color.green);
        
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
