using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class MemoryMatchingGame : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int gridWidth = 6;
    [SerializeField] private int gridHeight = 6;
    [SerializeField] private float cardFlipDuration = 0.3f;
    [SerializeField] private float matchCheckDelay = 1f;
    [SerializeField] private Sprite cardBackSprite;

    [Header("UI References")]
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardGrid;
    [SerializeField] private TextMeshProUGUI matchesText;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject completionPanel;
    [SerializeField] private TextMeshProUGUI completionText;

    [Header("Card Images")]
    [SerializeField] private List<Sprite> cardImages = new List<Sprite>();

    private List<MemoryCard> cards = new List<MemoryCard>();
    private MemoryCard firstSelectedCard;
    private MemoryCard secondSelectedCard;
    private bool canSelect = true;
    private int matchesFound = 0;
    private int totalPairs;
    private int moveCount = 0;

    private void Awake()
    {
        // If references aren't set in the inspector, create them dynamically
        if (gameCanvas == null)
        {
            gameCanvas = new GameObject("Memory Game Canvas");
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

            // Create the grid container
            GameObject gridObject = new GameObject("Card Grid");
            gridObject.transform.SetParent(gameCanvas.transform, false);
            cardGrid = gridObject.transform;

            RectTransform gridRect = gridObject.AddComponent<RectTransform>();
            GridLayoutGroup gridLayout = gridObject.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(100, 100);
            gridLayout.spacing = new Vector2(10, 10);
            gridLayout.padding = new RectOffset(20, 20, 20, 20);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = gridWidth;

            gridRect.anchorMin = new Vector2(0.5f, 0.5f);
            gridRect.anchorMax = new Vector2(0.5f, 0.5f);
            gridRect.pivot = new Vector2(0.5f, 0.5f);
            gridRect.sizeDelta = new Vector2((gridWidth * 110) + 40, (gridHeight * 110) + 40);

            // Create UI for match counter
            GameObject matchCounter = new GameObject("Match Counter");
            matchCounter.transform.SetParent(gameCanvas.transform, false);
            matchesText = matchCounter.AddComponent<TextMeshProUGUI>();
            matchesText.fontSize = 24;
            matchesText.color = Color.white;
            matchesText.alignment = TextAlignmentOptions.Center;

            RectTransform matchRect = matchCounter.GetComponent<RectTransform>();
            matchRect.anchorMin = new Vector2(0.5f, 1);
            matchRect.anchorMax = new Vector2(0.5f, 1);
            matchRect.pivot = new Vector2(0.5f, 1);
            matchRect.anchoredPosition = new Vector2(0, -20);
            matchRect.sizeDelta = new Vector2(300, 50);

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
            complTextRect.offsetMax = new Vector2(-20, -20);

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
            continueButtonRect.anchoredPosition = new Vector2(0, 30);
            continueButtonRect.sizeDelta = new Vector2(150, 50);
        }

        // Create card prefab if not assigned
        if (cardPrefab == null)
        {
            cardPrefab = new GameObject("Card Prefab");
            cardPrefab.SetActive(false); // Make it a prefab

            // Add main card image component for the face
            Image cardImage = cardPrefab.AddComponent<Image>();
            cardImage.raycastTarget = true;
            cardImage.color = Color.white;

            RectTransform cardRect = cardPrefab.GetComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(100, 100);

            Button cardButton = cardPrefab.AddComponent<Button>();
            ColorBlock colors = cardButton.colors;
            colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
            cardButton.colors = colors;

            // Add card back as a child object
            GameObject cardBack = new GameObject("Card Back");
            cardBack.transform.SetParent(cardPrefab.transform, false);
            Image cardBackImage = cardBack.AddComponent<Image>();
            cardBackImage.color = Color.gray; // Default back color

            RectTransform cardBackRect = cardBack.GetComponent<RectTransform>();
            cardBackRect.anchorMin = Vector2.zero;
            cardBackRect.anchorMax = Vector2.one;
            cardBackRect.sizeDelta = Vector2.zero;

            cardPrefab.AddComponent<MemoryCard>();
        }

        // Set up close button
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseGame);
        }

        if (completionPanel != null)
        {
            completionPanel.SetActive(false);
        }
    }

    public void StartGame()
    {
        gameCanvas.SetActive(true);
        completionPanel.SetActive(false);
        matchesFound = 0;
        moveCount = 0;

        UpdateMatchesText();
        ClearGrid();
        CreateCards();
        ShuffleCards();
    }

    private void ClearGrid()
    {
        cards.Clear();

        if (cardGrid != null)
        {
            foreach (Transform child in cardGrid)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void CreateCards()
    {
        int totalCards = gridWidth * gridHeight;
        totalPairs = totalCards / 2;

        // If we don't have enough card sprites, generate some simple colored cards
        if (cardImages == null || cardImages.Count < totalPairs)
        {
            GenerateSimpleCardSprites(totalPairs);
        }

        // Create card pairs
        for (int i = 0; i < totalPairs; i++)
        {
            for (int j = 0; j < 2; j++) // Create 2 cards for each pair
            {
                GameObject cardObj = Instantiate(cardPrefab, cardGrid);
                cardObj.SetActive(true);

                MemoryCard card = cardObj.GetComponent<MemoryCard>();
                card.Initialize(i, cardImages[i], cardBackSprite);
                card.OnCardSelected += OnCardSelected;

                cards.Add(card);
            }
        }
    }

    private void GenerateSimpleCardSprites(int count)
    {
        cardImages = new List<Sprite>();

        // Create a back card sprite if it doesn't exist
        if (cardBackSprite == null)
        {
            Texture2D backTexture = new Texture2D(100, 100);
            Color backColor = new Color(0.3f, 0.3f, 0.3f);

            for (int x = 0; x < backTexture.width; x++)
            {
                for (int y = 0; y < backTexture.height; y++)
                {
                    backTexture.SetPixel(x, y, backColor);
                }
            }

            backTexture.Apply();

            cardBackSprite = Sprite.Create(
                backTexture,
                new Rect(0, 0, backTexture.width, backTexture.height),
                new Vector2(0.5f, 0.5f)
            );
        }

        // Generate distinct colored card faces
        for (int i = 0; i < count; i++)
        {
            // Create a simple colored sprite
            Texture2D texture = new Texture2D(100, 100);

            // Use HSV color space to get distinct colors
            float hue = (float)i / count;
            Color color = Color.HSVToRGB(hue, 0.8f, 0.8f);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();

            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            cardImages.Add(sprite);
        }
    }

    private void ShuffleCards()
    {
        // Shuffle the cards in the grid
        List<MemoryCard> shuffledCards = cards.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < shuffledCards.Count; i++)
        {
            shuffledCards[i].transform.SetSiblingIndex(i);
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
                // First half of the flip - shrink horizontally
                float scale = 1 - normalizedTime * 2;
                card.transform.localScale = new Vector3(scale, 1, 1);
            }
            else
            {
                // Second half of the flip - expand horizontally and show the correct face
                if (normalizedTime >= 0.5f && normalizedTime <= 0.51f)
                {
                    card.SetFaceUp(faceUp);
                }

                float scale = (normalizedTime - 0.5f) * 2;
                card.transform.localScale = new Vector3(scale, 1, 1);
            }

            yield return null;
        }

        // Ensure card is properly scaled and flipped
        card.transform.localScale = Vector3.one;
        card.SetFaceUp(faceUp);
    }

    private IEnumerator CheckForMatch()
    {
        bool isMatch = firstSelectedCard.CardId == secondSelectedCard.CardId;

        // Wait for the second card to finish flipping
        yield return new WaitForSeconds(matchCheckDelay);

        if (isMatch)
        {
            firstSelectedCard.SetMatched(true);
            secondSelectedCard.SetMatched(true);

            matchesFound++;
            UpdateMatchesText();

            // Check if game is complete
            if (matchesFound >= totalPairs)
            {
                ShowCompletionPanel();
            }
        }
        else
        {
            // Flip both cards back
            StartCoroutine(FlipCard(firstSelectedCard, false));
            StartCoroutine(FlipCard(secondSelectedCard, false));
        }

        firstSelectedCard = null;
        secondSelectedCard = null;

        // Small delay before allowing more card selections to prevent rapid clicking issues
        yield return new WaitForSeconds(0.1f);
        canSelect = true;
    }

    private void UpdateMatchesText()
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

            // Find and set up the continue button event
            Button continueButton = completionPanel.GetComponentInChildren<Button>();
            if (continueButton != null)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(CloseGame);
            }
        }
    }

    public void CloseGame()
    {
        gameCanvas.SetActive(false);
    }
}

// Card script - completely revised
public class MemoryCard : MonoBehaviour
{
    public event System.Action<MemoryCard> OnCardSelected;

    public int CardId { get; private set; }
    public bool IsMatched { get; private set; }

    private Image cardImage;
    private GameObject cardBackObject;
    private Image cardBackImage;

    private Sprite frontSprite;
    private Sprite backSprite;
    private bool isFaceUp;

    private void Awake()
    {
        // Set up components if not already assigned
        cardImage = GetComponent<Image>();

        cardBackObject = transform.Find("Card Back")?.gameObject;
        if (cardBackObject != null)
        {
            cardBackImage = cardBackObject.GetComponent<Image>();
        }

        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => {
            if (OnCardSelected != null)
                OnCardSelected(this);
        });
    }

    public void Initialize(int id, Sprite frontSprite, Sprite backSprite)
    {
        this.CardId = id;
        this.frontSprite = frontSprite;
        this.backSprite = backSprite;
        this.IsMatched = false;

        // Set the card front color/sprite
        if (cardImage != null)
        {
            cardImage.sprite = frontSprite;
        }

        // Set the card back sprite
        if (cardBackImage != null)
        {
            cardBackImage.sprite = backSprite;
        }

        // Ensure the card starts face down
        SetFaceUp(false);
    }

    public void SetFaceUp(bool faceUp)
    {
        isFaceUp = faceUp;

        if (cardBackObject != null)
        {
            cardBackObject.SetActive(!faceUp);
        }
    }

    public void SetMatched(bool matched)
    {
        IsMatched = matched;

        if (matched)
        {
            // Visual indication that card is matched - make it slightly transparent
            cardImage.color = new Color(1f, 1f, 1f, 0.7f);

            // Disable button
            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.interactable = false;
            }
        }
    }
}