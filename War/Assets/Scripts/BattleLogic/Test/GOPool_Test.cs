using UnityEngine;
using System.Collections;

public class GOPool_Test : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
    //    QObjCreatorFactory<GameObject> creatorFactory = new QObjCreatorFactoryForMeshBaker(
    //paths, BattleScene2.E_BATTLE_OBJECT_TYPE.TANK, count);

        QObjCreatorForGameObject creator = new QObjCreatorForGameObject(
            "Effect/Tank/Prefab/Tank_dapao_fire", 32);

        QObjPool<GameObject> pool = new QObjPool<GameObject>();

        pool.Init(creator, null);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
