﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
	public float moveSpeed = 5.0f;

	public Crosshairs crosshairs;

	private Camera viewCamera;
	private PlayerController controller;
	private GunController gunController;

	protected override void Start()
	{
		base.Start();

		controller = GetComponent<PlayerController>();
		gunController = GetComponent<GunController>();
		viewCamera = Camera.main;
	}

	void Update()
	{
		// Movement input
		Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		Vector3 moveVelocity = moveInput.normalized * moveSpeed;
		controller.Move(moveVelocity);

		// Look input
		Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
		Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
		float rayDistance;

		if (groundPlane.Raycast(ray, out rayDistance))
		{
			Vector3 point = ray.GetPoint(rayDistance);
			Debug.DrawLine(ray.origin, point, Color.red);
			controller.LookAt(point);
			crosshairs.transform.position = point;
			crosshairs.DetectTargets(ray);

			if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).magnitude > 1)
			{
				gunController.Aim(point);
			}
		}

		// Weapon input
		if (Input.GetMouseButton(0))
		{
			gunController.OnTriggerHold();
		}
		else if (Input.GetMouseButtonUp(0))
		{
			gunController.OnTriggerRelease();
		}
	}
}