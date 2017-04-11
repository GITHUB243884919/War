using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if _WAR_TEST_
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
        //StartCoroutine("CreateCharObjs");
        Debug.Log("BtnClick");
        ResourcesManager2.Instance.Init();
        //GetAssetFromLocalAssetBundle<Material> loader
        //    = new GetAssetFromLocalAssetBundle<Material>();

        //loader.GetAsset(
        //    "Resources/RuntimeMeshBaker/M_Arm_Engineercorps/M_Arm_Engineercorps-mat",
        //    FinishLoad);
    }

    void FinishLoad(Material m)
    {

    }
}
