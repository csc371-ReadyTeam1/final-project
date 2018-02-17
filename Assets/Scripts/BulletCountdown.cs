﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCountdown : MonoBehaviour {
	public Vector2 to;
	public Vector2 from;

	// On third bullet, amount of time player is stunned for
	public float stunTime = 3.0f;
	public float SmoothTime = 0.1f;

	private float goalPos = 0;
	private float curPos;

	private float Damp(float a, float b, float smoothing, float dt)
	{
		return Mathf.Lerp(a, b, 1 - Mathf.Pow(smoothing, dt));
	}

	void performMovement()
	{
		goalPos += Input.GetAxis ("P2_Vertical");
		goalPos = Mathf.Clamp (goalPos, 0, 1);
	}

	void LateUpdate () {
		//Update edge points
		from = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0.9f));
		to = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0.99f));

		curPos = Damp(curPos, goalPos, SmoothTime, Time.deltaTime);

//		transform.position = Vector2.Lerp(from, to, curPos);

	}
}