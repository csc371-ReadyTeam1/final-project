using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NametagCreator : MonoBehaviour {

    public GameObject nametagPrefab;

    private GameObject nametag;
    private Text nametext;

	// Use this for initialization
	void Awake()
    {
        nametag = Instantiate(nametagPrefab, transform);
        nametext = nametag.GetComponentInChildren<Text>();
    }
	
    public void SetText(string text)
    {
        nametext.text = text;
    }

    public void SetColor(Color c)
    {
        nametext.color = c;
    }
}
