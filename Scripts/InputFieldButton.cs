using System.Net.Mime;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputFieldButton : MonoBehaviour
{
    private SurveyManager _surveyManager;
    
    public GameObject InputField;

    public void Start()
    {
		_surveyManager = FindObjectOfType<SurveyManager>();
    }

    public void DoneClick()
    {
        if (InputField.GetComponentInChildren<Text>().text.Length > 0)
        {
            Text[] fields = InputField.GetComponentsInChildren<Text>();
            foreach (Text field in fields)
            {
                if(field.name == "Text")
                    _surveyManager.LogAnswer(field.text);
            }
        }
        else
        {
            Debug.Log("Implementoi virheviesti tähän!");
        }
    }

}
