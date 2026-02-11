using UnityEngine;

public class WaveSystem : PersistentSingleton<WaveSystem>
{
    public enum DayState
    {
        Day,
        Night,
    }

    [SerializeField] private Material _daySkybox;
    [SerializeField] private Material _nightSkybox;
    private DayState _currentDayState;
    private int _currentWave = 0;
    private float _chaosFactor = 0;
    private float _projectFactor = 0;

    public float ChaosFactor => _chaosFactor;
    public float ProjectFactor => _projectFactor;

     

    public void NewWaveEntered()
    {
        _currentWave++;
        if (_currentWave % 2 == 1)
        {
            _currentDayState = DayState.Day;
            RenderSettings.skybox = _daySkybox;
            _chaosFactor = 1;
            _projectFactor = 1;
        }
        else
        {
            _currentDayState = DayState.Night;
            RenderSettings.skybox = _nightSkybox;
            _chaosFactor = 1.5f;
            _projectFactor = 2f;
        }
        DynamicGI.UpdateEnvironment();
    }
}
