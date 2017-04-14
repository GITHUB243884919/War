using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if _LOG_MEDIATOR_
using Debug = LogMediator;
#endif

public class Res_Test_2 : MonoBehaviour 
{

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClick);
    }

    public void BtnClick()
    {
        Debug.Log("BtnClick");
        ResourcesManager2.Instance.Init();

        Material m = ResourcesManager2.Instance.LoadAssetSync<Material>(
            "Resources/RuntimeMeshBaker/M_Arm_Engineercorps/M_Arm_Engineercorps-mat");
        if (m != null)
        {
            Debug.Log("m != null " + m.GetInstanceID());
        }

        GameObject go_1 = ResourcesManager2.Instance.LoadAssetSync<GameObject>(
            "Resources/RuntimeMeshBaker/M_Arm_Tank/M_Arm_Tank_ORG");
        if (go_1 != null)
        {
            Debug.Log("go_1 != null " + go_1.GetInstanceID());
        }
        GameObject tank_1 = GameObject.Instantiate<GameObject>(go_1);
        Debug.Log("tank_1 " + tank_1.GetInstanceID());

        GameObject go_2 = ResourcesManager2.Instance.LoadAssetSync<GameObject>(
            "Resources/RuntimeMeshBaker/M_Arm_Tank/M_Arm_Tank_ORG");
        if (go_1 != null)
        {
            Debug.Log("go_2 != null " + go_1.GetInstanceID());
        }
        GameObject tank_2 = GameObject.Instantiate<GameObject>(go_2);
        Debug.Log("tank_2 " + tank_2.GetInstanceID());

    }

}
