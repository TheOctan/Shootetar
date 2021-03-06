﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	public event Action<float> OnMove;

	private Vector3 velocity;
	private Vector3 lastPosition;
	private Vector3 currentPosition;
	private Rigidbody rigidbodyComponent;

	private void Start()
	{
		rigidbodyComponent = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		rigidbodyComponent.MovePosition(rigidbodyComponent.position + velocity * Time.fixedDeltaTime);
	}

	private void Update()
	{
		currentPosition = GetApproximatelyPosition();
		if (currentPosition != lastPosition)
		{
			var distance = Vector3.Distance(currentPosition, lastPosition);
			if (distance >= 0.09)
			{
				OnMove?.Invoke(distance);
			}
		}
		lastPosition = GetApproximatelyPosition();
	}

	private Vector3 GetApproximatelyPosition()
	{
		var position = rigidbodyComponent.transform.position;
		position.y = 0;

		return position;
	}

	public void Move(Vector3 velocity)
	{
		this.velocity = velocity;
	}

	public void LookAt(Vector3 lookPoint)
	{
		Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
		transform.LookAt(heightCorrectedPoint);
	}
}
