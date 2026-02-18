using UnityEngine;
using System.Collections;

public class SoundEmitter : MonoBehaviour
{
    private AudioSource _audioSource;
    private SoundManager _pool;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    public void Initialize(SoundManager pool) => _pool = pool;

    public void Play(AudioClip clip, float pitch, float volume, Vector3 position, bool is3D, bool persistBetweenScenes)
    {
        transform.position = position;
        _audioSource.clip = clip;
        _audioSource.pitch = pitch;

        // 여기서 개별 볼륨을 설정합니다 (0.0 ~ 1.0)
        _audioSource.volume = volume;

        _audioSource.spatialBlend = is3D ? 1.0f : 0.0f;

        if (persistBetweenScenes) transform.SetParent(_pool.transform);
        else transform.SetParent(null);

        _audioSource.Play();
        StartCoroutine(ReturnAfterFinish(clip.length));
    }

    private IEnumerator ReturnAfterFinish(float duration)
    {
        // 타임스케일 영향 없이 실제 시간 기준으로 대기
        yield return new WaitForSecondsRealtime(duration);

        _audioSource.Stop();
        transform.SetParent(_pool.transform); // 풀로 돌아갈 땐 다시 매니저 자식으로
        _pool.ReturnToPool(this);
    }
}