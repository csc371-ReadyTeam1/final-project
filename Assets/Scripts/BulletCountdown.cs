using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCountdown : MonoBehaviour {
	public Vector2 from;

	// On third bullet, amount of time player is stunned for
	public float stunTime = 3.0f;
	public float SmoothTime = 0.1f;

	private float goalPos = 0;
	private float curPos;

	/* Contributors: Megan Washburn */
	private float Damp(float a, float b, float smoothing, float dt)
	{
		return Mathf.Lerp(a, b, 1 - Mathf.Pow(smoothing, dt));
	}

	/* Contributors: Megan Washburn */
	void performMovement()
	{
		goalPos += Input.GetAxis ("P2_Vertical");
		goalPos = Mathf.Clamp (goalPos, 0, 1);
	}

	/* Contributors: Megan Washburn */
	void LateUpdate () {
		//Update edge points
		from = Camera.main.ViewportToWorldPoint(new Vector3(0.95f, 0.95f));

		curPos = Damp(curPos, goalPos, SmoothTime, Time.deltaTime);

		float newX = Vector2.Lerp(from, from, curPos).x;
		float ogY = transform.position.y;
		Vector2 newPos = new Vector2 (newX, ogY);

		transform.position = newPos;
	}
}
