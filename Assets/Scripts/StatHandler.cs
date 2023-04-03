using System.Diagnostics;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    // StatHandler.cs       a registry for all common values shared between the player and npcs

public class Slot{
    public short spellorigin = 0;
    public Stats stataffected = Stats.HP;
    public int magnitude = 0;
    public int timeleft = 0;
    public DispellType dt = DispellType.MAGICAL;
    public Slot(){}
    public Slot(short sp, Stats sa, int m, int t, DispellType d){
        spellorigin = sp;
        stataffected = sa;
        magnitude = m;
        timeleft = t;
        dt = d;
    }
}

public class StatHandler : MonoBehaviour
{
        // string values
    public string charname = "?";
    public string racename = "?";
    public string classname = "?";
    public string godname = "Faithless";
    public List<Slot> buffs = new List<Slot>();
    public List<Slot> debuffs = new List<Slot>();

    public void tickAura(Stats affected, int magnitude){
        if(GetComponent<MobBehavior>()){
            GetComponent<MobBehavior>().flashcolor = Ele.FIRE;
            GetComponent<MobBehavior>().flashtimer = 5;  // 3 flashing frames
        }
        switch(affected){
            case Stats.HOT:
                CURRENT_HP += magnitude;
                if(CURRENT_HP > MAX_HP){ CURRENT_HP = MAX_HP; }
                break;
            case Stats.DOT:
                CURRENT_HP -= magnitude;     // TODO: DEATH CODE
                break;
            case Stats.CRACK:
                CURRENT_MP += magnitude;
                if(CURRENT_MP > MAX_MP){ CURRENT_MP = MAX_MP; }
                break;
            default:            // TODO: FUUUUUUUUUUUUUUUUUUUCKKKKKKKKKKKKKKKKKKKKKKKK
                break;
        }
    }

    public bool strongerAuraPresent(short newAuraID, short[] spellfam, int newtime, EffectType et, Stats aff, int mag){
        foreach(short familymember in spellfam){   // look for every buff's id...
            foreach(Slot checkedslot in buffs){   // ...in the spellfamily of the aura being cast...
                if(checkedslot.spellorigin == familymember){
                    if(Array.IndexOf(spellfam, familymember) > newAuraID){
                        UnityEngine.Debug.Log("Cannot apply effect, stronger aura found");
                        return true;
                    } else if (Array.IndexOf(spellfam, familymember) > newAuraID){
                        UnityEngine.Debug.Log("Spell id " + newAuraID + " already found, refreshing...");
                        checkedslot.timeleft = newtime;
                        return true;
                    } else {
                        UnityEngine.Debug.Log("Spell id " + newAuraID + " already found, sacrificing old for new...");
                        dropAura(aff, mag);
                        return false;
                    }
                }
            }
            foreach(Slot checkedslot in debuffs){   // ...in the spellfamily of the aura being cast...
                if(checkedslot.spellorigin == familymember){
                    if(Array.IndexOf(spellfam, familymember) > newAuraID){
                        UnityEngine.Debug.Log("Cannot apply effect, stronger aura found");
                        return true;
                    } else if (Array.IndexOf(spellfam, familymember) > newAuraID){
                        UnityEngine.Debug.Log("Spell id " + newAuraID + " already found, refreshing...");
                        checkedslot.timeleft = newtime;
                        return true;
                    } else {
                        UnityEngine.Debug.Log("Spell id " + newAuraID + " already found, sacrificing old for new...");
                        dropAura(aff, mag);
                        return false;
                    }
                }
            }
        }
        return false;   
    }

    public void addAura(short origin, EffectType et, Stats affected, int points_, int duration, bool debuff, short[] spellfam, DispellType how2dispell){
        int points = points_;
        if(spellfam.Length > 0){    // spell comes in a set
            if(strongerAuraPresent(origin, spellfam, duration, et, affected, points)){
                return;
            }
        }
        if(et == EffectType.DEBUFF){ points = -points_; }
        if(debuff){
            debuffs.Add(new Slot(origin, affected, points, duration, how2dispell));
        } else { 
            buffs.Add(new Slot(origin, affected, points, duration, how2dispell));
        }
        switch(affected){
            case Stats.HP: MAX_HP += points; CURRENT_HP += points; break;
            case Stats.MP: MAX_MP += points; CURRENT_MP += points; break;
            case Stats.STAM: MAX_SP += points; CURRENT_SP += points; break;
            case Stats.ARMOR: ARMOR += points; break;
            case Stats.STR: STR += (byte)points; break;
            case Stats.CON: CON += (byte)points; break;
            case Stats.DEX: DEX += (byte)points; break;
            case Stats.AGI: AGI += (byte)points; break;
            case Stats.INT: INT += (byte)points; break;
            case Stats.WIS: WIS += (byte)points; break;
            case Stats.CHA: CHA += (byte)points; break;
            case Stats.RES_HOLY: RES_HOLY += (byte)points; break;
            case Stats.RES_FIRE: RES_FIRE += (byte)points; break;
            case Stats.RES_FROST: RES_FROST += (byte)points; break;
            case Stats.RES_ARCANE: RES_ARCANE += (byte)points; break;
            case Stats.RES_NATURE: RES_NATURE += (byte)points; break;
            case Stats.RES_SHADOW: RES_SHADOW += (byte)points; break;
            case Stats.ROOT: isRooted = true; break;
            case Stats.STUN: isStunned = true; break;
            case Stats.SILENCE: isSilenced = true; break; // TODO: fear, charm, command, pulse, feather, thorns, invis, fly, immunity flags..
            default: break;
        }
    }

    public void dropAura(Stats affected, int points){
        switch(affected){
            case Stats.HP: MAX_HP -= points; if(CURRENT_HP > MAX_HP){ CURRENT_HP = MAX_HP; }; break;
            case Stats.MP: MAX_MP -= points; if(CURRENT_MP > MAX_MP){ CURRENT_MP = MAX_MP; }; break;
            case Stats.STAM: MAX_SP -= points; if(CURRENT_SP > MAX_SP){ CURRENT_SP = MAX_SP; }; break;
            case Stats.ARMOR: ARMOR -= points; break;
            case Stats.STR: STR -= (byte)points; break;
            case Stats.CON: CON -= (byte)points; break;
            case Stats.DEX: DEX -= (byte)points; break;
            case Stats.AGI: AGI -= (byte)points; break;
            case Stats.INT: INT -= (byte)points; break;
            case Stats.WIS: WIS -= (byte)points; break;
            case Stats.CHA: CHA -= (byte)points; break;
            case Stats.RES_HOLY: RES_HOLY -= (byte)points; break;
            case Stats.RES_FIRE: RES_FIRE -= (byte)points; break;
            case Stats.RES_FROST: RES_FROST -= (byte)points; break;
            case Stats.RES_ARCANE: RES_ARCANE -= (byte)points; break;
            case Stats.RES_NATURE: RES_NATURE -= (byte)points; break;
            case Stats.RES_SHADOW: RES_SHADOW -= (byte)points; break;
            case Stats.ROOT:
                isRooted = false;
                foreach(Slot s in buffs){ if (s.stataffected == Stats.ROOT ){ isRooted = true; }}
                foreach(Slot s in debuffs){ if (s.stataffected == Stats.ROOT ){ isRooted = true; }}
                break;
            case Stats.STUN: 
                isStunned = false;
                foreach(Slot s in buffs){ if (s.stataffected == Stats.STUN ){ isStunned = true; }}
                foreach(Slot s in debuffs){ if (s.stataffected == Stats.STUN ){ isStunned = true; }}
                break;
            case Stats.SILENCE: 
                isSilenced = false;
                foreach(Slot s in buffs){ if (s.stataffected == Stats.SILENCE ){ isSilenced = true; }}
                foreach(Slot s in debuffs){ if (s.stataffected == Stats.SILENCE ){ isSilenced = true; }}
                break;// TODO: fear, charm, command, pulse, feather, thorns, invis, fly, immunity flags..
            default: break;
        }
    }

        // inventory ids for worn items
    // ear1 ear2 ring1 ring2 wrist1 wrist2 face head neck shoulders back chest hands waist legs feet weapon offhand
    short[] equipSlots = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

    public bool isSilenced = false;
    public bool isStunned = false;
    public bool isRooted = false;

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
    public byte RES_HOLY = 0;     byte bRES_HOLY = 0;   
    public byte RES_FIRE = 0;     byte bRES_FIRE = 0;   
    public byte RES_FROST = 0;    byte bRES_FROST = 0;  
    public byte RES_ARCANE = 0;   byte bRES_ARCANE = 0; 
    public byte RES_NATURE = 0;   byte bRES_NATURE = 0;
    public byte RES_SHADOW = 0;   byte bRES_SHADOW = 0;

    public void recalculateCaps(){      // TODO: FIX MANA VALUES
        int hval;
        switch(classname){
            case "Warrior":
            case "Knight":
            case "Monk":
                if(INT > WIS){ MAX_MP = (int)(LEVEL-1 + INT); } else { MAX_MP = (int)(LEVEL-1 + WIS); }
                hval = 5;
                break;
            case "Rogue":
            case "Bard":
            case "Ranger":
                if(INT > WIS){ MAX_MP = (int)(LEVEL-1 + INT); } else { MAX_MP = (int)(LEVEL-1 + WIS); }
                hval = 4;
                break;
            case "Cleric":
            case "Druid":
            case "Shaman":
                MAX_MP = (int)(WIS * (LEVEL + (LEVEL-1)));
                hval = 3;
                break;
            case "Wizard":
            case "Sorcerer":
            case "Necromancer":
                MAX_MP = (int)(INT * (LEVEL + (LEVEL-1)));
                hval = 2;
                break;
            default:
                hval = 2;
                MAX_MP = 0;
                break;
        }
        MAX_HP = ((int)LEVEL + (int)CON) * hval;
        MAX_SP = 100;
    }

    public void FixedUpdate(){
        for(int i = 0; i < buffs.Count; i++){
            if (buffs[i].timeleft % 50 == 0 && ((buffs[i].stataffected == Stats.HOT) || (buffs[i].stataffected == Stats.DOT) || (buffs[i].stataffected == Stats.CRACK))){
                tickAura(buffs[i].stataffected, buffs[i].magnitude);
            }
            if (buffs[i].timeleft > 0){
                buffs[i].timeleft -= 1;
            } else {
                dropAura(buffs[i].stataffected, buffs[i].magnitude);
                buffs.Remove(buffs[i]);
            }
        }
        for(int i = 0; i < debuffs.Count; i++){
            if (debuffs[i].timeleft % 50 == 0 && ((debuffs[i].stataffected == Stats.HOT) || (debuffs[i].stataffected == Stats.DOT) || (debuffs[i].stataffected == Stats.CRACK))){
                tickAura(debuffs[i].stataffected, debuffs[i].magnitude);
            }
            if (debuffs[i].timeleft > 0){
                debuffs[i].timeleft -= 1;
            } else {
                dropAura(debuffs[i].stataffected, debuffs[i].magnitude);
                debuffs.Remove(debuffs[i]);
            }
        }
    }

    void Start(){       // TODO: proper stat tracking
        bSTR = STR;
        bCON = CON;
        bDEX = DEX;
        bAGI = AGI;
        bINT = INT;
        bWIS = WIS;
        bCHA = CHA;
        RES_HOLY = bRES_HOLY;
        RES_FIRE = bRES_FIRE;
        RES_FROST = bRES_FROST;
        RES_ARCANE = bRES_ARCANE;
        RES_NATURE = bRES_NATURE;
        RES_SHADOW = bRES_SHADOW;

        recalculateCaps();

        CURRENT_HP = MAX_HP;
        CURRENT_MP = MAX_MP;
        CURRENT_SP = MAX_SP;
    }
}
