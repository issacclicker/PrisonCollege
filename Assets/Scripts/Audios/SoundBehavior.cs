using UnityEngine;
using static SoundUtils;

public class SoundBehavior : MonoBehaviour
{
    [SerializeField] private SoundData _gruntSD;
    [SerializeField] private SoundData _goodSongSD;
    [SerializeField] private SoundData _badSongSD;

    private SoundEmitterOwner _soundEmitterOwner;

    private Transform _headBone;



    private void Awake()
    {
        _soundEmitterOwner = GetComponent<SoundEmitterOwner>();
        Animator animator = GetComponent<Animator>();
        _headBone = animator.GetBoneTransform(HumanBodyBones.Head);
    }



    public void PlayGrunt()
    {
        _soundEmitterOwner.Play3DSound(_gruntSD, _headBone, true);
    }



    public void PlayGoodSong()
    {
        _soundEmitterOwner.Play3DSound(_goodSongSD, _headBone, false);
    }



    public void PlayBadSong()
    {
        _soundEmitterOwner.Play3DSound(_badSongSD, _headBone, false);
    }


    public void StopSing()
    {
        if (_soundEmitterOwner == null)
            _soundEmitterOwner = GetComponent<SoundEmitterOwner>();
        _soundEmitterOwner.StopSound(_goodSongSD);
        _soundEmitterOwner.StopSound(_badSongSD);
    }
}
