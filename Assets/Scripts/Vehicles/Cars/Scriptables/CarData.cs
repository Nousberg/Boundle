using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Vehicles.Cars.Scriptables
{
    public class CarData : ScriptableObject
    {
        [field: SerializeField] public float HorsePower { get; private set; }
        [field: SerializeField] public float Fuel { get; private set; }
    }
}
