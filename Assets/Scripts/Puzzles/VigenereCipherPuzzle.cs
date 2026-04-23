using System.Text.RegularExpressions;
using UnityEngine;

public class VigenereCipherPuzzle : PuzzleBase
{
    [Header("Vigenere — Part 2 (decrypt)")]
    [SerializeField] private string plainText = "FIREWALL";
    [SerializeField] private string key = "CYBER";

    [Header("Vigenere — Part 1 (formula)")]
    [SerializeField] [TextArea(4, 10)] private string step1Question =
        "To find the plaintext (P),\n" +
        "From the hidden message (C) I subtract the key (K).\n" +
        "If I go below A, I wrap around again.\n" +
        "In the circle of 26 letters, I move backward.\n" +
        "What formula do I use?";

    private string encryptedText;
    private bool step1Complete;

    public override void InitializePuzzle()
    {
        base.InitializePuzzle();
        encryptedText = Encrypt(plainText, key);
        step1Complete = false;
    }

    private void Start()
    {
        InitializePuzzle();
    }

    protected override bool TryHandlePartialProgress(string answer)
    {
        if (step1Complete || string.IsNullOrWhiteSpace(answer))
            return false;

        if (!MatchesDecryptionFormula(answer))
            return false;

        step1Complete = true;
        OnFeedback?.Invoke("<color=#00FF00>PART 1 — CORRECT</color>\nEnter the decoded word for Part 2.");
        PuzzleUIManager.Instance?.RefreshPuzzleContentBody();
        return true;
    }

    protected override bool CheckAnswer(string answer)
    {
        if (!step1Complete)
            return false;

        return answer.Trim().ToUpperInvariant() == plainText.ToUpperInvariant();
    }

    public override string GetPuzzleContent()
    {
        if (!step1Complete)
            return step1Question;

        return "Using this formula, decode the message:\n\n" +
               $"Ciphertext:\n{encryptedText}\n\n" +
               $"Key:\n{key.ToUpperInvariant()}\n\n" +
               "Repeat the key to match the ciphertext length:\n\n" +
               "Ciphertext:\n" +
               FormatLettersSpaced(encryptedText) + "\n\n" +
               "Key:\n" +
               FormatKeyAligned(encryptedText, key) + "\n\n" +
               "Submit the plaintext (one word, no spaces).";
    }

    public override string GetHint()
    {
        if (!step1Complete)
            return "Write the relationship between P, C, and K using modular arithmetic on 26 letters. " +
                   "Subtraction wraps like a clock.";

        return "Subtract each key letter from the ciphertext letter (A=0). If negative, add 26. " +
               "The key repeats: C Y B E R C Y B for eight letters.";
    }

    public override string GetLearningInfo()
    {
        return "DOC // Vigenère Cipher: Each letter is shifted by a letter from a repeating keyword. " +
               "Decryption uses P = (C − K) mod 26 per position. Key length and secrecy matter for strength.";
    }

    private static string FormatLettersSpaced(string text)
    {
        string u = text.ToUpperInvariant();
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < u.Length; i++)
        {
            if (i > 0) sb.Append(' ');
            sb.Append(u[i]);
        }

        return sb.ToString();
    }

    private static string FormatKeyAligned(string cipherUpper, string keyword)
    {
        string c = cipherUpper.ToUpperInvariant();
        string k = keyword.ToUpperInvariant();
        var sb = new System.Text.StringBuilder();
        int ki = 0;
        for (int i = 0; i < c.Length; i++)
        {
            if (i > 0) sb.Append(' ');
            sb.Append(k[ki % k.Length]);
            if (char.IsLetter(c[i])) ki++;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Accepts answers equivalent to P = (C − K) mod 26 (spacing / minus / MOD flexible).
    /// </summary>
    private static bool MatchesDecryptionFormula(string raw)
    {
        string n = NormalizeFormulaInput(raw);
        return n == "P=(C-K)MOD26";
    }

    private static string NormalizeFormulaInput(string raw)
    {
        string s = raw.Trim().ToUpperInvariant();
        s = s.Replace(" ", "").Replace("\u2212", "-").Replace("\u2013", "-").Replace("\u2014", "-");
        s = Regex.Replace(s, @"MOD\s*\(?\s*26\s*\)?", "MOD26", RegexOptions.IgnoreCase);
        s = s.Replace("%26", "MOD26");
        return s;
    }

    public static string Encrypt(string text, string key)
    {
        string upperText = text.ToUpper();
        string upperKey = key.ToUpper();
        char[] result = new char[text.Length];
        int keyIndex = 0;

        for (int i = 0; i < upperText.Length; i++)
        {
            char c = upperText[i];

            if (char.IsLetter(c))
            {
                int shift = upperKey[keyIndex % upperKey.Length] - 'A';
                result[i] = (char)(((c - 'A' + shift) % 26) + 'A');
                keyIndex++;
            }
            else
            {
                result[i] = c;
            }
        }

        return new string(result);
    }

    public static string Decrypt(string text, string key)
    {
        string upperText = text.ToUpper();
        string upperKey = key.ToUpper();
        char[] result = new char[text.Length];
        int keyIndex = 0;

        for (int i = 0; i < upperText.Length; i++)
        {
            char c = upperText[i];

            if (char.IsLetter(c))
            {
                int shift = upperKey[keyIndex % upperKey.Length] - 'A';
                result[i] = (char)(((c - 'A' - shift + 26) % 26) + 'A');
                keyIndex++;
            }
            else
            {
                result[i] = c;
            }
        }

        return new string(result);
    }
}
