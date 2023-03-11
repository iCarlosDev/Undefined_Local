using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    [SerializeField] private int damage;

    private void OnDamage()
    {
        transform.root.GetChild(0).SendMessage("TakeDamage", damage);
    }
}
