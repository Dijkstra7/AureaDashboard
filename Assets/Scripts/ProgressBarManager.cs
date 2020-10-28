using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarManager : MonoBehaviour
{

    bool toggle;

    // Start is called before the first frame update
    void Start()
    {
        //showProgressBars(false);
        toggle = false;
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.K)) {
            toggleProgressBars();
        }
    }


    public void toggleProgressBars() {
        if (toggle == false){
            toggle = true;
        } else toggle = false;

        showProgressBars(toggle);
    }

    private void showProgressBars(bool show) {
        GameObject [] students = GameObject.FindGameObjectsWithTag("student");

        foreach(GameObject student in students) {
            student.transform.Find("Progress_Bars").Find("EloScore_Bar").gameObject.SetActive(show);
            student.transform.Find("Marker").Find("IconSlot").gameObject.SetActive(!show);
        }
    }
}
