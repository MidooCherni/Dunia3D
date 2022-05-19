using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotCode : MonoBehaviour, IPointerDownHandler, IPointerUpHandler//, IPointerEnterHandler, IPointerExitHandler
{
    public short item = 0;
    public GameObject InventorySystem;

    /*void Start(){
        Button btn = transform.GetComponent<Button>();  
        btn.onClick.AddListener(send);
    } deprecated*/

    public void OnPointerDown(PointerEventData pointerEventData){}
    public void OnPointerUp(PointerEventData pointerEventData){
        if(item != 0){
            if (pointerEventData.button == PointerEventData.InputButton.Right) {
                InventorySystem.GetComponent<ItemList>().useItem(item);
            }
        }
    }

    void Update(){
        if(item != 0){
            if((Input.mousePosition.x > transform.position.x-20) && (Input.mousePosition.x < transform.position.x+20) && 
                (Input.mousePosition.y > transform.position.y-20) && (Input.mousePosition.y < transform.position.y+20)){
                transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = InventorySystem.GetComponent<ItemList>().Inventory[item].name;
                transform.GetChild(1).gameObject.SetActive(true);
            } else {
                transform.GetChild(1).gameObject.SetActive(false);
            }
        } else {
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    /*
    public void OnPointerExit(PointerEventData pointerEventData){
        NameLabel.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData pointerEventData){
        if(item != 0){
            NameLabel.GetComponent<UnityEngine.UI.Text>().text = InventorySystem.GetComponent<ItemList>().Inventory[item].name;
            NameLabel.SetActive(true);
        }
    }*/
}