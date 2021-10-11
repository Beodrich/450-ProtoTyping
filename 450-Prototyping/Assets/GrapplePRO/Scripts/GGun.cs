using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGun : MonoBehaviour {

	Quaternion originalrot;
	GrapplingHook grapple;

	void Start ()
	{
		originalrot = transform.localRotation;
		grapple = GameObject.Find("Player").GetComponent<GrapplingHook>();
	}
	

	void Update ()
	{
		if(grapple.getIsHookToRigid() && Vector3.Distance(transform.position, grapple.getPiviotPoint()) > 2)
		{
			transform.LookAt(GameObject.Find("DaHook").transform.position);
		}
		if(grapple.getIsHooked() && Vector3.Distance(transform.position, grapple.getPiviotPoint()) > 2)
		{
			transform.LookAt(grapple.getPiviotPoint());
		}
		else
		{
			transform.localRotation = Quaternion.Lerp(transform.localRotation, originalrot, 0.5F);
		}

	}
}
