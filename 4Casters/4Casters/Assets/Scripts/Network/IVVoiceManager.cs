using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;
    
public class IVVoiceManager : MonoBehaviour
{

    [SerializeField]
    GCSpeechRecognition _speech;

    AudioSource _source;


    public bool isRecording = false;

    // Use this for initialization
    void Start()
    {
        _speech = GetComponentInChildren<GCSpeechRecognition>();
        if (_speech == null)
        {
            Debug.Log("Speech Recognition is null; return");
            return;
        }
        _speech.configs[0].defaultLanguage = Enumerators.LanguageCode.ko_KR;
        _speech.RecognitionSuccessEvent += SpeechRecognizedSuccessEventHandler;

        _source = GetComponentInChildren<AudioSource>();
        _source.Stop();
    }

    public void StartRecording()
    {
        _speech.StartRecord(true);
    }

    public void StopRecording()
    {
        _speech.StopRecord();
    }

    private void SpeechRecognizedSuccessEventHandler(RecognitionResponse obj, long requestIndex)
    {
        if (obj != null && obj.results.Length > 0)
        {
            Debug.Log(obj.results[0].alternatives[0].transcript + " 이 맞느냐?");

            
            AudioClip _latest = _speech.LatestVoice;

            if (_latest != null)
            {
                _source.clip = _latest;
                _source.Play();
            }
            
        }
        else {
            Debug.Log("믿음이 부족하구나!");
        }
    }

    private void StopRecordingEventHandler()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
