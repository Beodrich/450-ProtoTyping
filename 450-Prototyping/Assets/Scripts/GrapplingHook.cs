using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private LayerMask whatIsGrapple;
    [SerializeField] private float maxDistance;

     private Camera mainCamera;

     [SerializeField] private GameObject player;

    private Vector3 hitPoint;

    private SpringJoint joint;

   [SerializeField] private float springForce=4.7f;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer=GetComponent<LineRenderer>();
        mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //the funny button
        if(Input.GetKeyDown(KeyCode.G)){
            StartGrapple();
        }
        else{
            StopGrapple();
        }
    }
    private void StartGrapple(){
        //cast a ray from the transform.postion to the object
        RaycastHit hit;
        if(Physics.Raycast(mainCamera.transform.position, transform.forward, out hit, maxDistance, whatIsGrapple)){
            Debug.Log("can grapple");
            hitPoint=hit.point;
            joint=player.gameObject.AddComponent<SpringJoint>();
            float distance = Vector3.Distance(this.transform.position, hitPoint);
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = this.transform.position;
            joint.maxDistance= distance*0.8f;
            joint.minDistance= distance *0.25f;
            joint.spring= springForce;
            


        }
        else{
            Debug.Log("can't grapple");
        }
    }
    private void StopGrapple(){

    }

}
