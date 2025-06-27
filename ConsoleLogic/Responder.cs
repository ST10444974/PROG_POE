using ChatBot_Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatBot_Final.ConsoleLogic
{
    public static class Responder
    {
        private static readonly Random _random = new();
        private const int MaxFollowUps = 3;

        // Enhanced sentiment handling with tone matching
        private static readonly Dictionary<string, (string Opener, string Tone)> SentimentMap = new()
        {
            { "worried",    ("I understand why you'd feel concerned about {topic}. ", "reassuring") },
            { "frustrated", ("Dealing with {topic} can be frustrating—let's break this down. ", "patient") },
            { "confused",   ("{topic} can be confusing at first. Here's a clear explanation: ", "simplifying") },
            { "curious",    ("Great question about {topic}! Let me share some insights: ", "enthusiastic") },
            { "urgent",     ("For urgent {topic} issues, here's what you need to know immediately: ", "direct") }
        };

        // Comprehensive topic knowledge base
        private static readonly Dictionary<string, TopicProfile> TopicDatabase = new()
        {
            ["password"] = new TopicProfile(
                keywords: new[] { "password", "credentials", "login", "passphrase", "vault", "manager" },
                baseTips: new[] {
                    "Use passphrases ≥ 14 characters—mix unrelated words with digits & symbols",
                    "Never reuse passwords across accounts - password managers solve this",
                    "Enable 2FA as an additional layer beyond passwords",
                    "Rotate critical passwords after breach notifications",
                    "Avoid personal info that hackers could easily guess"
                },
                deepDives: new[] {
                    new[] {
                        "Use password manager generators for truly random credentials",
                        "Enable biometric unlock for secure mobile access to your vault",
                        "Conduct quarterly password audits to identify weak/reused credentials"
                    },
                    new[] {
                        "Configure auto-fill only on trusted sites to prevent MITB attacks",
                        "Maintain separate vaults for sensitive vs regular accounts",
                        "Set up emergency access contacts for account recovery"
                    },
                    new[] {
                        "Implement passwordless authentication with FIDO2 security keys where possible",
                        "Use hardware-secured vaults for enterprise credential management",
                        "Deploy breach monitoring services that alert you of compromised credentials"
                    }
                },
                levels: new Dictionary<string, string[]>
                {
                    ["beginner"] = new[] {
                        "Start with 3 random words + number/symbol (e.g., Apple$Sky42Turtle)",
                        "Browser-based password managers are great for beginners"
                    },
                    ["intermediate"] = new[] {
                        "Use diceware method for true random passphrases",
                        "Consider open-source managers like Bitwarden for transparency"
                    },
                    ["advanced"] = new[] {
                        "Implement hardware security keys for critical accounts",
                        "Set up enterprise-grade solutions like 1Password for Teams"
                    }
                }
            ),

            ["phishing"] = new TopicProfile(
                keywords: new[] { "phishing", "scam", "email", "sms", "spoof", "spear", "fraud" },
                baseTips: new[] {
                    "Verify sender addresses - mismatched domains are red flags",
                    "Hover over links before clicking to see actual destinations",
                    "Legitimate organizations never pressure for immediate action",
                    "Use anti-phishing extensions like Cloudphish or Avira",
                    "Report suspicious messages to your security team immediately"
                },
                deepDives: new[] {
                    new[] {
                        "Inspect email headers for mismatched paths or forged SPF records",
                        "Use sandbox environments to safely analyze suspicious attachments",
                        "Conduct quarterly phishing simulations for your team"
                    },
                    new[] {
                        "Monitor DMARC reports to detect spoofing attempts on your domain",
                        "Implement link-analysis tools that render pages in isolation",
                        "Deploy AI-powered filters that detect spear-phishing patterns"
                    },
                    new[] {
                        "Set up canary tokens to detect credential harvesting attempts",
                        "Implement DMARC policy with quarantine/reject settings",
                        "Use email authentication protocols like BIMI for brand verification"
                    }
                },
                levels: new Dictionary<string, string[]>
                {
                    ["beginner"] = new[] {
                        "Look for spelling mistakes and poor grammar as warning signs",
                        "When in doubt, contact the organization through official channels"
                    },
                    ["intermediate"] = new[] {
                        "Check for subtle domain spoofs (e.g., micros0ft.com with zero)",
                        "Verify unexpected attachments through separate communication channels"
                    },
                    ["advanced"] = new[] {
                        "Implement S/MIME or PGP email encryption for sensitive communications",
                        "Deploy advanced threat protection with URL detonation capabilities"
                    }
                }
            ),

            ["privacy"] = new TopicProfile(
                keywords: new[] { "privacy", "gdpr", "data", "personal", "pii", "compliance" },
                baseTips: new[] {
                    "Review app permissions monthly—revoke unused access",
                    "Use encrypted messaging apps like Signal for sensitive conversations",
                    "Limit personal info shared on social media platforms",
                    "Enable full-disk encryption on all devices",
                    "Use VPNs on public Wi-Fi to encrypt your traffic"
                },
                deepDives: new[] {
                    new[] {
                        "Configure privacy settings using tools like Privacy Badger or DuckDuckGo",
                        "Use browser containers to isolate tracking between sites",
                        "Review privacy policies before sharing data with new services"
                    },
                    new[] {
                        "Implement data minimization principles in your workflows",
                        "Conduct periodic data audits to identify PII exposure risks",
                        "Use pseudonymization techniques for user data storage"
                    },
                    new[] {
                        "Deploy data loss prevention (DLP) solutions for sensitive data",
                        "Implement GDPR-compliant consent management systems",
                        "Use differential privacy techniques for analytics data"
                    }
                },
                levels: new Dictionary<string, string[]>
                {
                    ["beginner"] = new[] {
                        "Enable 'Do Not Track' in your browser settings",
                        "Regularly clear cookies and browsing history"
                    },
                    ["advanced"] = new[] {
                        "Implement homomorphic encryption for processing sensitive data",
                        "Deploy privacy-enhancing computation techniques"
                    }
                }
            ),

            ["malware"] = new TopicProfile(
                keywords: new[] { "malware", "virus", "ransomware", "trojan", "spyware", "infection" },
                baseTips: new[] {
                    "Install reputable antivirus software and keep it updated",
                    "Regularly patch operating systems and applications",
                    "Avoid downloading software from untrusted sources",
                    "Backup critical data using the 3-2-1 rule (3 copies, 2 media, 1 offsite)",
                    "Use application whitelisting to block unauthorized software"
                },
                deepDives: new[] {
                    new[] {
                        "Enable behavior-based detection in your antivirus settings",
                        "Use system restore points for quick recovery from infections",
                        "Implement email attachment scanning for malware detection"
                    },
                    new[] {
                        "Deploy endpoint detection and response (EDR) solutions",
                        "Use network segmentation to contain malware spread",
                        "Implement file integrity monitoring for critical systems"
                    },
                    new[] {
                        "Set up honeypots to detect and analyze new malware variants",
                        "Implement memory protection against code injection attacks",
                        "Use threat intelligence feeds to block known malicious indicators"
                    }
                }
            ),

            ["browsing"] = new TopicProfile(
                keywords: new[] { "browsing", "internet", "online", "https", "ssl", "vpn", "browser" },
                baseTips: new[] {
                    "Always look for HTTPS and padlock icon before entering sensitive data",
                    "Keep browsers and extensions updated to patch vulnerabilities",
                    "Use privacy-focused browsers like Brave or Firefox with strict settings",
                    "Avoid public Wi-Fi for sensitive activities without VPN protection",
                    "Regularly clear cookies and cached data"
                },
                deepDives: new[] {
                    new[] {
                        "Enable strict site isolation in your browser settings",
                        "Use DNS-over-HTTPS (DoH) to prevent DNS snooping",
                        "Install content security policy (CSP) monitoring extensions"
                    },
                    new[] {
                        "Configure browser security headers for enhanced protection",
                        "Implement certificate pinning for critical services",
                        "Use browser sandboxing features for risky activities"
                    },
                    new[] {
                        "Deploy enterprise browser security policies via group policy",
                        "Implement web application firewalls for enhanced protection",
                        "Use remote browser isolation for high-risk browsing"
                    }
                }
            )
        };

        // Core topic response templates
        private static readonly Dictionary<string, Func<ContextTracker, string>> TopicResponses = new()
        {
            ["hello"] = ctx => $"Hello{(ctx.UserName != null ? " " + ctx.UserName : "")}! How can I assist with cybersecurity today?",

            ["how"] = ctx => "I'm functioning well—ready to help you navigate security challenges!",

            ["purpose"] = ctx => "I'm designed to provide expert cybersecurity guidance tailored to your needs",

            ["topics"] = ctx => {
                var interests = ctx.Memory.TryGetValue("interest", out var i) ? $" Since you're interested in {i}," : "";
                return $"I specialize in:{interests}\n- Password security\n- Phishing defense\n- Secure browsing\n- Malware protection"
                    + "\n- Privacy management\n- 2FA implementation\n- Encryption\n- Social engineering";
            },

            ["exit"] = ctx => "Stay secure! Feel free to return anytime you have security questions."
        };

        public static string GetResponse(string input, ContextTracker context)
        {
            // Detect and handle sentiment first
            foreach (var (emotion, (opener, tone)) in SentimentMap)
            {
                if (Regex.IsMatch(input, $@"\b{emotion}\b", RegexOptions.IgnoreCase))
                {
                    var topic = DetectPrimaryTopic(input) ?? "this";
                    var sentimentResponse = opener.Replace("{topic}", topic);
                    context.CurrentTone = tone; // Track tone for follow-ups
                    return sentimentResponse + GetCoreResponse(input, context);
                }
            }

            return GetCoreResponse(input, context);
        }

        private static string GetCoreResponse(string input, ContextTracker context)
        {
            var cleanInput = SanitizeInput(input);

            // Handle conversation flow states
            if (context.AwaitingConfirmation)
            {
                return HandleConfirmation(cleanInput, context);
            }

            // Capture user context
            if (TryCaptureUserContext(cleanInput, context))
            {
                return BuildContextResponse(context);
            }

            // Identify primary topic
            var primaryTopic = DetectPrimaryTopic(cleanInput);
            if (!string.IsNullOrEmpty(primaryTopic))
            {
                return BuildTopicResponse(primaryTopic, context);
            }

            // Check for predefined topic responses
            foreach (var (topic, handler) in TopicResponses)
            {
                if (Regex.IsMatch(cleanInput, $@"\b{Regex.Escape(topic)}\b"))
                {
                    return handler(context);
                }
            }

            // Clarification for unknown inputs with contextual suggestions
            return GenerateClarificationResponse(context);
        }

        #region Core Processing Methods

        private static string SanitizeInput(string input)
        {
            return Regex.Replace(input.ToLower(), @"[^\w\s'-]", "");
        }

        private static string HandleConfirmation(string input, ContextTracker context)
        {
            if (Regex.IsMatch(input, @"\b(yes|y|sure|tell me more|continue|more)\b"))
            {
                if (context.FollowUpCount < MaxFollowUps)
                {
                    var followUp = GetFollowUp(context);
                    return followUp + $"\n\nWould you like deeper technical details? ({context.FollowUpCount + 1}/{MaxFollowUps})";
                }
                return "I've shared comprehensive insights. Would you like to explore another topic?";
            }

            if (Regex.IsMatch(input, @"\b(no|n|not now|that's enough)\b"))
            {
                context.ResetConversationState();
                return "Understood! What security topic would you like to discuss next?";
            }

            return "I wasn't clear on your response. Please answer yes/no or ask about another topic.";
        }

        private static bool TryCaptureUserContext(string input, ContextTracker context)
        {
            // Capture interest
            var interestMatch = Regex.Match(input,
                @"\b(i am|i'm|im|my)\s+(interested in|favorite topic is|focus on)\s+([a-z-]+)\b");
            if (interestMatch.Success)
            {
                var topic = interestMatch.Groups[3].Value;
                context.Memory["interest"] = topic;
                return true;
            }

            // Capture expertise level
            var levelMatch = Regex.Match(input,
                @"\b(i am|i'm|im)\s+(a\s+)?(beginner|novice|intermediate|advanced|expert)\b");
            if (levelMatch.Success)
            {
                var level = levelMatch.Groups[3].Value;
                context.Memory["level"] = level;
                return true;
            }

            return false;
        }

        private static string BuildContextResponse(ContextTracker context)
        {
            var response = new StringBuilder("Noted! ");

            if (context.Memory.TryGetValue("interest", out var interest))
                response.Append($"I'll prioritize {interest}-related content. ");

            if (context.Memory.TryGetValue("level", out var level))
                response.Append($"Adjusting for {level} expertise. ");

            response.Append("How can I assist with your security needs?");
            return response.ToString();
        }

        private static string DetectPrimaryTopic(string input)
        {
            // Calculate topic relevance scores
            var topicScores = new Dictionary<string, int>();
            foreach (var (topic, profile) in TopicDatabase)
            {
                foreach (var keyword in profile.Keywords)
                {
                    if (Regex.IsMatch(input, $@"\b{Regex.Escape(keyword)}\b"))
                    {
                        if (!topicScores.ContainsKey(topic)) topicScores[topic] = 0;
                        topicScores[topic]++;
                    }
                }
            }

            // Return highest scoring topic
            return topicScores.Any()
                ? topicScores.OrderByDescending(kv => kv.Value).First().Key
                : null;
        }

        private static string BuildTopicResponse(string topic, ContextTracker context)
        {
            context.CurrentTopic = topic;
            context.FollowUpCount = 0;

            // Get base response
            var response = new StringBuilder();

            // Add personalized introduction if available
            if (context.Memory.TryGetValue("interest", out var interest) && interest == topic)
            {
                response.Append($"Since you're interested in {topic}, ");
            }

            // Select appropriate tip based on user level
            string selectedTip;
            if (TopicDatabase.TryGetValue(topic, out var profile))
            {
                if (context.Memory.TryGetValue("level", out var level) &&
                    profile.Levels.TryGetValue(level, out var levelTips))
                {
                    selectedTip = levelTips[_random.Next(levelTips.Length)];
                }
                else
                {
                    selectedTip = profile.BaseTips[_random.Next(profile.BaseTips.Length)];
                }
            }
            else
            {
                selectedTip = "Here's important security information: " +
                    (TopicResponses.ContainsKey(topic)
                        ? TopicResponses[topic](context)
                        : "Let's discuss this important security topic");
            }

            response.Append(selectedTip);

            // Add follow-up prompt for security topics
            if (!new[] { "hello", "exit", "how", "purpose", "topics" }.Contains(topic))
            {
                response.Append("\n\nWould you like more detailed information? (yes/no)");
                context.AwaitingConfirmation = true;
            }

            return response.ToString();
        }

        private static string GenerateClarificationResponse(ContextTracker context)
        {
            var suggestions = new StringBuilder("I'm not quite sure what you're asking. ");

            // Suggest based on conversation history
            if (!string.IsNullOrEmpty(context.CurrentTopic))
            {
                suggestions.Append($"We were discussing {context.CurrentTopic}. ");
                suggestions.Append("Would you like to continue with that topic?");
                return suggestions.ToString();
            }

            // Suggest based on user interests
            if (context.Memory.TryGetValue("interest", out var interest))
            {
                suggestions.Append($"Since you're interested in {interest}, you might ask about: ");
                suggestions.Append(string.Join(", ", TopicDatabase[interest].Keywords.Take(3)));
                return suggestions.ToString();
            }

            // Generic suggestions
            suggestions.Append("Try asking about:\n- Password safety\n- Phishing detection\n- Privacy settings");
            suggestions.Append("\nOr say 'help' to see all available topics");
            return suggestions.ToString();
        }

        private static string GetFollowUp(ContextTracker context)
        {
            context.FollowUpCount++;

            if (TopicDatabase.TryGetValue(context.CurrentTopic, out var profile) &&
                context.FollowUpCount <= profile.DeepDives.Length)
            {
                var tips = profile.DeepDives[context.FollowUpCount - 1];
                var tip = tips[_random.Next(tips.Length)];

                // Apply tone from sentiment detection if available
                var tonePrefix = !string.IsNullOrEmpty(context.CurrentTone)
                    ? $"[Using {context.CurrentTone} approach] "
                    : "";

                return $"{tonePrefix}Deep dive #{context.FollowUpCount}: {tip}";
            }

            return context.FollowUpCount switch
            {
                1 => $"Advanced insight: {RandomPick(new[] {
                    "Consider threat modeling for your specific use case",
                    "Implement monitoring to detect related security events",
                    "Review industry frameworks like NIST or ISO 27001"
                })}",
                _ => "I've shared comprehensive insights. Let's explore another topic or task?"
            };
        }

        #endregion

        #region Helper Classes & Methods

        private class TopicProfile
        {
            public string[] Keywords { get; }
            public string[] BaseTips { get; }
            public string[][] DeepDives { get; }
            public Dictionary<string, string[]> Levels { get; }

            public TopicProfile(
                string[] keywords,
                string[] baseTips,
                string[][] deepDives,
                Dictionary<string, string[]> levels = null)
            {
                Keywords = keywords;
                BaseTips = baseTips;
                DeepDives = deepDives;
                Levels = levels ?? new Dictionary<string, string[]>();
            }
        }

        private static string RandomPick(string[] options) => options[_random.Next(options.Length)];

        #endregion
    }
}
