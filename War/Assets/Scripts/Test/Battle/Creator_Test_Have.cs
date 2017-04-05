using UnityEngine;
using System.Collections;

public class Creator_Test_Have : MonoBehaviour {

    [HideInInspector]
    public int m_type = 0;
    ClassA m_c = null; 
	// Use this for initialization
	void Start () 
    {
        m_c = CreatorClass.CreateClass(m_type);
        m_c.Print();
        string objType = BattleObjManager.E_BATTLE_OBJECT_TYPE.SOLDIER.ToString();
        Debug.Log(objType);
	}
	
}
