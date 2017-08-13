using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIManager : Singleton<TestUIManager> {

    public Canvas UICanvas;

    private void Start()
    {
        UIPopup uip = new HelpPopupUI("I am working, yes.");
        uip.Show();
    }

    public GameObject CreatePopupByResourceName(string resourceName)
    {
        GameObject prefab = Resources.Load<GameObject>("UI/" + resourceName) as GameObject;
        GameObject uip = Instantiate(prefab, UICanvas.transform.position, UICanvas.transform.rotation, UICanvas.transform) as GameObject;
        uip.SetActive(false);
        return uip;
    }
}
