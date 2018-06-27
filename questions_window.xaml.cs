using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace feedback_ui
{
    /// <summary>
    /// Interaction logic for questions_window.xaml
    /// </summary>
    public partial class questions_window : Window
    {
        private DispatcherTimer go_next_timer_ = new DispatcherTimer();

        public Action<survey_complete> on_complete;

        public questions_window(survey s)
        {
            InitializeComponent();
            model().survey = s;

            var r = System.Windows.SystemParameters.WorkArea;
            const int PAD = 10;
            Left = PAD;
            Top = r.Bottom - Height - PAD;

            go_next_timer_.Tick += (sender, args) => {
                if (model().go_next_value < 1)
                    return;
                int STEP = 5;
                if (model().go_next_value + STEP > 100) {
                    model().go_next_value = 0;
                    go_next(null,null);
                }
                else 
                    model().go_next_value += STEP;
            };
            go_next_timer_.Interval = TimeSpan.FromMilliseconds(100);
            go_next_timer_.IsEnabled = true;

            model().PropertyChanged += model_On_property_changed;
        }

        private void model_On_property_changed(object o, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case "is_option0_checked":
                case "is_option1_checked":
                case "is_option2_checked":
                case "is_option3_checked":
                case "is_option4_checked":
                case "is_option5_checked":
                    break;
            }
        }

        public questions_view_model model() {
            return DataContext as questions_view_model;
        }

        private void on_clickme_leave(object sender,MouseEventArgs e)
        {

        }

        private void toggle_show(object sender,RoutedEventArgs e) {
            // don't do anything
        }

        private void set_minimized(bool is_minimized) {
            if (model().is_minimized == is_minimized)
                return;
            model().is_minimized = is_minimized;
            Height = model().is_minimized ? 70 : 370;
            Top += model().is_minimized ? 300 : -300;            
        }

        private void on_clickme_enter(object sender,MouseEventArgs e)
        {

        }

        private void go_first(object sender,RoutedEventArgs e) {
            model().question_index = 0;
            set_focus_after_command();
        }

        private void set_focus_after_command() {
            // just in case the user goes back to a specific question where he may want to add text
            if (model().complete.answers_[model().question_index].answered)
                extra.Focus();
            else
                // take away focus from text, if any
                next.Focus();            
        }

        private void go_prev(object sender,RoutedEventArgs e) {
            if (model().question_index > 0) 
                model().question_index -= 1;
            set_focus_after_command();
        }

        private void go_next(object sender,RoutedEventArgs e)
        {
            if ( model().question_index < model().question_count - 1)
                model().question_index += 1;
            else 
                go_complete(sender,e);
            set_focus_after_command();
        }

        private void go_last(object sender,RoutedEventArgs e) {
            model().question_index = model().question_count - 1;
            set_focus_after_command();
        }

        private void on_radio_click(object sender,RoutedEventArgs e) {
            model().on_option_click();

            var opt = model().complete.answers_[model().question_index].option;
            model().complete.answers_[model().question_index].answer_time = DateTime.Now;
            var is_last = model().survey.questions [model().question_index].options.Count - 1 == opt;
            if (is_last && model().survey.questions[model().question_index].options[opt].msg == model().survey.skip_msg) {
                // for Skip -> very short delay
                go_next_timer_.Interval = TimeSpan.FromMilliseconds(30);
                model().go_next_value = 1;
            } else if (model().survey.questions[model().question_index].go_next_secs != 0 ) {
                go_next_timer_.Interval = TimeSpan.FromSeconds(model().survey.questions[model().question_index].go_next_secs / 20.0);
                model().go_next_value = 1;
                // make it easy for the user to tell us more!
                extra.Focus();
            }
        }

        private void go_complete(object sender,RoutedEventArgs e) {
            model().thank_you = true;
            util.postpone(Close, 2000);
        }

        private void on_closed(object sender,EventArgs e) {
            go_next_timer_.IsEnabled = false;

            // if not thank you, user aborted
            if (model().thank_you) {
                model().complete.user_email = model().user_email;
                model().complete.user_name = model().user_name;
                on_complete?.Invoke(model().complete);
            }
        }

        private void on_mouse_down_thank_you_text(object sender,MouseButtonEventArgs e) {
            Process.Start(new ProcessStartInfo(model().our_thank_you_link));
            e.Handled = true;
        }

        private void go_to_share_page(object sender,RoutedEventArgs e) {
            model().is_on_saying_hi_page = false;
            model().is_on_share_page = true;
        }

        private void on_load(object sender,RoutedEventArgs e) {
            if (!model().can_go_to_share_page && !model().is_existing_user)
                email.Focus();
        }

        private void go_abort(object sender,RoutedEventArgs e) {
            if ( MessageBox.Show(this, "Are You Sure You want to Close the Test?", "Phot-Awe", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Close();
        }

        private void go_minimize(object sender,RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void on_mouse_leave(object sender,MouseEventArgs e) {
            if (!model().is_asking_questions)
                return;
            set_minimized(true);
        }

        private void on_mouse_enter(object sender,MouseEventArgs e)
        {
            if (!model().is_asking_questions)
                return;
            set_minimized(false);
        }

        private void go_start_test(object sender,RoutedEventArgs e) {
            model().is_on_share_page = false;
            model().is_asking_questions = true;

        }

        private void on_do_not_share_click(object sender,MouseButtonEventArgs e) {
            model().can_start = true;
        }

        private void share_facebook_click(object sender,RoutedEventArgs e)
        {
            model().can_start = true;
            model().complete.shared_on_fb = true;
            model().show_do_not_share_offer = false;
            Process.Start(new ProcessStartInfo(model().facebook_link));
        }

        private void share_twitter_click(object sender,RoutedEventArgs e)
        {
            model().can_start = true;
            model().complete.shared_on_twitter = true;
            model().show_do_not_share_offer = false;
            Process.Start(new ProcessStartInfo(model().twitter_link));

        }

        private void on_mouse_down_share_text(object sender,MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo(model().share_link));
            e.Handled = true;
        }
    }
}
