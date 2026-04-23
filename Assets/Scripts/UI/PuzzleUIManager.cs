using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PuzzleUIManager : MonoBehaviour
{
    public static PuzzleUIManager Instance { get; private set; }

    [Header("Main Panel")]
    [SerializeField] private GameObject puzzlePanel;
    [SerializeField] private TextMeshProUGUI puzzleTitleText;
    [SerializeField] private TextMeshProUGUI puzzleContentText;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Input")]
    [SerializeField] private TMP_InputField answerInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button hintButton;
    [SerializeField] private Button closeButton;

    [Header("Multiple Choice Panel")]
    [SerializeField] private GameObject multipleChoicePanel;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private TextMeshProUGUI[] choiceTexts;

    [Header("Definitions Panel")]
    [SerializeField] private GameObject definitionsPanel;
    [SerializeField] private TextMeshProUGUI definitionText;
    [SerializeField] private TMP_InputField definitionInput;
    [SerializeField] private Button definitionSubmitButton;
    [SerializeField] private TextMeshProUGUI definitionProgressText;

    [Header("Hint Panel")]
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private Button hintCloseButton;

    [Header("Fun Facts Panel")]
    [SerializeField] private GameObject funFactsPanel;
    [SerializeField] private TextMeshProUGUI funFactsText;
    [SerializeField] private Button funFactsContinueButton;

    [Header("Animation")]
    [SerializeField] private float typewriterSpeed = 0.03f;

    private PuzzleBase currentPuzzle;
    private PuzzleBase lastSolvedPuzzle;
    private Coroutine typewriterCoroutine;
    private readonly Dictionary<PuzzleBase, string> cachedTextAnswers = new();

    public bool IsPuzzleOpen =>
        (puzzlePanel != null && puzzlePanel.activeSelf) ||
        (funFactsPanel != null && funFactsPanel.activeSelf);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        puzzlePanel?.SetActive(false);
        multipleChoicePanel?.SetActive(false);
        definitionsPanel?.SetActive(false);
        hintPanel?.SetActive(false);
        funFactsPanel?.SetActive(false);
    }

    private void Start()
    {
        submitButton?.onClick.AddListener(OnSubmitClicked);
        hintButton?.onClick.AddListener(OnHintClicked);
        closeButton?.onClick.AddListener(ClosePuzzle);
        definitionSubmitButton?.onClick.AddListener(OnDefinitionSubmitClicked);
        hintCloseButton?.onClick.AddListener(HideHint);
        funFactsContinueButton?.onClick.AddListener(OnFunFactsContinue);

        if (answerInput != null)
            answerInput.onSubmit.AddListener(_ => OnSubmitClicked());
        if (definitionInput != null)
            definitionInput.onSubmit.AddListener(_ => OnDefinitionSubmitClicked());
    }

    public void ShowPuzzle(PuzzleBase puzzle)
    {
        currentPuzzle = puzzle;
        puzzle.OnFeedback.AddListener(ShowFeedback);

        puzzlePanel.SetActive(true);
        feedbackText.text = "";

        puzzleTitleText.text = "[ TERMINAL ACCESS ]";

        if (puzzle is HashMatchingPuzzle hashPuzzle)
        {
            ShowHashPuzzle(hashPuzzle);
        }
        else if (puzzle is DefinitionsPuzzle defPuzzle)
        {
            ShowDefinitionsPuzzle(defPuzzle);
        }
        else
        {
            ShowTextInputPuzzle(puzzle);
        }
    }

    private void ShowTextInputPuzzle(PuzzleBase puzzle)
    {
        multipleChoicePanel?.SetActive(false);
        definitionsPanel?.SetActive(false);
        answerInput.gameObject.SetActive(true);
        submitButton.gameObject.SetActive(true);
        HideHint();

        answerInput.text = cachedTextAnswers.TryGetValue(puzzle, out var lastAnswer)
            ? lastAnswer
            : "";
        answerInput.ActivateInputField();

        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);

        typewriterCoroutine = StartCoroutine(TypewriterEffect(
            puzzleContentText, puzzle.GetPuzzleContent()));
    }

    /// <summary>
    /// Updates the puzzle body text without typewriter (e.g. after a correct multi-step checkpoint).
    /// </summary>
    public void RefreshPuzzleContentBody()
    {
        if (currentPuzzle == null || puzzlePanel == null || !puzzlePanel.activeSelf) return;
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        puzzleContentText.text = currentPuzzle.GetPuzzleContent();
        answerInput.text = "";
        cachedTextAnswers.Remove(currentPuzzle);
        answerInput.ActivateInputField();
    }

    private void ShowHashPuzzle(HashMatchingPuzzle puzzle)
    {
        answerInput.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
        definitionsPanel?.SetActive(false);
        multipleChoicePanel?.SetActive(true);
        HideHint();

        puzzleContentText.text = puzzle.GetPuzzleContent();

        var options = puzzle.Options;
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < options.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceTexts[i].text = options[i];

                string option = options[i];
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(option));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void ShowDefinitionsPuzzle(DefinitionsPuzzle puzzle)
    {
        answerInput.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
        multipleChoicePanel?.SetActive(false);
        definitionsPanel?.SetActive(true);
        HideHint();

        UpdateDefinitionDisplay(puzzle);
    }

    private void UpdateDefinitionDisplay(DefinitionsPuzzle puzzle)
    {
        if (puzzle.IsComplete)
        {
            puzzle.FinishAndEvaluate();
            return;
        }

        var def = puzzle.CurrentDefinition;
        if (def == null) return;

        definitionText.text = def.description;
        definitionInput.text = "";
        definitionInput.ActivateInputField();
        definitionProgressText.text =
            $"Progress: {puzzle.CorrectAnswers}/{puzzle.RequiredCorrect} needed | " +
            $"Question {puzzle.TotalDefinitions - (puzzle.TotalDefinitions - puzzle.CorrectAnswers)}/{puzzle.TotalDefinitions}";
    }

    private void OnSubmitClicked()
    {
        if (currentPuzzle == null || string.IsNullOrEmpty(answerInput.text)) return;

        cachedTextAnswers[currentPuzzle] = answerInput.text;
        currentPuzzle.SubmitAnswer(answerInput.text);

        if (currentPuzzle.IsSolved)
        {
            OpenExitDoorForPuzzle(currentPuzzle);
            ShowFunFacts(currentPuzzle);
            return;
        }

        answerInput.ActivateInputField();
    }

    private void OnChoiceSelected(string choice)
    {
        if (currentPuzzle == null) return;

        currentPuzzle.SubmitAnswer(choice);

        if (currentPuzzle.IsSolved)
        {
            OpenExitDoorForPuzzle(currentPuzzle);
            ShowFunFacts(currentPuzzle);
        }
    }

    private void OnDefinitionSubmitClicked()
    {
        if (currentPuzzle is not DefinitionsPuzzle defPuzzle) return;
        if (string.IsNullOrEmpty(definitionInput.text)) return;

        bool correct = defPuzzle.CheckSingleAnswer(definitionInput.text);

        feedbackText.text = correct
            ? "<color=#00FF00>CORRECT</color>"
            : $"<color=#FF0000>INCORRECT</color> - Answer was: {defPuzzle.LastCorrectTerm}";

        if (defPuzzle.IsComplete)
        {
            defPuzzle.FinishAndEvaluate();
            if (defPuzzle.HasPassed)
            {
                OpenExitDoorForPuzzle(defPuzzle);
                ShowFunFacts(defPuzzle);
            }
        }
        else
        {
            StartCoroutine(NextDefinitionDelayed(defPuzzle, 1.5f));
        }
    }

    private IEnumerator NextDefinitionDelayed(DefinitionsPuzzle puzzle, float delay)
    {
        yield return new WaitForSeconds(delay);
        feedbackText.text = "";
        UpdateDefinitionDisplay(puzzle);
    }

    private void OnHintClicked()
    {
        if (currentPuzzle == null) return;

        bool shouldShow = hintPanel == null || !hintPanel.activeSelf;
        if (!shouldShow)
        {
            HideHint();
            return;
        }

        hintPanel.SetActive(true);
        hintText.text = currentPuzzle.GetHint();
    }

    private void HideHint()
    {
        hintPanel?.SetActive(false);
    }

    private void ShowFunFacts(PuzzleBase puzzle)
    {
        lastSolvedPuzzle = puzzle;
        string info = puzzle.GetLearningInfo();

        if (funFactsPanel == null || string.IsNullOrWhiteSpace(info))
        {
            StartCoroutine(ClosePuzzleDelayed(2f));
            return;
        }

        puzzlePanel.SetActive(false);
        funFactsPanel.SetActive(true);
        funFactsText.text = info;
    }

    private void OnFunFactsContinue()
    {
        funFactsPanel.SetActive(false);
        ClosePuzzle();
    }

    private void OpenExitDoorForPuzzle(PuzzleBase puzzle)
    {
        if (puzzle == null) return;
        var rooms = Object.FindObjectsByType<Room>(FindObjectsSortMode.None);
        foreach (var room in rooms)
        {
            if (room.Puzzle == puzzle)
            {
                room.OpenExitDoor();
                break;
            }
        }
    }

    private void ShowFeedback(string message)
    {
        feedbackText.text = message;

        if (message.Contains("GRANTED"))
            feedbackText.color = Color.green;
        else if (message.Contains("DENIED"))
            feedbackText.color = Color.red;
        else
            feedbackText.color = new Color(1f, 0.6f, 0f);
    }

    public void ClosePuzzle()
    {
        if (currentPuzzle != null && answerInput != null && answerInput.gameObject.activeSelf)
            cachedTextAnswers[currentPuzzle] = answerInput.text;

        if (currentPuzzle != null)
            currentPuzzle.OnFeedback.RemoveListener(ShowFeedback);

        currentPuzzle = null;
        lastSolvedPuzzle = null;
        puzzlePanel.SetActive(false);
        multipleChoicePanel?.SetActive(false);
        definitionsPanel?.SetActive(false);
        funFactsPanel?.SetActive(false);
        HideHint();

        var player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.CanMove = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private IEnumerator ClosePuzzleDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClosePuzzle();
    }

    private IEnumerator TypewriterEffect(TextMeshProUGUI textComponent, string fullText)
    {
        textComponent.text = "";

        foreach (char c in fullText)
        {
            textComponent.text += c;
            AudioManager.Instance?.PlayTyping();
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }
}
