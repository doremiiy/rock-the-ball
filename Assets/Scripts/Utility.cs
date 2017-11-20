using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility {

    public enum Team { blue, red };
    static public int winningScore = 15;

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

    static public Team RandomTeam()
    {
        Array teams = Enum.GetValues(typeof(Team));
        System.Random random = new System.Random();
        Team randomTeam = (Team)teams.GetValue(random.Next(teams.Length));
        return randomTeam;
    }
}
