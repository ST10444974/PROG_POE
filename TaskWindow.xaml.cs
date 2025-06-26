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

    public partial class TaskWindow : Window
    {
        public static List<TaskItem> PendingTasks = new List<TaskItem>();

        public TaskWindow()
        {
            InitializeComponent();
            RefreshTaskList();
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a task title.");
                return;
            }

            TaskItem task = new TaskItem
            {
                Title = txtTitle.Text,
                Description = txtDescription.Text,
                ReminderDate = dpReminder.SelectedDate,
                IsCompleted = false
            };

            PendingTasks.Add(task);
            ClearInputs();
            RefreshTaskList();
        }

        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (lstTasks.SelectedIndex >= 0)
            {
                PendingTasks[lstTasks.SelectedIndex].IsCompleted = true;
                RefreshTaskList();
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (lstTasks.SelectedIndex >= 0)
            {
                PendingTasks.RemoveAt(lstTasks.SelectedIndex);
                RefreshTaskList();
            }
        }

        private void RefreshTaskList()
        {
            lstTasks.Items.Clear();
            foreach (var task in PendingTasks)
            {
                lstTasks.Items.Add(task.ToString());
            }
        }

        private void ClearInputs()
        {
            txtTitle.Clear();
            txtDescription.Clear();
            dpReminder.SelectedDate = null;
        }
    }
}