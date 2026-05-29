using System;
using System.Collections.Generic;
using System.Media;
using System.Windows.Forms;

namespace CyberSecurityChatBotGUI
{
    public partial class CyberSecurityChatBot : Form
    {
        Dictionary<string, List<string>> keywordResponses = new Dictionary<string, List<string>>();
        Dictionary<string, string> userMemory = new Dictionary<string, string>();
        Random random = new Random();

        public CyberSecurityChatBot()
        {
            InitializeComponent();
            SetupChatBot();
            PlayGreeting();
            ShowAsciiArt();
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
                "Don’t overshare personal information online.",
                "Use two‑factor authentication whenever possible."
            });

            keywordResponses.Add("phishing", new List<string>
            {
                "Look out for fake links and sender addresses.",
                "Don’t click attachments from unknown sources.",
                "Report phishing attempts to your email provider."
            });
        }

        private void PlayGreeting()
        {
            txtChat.AppendText("Bot: Welcome to CyberSecurity ChatBot! Stay safe online.\n");

            try
            {
                string path = System.IO.Path.Combine(Application.StartupPath, "Assets", "greeting.wav");
                SoundPlayer player = new SoundPlayer(path);
                player.Play();
            }
            catch
            {
                txtChat.AppendText("Bot: Could not play greeting sound.\n");
            }
           

        }
        


        private void ShowAsciiArt()
        {
            string asciiArt = @"
   ____       _                ____ _           _   ____ _           _   
  / ___|  ___| |__   ___ _ __ / ___| |__   ___ | |_| __ ) |__   ___ | |_ 
  \___ \ / __| '_ \ / _ \ '__| |   | '_ \ / _ \| __|  _ \ '_ \ / _ \| __|
   ___) | (__| | | |  __/ |  | |___| | | | (_) | |_| |_) | | | | (_) | |_ 
  |____/ \___|_| |_|\___|_|   \____|_| |_|\___/ \__|____/|_| |_|\___/ \__|
";
            txtChat.AppendText(asciiArt + "\n");
        }

      
        private void btnSend_Click(object sender, EventArgs e)
        {
            string userInput = txtUserInput.Text.ToLower().Trim();
            txtUserInput.Clear();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                txtChat.AppendText("Bot: Please type something.\n");
                return;
            }

            if (userInput.Contains("my name is"))
            {
                string name = userInput.Replace("my name is", "").Trim();
                userMemory["name"] = name;
                txtChat.AppendText($"Bot: Nice to meet you, {name}!\n");
                return;
            }

            if (userInput.Contains("i'm interested in"))
            {
                string topic = userInput.Replace("i'm interested in", "").Trim();
                userMemory["topic"] = topic;
                txtChat.AppendText($"Bot: Got it! I’ll remember you’re interested in {topic}.\n");
                return;
            }

            if (userInput.Contains("worried"))
            {
                txtChat.AppendText("Bot: It’s okay to feel worried. Here’s a tip:\n");
                txtChat.AppendText("Bot: " + GetRandomResponse("scam") + "\n");
                return;
            }

            foreach (var keyword in keywordResponses.Keys)
            { 
                if (userInput.Contains(keyword))
                {
                    txtChat.AppendText("Bot: " + GetRandomResponse(keyword) + "\n");
                    return;
                }
            }
             
            if (userInput.Contains("tell me more") || userInput.Contains("another tip"))
            {
                if (userMemory.ContainsKey("topic") && keywordResponses.ContainsKey(userMemory["topic"]))
                {
                    txtChat.AppendText("Bot: " + GetRandomResponse(userMemory["topic"]) + "\n");
                }
                else
                {
                    txtChat.AppendText("Bot: Remind me what topic you’re interested in?\n");
                }
                return;
            }

            txtChat.AppendText("Bot: I’m not sure I understand. Try rephrasing that.\n");
        }



        private string GetRandomResponse(string keyword)
        {
            var responses = keywordResponses[keyword];
            int index = random.Next(responses.Count);
            return responses[index];
        }
    }
}
