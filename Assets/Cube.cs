using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cube : MonoBehaviour
{
    public VirusInfo virusInfo;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Image>().color = virusInfo.virusColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
