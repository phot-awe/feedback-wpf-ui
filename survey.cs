using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using feedback_ui.Annotations;

namespace feedback_ui
{
    public class survey 
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<question> questions_  = new List<question>();

        public IReadOnlyList<question> questions {
            get { return questions_; }
        }

        // uniquely identify this survey
        public string id { get; set; } = "survey";

        // in case you want to show an image regarding the test - like, for a specific set of users
        public Uri survey_type_uri { get; set; } = null;

        public string hi_existing_user { get; set; } = "Hi %,\r\nLets get to know each other.";
        public string hi_new_user { get; set; } = "Hi %,\r\nThank you for choosing to give us feedback.\r\nLets Get Started!";

        public string skip_msg { get; set; } = "Skip";

        // Our "Thank You" for completing this: 
        public string in_it_for_you { get; set; } = "3 months FREE of using our product.";

        // Our "Thank You" for completing this: 
        public string in_it_for_you2 { get; set; } = "";

        public bool skip_social { get; set; } = false;

        // if you have a "share on fb/twitter" extra offer
        public string share_it_offer { get; set; } = "";

        // if you have a "share on fb/twitter" extra offer
        public string share_it_offer2 { get; set; } = "";

        public string do_not_share_it_offer { get; set; } = "I do not want this extra offer.";

        // where to go when user presses share on twitter
        public string share_twitter_link { get; set; } = "";
        // where to go when user presses share on fb
        public string share_facebook_link { get; set; } = "";

        // where to go when user presses share on reddit
        public string share_reddit_link { get; set; } = "";

        public static survey from_file(string file) {
            try {
                var lines = File.ReadAllLines(file);
                return from_lines(lines);
            } catch (Exception e) {
                logger.Error("can't read questions file " + file + " : " + e.Message);
            }
            return new survey();
        }

        public static survey from_lines(string[] lines) {
            survey s = new survey();

            var last_line_was_question = false;
            var last_line_was_options = false;
            foreach (var l in lines.Where(ll => ll.Trim().Length > 0)) {
                if (l.StartsWith("+")) {
                    var idx = l.IndexOf(" ");
                    var prefix = idx >= 0 ? l.Substring(0, idx).Trim() : l;
                    var suffix = idx >= 0 ? l.Substring(idx).Trim() : "";
                    suffix = suffix.Replace("\\r", "\r").Replace("\\n", "\n");
                    switch (prefix) {
                        case "+id":
                            s.id = suffix;
                            break;
                        case "+image_uri":
                            s.survey_type_uri = new Uri(suffix);
                            break;
                        case "+hi_existing_user":
                            s.hi_existing_user = suffix;
                            break;
                        case "+skip_social":
                            s.skip_social = true;
                            break;
                        case "+hi_new_user":
                            s.hi_new_user = suffix;
                            break;
                        case "+in_it_for_you":
                            s.in_it_for_you = suffix;
                            break;
                        case "+in_it_for_you2":
                            s.in_it_for_you2 = suffix;
                            break;
                        case "+share_it_offer":
                            s.share_it_offer = suffix;
                            break;
                        case "+share_it_offer2":
                            s.share_it_offer2 = suffix;
                            break;
                        case "+share_twitter_link":
                            s.share_twitter_link = suffix;
                            break;
                        case "+share_facebook_link":
                            s.share_facebook_link = suffix;
                            break;

                        case "+share_reddit_link":
                            s.share_reddit_link = suffix;
                            break;

                        case "+no_skip":
                            s.questions_.Last().allow_skip = false;
                            break;
                        case "+do_not_share_offer":
                            s.do_not_share_it_offer = suffix;
                            break;
                        case "+extra":
                            // note: we now allow questions with no answers - it's basically in-between explanations
                            var last = s.questions.Last();
                            last.extra_info = suffix;
                            if (last.options.Count == 0) {
                                last.options.Add(new question.option { msg = "Next", show_more_text = false} );
                                last.allow_skip = false;
                                // ... ignore the ending '?' - it's not a question
                                last.msg = last.msg.Substring(0, last.msg.Length - 1);
                            }
                            break;
                        default:
                            logger.Error("invalid survey line " + l);
                        break;
                    }
                }
                else if (l.StartsWith("/")) {
                    // extra for option
                    var options = s.questions_.Last().options;
                    var cur_option = options.FirstOrDefault(o => l.StartsWith("/" + o.msg));
                    if (cur_option != null) {
                        var extra = l.Substring(cur_option.msg.Length + 1).Trim();
                        cur_option.extra_info = extra;
                    }
                    else 
                        logger.Error("invalid option line " + l);
                } 
                else if (l.StartsWith("#")) {
                    // comment
                } else {
                    var line_is_question = l.Trim().EndsWith("?");
                    if (line_is_question) {
                        // title/message
                        var idx = l.IndexOf("/");
                        Debug.Assert(idx >= 0);
                        var title = l.Substring(0, idx).Trim();
                        var msg = l.Substring(idx + 1).Trim();
                        s.questions_.Add(new question {msg = msg, title = title});
                    } else {
                        Debug.Assert(last_line_was_question);
                        // this is the Options line - we need at least one option
                        var options = l.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries).Select(o => new question.option {msg = o.Trim()}).ToList();
                        s.questions_.Last().options = options;
                    }
                }
                last_line_was_options = last_line_was_question && !l.StartsWith("#") && l.Trim().Contains("/");
                last_line_was_question = !l.StartsWith("#") && l.Trim().EndsWith("?");
            }

            // add Skip - where needed
            foreach (var q in s.questions_)
                if ( q.allow_skip)
                    q.options.Add(new question.option { msg = s.skip_msg, show_more_text = false});
            return s;
        }

    }
}
