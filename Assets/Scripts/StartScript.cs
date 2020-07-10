using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("LoadScore", LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
