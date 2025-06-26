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

        // Sentiment prefix map
        private static readonly Dictionary<string, string> SentimentOpeners = new()
        {
            { "worried",    "I understand—it can feel worrying, but heres a usefull tip to take note of: " },
            { "frustrated", "I hear your frustration. heres a tip to help with your frustation:  " },
            { "curious",    "Great question—curiosity is key to security. so let me give you a helpfull tip: " }
        };

        // Top-level mapping of keyword sets to response-generators
        private static readonly List<(string[] Keywords, Func<ContextTracker, string> Handler)> Topics
            = new()
        {
            // Greetings & meta
            (new[]{ "hello","hi","hey" },
                ctx => "Hello! How can I help you with cybersecurity today?"),

            (new[]{ "how are you","how's it going" },
                ctx => "I'm doing great—ready to keep you safe online!"),

            (new[]{ "purpose","why are you here" },
                ctx => "I’m here to guide you through cybersecurity best practices."),

            (new[]{ "topics","ask","questions" },
                ctx => "Feel free to ask me about:\n" +
                       "- Password safety\n- Phishing & scams\n- Safe browsing\n" +
                       "- Malware protection\n- Privacy & GDPR\n- Two-factor auth (2FA)\n" +
                       "- Encryption tips\n- Social engineering"),

            // Password safety (5 random tips)
            (new[]{ "password","passwords","credentials","login" },
                ctx => RandomPick(new[]{
                    "Use passphrases ≥ 14 characters—mix words, digits & symbols.",
                    "Never reuse passwords. A password manager can generate & store them.",
                    "Enable 2FA wherever possible to add an extra layer of security.",
                    "Rotate critical passwords after any breach notifications.",
                    "Avoid personal info in passwords; hackers can guess those easily."
                })),

            // Phishing & scams (5 random tips)
            (new[]{ "phishing","scam", "scams", "email","sms","spoof","spear" },
                ctx => RandomPick(new[]{
                    "Verify sender addresses; mismatched domains are red flags.",
                    "Hover over links—if they don’t match displayed text, don’t click.",
                    "Beware of urgent demands for info—legitimate orgs don’t pressure.",
                    "Use anti-phishing toolbars/extensions in your browser.",
                    "Report suspicious messages and delete them immediately."
                })),

            // Privacy & GDPR (4 random tips)
            (new[]{ "privacy","data","gdpr","personal" },
                ctx => {
                    var baseTips = new[]{
                        "Review app permissions—revoke any you don’t use.",
                        "Use encrypted messaging (Signal, WhatsApp with E2EE).",
                        "Limit personal info on social networks and public forums.",
                        "Enable disk-encryption on laptops and backups."
                    };
                    var tip = RandomPick(baseTips);
                    // Personalize if user interest matches
                    if (ctx.Memory.TryGetValue("interest", out var fav) && fav == "privacy")
                        tip = "Since you’re keen on privacy: " + tip;
                    return tip;
                }),

            // Safe browsing (3 random tips)
            (new[]{ "browsing","internet","online","wifi","https" },
                ctx => RandomPick(new[]{
                    "Always look for HTTPS and the lock icon before entering data.",
                    "Keep your browser & plugins up to date automatically.",
                    "Use a privacy-focused extension (uBlock Origin / Privacy Badger)."
                })),

            // Malware (4 random tips)
            (new[]{ "malware","virus","ransomware","trojan","spyware" },
                ctx => RandomPick(new[]{
                    "Install reputable antivirus and run full scans weekly.",
                    "Keep your OS & software patched to close vulnerabilities.",
                    "Avoid downloading from unverified sources or torrents.",
                    "Backup critical data offline in case of ransomware."
                })),

            // Two-factor auth (2FA) (3 random tips)
            (new[]{ "2fa","two-factor","mfa","authenticator" },
                ctx => RandomPick(new[]{
                    "Use an authenticator app (Authy, Google Authenticator) over SMS.",
                    "Register backup codes securely in case you lose devices.",
                    "Consider hardware tokens (YubiKey) for top-tier security."
                })),

            // Encryption (3 random tips)
            (new[]{ "encryption","encrypt","ssl","tls","vpn" },
                ctx => RandomPick(new[]{
                    "Use a reputable VPN on public Wi-Fi to encrypt traffic.",
                    "Enable full-disk encryption on mobile & desktop devices.",
                    "Always check for HTTPS/TLS on websites handling sensitive data."
                })),

            // Social engineering (3 random tips)
            (new[]{ "social","engineering","manipulation","pretext" },
                ctx => RandomPick(new[]{
                    "Verify identity over a known channel before sharing info.",
                    "Be skeptical of unsolicited help requests—confirm authenticity.",
                    "Train yourself on common tactics: impersonation, urgency, flattery."
                })),

            // Exit
            (new[]{ "exit","quit","bye" },
                ctx => "Goodbye! Stay safe out there.")
        };

        public static string GetResponse(string input, ContextTracker context)
        {
            // Sentiment detection
            foreach (var kv in SentimentOpeners)
            {
                if (Regex.IsMatch(input, $@"\b{kv.Key}\b", RegexOptions.IgnoreCase))
                    return kv.Value + GetCoreResponse(input, context);
            }
            return GetCoreResponse(input, context);
        }

        private static string GetCoreResponse(string input, ContextTracker context)
        {
            var clean = Regex.Replace(input.ToLower(), @"[^\w\s'-]", "");

            // Yes/No & “tell me more” handler
            if (context.AwaitingConfirmation)
            {
                if (Regex.IsMatch(clean, @"\b(yes|y|sure|tell me more|continue|more)\b"))
                {
                    var extra = GetFollowUp(context);
                    return extra + "\n\nWould you like even more depth? (yes/no)";
                }
                if (Regex.IsMatch(clean, @"\b(no|n|not now)\b"))
                {
                    context.AwaitingConfirmation = false;
                    context.CurrentTopic = null;
                    return "Understood—what else can I help you with?";
                }
            }

            // Memory capture
            // Interest: “im interested in X” / “my favorite topic is X”
            var m1 = Regex.Match(clean, @"\b(i am|i'm|im|my)\s+(interested in|favorite topic is)\s+([a-z-]+)\b");
            if (m1.Success)
            {
                var topic = m1.Groups[3].Value;
                context.Memory["interest"] = topic;
                return $"Got it—I’ll remember you’re interested in {topic}.";
            }
            // Familiarity: “i am a beginner” / “i know a bit” / “advanced”
            var m2 = Regex.Match(clean, @"\b(i am|i'm|im)\s+(a\s+)?(beginner|advanced|intermediate)\b");
            if (m2.Success)
            {
                var level = m2.Groups[3].Value;
                context.Memory["level"] = level;
                return $"Thanks—I'll tailor my tips for a {level} level.";
            }

            // Topic matching 
            foreach (var (keywords, handler) in Topics)
            {
                if (keywords.Any(kw => Regex.IsMatch(clean, $@"\b{Regex.Escape(kw)}\b")))
                {
                    context.CurrentTopic = keywords.First();
                    var reply = handler(context);

                    // Only core security topics prompt follow-up
                    var noPrompt = new[] { "hello", "exit", "how are you", "purpose", "topics" };
                    if (!noPrompt.Contains(context.CurrentTopic))
                    {
                        reply += "\n\nWould you like more details? (yes/no)";
                        context.AwaitingConfirmation = true;
                    }
                    else
                    {
                        context.AwaitingConfirmation = false;
                    }
                    return reply;
                }
            }

            //  Clarification for unknown inputs 
            return "I’m not sure I understand. Could you rephrase or pick one of my topics?";
        }

        #region Helpers

        private static string RandomPick(string[] options)
            => options[_random.Next(options.Length)];

        private static string GetFollowUp(ContextTracker ctx)
        {
            // Increment follow-up counter
            ctx.FollowUpCount++;

            // Predefined deep-dive tips per topic
            var deepDives = new Dictionary<string, string[][]>
            {
                ["password"] = new[]
                {
            new[]
            {
                "Use a password manager’s built-in generator to create truly random credentials.",
                "Enable biometric unlock on your manager so you can access passwords securely on mobile.",
                "Regularly audit your vault for weak or reused passwords and replace them."
            },
            new[]
            {
                "Configure auto-fill only on trusted sites to avoid man-in-the-browser attacks.",
                "Use a separate vault or profile for highly sensitive accounts (e.g., banking).",
                "Set up emergency access contacts in your manager to recover locks."
            }
        },
                ["phishing"] = new[]
                {
            new[]
            {
                "Inspect the email header: look for mismatched Received paths or forged SPF records.",
                "Use a sandbox environment or web preview to open suspicious attachments safely.",
                "Implement anti-phishing training simulations every quarter for your team."
            },
            new[]
            {
                "Check DMARC reports for your domain to see if attackers are spoofing you.",
                "Deploy advanced link-analysis tools that render destination pages in isolation.",
                "Use AI-powered filters to flag spear-phishing attempts based on writing style."
            }
        },
                ["privacy"] = new[]
                {
            new[]
            {
                "Review your mobile apps’ permission logs monthly to spot new over-privileged installs.",
                "Use a personal VPN when collecting data on research apps to mask your identity.",
                "Enable multi-profile browsing (e.g., Firefox Containers) to isolate tracking."
            },
            new[]
            {
                "Check your social accounts’ download archive for past shared images and posts to delete anything sensitive.",
                "Use a privacy audit tool (like Privacy Badger) to block third-party cookies automatically.",
                "Consider self-hosting open-source alternatives (e.g., Nextcloud) for your files."
            }
        },
                ["browsing"] = new[]
                {
            new[]
            {
                "Deploy a content-security policy (CSP) extension to block inline scripts and dangerous sources.",
                "Use DNS-over-HTTPS (DoH) or DNS-over-TLS (DoT) to prevent DNS snooping on public Wi-Fi.",
                "Enable strict site isolation in Chrome or Firefox to mitigate Spectre-style attacks."
            },
            new[]
            {
                "Configure your browser to delete cookies and cache on exit to limit persistent tracking.",
                "Leverage browser CLI flags (e.g., `--incognito --disable-extensions`) for a clean session.",
                "Use a hardened fork like LibreWolf if you need maximum privacy."
            }
        },
                ["malware"] = new[]
                {
            new[]
            {
                "Set up automated rollback snapshots so you can revert an infected system instantly.",
                "Use behavioral-analysis AV engines rather than signature-only scanners.",
                "Whitelist applications to run only approved executables in sensitive environments."
            },
            new[]
            {
                "Implement network segmentation to limit ransomware spread in case of breach.",
                "Use file-integrity monitoring (FIM) to detect unauthorized changes in real time.",
                "Deploy honeypots to detect lateral movement and new malware variants early."
            }
        },
                ["2fa"] = new[]
                {
            new[]
            {
                "Switch from SMS to TOTP apps to avoid SIM-swap attacks.",
                "Backup your authenticator secrets in an encrypted offline vault.",
                "Use hardware security keys (like YubiKey) for accounts that support WebAuthn."
            },
            new[]
            {
                "Enable push-based 2FA (e.g., Duo Push) for faster, phishing-resistant login.",
                "Periodically review your list of 2FA devices and revoke old ones.",
                "Combine device-based 2FA with biometric checks where supported."
            }
        },
                ["encryption"] = new[]
                {
            new[]
            {
                "Use disk-level encryption (BitLocker, FileVault) and require pre-boot PINs.",
                "Configure mail clients to use PGP or S/MIME for end-to-end email encryption.",
                "Deploy SSL/TLS certificates from a trusted CA and automate renewal via ACME."
            },
            new[]
            {
                "Set up an encrypted container (e.g., VeraCrypt) for highly sensitive archives.",
                "Use HTTPS-only browser extensions to enforce strong TLS on all sites.",
                "Implement database-level encryption for sensitive fields (e.g., user SSNs, API keys)."
            }
        },
                ["social"] = new[]
                {
            new[]
            {
                "Establish a verification protocol for any unsolicited requests for help or data.",
                "Use voice-print or video ID checks for high-risk transactions or access requests.",
                "Train staff with realistic role-play exercises covering pretexting, baiting, and tailgating."
            },
            new[]
            {
                "Maintain an up-to-date “who’s who” roster so employees can verify callers quickly.",
                "Rotate social-engineering drills every quarter and debrief with real examples.",
                "Deploy AI to flag anomalous internal messages that mimic executive style."
            }
        }
                // Additional deep-dive mappings for other topics
            };

            // If topic found, choose appropriate level of detail
            if (deepDives.TryGetValue(ctx.CurrentTopic, out var levels))
            {
                // Choose the appropriate level (0 or 1) based on follow-up count
                int idx = Math.Min(ctx.FollowUpCount - 1, levels.Length - 1);
                var tips = levels[idx];
                var tip = RandomPick(tips);
                return $"Here’s some deeper insight on {ctx.CurrentTopic}: {tip}";
            }

            // Generic fallback if no deep-dive mapping exists
            return ctx.FollowUpCount switch
            {
                1 => $"Here’s some deeper insight on {ctx.CurrentTopic}: {RandomPick(new[]{
                  "Deep security tip 1.", "Deep security tip 2.", "Deep security tip 3." })}",
                _ => "I’ve shared quite a bit—let me know if you’d like to explore something else."
            };
        }


        #endregion
    }
}
