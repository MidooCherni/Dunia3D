using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum School{
    NONE,
    ALT,     // temporary stat buffs
    CON,    // magical transport/summon
    DIV,     // see invis, commune
    EVO,      // energy and element nukes
    ILL,       // niche stealth spells
    NEC,     // health transferral
    RES,    // health restoration
    TRA,  // object transformation
    MUS           // bard only
}
public enum Ele{
    NONE,   // physical
    ARCANE, FORCE,
    FROST, AIR,
        // +nature
    FIRE, LAVA,
        // +nature
    NATURE, LIGHTNING, TOXIN, EARTH, WATER, 
        // +arcane, +shadow
    DEATH, VOID, CHAOS, SPIRIT,
        // +frost, +fire, +holy
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
    SELF, TOUCH, TARGET, PBAE, TAE, AOE
}

public class SpellHandler : MonoBehaviour
{
    public GameObject spellbooksprite;
    public GameObject cam;
    public short equippedSpell = 0;
    public bool spellBookShown = false;

    public short[] hotbar = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

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
    public class Spell{
        public string iconname = "";
        public string name = "";
        public byte level = 0;      // used to calculate mana cost
        public Effect[] effects = {};
        public short[] schoolFamily = {};
        public bool isLearned = false;
        public Spell(){}
        public Spell(string ic, string n, byte l, Effect[] ef){     // isolated spell
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
            this.schoolFamily = sf;
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

    public void performEffect(Effect eff, Target){

    }

    public void tryCast(short spellid){
        if(spellid == 0){ return; } // TODO: tip to equip spell
        // test gcd
        // test mana
        // test target
        if(SpellList[spellid].effects[i].target == Target.SELF)
        // hand animation
        // play sound
        // for loop through effects
        for(int i = 0; i < 4; i++){
            if(SpellList[spellid].effects[i] != n){
                switch(SpellList[spellid].effects[i].type){
                    case EffectType.NUKE:
                        if(SpellList[spellid].effects[i].target == Target.SELF)
                        break;
                    case EffectType.HEAL:
                        break;
                    default:
                    // bug
                        break;
                }
            }
        }
    }

    void Start(){
        Effect n = new Effect();

        SpellList.Add(new Spell());  // Inventory[0] skip
        SpellList.Add(new Spell("fire", "test fireball", 1,
                            new Effect[]{new Effect(Ele.FIRE, EffectType.NUKE, School.EVO, Target.TARGET, 10, 0, 20),n,n,n}));
        SpellList.Add(new Spell("heal", "test healing", 1,
                            new Effect[]{new Effect(Ele.WATER, EffectType.HEAL, School.RES, Target.SELF, 10, 0, 0),n,n,n}));

        // TEST     TODO: REMOVE!!!!!!!
        SpellList[1].isLearned = true;
        SpellList[2].isLearned = true;
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
            // cast equipped spell
        if(Input.GetKeyDown("r")){
            if(equippedSpell != 0){
                tryCast(equippedSpell);
            }
        }
    }
}
