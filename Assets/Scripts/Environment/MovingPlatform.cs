using UnityEngine;

namespace BatteryRun.Environment
{
    public class MovingPlatform : MonoBehaviour
    {
        public Vector3 moveOffset = new Vector3(0, 0, 8f);
        public float speed = 2.0f;
        public float pauseTime = 1.0f;

        private Vector3 startPos;
        private Vector3 endPos;
        private float progress = 0f;
        private bool movingToEnd = true;
        private float pauseTimer = 0f;

        private void Start()
        {
            startPos = transform.position;
            endPos = startPos + moveOffset;
        }

        private void Update()
        {
            if (Time.timeScale <= 0f) return;

            if (pauseTimer > 0f)
            {
                pauseTimer -= Time.deltaTime;
                return;
            }

            float step = (speed / Mathf.Max(0.1f, moveOffset.magnitude)) * Time.deltaTime;

            if (movingToEnd)
            {
                progress += step;
                if (progress >= 1.0f)
                {
                    progress = 1.0f;
                    movingToEnd = false;
                    pauseTimer = pauseTime;
                }
            }
            else
            {
                progress -= step;
                if (progress <= 0f)
                {
                    progress = 0f;
                    movingToEnd = true;
                    pauseTimer = pauseTime;
                }
            }

            // Smooth ease
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
            transform.position = Vector3.Lerp(startPos, endPos, smoothProgress);
        }
    }
}
