using System.Collections;
using UnityEngine;

public abstract class GrenadeBehavior : MonoBehaviour
{
    public abstract IEnumerator Boom(GameObject projectile);
}