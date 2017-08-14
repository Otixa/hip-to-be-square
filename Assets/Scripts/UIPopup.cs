﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIPopup  {

    protected GameObject Instance;

    public UIPopup(string prefabName)
    {
        Instance = UIManager.Instance.CreatePopupByResourceName(prefabName);
        UIManager.OnDialogDismiss += OnDialogDismiss;
    }

  
    public virtual void Show()
    {
        Instance.SetActive(true);
        GameManager2.Instance.scoringEnabled = false;
        if(UIManager.OnDialogOpen != null)
        {
            UIManager.OnDialogOpen.Invoke();
        }
    }

    public virtual void Hide()
    {
        Instance.SetActive(false);
        if (UIManager.OnDialogClose != null)
        {
            UIManager.OnDialogClose.Invoke();
        }
    }

    //THIS IS CALLED WHEN EVENT FROM UI MANAGER IS SENT. 
    /* So first, the popup calls UIM.Instance.DestroyPopup(referenceToItself) when it is closed?
     * Then UIM.Instance.DestroyPopup() checks if it's Action OnDialogDismiss has been invoked
     * If it hasn't, it invokes it passing the reference it just receieved. This is an event that the Popup will listen for. When popup
     * gets this event, it calls OnDialogDismiss method (below). This checks if it was the initial popup that triggered the event. 
     * If it was, then we call the on dismiss method, which can be overriden to specify any actions we want to happen upon dismissing. 
     * Remember that the UI manager destroys the popup, so we don't need to do that part */
    
    protected void OnDialogDismiss(GameObject dialog)
    {
        //Debug.Log("IAMBEFORETHEIF");
        GameManager2.Instance.scoringEnabled = true;
        if (dialog.GetInstanceID() == Instance.GetInstanceID())
        {
           //Debug.Log("IAMINTHEIF");
            OnDismiss();
            if (UIManager.OnDialogClose != null)
            {
                UIManager.OnDialogClose.Invoke();
            }
        }
    }

    protected virtual void OnDismiss()
    {
        //things to do when you want to destroy - extended in child classes
    }
}
