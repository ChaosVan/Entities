using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Samples.Entities.RollSystem
{
	public class RollBehaviour : MonoBehaviour
	{
		public float3 speed = 0;
	
	    // Update is called once per frame
	    void Update()
	    {
	        transform.rotation = math.mul(transform.rotation, quaternion.Euler(math.radians(speed) * Time.deltaTime));
	    }
	}
}
