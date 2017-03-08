using UnityEngine;
using System.Collections;

public class LoadTest_1 : MonoBehaviour 
{
    void Start () 
    {
        ResourcesManagerMediator.GetGameObjectFromResourcesManager2(
            "Scripts/Test/ResourcesManager/Tank_1.prefab");
    }

}
