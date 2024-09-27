using System;
using Unity.Mathematics;

namespace Entities
{
    public class LocalToWorld : IComponentData, IDisposable
    {
        public float4x4 Value = float4x4.identity;
        public float3 Right => Value.c0.xyz;
        public float3 Up => Value.c1.xyz;
        public float3 Forward => Value.c2.xyz;
        public float3 Position => Value.c3.xyz;
        public quaternion Rotation => math.rotation(math.mulScale(new float3x3(Value), 1 / Scale));
        public float3 Scale
        {
            get
            {
                var scale = new float3(math.length(Right), math.length(Up), math.length(Forward));
                if (math.determinant(new float3x3(Value)) < 0) scale.x = -scale.x;
                return scale;
            }
        }

        public void Dispose()
        {
            Value = float4x4.identity;
        }

        public static implicit operator float4x4(LocalToWorld matrix)
        {
            return matrix.Value;
        }
    }
}
