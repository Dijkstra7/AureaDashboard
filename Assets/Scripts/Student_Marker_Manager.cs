using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Student_Marker_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Toggle_ELO_Menu(){
        GameObject [] students = GameObject.FindGameObjectsWithTag("student");

        foreach(var student in students) {
            Toggle_Marker_Script student_component = student.GetComponent<Toggle_Marker_Script>();
            student_component.Toggle_ELO_Menu();
        }
	}

    public void Toggle_Icon_Menu(){
        GameObject [] students = GameObject.FindGameObjectsWithTag("student");

        foreach(var student in students) {
            Toggle_Marker_Script student_component = student.GetComponent<Toggle_Marker_Script>();
            student_component.Toggle_Icon_Menu();
        }
	}

    public void Toggle_Bar_Menu(){
        GameObject [] students = GameObject.FindGameObjectsWithTag("student");

        foreach(var student in students) {
            Toggle_Marker_Script student_component = student.GetComponent<Toggle_Marker_Script>();
            student_component.Toggle_Progress_Exercise_Menu();
        }
	}
}
