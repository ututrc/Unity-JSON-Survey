using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace SurveyHandler
{
    public class Survey
    {
        public string Title { get; set; }
        public List<QuestionOption> Questions = new List<QuestionOption>();

        /// <summary>
        /// Loads json from resources
        /// </summary>
        /// <param name="path">Path to resourcefile</param>
		public void LoadJson(TextAsset testJson)
        {
            //TextAsset testJson = Resources.Load(path) as TextAsset;
            var dict = Json.Deserialize(testJson.text) as Dictionary<string, object>;

            Dictionary<string, object> jsonSurvey = (Dictionary<string, object>)dict["survey"];
            List<object> questionsList = (List<object>)jsonSurvey["questions"];
            this.Title = (string) jsonSurvey["title"];

            foreach (Dictionary<string, object> question in questionsList)
            {
                long id = (long)question["id"];
                string questionString = (string)question["question"];
                List<string> answers = new List<string>();

                foreach (var o in (List<object>)question["answers"])
                {
                    answers.Add((string)o);
                }

                Questions.Add(new QuestionOption()
                {
                    ID = (int)id,
                    Question = questionString,
                    Options = answers
                });
            }

        }
    }

}

