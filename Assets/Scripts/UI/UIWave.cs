using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UIWave : MonoBehaviour
{
    [SerializeField] private Slider _waveSlider;
    [SerializeField] private Image[] dots;
    [SerializeField] private Color defaultColor = Color.gray;
    [SerializeField] private Color activeColor = Color.yellow;
    private float duratiion = 480f;
    private float time = 0f;
    private float[] _checkpoints = new float[] {0f, 120f, 240f, 360f, 480f};

    void Start()
    {
        _waveSlider.minValue = 0f;
        _waveSlider.maxValue = 480f;

        foreach (var dot in dots)
        {
            dot.color = defaultColor;
        }
    }
    private void MoveSlider()
    {
        time += Time.deltaTime;
        if(time > duratiion)
        {
            time = duratiion;
        }
        _waveSlider.value = time;
    }
    private void UpdateDots()
    {
        for (int i = 0; i < _checkpoints.Length; i++)
        {
            if (time >= _checkpoints[i])
            {
                dots[i].color = activeColor;
            }
        }
    }
    void Update()
    {
        MoveSlider();
        UpdateDots();
    }
}
