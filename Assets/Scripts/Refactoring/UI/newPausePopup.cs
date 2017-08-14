using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class newPausePopup : UIPopup {

    public newPausePopup(string helpText) : base("newPausePopup")                   //name of prefab provided here, that suits in the Resources/UI/ folder
    {
        GameObject textGO = Instance.transform.Find("DialogText").gameObject;       //finds the text item that we can override in prefab
        textGO.GetComponent<Text>().text = helpText;
    }

    public override void Show()
    {
        base.Show();
        Time.timeScale = 0f;
    }
    protected override void OnDismiss()
    {
        //may need to reenable time here
        //things to do when you want to destroy - extended in child classes
    }
}
