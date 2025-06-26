using ChatBot_Final.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ChatBot_Final
{
    /// <summary>
    /// Interaction logic for QuizWindow.xaml
    /// </summary>
    public partial class QuizWindow : Window
    {
        private List<Question> questions = new List<Question>();
        private int currentQuestionIndex = 0;
        private int score = 0;
        private int selectedAnswer = -1;

        public QuizWindow()
        {
            InitializeComponent();
            LoadQuestions();
            DisplayQuestion();
        }

        private void LoadQuestions()
        {
            questions = new List<Question>
            {
                new Question { Text = "What should you do if you receive an email asking for your password?",
                    Options = new List<string>{ "Reply with your password", "Delete the email", "Report it as phishing", "Ignore it" },
                    CorrectOptionIndex = 2,
                    Explanation = "Reporting phishing emails helps protect others from scams." },

                new Question { Text = "True or False: Using the same password for every account is safe.",
                    Options = new List<string>{ "True", "False" },
                    CorrectOptionIndex = 1,
                    Explanation = "Each account should have a unique password to reduce risk." },

                // Add 8 more...
                new Question { Text = "What is two-factor authentication (2FA)?",
                    Options = new List<string>{ "An antivirus program", "A security method requiring two verification steps", "A password manager", "None of the above" },
                    CorrectOptionIndex = 1,
                    Explanation = "2FA adds an extra layer of security beyond just your password." },

                new Question { Text = "Which of the following is a strong password?",
                    Options = new List<string>{ "password123", "123456", "CorrectHorseBatteryStaple!", "qwerty" },
                    CorrectOptionIndex = 2,
                    Explanation = "A strong password uses mixed case, symbols, and is hard to guess." },

                new Question { Text = "What is phishing?",
                    Options = new List<string>{ "Fishing with a computer", "A method to catch viruses", "A scam to trick you into revealing info", "Using fake antivirus software" },
                    CorrectOptionIndex = 2,
                    Explanation = "Phishing tricks people into giving personal info." },

                new Question { Text = "True or False: Public Wi-Fi is always safe for online banking.",
                    Options = new List<string>{ "True", "False" },
                    CorrectOptionIndex = 1,
                    Explanation = "Public Wi-Fi can be insecure, avoid banking on it." },

                new Question { Text = "What should you look for in a secure website URL?",
                    Options = new List<string>{ "http://", "www.", "https://", "ftp://" },
                    CorrectOptionIndex = 2,
                    Explanation = "HTTPS means the site is encrypted and more secure." },

                new Question { Text = "Which one is an example of a social engineering attack?",
                    Options = new List<string>{ "Phishing email", "Firewall", "Malware", "Antivirus" },
                    CorrectOptionIndex = 0,
                    Explanation = "Phishing uses human manipulation – that’s social engineering." },

                new Question { Text = "True or False: Antivirus software makes your device invincible.",
                    Options = new List<string>{ "True", "False" },
                    CorrectOptionIndex = 1,
                    Explanation = "Antivirus helps but can't protect from every threat." },

                new Question { Text = "How often should you update your passwords?",
                    Options = new List<string>{ "Every few years", "Only when hacked", "Never", "Regularly every few months" },
                    CorrectOptionIndex = 3,
                    Explanation = "Frequent updates reduce the risk of stolen credentials." },
            };
        }

        private void DisplayQuestion()
        {
            optionsPanel.Children.Clear();
            txtFeedback.Text = "";
            btnNext.Visibility = Visibility.Collapsed;
            selectedAnswer = -1;

            var question = questions[currentQuestionIndex];
            txtQuestion.Text = $"Q{currentQuestionIndex + 1}: {question.Text}";

            for (int i = 0; i < question.Options.Count; i++)
            {
                RadioButton option = new RadioButton
                {
                    Content = question.Options[i],
                    Tag = i,
                    Margin = new Thickness(0, 5, 0, 5)
                };
                option.Checked += (s, e) => selectedAnswer = (int)((RadioButton)s).Tag;
                optionsPanel.Children.Add(option);
            }
        }

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAnswer == -1)
            {
                MessageBox.Show("Please select an answer first.");
                return;
            }

            var correctIndex = questions[currentQuestionIndex].CorrectOptionIndex;

            if (selectedAnswer == correctIndex)
            {
                score++;
                txtFeedback.Text = "✅ Correct! " + questions[currentQuestionIndex].Explanation;
            }
            else
            {
                txtFeedback.Text = $"❌ Incorrect. {questions[currentQuestionIndex].Explanation}";
            }

            btnNext.Visibility = Visibility.Visible;
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            currentQuestionIndex++;

            if (currentQuestionIndex < questions.Count)
            {
                DisplayQuestion();
            }
            else
            {
                MessageBox.Show($"Quiz complete! Your score: {score} / {questions.Count}", "Result");
                Close();
            }
        }
    }
}
