using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class Monitor : MonoBehaviour
{
    [SerializeField] private VideoPlayer _workingVideo;
    [SerializeField] private VideoPlayer _hackingVideo;
    [SerializeField] private VideoPlayer _gamingVideo;
    private MeshRenderer _renderer;
    private VideoPlayer _currentVideo;
    private Dictionary<DisplayState, VideoPlayer> _stateVideoDic = new();



    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _stateVideoDic.Add(DisplayState.Off, null);
        _stateVideoDic.Add(DisplayState.Working, _workingVideo);
        _stateVideoDic.Add(DisplayState.Hacking, _hackingVideo);
        _stateVideoDic.Add(DisplayState.Gaming, _gamingVideo);
        foreach (VideoPlayer video in _stateVideoDic.Values)
        {
            video?.Stop();
        }
        ChangeDisplay(DisplayState.Off);
    }



    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        ShowDisplay(DisplayState.Off);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.W))
    //    {
    //        ShowDisplay(DisplayState.Working);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        ShowDisplay(DisplayState.Hacking);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        ShowDisplay(DisplayState.Gaming);
    //    }
    //}



    public void PauseDisplay()
    {
        _currentVideo?.Pause();
    }



    public void ResumeDisplay()
    {
        _currentVideo?.Play();
    }




    public void ChangeDisplay(DisplayState displayState)
    {
        _currentVideo?.Stop();
        _currentVideo = _stateVideoDic[displayState];
        _currentVideo?.Play();
        _renderer.material.color = _currentVideo != null ? Color.white : Color.black;
    }
}



public enum DisplayState
{
    Off, Working, Hacking, Gaming
}