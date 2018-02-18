using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;
    
public class IVVoiceManager : MonoBehaviour
{

    IVSpellManager _spell;


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

        _spell = GameObject.Find("Manager").GetComponent<IVSpellManager>();
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
            string transcript = obj.results[0].alternatives[0].transcript;
            Debug.Log(transcript+" 이 맞느냐?");

            AudioClip _latest = _speech.LatestVoice;

            if (_latest != null && IsTranscriptInSpellKeyword(transcript))
            {
                _source.clip = _latest;
                _source.Play();
            }
            
        }
        else {
            Debug.Log("믿음이 부족하구나!");
        }
    }

    bool IsTranscriptInSpellKeyword(string transcript)
    {

        return false;
    }

    private void StopRecordingEventHandler()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
