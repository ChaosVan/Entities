using System;
using Unity.Mathematics;

namespace Entities
{
    public class Rotation : IComponentData, IDisposable
    {
        public quaternion Value = quaternion.identity;

        public void Dispose()
        {
            Value = quaternion.identity;
        }

        public static implicit operator quaternion(Rotation rotation)
        {
            return rotation.Value;
        }
    }
}
