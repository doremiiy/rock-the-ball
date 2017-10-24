using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility {
    public enum Team { blue, red };

    static public Team Opp(Team team)
    {
        Team opp;
        if  (team == Team.blue)
        {
            opp = Team.red;
        } else if (team == Team.red)
        {
            opp = Team.blue;
        } else
        {
            throw new Exception("Error : Unrecognized Team passed to method Utility.Opp()");
        }
        return opp;
    }
}
