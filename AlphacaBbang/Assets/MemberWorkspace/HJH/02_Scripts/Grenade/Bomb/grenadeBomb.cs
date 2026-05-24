using UnityEngine;

public class GrenadeBomb : Bomb
{
    [SerializeField] private GrenadeSO grenadeSO;
    protected override GrenadeSO GetGrenade() => grenadeSO;
}