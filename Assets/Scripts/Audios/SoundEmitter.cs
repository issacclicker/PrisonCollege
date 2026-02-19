using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class SoundEmitter : MonoBehaviour
{
    private AudioSource _audioSource;
    private SoundManager _pool;
    private static bool _isAppQuitting = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    private void OnApplicationQuit()
    {
        _isAppQuitting = true;
    }

    public void Initialize(SoundManager pool)
    {
        _pool = pool;
        SoundManager.Instance.OnPauseChanged -= HandlePauseChanged;
        SoundManager.Instance.OnPauseChanged += HandlePauseChanged;
        if (SoundManager.Instance.IsPaused)
        {
            _audioSource.Pause();
        }
    }



    private void HandlePauseChanged(bool isPaused)
    {
        if (isPaused) _audioSource.Pause();
        else _audioSource.UnPause();
    }


    public void Play(AudioClip clip, float pitch, float volume, Vector3 position, bool is3D, bool persistBetweenScenes, bool isLoop)
    {
        transform.position = position;
        _audioSource.clip = clip;
        _audioSource.pitch = pitch;
        _audioSource.loop = isLoop;

        // 여기서 개별 볼륨을 설정합니다 (0.0 ~ 1.0)
        _audioSource.volume = volume;

        _audioSource.spatialBlend = is3D ? 1.0f : 0.0f;

        if (persistBetweenScenes)
        {
            transform.SetParent(_pool.transform);
        }
        else
        {
            transform.SetParent(null);
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(gameObject, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        _audioSource.Play();
        if (SoundManager.Instance.IsPaused)
        {
            _audioSource.Pause();
        }
        if (!isLoop)
        {
            StartCoroutine(ReturnAfterFinish(clip.length));
        }
    }

    private IEnumerator ReturnAfterFinish(float duration)
    {
        // 타임스케일 영향 없이 실제 시간 기준으로 대기
        yield return new WaitForSecondsRealtime(duration);

        _audioSource.Stop();
        transform.SetParent(_pool.transform); // 풀로 돌아갈 땐 다시 매니저 자식으로
        _pool.ReturnToPool(this);
    }



    public void StopAndReturn()
    {
        if (_isAppQuitting) return;
        //if (SoundManager.Instance != null)
        //    SoundManager.Instance.OnPauseChanged -= HandlePauseChanged;
        StopAllCoroutines(); // 진행 중인 ReturnAfterFinish 코루틴 중단
        _audioSource.Stop();
        _audioSource.loop = false;
        _audioSource.clip = null;
        transform.SetParent(_pool.transform);
        _pool.ReturnToPool(this);
    }
}