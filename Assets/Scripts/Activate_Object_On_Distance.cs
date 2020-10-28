using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//----------------------------------------------------------
// This script makes objects activate at a certain distance
// from the camera
//----------------------------------------------------------

[ExecuteInEditMode()]
public class Activate_Object_On_Distance : MonoBehaviour
{
    private Transform camera;
    public float activate_Distance;
    private float dist;

    public GameObject object_To_Affect;
    public StudentMarkerState state;
    public GameObject WorldInfo;
    public GameObject EmotionInfo;
    public GameObject CognitionInfo;
    public GameObject MetaCognitionInfo;

    private void Awake() {
        camera = GameObject.Find("First Person Camera").transform; //The camera to use
    }

    // Update is called once per frame
    void Update()
    {
        //RICK: Deacvtivated to try to find bug
        //dist = Vector3.Distance(camera.position,transform.position); 
        
        //if(dist <= activate_Distance) {
        //    SetCloseDistance();
        //}
        //else SetFarDistance();
        
    }

    void SetCloseDistance()
    {
        
        WorldInfo.SetActive(state.state == StudentMarkerState.State.World);
        EmotionInfo.SetActive(state.state == StudentMarkerState.State.Emotion);
        CognitionInfo.SetActive(state.state == StudentMarkerState.State.Cognition);
        MetaCognitionInfo.SetActive(state.state == StudentMarkerState.State.MetaCognition);
        
        foreach (GameObject gameObject in new GameObject[] { EmotionInfo, CognitionInfo, MetaCognitionInfo })
        {
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(450, 450);
        }

        WorldInfo.transform.Find("WorldInfoIcon").gameObject.SetActive(true);
    }

    void SetFarDistance()
    {
        foreach (GameObject gameObject in new GameObject[] { EmotionInfo, CognitionInfo, MetaCognitionInfo })
        {
            gameObject.SetActive(true);
            EmotionInfo.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
        }
        WorldInfo.SetActive(true);
        EmotionInfo.GetComponent<RectTransform>().localPosition = new Vector3(0, -226);
        CognitionInfo.GetComponent<RectTransform>().localPosition = new Vector3(-160, 160);
        MetaCognitionInfo.GetComponent<RectTransform>().localPosition = new Vector3(160, 160);
        WorldInfo.transform.Find("WorldInfoIcon").gameObject.SetActive(false);
    }
}
