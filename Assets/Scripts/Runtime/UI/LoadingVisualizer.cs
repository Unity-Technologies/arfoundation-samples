using System;
using System.Threading;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class LoadingVisualizer : MonoBehaviour
    {
        const float k_LoadingVisualizerSpinRateInDegPerSecond = 350;

        CancellationTokenSource m_TokenSource = new();

        public void StartAnimating()
        {
            gameObject.SetActive(true);
            if (!m_TokenSource.IsCancellationRequested)
            {
                m_TokenSource.Cancel();
            }

            m_TokenSource = new();
            AnimateLoading(m_TokenSource.Token);
        }

        public void StopAnimating()
        {
            m_TokenSource.Cancel();
            gameObject.SetActive(false);
        }

        async void AnimateLoading(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var rotation = transform.rotation;
                var zAxisOffset = (-k_LoadingVisualizerSpinRateInDegPerSecond * Time.deltaTime) % 360f;
                rotation = Quaternion.Euler(
                    rotation.eulerAngles.x,
                    rotation.eulerAngles.y,
                    rotation.eulerAngles.z + zAxisOffset);

                transform.rotation = rotation;
                try
                {
                    await Awaitable.NextFrameAsync(token);
                }
                catch (OperationCanceledException)
                {
                    // do nothing
                }
            }
        }

        void OnDisable()
        {
            if (!m_TokenSource.IsCancellationRequested)
                m_TokenSource.Cancel();
        }
    }
}
