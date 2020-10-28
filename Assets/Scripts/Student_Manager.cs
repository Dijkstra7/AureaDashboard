using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using UnityEngine.EventSystems;

//----------------------------------------------------------
// This class instantiates students when a button is
// pressed on the list
//----------------------------------------------------------

public class Student_Manager : MonoBehaviour
{
    [SerializeField]
    private GameObject studentPrefab;

    private Transform camera;

    //Isn't used right now but left it just in case for the ar.
    //Distance from camera at which an object is generated
    private float distance; 

    List<Student> students = new List<Student>();
    private GameObject lastbuttonclicked;

    private GameObject instance;
    bool follow;

    private GameObject adduserbutton;
    //private GameObject menubutton;
    private GameObject studentlistmenu;

    private GameObject placestudentsign;

    //----------------------------------------------------------
    // Current student states. They all have to be named after
    // a prefab icon in Resources/States
    //
    // This won't be needed later on, it's already in every 'student' object
    //----------------------------------------------------------
    private enum State {
        Idle,
        Confused,
        Question,
        OK,
        Exceptional,
        Play_Green,
        Play_Orange,
        Play_Yellow,
        Idle_White,
        Idle_Grey,
    }

    private void Awake() {
        camera = GameObject.Find("First Person Camera").transform;
    }

    void Start()
    {
        distance = 100f;
       follow = false;
       adduserbutton = GameObject.Find("AddUsersButton");
       //menubutton = GameObject.Find("SideMenuButton"); //RICK: Remove extra menubuttons
    }

    private void Update() {
  
        //This method makes the object follow the mouse until 'r' is pressed for placement.
        //This will ofc have to be changed for the ar

        /* 
        if(follow) {

            Vector3 temp = Input.mousePosition;
            temp.z = 100f; // Set this to be the distance you want the object to be placed in front of the camera.
            instance.transform.position = camera.gameObject.GetComponent<Camera>().ScreenToWorldPoint(temp);

            if (Input.GetKeyDown(KeyCode.R)) 
                follow = false;

        }
        */
        
    }

     //ADD THIS METHOD
    List<GameObject> listButtons = new List<GameObject>();

    public void deleteStudent() {
        Student selectedStudent = GetComponent<Student_InfoBox_Manager>().getselectedStudent();

        foreach(var button in listButtons) {
            if(button.GetComponent<Student_Data>().getID() == selectedStudent.getID()) {
                enable_Button(button, true);
            }
        }
        Destroy(selectedStudent.gameObject);
    }

    public void activateStudentPlaceUI(bool hideactive){
        adduserbutton.SetActive(hideactive == false);
        //menubutton.SetActive(hideactive == false);

        if(hideactive){
            studentlistmenu = GameObject.Find("Student_List_Panel");
		}
        if(hideactive == false){
            GameObject.Find("PlaceStudentSign").SetActive(false);
		}
        studentlistmenu.SetActive(hideactive == false);
	}

    public void createStudent(Student_Data data) {
        activateStudentPlaceUI(true);

        instance = Instantiate(studentPrefab,new Vector3(1000,1000,199), transform.rotation);
        //instance.SetActive(true);

        //instance.transform.Find("Marker").Find("IconSlot").gameObject.SetActive(true);
        instance.transform.Find("StudentMarkerCanvas").gameObject.SetActive(true);
        instance.GetComponent<Student>().initialiseStudent(data.name, data.ID);
        students.Add(instance.GetComponent<Student>());
        lastbuttonclicked = data.gameObject;
        follow = true; //set object to follow mouse for placement
        Debug.Log("Got This Far 1");
        //enable_Button(data.gameObject, false);
       
    }

   public void SetLocationInstance(Vector3 pos, Anchor anchor){
        activateStudentPlaceUI(false);
        Debug.Log("GotThisFar2");
        if(follow){
            Debug.Log(pos.ToString());
            instance.transform.position = pos;
            instance.transform.parent = anchor.transform;
            instance.transform.localScale = new Vector3(0.0005f,0.0005f,0.0005f);
            instance.SetActive(true);
            //instance.transform.Find("Marker").Find("Extra_Info").gameObject.SetActive(true);
            
            GameObject.Find("Student_List_Panel").GetComponent<Image>().raycastTarget = true;
            enable_Button(lastbuttonclicked, false);
		}
        Debug.Log("GotThisFar4");
        follow = false;

        
        GameObject.Find("Student_Manager").GetComponent<Data_Manager>().ForceUpdate( instance.GetComponent<Student>().getID().ToString() );
	}


    private void enable_Button(GameObject button, bool enable) {
        button.GetComponent<Button>().interactable = enable;
    }

    //----------------------------------------------------------
    // Method to change a student's state based on it's id.
    // Nothing happens if a student id is not found
    //----------------------------------------------------------

    private void setStudentState(int studentID, State s) {

        //foreach(Student student in students) {
        //    if(student.getID() == studentID) {
        //        student.setIcon(s.ToString());
        //        break;
        //    }
        //}
        return;
    }

}

