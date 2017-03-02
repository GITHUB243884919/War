using UnityEngine;
using System.Collections;

/// <summary>
/// 判断物体是否在摄像机范围内
/// https://zhidao.baidu.com/question/500320586435589844.html
/// fanzhengyong
/// 2017-03-02
/// </summary>
public class ObjInCamera : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        Vector3 objInScreenPos =  Camera.main.WorldToScreenPoint(transform.position);
        Debug.Log(gameObject.name + " objInScreenPos " + objInScreenPos);
        Debug.Log(gameObject.name + " screen width " + Camera.main.pixelWidth);
        Debug.Log(gameObject.name + " screen height " + Camera.main.pixelHeight);
        Vector3 objInViewPos = Camera.main.ScreenToViewportPoint(objInScreenPos);
        Debug.Log(gameObject.name + " objInViewPos " + objInViewPos);

        if ((objInViewPos.x >= 0.0f)
            && (objInViewPos.x <= 1f)
            && (objInViewPos.y >= 0.0f)
            && (objInViewPos.y <= 1f)
            && (objInViewPos.z >= Camera.main.transform.position.z)
        )
        {
            Debug.Log(gameObject.name + " 看得到 方法1");
        }
        else
        {
            Debug.Log(gameObject.name + " 看不到 方法1");
        }

        if ((objInScreenPos.x >= 0.0f)
            && (objInScreenPos.x <= Camera.main.pixelWidth)
            && (objInScreenPos.y >= 0.0f)
            && (objInScreenPos.y <= Camera.main.pixelHeight)
            && (objInScreenPos.z >= Camera.main.transform.position.z)
        )
        {
            Debug.Log(gameObject.name + " 看得到 方法1");
        }
        else
        {
            Debug.Log(gameObject.name + " 看不到 方法1");
        }
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        //Camera.pixelWidth
        //Camera.main.WorldToScreenPoint(transform.position);
        //camera.WorldToScreenPoint(enemy.transform.position)
	
	}

    void FixedUpdate()
    {
        //Vector3 objInScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        //if (objInScreenPos.x < Camera.main.pixelWidth.x)

    }
}
