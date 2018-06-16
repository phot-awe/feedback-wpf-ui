using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using feedback_ui.Annotations;

namespace feedback_ui
{
    public class question 
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string title = "";

        // the question itself
        public string msg = "";

        // more information about the question - if needed
        public string extra_info = "";

        public class option {
            // the option itself
            public string msg = "";

            public string extra_info = "";

            public bool show_more_text = true;
        }
        public List<option> options = new List<option>();

        // if true, we'll give the user to write more info related to his answer
        public bool show_more_text = true;

        public double go_next_secs {
            get {
                return auto_go_next_secs == -1 ? (show_more_text ? 2.7 : .6) : auto_go_next_secs;
            }
        }

        // if > 0, once the user selects an option, we'll automatically go to the next question
        // (this is auto turned off if the user starts hovering the "more text" or has entered any text)
        //
        // -1 -> use defaults: .7 (if no "show more text") or 2.5 (if "show more text" -> to allow the user to decide if we'll write something)
        public double auto_go_next_secs = -1;

        // if true, we'll show an extra option that allows the user to skip to the next question
        public bool allow_skip = true;


    }
}
