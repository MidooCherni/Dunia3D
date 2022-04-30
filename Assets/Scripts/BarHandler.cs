using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarHandler : MonoBehaviour
{
    public GameObject ps;   // player script
    public RectTransform hpbar;
    public RectTransform mpbar;
    public RectTransform spbar;

    private int cMAX_HP;
    private int cMAX_MP;
    private int cMAX_SP;

    void Update()
    {
            // setting max to 1 if at 0 to avoid dividing by zero
        cMAX_HP = ps.GetComponent<StatHandler>().CURRENT_HP; if (cMAX_HP < 1) { cMAX_HP = 1; }
        cMAX_MP = ps.GetComponent<StatHandler>().CURRENT_MP; if (cMAX_MP < 1) { cMAX_MP = 1; }
        cMAX_SP = ps.GetComponent<StatHandler>().CURRENT_SP; if (cMAX_SP < 1) { cMAX_SP = 1; }
        hpbar.sizeDelta = new Vector2(ps.GetComponent<StatHandler>().MAX_HP / cMAX_HP * 400, 15);
        mpbar.sizeDelta = new Vector2(ps.GetComponent<StatHandler>().MAX_MP / cMAX_MP * 400, 15);
        spbar.sizeDelta = new Vector2(ps.GetComponent<StatHandler>().MAX_SP / cMAX_SP * 400, 15);
    }
}
