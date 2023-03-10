namespace Realstone
{
    using TMPro;
    using UnityEngine;

    public class FloatingUI : MonoBehaviour
    {
        TextMeshProUGUI text;

        Vector3 delta = new(0, 0.15f, 0);
        float timer = 0f;
        public float duration = 0.5f;

        void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
            gameObject.SetActive(false);
        }

        public void SetStartPosition(Vector3 pos)
        {
            text.rectTransform.position = pos;
        }

        public void SetText(string str, Color color)
        {
            text.color = color;
            text.text = str;
        }

        private void OnEnable()
        {
            timer = 0f;
        }

        void Update()
        {
            timer += Time.deltaTime;
            text.rectTransform.position += delta;
            if (timer > duration)
            {
                GameManager.instance.EnqueueFloatingUI(this.gameObject);
                gameObject.SetActive(false);
            }
        }
    }
}