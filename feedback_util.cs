using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace feedback_ui
{
    class feedback_util
    {
        public static void postpone( Action a, int milliseconds) {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(milliseconds);
            timer.Tick += (s, e) => {
                a();
                timer.Stop();
            };
            timer.Start();            
        }

        public static bool is_email_valid(string email) {
            bool email_valid = email.Count(c => c == '@') == 1 && email.Count(c => c == '.') >= 1;
            if (email_valid)
                // can't have email ending in dot
                email_valid = email.LastIndexOf(".") < email.Length-1;
            return email_valid;            
        }
    }
}
