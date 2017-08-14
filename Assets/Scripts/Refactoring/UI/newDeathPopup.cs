using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class newDeathPopup : UIPopup {
    public newDeathPopup(string helpText) : base("newDeathPopup")
    {
        GameObject textGO = Instance.transform.Find("DialogText").gameObject;       //finds the text item in prefab
        textGO.GetComponent<Text>().text = helpText;
    }

    public override void Show()
    {
        base.Show();
        Time.timeScale = 0f;
    }
    protected override void OnDismiss()
    {
        
        //things to do when you want to destroy - extended in child classes
    }
}
