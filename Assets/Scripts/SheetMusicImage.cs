using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SheetMusicImage : MonoBehaviour
{
    private Sprite img1;
    public GameObject MyImage;
 
    // Start is called before the first frame update
    void Start()
    {
        img1 = Resources.Load<Sprite>("./output.png");
        Debug.Log(img1);
        MyImage.GetComponent<Image>().sprite = img1;
        Debug.Log("Test script started");
    }
}
