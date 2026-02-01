using UnityEngine;
using System.Collections;
using Unity.VectorGraphics;
using UnityEngine.SceneManagement;

public class DelayAction : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitAndExecute());
    }

    IEnumerator WaitAndExecute()
    {
        yield return new WaitForSeconds(9f);
       SceneManager.LoadScene("Menu");                
        // execute after 5 seconds
        Debug.Log("Executed after 5 seconds");
    }
}
