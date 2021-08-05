using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Trivia
{
    public static class Extensions
    {
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }

        public static T RandomElement<T>(this IList<T> list)
        {
            return list.RandomElementUsing<T>(new Random());
        }

        public static T RandomElementUsing<T>(this IList<T> list, Random rand)
        {
            int index = rand.Next(0, list.Count());
            T val = list.ElementAt(index);
            list.RemoveAt(index);
            return val;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IList<TriviaQuestion> questions;
        private DispatcherTimer dispatcherTimer;
        private TimeSpan _time;

        private TriviaQuestion CurrentQuestion;

        public MainWindow()
        {
            InitializeComponent();

            QuestionTextBlock.Visibility = Visibility.Hidden;
            AnswerTextBlock.Visibility = Visibility.Hidden;
            TimerTextBlock.Visibility = Visibility.Hidden;

            SkipQuestionButton.Visibility = Visibility.Hidden;
            RevealAnswerButton.Visibility = Visibility.Hidden;
            SubjectTextBlock.Visibility = Visibility.Hidden;

            _time = TimeSpan.FromSeconds(60);

            TimerTextBlock.Text = _time.ToString("c");

            dispatcherTimer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                TimerTextBlock.Text = _time.ToString("c");
                if (_time == TimeSpan.Zero) dispatcherTimer.Stop();
                _time = _time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
        }

        public IList<TriviaQuestion> ReadCSV(string fileName)
        {
            Console.WriteLine("Reading questions from file: " + fileName);
            // We change file extension here to make sure it's a .csv file.
            // TODO: Error checking.
            string[] lines = File.ReadAllLines(fileName);

            // lines.Select allows me to project each line as a Person. 
            // This will give me an IEnumerable<Person> back.
            return lines.SubArray(1, lines.Count() - 1).Select(line =>
            {
                string[] data = line.Split(';');
                return new TriviaQuestion(data[3], data[4], Convert.ToInt32(data[2]), data[1]);
            }).ToList<TriviaQuestion>();
        }

        private void LoadQuestionsButtonClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            OpenFileDialog dlg = new OpenFileDialog();

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                questions = ReadCSV(dlg.FileName);
            }

            Console.WriteLine("Read " + questions.Count() + " questions.");

            LoadQuestionsButton.Visibility = Visibility.Hidden;
            StartButton.Visibility = Visibility.Visible;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Grid.SetRow(MainCard, 0);
            MainCard.SetValue(Grid.RowProperty, 0);
            WelcomeCard.Visibility = Visibility.Hidden;
            QuestionTextBlock.Visibility = Visibility.Visible;
            AnswerTextBlock.Visibility = Visibility.Visible;
            TimerTextBlock.Visibility = Visibility.Visible;
            SkipQuestionButton.Visibility = Visibility.Visible;
            RevealAnswerButton.Visibility = Visibility.Visible;
            SubjectTextBlock.Visibility = Visibility.Visible;

            StartButton.Visibility = Visibility.Hidden;

            _time = TimeSpan.FromSeconds(60);

            TimerTextBlock.Text = _time.ToString("c");

            if (dispatcherTimer.IsEnabled)
                dispatcherTimer.Stop();

            dispatcherTimer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                TimerTextBlock.Text = _time.ToString("c");
                if (_time == TimeSpan.Zero) dispatcherTimer.Stop();
                _time = _time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);

            UpdateQuestion();
        }

        private void RevealAnswer_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            AnswerTextBlock.Text = "Answer: " + CurrentQuestion.Answer;
            TimerTextBlock.Text = "00:00:00";
        }

        private void SkipQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (dispatcherTimer.IsEnabled)
                dispatcherTimer.Stop();

            _time = TimeSpan.FromSeconds(60);

            TimerTextBlock.Text = _time.ToString("c");

            dispatcherTimer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                TimerTextBlock.Text = _time.ToString("c");
                if (_time == TimeSpan.Zero)
                {
                    dispatcherTimer.Stop();
                    TimerTextBlock.Foreground = Brushes.DarkRed;
                }
                _time = _time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
            
            UpdateQuestion();
        }
        
        private void UpdateQuestion()
        {
            CurrentQuestion = questions.RandomElement<TriviaQuestion>();
            SubjectTextBlock.Text = "Subject: " + CurrentQuestion.Subject;
            QuestionTextBlock.Text = "Question: " + CurrentQuestion.Question;
            AnswerTextBlock.Text = "HIDDEN";
        }
    }
}
