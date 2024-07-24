using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelectState : HexSelectState
{
    private MenuSelect menuSelect;
    private MenuHighlight menuHighlight;
    public override void EnterState(HexSelectManager manager)
    {
        //enables buttons, disables other selections.
        //very basic atm, will need to split ui elements from perment fixtures to the ones needed here

        manager.UI.enabled = true;
        menuSelect = manager.GetComponent<MenuSelect>();
        menuHighlight = manager.GetComponent<MenuHighlight>();
        manager.Responce = menuSelect;
    }

    public override void ExitState(HexSelectManager manager)
    {
        //disables buttons
        manager.UI.enabled = false;
    }

    public override void UpdateState(HexSelectManager manager)
    {
        // for consideration: still allows mouse hex selection
        //otherwise should remain blank/allow button selection rollover from bottom and top both 
        //are options as unity handles button selection for key board. May have to configure for gamepads.
    }
}
