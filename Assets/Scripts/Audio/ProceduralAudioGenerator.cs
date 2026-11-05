using System;
using UnityEngine;

namespace BatteryRun.Audio
{
    public static class ProceduralAudioGenerator
    {
        private const int SampleRate = 44100;

        public static AudioClip GenerateProceduralClip(string clipName, bool isMusic)
        {
            int durationSeconds = isMusic ? 8 : 1;
            int totalSamples = SampleRate * durationSeconds;
            float[] data = new float[totalSamples];

            if (isMusic)
            {
                SynthesizeMusicTrack(clipName, data);
            }
            else
            {
                SynthesizeSoundEffect(clipName, data);
            }

            AudioClip clip = AudioClip.Create(clipName, totalSamples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static void SynthesizeMusicTrack(string name, float[] data)
        {
            double beat = 0.45;
            int root = 60; // C4

            if (name.Contains("Level1")) { root = 62; beat = 0.50; }
            else if (name.Contains("Level2")) { root = 57; beat = 0.38; }
            else if (name.Contains("Level3")) { root = 59; beat = 0.60; }
            else if (name.Contains("Level4")) { root = 55; beat = 0.35; }
            else if (name.Contains("Victory")) { root = 64; beat = 0.40; }
            else if (name.Contains("GameOver")) { root = 53; beat = 0.55; }

            int[] melody = { 0, 4, 7, 12, 11, 7, 4, 2, 0, 5, 9, 12, 7, 4, 2, 0 };
            double phase = 0;
            double bassPhase = 0;

            for (int i = 0; i < data.Length; i++)
            {
                double t = (double)i / SampleRate;
                int step = (int)(t / beat) % melody.Length;
                double freq = 440.0 * Math.Pow(2.0, (root + melody[step] - 69) / 12.0);
                double bassFreq = 440.0 * Math.Pow(2.0, (root - 24 - 69) / 12.0);

                phase += 2.0 * Math.PI * freq / SampleRate;
                bassPhase += 2.0 * Math.PI * bassFreq / SampleRate;

                double env = Math.Exp(-(t % beat) * 4.0);
                double sample = Math.Sin(phase) * env * 0.4 + Math.Sin(bassPhase) * 0.3;

                // Subtle drum beat
                double beatTime = t % beat;
                double kick = (beatTime < 0.08) ? Math.Sin(2.0 * Math.PI * 60.0 * beatTime) * Math.Exp(-beatTime * 30.0) * 0.35 : 0;

                data[i] = Mathf.Clamp((float)(sample + kick), -1.0f, 1.0f);
            }
        }

        private static void SynthesizeSoundEffect(string name, float[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                double t = (double)i / SampleRate;
                double s = 0;

                if (name.Contains("Walk"))
                {
                    s = (Math.Sin(2.0 * Math.PI * (300.0 - t * 2000.0) * t) + (UnityEngine.Random.value * 0.4 - 0.2)) * Math.Exp(-t * 50.0);
                }
                else if (name.Contains("Jump"))
                {
                    s = Math.Sin(2.0 * Math.PI * (250.0 + 800.0 * t) * t) * Math.Exp(-t * 8.0);
                }
                else if (name.Contains("Battery") || name.Contains("Pickup"))
                {
                    double f = t < 0.1 ? 523.25 : (t < 0.2 ? 659.25 : 783.99);
                    s = (Math.Sin(2.0 * Math.PI * f * t) + 0.3 * Math.Sin(4.0 * Math.PI * f * t)) * Math.Exp(-t * 6.0);
                }
                else if (name.Contains("Crystal"))
                {
                    s = (Math.Sin(2.0 * Math.PI * 1318.5 * t) + 0.5 * Math.Sin(2.0 * Math.PI * 1975.5 * t)) * Math.Exp(-t * 5.0);
                }
                else if (name.Contains("LowBattery"))
                {
                    double f = (Math.Sin(2.0 * Math.PI * 8.0 * t) > 0) ? 880.0 : 660.0;
                    s = Math.Sin(2.0 * Math.PI * f * t) * 0.6;
                }
                else if (name.Contains("Click"))
                {
                    s = Math.Sin(2.0 * Math.PI * 1400.0 * t) * Math.Exp(-t * 90.0);
                }
                else if (name.Contains("Door") || name.Contains("Unlock"))
                {
                    s = (Math.Sin(2.0 * Math.PI * 440.0 * t) + Math.Sin(2.0 * Math.PI * 659.25 * t)) * 0.4 * Math.Exp(-t * 3.0);
                }
                else if (name.Contains("Hit") || name.Contains("Obstacle"))
                {
                    s = (UnityEngine.Random.value * 2.0 - 1.0) * Math.Exp(-t * 20.0);
                }
                else if (name.Contains("Alert"))
                {
                    s = Math.Sin(2.0 * Math.PI * 880.0 * t) * (Math.Sin(2.0 * Math.PI * 20.0 * t) > 0 ? 0.7 : 0);
                }
                else
                {
                    s = Math.Sin(2.0 * Math.PI * 440.0 * t) * Math.Exp(-t * 4.0);
                }

                data[i] = Mathf.Clamp((float)s, -1.0f, 1.0f);
            }
        }
    }
}