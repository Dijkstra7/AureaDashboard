using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Globalization;

public class StudentCSVSim
{

    // extra variables generated
    private DateTime lastAnsweredQuestionTime;
    private int emotionsState = 0;
    private int correctStreak = 0;
    private int answeredQuestions = 0;

    // contains all current elo scores for objectives
    public Dictionary<string,int> objectiveELO;
    public Dictionary<string,string> objective_random_names;

    //Hardcoded right now, should be greater than average time between questions to show that the student is not doing anything
    private double lazyTime = 10;

    //student is away.
    private double awayTime = 20;

    private DateTime timeOnPreviousStateChange;
    
    public enum State {
        Play_Green,
        Play_Orange,
        Play_Red,
        Star,
        Idle_Green,
        Idle_Orange,
        Idle_Red,
        Idle_Yellow,
    }

    private State currentState;

    private string id;
    private List<string[]> myDataList;

    //  class manager script to send the information to (class overview)
    private Class_Manager classmanager;

    //-----------------------------------------------------------------------------------------------
	// Constructor
	//-----------------------------------------------------------------------------------------------
    public StudentCSVSim(){
        id = "0";
        myDataList = new List<string[]>();
        classmanager = GameObject.Find("Student_Manager").GetComponent<Class_Manager>();
        objectiveELO = new Dictionary<string, int>();
        

        // hardcoding the objective_value and objective_name
        objective_random_names = new Dictionary<string,string>();
        objective_random_names.Add("8232", "Wiskunde Basis");
        objective_random_names.Add("8234", "Wiskunde Verdiepend");
        objective_random_names.Add("8240", "Wiskunde Moeilijk");
        objective_random_names.Add("8181", "Wiskunde Toets");

    }

    public Dictionary<string,string> GetDictionarySubjects(){
        return objective_random_names;
	}

    //-----------------------------------------------------------------------------------------------
	// New line of data can be added
    // line is stored in the myDataList list
    // extra internal variables are created
    // modify external student gameobject with internal variables
	//-----------------------------------------------------------------------------------------------
    public void SetNewData(string[] str, DateTime timestamp, List<string> MicrogoalID, List<int> MaxExerciseNumber){
        id = str[37];
        // add to data list
        myDataList.Add(str);

        // update internal state
        setInternalState(timestamp);
        //Debug.Log(timestamp);

        // update student (if found)
        updateStudentPlay(MicrogoalID,MaxExerciseNumber);
	}

    //-----------------------------------------------------------------------------------------------
	// Getters and setters:
	//-----------------------------------------------------------------------------------------------
    public void setNewStateTime(DateTime newStateTime) {
        timeOnPreviousStateChange = newStateTime;     
    }

    public DateTime getStateTime() {
        return timeOnPreviousStateChange;
    }

    public string GetId(){
        return id;
	}

    public List<string[]> GetDataList(){
        return myDataList;
	}

    public string getCurrentExerciseID() {
        return myDataList[myDataList.Count-1][1];
    }

    public string getCurrentCorrectlyAnswered() {
        return myDataList[myDataList.Count-1][12];
    }

    public int getQuestionsAnsweredNumber() {
        return answeredQuestions;
    }

    public DateTime getlastAttemptTime() {
        return lastAnsweredQuestionTime;
    }

    public double getLazyTime() {
        return lazyTime;
    }

    public double getAwayTime() {
        return awayTime;
    }

    public string getCurrentObjective(){
        return myDataList[myDataList.Count-1][2];
	}

    public string getDurationLastQuestion(){
        return myDataList[myDataList.Count-1][43];
	}

    public int getELOScore(){
        
        string str = myDataList[myDataList.Count-1][7];
        string substr = str.Replace("\"", "");
        double i = (double.Parse(substr, new CultureInfo("nl"))+2.0)*25.0;
        return(int)(i);

	}

    //This is hardcoded right now. (5 = 5 min delay before
    //switching to idle state). Ideally it should take an average of
    // question time. TODO:
    public double getAverageQuestionDelay() {
        return 5;
    }

    public string getCurrentExerciseNumber () {
        return myDataList[myDataList.Count-1][42];
    }

    public string getCurrentState() {

        return currentState.ToString();
    }

    //ADDED
    public string getNextMicrogoal() {
        return myDataList[myDataList.Count-1][52];
    }

    //This method returns a student's current 'play' state based
    //on the number of incorrect attempts or streak of correct questions
    //It should be changed for the final thing but at least for the demo it should
    //be fine?
    public State getPlayState() {
        return currentState;

    }

    //-----------------------------------------------------------------------------------------------
	// Set internal member variables
	//-----------------------------------------------------------------------------------------------
    public void setInternalState(DateTime dt){

        // changing the elo scores for the current objective
        if(objectiveELO.ContainsKey(getCurrentObjective())){
            objectiveELO[getCurrentObjective()] = getELOScore();
        }
        else{
            objectiveELO.Add(getCurrentObjective(), getELOScore());
		}
        
        if(getCurrentCorrectlyAnswered() == "FALSE") {
            classmanager.addWronglyAnsweredExercise(getCurrentExerciseID());
        }
        setNewStateTime(dt);
        lastAnsweredQuestionTime = dt;

        if(getCurrentCorrectlyAnswered() == "FALSE")
        {
            if (emotionsState > 0)
            {
                emotionsState--;
            }
        }
        else 
        {
            if (emotionsState < 4)
            {
                emotionsState++;
            }
            answeredQuestions +=1;
            correctStreak += 1;
        }

        // RICK TODO: set emotion state
        currentState = State.Play_Green;

        if(correctStreak > 3) {
            currentState = State.Star;
        }
	}

    //-----------------------------------------------------------------------------------------------
    // Handles whenever a new data point is detected for a student (it means there was an attampt to
    // answer a question)
    //-----------------------------------------------------------------------------------------------
    // ONLY UPDATES EXTERNAL STUDENT WITH CURRENT INTERNAL VARIABLES
    public void updateStudentPlay(List<string> MicrogoalID, List<int> MaxExerciseNumber) {
        // update overview
        classmanager.addExerciseDuration(GetId(), int.Parse(getDurationLastQuestion()));

        
        //Get the student objects present in the scene RICK: Also gets prefab element
        GameObject [] students = GameObject.FindGameObjectsWithTag("student");

        foreach(var student in students) {
            Student student_component = student.GetComponent<Student>();
            //If the id of the object corresponds to the id of the student detected in the data
            if (student_component.getID().ToString() == GetId()) {
                // TODO RICK: Update logic in simulation changes

                //Set the new icon on the student it uses the 'getPlayState' method on the StudentCSVSim
                //script to figure out which play state it should be.
                //Debug.Log(getPlayState().ToString());
                //student_component.setIcon(getPlayState().ToString());
                

                //Set total number of answered questions
                //student_component.set_student_answered_questions(getQuestionsAnsweredNumber());

                //Set the id of the exercise the student is working on (Ideally, it should be an exercise name)
                //student_component.setCurrentExerciseID(getCurrentExerciseID());

                //Set the number of the exercise the student is working on
                //student_component.setCurrentExerciseNumber(getCurrentExerciseNumber());

                //Set the current elo score
                student_component.setELOScore(getELOScore());
           
                //Set current objective
                //student_component.setCurrentObjective(objective_random_names[getCurrentObjective()]);

                //student_component.updateELOScore();

                //student_component.setTaskBarVal(getCurrentExerciseNumber());
                //if(student_component.getMicrogoal() != getNextMicrogoal()) {

                //    for(int c = 0; c < MicrogoalID.Count; c++) {

                //        if(MicrogoalID[c] == getNextMicrogoal()) {

                //            student_component.setCurrentMicrogoal(getNextMicrogoal(), MaxExerciseNumber[c]);

                //        }
                //    }
                //}
                

            }
        }

    }
    




}
