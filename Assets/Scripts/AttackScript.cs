using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Status{
    IDLE,
    PULL,
    RECOVER
}

public class AttackScript : MonoBehaviour
{
    public bool unsheathed = false;
    public GameObject player;
    public GameObject weapon;

    public Sprite spr_idle;                             // TODO: Equip code will change these three
    public Sprite spr_pulled;
    public Sprite spr_recover;

    public Status status = Status.IDLE;
    public short attack_cd_CAP = 15;                 // TODO: FETCH FROM PLAYER ATTACK SPEED * 50 / 1000
    public short attack_cd = 15;

    void Start()
    {
        if(unsheathed){
            weapon.SetActive(true);
        } else {
            weapon.SetActive(false);
        }
    }

    void FixedUpdate(){
        if(status == Status.RECOVER && attack_cd != attack_cd_CAP){ attack_cd++; }
        if(status == Status.RECOVER && attack_cd == attack_cd_CAP){
            status = Status.IDLE;
            weapon.GetComponent<Image>().sprite = spr_idle;
        }
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            if(status == Status.IDLE){
                status = Status.PULL;
                weapon.GetComponent<Image>().sprite = spr_pulled;
            }
        }
        if(Input.GetMouseButtonUp(0)){
            if(status == Status.PULL){
                RaycastHit hit;
                if(Physics.Raycast(player.transform.position, player.transform.forward, out hit, 6.0f)){
                    if(hit.transform.gameObject.tag == "NPC"){ 
                        if(hit.transform.gameObject.GetComponent<MobBehavior>().status != State.FLINCH && 
                            hit.transform.gameObject.GetComponent<MobBehavior>().status != State.DEAD){
                            // WOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
                            hit.transform.gameObject.GetComponent<StatHandler>().CURRENT_HP -= player.GetComponent<StatHandler>().STR;
                            hit.transform.gameObject.GetComponent<MobBehavior>().status = State.FLINCH;
                        }
                    }
                }
                weapon.GetComponent<Image>().sprite = spr_recover;
                status = Status.RECOVER;
                attack_cd = 0;
            }
        }
        if(Input.GetKeyDown("f")){
            unsheathed = !unsheathed;
            weapon.SetActive(unsheathed);
        }
    }
}