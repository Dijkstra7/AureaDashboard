using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------
// This script manages the activation the student
// information box and the information passed
// onto it from each student marker.
//----------------------------------------------------------

public class Student_InfoBox_Manager : MonoBehaviour
{
    private GameObject studentInfo;
    private GameObject IconSlot;
    private GameObject current_Marker;

    private Student selectedStudent;

    private void Awake() {
        studentInfo = GameObject.Find("Information").gameObject;
    }

    private void Start() {
        studentInfo.SetActive(false);
    }

//----------------------------------------------------------
// This Method is passed the student marker object when a
// marker is clicked on. This method then uses the
// "Student" script component inside the gameobject to
// update the information accordingly.
//----------------------------------------------------------
    public void toggleInfo_on(GameObject marker) {

        if (!studentInfo.active) {
            IconSlot = marker.transform.Find("Marker").Find("IconSlot").gameObject;

            studentInfo.SetActive(true);
            current_Marker = marker;
            //ADD
            selectedStudent = current_Marker.GetComponent<Student>();
            //current_Marker.GetComponent<Student>().set_student_info();
            //current_Marker.GetComponent<Student>().setInfoBoxActive(true);
            IconSlot.SetActive(false);
            }
        
        }

    public void toggleInfo_off() {

        //current_Marker.GetComponent<Student>().setInfoBoxActive(false);
        studentInfo.SetActive(false);
        IconSlot.SetActive(true);
        
    }

    //ADD Method
    public Student getselectedStudent() {
        return selectedStudent;
    }

}
