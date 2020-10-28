using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class RadialSliderCorrectAnswers : MonoBehaviour
{
    public int totalAnswers;
    public int correctFirstTryAnswers;
    public int correctSecondTryAnswers;
    public int incorrectAnswers;
    public Image Incorrect;
    public Image CorrectFirst;
    public Image CorrectSecond;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
    }

    void GetCurrentFill()
    {
        CorrectFirst.fillAmount = (float)correctFirstTryAnswers / (float)totalAnswers;
        CorrectSecond.fillAmount = (float)(correctFirstTryAnswers + correctSecondTryAnswers) / (float)totalAnswers;
        Incorrect.fillAmount = (float)(incorrectAnswers + correctFirstTryAnswers + correctSecondTryAnswers) / (float)totalAnswers;
    }

}
