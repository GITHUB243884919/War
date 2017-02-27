using UnityEngine;
using System.Collections;

public class CharCommondFactory
{
    public static CharCommond CreateCommond(CharController cctr, int charType)
    {
        CharCommond cmd = null;
        switch (charType)
        {
            case 0:
                cmd = new TankCommond(cctr);
                break;
            default:
                cmd = new TankCommond(cctr);
                break;
        }

        return cmd;
    }
}

