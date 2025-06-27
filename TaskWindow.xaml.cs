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
        // A static list to store all pending tasks shared across the application
        public static List<TaskItem> PendingTasks = new List<TaskItem>();

        // Constructor: initializes the task window and refreshes the task list on load
        public TaskWindow()
        {
            InitializeComponent();
            RefreshTaskList();
        }

        // Adds a new task to the list when the "Add Task" button is clicked
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a task title.");
                return;
            }

            // Create a new task item using input fields
            TaskItem task = new TaskItem
            {
                Title = txtTitle.Text,
                Description = txtDescription.Text,
                ReminderDate = dpReminder.SelectedDate,
                IsCompleted = false
            };

            // Add the task to the list and refresh the UI
            PendingTasks.Add(task);
            ClearInputs();
            RefreshTaskList();
        }

        // Marks the selected task as completed
        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (lstTasks.SelectedIndex >= 0)
            {
                PendingTasks[lstTasks.SelectedIndex].IsCompleted = true;
                RefreshTaskList();
            }
        }

        // Deletes the selected task from the list
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (lstTasks.SelectedIndex >= 0)
            {
                PendingTasks.RemoveAt(lstTasks.SelectedIndex);
                RefreshTaskList();
            }
        }

        // Updates the task list display in the UI
        private void RefreshTaskList()
        {
            lstTasks.Items.Clear();
            foreach (var task in PendingTasks)
            {
                lstTasks.Items.Add(task.ToString()); // Uses TaskItem's ToString method
            }
        }

        // Clears input fields after a task is added
        private void ClearInputs()
        {
            txtTitle.Clear();
            txtDescription.Clear();
            dpReminder.SelectedDate = null;
        }
    }
}
