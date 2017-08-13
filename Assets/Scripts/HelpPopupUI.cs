using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpPopupUI : UIPopup {
    public HelpPopupUI(string helpText) : base("HelpPopupUI")
    {
        GameObject textGO = Instance.transform.Find("DialogText").gameObject;
        textGO.GetComponent<Text>().text = helpText;
    }
}
