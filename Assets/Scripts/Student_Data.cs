using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//----------------------------------------------------------
// This script is used to save student data in each
// button from the student list. (For now it's set on public
// variables on each button but should be retrieved from the
// log data)
//----------------------------------------------------------

public class Student_Data : MonoBehaviour
{
    public string name;
    public int ID;


    public string getName() {
        return name;
    }

    private void Start() {
    }

    //----------------------------------------------------------
    // Sets the student name on the button on the student list
    //----------------------------------------------------------
    public void setName(string Name) {
        name = Name;
        gameObject.transform.Find("Text").GetComponent<Text>().text = name;
    }

    public int getID() {
        return ID;
    }

    public void setID(int student_ID) {
        ID = student_ID;
    }
}
