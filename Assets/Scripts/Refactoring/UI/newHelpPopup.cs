using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class newHelpPopup : UIPopup {
    private float prevTimeScale;

    public newHelpPopup(string helpText) : base("newTutorialPopup")
    {
        GameObject textGO = Instance.transform.Find("DialogText").gameObject;       //finds the text item in prefab
        textGO.GetComponent<Text>().text = helpText;
    }

    public override void Show()
    {
        base.Show();
        prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;

    }
    protected override void OnDismiss()
    {
        Time.timeScale = prevTimeScale;
        //things to do when you want to destroy - extended in child classes
    }

}
