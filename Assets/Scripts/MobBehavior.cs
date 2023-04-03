using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum State{
    IDLE,
    WANDER,
    CHASE,
    RETURN,
    WINDUP,
    RECOVER,
    FLINCH,
    DEAD
}

public class MobBehavior : MonoBehaviour
{
    public bool wanders;    // check if it's allowed to move around
    public float movespeed = 4.0f;
    byte attackdelay_max = 150;      // 50 = 1 second... last 1/4th 
    byte attackdelay = 150;                 // ATTACK ANIM: 0-50%, idle sprite, 50-75% windup, 75% = do damage, 75-100% recovery
        // TODO: add magic delay

    public Sprite spr_idle;
    public Sprite spr_walk1;
    public Sprite spr_walk2;
    public Sprite spr_wind;
    public Sprite spr_strike;
    public Sprite spr_hit;
    public Sprite spr_dead;

    public GameObject sr;
    public GameObject player;
    public State status = State.IDLE;

    Vector3 originpoint;

    byte animcounter = 0;        // used to time walk cycles out of 25 (50 frames = 1 sec)
    bool step = false;           // single byte to decide current walk sprite 
    public byte flinchtimer = 20;
    public byte flashtimer = 0;
    public Ele flashcolor = Ele.NONE;

    void FixedUpdate(){
        if(flashtimer % 2 == 1){
            switch(flashcolor){
                case Ele.ARCANE:
                case Ele.ASTRAL:
                    sr.GetComponent<SpriteRenderer>().color = new Color(1, 0, 1, 1f);
                    break;
                case Ele.FROST:
                case Ele.AIR:
                case Ele.LIGHTNING:
                case Ele.WATER:
                    sr.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1, 1f);
                    break;
                case Ele.FIRE:
                case Ele.LAVA:
                    sr.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1f);
                    break;
                case Ele.NATURE:
                case Ele.EARTH:
                    sr.GetComponent<SpriteRenderer>().color = new Color(0, 0.75f, 0, 1f);
                    break;
                case Ele.SHADOW:
                case Ele.VOID:
                case Ele.CHAOS:
                case Ele.SPIRIT:
                case Ele.DEATH:
                    sr.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 0.5f, 1f);
                    break;
                case Ele.HOLY:
                case Ele.SUNLIGHT:
                    sr.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 1f);
                    break;
                case Ele.MOONLIGHT:
                    sr.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
                    break;
                case Ele.FORCE:
                default:
                    sr.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
                    break;
            }
        } else {
            sr.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        }
        if(flashtimer > 0){ flashtimer--; }

        switch(status){
            case State.WANDER:
            case State.CHASE:
            case State.RETURN:
                animcounter++;
                if(animcounter == 10){
                    animcounter = 0;
                    step = !step;
                }
                break;
            case State.WINDUP:
                if((float)attackdelay / (float)attackdelay_max < 0.5f){
                    sr.GetComponent<SpriteRenderer>().sprite = spr_idle;
                } else if ((float)attackdelay / (float)attackdelay_max < 0.90f){
                    sr.GetComponent<SpriteRenderer>().sprite = spr_wind;
                } else if ((float)attackdelay / (float)attackdelay_max >= 0.90f){
                    sr.GetComponent<SpriteRenderer>().sprite = spr_strike;
                    // TODO: CHANGE DAMAGE CHECK TO INCLUDE ARMOR EFFECTS!!!!!!       TODO: DEATH CODE
                    player.GetComponent<StatHandler>().CURRENT_HP -= GetComponent<StatHandler>().STR;
                    status = State.RECOVER;
                }
                attackdelay++;
                break;
            case State.RECOVER:
                attackdelay++;
                if(attackdelay == attackdelay_max){
                    attackdelay = 0;
                    status = State.WINDUP;
                }
                break;
            case State.FLINCH:
                flinchtimer--;
                if(flinchtimer == 0){
                    flinchtimer = 20;
                    status = State.IDLE;
                }
                break;
            default:
                break;
        }
    }

    void Start()
    {
        attackdelay = 0;
        originpoint = sr.transform.position;
        if(wanders){
            status = State.WANDER;
        }
    }

    void Update()
    {
        if(GetComponent<StatHandler>().CURRENT_HP <= 0){
                // TODO: make lootable
            status = State.DEAD;
        }

        sr.transform.eulerAngles = new Vector3 (0, Camera.main.transform.eulerAngles.y - 360, 0);      // keep sprite from tilting or rolling
        switch(status){
            case State.IDLE:
                sr.GetComponent<SpriteRenderer>().sprite = spr_idle;
                if(Vector3.Distance(transform.position, player.transform.position) < 15.0f){
                    status = State.CHASE;
                }
                break;
            case State.WANDER:
                if(step){
                    sr.GetComponent<SpriteRenderer>().sprite = spr_walk1;
                } else {
                    sr.GetComponent<SpriteRenderer>().sprite = spr_walk2;
                }   // TODO: implement wander code
                break;
            case State.CHASE:
                if(step){
                    sr.GetComponent<SpriteRenderer>().sprite = spr_walk1;
                } else {
                    sr.GetComponent<SpriteRenderer>().sprite = spr_walk2;
                }
                if(Vector3.Distance(transform.position, player.transform.position) > 2.5f){
		            if(GetComponent<StatHandler>().isStunned || GetComponent<StatHandler>().isRooted ){ return; }
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 4.0f * Time.deltaTime);
                } else if(Vector3.Distance(transform.position, player.transform.position) <= 3.0f){
                    status = State.WINDUP;
                }
                    // TODO: CHANGE THIS SHIT!!!!!!! ONLY RESET AGGRO ON ZONE OUT
                if(Vector3.Distance(transform.position, player.transform.position) > 30.0f){
                    status = State.RETURN;
                }
                break;
            case State.RETURN:
                if(step){
                    sr.GetComponent<SpriteRenderer>().sprite = spr_walk1;
                } else {
                    sr.GetComponent<SpriteRenderer>().sprite = spr_walk2;
                }
                    // still not there
                if(Vector3.Distance(transform.position, originpoint) > 0.001f){
		            if(GetComponent<StatHandler>().isStunned || GetComponent<StatHandler>().isRooted){ return; }
                    transform.position = Vector3.MoveTowards(transform.position, originpoint, 2.0f * Time.deltaTime);
                } else {
                    status = State.IDLE;
                }
                break;
            case State.WINDUP:
                if(Vector3.Distance(transform.position, player.transform.position) > 2.5f){
                    status = State.CHASE;
                }
                break;
            case State.FLINCH:
                sr.GetComponent<SpriteRenderer>().sprite = spr_hit;
                break;
            case State.DEAD:
                sr.GetComponent<SpriteRenderer>().sprite = spr_dead;
                break;
            default:
                break;
        }
    }
}
