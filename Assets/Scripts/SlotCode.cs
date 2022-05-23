using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotCode : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public short item = 0;
    public GameObject InventorySystem;

    public void OnPointerDown(PointerEventData pointerEventData){}
    public void OnPointerUp(PointerEventData pointerEventData){
        if(item != 0){
            if (pointerEventData.button == PointerEventData.InputButton.Right) {
                if(gameObject.tag == "EquipSlot"){
                    InventorySystem.GetComponent<ItemHandler>().Inventory[item].quantity++;
                    GameObject.Find("Player").GetComponent<PlayerFunctions>().unwear(item);
                    item = 0;
                    InventorySystem.GetComponent<ItemHandler>().drawInventory();
                } else {
                    InventorySystem.GetComponent<ItemHandler>().useItem(item);
                }
            }
        }
    }

    void Update(){
        // label rendering code
        byte i = 1; // not an equipslot
        if(gameObject.tag == "EquipSlot"){
            i = 0;
        }
        if(item != 0){
            if((Input.mousePosition.x > transform.position.x-20) && (Input.mousePosition.x < transform.position.x+20) && 
                (Input.mousePosition.y > transform.position.y-20) && (Input.mousePosition.y < transform.position.y+20)){
                transform.GetChild(i).GetComponent<UnityEngine.UI.Text>().text = InventorySystem.GetComponent<ItemHandler>().Inventory[item].name;
                transform.GetChild(i).gameObject.SetActive(true);
            } else {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        } else {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}