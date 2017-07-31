
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using AR.Core;
using MiniJSON;
using SurveyHandler;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class SurveyManager : MonoBehaviour {

    public GameObject ButtonPrefab;
    public GameObject QuestionContainer;
    public GameObject AnswerContainer;
    public GameObject SurveyFinishedButton;
    public GameObject InputPanelPrefab;
    public string UserID;
    public string BaseUrl;
    public string PublicKey;
    public string PrivateKey;

	public TextAsset JsonFile;

    public string sceneAfterDone;

    private Survey _survey;
    private int _currentQuestion = -1;

    private SurveyResult _surveyResult = new SurveyResult();

	private AR.Extras.DataLogger _datalogger;

    public void Start()
    {
		//_datalogger = GameObject.Find ("Datalogger").GetComponent<AR.Extras.DataLogger> ();

        this._survey = new Survey();
        _survey.LoadJson(JsonFile);
        _surveyResult.QuestionResults = new List<QuestionResult>();

		//AR.ServerConnection appManager = AR.ServerConnection.Instance;
        //if (appManager != null)
        //    UserID = appManager.;

        ShowNextQuestion();
    }

	public void ResetSurvey() {
		_surveyResult.QuestionResults.Clear ();
		_currentQuestion = -1;
		ShowNextQuestion ();
	}

    public void ShowNextQuestion()
    {
        if (_currentQuestion < _survey.Questions.Count - 1)
        {
            clearButtons();

            _currentQuestion++;

            QuestionContainer.GetComponentInChildren<Text>().text = _survey.Questions[_currentQuestion].Question;

            foreach (string answer in _survey.Questions[_currentQuestion].Options)
            {
                GameObject button = Instantiate(ButtonPrefab) as GameObject;
                button.GetComponentInChildren<Text>().text = answer;
                button.transform.SetParent(AnswerContainer.transform);
            }
            if (_survey.Questions[_currentQuestion].Options.Count == 0)
            {
                GameObject inputFieldGameObject = Instantiate(InputPanelPrefab) as GameObject;
                inputFieldGameObject.transform.SetParent(AnswerContainer.transform);
            }
        }
        else
        {
            clearButtons();

            QuestionContainer.GetComponentInChildren<Text>().text = "Kysely on valmis, ole hyvä ja tallenna vastauksesi!";

            GameObject button = Instantiate(SurveyFinishedButton) as GameObject;
            button.transform.SetParent(AnswerContainer.transform);
        }
    }

    public void ShowPreviousQuestion()
    {

    }

    public void LogAnswer(string answer)
    {
        QuestionResult questionResult = new QuestionResult()
        {
            Question = _survey.Questions[_currentQuestion].Question,
            Answer = answer,
            ID = _survey.Questions[_currentQuestion].ID
        };
        _surveyResult.QuestionResults.Add(questionResult);

		Dictionary<string, object> answerDict = new Dictionary<string, object> ();
		/*if (_datalogger.Ready) {
            _datalogger.LogSurvey(questionResult.Question, questionResult.Answer);
			//UserID = _datalogger.GetUserId();
		}*/
        //sendToWeb(questionResult);

        ShowNextQuestion();
    }

    public void FinishSurvey()
    {
        _surveyResult.Title = _survey.Title;
        var result = new Dictionary<string, object>();
        var surveyDict = new Dictionary<string, object>();
        surveyDict.Add("title", _surveyResult.Title);
        var listResults = new List<IDictionary<string, object>> ();
        foreach (QuestionResult questionResult in _surveyResult.QuestionResults)
        {
            IDictionary<string, object> res = ObjectToDictionaryHelper.ToDictionary(questionResult);
            listResults.Add(res);
        }
        surveyDict.Add("questions", listResults);
        result.Add("survey", surveyDict);
        string jsonResult = Json.Serialize(result);
        Debug.Log(jsonResult);
        writeFile(jsonResult);
		Debug.Log("file write complete");

        //GameObject survey = GameObject.Find("Survey");
        //Destroy(survey);
		ResetSurvey();
		this.gameObject.SetActive (false);
        Resources.UnloadUnusedAssets();

		if (sceneAfterDone != null && sceneAfterDone.Length > 0) 
			StartCoroutine(LoadScene(sceneAfterDone));
    }

	IEnumerator LoadScene(string scene) {
		yield return Application.LoadLevelAdditiveAsync (scene);
	}
    
    private void sendToWeb(QuestionResult questionResult)
    {
        
		if (BaseUrl.Length > 0) {
			string url =
				BaseUrl + "/input/" + PublicKey + "?private_key=" + PrivateKey + "&user_id=" + UserID + "&question=" + Uri.EscapeDataString (questionResult.Question) + "&answer=" + Uri.EscapeDataString (questionResult.Answer);

			var httpRequest = new WWW (url);

			StartCoroutine (WaitForRequest (httpRequest));
		}
        
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        if (www.error == null)
        {
            // OK
        }
        else
        {
            // error, retry
            Debug.Log("Added request to buffer");
            addToWwwBuffer(www);
        }
    }

    IEnumerator RetryRequest(WWW www)
    {
        yield return new WaitForSeconds(10.0f);
        WaitForRequest(www);
    }

    private void addToWwwBuffer(WWW www)
    {
        StartCoroutine(RetryRequest(www));
    }

    private void clearButtons()
    {
        foreach (Transform childTransform in AnswerContainer.transform)
        {
            Destroy(childTransform.gameObject);
        }
    }

    private void writeFile(string json)
    {
		string directory = Application.persistentDataPath;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        using (StreamWriter sw = new StreamWriter(directory + Path.DirectorySeparatorChar + _survey.Title + "__" + DateTime.Now.ToString("yy-mm-dd_hh-mm-ss") + ".json"))
        {
            sw.WriteLine(json);
        }

    }

}

public static class ObjectToDictionaryHelper
{
    public static IDictionary<string, object> ToDictionary(this object source)
    {
        return source.ToDictionary<object>();
    }

    public static IDictionary<string, T> ToDictionary<T>(this object source)
    {
        if (source == null)
            ThrowExceptionWhenSourceArgumentIsNull();

        var dictionary = new Dictionary<string, T>();
        foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
            AddPropertyToDictionary<T>(property, source, dictionary);
        return dictionary;
    }

    private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, T> dictionary)
    {
        object value = property.GetValue(source);
        if (IsOfType<T>(value))
            dictionary.Add(property.Name, (T)value);
    }

    private static bool IsOfType<T>(object value)
    {
        return value is T;
    }

    private static void ThrowExceptionWhenSourceArgumentIsNull()
    {
        throw new ArgumentNullException("source", "Unable to convert object to a dictionary. The source object is null.");
    }
}
