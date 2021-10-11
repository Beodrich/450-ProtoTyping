using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TP_Cam : MonoBehaviour {

	public Vector3 CameraStandard;
	public Transform CameraExpected;
	public float CameraSmoother = 2;
	GrapplingHook Grapple;
	public Image Cursor;
	private float RotX;
	private float RotY;
	private float HorizontalSensitivity = 3F;
	private float VerticalSensitivity = 1F;
	float SmoothingFactor;

	GameObject Player;

	void Start()
	{
		CameraStandard = Camera.main.transform.localPosition;
		Grapple = GameObject.FindGameObjectWithTag("Player").GetComponent<GrapplingHook>();
		Player = GameObject.FindGameObjectWithTag("Player");
	}


	void Rotate()
	{

		RotX = Input.GetAxis("Mouse X") * HorizontalSensitivity;
		RotY = Input.GetAxis("Mouse Y") * VerticalSensitivity;

		Player.transform.Rotate(0, RotX, 0);
		transform.Rotate(-RotY, 0, 0);
	}






	void Update()
	{
		if(Cursor!=null)
		{
			Cursor.rectTransform.position = Input.mousePosition;
		}
	
		if(!Grapple.getIsGrappling())
		{
			Rotate();
			//Camera.main.transform.position = CameraStandard;

		}
	}

	void FixedUpdate()
	{

		if(Grapple.getIsGrappling())
		{
			transform.SetParent(null);
			Vector3 SmoothedCameraPos = Vector3.Lerp(Camera.main.transform.position, CameraExpected.position, SmoothingFactor * Time.deltaTime);
			Camera.main.transform.position = SmoothedCameraPos;
			SmoothingFactor = CameraSmoother;
			transform.LookAt(Player.transform.position);

		}


			








	}
}
