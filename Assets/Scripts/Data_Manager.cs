using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class Data_Manager : MonoBehaviour
{

	private string[,] student_timestamp_data;
	private string[][] student_timestamp_data_jagged;

	private int pointer;
	private DateTime TIMESTAMP_CURRENT;
	private DateTime TIMESTAMP_SIMULATE;

	// Connect this to an object with TMPro.TextMeshProUGUI component!
	// to display the current simulated time
	public GameObject timepanel;

	//simulator per id discovered in the excel sheet
	private List<StudentCSVSim> mySimulators;
	private bool fast_run;
	private bool fast_simulate_on;

	private bool startSimulation;

	private Class_Manager class_Manager;

	private GlobalVars gv;


	// Start is called before the first frame update
	void Start()
	{
		gv = GameObject.Find("System_Scripts").transform.Find("GlobalVars").GetComponent<GlobalVars>();
		read_Exercise_File();
		class_Manager = GetComponent<Class_Manager>();
		startSimulation = false;
		// state variables
		fast_run = true;
		fast_simulate_on = false;
		mySimulators = new List<StudentCSVSim>();
		read_Data_File();
		TIMESTAMP_CURRENT = ExtractTimeStamp(0);
		// fast-run until this datetime
		TIMESTAMP_SIMULATE = new DateTime(2019,11,12,08,20,00);
		//pointer through the data
		pointer = 0;
		startSimulation = true;
	}
	

	private void read_Exercise_File() {
		TextAsset exercises = Resources.Load("Exercise_Data") as TextAsset;
		string[] fLines = Regex.Split( exercises.text, "\n" );

		// split into jagged array
		for( int i=1; i < fLines.Length; i++ ){
			string[] line = Regex.Split( fLines[i], "," );
			if(!gv.MicrogoalID.Contains(line[1])) {
				gv.MicrogoalID.Add(line[1]);

				try {
				    gv.MaxExerciseNumber.Add(int.Parse(line[0]));
				} catch(Exception) {
					gv.MaxExerciseNumber.Add(0);
				}
			}
			else {
				for(int j = 0; j < gv.MicrogoalID.Count; j++) {

					if(gv.MicrogoalID[j] == line[1]) {

						try {
							if(int.Parse(line[0]) > gv.MaxExerciseNumber[j]) {
								gv.MaxExerciseNumber[j] = int.Parse(line[0]); 
							}     
						} catch(Exception) {}
					}
				}
			}
		}
	}

	public DateTime getCurrentTime() {
		return TIMESTAMP_CURRENT;
	}

	public List<StudentCSVSim> getStudentSimulators(){
		return mySimulators;
	}

	private void Update() {
		// RICK: Changed, only start simulation when Start Demo is pressed.
			//startSimulation = true;
	}


	//-----------------------------------------------------------------------------------------------
	// Starting simulation in a different thread so that it runs in the background
	//-----------------------------------------------------------------------------------------------
	void FixedUpdate(){

		if(startSimulation) {
			// display current time
			timepanel.GetComponent<TMPro.TextMeshProUGUI>().text = TIMESTAMP_CURRENT.ToString("dd/MM/yyyy HH:mm:ss");
			// start coroutine once
			if(!fast_simulate_on){
				StartCoroutine("Simulate");
				fast_simulate_on = true;
			}
		}      
	}
	//-----------------------------------------------------------------------------------------------
	// Fast-simulate until timestamp, normal-simulate after timestamp
	//-----------------------------------------------------------------------------------------------
	IEnumerator Simulate()
	{
		// fast-run through the data
		int count = 0;
		while(TIMESTAMP_SIMULATE > TIMESTAMP_CURRENT){
			UpdateEveryRow();
			count++;
			// count to update multiple times per frame (to do multiple steps per frame)
			if(count > 1){
				yield return null;
				count = 0;
			}
		}
		// turn on normal run
		if((TIMESTAMP_SIMULATE <= TIMESTAMP_CURRENT) && fast_run){
			InvokeRepeating("UpdateEverySecond", 0, 1.0F);
			fast_run = false;
		}
	}


	//-----------------------------------------------------------------------------------------------
	// SIMULATOR SLOW
	// adds a second to the current simulated time and check if time matches the current row
	// adds updates correct user if needed
	// moves pointer for the excel sheet if needed
	//-----------------------------------------------------------------------------------------------
	private void UpdateEverySecond(){

		// check if current timestamp receives new information:
		while(TIMESTAMP_CURRENT == ExtractTimeStamp(pointer)){
			updateUser(pointer);
			pointer++;
		}

		// time ticks:
		TIMESTAMP_CURRENT = TIMESTAMP_CURRENT.Add(new TimeSpan(0,0,1));
		updateStudentsIdle();
	}


	//-----------------------------------------------------------------------------------------------
	//	SIMULATOR FAST
	// use this function when you want to generate the next row, regardless of the timestamp
	// reads next line
	// adds updates correct user
	// moves pointer for the excel sheet
	//-----------------------------------------------------------------------------------------------
	private void UpdateEveryRow(){
		updateUser(pointer);
		pointer++;
		TIMESTAMP_CURRENT = ExtractTimeStamp(pointer);
		updateStudentsIdle();
	}

	//-----------------------------------------------------------------------------------------------
	// Data is transferred to the correct simulator, where it can be processed and/or stored and/or 
	// displayed.
	//-----------------------------------------------------------------------------------------------
	private void updateUser(int index){
		int id_row = 37; // column for identification
		bool found = false;
		StudentCSVSim currentStudent = null;

		// find correct simulator
		foreach(StudentCSVSim scs in mySimulators){
			if(scs.GetId() == student_timestamp_data[index,id_row]){
				scs.SetNewData(student_timestamp_data_jagged[index], TIMESTAMP_CURRENT, gv.MicrogoalID, gv.MaxExerciseNumber);
				currentStudent = scs;
				found = true;
			}
		}
		// generate new student simulator
		if(!found){
			StudentCSVSim newcsv = new StudentCSVSim();  
			newcsv.SetNewData(student_timestamp_data_jagged[index], TIMESTAMP_CURRENT, gv.MicrogoalID, gv.MaxExerciseNumber);
			//print("new student sim created: " + newcsv.GetId());
			mySimulators.Add(newcsv);
			currentStudent = newcsv;
		}
	}


	//-----------------------------------------------------------------------------------------------
	// Handles wether a student should be set into an 'idle' state based on time spent in a question
	// and student performance up until that point.
	//-----------------------------------------------------------------------------------------------

	public void updateStudentsIdle() {

		foreach(var simulator in mySimulators) {
			Student studentObject = null;

			try{ 
				studentObject = findStudentObject(simulator.GetId());
			} catch(Exception) {}

			if(simulator.getlastAttemptTime().AddMinutes(simulator.getAverageQuestionDelay()) < TIMESTAMP_CURRENT) {
				
				string icon = "";

				//Student is procrastinating
				if(simulator.getlastAttemptTime().AddMinutes(simulator.getAwayTime()) < TIMESTAMP_CURRENT) {
					icon = "Idle_White";
				}
				//Student is away
				else if(simulator.getlastAttemptTime().AddMinutes(simulator.getLazyTime()) < TIMESTAMP_CURRENT) {
					icon = "Idle_Grey";
				}
				else {

					try {                     
						switch(simulator.getCurrentState()) {
							case "Play_Green":
								icon = "Idle_Green";
							break;
							case "Play_Orange":
								icon = "Idle_Orange";
							break;
							case "Play_Red":
								icon = "Idle_Red";
							break;
							case "Star":
								icon = "Idle_Yellow";
							break;
						}
							
					} catch(Exception) {};
				}

				try {
					// TODO RICK: try to change state icons 
					//Set new icon and re-start the time on state only if previous icon is 
					//different to new icon
					//if(studentObject.getIcon() != icon) {
					//	studentObject.setIcon(icon);
					//	simulator.setNewStateTime(TIMESTAMP_CURRENT);
					//}
				} catch (Exception){}
			}

			 try {
				// TODO: RICK can probably all be deleted
				//Calculate and set the time spent on the current state on the extraInfo box
				// and on the main infoBox (on the latter only if it's active for that student).
				//TimeSpan timeOnState = TIMESTAMP_CURRENT - simulator.getStateTime();
				//studentObject.setTimeSpan(timeOnState);

				//if(studentObject.getInfoBoxActive()) {
				//	studentObject.set_student_info();
				//}

				//studentObject.set_Extra_Info();

			} catch(Exception) {};
		}
	}

	// called when new student is placed
	public void ForceUpdate(string id){
		foreach (StudentCSVSim sim in mySimulators){
			if(sim.GetId() == id){
				sim.updateStudentPlay(gv.MicrogoalID, gv.MaxExerciseNumber);
			}  
		}
	}
	//-----------------------------------------------------------------------------------------------
	// ADDED
	//-----------------------------------------------------------------------------------------------
	// Method to return the Student component from a student object in the world based on student ID
	//-----------------------------------------------------------------------------------------------
	private Student findStudentObject(string ID) {
		GameObject [] students = GameObject.FindGameObjectsWithTag("student");

		foreach(var student in students) {
			Student student_component = student.GetComponent<Student>();

			if (student_component.getID().ToString() == ID) {
				return student_component;
			}
		}
		return null;
	}

	private DateTime ExtractTimeStamp(int index){
		return DateTime.Parse(student_timestamp_data[index,0].Substring(0, student_timestamp_data[index,0].Length-4));
	}

	// read from Resources the data sample and save in variable
	private void read_Data_File() {

		// initialisation
		TextAsset studnames = Resources.Load("GynzyDataSet") as TextAsset;
		string fs = studnames.text.Replace("\r", "");

		// split into array and cut off first and last row
		string[] fLines = Regex.Split( fs, "\n" );
		string[] fLines2 = fLines.Skip(1).Take(fLines.Length-2).ToArray();

		// sort array (automatically sorts on timestamp)
		Array.Sort(fLines2, (x,y) => String.Compare(x,y));

		//create empty jagged array
		string[][] f2dLines = new string[fLines2.Length][];

		// split into jagged array
		for( int i=0; i < fLines2.Length; i++ ){
			 f2dLines[i] = Regex.Split( fLines2[i], ";" );
		}
		// also save a non-jagged version of array (for extraction purposes)
		student_timestamp_data_jagged = f2dLines;

		// convert data from jagged to 2d array
		student_timestamp_data = To2D(f2dLines);
	}

	// helper function to convert from jagged array to 2d array
	static T[,] To2D<T>(T[][] source)
	{
		try
		{
			int FirstDim = source.Length;
			int SecondDim = 63; // throws InvalidOperationException if source is not rectangular

			var result = new T[FirstDim, SecondDim];
			for (int i = 0; i < FirstDim; ++i){
				for (int j = 0; j < SecondDim; ++j){
					result[i, j] = source[i][j];
				}
			}
			return result;
		}
		catch (InvalidOperationException e)
		{
			throw new InvalidOperationException("The given jagged array is not rectangular.");
		} 
	}

}
