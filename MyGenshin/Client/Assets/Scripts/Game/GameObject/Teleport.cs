using Services;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;

public class Teleport : MonoBehaviour {

	// Use this for initialization

	public int ID;
	Mesh mesh = null;

	private void OnTriggerEnter(Collider collider)
	{
		PlayerController pc = collider.GetComponent<PlayerController>();
		//Debug.LogFormat("asdfsadf");
		if (pc != null && pc.isActiveAndEnabled)
		{
			Debug.LogFormat("Play enter teleport {0}", this.ID);
			TeleporterDefine td = DataManager.Instance.Teleporters[this.ID];
			
			if (td.LinkTo>0 && DataManager.Instance.Teleporters.ContainsKey(td.LinkTo)){
				MapService.Instance.SendMapTeleport(this.ID);
			}
			else
			{
				Debug.LogWarningFormat("Teleport {0} equal to 0 or not exist ", td.LinkTo);
			}

		}
	}


#if UNITY_EDITOR
	void OnDrawGizmos()
	{

		Gizmos.color = Color.green;
		if (this.mesh != null)
		{
			Gizmos.DrawWireMesh(this.mesh,transform.position + Vector3.up * transform.localScale.y * 0.5f, transform.rotation,transform.localScale);
		}
		UnityEditor.Handles.color = Color.red;
		UnityEditor.Handles.ArrowHandleCap(0, this.transform.position, this.transform.rotation, 1f, EventType.Repaint);
	}

#endif
}
