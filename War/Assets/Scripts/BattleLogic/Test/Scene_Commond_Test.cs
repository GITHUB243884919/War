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
        Debug.Log("Begin click " + Time.realtimeSinceStartup);
        StartCoroutine("CreateTanks");
        //for (int i = 0; i < 128; i++)
        //{
        //    //CreateTank(i, false, false);
        //    //CreateTank(i, true, false);
        //    CreateTank(i, true, true);

        //}
        Debug.Log("End click " + Time.realtimeSinceStartup);

    }

    IEnumerator CreateTanks()
    {
        yield return null;
        for (int i = 0; i < 128; i++)
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

        //Vector3 initPos = new Vector3(0f, 0f, 0f);
        Vector3 initPos = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        CharObj obj = BattleObjManager.Instance.BorrowCharObj(
            type, serverEntityID, serverEntityType);

        obj.GameObject.transform.position = initPos;

        //
        if (withAI)
        {

            CharController ctr = obj.GameObject.GetComponent<CharController>();
            Vector3 target = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
            ctr.TargetForArrive = target;
            ctr.Commond(CharController.E_COMMOND.ARRIVE);
        }

        if (withEffect)
        {
            string effectPath = "Effect/Tank/Prefab/Tank_dapao_fire";
            GameObject effectRes = Resources.Load<GameObject>(effectPath);
            GameObject effectGo = GameObject.Instantiate<GameObject>(effectRes);
            effectGo.transform.Rotate(new Vector3(0f, -90f, 0f));
            Transform firePoint = obj.GameObject.transform.Find("Bone01/Bone02/Dummy01");
            effectGo.transform.SetParent(firePoint, false);
        }

    }

}
