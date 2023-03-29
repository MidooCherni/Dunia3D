using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum School{
    NONE,
    ABJ,    // magical shield spells
    ALT,    // temporary stat buffs
    CON,    // magical transport/summon
    DIV,    // see invis, commune
    EVO,    // energy and element nukes
    ILL,    // niche stealth spells
    NEC,    // health transferral
    RES,    // health restoration
    TRA,    // object transformation
    MUS     // bard only
}
public enum Ele{
    NONE,   // physical
    ARCANE, FORCE,
    FROST, AIR,
        // +nature
    FIRE, LAVA,
        // +nature
    NATURE, LIGHTNING, EARTH, WATER, 
        // +arcane, 
    SHADOW, VOID, CHAOS, SPIRIT, DEATH,
        // +frost, +fire, +holy, +nature
    HOLY, MOONLIGHT, SUNLIGHT, ASTRAL
        // +nature, +fire, +arcane
}
public enum EffectType{
    NONE,
    NUKE, HEAL,
    DOT, HOT, CRACK,     // TODO: HASTEUP
    STRDN, CONDN, DEXDN, AGIDN, INTDN, WISDN, CHADN, ATKDN, ARMORDN, SPEEDDN, FIREDN, FROSTDN, ARCDN, NATDN, HOLYDN, SHADDN,
    STRUP, CONUP, DEXUP, AGIUP, INTUP, WISUP, CHAUP, ATKUP, ARMORUP, SPEEDUP, FIREUP, FROSTUP, ARCUP, NATUP, HOLYUP, SHADUP,
    ROOT, FREEZE, FEAR, CHARM, COMMAND, SILENCE, PULSEBAD,
    FEATHER, THORNS, INVIS, FLY, IMMUNE, PULSEGOOD,             // TODO: WATERBREATHE
    REMTOXIN, REMSPIRIT, REMMAGIC, FREEMOVE, FREEACT,
    CONJUREMOB, CONJUREITEM, TELEPORT, BIND, GATE, OPEN, EXTRAINVENTORY
}
public enum Target{
    SELF, TOUCH, TARGET, PBAE, TAE, AOE, PARTY
}

public class SpellHandler : MonoBehaviour
{
    public GameObject spellbooksprite;
    public GameObject cam;
    public GameObject player;
    public GameObject weapon;

    public short equippedSpell = 0;
    public short gcdCap = 50;           // 1 second spell delay
    public short gcdTimer = 50;
    public bool spellBookShown = false;

    public short magic_gcd_cap = 15;
    public short magic_gcd = 15;

    public short[] hotbar = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

    public Sprite spr_firecast;
    public Sprite spr_frostcast;
    public Sprite spr_arcanecast;
    public Sprite spr_naturecast;
    public Sprite spr_thundercast;
    public Sprite spr_watercast;
    public Sprite spr_shadowcast;
    public Sprite spr_holycast;
    public Sprite spr_lightcast;
    public Sprite spr_mooncast;

    public AudioClip bookopen;
    public AudioClip pageflip;
    
    public AudioClip clip_casting;
    public AudioClip clip_castfail;
    
    public AudioClip snd_fire;
    public AudioClip snd_ice;
    public AudioClip snd_wind;
    public AudioClip snd_evil;
    public AudioClip snd_arc;
    public AudioClip snd_holy;
    public AudioClip snd_plant;
    public AudioClip snd_fairy;
    public AudioClip snd_zap;
    public AudioClip snd_earth;
    public AudioClip snd_water;

        // TODO: non-magical ability sounds

    public class Effect{
        public Ele element = Ele.NONE;
        public EffectType type = EffectType.NONE;
        public School school = School.NONE;
        public Target target = Target.SELF;
        public int magnitude = 0;   // points or id of summoned item/mob
        public int duration = 0;    // in milliseconds... 0 = immediate
        public int distance = 0;    // for AOEs
        public Effect(){}
        public Effect(Ele e, EffectType et, School sc, Target t, int m, int d, int di){
            this.element = e;
            this.type = et;
            this.school = sc;
            this.target = t;
            this.magnitude = m;
            this.duration = d / 20;
            this.distance = di;
        }
    }
    Effect n = new Effect();

    public class Spell{
        public string iconname = "";
        public string name = "";
        public byte level = 0;      // used to calculate mana cost
        public bool requiresTarget = false;
        public Effect[] effects = {};
        public short[] spellFamily = {};    // check if casting buff or debuff
        public bool isLearned = false;
        public Spell(){}
        public Spell(string ic, string n, byte l, Effect[] ef){     // targetless
            this.iconname = ic;
            this.name = n;
            this.level = l;
            this.effects = ef;
        }
        public Spell(string ic, string n, byte l, Effect[] ef, bool rt){     // isolated spell
            this.iconname = ic;
            this.name = n;
            this.level = l;
            this.effects = ef;
            this.requiresTarget = rt;
        }
        public Spell(string ic, string n, byte l, Effect[] ef, bool rt, short[] sf){
            this.iconname = ic;
            this.name = n;
            this.level = l;
            this.effects = ef;
            this.requiresTarget = rt;
            this.spellFamily = sf;
        }
    }
    public List<Spell> SpellList = new List<Spell>();

    public void drawSpellbook(){
        if(spellBookShown){
            int bookSlot = 0;
            // render spellbook slots
            for(short i = 0; i < SpellList.Count; i++){
                if(SpellList[i].isLearned){
                    GameObject slot = spellbooksprite.transform.GetChild(1).GetChild(bookSlot).gameObject;
                    slot.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/Spells/" + SpellList[i].iconname.ToString());
                    slot.GetComponent<SlotCodeBook>().spell = i;
                    slot.SetActive(true);
                    slot.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = SpellList[i].name.ToString();
                    bookSlot++;
                }
                for(int j = bookSlot; j < 12; j++){
                    spellbooksprite.transform.GetChild(1).GetChild(j).gameObject.SetActive(false);
                }
            }
        }
    }

    public void tryCast(short spellid){
        if(spellid == 0){ return; } // TODO: tip to equip spell
        if(cam.GetComponent<AttackScript>().attack_cd < cam.GetComponent<AttackScript>().attack_cd_CAP){ return; }
        if(magic_gcd < magic_gcd_cap){ return; }
            // test and consume mana
        if(GetComponent<StatHandler>().CURRENT_MP < SpellList[spellid].level*10){ return; } // TODO: NOOOOOOOO LMFAOOOOOOOOOOOOO CHANGE THIS SHIT ASAP
        GetComponent<StatHandler>().CURRENT_MP -= SpellList[spellid].level*10;
            // change icon
        weapon.SetActive(true);
        switch(SpellList[spellid].effects[0].element){
            case Ele.FIRE:
            case Ele.LAVA:
                weapon.GetComponent<Image>().sprite = spr_firecast;
                break;
            case Ele.FROST:
            case Ele.AIR:
                weapon.GetComponent<Image>().sprite = spr_frostcast;
                break;
            case Ele.ARCANE:
            case Ele.FORCE:
            case Ele.ASTRAL:
                weapon.GetComponent<Image>().sprite = spr_arcanecast;
                break;
            case Ele.NATURE:
            case Ele.TOXIN:
            case Ele.EARTH:
                weapon.GetComponent<Image>().sprite = spr_naturecast;
                break;
            case Ele.LIGHTNING:
                weapon.GetComponent<Image>().sprite = spr_thundercast;
                break;
            case Ele.WATER:
                weapon.GetComponent<Image>().sprite = spr_watercast;
                break;
            case Ele.DEATH:
            case Ele.VOID:
            case Ele.CHAOS:
            case Ele.SPIRIT:
                weapon.GetComponent<Image>().sprite = spr_shadowcast;
                break;
            case Ele.HOLY:
                weapon.GetComponent<Image>().sprite = spr_holycast;
                break;
            case Ele.MOONLIGHT:
                weapon.GetComponent<Image>().sprite = spr_mooncast;
                break;
            case Ele.SUNLIGHT:
                weapon.GetComponent<Image>().sprite = spr_lightcast;
                break;
            case Ele.NONE:
            default:
                break;
        }
            // trigger cds
        cam.GetComponent<AttackScript>().status = Status.RECOVER;
        cam.GetComponent<AttackScript>().attack_cd = 0;
        magic_gcd = 0;
            // test target and perform
        foreach(Effect f in SpellList[spellid].effects){
            List<GameObject> targets = new List<GameObject>();
            if (f.target == Target.SELF){ targets.Add(player); }
            else if (f.target == Target.TOUCH){
                RaycastHit hit;
                if(Physics.Raycast(player.transform.position, player.transform.forward, out hit, 6.0f)){
                    if(hit.transform.gameObject.tag == "NPC"){ 
                        targets.Add(hit.transform.gameObject);
                    }
                }
            }
            else if (f.target == Target.TARGET){
                RaycastHit hit;
                if(Physics.Raycast(player.transform.position, player.transform.forward, out hit, 40.0f)){
                    if(hit.transform.gameObject.tag == "NPC"){ 
                        targets.Add(hit.transform.gameObject);
                    }
                }
            } // TODO: aoe spells
            foreach(GameObject target in targets){ 
                Debug.Log("performing effect " + f);
            }
        }
    }

    void Start(){
        SpellList.Add(new Spell());  // Inventory[0] skip
        SpellList.Add(new Spell("fire", "test fireball", 1,
                            new Effect[]{new Effect(Ele.FIRE, EffectType.NUKE, School.EVO, Target.TARGET, 10, 0, 20),n,n,n}));
        SpellList.Add(new Spell("heal", "test healing", 1,
                            new Effect[]{new Effect(Ele.WATER, EffectType.HEAL, School.RES, Target.SELF, 10, 0, 0),n,n,n}));

        // TEST     TODO: REMOVE!!!!!!!
        SpellList[1].isLearned = true;
        SpellList[2].isLearned = true;
    }

    void FixedUpdate(){
        if(cam.GetComponent<AttackScript>().status == Status.RECOVER && magic_gcd != magic_gcd_cap){ magic_gcd++; }
        if(cam.GetComponent<AttackScript>().status == Status.RECOVER && magic_gcd == magic_gcd_cap){
            cam.GetComponent<AttackScript>().status = Status.IDLE;
            if(cam.GetComponent<AttackScript>().unsheathed){
                weapon.GetComponent<Image>().sprite = cam.GetComponent<AttackScript>().spr_idle;
            } else {
                weapon.SetActive(false);
            }
        }
        if(cam.GetComponent<AttackScript>().status == Status.IDLE && magic_gcd != magic_gcd_cap){ magic_gcd++; }
    }

    void Update(){
            // open spellbook
        if(Input.GetKeyDown("a")){
            GetComponent<AudioSource>().clip = bookopen;
            GetComponent<AudioSource>().Play();
            GetComponent<ItemHandler>().inventoryShown = false;
            GetComponent<ItemHandler>().InventoryMenu.SetActive(false);
            spellBookShown = !spellBookShown;
            spellbooksprite.SetActive(spellBookShown);
            cam.GetComponent<MouseLook>().mouselook = !spellBookShown;
            if(cam.GetComponent<MouseLook>().mouselook){ Cursor.lockState = CursorLockMode.Locked; }
            else { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
            drawSpellbook();
        }
            // change spell
        for(int i = 0; i < 9; i++){
            if(Input.GetKeyDown(((i+1)%10).ToString())){
                if(hotbar[i] != 0){
                    equippedSpell = hotbar[i];
                }
            }
        }
            // cast equipped spell
        if(Input.GetKeyDown("r")){
            if(equippedSpell != 0){
                tryCast(equippedSpell);
            }
        }
    }

    void FixedUpdate(){
            // gcd handling
        if (gcdTimer != gcdCap){ gcdTimer++; }
    }
}
