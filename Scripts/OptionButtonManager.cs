using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionButtonManager : MonoBehaviour
{

    private SurveyManager _surveyManager;

    public void Start()
    {
		_surveyManager = FindObjectOfType<SurveyManager>();
    }

    public void OptionClick(GameObject button)
    {
        _surveyManager.LogAnswer(button.GetComponentInChildren<Text>().text);
    }

    public void SurveyFinished()
    {
        _surveyManager.FinishSurvey();
    }

}
