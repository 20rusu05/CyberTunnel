using UnityEngine;
using UnityEngine.Events;

public abstract class PuzzleBase : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField] private string puzzleName;
    [SerializeField] [TextArea(2, 4)] private string puzzleDescription;
    [SerializeField] private int maxAttempts = 5;

    [Header("Events")]
    public UnityEvent OnPuzzleSolved;
    public UnityEvent OnPuzzleFailed;
    public UnityEvent<string> OnFeedback;

    private int currentAttempts;
    private bool isSolved;

    public string PuzzleName => puzzleName;
    public string PuzzleDescription => puzzleDescription;
    public int RemainingAttempts => maxAttempts - currentAttempts;
    public bool IsSolved => isSolved;

    public virtual void InitializePuzzle()
    {
        currentAttempts = 0;
        isSolved = false;
    }

    /// <summary>
    /// If true, the answer was consumed (e.g. correct intermediate step) without resolving the puzzle.
    /// No attempt is consumed.
    /// </summary>
    protected virtual bool TryHandlePartialProgress(string answer) => false;

    public void SubmitAnswer(string answer)
    {
        if (isSolved) return;

        if (TryHandlePartialProgress(answer))
            return;

        currentAttempts++;

        if (CheckAnswer(answer))
        {
            isSolved = true;
            OnFeedback?.Invoke("ACCESS GRANTED");
            OnPuzzleSolved?.Invoke();
            AudioManager.Instance?.PlayPuzzleSolved();
        }
        else
        {
            string feedback = currentAttempts >= maxAttempts
                ? "ACCESS DENIED - Too many attempts"
                : $"INCORRECT - {RemainingAttempts} attempts remaining";

            OnFeedback?.Invoke(feedback);

            if (currentAttempts >= maxAttempts)
            {
                OnPuzzleFailed?.Invoke();
                AudioManager.Instance?.PlayPuzzleFailed();
            }
        }
    }

    protected abstract bool CheckAnswer(string answer);
    public abstract string GetPuzzleContent();
    public abstract string GetHint();

    public virtual string GetLearningInfo()
    {
        return "";
    }
}
