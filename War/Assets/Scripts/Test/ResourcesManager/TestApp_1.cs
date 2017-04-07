using UnityEngine;
using System.Collections;

public class TestApp_1 : MonoBehaviour
{
    void Start()
    {
        ResourcesManager.Instance.Init();
        GetAssetFromLocalAssetBundle<Material> loader
            = new GetAssetFromLocalAssetBundle<Material>();

        loader.GetAsset(
            "Resources/RuntimeMeshBaker/M_Arm_Engineercorps/M_Arm_Engineercorps-mat",
            FinishLoad);
    }

    void FinishLoad(Material material)
    {
        LogMediator.Log("成功");
    }

    
}
