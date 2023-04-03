using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotCodeBook : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public short spell = 0;
    public GameObject SpellSystem;
    public GameObject magiclabel;

    public void OnPointerDown(PointerEventData pointerEventData){}
    public void OnPointerUp(PointerEventData pointerEventData){
        if(spell != 0){
            if (pointerEventData.button == PointerEventData.InputButton.Left) {
                SpellSystem.GetComponent<SpellHandler>().equippedSpell = spell;
                magiclabel.GetComponent<UnityEngine.UI.Text>().text = SpellSystem.GetComponent<SpellHandler>().SpellList[spell].name;
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

    void Start(){
        if(SpellSystem.GetComponent<SpellHandler>().equippedSpell == 0){
            magiclabel.GetComponent<UnityEngine.UI.Text>().text = "No spell prepared";
        } else {
            magiclabel.GetComponent<UnityEngine.UI.Text>().text = SpellSystem.GetComponent<SpellHandler>().SpellList[SpellSystem.GetComponent<SpellHandler>().equippedSpell].name;
        }
    }

    void Update(){
        // hotkey code
        if(spell != 0){
            if((Input.mousePosition.x > transform.position.x-20) && (Input.mousePosition.x < transform.position.x+20) && 
                (Input.mousePosition.y > transform.position.y-20) && (Input.mousePosition.y < transform.position.y+20)){
                    // TODO: FIX THIS YANDEREDEV ASS SHIT LMAOOOOOOOOOOOO
                for(byte i = 0; i < 9; i++){
                    if(Input.GetKeyDown(((i+1)%10).ToString())){
                        hotkeySpell(i);
                    }
                }
            }
        }
    }
}
