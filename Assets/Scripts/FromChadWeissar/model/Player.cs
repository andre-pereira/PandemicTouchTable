using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;

public class Player
{

    public enum Roles
    {
        [Description("Containment Specialist")]
        ContainmentSpecialist,

        [Description("Pilot")]
        Pilot,

        [Description("Quarantine Specialist")]
        QuarantineSpecialist,

        [Description("Virologist")]
        Virologist
    };

    public enum Statistic
    {
        NUM_TYPES
    }


    #region State
    #endregion

    #region Statistics
    public float ActingTime = 0f;
    public Dictionary<Statistic, int> Statistics = new Dictionary<Statistic, int>();
    #endregion

    public int Position;
    public int Place;
    public string Name;

    public Roles Role { get; set; }

    public Player()
    {
        for (int i = 0; i < (int)Statistic.NUM_TYPES; ++i)
            Statistics[(Statistic)i] = 0;
    }

}
