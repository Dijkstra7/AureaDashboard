using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle_Marker_Script : MonoBehaviour
{

    [SerializeField]
    private GameObject marker1;

    [SerializeField]
    private GameObject marker2;

    [SerializeField]
    private GameObject marker3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Toggle_ELO_Menu(){
        marker1.SetActive(false);

        marker2.SetActive(true);
        marker3.SetActive(false);
	}

    public void Toggle_Icon_Menu(){
       
        marker1.SetActive(true);
        marker2.SetActive(false);
        marker3.SetActive(false);
	}

    public void Toggle_Progress_Exercise_Menu(){
       
        marker1.SetActive(false);
        marker2.SetActive(false);
        marker3.SetActive(true);
	}
}
