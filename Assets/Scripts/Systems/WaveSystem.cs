using UnityEngine;

public class WaveSystem : PersistentSingleton<WaveSystem>
{
    public enum DayState
    {
        Day,
        Night,
    }

    [System.Serializable]
    public struct WaveEntry
    {
        public BehaviorWeightSet behaviorWeightSet;
        public DayState dayState;
    }

    [Header("Skybox")]
    [SerializeField] private Material _daySkybox;
    [SerializeField] private Material _nightSkybox;
    [Header("Wave Info Entries")]
    [SerializeField] private WaveEntry[] waveEntries;
    [Header("Stat Factors")]
    [SerializeField] private float _nightChaosFactor;
    [SerializeField] private float _nightProjectFactor;

    private int _currentWave = 0;
    private DayState _currentDayState;
    private float _chaosFactor = 0;
    private float _projectFactor = 0;

    public BehaviorWeightSet BehaviorWeightSet => waveEntries[_currentWave - 1].behaviorWeightSet;
    public float ChaosFactor => _chaosFactor;
    public float ProjectFactor => _projectFactor;

     

    public void NewWaveEntered()
    {
        _currentWave++;
        _currentDayState = waveEntries[_currentWave - 1].dayState;
        if (_currentDayState == DayState.Day)
        {
            RenderSettings.skybox = _daySkybox;
            _chaosFactor = 1;
            _projectFactor = 1;
        }
        else
        {
            RenderSettings.skybox = _nightSkybox;
            _chaosFactor = _nightChaosFactor;
            _projectFactor = _nightProjectFactor;
        }
        DynamicGI.UpdateEnvironment();
    }
}