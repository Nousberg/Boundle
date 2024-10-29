using Assets.Scripts.Movement.Scriptables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities.Effects.Interfaces
{
    public interface IMovementEffect<T> where T : Effect
    {
        void ModifyMovement(MovementData data, ref float speed, ref float runSpeedBoost, ref float flySpeed, ref float jumpPower);
    }
}
