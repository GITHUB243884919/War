/// <summary>
/// 摄像机控制类
/// author: fanzhengyong
/// data:2017-06-18
/// 
/// 键盘w，s控制水平（x，y），鼠标滑轮控制y
/// </summary>

using UnityEngine;
using System.Collections;


public class SimpleCameraController : MonoBehaviour 
{
    public Vector3 m_speed = Vector3.zero;
    
    void Update() 
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        float mouse = Input.GetAxis("Mouse ScrollWheel");

        transform.Translate(new Vector3(v * m_speed.x, mouse * m_speed.y, mouse * m_speed.z)
            * Time.deltaTime, Space.World);
	}
}
