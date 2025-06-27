# ChatBot_Final

# Cybersecurity Chatbot - WPF Application

A user-friendly WPF-based chatbot designed to educate users on cybersecurity topics while offering productivity tools such as task reminders and interactive quizzes. This is Part 3 of the Cybersecurity Awareness Chatbot POE project.

## 💡 Purpose

This application simulates a helpful cybersecurity assistant capable of:
- Engaging in basic natural language interactions
- Providing cybersecurity tips and responses
- Managing personal tasks and setting reminders
- Running an interactive cybersecurity awareness quiz
- Logging all key user interactions

## 🛠 Features

### 🧠 Chatbot Assistant
- Greets the user by name and engages in conversation.
- Recognizes cybersecurity-related questions.
- Offers guidance based on detected topic intent.

### ✅ Task Manager
- Add, complete, and delete personal cybersecurity-related tasks.
- Set reminders (via natural language input like “in 3 days”).

### 🔔 NLP Simulation
- Recognizes varied user input using basic natural language processing (e.g., “remind me to update password in 1 week”).

### 🧪 Cybersecurity Quiz
- 10 multiple-choice questions.
- Immediate feedback and final score display.
- Educational explanations for each answer.

### 📜 Activity Log
- Tracks recent actions including tasks added, reminders set, quiz attempts, and topic discussions.
- “Show more” command displays extended history.

## 🖼 User Interface

Built using **WPF (.NET 9)**:
- Clean, responsive layout with `MainWindow.xaml`
- Chat-style interaction panel (StackPanel-based)
- Separate windows for **Task** and **Quiz**

## 📁 Project Structure


## 🚀 How to Run

1. Clone or download the project.
2. Open the solution in **Visual Studio 2022+** with .NET 9 SDK.
3. Build the project.
4. Run the app — the chatbot will greet you and prompt for your name.
5. Start chatting, adding tasks, or launching the quiz!

## 🔍 Example Commands

```text
add task - update my firewall
remind me to check my email security in 2 days
start quiz
show activity
exit
