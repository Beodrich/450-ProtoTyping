using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGun : MonoBehaviour {

	Quaternion originalrot;
	Grapple_Base grapple;

	void Start ()
	{
		originalrot = transform.localRotation;
		grapple = GameObject.Find("Player").GetComponent<Grapple_Base>();
	}
	

	void Update ()
	{
		if(grapple.IsHookedToRigid && Vector3.Distance(transform.position, grapple.PivotPoint) > 2)
		{
			transform.LookAt(GameObject.Find("DaHook").transform.position);
		}
		if(grapple.IsHooked && Vector3.Distance(transform.position, grapple.PivotPoint) > 2)
		{
			transform.LookAt(grapple.PivotPoint);
		}
		else
		{
			transform.localRotation = Quaternion.Lerp(transform.localRotation, originalrot, 0.5F);
		}

	}
}
