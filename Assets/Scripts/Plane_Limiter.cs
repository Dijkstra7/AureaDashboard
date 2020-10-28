namespace GoogleARCore
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GoogleARCoreInternal;

    public class Plane_Limiter : MonoBehaviour
    {

        private bool started;
        // Start is called before the first frame update
        void Start()
        {
            started = false;
        }

        // Update is called once per frame
        void Update()
        {
            if(started == false){
                started = true;
                StartCoroutine("Coroutine");
			}
        }

        private void Coroutine(){
            InvokeRepeating("CheckStopTrack", 0, 1.0F);  
		}


        private void CheckStopTrack()
        {
            // In DetectedPlaneVisualizer we have multiple polygons so we need to loop and diable DetectedPlaneVisualizer script attatched to that prefab.
            if(GameObject.Find("Plane Generator").transform.childCount > 12){
                  GameObject.Find("ARCore Device").GetComponent<ARCoreSession>().SessionConfig.PlaneFindingMode = DetectedPlaneFindingMode.Disabled;
		    }
        }
    }

}
