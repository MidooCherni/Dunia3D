using System.IO.Pipes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarHandler : MonoBehaviour
{
    public GameObject ps;   // player script
    public RectTransform hpbar;
    public RectTransform mpbar;
    public RectTransform spbar;

    public GameObject spelllabel;

    private float cMAX_HP;
    private float cMAX_MP;
    private float cMAX_SP;

    void Update()
    {
            // setting max to 1 if at 0 to avoid dividing by zero
        cMAX_HP = (float)ps.GetComponent<StatHandler>().CURRENT_HP; if (cMAX_HP < 1) { cMAX_HP = 1; }
        cMAX_MP = (float)ps.GetComponent<StatHandler>().CURRENT_MP; if (cMAX_MP < 1) { cMAX_MP = 1; }
        cMAX_SP = (float)ps.GetComponent<StatHandler>().CURRENT_SP; if (cMAX_SP < 1) { cMAX_SP = 1; }
        hpbar.sizeDelta = new Vector2((cMAX_HP / (float)ps.GetComponent<StatHandler>().MAX_HP) * 400, 15);
        mpbar.sizeDelta = new Vector2((cMAX_MP / (float)ps.GetComponent<StatHandler>().MAX_MP) * 400, 15);
        spbar.sizeDelta = new Vector2((cMAX_SP / (float)ps.GetComponent<StatHandler>().MAX_SP) * 400, 15);
    }
}
