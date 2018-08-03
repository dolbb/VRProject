using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MenuToggle : MonoBehaviour {
    public VRTK_ControllerEvents controllerEvents;
    public GameObject menu_canvas;

    bool menuState = false;

    void OnEnable()
    {
        controllerEvents.ButtonTwoReleased += Controller_Events_ButtonTwoReleased;
    }

    void OnDisable()
    {
        controllerEvents.ButtonTwoReleased -= Controller_Events_ButtonTwoReleased;
    }

    private void Controller_Events_ButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
    {
        menuState = !menuState;
        menu_canvas.SetActive(menuState);
    }
}
