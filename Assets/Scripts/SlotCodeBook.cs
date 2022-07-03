using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotCodeBook : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public short spell = 0;
    public GameObject SpellSystem;

    public void OnPointerDown(PointerEventData pointerEventData){}
    public void OnPointerUp(PointerEventData pointerEventData){
        if(spell != 0){
            if (pointerEventData.button == PointerEventData.InputButton.Left) {
                SpellSystem.GetComponent<SpellHandler>().equippedSpell = spell;
            }
        }
    }

    void hotkeySpell(byte num){
            // swap
        for(int i = 0; i < 10; i++){
            if(SpellSystem.GetComponent<SpellHandler>().hotbar[i] == spell){
                SpellSystem.GetComponent<SpellHandler>().hotbar[i] = SpellSystem.GetComponent<SpellHandler>().hotbar[num];
            }
        }
        if(SpellSystem.GetComponent<SpellHandler>().hotbar[num] == spell){
            SpellSystem.GetComponent<SpellHandler>().hotbar[num] = 0;
        } else {
            SpellSystem.GetComponent<SpellHandler>().hotbar[num] = spell;
        }
        SpellSystem.GetComponent<SpellHandler>().drawSpellbook();
    }

    void Update(){
        // hotkey code
        if(spell != 0){
            if((Input.mousePosition.x > transform.position.x-20) && (Input.mousePosition.x < transform.position.x+20) && 
                (Input.mousePosition.y > transform.position.y-20) && (Input.mousePosition.y < transform.position.y+20)){
                    // TODO: FIX THIS YANDEREDEV ASS SHIT LMAOOOOOOOOOOOO
                if(Input.GetKeyDown("1")){
                    hotkeySpell(0);
                }
                if(Input.GetKeyDown("2")){
                    hotkeySpell(1);
                }
                if(Input.GetKeyDown("3")){
                    hotkeySpell(2);
                }
                if(Input.GetKeyDown("4")){
                    hotkeySpell(3);
                }
                if(Input.GetKeyDown("5")){
                    hotkeySpell(4);
                }
                if(Input.GetKeyDown("6")){
                    hotkeySpell(5);
                }
                if(Input.GetKeyDown("7")){
                    hotkeySpell(6);
                }
                if(Input.GetKeyDown("8")){
                    hotkeySpell(7);
                }
                if(Input.GetKeyDown("9")){
                    hotkeySpell(8);
                }
                if(Input.GetKeyDown("0")){
                    hotkeySpell(9);
                }
            }
        }
    }
}
