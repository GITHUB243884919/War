using UnityEngine;
using System.Collections;
using UF_FrameWork;
public class Audio_Test_1 : MonoBehaviour {

	// Use this for initialization
    CharObj m_charObj = null;
	void Start () 
    {
        //AudioManager.Instance.CameraTrs = Camera.main.GetComponent<Camera>().transform;
        
        AudioManager.Instance.PlayMusic("Audio/BGM_Login", true);
        AudioManager.Instance.PlayMusic("Audio/TankMove", true);
        m_charObj = BattleObjManager.Instance.BorrowCharObj(
            BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK,
            0, 0);

        //Vector3 p = Vector3.zero;
        Vector3 tankPos = new Vector3(10f, 0, 0);

        //Debug.Log((AudioManager.Instance.CameraTrs.position - tankPos).magnitude);
        //m_charObj.AI_Attack(tankPos, tankPos, 0f);

        //m_charObj.AI_Attack(Vector3.zero, Vector3.zero, 0f);

        //StartCoroutine(NextAudio());
	}

    IEnumerator NextAudio()
    {
        yield return new WaitForSeconds(2f);
        m_charObj.AI_Attack(Vector3.zero, Vector3.zero, 0f);
        //yield return new WaitForSeconds(2f);

    }
}
