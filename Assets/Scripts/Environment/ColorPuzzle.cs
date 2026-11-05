using System.Collections.Generic;
using UnityEngine;

namespace BatteryRun.Environment
{
    public class ColorPuzzle : MonoBehaviour
    {
        [Header("Pedestals & Gate")]
        public Transform[] pedestals; // 0 = Red, 1 = Green, 2 = Blue
        public GameObject barrierGate;
        public Renderer[] runeGlyphs;

        private List<int> correctSequence = new List<int> { 0, 2, 1 }; // Red -> Blue -> Green
        private List<int> enteredSequence = new List<int>();
        private bool isSolved = false;

        public void StepOnPedestal(int colorIndex)
        {
            if (isSolved) return;

            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            enteredSequence.Add(colorIndex);

            // Light up rune corresponding to this step
            int step = enteredSequence.Count - 1;
            if (step < runeGlyphs.Length && runeGlyphs[step] != null)
            {
                Color c = colorIndex == 0 ? Color.red : (colorIndex == 1 ? Color.green : Color.blue);
                runeGlyphs[step].material.color = c;
                if (runeGlyphs[step].material.HasProperty("_EmissionColor"))
                {
                    runeGlyphs[step].material.EnableKeyword("_EMISSION");
                    runeGlyphs[step].material.SetColor("_EmissionColor", c * 2.5f);
                }
            }

            // Verify
            if (enteredSequence[step] != correctSequence[step])
            {
                // Wrong sequence! Reset puzzle
                ResetPuzzle();
                Managers.LevelManager.Instance?.ShowMessage("Wrong order! Puzzle reset.");
                Audio.SoundManager.Instance?.PlaySFX("ObstacleHit");
                return;
            }

            if (enteredSequence.Count == correctSequence.Count)
            {
                // Solved!
                isSolved = true;
                Managers.LevelManager.Instance?.ShowMessage("Crystal gate unlocked!");
                Audio.SoundManager.Instance?.PlaySFX("DoorUnlock");
                if (barrierGate != null)
                {
                    barrierGate.SetActive(false);
                }
            }
        }

        private void ResetPuzzle()
        {
            enteredSequence.Clear();
            foreach (var rune in runeGlyphs)
            {
                if (rune != null)
                {
                    rune.material.color = Color.gray;
                    rune.material.SetColor("_EmissionColor", Color.black);
                }
            }
        }
    }
}