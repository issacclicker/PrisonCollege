using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundManager : PersistentSingleton<SoundManager>
{
    [SerializeField] private GameObject _emitterPrefab;
    [SerializeField] private int _poolSize = 20;
    private Queue<SoundEmitter> _pool = new Queue<SoundEmitter>();


    protected override void Awake()
    {   
        base.Awake();
        for (int i = 0; i < _poolSize; i++) CreateNewEmitter();
    }



    private void CreateNewEmitter()
    {
        GameObject obj = Instantiate(_emitterPrefab, transform);
        SoundEmitter emitter = obj.GetComponent<SoundEmitter>();
        emitter.Initialize(this);
        obj.SetActive(false);
        _pool.Enqueue(emitter);
    }



    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1.0f, bool is3D = true, bool persist = false, bool isRandomPitch = true)
    {
        if (clip == null) return;
        if (_pool.Count == 0) CreateNewEmitter();

        SoundEmitter emitter = _pool.Dequeue();
        emitter.gameObject.SetActive(true);

        float pitch = isRandomPitch ? Random.Range(0.9f, 1.1f) : 1f;
        emitter.Play(clip, pitch, volume, position, is3D, persist);
    }



    public void ReturnToPool(SoundEmitter emitter)
    {
        // 씬 전환 도중에는 풀이 비어있을 수 있어 안전장치 추가
        if (this == null) return;

        emitter.gameObject.SetActive(false);
        _pool.Enqueue(emitter);
    }
}



public static class SoundUtils
{
    public static void PlayScene3DSFX(AudioClip clip, Vector3 position, float volumeMultiplier = 1f)
    {
        SoundManager.Instance.PlaySFX(clip, position, volumeMultiplier, true, false, true);
    }



    public static void PlayScene3DSFX(SoundData soundData, Vector3 position, float volumeMultiplier = 1f)
    {
        Debug.Log("PlayScene3DSFX");
        SoundManager.Instance.PlaySFX(soundData.GetRandomClip(out float volume), position, volume * volumeMultiplier, true, false, true);
    }



    public static void PlayScene2DSFX(AudioClip clip, float volumeMultiplier = 1f)
    {
        SoundManager.Instance.PlaySFX(clip, Vector3.zero, volumeMultiplier, false, false, true);
    }



    public static void PlayScene2DSFX(SoundData soundData, float volumeMultiplier = 1f)
    {
        SoundManager.Instance.PlaySFX(soundData.GetRandomClip(out float volume), Vector3.zero, volume * volumeMultiplier, false, false, true);
    }



    public static void PlayUISFX(AudioClip clip, float volumeMultiplier = 1f)
    {
        SoundManager.Instance.PlaySFX(clip, Vector3.zero, volumeMultiplier, false, true, false);
    }
}