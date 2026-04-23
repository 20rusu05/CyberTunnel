using UnityEngine;

/// <summary>
/// Room 4 (index 3): Part 1 — IPv4 on left wall → answer is its binary. Part 2 — binary on right wall → answer is dotted IPv4.
/// </summary>
public class IPBinaryPuzzle : PuzzleBase
{
    [Header("Part 1 — IPv4 on left wall")]
    [SerializeField] private string part1Ip = "10.20.30.40";

    [Header("Part 2 — Binary on right wall (decoded to this IPv4)")]
    [SerializeField] private string part2Ip = "172.16.254.1";

    [SerializeField] [TextArea(4, 8)] private string part1Riddle =
        "On the left wall,\n" +
        "four numbers mark an address.\n" +
        "Break them apart,\n" +
        "turn each into 8 bits,\n" +
        "and the hidden code will be complete.";

    [SerializeField] [TextArea(4, 8)] private string part2Riddle =
        "On the right wall,\n" +
        "I am written in four groups of bits.\n" +
        "Read each group as a number,\n" +
        "place the dots between them,\n" +
        "and the address will show itself.";

    private string part1BinaryKey;
    private bool part1Complete;

    public override void InitializePuzzle()
    {
        base.InitializePuzzle();
        part1BinaryKey = NormalizeBinaryDigits(FormatIpAsBinaryDotted(part1Ip));
        part1Complete = false;
    }

    private void Start()
    {
        InitializePuzzle();
    }

    public string GetPart1WallLabel() => $"PART 1: {part1Ip}";

    public string GetPart2WallLabel() => $"PART 2: {FormatIpAsBinaryDotted(part2Ip)}";

    protected override bool TryHandlePartialProgress(string answer)
    {
        if (part1Complete || string.IsNullOrWhiteSpace(answer))
            return false;

        if (NormalizeBinaryDigits(answer) != part1BinaryKey)
            return false;

        part1Complete = true;
        OnFeedback?.Invoke("<color=#00FF00>PART 1 — CORRECT</color>\nDecode the right wall for Part 2.");
        PuzzleUIManager.Instance?.RefreshPuzzleContentBody();
        return true;
    }

    protected override bool CheckAnswer(string answer)
    {
        if (!part1Complete)
            return false;

        return NormalizeIp(answer) == NormalizeIp(part2Ip);
    }

    public override string GetPuzzleContent()
    {
        return part1Complete ? part2Riddle : part1Riddle;
    }

    public override string GetHint()
    {
        if (!part1Complete)
            return "Each of the four numbers is 0–255. Write eight 0/1 digits per number, in order (32 bits total). " +
                   "Dots between octets are optional.";

        return "Split on dots into four blocks of 8 bits. Each block is one decimal 0–255. Build x.x.x.x";
    }

    public override string GetLearningInfo()
    {
        return "DOC // IPv4 and binary: An address is 32 bits in four octets. Converting between dotted decimal and binary " +
               "is routine when reading ACLs, routing tables, and packet captures.";
    }

    /// <summary>Binary octets with dots, e.g. 11000000.10101000...</summary>
    public static string FormatIpAsBinaryDotted(string ip)
    {
        string[] octets = ip.Split('.');
        if (octets.Length != 4) return "";

        return $"{ToBin(octets[0])}.{ToBin(octets[1])}.{ToBin(octets[2])}.{ToBin(octets[3])}";
    }

    private static string ToBin(string octetText)
    {
        if (!int.TryParse(octetText, out int octet)) return "00000000";
        octet = Mathf.Clamp(octet, 0, 255);
        return System.Convert.ToString(octet, 2).PadLeft(8, '0');
    }

    private static string NormalizeBinaryDigits(string input)
    {
        var s = input.Trim().Replace(" ", "").Replace(".", "");
        foreach (char c in s)
        {
            if (c != '0' && c != '1')
                return "";
        }

        return s;
    }

    private static string NormalizeIp(string input)
    {
        return input.Trim().Replace(" ", "");
    }
}
