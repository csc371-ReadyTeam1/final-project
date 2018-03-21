using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraParallax : MonoBehaviour {

    /// <summary>
    /// Constant movement to be applied to the background, even without camera movement
    /// </summary>
    public float constantSpeed = 0.0f;

    /// <summary>
    /// How much camera movement affects the parallax
    /// </summary>
    public float cameraMoveScale = 0.4f;

    private SpriteRenderer[] layers;

    // Use this for initialization
    /* Contributors: Scott Kauker */
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
    /* Contributors: Scott Kauker */
    void Update()
    {
        int i = 1;
        foreach (SpriteRenderer sprite in layers)
        {
            float spriteWidth = sprite.transform.localScale.x * sprite.size.x;

            //Determines the 'virtual' movement of this layer
            float camX = Camera.main.transform.position.x + constantSpeed * Time.time;
            float offset = camX * 0.5f * i * cameraMoveScale;

            //Loop back on itself at half the sprite's width, around the center
            offset = offset % (spriteWidth * 0.5f);
            offset -= spriteWidth * 0.25f;

            //Set the actual position of the sprite
            sprite.gameObject.transform.position = new Vector2(Camera.main.transform.position.x - offset, transform.position.y);

            i++;
        }
    }
}
