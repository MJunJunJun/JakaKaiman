using UnityEngine;

public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;

    private float oldPosition;

    void Start()
    {
        oldPosition = transform.position.x;
    }

    void LateUpdate()
    {
        float currentPosition = transform.position.x;

        if (!Mathf.Approximately(currentPosition, oldPosition))
        {
            float delta = oldPosition - currentPosition;

            if (onCameraTranslate != null)
            {
                onCameraTranslate(delta);
            }

            oldPosition = currentPosition;
        }
    }
}