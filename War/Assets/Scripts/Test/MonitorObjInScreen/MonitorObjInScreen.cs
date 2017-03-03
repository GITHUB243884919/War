using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonitorObjInScreen : MonoBehaviour {

	// Use this for initialization
    private Dictionary<int, CharObj> m_cache = null;
    float   m_lastTime = 0f;
    Vector3 m_offset = Vector3.zero;
    void Monitor()
    {
        foreach (KeyValuePair<int, CharObj> pair in m_cache)
        {
            CharObj obj = pair.Value;
            
            Vector3 objInScreenPos = Camera.main.GetComponent<Camera>().
                WorldToScreenPoint(obj.GameObject.transform.position);

            //Vector3 objInScreenPos = Camera.main.WorldToScreenPoint(
            //    obj.GameObject.transform.position);

            //if ((objInScreenPos.x >= 0.0f)
            //    && (objInScreenPos.x <= Camera.main.pixelWidth)
            //    && (objInScreenPos.y >= 0.0f)
            //    && (objInScreenPos.y <= Camera.main.pixelHeight)
            //    && (objInScreenPos.z > 0)
            //)
            if ((objInScreenPos.x < (0.0f - m_offset.x))
                || (objInScreenPos.x > (Camera.main.pixelWidth + m_offset.x))
                || (objInScreenPos.y < (0.0f - m_offset.y))
                || (objInScreenPos.y > (Camera.main.pixelHeight + m_offset.y))
                || (objInScreenPos.z < 0)
            )
            {
                obj.InActive();
                Debug.Log(obj.GameObject.name + " 看不到");
                Debug.Log(obj.GameObject.name + " screen " + objInScreenPos);
                Debug.Log(obj.GameObject.name + " world" + obj.GameObject.transform.position);
                Debug.Log("screen "
                    + new Vector3(
                        Camera.main.pixelWidth,
                        Camera.main.pixelHeight,
                        Camera.main.transform.position.z));
            }
            else
            {
                Debug.Log(obj.GameObject.name + " 看得到");
            }
        }
    }

    //unity
	void Start () 
    {
        m_cache = CharObjCache.Instance.Cache;
        m_lastTime = Time.realtimeSinceStartup;
        m_offset.x += 50f;
        m_offset.y += 50f;
        //Debug.Log("offset in world " + Camera.main.ScreenToWorldPoint(m_offset));
	}


    void FixedUpdate()
    {

        Monitor();
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Pressed left click.");
            Ray ray = Camera.main.GetComponent<Camera>().
                ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Physics picked " + hit.point);
            }
            else
            {
                Debug.Log("no picked");
            }
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            float distance = 0;
            if (plane.Raycast(ray, out distance))
            {
                Debug.Log("Math picked " + ray.GetPoint(distance)); 
            }
            else
            {
                Debug.Log("no picked");
            }

        }
            
        

    }
}
