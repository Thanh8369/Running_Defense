using System.Collections;
using TMPro;
using UnityEngine;

namespace Son.Economy
{
    public class HudToast : MonoBehaviour
    {
        public TMP_Text label;
        [Range(0.5f, 5f)] public float duration = 1.6f;

        Coroutine _co;

        public void Show(string message)
        {
            if (label == null) return;
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(CoShow(message));
        }

        IEnumerator CoShow(string message)
        {
            label.text = message;
            label.gameObject.SetActive(true);
            yield return new WaitForSeconds(duration);
            label.gameObject.SetActive(false);
        }
    }
}
