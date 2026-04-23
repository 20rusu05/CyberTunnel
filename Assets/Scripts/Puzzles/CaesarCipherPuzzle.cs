using UnityEngine;

public class CaesarCipherPuzzle : PuzzleBase
{
    [Header("Caesar Cipher Settings")]
    [SerializeField] private string plainText = "OPEN PORT 443";
    [SerializeField] private int shift = 3;

    private string encryptedText;

    public override void InitializePuzzle()
    {
        base.InitializePuzzle();
        encryptedText = Encrypt(plainText, shift);
    }

    private void Start()
    {
        InitializePuzzle();
    }

    protected override bool CheckAnswer(string answer)
    {
        return answer.Trim().ToUpper() == plainText.ToUpper();
    }

    public override string GetPuzzleContent()
    {
        return $"ENCRYPTED LOG:\n\n{encryptedText}\n\n" +
               "The room around you hides the key to decoding this message.\n" +
               "Look at how you got here.";
    }

    public override string GetHint()
    {
        return "You jumped across exactly 3 platforms to reach this terminal. " +
               "Just like you, the letters in the message jump forward by the same number of positions in the alphabet.";
    }

    public override string GetLearningInfo()
    {
        return "DOC // Caesar Cipher: An ancient substitution cipher attributed to Julius Caesar. " +
               "Each letter is shifted by a fixed amount. It is easy to break with frequency analysis, " +
               "but remains a classic intro to cryptography.";
    }

    public static string Encrypt(string text, int shift)
    {
        char[] result = new char[text.Length];

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (char.IsLetter(c))
            {
                char baseChar = char.IsUpper(c) ? 'A' : 'a';
                result[i] = (char)(((c - baseChar + shift) % 26) + baseChar);
            }
            else
            {
                result[i] = c;
            }
        }

        return new string(result);
    }

    public static string Decrypt(string text, int shift)
    {
        return Encrypt(text, 26 - (shift % 26));
    }
}
