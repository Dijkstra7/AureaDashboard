using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleStudentInfo : MonoBehaviour
{
    public void toggleInfo(GameObject Marker) {
        GameObject StudentInfo = Marker.transform.Find("Information").gameObject;
        GameObject IconSlot = Marker.transform.Find("IconSlot").gameObject;

        if (StudentInfo.active) {
            StudentInfo.SetActive(false);
            IconSlot.SetActive(true);
        } else {
            StudentInfo.SetActive(true);
            IconSlot.SetActive(false);
        }

    }
}
