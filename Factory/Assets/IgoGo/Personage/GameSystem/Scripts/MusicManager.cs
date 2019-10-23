using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MusicBox
{
    [Tooltip("Композиция")] public AudioClip clip;
    [Tooltip("Зациклить композицию, пока не будет переключено извне")] public bool loop;
    [Tooltip("Не показывать название композиции, когда начинает играть этот трэк")] public bool hide;
}

public class MusicManager : MyTools {
    [Tooltip("Текст, где будет отображаться название композиции")] public Text audioName;
    [Tooltip("Динамик")] public AudioSource source;
    [Tooltip("Аниматор панельки с текстом")] public Animator anim;
    [Space(10)]
    [Tooltip("Последовательность композций с настройками")] public MusicBox[] musicBoxes;

    public int CurrentBox
    {
        get
        {
            return _currentBox;
        }
        set
        {
            if(_currentBox != value)
            {
                _currentBox = value;
                change = -1;
            }
        }
    }

    [Space(20)]
    [Tooltip("Когда включено, переключать композиции можно указанием номера в поле ниже")] public bool debug;
    [Tooltip("Реботает только при включенном debug")] public int number;

    #region Служебные
    private int _currentBox;
    private sbyte change;
    private float targetVolume;
    private float currentMultiplicator;


    #endregion

    private void Start()
    {
        currentMultiplicator = AudioSettingsPack.musicMultiplicator;
        source.volume = targetVolume = currentMultiplicator;
        ChangeClip(0);
    }
    private void Update()
    {
        if (debug)
        {
            CurrentBox = number;
        }
        SetMusicBox();
        CheckMusic();
    }

    public void AudioUpdate(float value)
    {
        currentMultiplicator = value;
        if(change == 0)
        {
            if(source.volume != currentMultiplicator)
            {
                source.volume = currentMultiplicator;
            }
        }
    }

    private void SetMusicBox()
    {
        if (change > 0)
        {
            source.volume = Mathf.Lerp(source.volume, currentMultiplicator, Time.deltaTime * 10);
            if (Mathf.Abs(currentMultiplicator - source.volume) < 0.05)
            {
                change = 0;
            }
        }
        else if(change < 0)
        {
            source.volume = Mathf.Lerp(source.volume, 0, Time.deltaTime * 10);
            if (Mathf.Abs(0 - source.volume) < 0.05)
            {
                ChangeClip(_currentBox);
            }
        }
    }
    private void CheckMusic()
    {
        if(!source.loop)
        {
            if(!source.isPlaying)
            {
                int next = CurrentBox + 1;
                if(next > musicBoxes.Length - 1)
                {
                    next = 0;
                }
                CurrentBox = next;
            }
        }
    }
    private void ChangeClip(int number)
    {
        if(number < 0 || number > musicBoxes.Length - 1)
        {
            Debug.LogError("MusicManager. Передан некорректный нормер музыкальной заготовки");
        }
        if(source.isPlaying)
        {
            source.Stop();
        }
        source.clip = musicBoxes[number].clip;
        audioName.text = source.clip.name;
        if(!musicBoxes[number].hide)
        {
            anim.SetTrigger("ChangeMusic");
        }
        source.loop = musicBoxes[number].loop;
        targetVolume = currentMultiplicator;
        change = 1;
        source.Play();
    }
}
