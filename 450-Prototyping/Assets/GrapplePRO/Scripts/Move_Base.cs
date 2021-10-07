using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Base : MonoBehaviour {


	public float WalkSpeed;
	public float JumpMultiplier;
	public float GroundedRayDistance;
	public float Airtime;
	public bool CanPlayerMove = true;
	public bool Grounded;
	public Animator Anim;
	public bool ThirdPerson;

	private float MoveForward;
	private float MoveStrafe;
	private float RotX;
	private float RotY;
	private float HorizontalSensitivity = 3F;
	private float VerticalSensitivity = 1F;


	Rigidbody PlayerRigid;
	//Basic_Climb Climb;



	void Start ()
	{
		PlayerRigid = GetComponent<Rigidbody>();

		//Climb = GetComponent<Basic_Climb>();
	}


	void Update ()
	{
		if(!ThirdPerson)Rotate();


		//Animations();
		CheckIfGrounded();

	}

	void Rotate()
	{

		RotX = Input.GetAxis("Mouse X") * HorizontalSensitivity;
		RotY = Input.GetAxis("Mouse Y") * VerticalSensitivity;

		transform.Rotate(0, RotX, 0);
		Camera.main.transform.Rotate(-RotY, 0, 0);
	}
	void FixedUpdate()
	{
		if(CanPlayerMove)
		{
			Movement();

			if(Input.GetKeyDown(KeyCode.Space) && Grounded)
			{


				/*Anim.Play("Jump");*/

				PlayerRigid.AddForce(0, JumpMultiplier, 0, ForceMode.Impulse);


			}
		}
	}


	/*void Animations()
	{
		if(Input.GetKeyDown(KeyCode.Space) && Grounded)
		{


			Anim.Play("Jump");

			PlayerRigid.AddForce(0, JumpMultiplier, 0, ForceMode.Impulse);


		}
		else if(!Climb.IsClimbing && !Grounded)
		{
			Anim.Play("Fall");


		}
		if(!Grounded)
		{
			Airtime++;
			Anim.SetBool("Grounded", false);
		}
		else if(Grounded)
		{
			if(Airtime > 0)Anim.Play("Land");
			Airtime = 0;

			Anim.SetBool("Grounded", true);
			Anim.SetFloat("Speed", 0F);
			if(Input.GetKey(KeyCode.W))Anim.SetFloat("Speed", 1F);
			if(Input.GetKey(KeyCode.S))Anim.SetFloat("Speed", 2F);
			if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) )Anim.SetFloat("Speed", 1F);


		}


		else if(!Climb.IsClimbing && Grounded)
		{
			Anim.Play("Motion");


		}

		if(Climb.IsClimbing)
		{
			Anim.Play("Climbing");
			Anim.SetFloat("Speed", 0F);

			if(Input.GetKey(KeyCode.W))Anim.SetFloat("Speed", 1F);
			if(Input.GetKey(KeyCode.S))Anim.SetFloat("Speed", 2F);
			if(Input.GetKey(KeyCode.A))Anim.SetFloat("Speed", 3F);
			if(Input.GetKey(KeyCode.D))Anim.SetFloat("Speed", 4F);
		}



	}*/

	void Movement()
	{
		float hAxis = Input.GetAxis("Horizontal");
		float vAxis = Input.GetAxis("Vertical");

		Vector3 movement = new Vector3(hAxis, 0, vAxis) * WalkSpeed;

		Vector3 newPosition = PlayerRigid.position + PlayerRigid.transform.TransformDirection(movement);

		PlayerRigid.MovePosition(newPosition);
	}








	void CheckIfGrounded()
	{

		if(Grounded && Airtime > 0)
		{
			PlayerRigid.velocity = new Vector3(0, 0, 0);
			Airtime = 0;
		}
		if(Physics.Raycast(transform.position, -transform.up, GroundedRayDistance))
		{
			Grounded = true;
		}
		else
		{
			Grounded = false;
			Airtime++;
		}


	}
}
