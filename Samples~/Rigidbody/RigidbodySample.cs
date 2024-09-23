/*
 * COPYRIGHT © 2024 CHANGYOU.COM LIMITED. ALL RIGHTS RESERVED.
 * 
 * FILENAME:    RigidbodySample.cs
 * TIME:        2024年9月19日 10:20:22
 * AUTHOR:      赵朝凡
 * CONTACT:     zhaochaofan@cyou-inc.com
 * DESCRIPTION: LocalToWorldInfo
 */

using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.Entities.Rigidbody
{
	public class RigidbodySample : MonoBehaviour
	{
		public Transform Info;

		public void AddForce()
		{
			var body = GetComponent<UnityEngine.Rigidbody>();
			body.AddForce(Vector3.up * 10, ForceMode.Impulse);
		}

		public void Reset()
		{
			var body = GetComponent<UnityEngine.Rigidbody>();
			body.velocity = Vector3.zero;
			body.angularVelocity = Vector3.zero;
			body.transform.position = new Vector3(2, 20, 2);
			body.transform.rotation = Quaternion.identity;
		}

		void Update()
		{
			if (Info != null && EntityManager.TryGetEntityByGameObject(this.gameObject, out var entity))
			{
				var matrix = EntityManager.GetComponentData<LocalToWorld>(entity);
				if (matrix != null)
				{
					Info.GetChild(0).GetComponent<Text>().text = matrix.Value.c0.x.ToString("f2");
					Info.GetChild(1).GetComponent<Text>().text = matrix.Value.c0.y.ToString("f2");
					Info.GetChild(2).GetComponent<Text>().text = matrix.Value.c0.z.ToString("f2");
					Info.GetChild(3).GetComponent<Text>().text = matrix.Value.c3.x.ToString("f2");
					Info.GetChild(4).GetComponent<Text>().text = matrix.Value.c1.x.ToString("f2");
					Info.GetChild(5).GetComponent<Text>().text = matrix.Value.c1.y.ToString("f2");
					Info.GetChild(6).GetComponent<Text>().text = matrix.Value.c1.z.ToString("f2");
					Info.GetChild(7).GetComponent<Text>().text = matrix.Value.c3.y.ToString("f2");
					Info.GetChild(8).GetComponent<Text>().text = matrix.Value.c2.x.ToString("f2");
					Info.GetChild(9).GetComponent<Text>().text = matrix.Value.c2.y.ToString("f2");
					Info.GetChild(10).GetComponent<Text>().text = matrix.Value.c2.z.ToString("f2");
					Info.GetChild(11).GetComponent<Text>().text = matrix.Value.c3.z.ToString("f2");
					Info.GetChild(12).GetComponent<Text>().text = matrix.Value.c0.w.ToString("f2");
					Info.GetChild(13).GetComponent<Text>().text = matrix.Value.c1.w.ToString("f2");
					Info.GetChild(14).GetComponent<Text>().text = matrix.Value.c2.w.ToString("f2");
					Info.GetChild(15).GetComponent<Text>().text = matrix.Value.c3.w.ToString("f2");
				}
			}
		}
	}
}
