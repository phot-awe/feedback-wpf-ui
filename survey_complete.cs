using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feedback_ui
{
    public class survey_complete {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool shared_on_twitter = false;
        public bool shared_on_fb = false;

        private survey survey_ = null;

        public readonly DateTime survey_start = DateTime.Now;

        // the answer to a single question
        internal class answer {
            public bool answered = false;
            public int option = -1;
            public string extra = "";

            public int answer_ms = 0;

            public DateTime see_question = DateTime.Now;
            private DateTime answer_time_ = DateTime.Now;

            public DateTime answer_time {
                get { return answer_time_; }
                set {
                    answer_time_ = value; 
                    Debug.Assert(answer_time > see_question);
                    answer_ms = (int) (answer_time - see_question).TotalMilliseconds;
                    //logger.Debug("question answered in " + answer_ms);
                }
            }
        }

        internal List<answer> answers_ = new List<answer>();

        public survey survey {
            get { return survey_; }
            internal set {
                // set it only once!
                Debug.Assert(survey_ == null);
                survey_ = value; 
                for (int i = 0; i < survey.questions.Count; ++i)
                    answers_.Add(new answer());
            }
        }

        public string user_name = "";
        public string user_email = "";

        // note: if a question is not answered, we return "","" 
        public IReadOnlyList<Tuple<string,string,int> > answers {
            get {
                int idx = 0;
                return answers_.Select(a => {
                    var result = new Tuple<string, string, int>(answers_[idx].answered ? survey_.questions[idx].options[a.option].msg : "", answers_[idx].extra, answers_[idx].answer_ms);
                    ++idx;
                    return result;
                }).ToList() ;
            }
        }

        public string friendly_msg() {
            var msg = "";
            msg += "Feedback from " + user_name + " (" + user_email + ")\r\n\r\n";
            var answers = this.answers;
            for (var i = 0; i < answers.Count; i++) {
                var a = answers[i];
                var q = survey.questions[i];
                msg += (i + 1) + ": " + q.msg + "\r\n" + (a.Item1 != "" ? a.Item1 : "(skipped)") + (a.Item2 != "" ? "\r\n" + a.Item2 : "");
                msg += "\r\n(answered: " + a.Item3 + "ms)";
                msg += "\r\n\r\n";
            }

            msg += "Shared on Twitter: " + (shared_on_twitter ? "YES" : "-") + "\r\n";
            msg += "Shared on Facebook: " + (shared_on_fb ? "YES" : "-") + "\r\n";

            msg += "\r\nTotal time:" + (int)(DateTime.Now - survey_start).TotalSeconds + "secs";

            return msg;
        }
    }
}
