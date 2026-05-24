using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FireBomb : Bomb
{
    [SerializeField] private GrenadeSO grenadeSO;
    protected override GrenadeSO GetGrenade() => grenadeSO;
}