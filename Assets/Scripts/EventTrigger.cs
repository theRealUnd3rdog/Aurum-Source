using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    public enum EventMode
    {
        trigger,
        update,
    }
    public UnityEvent EventIn;
    public UnityEvent EventOut;
    public EventMode mode;
    public GameObject[] objects;
    public AudioSource eventAudio;
    private Collider myTrigger;
    private bool collided;

    // Start is called before the first frame update
    void Start()
    {
        myTrigger = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == EventMode.update)
        {
            if (collided) EventIn.Invoke();
            if (!collided) EventOut.Invoke();
        }
    }

    public void PlayAudio()
    {
        if(!eventAudio.isPlaying) eventAudio.Play();
    }

    public void GoNextLevel()
    {
        LevelLoader.loaderInstance.LoadNextLevel();
    }

    public void LoadSameLevel()
    {
        LevelLoader.loaderInstance.LoadSameLevel();
    }

    public void EnableObjects()
    {
        foreach(GameObject o in objects)
        {
            o.SetActive(true);
        }
    }

    public void DisableObjects()
    {
        foreach(GameObject o in objects)
        {
            o.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            collided = true;

            if (mode == EventMode.trigger) EventIn.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            collided = false;

            if (mode == EventMode.trigger) EventOut.Invoke();
        }
    }
}
