using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using feedback_ui.Annotations;

namespace feedback_ui
{
    public class questions_view_model : INotifyPropertyChanged
    {
        private string user_email_ = "";
        private string user_name_ = "";
        private survey survey_;
        private bool is_minimized_;
        private bool is_asking_questions_;
        private string question_title_;
        private string question_msg_;
        private int option_count_;
        private string option0_;
        private string option1_;
        private string option2_;
        private string option3_;
        private string option4_;
        private string option5_;
        private bool is_option0_checked_;
        private bool is_option1_checked_;
        private bool is_option2_checked_;
        private bool is_option3_checked_;
        private bool is_option4_checked_;
        private bool is_option5_checked_;
        private string question_extra_;
        private string answer_extra_ = "";
        private int question_index_ = -1;
        private int question_count_;
        private string x_of_y_;

        private survey_complete survey_complete_;
        private int go_next_value_;
        private bool thank_you_;
        private bool is_on_saying_hi_page_;
        private Uri survey_type_uri_;
        private string hi_msg_;
        private bool is_existing_user_;
        private string our_thank_you_prefix_;
        private string our_thank_you_text_;
        private string our_thank_you_suffix_;
        private string our_thank_you_link_;
        private bool can_go_to_share_page_;
        private string our_thank_you2_;
        private string go_next_str_ = "";
        private bool is_on_share_page_;
        private bool can_start_;
        private string share_offer_;
        private string do_not_share_offer_;
        private string twitter_link_;
        private string facebook_link_;

        private string share_prefix_;
        private string share_text_;
        private string share_suffix_;
        private string share_link_;
        private string share2_;
        private string friendly_user_name_;
        private bool show_do_not_share_offer_ = true;
        private string reddit_link_;

        public Uri survey_type_uri {
            get { return survey_type_uri_; }
            set {
                if (Equals(value, survey_type_uri_))
                    return;
                survey_type_uri_ = value;
                on_property_changed();
            }
        }

        // when we know the user email and/or name, show that at the beginning
        public string user_email {
            get { return user_email_; }
            set {
                if (value == user_email_)
                    return;
                user_email_ = value;
                on_property_changed();
                can_go_to_share_page = util.is_email_valid(user_email_);
                update_friendly_user_name();
            }
        }

        public string friendly_user_name {
            get { return friendly_user_name_; }
            set {
                if (value == friendly_user_name_)
                    return;
                friendly_user_name_ = value;
                on_property_changed();
            }
        }

        public string user_name {
            get { return user_name_; }
            set {
                if (value == user_name_)
                    return;
                user_name_ = value;
                on_property_changed();
                update_friendly_user_name();
            }
        }

        public survey survey {
            get { return survey_; }
            internal set {
                // set this only once!
                Debug.Assert(survey_ == null);
                if (Equals(value, survey_))
                    return;
                survey_ = value;
                on_property_changed();
                on_survey_set();
            }
        }

        public bool is_minimized {
            get { return is_minimized_; }
            set {
                if (value == is_minimized_)
                    return;
                is_minimized_ = value;
                on_property_changed();
            }
        }

        public bool is_asking_questions {
            get { return is_asking_questions_; }
            set {
                if (value == is_asking_questions_)
                    return;
                is_asking_questions_ = value;
                on_property_changed();
                survey_complete_.answers_[question_index_].see_question = DateTime.Now;
            }
        }

        public string question_title {
            get { return question_title_; }
            set {
                if (value == question_title_)
                    return;
                question_title_ = value;
                on_property_changed();
            }
        }

        public string question_msg {
            get { return question_msg_; }
            set {
                if (value == question_msg_)
                    return;
                question_msg_ = value;
                on_property_changed();
            }
        }

        public int option_count {
            get { return option_count_; }
            set {
                if (value == option_count_)
                    return;
                option_count_ = value;
                on_property_changed();
            }
        }

        public string option0 {
            get { return option0_; }
            set {
                if (value == option0_)
                    return;
                option0_ = value;
                on_property_changed();
            }
        }

        public string option1 {
            get { return option1_; }
            set {
                if (value == option1_)
                    return;
                option1_ = value;
                on_property_changed();
            }
        }

        public string option2 {
            get { return option2_; }
            set {
                if (value == option2_)
                    return;
                option2_ = value;
                on_property_changed();
            }
        }

        public string option3 {
            get { return option3_; }
            set {
                if (value == option3_)
                    return;
                option3_ = value;
                on_property_changed();
            }
        }

        public string option4 {
            get { return option4_; }
            set {
                if (value == option4_)
                    return;
                option4_ = value;
                on_property_changed();
            }
        }

        public string option5 {
            get { return option5_; }
            set {
                if (value == option5_)
                    return;
                option5_ = value;
                on_property_changed();
            }
        }

        private void on_option_checked(int idx) {            
            survey_complete_.answers_[question_index_].option = idx;
            update_extra_info();
        }


        public bool is_option0_checked {
            get { return is_option0_checked_; }
            set {
                if (value == is_option0_checked_)
                    return;
                is_option0_checked_ = value;
                on_property_changed();

                if (question_index_ < survey.questions.Count && value)
                    on_option_checked(0);
            }
        }

        public bool is_option1_checked {
            get { return is_option1_checked_; }
            set {
                if (value == is_option1_checked_)
                    return;
                is_option1_checked_ = value;
                on_property_changed();

                if (question_index_ < survey.questions.Count && value)
                    on_option_checked(1);
            }
        }

        public bool is_option2_checked {
            get { return is_option2_checked_; }
            set {
                if (value == is_option2_checked_)
                    return;
                is_option2_checked_ = value;
                on_property_changed();
                if (question_index_ < survey.questions.Count && value)
                    on_option_checked(2);
            }
        }

        public bool is_option3_checked {
            get { return is_option3_checked_; }
            set {
                if (value == is_option3_checked_)
                    return;
                is_option3_checked_ = value;
                on_property_changed();
                if (question_index_ < survey.questions.Count && value)
                    on_option_checked(3);
            }
        }

        public bool is_option4_checked {
            get { return is_option4_checked_; }
            set {
                if (value == is_option4_checked_)
                    return;
                is_option4_checked_ = value;
                on_property_changed();
                if (question_index_ < survey.questions.Count && value)
                    on_option_checked(4);
            }
        }

        public bool is_option5_checked {
            get { return is_option5_checked_; }
            set {
                if (value == is_option5_checked_)
                    return;
                is_option5_checked_ = value;
                on_property_changed();
                if (question_index_ < survey.questions.Count && value)
                    on_option_checked(5);
            }
        }

        public string question_extra {
            get { return question_extra_; }
            set {
                if (value == question_extra_)
                    return;
                question_extra_ = value;
                on_property_changed();
            }
        }

        public string answer_extra {
            get { return answer_extra_; }
            set {
                if (value == answer_extra_)
                    return;
                var user_has_typed =  question_index < survey.questions.Count && value.Length == answer_extra_.Length + 1 && value.StartsWith(answer_extra_);
                if ( user_has_typed)
                    complete.answers_[question_index].answer_time = DateTime.Now;

                answer_extra_ = value;
                on_property_changed();

                if ( question_index < survey.questions.Count)
                    survey_complete_.answers_[question_index_].extra = value;

                if (value != "")
                    // at this point, the user started writing something - don't auto go to the next question anymore - let him finish
                    go_next_value = 0;
            }
        }

        public int question_index {
            get { return question_index_; }
            set {
                if (value == question_index_)
                    return;
                question_index_ = value;
                on_property_changed();
                x_of_y = "" + (question_index+1) + "/" + question_count;
                on_question_change();

                if ( question_index < survey.questions.Count)
                    survey_complete_.answers_[question_index_].see_question = DateTime.Now;
            }
        }

        public int question_count {
            get { return question_count_; }
            set {
                if (value == question_count_)
                    return;
                question_count_ = value;
                on_property_changed();
            }
        }

        public string x_of_y {
            get { return x_of_y_; }
            set {
                if (value == x_of_y_)
                    return;
                x_of_y_ = value;
                on_property_changed();
            }
        }

        public int go_next_value {
            get { return go_next_value_; }
            set {
                if (value == go_next_value_)
                    return;
                go_next_value_ = value;
                on_property_changed();
                go_next_str = new string('.', go_next_value / 10);
            }
        }

        public string go_next_str {
            get { return go_next_str_; }
            set {
                if (value == go_next_str_)
                    return;
                go_next_str_ = value;
                on_property_changed();
            }
        }

        public bool thank_you {
            get { return thank_you_; }
            set {
                if (value == thank_you_)
                    return;
                thank_you_ = value;
                on_property_changed();
                if (thank_you)
                    is_asking_questions = false;
            }
        }

        public bool is_on_saying_hi_page {
            get { return is_on_saying_hi_page_; }
            set {
                if (value == is_on_saying_hi_page_)
                    return;
                is_on_saying_hi_page_ = value;
                on_property_changed();
                if (is_on_saying_hi_page) {
                    is_asking_questions = thank_you = false;
                    hi_msg = user_email != "" ? survey.hi_existing_user.Replace("%", friendly_user_name_impl()) : survey.hi_new_user;
                    is_existing_user = user_email != "";
                    can_go_to_share_page = is_existing_user;
                }
            }
        }

        public bool is_existing_user {
            get { return is_existing_user_; }
            set {
                if (value == is_existing_user_)
                    return;
                is_existing_user_ = value;
                on_property_changed();
            }
        }

        public string hi_msg {
            get { return hi_msg_; }
            set {
                if (value == hi_msg_)
                    return;
                hi_msg_ = value;
                on_property_changed();
            }
        }

        public string our_thank_you_prefix {
            get { return our_thank_you_prefix_; }
            set {
                if (value == our_thank_you_prefix_)
                    return;
                our_thank_you_prefix_ = value;
                on_property_changed();
            }
        }

        public string our_thank_you2 {
            get { return our_thank_you2_; }
            set {
                if (value == our_thank_you2_)
                    return;
                our_thank_you2_ = value;
                on_property_changed();
            }
        }

        public string our_thank_you_text {
            get { return our_thank_you_text_; }
            set {
                if (value == our_thank_you_text_)
                    return;
                our_thank_you_text_ = value;
                on_property_changed();
            }
        }

        public string our_thank_you_link {
            get { return our_thank_you_link_; }
            set {
                if (value == our_thank_you_link_)
                    return;
                our_thank_you_link_ = value;
                on_property_changed();
            }
        }

        public string our_thank_you_suffix {
            get { return our_thank_you_suffix_; }
            set {
                if (value == our_thank_you_suffix_)
                    return;
                our_thank_you_suffix_ = value;
                on_property_changed();
            }
        }

        public bool can_go_to_share_page {
            get { return can_go_to_share_page_; }
            set {
                if (value == can_go_to_share_page_)
                    return;
                can_go_to_share_page_ = value;
                on_property_changed();
            }
        }

        public bool is_on_share_page {
            get { return is_on_share_page_; }
            set {
                if (value == is_on_share_page_)
                    return;
                is_on_share_page_ = value;
                on_property_changed();
            }
        }

        public bool can_start {
            get { return can_start_; }
            set {
                if (value == can_start_)
                    return;
                can_start_ = value;
                on_property_changed();
            }
        }


        
               
        
        public string share_prefix {
            get { return share_prefix_; }
            set {
                if (value == share_prefix_)
                    return;
                share_prefix_ = value;
                on_property_changed();
            }
        }

        public string share2 {
            get { return share2_; }
            set {
                if (value == share2_)
                    return;
                share2_ = value;
                on_property_changed();
            }
        }

        public string share_text {
            get { return share_text_; }
            set {
                if (value == share_text_)
                    return;
                share_text_ = value;
                on_property_changed();
            }
        }

        public string share_link {
            get { return share_link_; }
            set {
                if (value == share_link_)
                    return;
                share_link_ = value;
                on_property_changed();
            }
        }

        public string share_suffix {
            get { return share_suffix_; }
            set {
                if (value == share_suffix_)
                    return;
                share_suffix_ = value;
                on_property_changed();
            }
        }
        
        public string do_not_share_offer {
            get { return do_not_share_offer_; }
            set {
                if (value == do_not_share_offer_)
                    return;
                do_not_share_offer_ = value;
                on_property_changed();
            }
        }

        public bool show_do_not_share_offer {
            get { return show_do_not_share_offer_; }
            set {
                if (value == show_do_not_share_offer_)
                    return;
                show_do_not_share_offer_ = value;
                on_property_changed();
            }
        }

        public string twitter_link {
            get { return twitter_link_; }
            set {
                if (value == twitter_link_)
                    return;
                twitter_link_ = value;
                on_property_changed();
            }
        }

        public string reddit_link {
            get { return reddit_link_; }
            set {
                if (value == reddit_link_)
                    return;
                reddit_link_ = value;
                on_property_changed();
            }
        }

        public string facebook_link {
            get { return facebook_link_; }
            set {
                if (value == facebook_link_)
                    return;
                facebook_link_ = value;
                on_property_changed();
            }
        }


        public survey_complete complete {
            get { return survey_complete_; }
        }

        private void update_friendly_user_name() {
            friendly_user_name = friendly_user_name_impl();
        }

        private string friendly_user_name_impl() {
            var name = user_name != "" ? user_name : user_email;
            var space = name.IndexOf(" ");
            var at = name.IndexOf("@");
            var idx = space >= 0 ? space : at;
            return idx >= 0 ? name.Substring(0, idx) : name;
        }

        private void on_survey_set() {
            survey_complete_ = new survey_complete { survey = survey_};
            //is_asking_questions = true;
            question_count = survey_.questions.Count;
            is_minimized = false;
            question_index = 0;
            survey_type_uri = survey_.survey_type_uri;
            twitter_link = survey.share_twitter_link;
            facebook_link = survey.share_facebook_link;
            reddit_link = survey.share_reddit_link;
            set_our_thank_you();
            set_share();
        }



        private void set_our_thank_you() {
            var for_you = survey.in_it_for_you;
            // [text](link)
            var link_idx = for_you.IndexOf("](");
            if (link_idx >= 0) {
                var before = for_you.IndexOf("[");
                var after = for_you.IndexOf(")");
                our_thank_you_prefix = for_you.Substring(0, before);
                our_thank_you_suffix = for_you.Substring(after + 1);
                our_thank_you_text = for_you.Substring(before + 1, link_idx - before - 1);
                our_thank_you_link = for_you.Substring(link_idx + 2, after - link_idx - 2);
            } else {
                our_thank_you_prefix = for_you;
                our_thank_you_suffix = "";
            }
            our_thank_you2 = survey.in_it_for_you2;
        }

        private void set_share() {
            var for_you = survey.share_it_offer;
            // [text](link)
            var link_idx = for_you.IndexOf("](");
            if (link_idx >= 0) {
                var before = for_you.IndexOf("[");
                var after = for_you.IndexOf(")");
                share_prefix = for_you.Substring(0, before);
                share_suffix = for_you.Substring(after + 1);
                share_text = for_you.Substring(before + 1, link_idx - before - 1);
                share_link = for_you.Substring(link_idx + 2, after - link_idx - 2);
            } else {
                share_prefix = for_you;
                share_suffix = "";
            }
            share2 = survey.share_it_offer2;
            do_not_share_offer = survey.do_not_share_it_offer;
        }

        private void on_question_change() {
            if (question_index_ >= survey.questions.Count)
                return; // can happen when user presses Complete
            var q = survey.questions[question_index_];
            var answer = survey_complete_.answers_[question_index_];
            question_title = q.title;
            question_msg = q.msg;
            update_extra_info();
            answer_extra = answer.extra;

            option_count = q.options.Count;
            if (option_count > 0) 
                option0 = q.options[0].msg;
            if (option_count > 1)
                option1 = q.options[1].msg;
            if (option_count > 2)
                option2 = q.options[2].msg;
            if (option_count > 3)
                option3 = q.options[3].msg;
            if (option_count > 4)
                option4 = q.options[4].msg;
            if (option_count > 5)
                option5 = q.options[5].msg;

            if (option_count > 0) 
                is_option0_checked = answer.option == 0 && answer.answered;
            if (option_count > 1) 
                is_option1_checked = answer.option == 1 && answer.answered;
            if (option_count > 2) 
                is_option2_checked = answer.option == 2 && answer.answered;
            if (option_count > 3) 
                is_option3_checked = answer.option == 3 && answer.answered;
            if (option_count > 4) 
                is_option4_checked = answer.option == 4 && answer.answered;
            if (option_count > 5) 
                is_option5_checked = answer.option == 5 && answer.answered;
        }

        private void update_extra_info() {
            var q = survey.questions[question_index_];
            var answer = survey_complete_.answers_[question_index_];
            if (!answer.answered)
                question_extra = q.extra_info;
            else
                question_extra = answer.option >= 0 & q.options[answer.option].extra_info != "" ? q.options[answer.option].extra_info : q.extra_info;            
        }

        public void on_option_click() {
            var answer = survey_complete_.answers_[question_index_];
            answer.answered = true;
            update_extra_info();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void on_property_changed([CallerMemberName] string property_name = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name));
        }
    }
}
