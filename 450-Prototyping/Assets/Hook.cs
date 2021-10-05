using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public GameObject hookGameObject;
    public GameObject hookHolder;

    public float hookTravelSpeed;
    public float playerTravelSpeed;

    public  bool hooked;
    public static bool fired;

    public GameObject hookedObj;
    public float maxDistance;

    private float currentDistance;

    private bool grounded;

    private void Update() {
        //firing hook
        if(Input.GetMouseButtonDown(0)&& !fired){
            fired=true;
        }
        if(fired && !hooked){
            hookGameObject.transform.Translate(Vector3.forward*Time.deltaTime*hookTravelSpeed);
            currentDistance=Vector3.Distance(transform.position,hookGameObject.transform.position);
            if(currentDistance>=maxDistance){
                ReturnHook();
            }
        }
        if(fired){
            LineRenderer lineRenderer= hookGameObject.GetComponent<LineRenderer>();
            lineRenderer.positionCount=2;
            lineRenderer.SetPosition(0,hookHolder.transform.position);
            lineRenderer.SetPosition(1,hookGameObject.transform.position);
        }
       
        if(hooked && fired){
            hookGameObject.transform.parent=hookedObj.transform;
           transform.position=Vector3.MoveTowards(transform.position,hookGameObject.transform.position,playerTravelSpeed*Time.deltaTime);
            float distanceToHook=Vector3.Distance(transform.position,hookGameObject.transform.position);
            this.GetComponent<Rigidbody>().useGravity=false;
            if(distanceToHook<1){
                if(!isGrounded()){
                    this.transform.Translate(Vector3.forward*Time.deltaTime*10f);
                    this.transform.Translate(Vector3.up*Time.deltaTime*15f);
                    StartCoroutine("Climb");
                }
            }
        }
        else{
        this.GetComponent<Rigidbody>().useGravity=true;
        hookGameObject.transform.parent=hookHolder.transform;
 
        }
    }
    IEnumerator Climb(){
        yield return new WaitForSeconds(0.1f);
        ReturnHook();
    }
    void ReturnHook(){
        hookGameObject.transform.rotation=hookHolder.transform.rotation;
        hookGameObject.transform.position=hookHolder.transform.position;
        fired=false;
        hooked=false;
        hookGameObject.GetComponent<LineRenderer>().positionCount=0;


    }
    bool isGrounded(){
        RaycastHit hit;
        float distance=1f;
        Vector3 dir= new Vector3(0,-1);
        if(Physics.Raycast(transform.position,dir,out hit,distance)){
            grounded=true;
        }
        else{
            grounded=false;
        }
        return grounded;
    }
}
