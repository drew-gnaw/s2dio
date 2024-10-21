using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    private DialogueBox activeDialogueBox;
    [SerializeField]
    private DialogueBox picturelessDialogueBoxComponent;
    [SerializeField]
    private DialogueBox pictureDialogueBox;
    private List<DialogueText> sentences = new();

    private bool inDialogue = false;

    List<DialogueText> dialogueHistory = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        activeDialogueBox = pictureDialogueBox;
        SceneManager.sceneLoaded += OnSceneChange;
    }

    //Manager is going to listen to a bunch of events that then cause it to gain new sentences and 
    void Start()
    {
        ClearPanel();
    }

    public bool IsInDialogue()
    {
        return inDialogue;
    }

    public IEnumerator StartDialogue(List<DialogueText> newSentences)
    {
        if (activeDialogueBox.gameObject.activeInHierarchy)
        {
            yield break;
        }

        activeDialogueBox.gameObject.SetActive(true);

        sentences = newSentences;
        inDialogue = true;

        DisplayNextSentence();
        yield return new WaitUntil(() => !inDialogue);
    }


    void AddLineToHistory(DialogueText text)
    {
        dialogueHistory.Add(text);
    }

    public List<DialogueText> GetHistory()
    {
        return dialogueHistory;
    }


    void ClearPanel()
    {
        activeDialogueBox.gameObject.SetActive(false);
        sentences.Clear();
        inDialogue = false;
    }

    public void MoveBoxToBottom()
    {
        pictureDialogueBox.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
        pictureDialogueBox.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
        pictureDialogueBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        if (picturelessDialogueBoxComponent != null)
        {
            picturelessDialogueBoxComponent.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
            picturelessDialogueBoxComponent.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
            picturelessDialogueBoxComponent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
    }

    public void MoveBoxToTop()
    {
        pictureDialogueBox.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
        pictureDialogueBox.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
        pictureDialogueBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1 * pictureDialogueBox.GetComponent<RectTransform>().rect.height);

        if (picturelessDialogueBoxComponent != null)
        {
            picturelessDialogueBoxComponent.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            picturelessDialogueBoxComponent.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            picturelessDialogueBoxComponent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1 * picturelessDialogueBoxComponent.GetComponent<RectTransform>().rect.height);
        }
    }

    void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        DialogueText sentence = sentences[0];
        sentences.RemoveAt(0); //Uses a list instead of a Queue so we can see it in the Unity Editor

        AddLineToHistory(sentence);

        if (sentence.DisplayingImage == null && picturelessDialogueBoxComponent != null)
        {
            SetDialogueBoxActive(picturelessDialogueBoxComponent);
            picturelessDialogueBoxComponent.SetLine(sentence);

        }
        else
        {
            SetDialogueBoxActive(pictureDialogueBox);
            pictureDialogueBox.SetLine(sentence);
        }
    }

    void SetDialogueBoxActive(DialogueBox dialogueBox)
    {
        if (picturelessDialogueBoxComponent != null)
        {
            picturelessDialogueBoxComponent.gameObject.SetActive(dialogueBox == picturelessDialogueBoxComponent);
            pictureDialogueBox.gameObject.SetActive(dialogueBox == pictureDialogueBox);
            activeDialogueBox = dialogueBox;
        }
        else
        {
            pictureDialogueBox.gameObject.SetActive(true);
            activeDialogueBox = pictureDialogueBox;
        }
    }

    void OnSceneChange(Scene s, LoadSceneMode m)
    {
        EndDialogue();
    }

    void EndDialogue()
    {
        ClearPanel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.RightArrow))
        {
            if (inDialogue)
            {
                if (activeDialogueBox.FinishedLine())
                {
                    DisplayNextSentence();
                }
                else
                {
                    activeDialogueBox.StopScrollingText();
                }
            }
        }
    }
}
