using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grapple_Base : MonoBehaviour {

	public float MaxGrappleRange;
	public float CurrentRopeLength;
	public float TravelSpeed;
	public bool IsGrappling;
	public bool IsHooked;
	public bool FalseHook;
	public bool IsHookedToRigid;
	public Vector3 PivotPoint;
	public GameObject HookPrefab;
	RaycastHit Hit;
	SpringJoint PlayerSpring;
	Move_Base PlayerMove;
	Rigidbody PlayerRigid;
	LineRenderer LR;
	GameObject TheHook;
	public float GrapplingCooldown;
	public float CooldownTimer;
	public GameObject Pulling;
	public GameObject LineStart;
	public AudioSource Audio;
	public AudioClip ReelSound;
	public AudioClip LaunchSound;
	public AudioClip HitSuccess;
	public AudioClip HitFailure;
	public AudioSource WindSoundFX;
	Vector3 CameraStandard;
	bool Test69;
	public GameObject FakeHook;
	public bool ThirdPerson;
	public Text VelocityReadout;
	public GameObject SpeedFX;


	void Start ()
	{
		IsHooked = false;
		IsGrappling = false;
		PlayerMove = GetComponent<Move_Base>();
		PlayerRigid = GetComponent<Rigidbody>();
		Audio = GetComponent<AudioSource>();
		LR = GetComponent<LineRenderer>();
		LR.enabled = false;
		FakeHook.SetActive(true);
		CameraStandard = Camera.main.transform.localPosition;
		SpeedFX.GetComponent<ParticleSystem>().Stop();
		VelocityReadout = GameObject.Find("Vel_Read").GetComponent<Text>();
	}
	
	void FixedUpdate()
	{
		ReelIn();
		ShootHook();
		PullPhysicsObjects();


	}


	void LateUpdate()
	{

		if(PlayerRigid.velocity.magnitude > 35){
		PlayerRigid.velocity = PlayerRigid.velocity.normalized * 35F;
		}
		VelocityReadout.text = Mathf.Round(PlayerRigid.velocity.magnitude).ToString();
		DrawRope();

		if(Input.GetMouseButtonDown(1) && IsHookedToRigid)
		{

			Rigidbody HookedRigid = Pulling.GetComponent<Rigidbody>();
			HookedRigid.AddForce(-Camera.main.transform.forward * CurrentRopeLength/1.5F, ForceMode.VelocityChange);
			UnHook();

		}
	}




	void SoundFX()
	{
		if(TheHook != null)
		{
			
			if(FalseHook && Vector3.Distance(TheHook.transform.position, Hit.point) < 10)
			{
				Audio.clip = HitFailure;
				if(!Audio.isPlaying)Audio.Play();
			}

		}



		if(IsHooked && PlayerRigid.velocity.magnitude < 20 && WindSoundFX.isPlaying)
		{
			
			WindSoundFX.Stop();
			WindSoundFX.enabled = false;
			SpeedFX.GetComponent<ParticleSystem>().Stop();

		}
		if(IsHooked && PlayerRigid.velocity.magnitude > 20 && !WindSoundFX.isPlaying)
		{
			WindSoundFX.enabled = true;
			SpeedFX.GetComponent<ParticleSystem>().Play();
			WindSoundFX.Play();

		}

		if(IsGrappling)
		{
			if(!Audio.isPlaying)
			{
				Audio.clip = ReelSound;
				Audio.Play();
			}

		}
		else if(!IsGrappling && !IsHooked && !IsHookedToRigid && !FalseHook)
		{
			Audio.Stop();
		}
	}

	void Update ()
	{
		SoundFX();



		if(Vector3.Distance(transform.position, Hit.point) < 5 && IsGrappling)
		{
			PlayerRigid.velocity = new Vector3(0, 0, 0);
			PlayerRigid.AddForce(transform.up * 10, ForceMode.Impulse);
			UnHook();
		}

		if(CooldownTimer < GrapplingCooldown)CooldownTimer++;

		if(GameObject.Find("DaHook") != null)TheHook = GameObject.Find("DaHook");

		if(Input.GetMouseButtonUp(1) && IsHooked || Input.GetMouseButtonUp(1) && IsHookedToRigid)
		{
			UnHook();
			PlayerRigid.velocity = new Vector3(0,0,0);
		}

		if(TheHook != null && Hit.point != null)
		{


			if(Vector3.Distance(TheHook.transform.position, Hit.point) < 2 && IsHooked == true && Test69 == false || 
				Vector3.Distance(TheHook.transform.position, Hit.point) < 2 && IsHookedToRigid == true && Test69 == false)
			{
				Audio.clip = HitSuccess;
				Audio.Play();
				Test69 = true;
			}

			if(Vector3.Distance(TheHook.transform.position, Hit.point) < 3 && FalseHook == true)
			{


				IsGrappling = false;
				IsHooked = false;
				FalseHook = false;
				LR.enabled = false;
				PlayerMove.CanPlayerMove = true;
				Destroy(GameObject.Find("DaHook"));


			}

		}

		if(Input.GetMouseButtonDown(0) && GrapplingCooldown == CooldownTimer){
			CheckForGrapple();
			}

		if(IsHooked || IsHookedToRigid)
		{
			if(Input.GetKeyDown(KeyCode.LeftShift))
			{
				PlayerRigid.velocity = new Vector3(0,0,0);
				UnHook();
			}

			if(Input.GetKeyDown(KeyCode.Space))
			{
				UnHook();
				PlayerRigid.AddForce(transform.up * PlayerRigid.velocity.magnitude * 0.5F, ForceMode.Impulse);
				PlayerRigid.AddForce(transform.forward * PlayerRigid.velocity.magnitude * 0.5F, ForceMode.Impulse);
			}
		}
	}


	void ReelIn()
	{
		if(IsHooked)
		{
			


			if(Input.GetMouseButton(1))
			{
				PlayerRigid.useGravity = false;
				SpeedFX.GetComponent<ParticleSystem>().Play();
				transform.position = Vector3.Lerp(transform.position, Hit.point, TravelSpeed * Time.deltaTime/Vector3.Distance(transform.position, Hit.point));
				//PlayerRigid.AddForce(transform.forward * TravelSpeed, ForceMode.Force);
				CurrentRopeLength = Vector3.Distance(transform.position, Hit.point);


				IsHooked = true;
				IsGrappling = true;

			}

		}
	}

	void HookedSettings()
	{
		

		if(gameObject.GetComponent<SpringJoint>() == null){
			gameObject.AddComponent<SpringJoint>();
			
			}

		



			IsHooked = true;
			FalseHook = false;
			IsHookedToRigid = false;
			PlayerSpring = GetComponent<SpringJoint>();
			CurrentRopeLength = Vector3.Distance(transform.position, Hit.point);
			PivotPoint = Hit.point;
			PlayerSpring.autoConfigureConnectedAnchor = false;
			PlayerSpring.connectedAnchor = PivotPoint;
			PlayerSpring.maxDistance = CurrentRopeLength * 0.8F;
			PlayerSpring.minDistance = CurrentRopeLength * 0.25F;
			PlayerSpring.spring = 2f;
			PlayerSpring.damper = 7F;
			PlayerSpring.massScale = 4.5F;

			

		}
		

	void ShootHook()
	{
		if(TheHook != null)
		{
			TheHook.transform.position = Vector3.Lerp(GameObject.Find("DaHook").transform.position, Hit.point, 0.1F);
			if(Vector3.Distance(TheHook.transform.position, Hit.point) > 2){
			TheHook.transform.RotateAround(TheHook.transform.position, TheHook.transform.forward, 69);
			}
		}

	}


	void UnHook()
	{
		
		//PlayerRigid.velocity = new Vector3(0, 1,0);
		FakeHook.SetActive(true);
		Test69 = false;
		Destroy(GameObject.Find("DaHook"));
		IsGrappling = false;
		IsHooked = false;
		PlayerRigid.useGravity = true;
		Destroy(gameObject.GetComponent<SpringJoint>());
		PlayerSpring = null;
		CurrentRopeLength = 0;
		LR.enabled = false;
		FalseHook = false;
		IsHookedToRigid = false;
		Camera.main.transform.SetParent(transform);
		Camera.main.transform.localPosition = CameraStandard;
		if(ThirdPerson){
		Camera.main.transform.localRotation = new Quaternion(0,0,0,1);
		}
		WindSoundFX.enabled = false;
		SpeedFX.GetComponent<ParticleSystem>().Stop();
		if(Pulling !=null)
		{
			if(Pulling.GetComponent<SpringJoint>() != null){
			Destroy(Pulling.GetComponent<SpringJoint>());
			}
			Pulling = null;
		}

	}


	void DrawRope()
	{
		if(GameObject.Find("DaHook") != null)
		{
			LR.enabled = true;
			LR.SetPosition(0, LineStart.transform.position);
			LR.SetPosition(1, GameObject.Find("DaHook").transform.Find("RopeConnectionPoint").transform.position);

		}
		else
		{
			LR.enabled = false;
		}


	}


	void PullPhysicsObjects()
	{
		if(IsHookedToRigid)
		{
			if(Pulling != null)
			{
				if(Pulling.GetComponent<SpringJoint>() != null)Pulling.GetComponent<SpringJoint>().connectedAnchor = transform.position;

				if(TheHook != null && !Input.GetMouseButton(0))
				{
					TheHook.transform.position = Pulling.GetComponent<Collider>().ClosestPoint(TheHook.transform.position);
					TheHook.transform.RotateAround(TheHook.transform.position, TheHook.transform.forward, -69);
				}



			}

		}
	}


	void CheckForGrapple()
	{
		// if(!ThirdPerson)
		// {
		// 	if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out Hit, MaxGrappleRange) && GrapplingCooldown == CooldownTimer)
		// 	{
		// 		if(Hit.collider.tag == "CanGrapple" && Hit.collider.GetComponent<Rigidbody>() == null)
		// 		{
		// 			FakeHook.SetActive(false);
		// 			Test69 = false;
		// 			Audio.clip = LaunchSound;
		// 			Audio.Play();
		// 			HookedSettings();
		// 			if(GameObject.Find("DaHook") != null)
		// 			{
		// 				Destroy(GameObject.Find("DaHook"));
		// 			}
		// 			IsHookedToRigid = false;
		// 			GameObject GrappleHook = Instantiate(HookPrefab) as GameObject;
		// 			GrappleHook.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
		// 			GrappleHook.name = "DaHook";
		// 			GrappleHook.transform.position = transform.position;
		// 			CooldownTimer = 0;
		// 			if(Pulling !=null)
		// 			{
		// 				if(Pulling.GetComponent<SpringJoint>() != null)Destroy(Pulling.GetComponent<SpringJoint>());
		// 				Pulling = null;
		// 			}

		// 		}
		// 		else if(Hit.collider.tag != "CanGrapple")
		// 		{
		// 			FakeHook.SetActive(false);
		// 			Test69 = false;
		// 			IsHooked = false;
		// 			IsHookedToRigid = false;
		// 			if(GameObject.Find("DaHook") != null)
		// 			{
		// 				Destroy(GameObject.Find("DaHook"));
		// 			}
		// 			if(gameObject.GetComponent<SpringJoint>() != null)
		// 			{
		// 				Destroy(gameObject.GetComponent<SpringJoint>());
		// 			}
		// 			Audio.clip = LaunchSound;
		// 			Audio.Play();
		// 			FalseHook = true;

		// 			GameObject GrappleHook = Instantiate(HookPrefab) as GameObject;
		// 			GrappleHook.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
		// 			GrappleHook.name = "DaHook";
		// 			GrappleHook.transform.position = transform.position;
		// 			CooldownTimer = 0;
		// 			if(Pulling !=null)
		// 			{
		// 				if(Pulling.GetComponent<SpringJoint>() != null)Destroy(Pulling.GetComponent<SpringJoint>());
		// 				Pulling = null;
		// 			}
		// 		}
		// 		else if(Hit.collider.GetComponent<Rigidbody>() != null && Hit.collider.tag == "CanGrapple")
		// 		{
		// 			FakeHook.SetActive(false);
		// 			Test69 = false;
		// 			Audio.clip = LaunchSound;
		// 			Audio.Play();
		// 			if(GameObject.Find("DaHook") != null)
		// 			{
		// 				Destroy(GameObject.Find("DaHook"));
		// 			}
		// 			if(gameObject.GetComponent<SpringJoint>() != null)
		// 			{
		// 				Destroy(gameObject.GetComponent<SpringJoint>());
		// 			}
		// 			IsHookedToRigid = true;
		// 			IsHooked = false;
		// 			FalseHook = false;
		// 			GameObject GrappleHook = Instantiate(HookPrefab) as GameObject;
		// 			GrappleHook.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
		// 			GrappleHook.name = "DaHook";
		// 			GrappleHook.transform.position = transform.position;
		// 			CooldownTimer = 0;
		// 			if(Hit.collider.gameObject.GetComponent<SpringJoint>() == null)Hit.collider.gameObject.AddComponent<SpringJoint>();



		// 			CurrentRopeLength = Vector3.Distance(transform.position, Hit.point);
		// 			SpringJoint ObjjSpring  = Hit.collider.gameObject.GetComponent<SpringJoint>();
		// 			ObjjSpring.autoConfigureConnectedAnchor = false;
		// 			ObjjSpring.connectedAnchor = transform.position;
		// 			ObjjSpring.maxDistance = CurrentRopeLength * 1.25F;
		// 			ObjjSpring.minDistance = CurrentRopeLength * 0.25F;
		// 			ObjjSpring.spring = 4.5F;
		// 			ObjjSpring.damper = 7F;
		// 			ObjjSpring.massScale = 4.5F;

		// 			Pulling = Hit.collider.gameObject;
		// 			Pulling.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);

		// 		}

		// 	}
		// }
		 if(ThirdPerson)
		{
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out Hit, MaxGrappleRange) && GrapplingCooldown == CooldownTimer)
			{
				if(Hit.collider.tag == "CanGrapple" && Hit.collider.GetComponent<Rigidbody>() == null)
				{
					FakeHook.SetActive(false);
					Test69 = false;
					Audio.clip = LaunchSound;
					Audio.Play();
					HookedSettings();
					if(GameObject.Find("DaHook") != null)
					{
						Destroy(GameObject.Find("DaHook"));
					}
					IsHookedToRigid = false;
					GameObject GrappleHook = Instantiate(HookPrefab) as GameObject;
					GrappleHook.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
					GrappleHook.name = "DaHook";
					GrappleHook.transform.position = transform.position;
					CooldownTimer = 0;
					if(Pulling !=null)
					{
						if(Pulling.GetComponent<SpringJoint>() != null)Destroy(Pulling.GetComponent<SpringJoint>());
						Pulling = null;
					}

				}
				else if(Hit.collider.tag != "CanGrapple")
				{
					FakeHook.SetActive(false);
					Test69 = false;
					IsHooked = false;
					IsHookedToRigid = false;
					if(GameObject.Find("DaHook") != null)
					{
						Destroy(GameObject.Find("DaHook"));
					}
					if(gameObject.GetComponent<SpringJoint>() != null)
					{
						Destroy(gameObject.GetComponent<SpringJoint>());
					}
					Audio.clip = LaunchSound;
					Audio.Play();
					FalseHook = true;

					GameObject GrappleHook = Instantiate(HookPrefab) as GameObject;
					GrappleHook.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
					GrappleHook.name = "DaHook";
					GrappleHook.transform.position = transform.position;
					CooldownTimer = 0;
					if(Pulling !=null)
					{
						if(Pulling.GetComponent<SpringJoint>() != null)Destroy(Pulling.GetComponent<SpringJoint>());
						Pulling = null;
					}
				}
				else if(Hit.collider.GetComponent<Rigidbody>() != null && Hit.collider.tag == "CanGrapple")
				{
					FakeHook.SetActive(false);
					Test69 = false;
					Audio.clip = LaunchSound;
					Audio.Play();
					if(GameObject.Find("DaHook") != null)
					{
						Destroy(GameObject.Find("DaHook"));
					}
					if(gameObject.GetComponent<SpringJoint>() != null)
					{
						Destroy(gameObject.GetComponent<SpringJoint>());
					}
					IsHookedToRigid = true;
					IsHooked = false;
					FalseHook = false;
					GameObject GrappleHook = Instantiate(HookPrefab) as GameObject;
					GrappleHook.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
					GrappleHook.name = "DaHook";
					GrappleHook.transform.position = transform.position;
					CooldownTimer = 0;
					if(Hit.collider.gameObject.GetComponent<SpringJoint>() == null){
					Hit.collider.gameObject.AddComponent<SpringJoint>();
					}



					CurrentRopeLength = Vector3.Distance(transform.position, Hit.point);
					SpringJoint ObjjSpring  = Hit.collider.gameObject.GetComponent<SpringJoint>();
					ObjjSpring.autoConfigureConnectedAnchor = false;
					ObjjSpring.connectedAnchor = transform.position;
					ObjjSpring.maxDistance = CurrentRopeLength * 1.25F;
					ObjjSpring.minDistance = CurrentRopeLength * 0.25F;
					ObjjSpring.spring = 4.5F;
					ObjjSpring.damper = 7F;
					ObjjSpring.massScale = 4.5F;

					Pulling = Hit.collider.gameObject;
					Pulling.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);

				}

			}
		}


	}
}
