using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookDetector : MonoBehaviour
{
    public GameObject player;
     private void OnTriggerEnter(Collider other) {
        if(other.tag=="hookable"){
            player.GetComponent<Hook>().hooked=true;
            player.GetComponent<Hook>().hookedObj=other.gameObject;
        }
    }
}
