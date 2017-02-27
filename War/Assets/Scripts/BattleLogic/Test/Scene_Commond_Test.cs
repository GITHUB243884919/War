using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Scene_Commond_Test : MonoBehaviour
{

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClick);
    }



    public void BtnClick()
    {
        //Debug.Log("Begin click " + Time.realtimeSinceStartup);
        StartCoroutine("CreateTanks");
        //for (int i = 0; i < 128; i++)
        //{
        //    //CreateTank(i, false, false);
        //    //CreateTank(i, true, false);
        //    CreateTank(i, true, true);

        //}
        //Debug.Log("End click " + Time.realtimeSinceStartup);

    }

    IEnumerator CreateTanks()
    {
        yield return null;
        //for (int i = 0; i < 128; i++)
        for (int i = 0; i < 1; i++)
        {
            //CreateTank(i, false, false);
            //CreateTank(i, true, false);
            CreateTank(i, true, true);

        }
    }
    void CreateTank(int entityID, bool withAI, bool withEffect)
    {
        BattleObjManager.E_BATTLE_OBJECT_TYPE type
            = BattleObjManager.E_BATTLE_OBJECT_TYPE.TANK;
        int serverEntityID = entityID;
        int serverEntityType = 1;

        Vector3 startPos = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        Vector3 endPos   = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        //假定服务器规定这段路必须time秒走完
        float   time     = 30;
        float   speed    = (endPos - startPos).magnitude / time;
        CharObj obj      = BattleObjManager.Instance.BorrowCharObj(
            type, serverEntityID, serverEntityType);
        //
        CharController cctr = null;
        if (withAI)
        {
            cctr = obj.GameObject.GetComponent<CharController>();
            cctr.Arrive(startPos, endPos, speed);
        }

        if (withEffect)
        {
            //string effectPath = "Effect/Tank/Prefab/Tank_dapao_fire";
            //GameObject effectRes = Resources.Load<GameObject>(effectPath);
            //GameObject effectGo = GameObject.Instantiate<GameObject>(effectRes);
            //effectGo.transform.Rotate(new Vector3(0f, -90f, 0f));
            //Transform firePoint = obj.GameObject.transform.Find("Bone01/Bone02/Dummy01");
            //effectGo.transform.SetParent(firePoint, false);

            //string effectPath = "Effect/Tank/Prefab/Tank_dapao_fire";
            //GameObject particleObj = null;
            //Transform trs = obj.GameObject.transform.Find("Bone01/Bone02/Dummy01/Tank_dapao_fire");
            //if (trs == null)
            //{
            //    particleObj = BattleObjManager.Instance.BorrowParticleObj(effectPath);
            //    particleObj.transform.Rotate(new Vector3(0f, -90f, 0f));
            //    Transform firePoint = obj.GameObject.transform.Find("Bone01/Bone02/Dummy01");
            //    particleObj.transform.SetParent(firePoint, false);
            //}

            //cctr.Commond(CharController.E_COMMOND.ATTACK);
            cctr.Attack();


        }

    }

}
