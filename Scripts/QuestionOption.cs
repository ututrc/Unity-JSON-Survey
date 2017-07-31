using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurveyHandler
{
    public class QuestionOption
    {
        public int ID { get; set; }
        public string Question { get; set; }
        public List<string> Options { get; set; }
    }
}

