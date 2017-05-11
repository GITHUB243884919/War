using UnityEngine;
using System.Collections;
using System;

public class Test_LocalPosition : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        //Transform trsCube = transform.FindChild("Cube");
        //Debug.Log("local " + trsCube.localPosition
        //    + " calc world " + transform.TransformPoint(trsCube.localPosition));
        //Debug.Log("world" + trsCube.position);

        string strParam = "10ffff";
        try
        {
            int nParam = Convert.ToInt32(strParam);
            Debug.Log("33333333");
        }
        catch (Exception)
        {
            Debug.Log("参数非法");
            //throw;
        }
        
        Debug.Log(Enum.IsDefined(typeof(BattleObjManager.E_BATTLE_OBJECT_TYPE), Convert.ToInt32(strParam)));


        BattleObjManager.E_BATTLE_OBJECT_TYPE type =
            (BattleObjManager.E_BATTLE_OBJECT_TYPE)Enum.Parse(typeof(BattleObjManager.E_BATTLE_OBJECT_TYPE), strParam);

        Debug.Log(type);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
