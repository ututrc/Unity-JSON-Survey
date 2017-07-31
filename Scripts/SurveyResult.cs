using System.Collections.Generic;
using SurveyHandler;
using UnityEngine;
using System.Collections;
using UnityEngineInternal;

public class SurveyResult {

    public string Title { get; set; }
    public List<QuestionResult> QuestionResults = new List<QuestionResult>();

}
