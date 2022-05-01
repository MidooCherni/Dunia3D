using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    // StatHandler.cs       a registry for all common values shared between the player and npcs

public class StatHandler : MonoBehaviour
{
        // string values
    public string charname = "?";
    public string racename = "?";
    public string classname = "?";
    public string godname = "Faithless";
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
    public byte LEVEL = 1;
    public byte STR = 0;  byte bSTR = 0;
    public byte CON = 0;  byte bCON = 0;
    public byte DEX = 0;  byte bDEX = 0;
    public byte AGI = 0;  byte bAGI = 0;
    public byte INT = 0;  byte bINT = 0;
    public byte WIS = 0;  byte bWIS = 0;
    public byte CHA = 0;  byte bCHA = 0;

        // resistances
    byte RES_HOLY = 0;     byte bRES_HOLY = 0;   
    byte RES_FIRE = 0;     byte bRES_FIRE = 0;   
    byte RES_FROST = 0;    byte bRES_FROST = 0;  
    byte RES_ARCANE = 0;   byte bRES_ARCANE = 0; 
    byte RES_NATURE = 0;   byte bRES_NATURE = 0; 
    byte RES_SHADOW = 0;   short bRES_SHADOW = 0; 

    void Start(){       // TODO: proper calculations!!
        bSTR = STR;
        bCON = CON;
        bDEX = DEX;
        bAGI = AGI;
        bINT = INT;
        bWIS = WIS;
        bCHA = CHA;

        int hval;
        switch(classname){
            case "Warrior":
            case "Knight":
            case "Monk":
                if(INT > WIS){ MAX_MP = (int)(LEVEL + INT) * 3; } else { MAX_MP = (int)(LEVEL + WIS) * 3; }
                hval = 5;
                break;
            case "Rogue":
            case "Bard":
            case "Ranger":
                if(INT > WIS){ MAX_MP = (int)(LEVEL + INT) * 3; } else { MAX_MP = (int)(LEVEL + WIS) * 3; }
                hval = 4;
                break;
            case "Cleric":
            case "Druid":
            case "Shaman":
                MAX_MP = (int)(LEVEL + WIS) * 10;
                hval = 3;
                break;
            case "Wizard":
            case "Sorcerer":
            case "Necromancer":
                MAX_MP = (int)(LEVEL + INT) * 10;
                hval = 2;
                break;
            default:
                hval = 2;
                MAX_MP = 0;
                break;
        }
        MAX_HP = (int)(LEVEL + CON) * hval;
        MAX_SP = 100;                                 // TODO: ATHLETICS

        CURRENT_HP = MAX_HP;
        CURRENT_MP = MAX_MP;
        CURRENT_SP = MAX_SP;
    }
}
