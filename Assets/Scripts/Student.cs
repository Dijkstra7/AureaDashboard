using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;
using System;

//----------------------------------------------------------
// This class manages the student instance
//----------------------------------------------------------

public class Student : MonoBehaviour
{
    public enum VisibleCanvas
    {
        Main,
        Emotion,
        Cognition,
        MetaCognition
    };

    private string student_name;
    private VisibleCanvas visibleCanvas;
    private EmotionState emotionState;
    private MbmlcState mbmlcState;
    private CognitiveState cognitiveState;
    private string id;
    private int currentELOScore;

    private int correctFirstAnswers;
    private int correctSecondAnswers;
    private int incorrectAnswers;
    //Current student vars from the student data
    //private string Name;
    //private int ID;
    //private int answeredQuestionsNumber;
    //private string currentExerciseNumber;
    //private string currentExerciseID;
    //private int currentELOScore;
    //private string currentObjective;

    //private int taskBarMaxVal = 10;
    //private string currentMicrogoal;



    //private bool InfoBoxActive;

    //TimeSpan timeOnState;


    //private string icon;


    //Sprite current_Icon;


    private Transform camera;

    //private string previousIcon;



    /*
    All the objects controllable by this student:
    */
    [SerializeField]
    private GameObject WorldInfo;

    [SerializeField]
    private GameObject EmotionInfo;

    [SerializeField]
    private GameObject CognitionInfo;

    [SerializeField]
    private GameObject MetaCognitionInfo;

    //[SerializeField]
    //private GameObject student_Icon_Holder;

    //[SerializeField]
    //private GameObject nameBox;

    //[SerializeField]
    //private GameObject student_info_Box;




    private void Awake()
    {
        camera = GameObject.Find("First Person Camera").transform;
        //extra_Info = transform.Find("Marker").Find("Extra_Info").gameObject;
        //InfoBoxActive = false;
    }

    private void Start()
    {
        currentELOScore = 0;
        correctFirstAnswers = 0;
        correctSecondAnswers = 0;
        incorrectAnswers = 0;
    }

    private void LateUpdate()
    {
        Debug.Log("GotThisFar5");
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
    }

    public void initialiseStudent(string name, int student_ID)
    {
        this.student_name = name;
        this.id = student_ID.ToString();
        emotionState = new EmotionState();
        mbmlcState = new MbmlcState();
        cognitiveState = new CognitiveState();
        setStatusIcons();

    }

    public void setStatusIcons()
    {
        try
        {
            var canvas = transform
                .Find("StudentMarkerCanvas");
            canvas
                .Find("EmotionInfo")
                .Find("SpecificEmotion")
                .GetComponent<Image>().sprite = emotionState.getIcon();
        }
        catch (Exception exception) {
            Debug.Log(exception.ToString());
        }
    }

    public void setName(string name)
    {
        this.student_name = name.Replace("\r", "");
        //nameBox.GetComponent<TMP_Text>().text = Name;
    }

    public void setID(int student_ID)
    {
        id = student_ID.ToString();
    }

    public int getID()
    {
        return int.Parse(id);
    }

    //public void setTimeSpan(TimeSpan t)
    //{
    //    timeOnState = t;
    //}

    //public bool getInfoBoxActive()
    //{
    //    return InfoBoxActive;
    //}

    //public void setInfoBoxActive(bool active)
    //{
    //    InfoBoxActive = active;
    //}

    //public void setCurrentExerciseNumber(string exercise)
    //{
    //    currentExerciseNumber = exercise;
    //}

    //public void setCurrentExerciseID(string exercise)
    //{
    //    currentExerciseID = exercise;
    //}

    //public void set_student_answered_questions(int answeredQuestions)
    //{
    //    answeredQuestionsNumber = answeredQuestions;
    //}

    public void setELOScore(int elo)
    {
        currentELOScore = elo;
    }

    //public void setCurrentObjective(string obj)
    //{
    //    currentObjective = obj;
    //}

    //ADDED
    //public void setCurrentMicrogoal(string value, int microgoalMaxVal)
    //{
    //    currentMicrogoal = value;
    //    setTaskBarMaxVal(microgoalMaxVal);

    //}

    //public string getMicrogoal()
    //{
    //    return currentMicrogoal;

    //}

    //ADDED
    public void setTaskBarMaxVal(int value)
    {

        gameObject.transform.Find("Marker_3").transform.Find("Task_Bar")
        .GetComponent<Slider>().maxValue = value;

    }

    public void setTaskBarVal(string value)
    {

        //try {
        //    double i = (double.Parse(value));

        //    gameObject.transform.Find("Marker_3").transform.Find("Task_Bar")
        //    .GetComponent<Slider>().value = (int)(i);
        //} catch (Exception) { };

    }

    //----------------------------------------------------------
    // changing the ELO score panel
    //----------------------------------------------------------
    public void updateELOScore()
    {

    }

    public void setEloBarVal(float value)
    {

        //gameObject.transform.Find("Progress_Bars").transform.Find("EloScore_Bar").GetComponent<Slider>().value = value;
        //ProgressPanel_Extra.transform.Find("Text").GetComponent<Text>().text = "Task:\n" + currentObjective;

    }

    //----------------------------------------------------------
    // Setting student information on student information panel
    //----------------------------------------------------------

    public void set_student_info()
    {

    }
}

//-----------------------
// Emotion states handler
//-----------------------
public class EmotionState
{
    public State state;
    public enum State
    {
        VeryHappy,
        Happy,
        Neutral,
        Sad,
        VerySad
    };

    public EmotionState() : this(State.Sad)
    {
    }

    public EmotionState(State state)
    {
        this.state = state;
    }

    public Sprite getIcon()
    {
        string path = "Resource/Aurea01/AR dashboard losse iconen 2010-";
        switch (state)
        {
            case State.VeryHappy:
                path += "30";
                break;
            case State.Happy:
                path += "28";
                break;
            case State.Neutral:
                path += "31";
                break;
            case State.Sad:
                path += "32";
                break;
            case State.VerySad:
                path += "29";
                break;
        }
        path += ".png";

        return Resources.Load(path, typeof(Sprite)) as Sprite;
    }
}

//-----------------------
// MbMlc states handler
//-----------------------
public class MbmlcState
{
    public enum State
    {
        HighSwimmer,
        QuickRiser,
        TwoStageRiser,
        SlowRiser,
        RiserAndDescender,
        Unknown
    }

    public State state;

    public MbmlcState() : this(State.Unknown)
    {
    }

    public MbmlcState(State state)
    {
        this.state = state;
    }

    public Sprite getIcon()
    {
        string path = "Aurea01/AR dashboard losse iconen 2010-";
        switch (state)
        {
            case State.HighSwimmer:
                path += "20";
                break;
            case State.QuickRiser:
                path += "21";
                break;
            case State.RiserAndDescender:
                path += "24";
                break;
            case State.SlowRiser:
                path += "22";
                break;
            case State.TwoStageRiser:
                path += "23";
                break;
            default:
                path += "06";
                break;
        }
        path += ".png";

        return Resources.Load(path, typeof(Sprite)) as Sprite;
    }
}

//-----------------------
// Cognitive states handler
//-----------------------
public class CognitiveState
{
    public enum State
    {
        Decrease,
        Stable,
        Increase,
        Idle
    }

    public State state;

    public CognitiveState() : this(State.Idle)
    {
    }

    public CognitiveState(State state)
    {
        this.state = state;
    }

    public Sprite getIcon()
    {
        string path = "Aurea01/AR dashboard losse iconen 2010-";
        switch (state)
        {
            case State.Increase:
                path += "05";
                break;
            case State.Decrease:
                path += "08";
                break;
            case State.Stable:
                path += "07";
                break;
            default:
                path += "06";
                break;
        }
        path += ".png";

        return Resources.Load(path, typeof(Sprite)) as Sprite;
    }
}
