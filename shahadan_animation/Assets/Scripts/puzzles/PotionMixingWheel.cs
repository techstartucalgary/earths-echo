using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class PotionMixingWheel : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int startSequenceLength = 2;
    [SerializeField] private int maxSequenceLength = 5;
    [SerializeField] private float displayInterval = 1f;
    [SerializeField] private float rotationDuration = 0.5f;

    [Header("UI References")]
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject wheelObject;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI sequenceText;
    [SerializeField] private Button rotateLeftButton;
    [SerializeField] private Button rotateRightButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject completionPanel;
    [SerializeField] private TextMeshProUGUI completionText;

    [Header("Color Settings")]
    [SerializeField]
    private Color[] potionColors = new Color[4] {
        Color.red, Color.blue, Color.green, Color.yellow
    };
    [SerializeField]
    private string[] colorNames = new string[4] {
        "Red", "Blue", "Green", "Yellow"
    };

    private List<int> targetSequence = new List<int>();
    private List<int> playerSequence = new List<int>();
    private int currentLevel = 1;
    private int currentWheelPosition = 0; // 0: Red, 1: Blue, 2: Green, 3: Yellow
    private bool isShowingSequence = false;
    private bool isPlayerTurn = false;
    private int playbackIndex = 0;

    private void CreateUIElements()
    {
        // Create the main canvas
        gameCanvas = new GameObject("Potion Mixing Canvas");
        Canvas canvas = gameCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10; // Ensure it's on top of other UI

        CanvasScaler scaler = gameCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        gameCanvas.AddComponent<GraphicRaycaster>();

        // Create a dark panel as background
        GameObject backgroundPanel = new GameObject("Background Panel");
        backgroundPanel.transform.SetParent(gameCanvas.transform, false);
        Image bgImage = backgroundPanel.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.8f);
        RectTransform bgRect = backgroundPanel.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Create the wheel container
        GameObject wheelContainer = new GameObject("Wheel Container");
        wheelContainer.transform.SetParent(gameCanvas.transform, false);
        RectTransform wheelContainerRect = wheelContainer.AddComponent<RectTransform>();
        wheelContainerRect.anchorMin = new Vector2(0.5f, 0.5f);
        wheelContainerRect.anchorMax = new Vector2(0.5f, 0.5f);
        wheelContainerRect.sizeDelta = new Vector2(300, 300);

        // Create the wheel
        wheelObject = new GameObject("Wheel");
        wheelObject.transform.SetParent(wheelContainer.transform, false);
        Image wheelImage = wheelObject.AddComponent<Image>();
        wheelImage.color = Color.white;
        RectTransform wheelRect = wheelObject.GetComponent<RectTransform>();
        wheelRect.anchorMin = new Vector2(0.5f, 0.5f);
        wheelRect.anchorMax = new Vector2(0.5f, 0.5f);
        wheelRect.sizeDelta = new Vector2(200, 200);

        // Create color segments
        for (int i = 0; i < 4; i++)
        {
            GameObject segment = new GameObject($"Segment {i}");
            segment.transform.SetParent(wheelObject.transform, false);
            Image segmentImage = segment.AddComponent<Image>();
            segmentImage.color = potionColors[i];

            RectTransform segmentRect = segment.GetComponent<RectTransform>();
            segmentRect.anchorMin = Vector2.zero;
            segmentRect.anchorMax = Vector2.one;

            // Position each segment in a quadrant
            // Note: Rearranged so position 0 (Red) is at BOTTOM
            switch (i)
            {
                case 0: // Bottom (RED)
                    segmentRect.offsetMin = new Vector2(25, 0);
                    segmentRect.offsetMax = new Vector2(-25, -100);
                    break;
                case 1: // Left (BLUE)
                    segmentRect.offsetMin = new Vector2(0, 25);
                    segmentRect.offsetMax = new Vector2(-100, -25);
                    break;
                case 2: // Top (GREEN)
                    segmentRect.offsetMin = new Vector2(25, 100);
                    segmentRect.offsetMax = new Vector2(-25, 0);
                    break;
                case 3: // Right (YELLOW)
                    segmentRect.offsetMin = new Vector2(100, 25);
                    segmentRect.offsetMax = new Vector2(0, -25);
                    break;
            }
        }

        // Create center circle
        GameObject centerCircle = new GameObject("Center Circle");
        centerCircle.transform.SetParent(wheelObject.transform, false);
        Image centerImage = centerCircle.AddComponent<Image>();
        centerImage.color = Color.white;

        RectTransform centerRect = centerCircle.GetComponent<RectTransform>();
        centerRect.anchorMin = new Vector2(0.5f, 0.5f);
        centerRect.anchorMax = new Vector2(0.5f, 0.5f);
        centerRect.sizeDelta = new Vector2(50, 50);

        // Create instruction text
        GameObject instructionObj = new GameObject("Instruction Text");
        instructionObj.transform.SetParent(gameCanvas.transform, false);
        instructionText = instructionObj.AddComponent<TextMeshProUGUI>();
        instructionText.fontSize = 24;
        instructionText.color = Color.white;
        instructionText.alignment = TextAlignmentOptions.Center;
        instructionText.text = "Watch the sequence of colors...";

        RectTransform instructionRect = instructionObj.GetComponent<RectTransform>();
        instructionRect.anchorMin = new Vector2(0.5f, 1);
        instructionRect.anchorMax = new Vector2(0.5f, 1);
        instructionRect.pivot = new Vector2(0.5f, 1);
        instructionRect.anchoredPosition = new Vector2(0, -50);
        instructionRect.sizeDelta = new Vector2(500, 50);

        // Create sequence text
        GameObject sequenceObj = new GameObject("Sequence Text");
        sequenceObj.transform.SetParent(gameCanvas.transform, false);
        sequenceText = sequenceObj.AddComponent<TextMeshProUGUI>();
        sequenceText.fontSize = 20;
        sequenceText.color = Color.white;
        sequenceText.alignment = TextAlignmentOptions.Center;

        RectTransform sequenceRect = sequenceObj.GetComponent<RectTransform>();
        sequenceRect.anchorMin = new Vector2(0.5f, 0);
        sequenceRect.anchorMax = new Vector2(0.5f, 0);
        sequenceRect.pivot = new Vector2(0.5f, 0);
        sequenceRect.anchoredPosition = new Vector2(0, 150);
        sequenceRect.sizeDelta = new Vector2(500, 50);

        // Create control buttons
        GameObject buttonsContainer = new GameObject("Buttons Container");
        buttonsContainer.transform.SetParent(gameCanvas.transform, false);
        HorizontalLayoutGroup buttonLayout = buttonsContainer.AddComponent<HorizontalLayoutGroup>();
        buttonLayout.spacing = 20;
        buttonLayout.childAlignment = TextAnchor.MiddleCenter;

        RectTransform buttonsRect = buttonsContainer.GetComponent<RectTransform>();
        buttonsRect.anchorMin = new Vector2(0.5f, 0);
        buttonsRect.anchorMax = new Vector2(0.5f, 0);
        buttonsRect.pivot = new Vector2(0.5f, 0);
        buttonsRect.anchoredPosition = new Vector2(0, 80);
        buttonsRect.sizeDelta = new Vector2(400, 60);

        // Create left rotate button
        GameObject leftButtonObj = new GameObject("Left Button");
        leftButtonObj.transform.SetParent(buttonsContainer.transform, false);
        rotateLeftButton = leftButtonObj.AddComponent<Button>();
        Image leftButtonImage = leftButtonObj.AddComponent<Image>();
        leftButtonImage.color = new Color(0.2f, 0.2f, 0.7f);

        GameObject leftText = new GameObject("Left Text");
        leftText.transform.SetParent(leftButtonObj.transform, false);
        TextMeshProUGUI leftTextMesh = leftText.AddComponent<TextMeshProUGUI>();
        leftTextMesh.text = "?";
        leftTextMesh.fontSize = 32;
        leftTextMesh.color = Color.white;
        leftTextMesh.alignment = TextAlignmentOptions.Center;

        RectTransform leftTextRect = leftText.GetComponent<RectTransform>();
        leftTextRect.anchorMin = Vector2.zero;
        leftTextRect.anchorMax = Vector2.one;
        leftTextRect.sizeDelta = Vector2.zero;

        RectTransform leftButtonRect = leftButtonObj.GetComponent<RectTransform>();
        leftButtonRect.sizeDelta = new Vector2(80, 60);

        // Create right rotate button
        GameObject rightButtonObj = new GameObject("Right Button");
        rightButtonObj.transform.SetParent(buttonsContainer.transform, false);
        rotateRightButton = rightButtonObj.AddComponent<Button>();
        Image rightButtonImage = rightButtonObj.AddComponent<Image>();
        rightButtonImage.color = new Color(0.2f, 0.2f, 0.7f);

        GameObject rightText = new GameObject("Right Text");
        rightText.transform.SetParent(rightButtonObj.transform, false);
        TextMeshProUGUI rightTextMesh = rightText.AddComponent<TextMeshProUGUI>();
        rightTextMesh.text = "?";
        rightTextMesh.fontSize = 32;
        rightTextMesh.color = Color.white;
        rightTextMesh.alignment = TextAlignmentOptions.Center;

        RectTransform rightTextRect = rightText.GetComponent<RectTransform>();
        rightTextRect.anchorMin = Vector2.zero;
        rightTextRect.anchorMax = Vector2.one;
        rightTextRect.sizeDelta = Vector2.zero;

        RectTransform rightButtonRect = rightButtonObj.GetComponent<RectTransform>();
        rightButtonRect.sizeDelta = new Vector2(80, 60);

        // Create confirm button
        GameObject confirmButtonObj = new GameObject("Confirm Button");
        confirmButtonObj.transform.SetParent(buttonsContainer.transform, false);
        confirmButton = confirmButtonObj.AddComponent<Button>();
        Image confirmButtonImage = confirmButtonObj.AddComponent<Image>();
        confirmButtonImage.color = new Color(0.2f, 0.7f, 0.3f);

        GameObject confirmText = new GameObject("Confirm Text");
        confirmText.transform.SetParent(confirmButtonObj.transform, false);
        TextMeshProUGUI confirmTextMesh = confirmText.AddComponent<TextMeshProUGUI>();
        confirmTextMesh.text = "Confirm";
        confirmTextMesh.fontSize = 20;
        confirmTextMesh.color = Color.white;
        confirmTextMesh.alignment = TextAlignmentOptions.Center;

        RectTransform confirmTextRect = confirmText.GetComponent<RectTransform>();
        confirmTextRect.anchorMin = Vector2.zero;
        confirmTextRect.anchorMax = Vector2.one;
        confirmTextRect.sizeDelta = Vector2.zero;

        RectTransform confirmButtonRect = confirmButtonObj.GetComponent<RectTransform>();
        confirmButtonRect.sizeDelta = new Vector2(120, 60);

        // Create close button
        GameObject closeButtonObj = new GameObject("Close Button");
        closeButtonObj.transform.SetParent(gameCanvas.transform, false);
        closeButton = closeButtonObj.AddComponent<Button>();
        Image closeButtonImage = closeButtonObj.AddComponent<Image>();
        closeButtonImage.color = new Color(0.8f, 0.2f, 0.2f);

        GameObject closeText = new GameObject("Close Text");
        closeText.transform.SetParent(closeButtonObj.transform, false);
        TextMeshProUGUI closeTextMesh = closeText.AddComponent<TextMeshProUGUI>();
        closeTextMesh.text = "X";
        closeTextMesh.fontSize = 24;
        closeTextMesh.color = Color.white;
        closeTextMesh.alignment = TextAlignmentOptions.Center;

        RectTransform closeTextRect = closeText.GetComponent<RectTransform>();
        closeTextRect.anchorMin = Vector2.zero;
        closeTextRect.anchorMax = Vector2.one;
        closeTextRect.sizeDelta = Vector2.zero;

        RectTransform closeButtonRect = closeButtonObj.GetComponent<RectTransform>();
        closeButtonRect.anchorMin = new Vector2(1, 1);
        closeButtonRect.anchorMax = new Vector2(1, 1);
        closeButtonRect.pivot = new Vector2(1, 1);
        closeButtonRect.anchoredPosition = new Vector2(-20, -20);
        closeButtonRect.sizeDelta = new Vector2(50, 50);

        // Create completion panel
        completionPanel = new GameObject("Completion Panel");
        completionPanel.transform.SetParent(gameCanvas.transform, false);
        Image completionBg = completionPanel.AddComponent<Image>();
        completionBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

        RectTransform completionRect = completionPanel.GetComponent<RectTransform>();
        completionRect.anchorMin = new Vector2(0.5f, 0.5f);
        completionRect.anchorMax = new Vector2(0.5f, 0.5f);
        completionRect.pivot = new Vector2(0.5f, 0.5f);
        completionRect.sizeDelta = new Vector2(400, 200);

        GameObject completionTextObj = new GameObject("Completion Text");
        completionTextObj.transform.SetParent(completionPanel.transform, false);
        completionText = completionTextObj.AddComponent<TextMeshProUGUI>();
        completionText.fontSize = 24;
        completionText.color = Color.white;
        completionText.alignment = TextAlignmentOptions.Center;

        RectTransform complTextRect = completionTextObj.GetComponent<RectTransform>();
        complTextRect.anchorMin = Vector2.zero;
        complTextRect.anchorMax = Vector2.one;
        complTextRect.offsetMin = new Vector2(20, 20);
        complTextRect.offsetMax = new Vector2(-20, -70);

        GameObject continueButton = new GameObject("Continue Button");
        continueButton.transform.SetParent(completionPanel.transform, false);
        Button continueButtonComp = continueButton.AddComponent<Button>();
        Image continueButtonImage = continueButton.AddComponent<Image>();
        continueButtonImage.color = new Color(0.2f, 0.7f, 0.3f);
        continueButtonComp.onClick.AddListener(CloseGame);

        GameObject continueText = new GameObject("Continue Text");
        continueText.transform.SetParent(continueButton.transform, false);
        TextMeshProUGUI continueTextMesh = continueText.AddComponent<TextMeshProUGUI>();
        continueTextMesh.text = "Continue";
        continueTextMesh.fontSize = 20;
        continueTextMesh.color = Color.white;
        continueTextMesh.alignment = TextAlignmentOptions.Center;

        RectTransform continueTextRect = continueText.GetComponent<RectTransform>();
        continueTextRect.anchorMin = Vector2.zero;
        continueTextRect.anchorMax = Vector2.one;
        continueTextRect.sizeDelta = Vector2.zero;

        RectTransform continueButtonRect = continueButton.GetComponent<RectTransform>();
        continueButtonRect.anchorMin = new Vector2(0.5f, 0);
        continueButtonRect.anchorMax = new Vector2(0.5f, 0);
        continueButtonRect.pivot = new Vector2(0.5f, 0);
        continueButtonRect.anchoredPosition = new Vector2(0, 20);
        continueButtonRect.sizeDelta = new Vector2(150, 50);
    }

    private void SetupButtonListeners()
    {
        if (rotateLeftButton != null)
        {
            rotateLeftButton.onClick.RemoveAllListeners();
            rotateLeftButton.onClick.AddListener(RotateWheelLeft);
        }

        if (rotateRightButton != null)
        {
            rotateRightButton.onClick.RemoveAllListeners();
            rotateRightButton.onClick.AddListener(RotateWheelRight);
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(ConfirmSelection);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseGame);
        }
    }

    // Add to the Awake method to hide the UI until game starts
    private void Awake()
    {
        // If references aren't set in the inspector, create them dynamically
        if (gameCanvas == null)
        {
            CreateUIElements();
        }

        // Set up button listeners
        SetupButtonListeners();

        // IMPORTANT: Hide UI until game is explicitly started
        gameCanvas.SetActive(false);
    }

    // Fix the completion check in CheckPlayerSequence method
    private IEnumerator CheckPlayerSequence()
    {
        EnablePlayerControls(false);

        // Check if the sequences match
        bool success = true;
        for (int i = 0; i < targetSequence.Count; i++)
        {
            if (playerSequence[i] != targetSequence[i])
            {
                success = false;
                break;
            }
        }

        yield return new WaitForSeconds(0.5f);

        if (success)
        {
            // Check if this was the final level (sequence reached max length)
            int currentSequenceLength = startSequenceLength + currentLevel - 1;

            if (currentSequenceLength >= maxSequenceLength)
            {
                // Player has completed the final level - show completion immediately
                instructionText.text = "Congratulations! You've completed all sequences!";
                yield return new WaitForSeconds(1.5f);
                ShowCompletionPanel(true);
            }
            else
            {
                // Move to next level
                instructionText.text = "Great job! Moving to next level...";
                currentLevel++;
                yield return new WaitForSeconds(1.5f);
                StartCoroutine(PlayRound());
            }
        }
        else
        {
            // Player failed - rest of code unchanged
            instructionText.text = "Oh no! That wasn't the right sequence. Try again.";

            // Show the correct sequence
            yield return new WaitForSeconds(1f);

            instructionText.text = "The correct sequence was:";
            string correctSequence = "";

            for (int i = 0; i < targetSequence.Count; i++)
            {
                int color = targetSequence[i];
                correctSequence += $"<color=#{ColorUtility.ToHtmlStringRGB(potionColors[color])}>{colorNames[color]}</color> ";
            }

            sequenceText.text = correctSequence;

            yield return new WaitForSeconds(2f);

            // Reset same level
            StartCoroutine(PlayRound());
        }
    }

    // Improve the completion panel to ensure it has a prominent close button
    private void ShowCompletionPanel(bool success)
    {
        if (completionPanel != null && completionText != null)
        {
            completionPanel.SetActive(true);

            if (success)
            {
                completionText.text = "Congratulations!\n\nYou have successfully mixed all the potions!";

                // Make sure there's a visible way to close the game
                Transform continueButtonTrans = completionPanel.transform.Find("Continue Button");
                if (continueButtonTrans != null)
                {
                    Button continueButton = continueButtonTrans.GetComponent<Button>();
                    if (continueButton != null)
                    {
                        // Make the button more prominent
                        RectTransform buttonRect = continueButton.GetComponent<RectTransform>();
                        buttonRect.sizeDelta = new Vector2(200, 60);

                        // Find the text component and update it
                        TextMeshProUGUI buttonText = continueButton.GetComponentInChildren<TextMeshProUGUI>();
                        if (buttonText != null)
                        {
                            buttonText.text = "CLOSE";
                            buttonText.fontSize = 24;
                        }

                        // Ensure it has the close game function assigned
                        continueButton.onClick.RemoveAllListeners();
                        continueButton.onClick.AddListener(CloseGame);
                    }
                }
            }
            else
            {
                completionText.text = "Game Over!\n\nBetter luck next time.";
            }
        }
    }

    private IEnumerator PlayRound()
    {
        EnablePlayerControls(false);

        instructionText.text = $"Level {currentLevel}: Watch the sequence of colors...";

        // Generate a sequence based on current level
        int sequenceLength = Mathf.Min(startSequenceLength + currentLevel - 1, maxSequenceLength);
        GenerateSequence(sequenceLength);

        yield return new WaitForSeconds(1f);

        // Show the sequence to the player
        isShowingSequence = true;
        yield return StartCoroutine(ShowSequence());
        isShowingSequence = false;

        // Now it's the player's turn
        instructionText.text = "Your turn! Rotate the wheel to match the sequence.";
        playerSequence.Clear();
        playbackIndex = 0;

        UpdateSequenceText();
        EnablePlayerControls(true);
        isPlayerTurn = true;
    }


    private void GenerateSequence(int length)
    {
        // If we're beyond level 1, keep the previous sequence and add to it
        if (currentLevel > 1 && targetSequence.Count > 0)
        {
            // Store the old sequence
            List<int> previousSequence = new List<int>(targetSequence);

            // Clear and rebuild with the previous sequence as foundation
            targetSequence.Clear();
            targetSequence.AddRange(previousSequence);

            // Add new random colors to reach the desired length, avoiding repeats
            int additionalColors = length - previousSequence.Count;
            int lastColor = targetSequence.Count > 0 ? targetSequence[targetSequence.Count - 1] : -1;

            for (int i = 0; i < additionalColors; i++)
            {
                // Generate a new color that's different from the last one
                int newColor;
                do
                {
                    newColor = Random.Range(0, 4);
                } while (newColor == lastColor);

                targetSequence.Add(newColor);
                lastColor = newColor;
            }
        }
        else
        {
            // First level - generate a fresh sequence with no consecutive repeats
            targetSequence.Clear();
            int lastColor = -1;

            for (int i = 0; i < length; i++)
            {
                int newColor;
                do
                {
                    newColor = Random.Range(0, 4);
                } while (newColor == lastColor);

                targetSequence.Add(newColor);
                lastColor = newColor;
            }
        }

        // Debug output to verify the sequence
        string sequenceDebug = "Generated sequence: ";
        foreach (int color in targetSequence)
        {
            sequenceDebug += colorNames[color] + " ";
        }
    }

    private void UpdateSequenceText()
    {
        // Build sequence text to show progress
        StringBuilder text = new StringBuilder();

        if (currentLevel > 1)
        {
            // Show which colors are from previous levels
            int previousLevelCount = targetSequence.Count - (startSequenceLength + currentLevel - 1);

            text.Append("Previous: ");
            for (int i = 0; i < previousLevelCount; i++)
            {
                if (i < playerSequence.Count)
                {
                    // Already selected color
                    int playerColor = playerSequence[i];
                    text.Append($"<color=#{ColorUtility.ToHtmlStringRGB(potionColors[playerColor])}>{colorNames[playerColor]}</color> ");
                }
                else
                {
                    text.Append("? ");
                }
            }

            text.Append(" | New: ");
        }

        // Show current level colors
        int newColorsStart = currentLevel > 1 ? targetSequence.Count - (startSequenceLength + currentLevel - 1) : 0;
        for (int i = newColorsStart; i < targetSequence.Count; i++)
        {
            if (i < playerSequence.Count)
            {
                // Already selected color
                int playerColor = playerSequence[i];
                text.Append($"<color=#{ColorUtility.ToHtmlStringRGB(potionColors[playerColor])}>{colorNames[playerColor]}</color> ");
            }
            else if (i == playerSequence.Count)
            {
                // Current position
                text.Append("<color=#FFFFFF><b>?</b></color> ");
            }
            else
            {
                // Not yet reached
                text.Append("? ");
            }
        }

        sequenceText.text = text.ToString();
    }

    private void EnablePlayerControls(bool enable)
    {
        if (rotateLeftButton != null)
            rotateLeftButton.interactable = enable;

        if (rotateRightButton != null)
            rotateRightButton.interactable = enable;

        if (confirmButton != null)
            confirmButton.interactable = enable;
    }

   

    public void CloseGame()
    {
        gameCanvas.SetActive(false);
    }

    // Adjust the wheel mapping - index 0 (Red) must be at the bottom
    // This defines which rotation angle corresponds to which color at the bottom
    private void UpdateWheelConfiguration()
    {
        // Set the wheel orientation so that:
        // 0 degrees = Red (index 0) at bottom
        // 90 degrees = Blue (index 1) at bottom
        // 180 degrees = Green (index 2) at bottom
        // 270 degrees = Yellow (index 3) at bottom

        // Reset the wheel to default position (Red at bottom)
        wheelObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        currentWheelPosition = 0; // Red is at position 0
    }

    // Revised method to properly show and highlight the bottom color
    private IEnumerator ShowSequence()
    {
        sequenceText.text = "Watch the colors at the BOTTOM of the wheel:";
        yield return new WaitForSeconds(1.5f);

        // Make sure wheel starts in correct position
        UpdateWheelConfiguration();

        for (int i = 0; i < targetSequence.Count; i++)
        {
            int targetPosition = targetSequence[i];

            // Calculate the rotation needed to bring this color to the bottom
            yield return StartCoroutine(RotateWheelToPosition(targetPosition));

            // Verify the color at bottom matches what we expect
            int bottomColor = currentWheelPosition;

            // Add visual indicator to show which color is at the bottom
            GameObject indicator = new GameObject("BottomIndicator");
            indicator.transform.SetParent(wheelObject.transform.parent, false);
            RectTransform indicatorRect = indicator.AddComponent<RectTransform>();
            Image indicatorImage = indicator.AddComponent<Image>();

            // Position the indicator pointing to bottom of wheel
            indicatorRect.anchorMin = new Vector2(0.5f, 0);
            indicatorRect.anchorMax = new Vector2(0.5f, 0);
            indicatorRect.pivot = new Vector2(0.5f, 0);
            indicatorRect.sizeDelta = new Vector2(30, 50);
            indicatorRect.anchoredPosition = new Vector2(0, -30);

            // Triangle shape pointing up
            indicatorImage.color = Color.white;

            // Highlight the current bottom color in the sequence text
            sequenceText.text = $"Color {i + 1}: <color=#{ColorUtility.ToHtmlStringRGB(potionColors[bottomColor])}>{colorNames[bottomColor]}</color> (at bottom)";

            // Wait for a moment
            yield return new WaitForSeconds(displayInterval);

            // Destroy the indicator
            Destroy(indicator);

            // Wait between colors
            yield return new WaitForSeconds(0.5f);
        }

        sequenceText.text = "Now your turn! Rotate the wheel to place each color at the BOTTOM.";
    }

    // Improved rotation method that ensures accurate positioning
    private IEnumerator RotateWheelToPosition(int targetPosition)
    {
        // Calculate the rotation needed to bring the target color to the bottom position
        // In our wheel setup:
        // Position 0 (Red) is at bottom with rotation 0
        // Position 1 (Blue) is at bottom with rotation 90
        // Position 2 (Green) is at bottom with rotation 180
        // Position 3 (Yellow) is at bottom with rotation 270

        int currentPos = currentWheelPosition;

        // Calculate the shortest rotation direction (clockwise or counterclockwise)
        int diff = (targetPosition - currentPos + 4) % 4;
        if (diff > 2)
        {
            diff -= 4; // This makes the rotation counter-clockwise if it's shorter
        }

        // Calculate the target rotation in degrees (90 degrees per position)
        float startRotation = wheelObject.transform.rotation.eulerAngles.z;
        float targetRotation = (targetPosition * 90) % 360; // Direct calculation of final angle

        // Handle the case where we need to rotate through 0 degrees
        if (Mathf.Abs(targetRotation - startRotation) > 180)
        {
            if (targetRotation > startRotation)
            {
                startRotation += 360;
            }
            else
            {
                targetRotation += 360;
            }
        }

        float time = 0;
        while (time < rotationDuration)
        {
            time += Time.deltaTime;
            float t = time / rotationDuration;
            float currentRotation = Mathf.Lerp(startRotation, targetRotation, t);

            wheelObject.transform.rotation = Quaternion.Euler(0, 0, currentRotation % 360);
            yield return null;
        }

        // Ensure exact rotation angle
        wheelObject.transform.rotation = Quaternion.Euler(0, 0, targetRotation % 360);
        currentWheelPosition = targetPosition;

        // Debug confirmation
        Debug.Log($"Wheel rotated to position {targetPosition}, color {colorNames[targetPosition]} is now at the bottom");
    }

    // Modified player control methods to clearly show which color is at the bottom
    private void RotateWheelLeft()
    {
        if (!isPlayerTurn || isShowingSequence)
            return;

        // Rotate counterclockwise (bottom color changes to next color)
        int newPosition = (currentWheelPosition + 1) % 4;
        StartCoroutine(RotateWheelToPosition(newPosition));

        // Show which color is now at the bottom
        instructionText.text = $"Current bottom color: {colorNames[newPosition]}";
    }

    private void RotateWheelRight()
    {
        if (!isPlayerTurn || isShowingSequence)
            return;

        // Rotate clockwise (bottom color changes to previous color)
        int newPosition = (currentWheelPosition - 1 + 4) % 4;
        StartCoroutine(RotateWheelToPosition(newPosition));

        // Show which color is now at the bottom
        instructionText.text = $"Current bottom color: {colorNames[newPosition]}";
    }

    private void ConfirmSelection()
    {
        if (!isPlayerTurn || isShowingSequence)
            return;

        // Add current bottom color to player sequence
        playerSequence.Add(currentWheelPosition);
        playbackIndex++;

        // Provide feedback about the selection
        instructionText.text = $"Selected: {colorNames[currentWheelPosition]} - {playerSequence.Count} of {targetSequence.Count}";

        // Update the display to show player's progress
        UpdateSequenceText();

        // Check if the player has completed the sequence
        if (playerSequence.Count >= targetSequence.Count)
        {
            isPlayerTurn = false;
            StartCoroutine(CheckPlayerSequence());
        }
    }

    // Add this method to your Start or StartGame method
    public void StartGame()
    {
        gameCanvas.SetActive(true);
        if (completionPanel != null)
        {
            completionPanel.SetActive(false);
        }

        // Reset game state
        currentLevel = 1;
        targetSequence.Clear();
        playerSequence.Clear();

        // Start with wheel in default position - Red at bottom
        UpdateWheelConfiguration();

        // Begin first round
        StartCoroutine(PlayRound());
    }
}

