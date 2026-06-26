using System;
using System.Collections.Generic;
using System.Media;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace CyberSecurityChatBotGUI
{
    public partial class CyberSecurityChatBot : Form
    {
        
        Dictionary<string, List<string>> keywordResponses = new Dictionary<string, List<string>>();
        Dictionary<string, string> userMemory = new Dictionary<string, string>();
        Random random = new Random();

       
        private List<string> activityLog = new List<string>();

        
        private bool quizActive = false;
        private int quizIndex = 0;
        private int quizScore = 0;

        private List<(string Question, string[] Options, int CorrectIndex, string Explanation)> quizQuestions
            = new List<(string, string[], int, string)>
        {
            ("What should you do if you receive an email asking for your password?",
             new[] { "A) Reply with your password", "B) Delete the email", "C) Report it as phishing", "D) Ignore it" },
             2, "Reporting phishing emails helps prevent scams."),

            ("True or False: Using the same password for all accounts is safe.",
             new[] { "A) True", "B) False" },
             1, "Using the same password everywhere is dangerous. A breach on one site compromises all your accounts."),

            ("Which of the following is a strong password?",
             new[] { "A) password123", "B) John1990", "C) !Gx7#mL9@", "D) qwerty" },
             2, "Strong passwords use a mix of uppercase, lowercase, numbers and symbols."),

            ("What does 2FA stand for?",
             new[] { "A) Two-Factor Authentication", "B) Two-File Access", "C) Total Firewall Activation", "D) None of the above" },
             0, "Two-Factor Authentication adds an extra layer of security beyond just a password."),

            ("True or False: Public Wi-Fi is always safe to use for banking.",
             new[] { "A) True", "B) False" },
             1, "Public Wi-Fi is unencrypted and can be intercepted by attackers."),

            ("What is phishing?",
             new[] { "A) A sport", "B) A trick to steal your info via fake emails/sites", "C) A type of malware", "D) Antivirus software" },
             1, "Phishing tricks users into revealing sensitive info through deceptive messages."),

            ("Which action helps protect your privacy on social media?",
             new[] { "A) Share your location always", "B) Accept all friend requests", "C) Review your privacy settings", "D) Use your full name everywhere" },
             2, "Reviewing privacy settings limits who can see your personal information."),

            ("True or False: Clicking unknown links in emails is safe.",
             new[] { "A) True", "B) False" },
             1, "Unknown links can lead to malware downloads or phishing sites."),

            ("What should you do if you suspect your account has been hacked?",
             new[] { "A) Do nothing", "B) Change your password immediately", "C) Delete the account", "D) Tell no one" },
             1, "Changing your password immediately limits the damage a hacker can do."),

            ("What is social engineering?",
             new[] { "A) Building social networks", "B) Manipulating people into giving up confidential info", "C) A programming language", "D) Antivirus software" },
             1, "Social engineering exploits human psychology rather than technical vulnerabilities."),

            ("True or False: Antivirus software eliminates all cyber threats.",
             new[] { "A) True", "B) False" },
             1, "Antivirus helps but is not foolproof — good habits are equally important."),

            ("Which of these is an example of a secure browsing habit?",
             new[] { "A) Ignoring HTTPS warnings", "B) Using a VPN on public Wi-Fi", "C) Downloading software from any site", "D) Sharing passwords with friends" },
             1, "A VPN encrypts your traffic on public networks, keeping your data safe.")
        };

        
        private string connectionString = "server=localhost;user=root;password=Kagofred2004;database=chatbotdb;";

        // ======================================================================
        public CyberSecurityChatBot()
        {
            InitializeComponent();
            SetupChatBot();
            InitialiseDatabase();
            PlayGreeting();
            ShowAsciiArt();
        }

        
        private void InitialiseDatabase()
        {
            try
            {
               
                string setupConnection = "server=localhost;user=root;password=Kagofred2004;";
                using (var conn = new MySqlConnection(setupConnection))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "CREATE DATABASE IF NOT EXISTS chatbotdb; USE chatbotdb;" +
                        "CREATE TABLE IF NOT EXISTS tasks (" +
                        "id INT AUTO_INCREMENT PRIMARY KEY," +
                        "title VARCHAR(255) NOT NULL," +
                        "description TEXT," +
                        "reminder VARCHAR(255)," +
                        "is_completed TINYINT(1) DEFAULT 0" +
                        ");", conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                txtChat.AppendText($"Bot: Database setup failed — {ex.Message}\n");
                txtChat.AppendText("Bot: Running in offline mode (tasks won't be saved).\n");
            }
        }

        
        private void SetupChatBot()
        {
            keywordResponses.Add("password", new List<string>
            {
                "Use strong passwords that include numbers and symbols.",
                "Avoid using personal details like your name or birthday.",
                "Change your passwords regularly to stay secure."
            });

            keywordResponses.Add("scam", new List<string>
            {
                "Be careful with suspicious emails or messages.",
                "Never share your OTP or banking details online.",
                "If something feels off, report it immediately."
            });

            keywordResponses.Add("privacy", new List<string>
            {
                "Check your social media privacy settings often.",
                "Don't overshare personal information online.",
                "Use two-factor authentication whenever possible."
            });

            keywordResponses.Add("phishing", new List<string>
            {
                "Look out for fake links and sender addresses.",
                "Don't click attachments from unknown sources.",
                "Report phishing attempts to your email provider."
            });
        }

        private void PlayGreeting()
        {
            txtChat.AppendText("Bot: Welcome to CyberSecurity ChatBot! Stay safe online.\n");
            try
            {
                string path = System.IO.Path.Combine(Application.StartupPath, "Assets", "greeting.wav");
                SoundPlayer player = new SoundPlayer(path);
                player.Play();
            }
            catch
            {
                txtChat.AppendText("Bot: (Greeting sound not found — continuing without it.)\n");
            }
        }

        private void ShowAsciiArt()
        {
            string art = @"
   ____      _               ____            ____          _   
  / ___|   _| |__   ___ _ __| __ )  ___ | |_| __ ) ___ | |_ 
 | |  | | | | '_ \ / _ \ '__|  _ \ / _ \| __|  _ \/ _ \| __|
 | |__| |_| | |_) |  __/ |  | |_) | (_) | |_| |_) | (_) | |_ 
  \____\__, |_.__/ \___|_|  |____/ \___/ \__|____/ \___/ \__|
       |___/                                                    
";
            txtChat.AppendText(art + "\n");
            txtChat.AppendText("Bot: Type 'help' to see what I can do.\n\n");
        }

        // ─── MAIN INPUT HANDLER ────────────────────────────────────────
        private void btnSend_Click(object sender, EventArgs e)
        {
            string userInput = txtUserInput.Text.Trim();
            txtUserInput.Clear();

            if (string.IsNullOrWhiteSpace(userInput)) return;

            txtChat.AppendText($"You: {userInput}\n");
            string lower = userInput.ToLower();

            // ── Quiz mode: intercept answer ──
            if (quizActive)
            {
                HandleQuizAnswer(lower);
                return;
            }

            // ── Help ──
            if (lower == "help")
            {
                ShowHelp();
                return;
            }

            // ── Activity log ──
            if (lower.Contains("show activity log") || lower.Contains("what have you done for me"))
            {
                ShowActivityLog();
                return;
            }

            // ── Task: Add ──
            if (lower.Contains("add task") || lower.Contains("create task") || lower.Contains("new task"))
            {
                HandleAddTask(userInput);
                return;
            }

            // ── Task: View ──
            if (lower.Contains("view tasks") || lower.Contains("show tasks") || lower.Contains("list tasks") || lower.Contains("my tasks"))
            {
                ShowTasks();
                return;
            }

            // ── Task: Complete ──
            if (lower.Contains("complete task") || lower.Contains("mark task") || lower.Contains("done task"))
            {
                HandleCompleteTask(userInput);
                return;
            }

            // ── Task: Delete ──
            if (lower.Contains("delete task") || lower.Contains("remove task"))
            {
                HandleDeleteTask(userInput);
                return;
            }

            // ── Reminder ──
            if (lower.Contains("remind me") || lower.Contains("set a reminder") || lower.Contains("set reminder"))
            {
                HandleReminder(userInput);
                return;
            }

            // ── Quiz ──
            if (lower.Contains("quiz") || lower.Contains("start quiz") || lower.Contains("test me"))
            {
                StartQuiz();
                return;
            }

            
            if (lower.Contains("my name is"))
            {
                string name = userInput.Substring(lower.IndexOf("my name is") + 10).Trim();
                userMemory["name"] = name;
                txtChat.AppendText($"Bot: Nice to meet you, {name}!\n");
                return;
            }

            
            if (lower.Contains("i'm interested in") || lower.Contains("i am interested in"))
            {
                string topic = lower.Contains("i'm interested in")
                    ? userInput.Substring(lower.IndexOf("i'm interested in") + 17).Trim()
                    : userInput.Substring(lower.IndexOf("i am interested in") + 18).Trim();
                userMemory["topic"] = topic.ToLower();
                txtChat.AppendText($"Bot: Got it! I'll remember you're interested in {topic}.\n");
                return;
            }

            
            if (lower.Contains("worried") || lower.Contains("scared") || lower.Contains("nervous"))
            {
                txtChat.AppendText("Bot: It's okay to feel that way. Here's a tip:\n");
                txtChat.AppendText("Bot: " + GetRandomResponse("scam") + "\n");
                return;
            }

            
            if (lower.Contains("tell me more") || lower.Contains("another tip"))
            {
                if (userMemory.ContainsKey("topic") && keywordResponses.ContainsKey(userMemory["topic"]))
                    txtChat.AppendText("Bot: " + GetRandomResponse(userMemory["topic"]) + "\n");
                else
                    txtChat.AppendText("Bot: What topic are you interested in? (e.g. password, phishing, privacy)\n");
                return;
            }

            
            foreach (var keyword in keywordResponses.Keys)
            {
                if (lower.Contains(keyword))
                {
                    txtChat.AppendText("Bot: " + GetRandomResponse(keyword) + "\n");
                    return;
                }
            }

            
            txtChat.AppendText("Bot: I didn't quite understand that. Type 'help' to see what I can do.\n");
        }

       
        private void ShowHelp()
        {
            txtChat.AppendText("Bot: Here's what I can help with:\n");
            txtChat.AppendText("  • Add task [title] - Add a cybersecurity task\n");
            txtChat.AppendText("  • View tasks - See all your tasks\n");
            txtChat.AppendText("  • Complete task [title] - Mark a task as done\n");
            txtChat.AppendText("  • Delete task [title] - Remove a task\n");
            txtChat.AppendText("  • Remind me to [task] in [X] days - Set a reminder\n");
            txtChat.AppendText("  • Start quiz - Test your cybersecurity knowledge\n");
            txtChat.AppendText("  • Show activity log - See recent bot actions\n");
            txtChat.AppendText("  • Ask about: password, phishing, scam, privacy\n\n");
        }

        // ─── TASK ASSISTANT ────────────────────────────────────────────
        private void HandleAddTask(string userInput)
        {
            // Extract everything after "add task", "create task", or "new task"
            string lower = userInput.ToLower();
            string title = "";

            if (lower.Contains("add task"))
                title = userInput.Substring(lower.IndexOf("add task") + 8).Trim();
            else if (lower.Contains("create task"))
                title = userInput.Substring(lower.IndexOf("create task") + 11).Trim();
            else if (lower.Contains("new task"))
                title = userInput.Substring(lower.IndexOf("new task") + 8).Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                txtChat.AppendText("Bot: What should the task be called? e.g. 'Add task Enable 2FA'\n");
                return;
            }

            string description = $"Cybersecurity task: {title}";
            string reminder = "No reminder set";

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "INSERT INTO tasks (title, description, reminder) VALUES (@t, @d, @r)", conn);
                    cmd.Parameters.AddWithValue("@t", title);
                    cmd.Parameters.AddWithValue("@d", description);
                    cmd.Parameters.AddWithValue("@r", reminder);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // Database not available — continue silently
            }

            string logEntry = $"[{DateTime.Now:HH:mm}] Task added: '{title}'";
            activityLog.Add(logEntry);

            txtChat.AppendText($"Bot: Task added: '{title}'. Would you like to set a reminder? (e.g. 'Remind me in 3 days')\n");
        }

        private void ShowTasks()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT title, description, reminder, is_completed FROM tasks", conn);
                    var reader = cmd.ExecuteReader();

                    bool hasTasks = false;
                    txtChat.AppendText("Bot: Your tasks:\n");

                    while (reader.Read())
                    {
                        hasTasks = true;
                        string status = reader.GetInt32(3) == 1 ? "✓ Done" : "Pending";
                        txtChat.AppendText($"  [{status}] {reader.GetString(0)} — {reader.GetString(2)}\n");
                    }

                    if (!hasTasks)
                        txtChat.AppendText("Bot: No tasks found. Add one with 'Add task [name]'.\n");
                }
            }
            catch
            {
                txtChat.AppendText("Bot: Couldn't load tasks — database may not be connected.\n");
            }
        }

        private void HandleCompleteTask(string userInput)
        {
            string lower = userInput.ToLower();
            string title = "";

            if (lower.Contains("complete task"))
                title = userInput.Substring(lower.IndexOf("complete task") + 13).Trim();
            else if (lower.Contains("mark task"))
                title = userInput.Substring(lower.IndexOf("mark task") + 9).Trim();
            else if (lower.Contains("done task"))
                title = userInput.Substring(lower.IndexOf("done task") + 9).Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                txtChat.AppendText("Bot: Which task did you complete? e.g. 'Complete task Enable 2FA'\n");
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "UPDATE tasks SET is_completed=1 WHERE title LIKE @t", conn);
                    cmd.Parameters.AddWithValue("@t", $"%{title}%");
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        activityLog.Add($"[{DateTime.Now:HH:mm}] Task completed: '{title}'");
                        txtChat.AppendText($"Bot: Great job! Task '{title}' marked as completed.\n");
                    }
                    else
                    {
                        txtChat.AppendText($"Bot: I couldn't find a task matching '{title}'. Try 'View tasks' to see your list.\n");
                    }
                }
            }
            catch
            {
                txtChat.AppendText("Bot: Couldn't update task — database may not be connected.\n");
            }
        }

        private void HandleDeleteTask(string userInput)
        {
            string lower = userInput.ToLower();
            string title = lower.Contains("delete task")
                ? userInput.Substring(lower.IndexOf("delete task") + 11).Trim()
                : userInput.Substring(lower.IndexOf("remove task") + 11).Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                txtChat.AppendText("Bot: Which task should I delete? e.g. 'Delete task Enable 2FA'\n");
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM tasks WHERE title LIKE @t", conn);
                    cmd.Parameters.AddWithValue("@t", $"%{title}%");
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        activityLog.Add($"[{DateTime.Now:HH:mm}] Task deleted: '{title}'");
                        txtChat.AppendText($"Bot: Task '{title}' has been deleted.\n");
                    }
                    else
                    {
                        txtChat.AppendText($"Bot: No task matching '{title}' was found.\n");
                    }
                }
            }
            catch
            {
                txtChat.AppendText("Bot: Couldn't delete task — database may not be connected.\n");
            }
        }

        private void HandleReminder(string userInput)
        {
            string lower = userInput.ToLower();

            // find a number of days mentioned
            int days = 0;
            string[] words = lower.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (int.TryParse(words[i], out days)) break;
            }

            // Extract what the reminder is for
            string topic = userInput;
            if (lower.Contains("remind me to"))
                topic = userInput.Substring(lower.IndexOf("remind me to") + 12).Trim();
            else if (lower.Contains("remind me"))
                topic = userInput.Substring(lower.IndexOf("remind me") + 9).Trim();

            // Remove time part if present
            if (lower.Contains(" in "))
                topic = topic.Substring(0, topic.ToLower().IndexOf(" in ")).Trim();

            string reminderDate = days > 0
                ? DateTime.Now.AddDays(days).ToString("dd MMM yyyy")
                : "a future date";

            // Update the latest pending task in DB with this reminder
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "UPDATE tasks SET reminder=@r WHERE id = (SELECT MAX(id) FROM tasks)", conn);
                    cmd.Parameters.AddWithValue("@r", $"Remind on {reminderDate}");
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }

            string logEntry = $"[{DateTime.Now:HH:mm}] Reminder set: '{topic}' on {reminderDate}";
            activityLog.Add(logEntry);

            txtChat.AppendText($"Bot: Got it! I'll remind you about '{topic}' on {reminderDate}.\n");
        }

        // ─── QUIZ ──────────────────────────────────────────────────────
        private void StartQuiz()
        {
            quizActive = true;
            quizIndex = 0;
            quizScore = 0;
            activityLog.Add($"[{DateTime.Now:HH:mm}] Quiz started.");
            txtChat.AppendText("Bot: Let's test your cybersecurity knowledge! Answer with A, B, C, or D (or True/False).\n\n");
            ShowQuizQuestion();
        }

        private void ShowQuizQuestion()
        {
            if (quizIndex >= quizQuestions.Count)
            {
                EndQuiz();
                return;
            }

            var q = quizQuestions[quizIndex];
            txtChat.AppendText($"Bot: Q{quizIndex + 1}: {q.Question}\n");
            foreach (var option in q.Options)
                txtChat.AppendText($"     {option}\n");
        }

        private void HandleQuizAnswer(string input)
        {
            var q = quizQuestions[quizIndex];

            // Map input to index
            int answerIndex = -1;
            if (input.StartsWith("a")) answerIndex = 0;
            else if (input.StartsWith("b")) answerIndex = 1;
            else if (input.StartsWith("c")) answerIndex = 2;
            else if (input.StartsWith("d")) answerIndex = 3;
            else if (input.Contains("true")) answerIndex = 0;
            else if (input.Contains("false")) answerIndex = 1;

            if (answerIndex == -1)
            {
                txtChat.AppendText("Bot: Please answer with A, B, C, D, True, or False.\n");
                return;
            }

            if (answerIndex == q.CorrectIndex)
            {
                quizScore++;
                txtChat.AppendText($"Bot:  Correct! {q.Explanation}\n\n");
            }
            else
            {
                txtChat.AppendText($"Bot:  Incorrect. {q.Explanation}\n\n");
            }

            quizIndex++;
            ShowQuizQuestion();
        }

        private void EndQuiz()
        {
            quizActive = false;
            int total = quizQuestions.Count;
            string feedback = quizScore >= total * 0.8
                ? "Great job! You're a cybersecurity pro! 🎉"
                : quizScore >= total * 0.5
                    ? "Not bad! Keep learning to stay safe online."
                    : "Keep learning! Cybersecurity is important. 💪";

            txtChat.AppendText($"Bot: Quiz complete! You scored {quizScore}/{total}.\n");
            txtChat.AppendText($"Bot: {feedback}\n\n");
            activityLog.Add($"[{DateTime.Now:HH:mm}] Quiz completed — Score: {quizScore}/{total}");
        }

        // ─── ACTIVITY LOG ──────────────────────────────────────────────
        private void ShowActivityLog()
        {
            if (activityLog.Count == 0)
            {
                txtChat.AppendText("Bot: No activity recorded yet.\n");
                return;
            }

            txtChat.AppendText("Bot: Here's a summary of recent actions:\n");

            // Show last 10 only
            int start = Math.Max(0, activityLog.Count - 10);
            for (int i = start; i < activityLog.Count; i++)
                txtChat.AppendText($"  {i - start + 1}. {activityLog[i]}\n");

            if (activityLog.Count > 10)
                txtChat.AppendText($"  (Showing last 10 of {activityLog.Count} actions. Ask 'show full log' for all.)\n");

            txtChat.AppendText("\n");
        }

        // ─── HELPERS ───────────────────────────────────────────────────
        private string GetRandomResponse(string keyword)
        {
            var responses = keywordResponses[keyword];
            return responses[random.Next(responses.Count)];
        }
    }
}