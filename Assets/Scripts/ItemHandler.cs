using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Type{
    JUNK,
    FOOD,
    DRINK,
    SCROLL,
    POTION,
    SPELL,
    READ,
    WEAPON,
    ARMOR,
    AMMO,
    INGREDIENT,
    TOOL
}
public enum SubType{        // exclusively for weapons and armor
    NONE, EAR, RING, NECK, BACK, ARROW, BOLT, BULLET, OFF,  // TODO: offhand types
    FIST, STAFF, LPIERCE, HPIERCE, LSLASH, HSLASH, LBLUNT, HBLUNT, LGUN, HGUN, BOW, 
    UFACE, UHEAD, USHOULDER, UCHEST, ULEGS, UWAIST, UFEET, UHANDS, UWRIST,  // cloth
    LFACE, LHEAD, LSHOULDER, LCHEST, LLEGS, LWAIST, LFEET, LHANDS, LWRIST,  // leathers/furs
    HFACE, HHEAD, HSHOULDER, HCHEST, HLEGS, HWAIST, HFEET, HHANDS, HWRIST   // metal/mineral/bone
}
public enum Rarity{
    CURSED,
    JUNK,
    COMMON,
    UNCOMMON,
    RARE,
    EPIC,
    LEGENDARY
}

public class ItemHandler : MonoBehaviour {
    public GameObject cam;
    public GameObject InventoryMenu;
    public bool inventoryShown = false;
    
    AudioClip clip_eat;
    AudioClip clip_drink;
    AudioClip clip_read;
    AudioClip clip_wear;

    public class Item{
        public string iconName = "";
        public string name = "";
        public Type type = Type.JUNK;
        public SubType subtype = SubType.NONE;
        public Rarity rarity = Rarity.COMMON;
        public string description = "missing";
        public float weight = 0.0f;

        public string[] w_sprites = {"", "", ""};
        public int magnitude = 0;       // damage per strike or armor value
        public short delay = 0;                 //  !!! TODO: item spells should be unique !!!
        public short onUse = 0;  // spell added to spellbook when item is in inventory if useCounter > 0
        public short onHit = 0;  // spell cast on target when strike occurs
        public short onWear = 0;  // spell cast on self when worn and purged when unworn
        public byte[] buffs = {0, 0, 0, 0, 0, 0, 0};     // str, con, dex, agi, int, wis, cha
        public byte[] resist = {0, 0, 0, 0, 0, 0};       // holy, fire, frost, arcane, nature, shadow

        public byte useCounter = 0;             // potion doses, food portions, enchantment uses
        public byte current_useCounter = 0;
        public int baseGoldValue = 0;
        public int quantity = 0;

        public Item(){} // nothing
        public Item(string _i, string _name, Rarity r, Type _type, string _desc, float w, int bgv){  // junk and food/drink constructor
            this.iconName = _i;
            this.name = _name;
            this.rarity = r;
            this.type = _type;
            this.description = _desc;
            this.weight = w;
            this.baseGoldValue = bgv;
        }
        public Item(string _i, string _name, Rarity r, Type _type, string _desc, float w, int bgv, short _onUse, byte _uc){ // consumable
            this.iconName = _i;
            this.name = _name;
            this.type = _type;
            this.rarity = r;
            this.description = _desc;
            this.weight = w;
            this.baseGoldValue = bgv;
            this.onUse = _onUse;
            this.useCounter = _uc;
            this.current_useCounter = _uc;
        }
        public Item(string _i, string _name, Rarity r, Type _type, SubType _st, string _desc, float w, int bgv, int mag, short del,  // statless 
                    string[] _w_sprites){
            this.iconName = _i;
            this.name = _name;
            this.type = _type;
            this.subtype = _st;
            this.rarity = r;
            this.description = _desc;
            this.weight = w;
            this.baseGoldValue = bgv;
            this.magnitude = mag;
            this.delay = del;
            this.w_sprites = _w_sprites;
        }
        public Item(string _i, string _name, Rarity r, Type _type, SubType _st, string _desc, float w, int bgv, int mag, short del,   // gear 
                    byte[] _buffs, byte[] _resist, short _onUse, short _onHit, short _onWear, byte _uc, string[] _w_sprites){
            this.iconName = _i;
            this.name = _name;
            this.type = _type;
            this.subtype = _st;
            this.rarity = r;
            this.description = _desc;
            this.weight = w;
            this.baseGoldValue = bgv;
            this.onUse = _onUse;
            this.onHit = _onHit;
            this.onWear = _onWear;
            this.useCounter = _uc;
            this.current_useCounter = _uc;
            this.magnitude = mag;
            this.delay = del;
            this.buffs = _buffs;
            this.resist = _resist;
            this.w_sprites = _w_sprites;
        }
    }
    public List<Item> Inventory = new List<Item>();

    public void drawInventory(){
        if(inventoryShown){
            int itemSlot = 0;
            // render inventory slots
            for(short i = 0; i < Inventory.Count; i++){
                if(Inventory[i].quantity > 0){
                    GameObject slot = InventoryMenu.gameObject.transform.GetChild(3).GetChild(itemSlot).gameObject;
                    slot.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/Icons/" + Inventory[i].iconName);
                    slot.GetComponent<SlotCode>().item = i;
                    if(Inventory[i].quantity == 1){
                        slot.transform.GetChild(0).gameObject.SetActive(false);
                    } else {
                        slot.SetActive(true);
                        slot.transform.GetChild(0).gameObject.SetActive(true);
                        slot.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = Inventory[i].quantity.ToString();
                    }
                    itemSlot++;
                } //else { InventoryMenu.gameObject.transform.GetChild(3).GetChild(itemSlot).gameObject.GetComponent<SlotCode>().item = 0; }
                // render empty inventory slots
                for(int j = itemSlot; j < 36; j++){
                        // TODO: just set that shit to inactive bro
                    InventoryMenu.gameObject.transform.GetChild(3).GetChild(j).gameObject.GetComponent<Image>().sprite = 
                        Resources.Load<Sprite>("Textures/Icons/empty");
                    InventoryMenu.gameObject.transform.GetChild(3).GetChild(j).GetChild(0).gameObject.SetActive(false);
                }
            }
            // render equipment slots
            for(short i = 0; i < 18; i++){
                GameObject slot = InventoryMenu.gameObject.transform.GetChild(4).GetChild(i).gameObject;
                slot.SetActive(slot.GetComponent<SlotCode>().item != 0);
                if(slot.GetComponent<SlotCode>().item != 0){
                    slot.GetComponent<Image>().sprite = 
                        Resources.Load<Sprite>("Textures/Icons/" + Inventory[slot.GetComponent<SlotCode>().item].iconName);
                }
            }
        }
    }

    public void useItem(short id){
        if(Inventory[id].quantity > 0){     // TODO: ADD ITEM EFFECTS
            switch(Inventory[id].type){
                case Type.FOOD:
                    GetComponent<AudioSource>().clip = clip_eat;
                    Debug.Log("DEBUG: hunger sated");
                    Inventory[id].quantity--;
                    break;
                case Type.DRINK:
                    GetComponent<AudioSource>().clip = clip_drink;
                    Debug.Log("DEBUG: thirst quenched");
                    Inventory[id].quantity--;
                    break;
                case Type.SCROLL:
                    Debug.Log("DEBUG: scroll casted");
                    GetComponent<AudioSource>().clip = clip_read;
                    Inventory[id].quantity--;
                    break;
                case Type.POTION:
                    Debug.Log("DEBUG: potion effect used");
                    GetComponent<AudioSource>().clip = clip_drink;
                    Inventory[id].quantity--;
                    break;
                case Type.SPELL:
                    Debug.Log("DEBUG: spell learned");
                    GetComponent<AudioSource>().clip = clip_read;
                    Inventory[id].quantity--;
                    break;
                case Type.READ:
                    Debug.Log("DEBUG: reading menu");
                    GetComponent<AudioSource>().clip = clip_read;
                    break;
                case Type.WEAPON:
                case Type.ARMOR:
        // TODO: check if user can even wear it LMFAOOOOOOOO
                    Debug.Log("DEBUG: equipment worn");
                    GetComponent<AudioSource>().clip = clip_wear;
                    Inventory[id].quantity--;
                    GetComponent<PlayerFunctions>().wear(id);
                    break;
                case Type.INGREDIENT:
                    GetComponent<AudioSource>().clip = clip_eat;
                    Debug.Log("DEBUG: ingredient eaten");
                    Inventory[id].quantity--;
                    break;
                case Type.TOOL:
                    Debug.Log("DEBUG: crafting menu opened");
                    GetComponent<AudioSource>().clip = clip_wear;
                    break;
                default:
                    Debug.Log("DEBUG: consumed unknown item type");
                    break;
            }
            GetComponent<AudioSource>().Play();
            drawInventory();
        }
    }

    void Start(){
        clip_eat = Resources.Load<AudioClip>("Sounds/item/eat");
        clip_drink = Resources.Load<AudioClip>("Sounds/item/drink");
        clip_read = Resources.Load<AudioClip>("Sounds/item/bookpag1");
        clip_wear = Resources.Load<AudioClip>("Sounds/item/clothes");

        Inventory.Add(new Item());  // Inventory[0] skip
        Inventory.Add(new Item("rock", "a rock", Rarity.COMMON, Type.WEAPON, SubType.LBLUNT, "im doing a vidio with it", 0.0f, 0, 5, 20,
                                new string[]{"rock1", "rock2", "rock3"}));
        Inventory.Add(new Item("lean", "a mug of lean", Rarity.COMMON, Type.DRINK, "the drink of the gods", 0.0f, 0));

        // TEST     TODO: REMOVE!!!!!!!
        Inventory[1].quantity++;
        Inventory[2].quantity = 3;
    }

    void Update(){
        if(Input.GetKeyDown("b") || Input.GetKeyDown(KeyCode.Tab)){
            GetComponent<SpellHandler>().spellBookShown = false;
            GetComponent<SpellHandler>().spellbooksprite.SetActive(false);
            inventoryShown = !inventoryShown;
            InventoryMenu.SetActive(inventoryShown);
            cam.GetComponent<MouseLook>().mouselook = !inventoryShown;
            if(cam.GetComponent<MouseLook>().mouselook){ Cursor.lockState = CursorLockMode.Locked; }
            else { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
            drawInventory();
        }
    }
}
