using System;
using UnityEngine;

namespace AquaGarden.Systems
{
    public sealed class TimeManager : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float gameSecondsPerRealSecond = 1f;
        public double TotalGameSeconds { get; private set; }
        public event Action<double> Advanced;

        private void Update()
        {
            Advance(Time.deltaTime * gameSecondsPerRealSecond);
        }

        public void Advance(double gameSeconds)
        {
            if (gameSeconds <= 0d || double.IsNaN(gameSeconds) || double.IsInfinity(gameSeconds))
                return;
            TotalGameSeconds += gameSeconds;
            Advanced?.Invoke(gameSeconds);
        }

        public void Restore(double totalGameSeconds)
        {
            TotalGameSeconds = Math.Max(0d, totalGameSeconds);
        }
    }
}
