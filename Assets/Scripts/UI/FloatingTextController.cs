using TMPro;
using UnityEngine;

namespace EntropySyndicate.UI
{
    public class FloatingTextController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private float speed = 45f;
        [SerializeField] private float duration = 0.55f;

        private float _life;

        public void Show(string message, Color color)
        {
            text.text = message;
            text.color = color;
            _life = duration;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (_life <= 0f)
            {
                gameObject.SetActive(false);
                return;
            }

            _life -= Time.deltaTime;
            transform.position += Vector3.up * (speed * Time.deltaTime);
        }
    }
}
