using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public class HashMatchingPuzzle : PuzzleBase
{
    [Header("Hash Matching Settings")]
    [SerializeField] private string correctPassword = "password";
    [SerializeField] private List<string> decoyPasswords = new List<string>
    {
        "admin",
        "root",
        "123456"
    };

    private string targetHash;
    private List<string> allOptions;

    public List<string> Options => allOptions;

    public override void InitializePuzzle()
    {
        base.InitializePuzzle();
        targetHash = ComputeMD5(correctPassword);

        allOptions = new List<string>(decoyPasswords);
        if (!allOptions.Contains(correctPassword))
            allOptions.Add(correctPassword);

        ShuffleList(allOptions);
    }

    private void Start()
    {
        InitializePuzzle();
    }

    protected override bool CheckAnswer(string answer)
    {
        return answer.Trim().ToLower() == correctPassword.ToLower();
    }

    public override string GetPuzzleContent()
    {
        string optionsList = "";
        for (int i = 0; i < allOptions.Count; i++)
        {
            optionsList += $"\n  [{i + 1}] {allOptions[i]}";
        }

        return $"HASH CRACKING CHALLENGE\n\n" +
               $"Target MD5 Hash:\n{targetHash}\n\n" +
               $"Which password produces this hash?\n{optionsList}\n\n" +
               "Select the correct password.";
    }

    public override string GetHint()
    {
        return "MD5 hashes are one-way functions. In real life, attackers use " +
               "rainbow tables or dictionary attacks to find passwords from hashes. " +
               "Try each option and think about which common password matches.";
    }

    public override string GetLearningInfo()
    {
        return "DOC // MD5: A fast legacy hash (1991) now considered cryptographically broken for collision resistance. " +
               "Still appears in tooling and malware workflows, but should not be used for password storage.";
    }

    public static string ComputeMD5(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
                sb.Append(hashBytes[i].ToString("x2"));

            return sb.ToString();
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
