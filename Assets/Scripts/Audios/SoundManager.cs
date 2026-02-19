using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundManager : PersistentSingleton<SoundManager>
{
    [SerializeField] private GameObject _emitterPrefab;
    [SerializeField] private GameObject _longDistancemitterPrefab;
    [SerializeField] private int _poolSize = 20;
    private Queue<SoundEmitter> _pool = new Queue<SoundEmitter>();
    private bool _isPaused;
    public bool IsPaused => _isPaused;
    public System.Action<bool> OnPauseChanged;


    protected override void Awake()
    {   
        base.Awake();
        for (int i = 0; i < _poolSize; i++) CreateNewEmitter();
    }



    private void Start()
    {
        StartCoroutine(CheckTimeScaleRoutine());
    }



    IEnumerator CheckTimeScaleRoutine()
    {
        while (true)
        {
            float currentTimeScale = Time.timeScale;
            bool shouldPause = Mathf.Approximately(currentTimeScale, 0);

            if (shouldPause != IsPaused)
            {
                _isPaused = shouldPause;
                SetPause(_isPaused);
            }
            yield return new WaitForSecondsRealtime(0f);
        }
    }



    public void SetPause(bool pause)
    {
        _isPaused = pause;
        OnPauseChanged?.Invoke(pause); // 모든 이미터에게 "상태 변했다!"고 한 번만 알림
    }



    private void CreateNewEmitter(bool isLongDistance = false)
    {
        GameObject obj = Instantiate(isLongDistance ? _longDistancemitterPrefab : _emitterPrefab, transform);
        SoundEmitter emitter = obj.GetComponent<SoundEmitter>();
        emitter.Initialize(this);
        obj.SetActive(false);
        _pool.Enqueue(emitter);
    }



    public SoundEmitter PlaySFX(AudioClip clip, Vector3 position, float volume = 1.0f, bool is3D = true, bool persist = false, bool isRandomPitch = true, bool isLoop = false, bool isLongDistance = false)
    {
        if (clip == null) return null;
        if (_pool.Count == 0) CreateNewEmitter();

        SoundEmitter emitter = _pool.Dequeue();
        emitter.gameObject.SetActive(true);

        float pitch = isRandomPitch ? Random.Range(0.9f, 1.1f) : 1f;
        emitter.Play(clip, pitch, volume, position, is3D, persist, isLoop);
        return emitter; 
    }



    public void ReturnToPool(SoundEmitter emitter)
    {
        // 씬 전환 도중에는 풀이 비어있을 수 있어 안전장치 추가
        if (this == null) return;

        emitter.transform.SetParent(transform);
        emitter.gameObject.SetActive(false);
        _pool.Enqueue(emitter);
    }



    // [핵심] 오브젝트가 직접 이미터를 빌려갈 때 쓰는 함수
    public SoundEmitter GetEmitter()
    {
        if (_pool.Count == 0)
        {
            CreateNewEmitter();
        }

        SoundEmitter emitter = _pool.Dequeue();
        emitter.gameObject.SetActive(true);
        return emitter;
    }
}



public static class SoundUtils
{
    public static void PlayScene3DSFX(AudioClip clip, Vector3 position, float volumeMultiplier = 1f, bool isLoop = false, bool isLongDistance = false)
    {
        SoundManager.Instance.PlaySFX(clip, position, volumeMultiplier, true, false, true, isLoop, isLongDistance);
    }



    public static void PlayScene3DSFX(SoundData soundData, Vector3 position, float volumeMultiplier = 1f, bool isLoop = false, bool isLongDistance = false)
    {
        SoundManager.Instance.PlaySFX(soundData.GetRandomClip(out float volume), position, volume * volumeMultiplier, true, false, true, isLoop, isLongDistance);
    }



    public static SoundEmitter PlayOwnedScene3DSFX(SoundData soundData, Vector3 position, bool isRandomPitch, float volumeMultiplier = 1f, bool isLoop = false, bool isLongDistance = false)
    {
        return SoundManager.Instance.PlaySFX(soundData.GetRandomClip(out float volume), position, volume * volumeMultiplier, true, false, isRandomPitch, isLoop, isLongDistance);
    }



    public static void PlayScene2DSFX(AudioClip clip, float volumeMultiplier = 1f, bool isLoop = false)
    {
        SoundManager.Instance.PlaySFX(clip, Vector3.zero, volumeMultiplier, false, false, true, isLoop);
    }



    public static SoundEmitter PlayOwnedScene2DSFX(SoundData soundData, bool isRandomPitch, float volumeMultiplier = 1f, bool isLoop = false)
    {
        return SoundManager.Instance.PlaySFX(soundData.GetRandomClip(out float volume), Vector3.zero, volume * volumeMultiplier, false, false, isRandomPitch, isLoop);
    }



    public static void PlayScene2DSFX(SoundData soundData, float volumeMultiplier = 1f, bool isLoop = false)
    {
        SoundManager.Instance.PlaySFX(soundData.GetRandomClip(out float volume), Vector3.zero, volume * volumeMultiplier, false, false, true, isLoop);
    }



    public static void PlayUISFX(AudioClip clip, float volumeMultiplier = 1f, bool isLoop = false)
    {
        SoundManager.Instance.PlaySFX(clip, Vector3.zero, volumeMultiplier, false, false, false, isLoop);
    }



    public static void PlayUISFX(SoundData soundData, float volumeMultiplier = 1f, bool isLoop = false)
    {
        SoundManager.Instance.PlaySFX(soundData.GetRandomClip(out float volume), Vector3.zero, volume * volumeMultiplier, false, false, false, isLoop);
    }
}