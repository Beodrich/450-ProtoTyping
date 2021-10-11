using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
//    private LineRenderer lineRenderer;
//    private Vector3 grapplePoint;

//    private SpringJoint joint;

//    private Vector3 currentGrapplePos;

//    [SerializeField] private LayerMask whatIsGrapple;

//    //tip of the gun
//    [SerializeField] private Transform gunTip, mainCamera,player;

//    [SerializeField] private float maxdistanceToPoint=100f;

//    [Header("Spring Variables")]

//     [Range(1f,100f)]
//    [SerializeField] private float springForce=15f;
   
//     [Range(1f,100f)]

//     [SerializeField] private float damper=7f;
    
//     [Range(1f,100f)]
//     [SerializeField] private float massConstant=10f;

//     [SerializeField] private float minDistance=0.25f;

//     [SerializeField] private float maxDistance=0.8f;


    //new hook variables
    private bool isHooked;
    private Rigidbody playerRb;
    private RaycastHit hit;
    //length of the rope
    public float currentRopeLength;

    public float travelSpeed;
    private bool isGrappling;

    private GameObject hook;

    public float rotationAngle=69f;
    private bool isHookToRigid;

    public GameObject pulling;

    private bool isFalseHook;

    private SpringJoint playerSpring;

    private Vector3 piviotPoint;

    private GameObject lineStart;

    public float maxGrappleRange;
    
    public GameObject fakeHook;

    public GameObject hookPrefab;

    private Vector3 cameraPlace;

    private LineRenderer lineRenderer;


private void Start() {
    isHooked=false;
    isGrappling=false;
    lineRenderer=GetComponent<LineRenderer>();
    lineRenderer.enabled=false;
    fakeHook.SetActive(true);
    cameraPlace=Camera.main.transform.localPosition;
    playerRb=GetComponent<Rigidbody>();

}
public bool getIsGrappling(){return isGrappling;}
public bool getIsHookToRigid(){return isHookToRigid;}

public bool getIsHooked(){return isHooked;}

public Vector3 getPiviotPoint(){return piviotPoint;}
       private void Update() {
  
            if(Vector3.Distance(transform.position,hit.point)<5&& isGrappling){
                playerRb.velocity= Vector3.zero;
                playerRb.AddForce(transform.up*10, ForceMode.Impulse);
                UnHook();

            }
            //could add a coolDownTimer for the hook here
            if(GameObject.Find("HookAtRunTime")!=null){
                hook=GameObject.Find("HookAtRunTime");
            }
            if(Input.GetMouseButtonUp(1)&& isHooked || Input.GetMouseButtonUp(1)&& isHookToRigid){
                //unhook the player
                UnHook();
                playerRb.velocity=Vector3.zero;
            }
            if(hook!=null && hit.point!=null){
                if(Vector3.Distance(hook.transform.position,hit.point)<3&& isFalseHook){
                isGrappling=false;
                isHooked=false;
                isFalseHook=false;
                //enable player movment here
                Destroy(GameObject.Find("HookAtRunTime"));
                }
                
            }
            //again could check for cool down here
            if(Input.GetMouseButtonDown(0)){
                CheckForGrapple();
            }
            if(isHooked || isHookToRigid){
                if(Input.GetKeyDown(KeyCode.LeftShift)){
                    //unhook this can be changed
                    playerRb.velocity= Vector3.zero;
                    UnHook();
                }
                if(Input.GetKeyDown(KeyCode.Space)){
                    UnHook();
                    //add a little bit of force for the player
                    playerRb.AddForce(transform.up*playerRb.velocity.magnitude*0.5f,ForceMode.Impulse);
                    playerRb.AddForce(transform.forward*playerRb.velocity.magnitude*0.5f,ForceMode.Impulse);

                }
            }
   }
   private void LateUpdate() {
      // DrawLine();
      if(playerRb.velocity.magnitude>35){
          playerRb.velocity=playerRb.velocity.normalized*35f;//can change value..

      }
      DrawLine();
      if(Input.GetMouseButtonDown(1)&& isHookToRigid){
          Rigidbody hookedRigidRB= pulling.GetComponent<Rigidbody>();
          hookedRigidRB.AddForce(-Camera.main.transform.forward*currentRopeLength/1.5f, ForceMode.VelocityChange);
          UnHook();
      }  
   }
     void UnHook(){
         fakeHook.SetActive(true);
         Destroy(GameObject.Find("HookAtRunTime"));
         isGrappling=false;
         isHooked=false;
         playerRb.useGravity=true;
         Destroy(gameObject.GetComponent<SpringJoint>());
         playerSpring=null;
         currentRopeLength=0;
         lineRenderer.enabled=false;
         isFalseHook=false;
         isHookToRigid=false;
         Camera.main.transform.SetParent(transform);//might have to change
         Camera.main.transform.localPosition=cameraPlace;
         Camera.main.transform.localRotation= new Quaternion(0,0,0,1);
        SetPulling();


   }
 
   private void FixedUpdate() {
       ReelInHook();
       ShootHook();
       PullPhysicsObject();
   }
   void ShootHook(){
       //if hook!=null
       if(hook){
           hook.transform.position= Vector3.Lerp(GameObject.Find("HookAtRunTime").transform.position,hit.point,0.1f);
           if(Vector3.Distance(hook.transform.position,hit.point)>2){
               hook.transform.RotateAround(hook.transform.position,hook.transform.forward, rotationAngle);
           }
       }
   }
    void ReelInHook(){
        if(isHooked){
            //if user uses right click
            if(Input.GetMouseButton(1)){
                playerRb.useGravity=false;
                //added partical system here
                transform.position=Vector3.Lerp(transform.position,hit.point,travelSpeed*Time.deltaTime/Vector3.Distance(transform.position,hit.point));
                currentRopeLength= Vector3.Distance(transform.position,hit.point);

                isHooked=true;
                isGrappling=true;
           
           
            }


        }
        
    }
    void PullPhysicsObject(){
        if(isHookToRigid){
            if(pulling!=null){
                if(pulling.GetComponent<SpringJoint>()!=null){
                    pulling.GetComponent<SpringJoint>().connectedAnchor=transform.position;
                }
                if(hook!=null && !Input.GetMouseButtonDown(0)){
                    hook.transform.position=pulling.GetComponent<Collider>().ClosestPoint(hook.transform.position);
                    hook.transform.RotateAround(hook.transform.position,hook.transform.position,-rotationAngle);

                }
            }
        }
        }

        void HookSettings(){
            if(!gameObject.GetComponent<SpringJoint>()){
                gameObject.AddComponent<SpringJoint>();
            }
            isHooked=true;
            isFalseHook=false;
            isHookToRigid=false;
            playerSpring= GetComponent<SpringJoint>();
            currentRopeLength=Vector3.Distance(transform.position,hit.point);
            piviotPoint= hit.point;
            playerSpring.autoConfigureConnectedAnchor=false;
            playerSpring.connectedAnchor= piviotPoint;
            playerSpring.maxDistance= currentRopeLength*0.8f;
            playerSpring.minDistance= currentRopeLength * 0.25f;
            playerSpring.spring= 20f;
            playerSpring.damper=0f;
            playerSpring.massScale=1f;
        }

        void DrawLine(){
            if(GameObject.Find("HookAtRunTime")!=null){
                lineRenderer.enabled=true;
                lineRenderer.SetPosition(0,lineStart.transform.position);
                lineRenderer.SetPosition(1,GameObject.Find("HookAtRunTime").transform.Find("RopeConnectionPoint").transform.position);
            }
            else{
                lineRenderer.enabled=false;
            }
        }
   void CheckForGrapple(){
       if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit,maxGrappleRange)){
           if(hit.collider.tag=="CanGrapple"&& hit.collider.GetComponent<Rigidbody>()==null)
            {
                fakeHook.SetActive(false);
                HookSettings();
                DestroyHook();
                isHookToRigid = false;
                InstantiateGrapplingHook();
                SetPulling();
            }
            else if(hit.collider.tag!="CanGrapple"){
               fakeHook.SetActive(false);
               isHooked=false;
               isHookToRigid=false;
               DestroyHook();
               if(gameObject.GetComponent<SpringJoint>()!=null){
                   Destroy(gameObject.GetComponent<SpringJoint>());
               }
               if(gameObject.GetComponent<SpringJoint>()!=null){
                   Destroy(gameObject.GetComponent<SpringJoint>());
               }
               //can play audio here
               isFalseHook=false;
               InstantiateGrapplingHook();
               SetPulling();

           }
           else if(hit.collider.GetComponent<Rigidbody>()!=null && hit.collider.tag=="CanGrapple"){
               fakeHook.SetActive(false);
               //play audio
               DestroyHook();
               if(gameObject.GetComponent<SpringJoint>()!=null){
                   Destroy(gameObject.GetComponent<SpringJoint>());

               }
               isHookToRigid=true;
               isHooked=false;
               isFalseHook=false;
               InstantiateGrapplingHook();
               if(hit.collider.gameObject.GetComponent<SpringJoint>()!=null){
                   hit.collider.gameObject.AddComponent<SpringJoint>();
               }
                SetUpHitSpringJoint();
           }
       }
    }
    void SetUpHitSpringJoint(){
        currentRopeLength=Vector3.Distance(transform.position,hit.point);
        SpringJoint hitSpringJoint= hit.collider.gameObject.GetComponent<SpringJoint>();
        hitSpringJoint.autoConfigureConnectedAnchor=false;
        hitSpringJoint.connectedAnchor=transform.position;
        hitSpringJoint.maxDistance=currentRopeLength*1.25f;
        hitSpringJoint.minDistance=currentRopeLength*0.25f;
        hitSpringJoint.spring=4.5f;
        hitSpringJoint.damper=7f;
        hitSpringJoint.massScale=4.5f;
        pulling=hit.collider.gameObject;
        pulling.GetComponent<Rigidbody>().velocity=Vector3.zero;

    }


    private static void DestroyHook()
    {
        if (GameObject.Find("HookAtRunTime") != null)
        {
            Destroy(GameObject.Find("HookAtRunTime"));

        }
    }

    private void SetPulling()
    {
        if (pulling != null)
        {
            if (pulling.GetComponent<SpringJoint>() != null)
            {
                Destroy(pulling.GetComponent<SpringJoint>());
                pulling = null;
            }
        }
    }

    private void InstantiateGrapplingHook()
    {
        GameObject grappleHook = Instantiate(hookPrefab) as GameObject;
        grappleHook.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        grappleHook.name = "HookAtRunTime";
        grappleHook.transform.position = transform.position;
    }
}
