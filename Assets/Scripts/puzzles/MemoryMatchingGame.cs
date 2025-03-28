using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class MemoryMatchingGame : MonoBehaviour
{
    // Game settings
    private int gridWidth = 4;
    private int gridHeight = 4;
    private float cardFlipDuration = 0.3f;
    private float matchCheckDelay = 1f;

    // Runtime references
    private GameObject gameCanvas;
    private GameObject cardContainer;
    private TextMeshProUGUI matchesText;
    private Button closeButton;
    private GameObject completionPanel;
    private TextMeshProUGUI completionText;

    // State tracking
    private List<MemoryCard> cards = new List<MemoryCard>();
    private MemoryCard firstSelectedCard;
    private MemoryCard secondSelectedCard;
    private bool canSelect = true;
    private int matchesFound = 0;
    private int totalPairs;
    private int moveCount = 0;

    public void StartGame()
    {
        Debug.Log("MemoryMatchingGame: Starting game");

        // Create fresh UI every time
        CleanupPreviousGame();
        CreateGameUI();
        CreateCards();

        Debug.Log($"MemoryMatchingGame: Created {cards.Count} cards");
    }

    private void CleanupPreviousGame()
    {
        // Destroy any existing canvas
        if (gameCanvas != null)
        {
            Destroy(gameCanvas);
        }

        // Reset state
        cards.Clear();
        firstSelectedCard = null;
        secondSelectedCard = null;
        matchesFound = 0;
        moveCount = 0;
        canSelect = true;
    }

    private void CreateGameUI()
    {
        // Create canvas
        gameCanvas = new GameObject("Memory Game Canvas");
        gameCanvas.transform.SetParent(transform);

        Canvas canvas = gameCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        gameCanvas.AddComponent<CanvasScaler>();
        gameCanvas.AddComponent<GraphicRaycaster>();

        // Create background panel
        GameObject panel = new GameObject("Background Panel");
        panel.transform.SetParent(gameCanvas.transform, false);

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.9f);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.2f, 0.1f);
        panelRect.anchorMax = new Vector2(0.8f, 0.9f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Create title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panel.transform, false);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "Memory Matching Game";
        titleText.fontSize = 30;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;

        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.sizeDelta = new Vector2(0, 50);
        titleRect.anchoredPosition = new Vector2(0, -25);

        // Create card container
        cardContainer = new GameObject("Card Container");
        cardContainer.transform.SetParent(panel.transform, false);

        Image containerImage = cardContainer.AddComponent<Image>();
        containerImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);

        RectTransform containerRect = cardContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.1f, 0.2f);
        containerRect.anchorMax = new Vector2(0.9f, 0.8f);
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;

        // Add a visible grid layout
        GridLayoutGroup grid = cardContainer.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(70, 70);
        grid.spacing = new Vector2(10, 10);
        grid.padding = new RectOffset(10, 10, 10, 10);
        grid.constraintCount = gridWidth;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

        // Create status text
        GameObject statusObj = new GameObject("Status");
        statusObj.transform.SetParent(panel.transform, false);

        matchesText = statusObj.AddComponent<TextMeshProUGUI>();
        matchesText.text = "Matches: 0/0 | Moves: 0";
        matchesText.fontSize = 20;
        matchesText.color = Color.white;
        matchesText.alignment = TextAlignmentOptions.Center;

        RectTransform statusRect = statusObj.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0, 0);
        statusRect.anchorMax = new Vector2(1, 0);
        statusRect.sizeDelta = new Vector2(0, 40);
        statusRect.anchoredPosition = new Vector2(0, 20);

        // Create close button
        GameObject closeObj = new GameObject("Close Button");
        closeObj.transform.SetParent(panel.transform, false);

        closeButton = closeObj.AddComponent<Button>();
        Image closeImage = closeObj.AddComponent<Image>();
        closeImage.color = Color.red;

        RectTransform closeRect = closeObj.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1, 1);
        closeRect.anchorMax = new Vector2(1, 1);
        closeRect.sizeDelta = new Vector2(40, 40);
        closeRect.anchoredPosition = new Vector2(-20, -20);

        // Add X text to close button
        GameObject closeTextObj = new GameObject("X");
        closeTextObj.transform.SetParent(closeObj.transform, false);

        TextMeshProUGUI closeTextMesh = closeTextObj.AddComponent<TextMeshProUGUI>();
        closeTextMesh.text = "X";
        closeTextMesh.fontSize = 24;
        closeTextMesh.color = Color.white;
        closeTextMesh.alignment = TextAlignmentOptions.Center;

        RectTransform closeTextRect = closeTextObj.GetComponent<RectTransform>();
        closeTextRect.anchorMin = Vector2.zero;
        closeTextRect.anchorMax = Vector2.one;
        closeTextRect.offsetMin = Vector2.zero;
        closeTextRect.offsetMax = Vector2.zero;

        closeButton.onClick.AddListener(CloseGame);

        // Create completion panel
        CreateCompletionPanel();

        // Calculate total pairs
        totalPairs = (gridWidth * gridHeight) / 2;

        // Update status text
        UpdateStatusText();
    }

    private void CreateCompletionPanel()
    {
        completionPanel = new GameObject("Completion Panel");
        completionPanel.transform.SetParent(gameCanvas.transform, false);

        Image panelImage = completionPanel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);

        RectTransform panelRect = completionPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(400, 250);
        panelRect.anchoredPosition = Vector2.zero;

        // Create completion text
        GameObject textObj = new GameObject("Completion Text");
        textObj.transform.SetParent(completionPanel.transform, false);

        completionText = textObj.AddComponent<TextMeshProUGUI>();
        completionText.fontSize = 28;
        completionText.color = Color.white;
        completionText.alignment = TextAlignmentOptions.Center;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0.5f);
        textRect.anchorMax = new Vector2(1, 0.8f);
        textRect.offsetMin = new Vector2(20, 0);
        textRect.offsetMax = new Vector2(-20, 0);

        // Create continue button
        GameObject btnObj = new GameObject("Continue Button");
        btnObj.transform.SetParent(completionPanel.transform, false);

        Button continueBtn = btnObj.AddComponent<Button>();
        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.7f, 0.3f);
        continueBtn.onClick.AddListener(CloseGame);

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.2f);
        btnRect.anchorMax = new Vector2(0.5f, 0.2f);
        btnRect.sizeDelta = new Vector2(150, 50);
        btnRect.anchoredPosition = Vector2.zero;

        GameObject btnTextObj = new GameObject("Button Text");
        btnTextObj.transform.SetParent(btnObj.transform, false);

        TextMeshProUGUI btnText = btnTextObj.AddComponent<TextMeshProUGUI>();
        btnText.text = "Continue";
        btnText.fontSize = 24;
        btnText.color = Color.white;
        btnText.alignment = TextAlignmentOptions.Center;

        RectTransform btnTextRect = btnTextObj.GetComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.offsetMin = Vector2.zero;
        btnTextRect.offsetMax = Vector2.zero;

        // Hide panel initially
        completionPanel.SetActive(false);
    }

    private void CreateCards()
    {
        for (int i = 0; i < totalPairs; i++)
        {
            // Create two matching cards
            for (int j = 0; j < 2; j++)
            {
                // Create card game object
                GameObject cardObj = new GameObject($"Card_{i}_{j}");
                cardObj.transform.SetParent(cardContainer.transform, false);

                // Create card face (front)
                GameObject cardFace = new GameObject("Card Face");
                cardFace.transform.SetParent(cardObj.transform, false);

                // Add face image with color based on card ID
                Image faceImage = cardFace.AddComponent<Image>();
                float hue = (float)i / totalPairs;
                faceImage.color = Color.HSVToRGB(hue, 0.8f, 0.8f);

                RectTransform faceRect = cardFace.GetComponent<RectTransform>();
                faceRect.anchorMin = Vector2.zero;
                faceRect.anchorMax = Vector2.one;
                faceRect.offsetMin = Vector2.zero;
                faceRect.offsetMax = Vector2.zero;

                // Add card ID text
                GameObject labelObj = new GameObject("Card ID");
                labelObj.transform.SetParent(cardFace.transform, false);

                TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
                label.text = i.ToString();
                label.fontSize = 30;
                label.color = Color.white;
                label.alignment = TextAlignmentOptions.Center;

                RectTransform labelRect = labelObj.GetComponent<RectTransform>();
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.offsetMin = Vector2.zero;
                labelRect.offsetMax = Vector2.zero;

                // Create card back
                GameObject cardBack = new GameObject("Card Back");
                cardBack.transform.SetParent(cardObj.transform, false);

                Image backImage = cardBack.AddComponent<Image>();
                backImage.color = new Color(0.3f, 0.3f, 0.3f);

                RectTransform backRect = cardBack.GetComponent<RectTransform>();
                backRect.anchorMin = Vector2.zero;
                backRect.anchorMax = Vector2.one;
                backRect.offsetMin = Vector2.zero;
                backRect.offsetMax = Vector2.zero;

                // Add a decorative pattern on the back
                GameObject patternObj = new GameObject("Pattern");
                patternObj.transform.SetParent(cardBack.transform, false);

                Image patternImage = patternObj.AddComponent<Image>();
                patternImage.color = new Color(0.5f, 0.5f, 0.5f);

                RectTransform patternRect = patternObj.GetComponent<RectTransform>();
                patternRect.anchorMin = new Vector2(0.2f, 0.2f);
                patternRect.anchorMax = new Vector2(0.8f, 0.8f);
                patternRect.offsetMin = Vector2.zero;
                patternRect.offsetMax = Vector2.zero;

                // Add button component
                Button cardButton = cardObj.AddComponent<Button>();
                cardButton.transition = Selectable.Transition.ColorTint;

                // Add an image component to make the button work
                Image cardImage = cardObj.AddComponent<Image>();
                cardImage.color = new Color(1, 1, 1, 0.01f); // Almost transparent

                // Add memory card component
                MemoryCard card = cardObj.AddComponent<MemoryCard>();
                card.Initialize(i, cardFace, cardBack);

                // Register callback
                card.OnCardSelected += OnCardSelected;

                // Add card to list
                cards.Add(card);
            }
        }

        // Shuffle the cards
        ShuffleCards();
    }

    private void ShuffleCards()
    {
        // Shuffle card transforms
        List<Transform> cardTransforms = new List<Transform>();

        foreach (Transform child in cardContainer.transform)
        {
            cardTransforms.Add(child);
        }

        // Fisher-Yates shuffle
        for (int i = cardTransforms.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            cardTransforms[i].SetSiblingIndex(randomIndex);
        }
    }

    private void OnCardSelected(MemoryCard card)
    {
        if (!canSelect || card.IsMatched || card == firstSelectedCard)
            return;

        StartCoroutine(FlipCard(card, true));

        if (firstSelectedCard == null)
        {
            firstSelectedCard = card;
        }
        else
        {
            moveCount++;
            secondSelectedCard = card;
            canSelect = false;
            StartCoroutine(CheckForMatch());
        }

        UpdateStatusText();
    }

    private IEnumerator FlipCard(MemoryCard card, bool faceUp)
    {
        float time = 0;

        while (time < cardFlipDuration)
        {
            time += Time.deltaTime;
            float normalizedTime = time / cardFlipDuration;

            if (normalizedTime < 0.5f)
            {
                // First half - shrink horizontally
                float scale = 1 - normalizedTime * 2;
                card.transform.localScale = new Vector3(scale, 1, 1);
            }
            else
            {
                // Flip the card at the halfway point
                if (normalizedTime >= 0.5f && normalizedTime <= 0.51f)
                {
                    card.SetFaceUp(faceUp);
                }

                // Second half - expand horizontally
                float scale = (normalizedTime - 0.5f) * 2;
                card.transform.localScale = new Vector3(scale, 1, 1);
            }

            yield return null;
        }

        // Ensure final state is correct
        card.transform.localScale = Vector3.one;
        card.SetFaceUp(faceUp);
    }

    private IEnumerator CheckForMatch()
    {
        // Wait before checking
        yield return new WaitForSeconds(matchCheckDelay);

        bool isMatch = firstSelectedCard.CardId == secondSelectedCard.CardId;

        if (isMatch)
        {
            // Mark cards as matched
            firstSelectedCard.SetMatched(true);
            secondSelectedCard.SetMatched(true);

            matchesFound++;
            UpdateStatusText();

            // Check if game is complete
            if (matchesFound >= totalPairs)
            {
                ShowCompletionPanel();
            }
        }
        else
        {
            // Flip cards back
            StartCoroutine(FlipCard(firstSelectedCard, false));
            StartCoroutine(FlipCard(secondSelectedCard, false));
        }

        // Reset selection
        firstSelectedCard = null;
        secondSelectedCard = null;

        // Allow selection again
        yield return new WaitForSeconds(0.1f);
        canSelect = true;
    }

    private void UpdateStatusText()
    {
        if (matchesText != null)
        {
            matchesText.text = $"Matches: {matchesFound}/{totalPairs} | Moves: {moveCount}";
        }
    }

    private void ShowCompletionPanel()
    {
        if (completionPanel != null && completionText != null)
        {
            completionPanel.SetActive(true);
            completionText.text = $"Congratulations!\n\nYou completed the memory match game in {moveCount} moves.";
        }
    }

    public void CloseGame()
    {
        Debug.Log("MemoryMatchingGame: Closing game");

        if (gameCanvas != null)
        {
            gameCanvas.SetActive(false);
        }
    }
}

// Enhanced card implementation
public class MemoryCard : MonoBehaviour
{
    public event System.Action<MemoryCard> OnCardSelected;

    public int CardId { get; private set; }
    public bool IsMatched { get; private set; }

    private GameObject cardFace;
    private GameObject cardBack;
    private Button cardButton;

    public void Initialize(int id, GameObject face, GameObject back)
    {
        CardId = id;
        IsMatched = false;
        cardFace = face;
        cardBack = back;

        // Set up the button
        cardButton = GetComponent<Button>();
        if (cardButton != null)
        {
            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(OnButtonClick);
        }

        // Start face down
        SetFaceUp(false);
    }

    private void OnButtonClick()
    {
        if (OnCardSelected != null)
        {
            OnCardSelected(this);
        }
    }

    public void SetFaceUp(bool faceUp)
    {
        if (cardFace != null)
            cardFace.SetActive(faceUp);

        if (cardBack != null)
            cardBack.SetActive(!faceUp);
    }

    public void SetMatched(bool matched)
    {
        IsMatched = matched;

        if (matched && cardButton != null)
        {
            // Visually indicate matched
            Image faceImage = cardFace.GetComponent<Image>();
            if (faceImage != null)
            {
                Color color = faceImage.color;
                faceImage.color = new Color(color.r, color.g, color.b, 0.7f);
            }

            // Disable interaction
            cardButton.interactable = false;
        }
    }
}