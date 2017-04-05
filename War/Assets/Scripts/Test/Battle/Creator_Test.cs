using UnityEngine;
using System.Collections;

public abstract class ClassA 
{
    public virtual void Print()
    {

    }
}

public class ClassB:ClassA
{
    public override void Print()
    {
        //base.Print();
        Debug.Log("ClassB Print");
    }
}

public class ClassC : ClassA
{
    public override void Print()
    {
        //base.Print();
        Debug.Log("ClassC Print");
    }
}
public class CreatorClass
{
    public static ClassA CreateClass(int type)
    {
        ClassA classWhat = null;
        switch(type)
        {
            case 0:
                classWhat = new ClassB();
                break;
            case 1:
                classWhat = new ClassC();
                break;
            default:
                break;
        }

        return classWhat;
    }
}
