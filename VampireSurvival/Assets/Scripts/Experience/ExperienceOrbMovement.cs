using UnityEngine;

public class ExperienceOrbMovement : MonoBehaviour
{
    private ExperienceOrbCollector collector;

    void Awake()
    {
        collector = GetComponent<ExperienceOrbCollector>();
    }

    void Update()
    {
        if (collector.IsAttracting)
        {
            Vector2 direction = (collector.PlayerPosition - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(direction * collector.MoveSpeed * Time.deltaTime);
        }
    }
}
