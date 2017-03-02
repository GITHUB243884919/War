using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonitorObjInScreen : MonoBehaviour {

	// Use this for initialization
    private Dictionary<int, CharObj> m_cache = null;
    bool m_outofScreen = false;
	void Start () 
    {
        m_cache = CharObjCache.Instance.Cache;
        //Debug.Log("cache count " + m_cache.Count);
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void FixedUpdate()
    {
        if (m_outofScreen)
        {
            return;
        }

        //Debug.Log("cache count " + m_cache.Count);
        foreach (KeyValuePair<int, CharObj> pair in m_cache) 
        {
            CharObj obj = pair.Value;

            Vector3 objInScreenPos = Camera.main.WorldToScreenPoint(
                obj.GameObject.transform.position);

            if ((objInScreenPos.x >= 0.0f)
                && (objInScreenPos.x <= Camera.main.pixelWidth)
                && (objInScreenPos.y >= 0.0f)
                && (objInScreenPos.y <= Camera.main.pixelHeight)
                //&& (objInScreenPos.z >= (Camera.main.transform.position.z)
            )
            {
                Debug.Log(obj.GameObject.name + " 看得到");
                
            }
            else
            {
                obj.InActive();
                Debug.Log(obj.GameObject.name + " 看不到");
                Debug.Log(obj.GameObject.name + " " + objInScreenPos);
                Debug.Log("screen " 
                    + new Vector3(
                        Camera.main.pixelWidth, 
                        Camera.main.pixelHeight,
                        Camera.main.transform.position.z));
                m_outofScreen = true;

            }

        }
    }
}
