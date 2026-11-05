using UnityEngine;

namespace BatteryRun.Managers
{
    public class CheckpointManager : MonoBehaviour
    {
        public static CheckpointManager Instance { get; private set; }

        [Header("Active Checkpoint")]
        public Vector3 activeCheckpointPosition;
        public Quaternion activeCheckpointRotation;
        public int activeCheckpointIndex = 0;

        private void Awake()
        {
            Instance = this;
            activeCheckpointPosition = transform.position;
            activeCheckpointRotation = transform.rotation;
        }

        public void SetCheckpoint(Vector3 position, Quaternion rotation, int index)
        {
            if (index >= activeCheckpointIndex)
            {
                activeCheckpointIndex = index;
                activeCheckpointPosition = position;
                activeCheckpointRotation = rotation;

                var player = FindObjectOfType<Player.PlayerController>();
                if (player != null)
                {
                    player.SetCheckpoint(position, rotation);
                }
            }
        }
    }
}