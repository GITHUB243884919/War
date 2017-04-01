using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Test_AI_Group_1 : MonoBehaviour
{
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClick);
    }

    public void BtnClick()
    {
        //CreateCharObjs_TARGET_VERTICAL_LINE();
    }

    void CreateCharObjs_TARGET_VERTICAL_LINE()
    {
        GroupController groupController = new GroupController();
        GroupElement [] elements = new GroupElement[3];
        elements[0] = new GroupElement();
        elements[0].ServerEntityID = 0;
        elements[0].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;

        elements[1] = new GroupElement();
        elements[1].ServerEntityID = 1;
        elements[1].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;

        elements[2] = new GroupElement();
        elements[2].ServerEntityID = 2;
        elements[2].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;
        Vector3 startPos = new Vector3(32f, 0f, 32f);
        //Vector3 endPos = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        Vector3 endPos = new Vector3(160f, 0f, 100f);
        //假定服务器规定这段路必须time秒走完
        float time = 15f;
        float speed = (endPos - startPos).magnitude / time;

        groupController.AI_Arrive_TARGET_VERTICAL_LINE(elements,
            GroupController.E_FORMATION.TARGET_VERTICAL_LINE,
            startPos, endPos, speed, null);

        //groupController.AI_Arrive_TARGET_TRANGLE(elements,
        //    GroupController.E_FORMATION.TARGET_TRANGLE,
        //    startPos, endPos, speed, null);

        
    }

    void OnDrawGizmos()
    {
        //Test_1();
        //Test_2();
        //Test_3();
        Test_4();
    }

    void Test_1()
    {
        float fireRange = 32f;
        Vector3 startPos = new Vector3(10, 0, 32);
        Vector3 endPos = startPos + new Vector3(fireRange, 0f, 0f);
        LogMediator.DrawLine(startPos, endPos, Color.red, (endPos - startPos).magnitude);
        LogMediator.Log("dis 0 " + (endPos - startPos).magnitude);
        float deg = 15f;
        float radStart = Mathf.Deg2Rad * deg;
        Vector3 rayPointStart = new Vector3(
            fireRange * Mathf.Sin(radStart),
            0,
            fireRange * Mathf.Cos(radStart));
        rayPointStart += (startPos - Vector3.zero);
        LogMediator.DrawLine(startPos, rayPointStart, Color.blue, (rayPointStart - startPos).magnitude);
        LogMediator.Log("dis 1 " + (rayPointStart - startPos).magnitude);

        float radEnd = Mathf.Deg2Rad * (deg + 90f);
        Vector3 rayPointEnd = new Vector3(
            fireRange * Mathf.Sin(radEnd),
            0,
            fireRange * Mathf.Cos(radEnd));
        rayPointEnd += (startPos - Vector3.zero);
        LogMediator.DrawLine(startPos, rayPointEnd, Color.cyan, (rayPointEnd - startPos).magnitude);
        LogMediator.Log("dis 2 " + (rayPointEnd - startPos).magnitude);
    }

    void Test_2()
    {
        //float fireRange = 32f;
        Vector3 startPos = new Vector3(10, 0, 32);
        Vector3 endPos = startPos + new Vector3(20, 0f, 40f);
        float fireRange = (endPos - startPos).magnitude;
        LogMediator.DrawLine(startPos, endPos, Color.red, (endPos - startPos).magnitude);
        LogMediator.Log("dis 0 " + (endPos - startPos).magnitude);
        float degStart = 60f;
        float radStart = Mathf.Deg2Rad * degStart;
        Vector3 rayPointStart = new Vector3(
            fireRange * Mathf.Cos(radStart),
            0,
            fireRange * Mathf.Sin(radStart));
        rayPointStart += (startPos - Vector3.zero);
        LogMediator.DrawLine(startPos, rayPointStart, Color.blue, (rayPointStart - startPos).magnitude);
        LogMediator.Log("dis 1 " + (rayPointStart - startPos).magnitude);

        //float degEnd = Mathf.Acos((endPos - startPos).x / (endPos - startPos).magnitude);
        //float radEnd = Mathf.Deg2Rad * (degStart + degStart);
        float orginRad = Mathf.Acos((endPos - startPos).x / (endPos - startPos).magnitude);
        float orginDeg = Mathf.Rad2Deg * orginRad;
        LogMediator.Log("orginDeg " + orginDeg);
        float degEnd = orginDeg + (orginDeg - degStart);
        LogMediator.Log("degEnd " + degEnd);
        float radEnd = Mathf.Deg2Rad * degEnd;

        Vector3 rayPointEnd = new Vector3(
            fireRange * Mathf.Cos(radEnd),
            0,
            fireRange * Mathf.Sin(radEnd));
        rayPointEnd += (startPos - Vector3.zero);
        LogMediator.DrawLine(startPos, rayPointEnd, Color.cyan, (rayPointEnd - startPos).magnitude);
        LogMediator.Log("dis 2 " + (rayPointEnd - startPos).magnitude);
    }

    void Test_3()
    {
        Vector3 startPos = new Vector3(0, 0, 0);
        Vector3 endPos = startPos + new Vector3(32f, 0f, 32f);
        Vector3 toTarget = endPos - startPos;
        float fireRange = toTarget.magnitude;
        //float orginRad = Mathf.Acos(toTarget.x / toTarget.magnitude);
        float orginRad = BattleCalc.Vector3AngleRad(startPos, endPos);
        //float orginDeg = Mathf.Rad2Deg * orginRad;
        float orginDeg = BattleCalc.Vector3AngleDeg(startPos, endPos);
        LogMediator.DrawLine(startPos, endPos, Color.red, toTarget.magnitude);
        LogMediator.Log("dis 0 " + toTarget.magnitude
            + " orginDeg " + orginDeg);
        
        
        float startDeg = 5f;
        float startRad = Mathf.Deg2Rad * (orginDeg - startDeg);
        //float startRad = orginRad - Mathf.Deg2Rad * startDeg;
        Debug.Log("startDeg " + Mathf.Rad2Deg * startRad);
        Vector3 rayPointStart = new Vector3(
            fireRange * Mathf.Cos(startRad),
            0,
            fireRange * Mathf.Sin(startRad));
        rayPointStart += (startPos - Vector3.zero);
        LogMediator.DrawLine(startPos, rayPointStart, Color.blue, (rayPointStart - startPos).magnitude);
        LogMediator.Log("dis 1 " + (rayPointStart - startPos).magnitude);

        LogMediator.Log("orginDeg " + orginDeg);
        float endDeg = orginDeg + startDeg;
        LogMediator.Log("degEnd " + endDeg);
        float endRad = Mathf.Deg2Rad * endDeg;
        //float endRad = orginRad + (orginRad - startRad);
        Vector3 rayPointEnd = new Vector3(
            fireRange * Mathf.Cos(endRad),
            0,
            fireRange * Mathf.Sin(endRad));
        rayPointEnd += (startPos - Vector3.zero);
        LogMediator.DrawLine(startPos, rayPointEnd, Color.cyan, (rayPointEnd - startPos).magnitude);
        LogMediator.Log("dis 2 " + (rayPointEnd - startPos).magnitude);
    }

    void Test_4()
    {
        Vector3 startPos = new Vector3(0, 0, 0);
        Vector3 endPos = startPos + new Vector3(-32f, 0f, 32f);
        Vector3 toTarget = endPos - startPos;
        float fireRange = toTarget.magnitude;

        float orginRad = BattleCalc.Vector3AngleRad(startPos, endPos);
        float orginDeg = BattleCalc.Vector3AngleDeg(startPos, endPos);
        LogMediator.DrawLine(startPos, endPos, Color.red, toTarget.magnitude);
        LogMediator.Log("dis 0 " + toTarget.magnitude
            + " orginDeg " + orginDeg);


        float startDeg = 5f;
        //float startRad = Mathf.Deg2Rad * (orginDeg - startDeg);
        float startRad = orginRad - Mathf.Deg2Rad * startDeg;
        Debug.Log("startDeg " + Mathf.Rad2Deg * startRad);
        Vector3 rayPointStart = new Vector3(
            fireRange * Mathf.Cos(startRad),
            0,
            fireRange * Mathf.Sin(startRad));
        rayPointStart += (startPos - Vector3.zero);
        LogMediator.DrawLine(startPos, rayPointStart, Color.blue, (rayPointStart - startPos).magnitude);
        LogMediator.Log("dis 1 " + (rayPointStart - startPos).magnitude);

        LogMediator.Log("orginDeg " + orginDeg);
        //float endDeg = orginDeg + startDeg;
        //LogMediator.Log("degEnd " + endDeg);
        float endRad = orginRad + Mathf.Deg2Rad * startDeg;
        //float endRad = orginRad + (orginRad - startRad);
        Vector3 rayPointEnd = new Vector3(
            fireRange * Mathf.Cos(endRad),
            0,
            fireRange * Mathf.Sin(endRad));
        rayPointEnd += (startPos - Vector3.zero);
        LogMediator.DrawLine(startPos, rayPointEnd, Color.cyan, (rayPointEnd - startPos).magnitude);
        LogMediator.Log("dis 2 " + (rayPointEnd - startPos).magnitude);
    }
}
