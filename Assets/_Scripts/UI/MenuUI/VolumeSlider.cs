using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace TeamPinguim.UI.MatchUI
{
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _sliderNameText;
        [SerializeField]
        private TMP_Text _volumePercentageText;
        [SerializeField]
        private Slider _volumeSlider;
        [SerializeField]
        private Button _testButton;
        [SerializeField]
        private AudioSource _testAudioSource;
        [SerializeField]
        private AudioMixerGroup _grup;
        [SerializeField]
        private AudioMixer _audioMixer;
        [SerializeField]
        private float _defaultVolume = 50f;

        private void Start()
        {
            _testAudioSource.outputAudioMixerGroup = _grup;
            _sliderNameText.text = _grup.name;
            _volumePercentageText.text = $"{(int)_volumeSlider.value}%";
            LoadValues(_defaultVolume);
            OnVolumeSliderValueChanged(_volumeSlider.value);
        }
        private void LoadValues(float defaultVolume)
        {
            _volumeSlider.value = PlayerPrefs.GetFloat(_grup.name, defaultVolume);
        }
        private void SaveValues()
        {
            PlayerPrefs.SetFloat(_grup.name, _volumeSlider.value);
        }
        private void OnEnable()
        {
            _volumeSlider.onValueChanged.AddListener(OnVolumeSliderValueChanged);
            _testButton.onClick.AddListener(OnTestButtonClicked);
        }
        private void OnDisable()
        {
            _volumeSlider.onValueChanged.RemoveListener(OnVolumeSliderValueChanged);
            _testButton.onClick.RemoveListener(OnTestButtonClicked);
        }
        public float ScaleDecibels(float value)
        {
            float scale = 20.0f;
            float divisor = 50.0f;
            if(value <= 0.0f)
            {
                return -80.0f;
            }
            return scale * (float)Math.Log10(value / divisor);
        }
        private void OnTestButtonClicked()
        {
            _testAudioSource.Play();
        }

        private void OnVolumeSliderValueChanged(float value)
        {
            _volumePercentageText.text = $"{(int)value}%";
            _audioMixer.SetFloat(_grup.name, ScaleDecibels(value));
            SaveValues();
        }
    }
}
