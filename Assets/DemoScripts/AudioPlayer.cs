using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource m_source;

    // Start is called before the first frame update
    void Start()
    {
        if (m_source == null)
        {
            Debug.LogError("AudioPlayer: No AudioSource was set. AudioPlayer will be disabled");
            enabled = false;
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == transform.name)
                {
                    if (m_source.clip)
                    {
                        m_source.Play();
                    }
                    else
                    {
                        Debug.LogError("No Audio Clip selected.");
                    }
                }
            }
        }
    }
}
