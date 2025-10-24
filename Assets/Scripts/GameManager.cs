using GameWise.ShapeMatching;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("References")]
    public GridManager gridManager;
    public TextMeshProUGUI levelText;

    [Header("UI Panels")]
    public GameObject startPanel;
    public GameObject NextLevelPanel;
    public GameObject gameOverPanel;
    public GameObject QuitPanel;
    [SerializeField] private Slider maxLevelSlider;
    [SerializeField] private TextMeshProUGUI maxLevelText;
    public GameObject SettingPanel;

    [Header("UI Buttons")]
    public GameObject HintButton;
    public GameObject MusicOnButton;
    public GameObject MusicOffButton;
    public GameObject SoundOnButton;
    public GameObject SoundOffButton;


    [Header("Level Settings")]
    public int maxLevel = 4;
    public float hintDisplayDuration = 2f;

    private int currentLevel = 1;
    private bool isPaused = false;
    public static bool IsGamePaused { get; private set; } = false;
    private bool isSettingOpen = false;
    private bool isSoundOff = false;
    private bool isMusicOff = false;


    private void Start()
    {
        UpdateLevelText();
        SoundManager.Instance.Background();
        if (maxLevelSlider != null)
        {
            maxLevelSlider.minValue = 1;
            maxLevelSlider.maxValue = 5;
            maxLevelSlider.wholeNumbers = true;
            maxLevelSlider.value = maxLevel; // initialize slider 
            maxLevelSlider.onValueChanged.AddListener(OnMaxLevelSliderChanged);
           
        }
    }
    private void OnMaxLevelSliderChanged(float value)
    {
        SoundManager.Instance.Tap();
        maxLevel = Mathf.RoundToInt(value);
        maxLevelText.text = "Max Level: " + maxLevel;

        UpdateLevelText();
        
    }

    public void CheckLevelComplete()
    {
        Cell[] cells = FindObjectsOfType<Cell>();
        bool allFilled = true;

        foreach (Cell cell in cells)
        {
            if (!cell.IsFilled)
            {
                allFilled = false;
                break;
            }
        }

        if (allFilled)
        {
            StartCoroutine(DelayLevelComplete(2f));
        }
    }


    private void ShowNextLevelPanel()
    {
        SoundManager.Instance.LevelWon();
        if (NextLevelPanel != null)
        {
            NextLevelPanel.SetActive(true);

          
            TextMeshProUGUI panelText = NextLevelPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (panelText != null)
            {
                panelText.text = "Level " + currentLevel + " Complete!";
            }
        }
    }

    public void OnNextLevelButton()
    {
        SoundManager.Instance.Tap();

        if (NextLevelPanel != null)
            NextLevelPanel.SetActive(false);

       
        ClearCurrentLevel();

        currentLevel++;

        if (currentLevel > maxLevel)
        {
            
            ShowGameOver();
           
            return;
        }

       
        UpdateLevelText();
        gridManager.IncreaseGridSize();
    }

    public void OnRetryButton()
    {
        SoundManager.Instance.Tap();

        if (NextLevelPanel != null)
            NextLevelPanel.SetActive(false);

        ClearCurrentLevel();

        gridManager.GenerateGrid();
        gridManager.StartShowNumbersSequence();
    }

    private void ClearCurrentLevel()
    {
      
        DraggableNumber[] draggables = FindObjectsOfType<DraggableNumber>();
        foreach (DraggableNumber d in draggables)
        {
            Destroy(d.gameObject);
        }

    
        if (gridManager.numberPoolManager != null)
        {
            foreach (Transform child in gridManager.numberPoolManager.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void ShowGameOver()
    {
        SoundManager.Instance.GameWon();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (HintButton != null)
            HintButton.SetActive(false);
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel + "/" + maxLevel;
        }
    }

    public void StartGame()
    {
        SoundManager.Instance.Tap();
        if (startPanel != null)
            startPanel.SetActive(false);

        if (HintButton != null)
            HintButton.SetActive(true);

        RestartGame();
    }

    public void RestartGame()
    {
        SoundManager.Instance.Tap();
        currentLevel = 1;
        gridManager.gridSize = 2;

     
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (NextLevelPanel != null)
            NextLevelPanel.SetActive(false);

        if (HintButton != null)
            HintButton.SetActive(true);

     
        Time.timeScale = 1f;
        isPaused = false;
        UpdateLevelText();

        
        gridManager.GenerateGrid();
        gridManager.StartShowNumbersSequence();
    }


    public void PauseResumeGame()
    {
        if (!isPaused)
        {
            Time.timeScale = 0f;
            isPaused = true;
            IsGamePaused = true;
           
        }
        else
        {
            Time.timeScale = 1f;
            isPaused = false;
            IsGamePaused = false;
            
        }
    }


    public void ShowHideQuitPanel()
    {
        SoundManager.Instance.Tap();
        if (QuitPanel != null)
        {
           
            QuitPanel.SetActive(!QuitPanel.activeSelf);
        }
    }

    public void QuitGame()
    {
        SoundManager.Instance.Tap();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnHintButtonClick()
    {
        SoundManager.Instance.Tap();
        StartCoroutine(ShowAllHints());
    }

    public void MusicControl()
    {
        SoundManager.Instance.Tap();

        if (isMusicOff)
        {
            SoundManager.Instance.BgSoundControl(false);
            isMusicOff = false;
            MusicOffButton.SetActive(false);
            MusicOnButton.SetActive(true);
        }
        else
        {
            SoundManager.Instance.BgSoundControl(true);
            isMusicOff = true;
            MusicOnButton.SetActive(false);
            MusicOffButton.SetActive(true);
        }
    }

    public void SoundControl()
    {
        SoundManager.Instance.Tap();
        if (!isSoundOff)
        {
            SoundManager.Instance.SfXControl(false);
            isSoundOff = true;
            SoundOnButton.SetActive(false);
            SoundOffButton.SetActive(true);
        }
        else
        {
            SoundManager.Instance.SfXControl(true);
            isSoundOff = false;
            SoundOnButton.SetActive(true);
            SoundOffButton.SetActive(false);
        }
    }

    private IEnumerator ShowAllHints()
    {
        Cell[] allCells = FindObjectsOfType<Cell>();
        List<Cell> unfilledCells = new List<Cell>();

        foreach (Cell cell in allCells)
        {
            if (!cell.IsFilled)
            {
                unfilledCells.Add(cell);
            }
        }

        if (unfilledCells.Count == 0)
            yield break;

        // Show all unfilled numbers
        foreach (Cell cell in unfilledCells)
        {
            cell.ShowNumber();
        }

        yield return new WaitForSeconds(hintDisplayDuration);

        // Hide all numbers
        foreach (Cell cell in unfilledCells)
        {
            cell.HideNumber();
        }
    }

    private IEnumerator DelayLevelComplete(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentLevel < maxLevel)
        {
            ShowNextLevelPanel();
        }
        else
        {
            ShowGameOver();
        }
    }

    public void SettingOpenClose()
    {
        SoundManager.Instance.Tap();
        if (!isSettingOpen)
        {
            SettingPanel.SetActive(true);
            isSettingOpen = true;
        }
        else
        {
            SettingPanel.SetActive(false);
            isSettingOpen = false;
        }
    }

}