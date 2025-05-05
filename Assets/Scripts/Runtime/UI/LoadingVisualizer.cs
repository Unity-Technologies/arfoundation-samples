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
            m_TokenSource.Cancel();

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
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var localRotation = transform.localRotation;
                    var zAxisOffset = (-k_LoadingVisualizerSpinRateInDegPerSecond * Time.deltaTime) % 360f;
                    localRotation = Quaternion.Euler(
                        localRotation.eulerAngles.x,
                        localRotation.eulerAngles.y,
                        localRotation.eulerAngles.z + zAxisOffset);

                    try
                    {
                        await Awaitable.NextFrameAsync(token);
                    }
                    catch (OperationCanceledException)
                    {
                        // do nothing
                    }

                    transform.localRotation = localRotation;
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
        }

        void OnDisable()
        {
            m_TokenSource.Cancel();
        }
    }
}
