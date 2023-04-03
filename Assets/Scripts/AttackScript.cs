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

    public Sprite spr_idle;
    public Sprite spr_pulled;
    public Sprite spr_recover;

    public AudioClip snd_sheathe;
    public AudioClip snd_unsheathe;
    public AudioClip snd_swing;
    public AudioClip snd_hit;

    public AudioClip snd_punch;
    public AudioClip snd_blade;
    public AudioClip snd_blunt;

    public Status status = Status.IDLE;
    public int attack_cd_CAP = 15;                 // TODO: FETCH FROM PLAYER ATTACK SPEED * 50 / 1000
    public int attack_cd = 15;
    public int attack_value = 0;      // updated on buff gain or loss, debuff gain or loss, wear and unwear

    void Start()
    {
        snd_sheathe = Resources.Load<AudioClip>("Sounds/combat/w_sheathe");
        snd_unsheathe = Resources.Load<AudioClip>("Sounds/combat/w_unsheathe");
        snd_swing = Resources.Load<AudioClip>("Sounds/combat/swing");
        snd_hit = Resources.Load<AudioClip>("Sounds/combat/punch");
        snd_punch = Resources.Load<AudioClip>("Sounds/combat/punch");
        snd_blade = Resources.Load<AudioClip>("Sounds/combat/slice");
        snd_blunt = Resources.Load<AudioClip>("Sounds/combat/blunt");

        weapon.GetComponent<Image>().sprite = spr_idle;
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
            if(unsheathed){
                weapon.GetComponent<Image>().sprite = spr_idle;
            } else {
                weapon.GetComponent<Image>().sprite = spr_idle;
                weapon.SetActive(false);
            }
        }
        if(status == Status.IDLE && attack_cd != attack_cd_CAP){ attack_cd++; }
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            if(status == Status.IDLE && (unsheathed) && player.GetComponent<SpellHandler>().magic_gcd == player.GetComponent<SpellHandler>().magic_gcd_cap){
                status = Status.PULL;
                weapon.GetComponent<Image>().sprite = spr_pulled;
            }
        }
        if(Input.GetMouseButtonUp(0)){
            if(status == Status.PULL){
                RaycastHit hit;
                GetComponent<AudioSource>().clip = snd_swing;
                GetComponent<AudioSource>().Play();
                if(Physics.Raycast(player.transform.position, player.transform.forward, out hit, 6.0f)){
                    if(hit.transform.gameObject.tag == "NPC"){ 
                        if(hit.transform.gameObject.GetComponent<MobBehavior>().status != State.FLINCH && 
                            hit.transform.gameObject.GetComponent<MobBehavior>().status != State.DEAD){
                            // WOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
                                    // TODO: buff and debuff attack_value before calculation
                            hit.transform.gameObject.GetComponent<StatHandler>().CURRENT_HP -= attack_value;
                            hit.transform.gameObject.GetComponent<MobBehavior>().status = State.FLINCH;
                            GetComponent<AudioSource>().clip = snd_hit;
                            GetComponent<AudioSource>().Play();
                        }
                    }
                }
                weapon.GetComponent<Image>().sprite = spr_recover;
                status = Status.RECOVER;
                attack_cd = 0;
                player.GetComponent<SpellHandler>().magic_gcd = (short)(player.GetComponent<SpellHandler>().magic_gcd) - attack_cd_CAP;
            }
        }
        if(Input.GetKeyDown("f")){
            unsheathed = !unsheathed;
            weapon.SetActive(unsheathed);
            if(player.GetComponent<PlayerFunctions>().weaponlabel.GetComponent<UnityEngine.UI.Text>().text != "Fists"){
                if(unsheathed){ GetComponent<AudioSource>().clip = snd_sheathe; } else { GetComponent<AudioSource>().clip = snd_unsheathe; }
                GetComponent<AudioSource>().Play();
            }
        }
    }
}