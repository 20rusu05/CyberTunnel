using UnityEngine;
using System.Collections.Generic;

public class DefinitionsPuzzle : PuzzleBase
{
    [System.Serializable]
    public class Definition
    {
        public string term;
        [TextArea(2, 4)]
        public string description;
    }

    [Header("Definitions Settings")]
    [SerializeField] private List<Definition> definitions = new List<Definition>
    {
        new Definition
        {
            term = "Firewall",
            description = "A ______ is a network security system that monitors and controls incoming and outgoing network traffic based on predefined security rules."
        },
        new Definition
        {
            term = "Encryption",
            description = "______ is the process of converting readable data into an unreadable format to protect it from unauthorized access."
        },
        new Definition
        {
            term = "Phishing",
            description = "______ is a type of cyber attack that uses fake emails or websites to trick users into revealing sensitive information."
        },
        new Definition
        {
            term = "Malware",
            description = "______ is software designed to damage, disrupt, or gain unauthorized access to computer systems."
        },
        new Definition
        {
            term = "Authentication",
            description = "______ is the process of verifying the identity of a user before granting access to a system."
        }
    };

    [SerializeField] private int requiredCorrect = 3;

    private int currentDefinitionIndex;
    private int correctAnswers;
    private int totalAnswered;

    public Definition CurrentDefinition =>
        currentDefinitionIndex < definitions.Count ? definitions[currentDefinitionIndex] : null;

    public int CorrectAnswers => correctAnswers;
    public int TotalDefinitions => definitions.Count;
    public int RequiredCorrect => requiredCorrect;

    public override void InitializePuzzle()
    {
        base.InitializePuzzle();
        currentDefinitionIndex = 0;
        correctAnswers = 0;
        totalAnswered = 0;
        ShuffleDefinitions();
    }

    private void Start()
    {
        InitializePuzzle();
    }

    protected override bool CheckAnswer(string answer)
    {
        if (currentDefinitionIndex >= definitions.Count) return false;

        bool isCorrect = answer.Trim().ToLower() ==
                         definitions[currentDefinitionIndex].term.ToLower();

        if (isCorrect)
            correctAnswers++;

        totalAnswered++;
        currentDefinitionIndex++;

        if (currentDefinitionIndex >= definitions.Count)
            return correctAnswers >= requiredCorrect;

        return false;
    }

    private string lastCorrectTerm;

    public string LastCorrectTerm => lastCorrectTerm;

    public bool CheckSingleAnswer(string answer)
    {
        if (currentDefinitionIndex >= definitions.Count) return false;

        lastCorrectTerm = definitions[currentDefinitionIndex].term;

        bool isCorrect = answer.Trim().ToLower() == lastCorrectTerm.ToLower();

        if (isCorrect)
            correctAnswers++;
        else
            GameManager.Instance?.ConsumeGlobalAttempt();

        totalAnswered++;
        currentDefinitionIndex++;

        return isCorrect;
    }

    public bool IsComplete => currentDefinitionIndex >= definitions.Count;
    public bool HasPassed => correctAnswers >= requiredCorrect;

    public void FinishAndEvaluate()
    {
        if (HasPassed)
        {
            OnFeedback?.Invoke($"ACCESS GRANTED - {correctAnswers}/{definitions.Count} correct");
            OnPuzzleSolved?.Invoke();
            AudioManager.Instance?.PlayPuzzleSolved();
        }
        else
        {
            OnFeedback?.Invoke($"ACCESS DENIED - Only {correctAnswers}/{requiredCorrect} correct");
            OnPuzzleFailed?.Invoke();
            AudioManager.Instance?.PlayPuzzleFailed();
        }
    }

    public override string GetPuzzleContent()
    {
        if (CurrentDefinition == null) return "All definitions answered.";

        return $"CYBERSECURITY DEFINITIONS\n" +
               $"Question {currentDefinitionIndex + 1}/{definitions.Count}\n\n" +
               $"{CurrentDefinition.description}\n\n" +
               $"Fill in the blank. ({correctAnswers} correct so far, need {requiredCorrect})";
    }

    public override string GetHint()
    {
        if (CurrentDefinition == null) return "";

        string term = CurrentDefinition.term;
        return $"The answer starts with '{term[0]}' and has {term.Length} letters.";
    }

    public override string GetLearningInfo()
    {
        return "DOC // Security Vocabulary: Precise terminology improves incident response quality, " +
               "threat communication, and policy implementation across technical and non-technical teams.";
    }

    private void ShuffleDefinitions()
    {
        for (int i = definitions.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (definitions[i], definitions[j]) = (definitions[j], definitions[i]);
        }
    }
}
