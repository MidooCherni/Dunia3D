using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    // StatHandler.cs       a registry for all common values shared between the player and npcs

public class StatHandler : MonoBehaviour
{
        // string values
    string charname = "?";
    string racename = "?";
    string classname = "?";
    string godname = "Faithless";
    int[] buffs = {0};
    int[] debuffs = {0};

        // dynamic values
    public int CURRENT_HP = 1;
    public int CURRENT_MP = 0;
    public int CURRENT_SP = 1;
    public int CURRENT_XP = 0;
    public int MAX_HP = 1;
    public int MAX_MP = 0;
    public int MAX_SP = 1;
    public int XP_CAP = 1;
    public int ARMOR = 0;

        // attributes
    byte STR = 0;  byte bSTR = 0;
    byte CON = 0;  byte bCON = 0;
    byte DEX = 0;  byte bDEX = 0;
    byte AGI = 0;  byte bAGI = 0;
    byte INT = 0;  byte bINT = 0;
    byte WIS = 0;  byte bWIS = 0;
    byte CHA = 0;  byte bCHA = 0;

        // resistances
    byte RES_HOLY = 0;     byte bRES_HOLY = 0;   
    byte RES_FIRE = 0;     byte bRES_FIRE = 0;   
    byte RES_FROST = 0;    byte bRES_FROST = 0;  
    byte RES_ARCANE = 0;   byte bRES_ARCANE = 0; 
    byte RES_NATURE = 0;   byte bRES_NATURE = 0; 
    byte RES_SHADOW = 0;   short bRES_SHADOW = 0; 
}
