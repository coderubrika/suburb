using MPUIKIT;
using UnityEngine;

namespace FFA.Battle.UI
{
    public class PlayerHealthIndicator : MonoBehaviour
    {
        [SerializeField] private MPImage image;

        public void SetHealthPercentage(float percentage)
        {
            // 180-180 * value : indicator formula
            image.fillAmount = percentage;
            image.transform.localRotation = Quaternion.Euler(0, 0, GetAngle(percentage));
        }

        private float GetAngle(float percentage)
        {
            percentage = Mathf.Clamp(percentage, 0f, 1f);
            return 180 * (1 - percentage);
        }
    }
}