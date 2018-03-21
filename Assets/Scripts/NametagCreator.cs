using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NametagCreator : MonoBehaviour {

    public GameObject nametagPrefab;

    private GameObject nametag;
    private Text nametext;

    // Use this for initialization
    /* Contributors: Scott Kauker */
    void Awake()
    {
        nametag = Instantiate(nametagPrefab, transform);
        nametext = nametag.GetComponentInChildren<Text>();
        nametext.text = ""; //Reset text on start. Default text is for preview only
    }

    /* Contributors: Scott Kauker */
    public void SetText(string text)
    {
        nametext.text = text;
    }

    /* Contributors: Scott Kauker */
    public void SetColor(Color c)
    {
        nametext.color = c;
    }
}
