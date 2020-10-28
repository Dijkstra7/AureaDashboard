using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class WorldInfoManager : MonoBehaviour
{
    public bool toggle;
    // Start is called before the first frame update
    void Start()
    {
        toggle = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if worldInfo is clicked

        GetCurrentView();
    }

    public void toggle_view()
    {
        toggle = !toggle;        
    }

    public void GetCurrentView()
    {
        var student_markers = GameObject.FindGameObjectsWithTag("student");
        foreach (var student in student_markers)
        {
            var canvas = student.transform
                                .Find("StudentMarkerCanvas");

            canvas
                .Find("EmotionInfo")
                .gameObject.SetActive(!toggle);
            canvas
                .Find("CognitionInfo")
                .gameObject.SetActive(!toggle);
            canvas
                .Find("MetaCognitionInfo")
                .gameObject.SetActive(!toggle);
            canvas
                .Find("WorldInfo")
                .Find("WorldInfoIcon")
                .gameObject.SetActive(toggle);
        }
    }
}
