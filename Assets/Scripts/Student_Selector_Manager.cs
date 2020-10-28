using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Linq;
using System;

//----------------------------------------------------------
// Generates a student list inside the student selector
// From the file 'Ginzy_Students_List.csv' in 'Resources
//----------------------------------------------------------
public class Student_Selector_Manager : MonoBehaviour
{
    public GameObject canvas;

    [SerializeField]
    private GameObject button;

    private List<string> stringList;
    private List<string[]> parsedList;

    List<string> student_Names = new List<string>();
    List<int> student_IDs = new List<int>();

    private void Awake() {
        //button = canvas.transform.Find("Student_List_Button").gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        stringList = new List<string>();
        parsedList = new List<string[]>();

        read_Student_File_Android();
        initialise_student_List();
    }


    // Read student names and ids from text file in Resources.
    private void read_Student_File_Android() {
        TextAsset studnames = Resources.Load("Ginzy_Students_List") as TextAsset;
        string[] fLines = Regex.Split( studnames.text, "\n" );
        //print(fLines[0]);

        // split into jagged array
        for( int i=0; i < fLines.Length; i++ ){
             string[] student_line = Regex.Split( fLines[i], ";" );
             student_Names.Add(student_line[1]);
             student_IDs.Add(int.Parse(student_line[0]));

		}

    }

    private void read_Student_File() {
        string path = "Assets/Resources/Ginzy_Students_List.csv";
        StreamReader inp_stm = new StreamReader(path);

        while (!inp_stm.EndOfStream)
        {
            string inp_ln = inp_stm.ReadLine();

            stringList.Add(inp_ln);
        }

        inp_stm.Close();

            parseList();
        }

        void parseList()
        {
            for (int i = 0; i < stringList.Count; i++)
            {
                string[] temp = stringList[i].Split(';');
                for (int j = 0; j < temp.Length; j++)
                {
                    temp[j] = temp[j].Trim();
                }
                parsedList.Add(temp);
            }

            foreach (var item in parsedList)
            {
                student_Names.Add(item[1]);
                student_IDs.Add(int.Parse(item[0]));
            }
            
        }

    private void initialise_student_List() {
        GameObject newButton;
        int count = 0;

        foreach(string name in student_Names) {

            newButton = Instantiate(button) as GameObject;
            newButton.transform.SetParent(canvas.transform, false);

            newButton.SetActive(true);
            newButton.transform.Find("Text").gameObject.GetComponent<Text>().text = name;
            newButton.GetComponent<Student_Data>().ID = student_IDs[count];

            newButton.GetComponent<Student_Data>().name = name + ", ID: " + student_IDs[count];

            count += 1;

        }
    }
}
