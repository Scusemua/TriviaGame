using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivia
{
    public class TriviaQuestion
    {
        public string Question { get; set; }

        public string Answer { get; set; }

        public int Grade { get; set; }

        public string Subject { get; set; }

        public TriviaQuestion(string question, string answer, int grade, string subject)
        {
            Question = question;
            Answer = answer;
            Grade = grade;
            Subject = subject;
        }
    }
}
