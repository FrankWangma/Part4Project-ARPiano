using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MidiJack;
using symbol;

namespace summary
{
    public class Change : MonoBehaviour
    {
        //Gets note text object from canvas
        //public GameObject note;

        //Get overlay master
        private SummaryMaster _summaryMaster = SummaryMaster.GetInstance();

        private Symbol _symbol;

        // Start is called before the first frame update
        void Start()
        {
            _symbol = new Note("D", "4");
            _symbol.SetChord(false);
            _symbol.SetDuration("256", "256");
            _symbol.SetType("quarter");
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
            //HashSet to keep track of current notes pressed    
            //Reset the list each time
            //HashSet<int> notesPressed = new HashSet<int>();

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
                    if (!notesPressed.Contains(noteNumber))
                    {
                        updateSheetMusic(noteNumber);
                    }
                    notesPressed.Add(noteNumber);
                }
                else if (noteStatus == release)
                {
                    notesPressed.Remove(noteNumber);
                }
            }
        }

        // Method to display all notes currently being pressed
        public void ChangeText(HashSet<int> notes)
        {
            if (notes.Count == 0)
            {
                //note.GetComponent<TextMeshProUGUI>().text = "0";
            }
            else
            {
                string notesPlaying = "";
                foreach (var note in notes)
                {

                    notesPlaying += " " + note.ToString();
                }
                //note.GetComponent<TextMeshProUGUI>().text = notesPlaying;
            }

        }

        //Method to pass in notes played
        public void updateSheetMusic(int noteNumber)
        {
 
            int octave = (noteNumber - 12) / 12;
            int stepNum = noteNumber - 12 - (octave * 12);
            string step = "";

            switch (stepNum)
            {
                case 0: step = "C"; break;
                case 2: step = "D"; break;
                case 4: step = "E"; break;
                case 5: step = "F"; break;
                case 7: step = "G"; break;
                case 9: step = "A"; break;
                case 11: step = "B"; break;
            }

            ((Note) _symbol).SetOctave(octave.ToString());
            ((Note)_symbol).SetStep(step);

            //Pass in notes played
            _summaryMaster.AddNotePlayed((Note) _symbol);
        }
    }
}
