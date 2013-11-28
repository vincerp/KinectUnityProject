using UnityEngine;
using System.Collections;

public class LavaScript : DangerousObject
{
	protected override void ApplyDamage (Player to)
	{
		if(to.health <= damageDealt){
			SharkManager.instance.ChangeSharkState(SharkManager.SharkState.ATTACK, to.gameObject);
			StartCoroutine("WaitToDie", to);
			return;
		}
		base.ApplyDamage (to);
	}

	private IEnumerator WaitToDie(Player to){
		yield return new WaitForSeconds(4.5f);
		to.ApplyDamage(damageDealt);
	}

	[ContextMenu("Test My Shark")]
	public void PresentShark(){
		SharkManager.instance.ChangeSharkState(SharkManager.SharkState.LOOK_AROUND);
	}
}
