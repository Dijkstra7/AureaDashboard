using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using UnityEngine.UI;
using GoogleARCore;
using GoogleARCore.Examples.Common;

//--------------------------------------------------------------------------------
// To manage overall class data
//--------------------------------------------------------------------------------
public class Class_Manager : MonoBehaviour
{
    public GameObject classOverviewImage;

    // ordered list of top k wrong questions:
    private IEnumerable<string> mostDifficultExercises;
    private List<string> incorrectly_answered_exercises;

    // ordered list of exercise durations:
    Dictionary<string, List<int>> id_durations_dict;
    private IEnumerable<string> longest_average_exercise;

    // ordered list of student durations:
    Dictionary<string, List<int>> student_durations_dict;
    private IEnumerable<string> ordered_average_duration_student;

    // list of elo scores per objective
    Dictionary<string,List<int>> eloscoresperobjective;
    Dictionary<string,int> averageELOperobjective;
    
    private bool started;

    
    public Dictionary<string,string> objective_random_names;

    /// <summary>
    /// List of markers that will be scanned for
    /// </summary>
    private List<AugmentedImage> m_markers = new List<AugmentedImage>();

    /* Output variables:
    */


    private void Start() {
        mostDifficultExercises = new List<string>();
        incorrectly_answered_exercises = new List<string>();

        id_durations_dict = new Dictionary<string, List<int>>();
        student_durations_dict = new Dictionary<string, List<int>>();

        eloscoresperobjective = new Dictionary<string,List<int>>();
        averageELOperobjective = new Dictionary<string,int>();
        started = false;
    }

    private void Update(){
        if(started == false){
            started = true;
            StartCoroutine("UpdateEverySecond");
		}
	}

    private void UpdateEverySecond(){
        InvokeRepeating("addELOObjvalues", 0, 1.0F);
	}

    // function is called every second
    public void addELOObjvalues(){
        // find all the elo scores
        List<StudentCSVSim> scsvsim = GameObject.Find("Student_Manager").GetComponent<Data_Manager>().getStudentSimulators();

        objective_random_names = scsvsim[0].GetDictionarySubjects();
        eloscoresperobjective = new Dictionary<string,List<int>>();
        // ohno a double for-loop appeared, its not very effective
        foreach(StudentCSVSim sim in scsvsim){
            foreach(KeyValuePair<string, int> pair in sim.objectiveELO){
                if(eloscoresperobjective.ContainsKey(pair.Key)){
                    eloscoresperobjective[pair.Key].Add(pair.Value);
                }
                else{
                    eloscoresperobjective.Add(pair.Key, new List<int>(){pair.Value});
				}
            }
        }
        averageELOperobjective = new Dictionary<string,int>();
        foreach(KeyValuePair<string, List<int>> pair in eloscoresperobjective){
            print(pair.Value);

            averageELOperobjective.Add(pair.Key, pair.Value.Sum()/pair.Value.Count());
		}

        update_Info();
        
	}
        

    /*
    public void addStudentDuration(string studentID, int duration){
        if(id_durations_dict.ContainsKey(studentID)){
            id_durations_dict[studentID].Add(duration);
        }
        else{
            id_durations_dict.Add(studentID, new List<int>(){duration});
            print(duration);
		}
        longest_average_exercise = from pair in id_durations_dict
                            orderby pair.Value.Sum()/pair.Value.Count() descending
                            select pair.Key;
        update_Info();
	}
    */

    public void addExerciseDuration(string exerciseID, int duration) {
        if(id_durations_dict.ContainsKey(exerciseID)){
            id_durations_dict[exerciseID].Add(duration);
        }
        else{
            id_durations_dict.Add(exerciseID, new List<int>(){duration});
            print(duration);
		}
        longest_average_exercise = from pair in id_durations_dict
                            orderby pair.Value.Sum()/pair.Value.Count() descending
                            select pair.Key;
        update_Info();

    }

    public void addWronglyAnsweredExercise(string exerciseID) {
        incorrectly_answered_exercises.Add(exerciseID);
        mostDifficultExercises = from i in incorrectly_answered_exercises
            group i by i into grp
            orderby grp.Count() descending
            select grp.Key;
        update_Info();
    }

    //Only updates the most difficult exercises for now.
    private void update_Info() {

        // update hardest exercises
        string exercises = "Hardest exercises:";
        for(int i = 0;i < 5; i++){
            try{
                exercises = exercises + "\n ex." + mostDifficultExercises.ElementAt(i);
			}
            catch(Exception e){
            
			}
        }
        classOverviewImage.transform.Find("Panel1").transform.Find("Progress_1").GetComponent<Text>().text = exercises;

        // update longest exercises
        string exercises2 = "Longest exercises:";
        for(int i = 0;i < 5; i++){
            try{
                exercises2 = exercises2 + "\n ex." + longest_average_exercise.ElementAt(i);
			}
            catch(Exception e){
            
			}
        }

        classOverviewImage.transform.Find("Panel2").transform.Find("Progress_2").GetComponent<Text>().text = exercises2;

        // update elo score per objective
        string exercises3 = "Average elo score per objective:";
        foreach(KeyValuePair<string, int> pair in averageELOperobjective){
            try{
                exercises3 = exercises3 + "\n" + objective_random_names[pair.Key] + ", Average ELO: " + + pair.Value;
			}
            catch(Exception e){
            
			}
        }

        classOverviewImage.transform.Find("Panel3").transform.Find("Progress_3").GetComponent<Text>().text = exercises3;


        // Update testing whether images can be found
        Session.GetTrackables<AugmentedImage>(
            m_markers, TrackableQueryFilter.Updated);
        var update_text = "";
        if (m_markers.Count > 0)
        {
            var image = m_markers.First();
            update_text = image.Name.ToString() + " is found"
                +"\n"+m_markers.Count.ToString() + "Images found in total.";
        }
        else update_text = m_markers.Count.ToString() + " Images that will be detected";
        classOverviewImage.transform.Find("Panel3 (1)").transform.Find("Progress_3").GetComponent<Text>().text = update_text;
    }   

    private void resetList() {
        incorrectly_answered_exercises.Clear();
    }

    
}
