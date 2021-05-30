using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemInfoView : MonoBehaviour
{
    [SerializeField] [Min(0.1f)] private float updateInterval = 1.0f;
    [SerializeField] private Text modelText = null;
    [SerializeField] private Text deviceText = null;
    [SerializeField] private Text osText = null;
    [SerializeField] private Text cpuText = null;
    [SerializeField] private Text gpuText = null;
    [SerializeField] private Text systemMemoryText = null;
    [SerializeField] private Text resolutionText = null;
    [SerializeField] private Text refreshRateText = null;
    [SerializeField] private Text safeAreaText = null;
    [SerializeField] private Text networkText = null;
    [SerializeField] private Text batteryLevelText = null;
    [SerializeField] private Text batteryStateText = null;
    [SerializeField] private Text audioText = null;

    private float playTime_ = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        modelText.text = SystemInfo.deviceModel;
        deviceText.text = SystemInfo.deviceName;
        osText.text = SystemInfo.operatingSystem;
        cpuText.text = String.Format("{0} / {1}cores", SystemInfo.processorType, SystemInfo.processorCount);
        gpuText.text = String.Format("{0} / {1}MB", SystemInfo.graphicsDeviceName, SystemInfo.graphicsMemorySize);
        systemMemoryText.text = String.Format("{0}MB", SystemInfo.systemMemorySize);
        refreshRateText.text = String.Format("{0}Hz", Screen.currentResolution.refreshRate);
        UpdateNetwork();
        UpdateBattery();
        UpdateResolutionText();
        OnAudioConfigurationChanged(true);
    }

    private void Update()
    {
        playTime_ += Time.deltaTime;
        if (playTime_ >= updateInterval)
        {
            playTime_ = 0.0f;
            UpdateNetwork();
            UpdateBattery();

            if (Application.isEditor)
            {
                UpdateResolutionText();
            }
        }
    }

    private void OnEnable()
    {
        AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;
    }

    private void OnDisable()
    {
        AudioSettings.OnAudioConfigurationChanged -= OnAudioConfigurationChanged;
    }

    /// <summary>
    /// �ʐM�󋵂��X�V����
    /// </summary>
    private void UpdateNetwork()
    {
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable: networkText.text = "���ڑ�"; break;
            case NetworkReachability.ReachableViaCarrierDataNetwork: networkText.text = "�L�����A�ڑ�"; break;
            case NetworkReachability.ReachableViaLocalAreaNetwork: networkText.text = "Wifi�ڑ�"; break;
            default: break;
        }
    }

    /// <summary>
    /// �o�b�e���[�����X�V����
    /// </summary>
    private void UpdateBattery()
    {
        if (SystemInfo.batteryStatus == BatteryStatus.Unknown)
        {
            batteryLevelText.text = "�擾���s";
            batteryStateText.text = "�擾���s";
        }
        else
        {
            int batteryLevel = (int)(SystemInfo.batteryLevel * 100.0f);
            if (batteryLevel < 0) { batteryLevel = 0; }
            batteryLevelText.text = String.Format("{0}%", batteryLevel);
            switch (SystemInfo.batteryStatus)
            {
                case BatteryStatus.Charging:
                    batteryStateText.text = "�[�d��";
                    break;
                case BatteryStatus.Discharging:
                case BatteryStatus.NotCharging:
                    batteryStateText.text = "�[�d���Ă��Ȃ�";
                    break;
                case BatteryStatus.Full:
                    batteryStateText.text = "�t���[�d";
                    break;
                default: break;
            }
        }
    }

    /// <summary>
    /// �𑜓x�����X�V����
    /// </summary>
    private void UpdateResolutionText()
    {
        resolutionText.text = String.Format("{0} x {1}", Screen.currentResolution.width, Screen.currentResolution.height);
        float top = (Screen.currentResolution.height - Screen.safeArea.yMax);
        float bottom = Screen.safeArea.yMin;
        float right = Screen.safeArea.xMin;
        float left = (Screen.currentResolution.width - Screen.safeArea.xMax);
        safeAreaText.text = String.Format("[top] {0}px [bottom] {1}px [right] {2}px [left] {3}px", top, bottom, right, left);
    }

    /// <summary>
    /// �I�[�f�B�I�ݒ�
    /// </summary>
    /// <param name="deviceWasChanged"></param>
    void OnAudioConfigurationChanged(bool deviceWasChanged)
    {
        int bufferLength, numBuffers;
        AudioSettings.GetDSPBufferSize(out bufferLength, out numBuffers);
        AudioConfiguration config = AudioSettings.GetConfiguration();
        audioText.text = String.Format("{0}Hz / {1} / {2}samples / {3}buffers", config.sampleRate, config.speakerMode.ToString(), config.dspBufferSize, numBuffers);
    }
}
