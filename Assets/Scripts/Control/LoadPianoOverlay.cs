using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPianoOverlay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject parent = GameObject.Find("PianoOverlay");
        if (parent.transform.childCount != 0)
            {
                
                RectTransform prevChildRect = null; 
                foreach(Transform child in parent.transform)
                {
                   RectTransform rect = child.GetComponent<RectTransform>();
                   if(prevChildRect != null) 
                   {
                       rect.localPosition = prevChildRect.localPosition - new Vector3(93,0,0);
                   }

                   prevChildRect = rect;
                }
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
