using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFunctions : MonoBehaviour
{
    public GameObject cam;

    public GameObject slot_ear1;
    public GameObject slot_ear2;
    public GameObject slot_ring1;
    public GameObject slot_ring2;
    public GameObject slot_wrist1;
    public GameObject slot_wrist2;
    public GameObject slot_neck;
    public GameObject slot_back;
    public GameObject slot_face;
    public GameObject slot_head;
    public GameObject slot_shoulder;
    public GameObject slot_chest;
    public GameObject slot_waist;
    public GameObject slot_legs;
    public GameObject slot_feet;
    public GameObject slot_hands;
    public GameObject slot_weapon;
    public GameObject slot_offhand;

    public void handleSlot(GameObject slot, short item_id){
        if(slot.GetComponent<SlotCode>().item != 0){
            unwear(slot.GetComponent<SlotCode>().item);
            GetComponent<ItemHandler>().Inventory[slot.GetComponent<SlotCode>().item].quantity++;
            slot.GetComponent<SlotCode>().item = 0;
        }
        slot.GetComponent<SlotCode>().item = item_id;
    }

    public void wear(short item_id){
        if(GetComponent<ItemHandler>().Inventory[item_id].type == Type.WEAPON){
            if(slot_weapon.GetComponent<SlotCode>().item != 0){
                unwear(slot_weapon.GetComponent<SlotCode>().item);
                GetComponent<ItemHandler>().Inventory[slot_weapon.GetComponent<SlotCode>().item].quantity++;
                slot_weapon.GetComponent<SlotCode>().item = 0;
            }
            slot_weapon.GetComponent<SlotCode>().item = item_id;
            
            cam.GetComponent<AttackScript>().spr_idle = 
                Resources.Load<Sprite>("Textures/Weapons/" + GetComponent<ItemHandler>().Inventory[item_id].w_sprites[0]);
            cam.GetComponent<AttackScript>().spr_pulled = 
                Resources.Load<Sprite>("Textures/Weapons/" + GetComponent<ItemHandler>().Inventory[item_id].w_sprites[1]);
            cam.GetComponent<AttackScript>().spr_recover = 
                Resources.Load<Sprite>("Textures/Weapons/" + GetComponent<ItemHandler>().Inventory[item_id].w_sprites[2]);
            GameObject.Find("Weapon").GetComponent<Image>().sprite = cam.GetComponent<AttackScript>().spr_idle;
        } else {
            switch(GetComponent<ItemHandler>().Inventory[item_id].subtype){
                case SubType.EAR:
                    if(slot_ear1.GetComponent<SlotCode>().item == 0){ handleSlot(slot_ear1, item_id);} 
                        else { handleSlot(slot_ear2, item_id); }
                    break;
                case SubType.RING:
                    if(slot_ring1.GetComponent<SlotCode>().item == 0){ handleSlot(slot_ring1, item_id);} 
                        else { handleSlot(slot_ring2, item_id); }
                    break;
                case SubType.UWRIST:
                case SubType.LWRIST:
                case SubType.HWRIST:
                    if(slot_wrist1.GetComponent<SlotCode>().item == 0){ handleSlot(slot_wrist1, item_id);} 
                        else { handleSlot(slot_wrist2, item_id); }
                    break;
                case SubType.NECK:
                    handleSlot(slot_neck, item_id);
                    break;
                case SubType.BACK:
                    handleSlot(slot_back, item_id);
                    break;
                case SubType.UFACE:
                case SubType.LFACE:
                case SubType.HFACE:
                    handleSlot(slot_face, item_id);
                    break;
                case SubType.UHEAD:
                case SubType.LHEAD:
                case SubType.HHEAD:
                    handleSlot(slot_head, item_id);
                    break;
                case SubType.USHOULDER:
                case SubType.LSHOULDER:
                case SubType.HSHOULDER:
                    handleSlot(slot_shoulder, item_id);
                    break;
                case SubType.UCHEST:
                case SubType.LCHEST:
                case SubType.HCHEST:
                    handleSlot(slot_chest, item_id);
                    break;
                case SubType.ULEGS:
                case SubType.LLEGS:
                case SubType.HLEGS:
                    handleSlot(slot_legs, item_id);
                    break;
                case SubType.UWAIST:
                case SubType.LWAIST:
                case SubType.HWAIST:
                    handleSlot(slot_waist, item_id);
                    break;
                case SubType.UFEET:
                case SubType.LFEET:
                case SubType.HFEET:
                    handleSlot(slot_feet, item_id);
                    break;
                case SubType.UHANDS:
                case SubType.LHANDS:
                case SubType.HHANDS:
                    handleSlot(slot_hands, item_id);
                    break;
                case SubType.OFF:
                    handleSlot(slot_offhand, item_id);
                    break;
                default:
                    Debug.Log("bug");
                    break;
            }
        }

        GetComponent<StatHandler>().STR += GetComponent<ItemHandler>().Inventory[item_id].buffs[0];
        GetComponent<StatHandler>().CON += GetComponent<ItemHandler>().Inventory[item_id].buffs[1];
        GetComponent<StatHandler>().DEX += GetComponent<ItemHandler>().Inventory[item_id].buffs[2];
        GetComponent<StatHandler>().AGI += GetComponent<ItemHandler>().Inventory[item_id].buffs[3];
        GetComponent<StatHandler>().INT += GetComponent<ItemHandler>().Inventory[item_id].buffs[4];
        GetComponent<StatHandler>().WIS += GetComponent<ItemHandler>().Inventory[item_id].buffs[5];
        GetComponent<StatHandler>().CHA += GetComponent<ItemHandler>().Inventory[item_id].buffs[6];
        GetComponent<StatHandler>().RES_HOLY += GetComponent<ItemHandler>().Inventory[item_id].resist[0];
        GetComponent<StatHandler>().RES_FIRE += GetComponent<ItemHandler>().Inventory[item_id].resist[1];
        GetComponent<StatHandler>().RES_FROST += GetComponent<ItemHandler>().Inventory[item_id].resist[2];
        GetComponent<StatHandler>().RES_ARCANE += GetComponent<ItemHandler>().Inventory[item_id].resist[3];
        GetComponent<StatHandler>().RES_NATURE += GetComponent<ItemHandler>().Inventory[item_id].resist[4];
        GetComponent<StatHandler>().RES_SHADOW += GetComponent<ItemHandler>().Inventory[item_id].resist[5];
        
        // TODO: ONUSE SPELL TO SPELLBOOK!!!!!!!! ADD ONHITSPELL TO PLAYER SCRIPT THAT ATTACK WILL CHECK FROM!!!! CAST ONWEAR ON SELF!!!!
        
        cam.GetComponent<AttackScript>().attack_cd_CAP = GetComponent<ItemHandler>().Inventory[item_id].delay;
        cam.GetComponent<AttackScript>().attack_cd = GetComponent<ItemHandler>().Inventory[item_id].delay;
        
        // TODO: figure an actual damage value function that uses weapon damage, str bonus and magical damage bonus
        cam.GetComponent<AttackScript>().attack_value =
            GetComponent<ItemHandler>().Inventory[item_id].magnitude + GetComponent<StatHandler>().STR;
    }

    public void unwear(short item_id){
        if(GetComponent<ItemHandler>().Inventory[item_id].type == Type.WEAPON){
            cam.GetComponent<AttackScript>().spr_idle = 
                Resources.Load<Sprite>("Textures/Weapons/fist1");
            cam.GetComponent<AttackScript>().spr_pulled = 
                Resources.Load<Sprite>("Textures/Weapons/fist2");
            cam.GetComponent<AttackScript>().spr_recover = 
                Resources.Load<Sprite>("Textures/Weapons/fist3");
            GameObject.Find("Weapon").GetComponent<Image>().sprite = cam.GetComponent<AttackScript>().spr_idle;
        }
        GetComponent<StatHandler>().STR -= GetComponent<ItemHandler>().Inventory[item_id].buffs[0];
        GetComponent<StatHandler>().CON -= GetComponent<ItemHandler>().Inventory[item_id].buffs[1];
        GetComponent<StatHandler>().DEX -= GetComponent<ItemHandler>().Inventory[item_id].buffs[2];
        GetComponent<StatHandler>().AGI -= GetComponent<ItemHandler>().Inventory[item_id].buffs[3];
        GetComponent<StatHandler>().INT -= GetComponent<ItemHandler>().Inventory[item_id].buffs[4];
        GetComponent<StatHandler>().WIS -= GetComponent<ItemHandler>().Inventory[item_id].buffs[5];
        GetComponent<StatHandler>().CHA -= GetComponent<ItemHandler>().Inventory[item_id].buffs[6];
        GetComponent<StatHandler>().RES_HOLY -= GetComponent<ItemHandler>().Inventory[item_id].resist[0];
        GetComponent<StatHandler>().RES_FIRE -= GetComponent<ItemHandler>().Inventory[item_id].resist[1];
        GetComponent<StatHandler>().RES_FROST -= GetComponent<ItemHandler>().Inventory[item_id].resist[2];
        GetComponent<StatHandler>().RES_ARCANE -= GetComponent<ItemHandler>().Inventory[item_id].resist[3];
        GetComponent<StatHandler>().RES_NATURE -= GetComponent<ItemHandler>().Inventory[item_id].resist[4];
        GetComponent<StatHandler>().RES_SHADOW -= GetComponent<ItemHandler>().Inventory[item_id].resist[5];
        
        // TODO: ONUSE SPELL TO SPELLBOOK!!!!!!!! ADD ONHITSPELL TO PLAYER SCRIPT THAT ATTACK WILL CHECK FROM!!!! CAST ONWEAR ON SELF!!!!
        
        cam.GetComponent<AttackScript>().attack_cd_CAP = 15;    // TODO: an actual balanced fist damage value
        cam.GetComponent<AttackScript>().attack_cd = 15;
        
        // TODO: figure a final damage value function that uses weapon damage, str bonus and magical damage bonus
        cam.GetComponent<AttackScript>().attack_value = GetComponent<StatHandler>().STR;
    }

    public void Start(){
            // TODO: nooooooo lmfaoooooo
        cam.GetComponent<AttackScript>().attack_value = GetComponent<StatHandler>().STR;
    }
}
