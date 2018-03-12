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
            SpriteRenderer render = child.GetComponent<SpriteRenderer>();

            //Ensure sprite widths are exactly twice their source size
            render.size = new Vector2(render.sprite.bounds.size.x * 2, render.size.y);

            layers[i] = render;
            i++;
        }
	}

    // Update is called once per frame
    void Update()
    {
        int i = 1;
        foreach (SpriteRenderer sprite in layers)
        {
            float spriteWidth = sprite.transform.localScale.x * sprite.size.x;

            //Determines the 'virtual' movement of this layer
            float offset = Camera.main.transform.position.x * 0.5f * i * 0.4f;

            //Loop back on itself at half the sprite's width, around the center
            offset = offset % (spriteWidth * 0.5f);
            offset -= spriteWidth * 0.25f;

            //Set the actual position of the sprite
            sprite.gameObject.transform.position = new Vector2(Camera.main.transform.position.x - offset, transform.position.y);

            i++;
        }
    }
}
