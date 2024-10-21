using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueWrapper", order = 1)]
public class DialogueWrapper : ScriptableObject
{
    [SerializeField] private List<DialogueText> dialogue;

    public List<DialogueText> Dialogue { get { return new List<DialogueText>(dialogue); } }
}