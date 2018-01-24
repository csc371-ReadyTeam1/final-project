using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraParallax : MonoBehaviour {

    private SpriteRenderer[] layers;

	// Use this for initialization
	void Start () {

        layers = new SpriteRenderer[transform.childCount];
        int i = 0;
        foreach (Transform child in transform)
        {
            layers[i] = child.GetComponent<SpriteRenderer>();
            i++;
        }
	}
	
	// Update is called once per frame
	void Update () {
        int i = 1;
		foreach (SpriteRenderer sprite in layers)
        {
            float delta = Camera.main.transform.position.x * 0.5f * ( i * 0.4f);
            float offset = delta % (sprite.size.x * 0.1f);

            //this is totally wrong but it doesn't really matter
            sprite.gameObject.transform.position = new Vector2(Camera.main.transform.position.x - offset + sprite.size.x * 0.1f * 0.5f, transform.position.y);

            i++;
        }

    }
}
