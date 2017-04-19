using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if _LOG_MEDIATOR_
using Debug = LogMediator;
#endif
public class Test_AI_Group_2 : MonoBehaviour
{
    
    GroupCharObjsController m_groupCtr = null;
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClick);
        m_groupCtr = new GroupCharObjsController();
    }

    public void BtnClick()
    {
        CreateGroupCharObjs();
    }

    void CreateGroupCharObjs()
    {
        GroupCharObjsElement[] elements = new GroupCharObjsElement[5];
        elements[0] = new GroupCharObjsElement();
        elements[0].ServerEntityID = 0;
        elements[0].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;

        elements[1] = new GroupCharObjsElement();
        elements[1].ServerEntityID = 1;
        elements[1].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;

        elements[2] = new GroupCharObjsElement();
        elements[2].ServerEntityID = 2;
        elements[2].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;

        elements[3] = new GroupCharObjsElement();
        elements[3].ServerEntityID = 3;
        elements[3].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;

        elements[4] = new GroupCharObjsElement();
        elements[4].ServerEntityID = 4;
        elements[4].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;
        
        GroupCharObjsController.E_FORMATION_TYPE orgFormation =
            //GroupCharObjsController.E_FORMATION_TYPE.TARGET_HORIZONTAL_LINE;
            GroupCharObjsController.E_FORMATION_TYPE.TARGET_VERTICAL_LINE;
        m_groupCtr.Init(elements, orgFormation, new Vector3(32f, 0f, 32f));

        GroupCharObjsController.E_FORMATION_TYPE switchFormation =
            //GroupCharObjsController.E_FORMATION_TYPE.TARGET_HORIZONTAL_LINE;
            //GroupCharObjsController.E_FORMATION_TYPE.TARGET_VERTICAL_LINE;
            //GroupCharObjsController.E_FORMATION_TYPE.TARGET_TRANGLE;
            GroupCharObjsController.E_FORMATION_TYPE.TARGET_CYCLE;
        StartCoroutine(SwitchFormation(switchFormation, new Vector3(64f, 0f, 32f)));

    }

    public IEnumerator SwitchFormation(
        GroupCharObjsController.E_FORMATION_TYPE formation,
        Vector3 target)
    {

        Debug.Log("3秒后开始阵型变换");
        yield return new WaitForSeconds(3f);
        Debug.Log("阵型变换中。。。。");
        m_groupCtr.SwitchFormation(formation, target);
    }
}
