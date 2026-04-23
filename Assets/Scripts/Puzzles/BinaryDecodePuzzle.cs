using UnityEngine;
using System.Text;

public class BinaryDecodePuzzle : PuzzleBase
{
    [Header("Binary Decode Settings")]
    [SerializeField] private string plainText = "CODE";

    [SerializeField] [TextArea(8, 16)] private string terminalRiddle =
        "On one wall, I speak in only two voices:\n" +
        "0 and 1, again and again.\n" +
        "But I am not numbers forever.\n" +
        "Look to the other wall —\n" +
        "First count me into numbers,\n" +
        "Then find my place in the grand alphabet table.\n" +
        "Follow the path from the wall!\n" +
        "Only then will my true message appear.";

    private string binaryText;

    public override void InitializePuzzle()
    {
        base.InitializePuzzle();
        binaryText = TextToBinary(plainText);
    }

    private void Start()
    {
        InitializePuzzle();
    }

    protected override bool CheckAnswer(string answer)
    {
        return answer.Trim().ToUpperInvariant() == plainText.ToUpperInvariant();
    }

    public override string GetPuzzleContent()
    {
        return terminalRiddle;
    }

    public override string GetHint()
    {
        return "The left wall shows eight bits per letter. Each group → one decimal → one ASCII letter. " +
               "Submit one word in ALL CAPS (four letters).";
    }

    public override string GetLearningInfo()
    {
        return "DOC // Binary to text: Bytes in logs and malware are often read as ASCII. " +
               "Each 8 bits is one character; analysts follow Binary → decimal → character to recover strings like paths or commands.";
    }

    public static string TextToBinary(string text)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < text.Length; i++)
        {
            if (i > 0) sb.Append(' ');
            sb.Append(System.Convert.ToString(text[i], 2).PadLeft(8, '0'));
        }

        return sb.ToString();
    }

    public static string BinaryToText(string binary)
    {
        string cleaned = binary.Replace(" ", "");
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < cleaned.Length; i += 8)
        {
            if (i + 8 > cleaned.Length) break;
            string byteStr = cleaned.Substring(i, 8);
            sb.Append((char)System.Convert.ToInt32(byteStr, 2));
        }

        return sb.ToString();
    }

    /// <summary>Same bytes as on the left wall (for tools / debugging).</summary>
    public string GetBinaryDisplayString() => binaryText;
}
