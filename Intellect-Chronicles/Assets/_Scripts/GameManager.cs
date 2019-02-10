using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject DialogPanel;
    public GameObject PersonA;
    public GameObject PersonB;
    public Text Dialogue;
    public GameObject Door;
    public Vector3 Savepoint = new Vector3(25, 2, 1);

    private int dialogueIndex = 0;
    public string[] dialogueText;
    private bool InDialogue, PersonABefore, PersonBBefore;
    private int detectedStage;



    public string[] endDialogueText;
    public GameObject[] endChars;
    public string nextScene;
    private int endDialogueIndex = 0;
    private bool InEndDialogue;

    private void Start()
    {
        PauseGame();
        NextDialogue();
        detectedStage = 0;
    }

    public void endLevel()
    {
        PersonA.SetActive(false);
        if (endDialogueText == null || endChars == null)
        {
            PauseGame();
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            PauseGame();
            foreach (GameObject o in endChars)
            {
                o.SetActive(true);
            }
            DialogPanel.SetActive(true);
            endDialogue();
        }
    }

    public void endDialogue()
    {
        if (endDialogueIndex >= endDialogueText.Length || endDialogueText[endDialogueIndex] == null || endDialogueText[endDialogueIndex] == "")
        {
            Debug.Log("End of Dialogue");
            endDialogueIndex++;
            InEndDialogue = false;
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            InEndDialogue = true;
            Dialogue.text = endDialogueText[endDialogueIndex++];
        }
    }

    int interactableIndex = 0;
    private string[] tempInteract;
    public void interactableText(string[] interactableDialogue)
    {
        PauseGame();
        PersonABefore = PersonA.activeSelf;
        if(PersonB != null)
        {
            PersonBBefore = PersonB.activeSelf;
            PersonB.SetActive(false);
        }
        PersonA.SetActive(true);
        DialogPanel.SetActive(true);
        tempInteract = interactableDialogue;
        nextInteractableText();
    }

    public void nextInteractableText()
    {
        if (tempInteract == null || interactableIndex == tempInteract.Length)
        {
            interactableIndex = 0;
            tempInteract = null;
            PersonA.SetActive(PersonABefore);
            if (PersonB != null)
            {
                PersonB.SetActive(PersonBBefore);
            }
            DialogPanel.SetActive(false);
            ResumeGame();
        }
        else
        {
            Dialogue.text = tempInteract[interactableIndex++];
        }
    }

    int stage = 0;
    public void NextObjective()
    {
        switch (++stage)
        {
            case 1:
                DialogPanel.SetActive(false);
                ResumeGame();
                break;
            case 2:
                PauseGame();
                DialogPanel.SetActive(true);
                PersonB.SetActive(false);
                NextDialogue();
                break;
            case 3:
                DialogPanel.SetActive(false);
                Door.SetActive(false);
                ResumeGame();
                break;
            case 4:
                PauseGame();
                DialogPanel.SetActive(true);
                NextDialogue();
                break;
            case 5:
                DialogPanel.SetActive(false);
                ResumeGame();
                break;
            case 6:
                PauseGame();
                DialogPanel.SetActive(true);
                NextDialogue();
                break;
            case 7:
                ResumeGame();
                SceneManager.LoadScene("Level2");
                break;
            default:
                break;
        }
    }

    private void PauseGame()
    {
        Object[] objects = FindObjectsOfType(typeof(GameObject));
        foreach (GameObject go in objects)
        {
            go.SendMessage("OnPauseGame", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void ResumeGame()
    {
        Object[] objects = FindObjectsOfType(typeof(GameObject));
        foreach (GameObject go in objects)
        {
            go.SendMessage("OnResumeGame", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void NextDialogue()
    {
        if (dialogueIndex >= dialogueText.Length || dialogueText[dialogueIndex] == null || dialogueText[dialogueIndex] == "")
        {
            Debug.Log("End of Dialogue");
            dialogueIndex++;
            InDialogue = false;
            NextObjective();
        }
        else
        {
            InDialogue = true;
            Dialogue.text = dialogueText[dialogueIndex++];
        }
    }

    public void DetectedByEnemy(GameObject player)
    {
        player.transform.SetPositionAndRotation(Savepoint, transform.rotation);
        detectedStage = 3;
        detectionText();


    }

    public void detectionText()
    {
        switch (detectedStage)
        {
            case 3:
                PauseGame();
                PersonABefore = PersonA.activeSelf;
                PersonBBefore = PersonB.activeSelf;
                PersonA.SetActive(true);
                PersonB.SetActive(false);
                DialogPanel.SetActive(true);
                Dialogue.text = "Chester: Mist, ich wurde entdeckt..";
                break;
            case 2:
                Dialogue.text = "Chester: Vlt war ich zu Laut ,ich probiere es nochmal.";
                break;
            case 1:
                PersonA.SetActive(PersonABefore);
                PersonB.SetActive(PersonBBefore);
                DialogPanel.SetActive(false);
                ResumeGame();
                break;
            default:
                break;

        }
        detectedStage--;
    }

    private void Update()
    {
        if (Input.GetKeyDown("space") || Input.GetMouseButtonDown(0))
        {

            if (InEndDialogue)
            {
                endDialogue();

            }
            else if (InDialogue)
            {
                NextDialogue();
            }
            else if (detectedStage != 0)
            {
                detectionText();
            }
            else if (interactableIndex != 0)
            {   
                nextInteractableText();
            }
        }
    }
}
