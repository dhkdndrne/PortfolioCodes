using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCell : Cell 
{
    #if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(transform.position, 0.2f);
	}
#endif
}
