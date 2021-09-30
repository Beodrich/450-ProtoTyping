using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
   private LineRenderer lineRenderer;
   private Vector3 grapplePoint;

   private SpringJoint joint;

   private Vector3 currentGrapplePos;

   [SerializeField] private LayerMask whatIsGrapple;

   //tip of the gun
   [SerializeField] private Transform gunTip, mainCamera,player;

   [SerializeField] private float maxdistanceToPoint=100f;

   [Header("Spring Variables")]

    [Range(1f,100f)]
   [SerializeField] private float springForce=15f;
   
    [Range(1f,100f)]

    [SerializeField] private float damper=7f;
    
    [Range(1f,100f)]
    [SerializeField] private float massConstant=10f;

    [SerializeField] private float minDistance=0.25f;

    [SerializeField] private float maxDistance=0.8f;

private void Awake() {
    lineRenderer=GetComponent<LineRenderer>();
}
       private void Update() {
       if(Input.GetMouseButtonDown(0)){
           StartGrapple();
       }
       else if(Input.GetMouseButtonUp(0)){
           StopGrapple();
       }
   }
   private void LateUpdate() {
       DrawLine();
   }
    void StartGrapple(){
        RaycastHit raycastHit;
        Ray test= mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(test,out raycastHit,maxdistanceToPoint ,whatIsGrapple)){
            Debug.Log("hit");
            grapplePoint= raycastHit.point;
            joint= player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor=false;
            joint.connectedAnchor=grapplePoint;
            float distanceToPoint= Vector3.Distance(player.transform.position,grapplePoint);
            
            joint.maxDistance = distanceToPoint*maxDistance;
            joint.minDistance = distanceToPoint*minDistance;

            joint.spring=springForce;
            joint.damper=damper;
            joint.massScale=massConstant;
            lineRenderer.positionCount=2;
            currentGrapplePos=gunTip.position;
            


        }
    }
    void StopGrapple(){
        lineRenderer.positionCount=0;
        Destroy(joint);
    }
    void DrawLine(){
        if(!joint){
            return;
        }
        currentGrapplePos= Vector3.Lerp(currentGrapplePos,grapplePoint,Time.deltaTime*0.8f);
        lineRenderer.SetPosition(0,gunTip.position);
        lineRenderer.SetPosition(1,currentGrapplePos);

    }

    
}
