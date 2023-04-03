using System.Diagnostics.Contracts;
using System.Security;
using System;
using System.Globalization;
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

public enum Target{
    SELF, TOUCH, TARGET, PBAE, TAE, AOE, PARTY
}

public enum EffectType{
    NONE,
    NUKE, HEAL,
    BUFF, DEBUFF,
    FEATHER, THORNS, INVIS, FLY, IMMUNE, PULSEGOOD,             // TODO: WATERBREATHE
    REMTOXIN, REMSPIRIT, REMMAGIC, FREEMOVE, FREEACT,
    CONJUREMOB, CONJUREITEM, TELEPORT, BIND, GATE, OPEN, EXTRAINVENTORY
}

public enum Stats{
    HP, MP, STAM, ARMOR, STR, CON, DEX, AGI, INT, WIS, CHA, RES_HOLY, RES_FIRE, RES_FROST, RES_ARCANE, RES_NATURE, RES_SHADOW,
    DOT, HOT, CRACK,
    ROOT, STUN, FEAR, CHARM, COMMAND, SILENCE, PULSEBAD,
    FEATHER, THORNS, INVIS, FLY, IMMUNE, PULSEGOOD,             // TODO: WATERBREATHE
}

public enum DispellType{
    IMPOSSIBLE, PHYSICAL, MAGICAL, POISON, DISEASE, CURSE
}

public class SpellHandler : MonoBehaviour
{
    public GameObject spellbooksprite;
    public GameObject cam;
    public GameObject player;
    public GameObject weapon;

    public GameObject magiclabel;

    public short equippedSpell = 0;
    public bool spellBookShown = false;

    public int magic_gcd_cap = 100;
    public int magic_gcd = 100;

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
    
    public AudioClip snd_fire;
    public AudioClip snd_ice;
    public AudioClip snd_evil;
    public AudioClip snd_arc;
    public AudioClip snd_holy;
    public AudioClip snd_light;
    public AudioClip snd_nature;
    public AudioClip snd_zap;
    public AudioClip snd_water;

        // TODO: non-magical ability sounds

    public class Effect{
        public Ele element = Ele.NONE;
        public EffectType type = EffectType.NONE;
        public Stats subtype = Stats.HP;
        public School school = School.NONE;
        public Target target = Target.SELF;
        public int magnitude = 0;   // points or id of summoned item/mob
        public int duration = 0;    // in milliseconds... 0 = immediate
        public int distance = 0;    // for AOEs
        public Effect(){}
        public Effect(Ele e, EffectType et, School sc, Target t, int m){ // instant
            this.element = e;
            this.type = et;
            this.school = sc;
            this.target = t;
            this.magnitude = m;
        }
        public Effect(Ele e, EffectType et, School sc, Target t, int m, int d){ // no distance
            this.element = e;
            this.type = et;
            this.school = sc;
            this.target = t;
            this.magnitude = m;
            this.duration = d / 20;
        }
        public Effect(Ele e, EffectType et, Stats st, School sc, Target t, int m, int d){ // buff
            this.element = e;
            this.type = et;
            this.subtype = st;
            this.school = sc;
            this.target = t;
            this.magnitude = m;
            this.duration = d / 20;
        }
        public Effect(Ele e, EffectType et, School sc, Target t, int m, int d, int di){ // distant spell
            this.element = e;
            this.type = et;
            this.school = sc;
            this.target = t;
            this.magnitude = m;
            this.duration = d / 20;
            this.distance = di;
        }
        public Effect(Ele e, EffectType et, Stats st, School sc, Target t, int m, int d, int di){   // distant buff
            this.element = e;
            this.type = et;
            this.subtype = st;
            this.school = sc;
            this.target = t;
            this.magnitude = m;
            this.duration = d / 20;
            this.distance = di;
        }
    }

    public class Spell{
        public string iconname = "";
        public string name = "";
        public byte level = 0;      // used to calculate mana cost
        public Effect[] effects = {};
        public DispellType dt = DispellType.MAGICAL;
        public short[] spellFamily = {};    // check if casting buff or debuff
        public bool isLearned = false;
        public bool isDebuff = false;
        public Spell(){}
        public Spell(string ic, string n, byte l, Effect[] ef){     // self or targetless
            this.iconname = ic;
            this.name = n;
            this.level = l;
            this.effects = ef;
        }
        public Spell(string ic, string n, byte l, Effect[] ef, short[] sf){
            this.iconname = ic;
            this.name = n;
            this.level = l;
            this.effects = ef;
            this.spellFamily = sf;
        }
        public Spell(string ic, string n, byte l, Effect[] ef, DispellType d){
            this.iconname = ic;
            this.name = n;
            this.level = l;
            this.effects = ef;
            this.dt = d;
        }
        public Spell(string ic, string n, byte l, Effect[] ef, DispellType d, short[] sf){
            this.iconname = ic;
            this.name = n;
            this.level = l;
            this.effects = ef;
            this.spellFamily = sf;
            this.dt = d;
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
        if(GetComponent<StatHandler>().isStunned || GetComponent<StatHandler>().isSilenced){ return; }
        if(cam.GetComponent<AttackScript>().attack_cd < cam.GetComponent<AttackScript>().attack_cd_CAP){ return; }
        if(magic_gcd < magic_gcd_cap){ return; }
            // test and consume mana
        if(GetComponent<StatHandler>().CURRENT_MP < SpellList[spellid].level*10){ return; } // TODO: NOOOOOOOO LMFAOOOOOOOOOOOOO CHANGE THIS SHIT ASAP
        GetComponent<StatHandler>().CURRENT_MP -= SpellList[spellid].level*10;
        weapon.SetActive(true);
        switch(SpellList[spellid].effects[0].element){
            case Ele.FIRE:
            case Ele.LAVA:
                weapon.GetComponent<Image>().sprite = spr_firecast;
                GetComponent<AudioSource>().clip = snd_fire;
                break;
            case Ele.FROST:
            case Ele.AIR:
                weapon.GetComponent<Image>().sprite = spr_frostcast;
                GetComponent<AudioSource>().clip = snd_ice;
                break;
            case Ele.ARCANE:
            case Ele.FORCE:
            case Ele.ASTRAL:
                weapon.GetComponent<Image>().sprite = spr_arcanecast;
                GetComponent<AudioSource>().clip = snd_arc;
                break;
            case Ele.NATURE:
            case Ele.EARTH:
                weapon.GetComponent<Image>().sprite = spr_naturecast;
                GetComponent<AudioSource>().clip = snd_nature;
                break;
            case Ele.LIGHTNING:
                weapon.GetComponent<Image>().sprite = spr_thundercast;
                GetComponent<AudioSource>().clip = snd_zap;
                break;
            case Ele.WATER:
                weapon.GetComponent<Image>().sprite = spr_watercast;
                GetComponent<AudioSource>().clip = snd_water;
                break;
            case Ele.DEATH:
            case Ele.VOID:
            case Ele.CHAOS:
            case Ele.SPIRIT:
                weapon.GetComponent<Image>().sprite = spr_shadowcast;
                GetComponent<AudioSource>().clip = snd_evil;
                break;
            case Ele.HOLY:
                weapon.GetComponent<Image>().sprite = spr_holycast;
                GetComponent<AudioSource>().clip = snd_holy;
                break;
            case Ele.MOONLIGHT:
                weapon.GetComponent<Image>().sprite = spr_mooncast;
                GetComponent<AudioSource>().clip = snd_light;
                break;
            case Ele.SUNLIGHT:
                weapon.GetComponent<Image>().sprite = spr_lightcast;
                GetComponent<AudioSource>().clip = snd_light;
                break;
            case Ele.NONE:
            default:
                break;
        }
        GetComponent<AudioSource>().Play();
            // trigger cds
        cam.GetComponent<AttackScript>().status = Status.RECOVER;
        cam.GetComponent<AttackScript>().attack_cd = cam.GetComponent<AttackScript>().attack_cd - magic_gcd_cap;
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
                if(target.GetComponent<MobBehavior>()){
                    target.GetComponent<MobBehavior>().flashcolor = f.element;
                    target.GetComponent<MobBehavior>().flashtimer = 11;  // 4 flashing frames
                }
                switch(f.type){
                    case EffectType.NONE:
                        Debug.Log("BROKEN SPELL WITH NO EFFECT JUST CAST");
                        break;
                    case EffectType.NUKE:
                        if(target.GetComponent<MobBehavior>().status != State.DEAD){
                                // TODO: calculate resist
                            target.GetComponent<StatHandler>().CURRENT_HP -= f.magnitude;
                            target.GetComponent<MobBehavior>().status = State.FLINCH;
                        }
                        break;
                    case EffectType.HEAL:
                        target.GetComponent<StatHandler>().CURRENT_HP += f.magnitude;
                        if(target.GetComponent<StatHandler>().CURRENT_HP > target.GetComponent<StatHandler>().MAX_HP){
                            target.GetComponent<StatHandler>().CURRENT_HP = target.GetComponent<StatHandler>().MAX_HP;
                        }
                        break;
                    default:        // saves a lot of space
                        target.GetComponent<StatHandler>().addAura(spellid, f.type, f.subtype, f.magnitude, f.duration, SpellList[spellid].isDebuff,
                            SpellList[spellid].spellFamily, SpellList[spellid].dt);
                        break;
                }
            }
        }
    }

    void Start(){
        snd_fire = Resources.Load<AudioClip>("Sounds/magic/firespell");
        snd_ice = Resources.Load<AudioClip>("Sounds/magic/frostspell");
        snd_evil = Resources.Load<AudioClip>("Sounds/magic/evilspell");
        snd_arc = Resources.Load<AudioClip>("Sounds/magic/arcanespell");
        snd_holy = Resources.Load<AudioClip>("Sounds/magic/holyspell");
        snd_light = Resources.Load<AudioClip>("Sounds/magic/lightspell");
        snd_nature = Resources.Load<AudioClip>("Sounds/magic/naturespell");
        snd_zap = Resources.Load<AudioClip>("Sounds/magic/thunderspell");
        snd_water = Resources.Load<AudioClip>("Sounds/magic/waterspell");

        if(equippedSpell == 0){
            magiclabel.GetComponent<UnityEngine.UI.Text>().text = "No spell prepared";
        } else {
            magiclabel.GetComponent<UnityEngine.UI.Text>().text = SpellList[equippedSpell].name;
        }

        SpellList.Add(new Spell());  // Inventory[0] skip
        SpellList.Add(new Spell("fire", "test fireball", 1,
                            new Effect[]{new Effect(Ele.FIRE, EffectType.NUKE, School.EVO, Target.TARGET, 10, 0, 20)}));
        SpellList.Add(new Spell("heal", "test healing", 1,
                            new Effect[]{new Effect(Ele.NATURE, EffectType.HEAL, School.RES, Target.SELF, 10)}));
        SpellList.Add(new Spell("water", "test regen", 1,
                            new Effect[]{new Effect(Ele.WATER, EffectType.BUFF, Stats.HOT, School.RES, Target.SELF, 3, 5000)}, new short[]{3}));
        SpellList.Add(new Spell("evil", "test darkness", 1,
                            new Effect[]{new Effect(Ele.DEATH, EffectType.DEBUFF, Stats.DOT, School.ALT, Target.TARGET, 3, 5000)}, new short[]{4}));
        SpellList.Add(new Spell("wind", "test thunder", 1,
                            new Effect[]{new Effect(Ele.LIGHTNING, EffectType.NUKE, School.EVO, Target.TARGET, 10, 0, 20)}));
        SpellList.Add(new Spell("shield", "test holy shield", 1,
                            new Effect[]{new Effect(Ele.HOLY, EffectType.BUFF, Stats.ARMOR, School.ABJ, Target.SELF, 200, 30000)}, new short[]{6}));
        SpellList.Add(new Spell("earf", "test strength of earth", 1,
                            new Effect[]{new Effect(Ele.EARTH, EffectType.BUFF, Stats.STR, School.ALT, Target.SELF, 5, 30000)}, new short[]{7}));
        // TEST     TODO: REMOVE!!!!!!!
        SpellList[1].isLearned = true;
        SpellList[2].isLearned = true;
        SpellList[3].isLearned = true;
        SpellList[4].isLearned = true;
        SpellList[5].isLearned = true;
        SpellList[6].isLearned = true;
        SpellList[7].isLearned = true;
    }

    void FixedUpdate(){
        if(cam.GetComponent<AttackScript>().status == Status.RECOVER && magic_gcd != magic_gcd_cap){ magic_gcd++; }
        if(cam.GetComponent<AttackScript>().status == Status.RECOVER && magic_gcd == magic_gcd_cap){
            cam.GetComponent<AttackScript>().status = Status.IDLE;
            if(cam.GetComponent<AttackScript>().unsheathed){
                weapon.GetComponent<Image>().sprite = cam.GetComponent<AttackScript>().spr_idle;
            } else {
                weapon.GetComponent<Image>().sprite = cam.GetComponent<AttackScript>().spr_idle;
                weapon.SetActive(false);
            }
        }
        if(cam.GetComponent<AttackScript>().status == Status.IDLE && magic_gcd != magic_gcd_cap){ magic_gcd++; }
    }

    void Update(){
        if(equippedSpell != 0){
            if(player.GetComponent<StatHandler>().CURRENT_MP < (SpellList[equippedSpell].level*10)){
                magiclabel.GetComponent<UnityEngine.UI.Text>().color = new Color(1, 1, 1, 0.3f);
            } else {
                magiclabel.GetComponent<UnityEngine.UI.Text>().color = new Color(1, 1, 1, 1);
            }
        } else {
            magiclabel.GetComponent<UnityEngine.UI.Text>().color = new Color(1, 1, 1, 1);
        }
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
}
