using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MidiJack;

public class TextChangeBehaviour : MonoBehaviour
{
    //Gets note text object from canvas
    public GameObject note;

    // Start is called before the first frame update
    void Start()
    {

    }



    int noteStatus;
    public int noteNumber;

    //Release and press status values for notes
    int release = 128;
    int press = 144;
    //HashSet to keep track of current notes pressed
    HashSet<int> notesPressed = new HashSet<int>();

    // Update is called once per frame
    void Update()
    {

        //Get queue of notes played
        Queue<MidiJack.MidiMessage> myQueue = MidiDriver.Instance.History;

        //Change queue to array so we can get last note pressed or released
        MidiMessage[] messageArray = myQueue.ToArray();

        if (messageArray.Length > 0)
        {
            //Get the last note played (represented as message)
            MidiMessage message = messageArray[messageArray.Length - 1];

            noteNumber = message.data1;
            noteStatus = message.status;

            //If notes status indicates press, add the note to the hashset
            //Otherwise, if it detects the note is released, then remove the note from the hashset
            if (noteStatus == press) 
            { 
                notesPressed.Add(noteNumber); 
            }
            else if (noteStatus == release) 
            { 
                notesPressed.Remove(noteNumber); 
            }

            //Update display to show current notes pressed
            ChangeText(notesPressed);

        }
    }

    // Method to display all notes currently being pressed
    public void ChangeText(HashSet<int> notes){
        if(notes.Count == 0){
            note.GetComponent<TextMeshProUGUI>().text = "0";
        }
        else
        {
            string notesPlaying = "";
            foreach (var note in notes)
            {
                
                notesPlaying += " " + note.ToString();
            }
            note.GetComponent<TextMeshProUGUI>().text = notesPlaying;
        }

    }
}
