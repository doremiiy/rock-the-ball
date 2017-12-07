using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility {

    public enum Team { BLUE, RED };
    public enum Hand { FIRST, SECONDARY };
    public enum TrainingStep { INITIAL, GOAL, SERVICE, FREE };

    static public int winningScore = 15;

    static public Team Opp(Team team)
    {
        Team opp;
        if  (team == Team.BLUE)
        {
            opp = Team.RED;
        } else if (team == Team.RED)
        {
            opp = Team.BLUE;
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
