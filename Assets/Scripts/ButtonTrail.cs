using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonTrail : MonoBehaviour {

	public Material trailMaterial;

	[HideInInspector]
	public List<Transform> trailPositions = new List<Transform>();
	[HideInInspector]
	public List<Transform> quads = new List<Transform>();

	public Color displayColor;

	private Color _color;
	public Color color{
		get{
			return _color;
		}
		set {
			_color = value;
			foreach(Transform tr in quads){
				tr.renderer.material.color = _color;
			}
		}
	}

	private bool _isOn;
	public bool isOn{
		get{
			return _isOn;
		}
		set{
			if(_isOn==value)return;
			_isOn = value;
			Color _c = color;
			_c.a = (value)?1f:0.4f;
			color = _c;
		}
	}

	private void Start(){
		UpdateQuads();
	}

	[ContextMenu("Reposition points")]
	public void UpdateQuads(){
		if(trailPositions.Count-1!=quads.Count){
			Debug.LogError("What have you done? Something is wrong with lengths");
			return;
		}
		for(int i = 0; i < quads.Count; i++){
			SetQuadTransform(quads[i], trailPositions[i], trailPositions[i+1]);
		}
	}
	
	private void SetQuadTransform(Transform quad, Transform transformStart, Transform transformEnd){
		Vector3 tsPos = transformStart.position;
		Vector3 tePos = transformEnd.position;
		float aMult = (tsPos.y<tePos.y)?-1f:1f;
		float distance = Vector3.Distance(tsPos, tePos)+1f;
		
		quad.position = Vector3.Lerp(tsPos, tePos, 0.5f);
		quad.rotation = Quaternion.AngleAxis(Vector3.Angle(tsPos-tePos, Vector3.right)*aMult, Vector3.forward);
		quad.localScale = new Vector3(distance, 1f, 1f);
		quad.renderer.material.mainTextureScale = new Vector2(Mathf.Round(distance), 1f);
	}

#if UNITY_EDITOR
	[ContextMenu("Reset this Shit")]
	private void Setup(){
		trailPositions = new List<Transform>();
		quads = new List<Transform>();
		AddTrail();
		AddTrail();
		AddQuad();
	}

	public void AddTrailPosition(){
		AddTrail();
		AddQuad();
	}

	private Transform AddQuad(){
		Transform quad = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
		quad.parent = transform;
		quad.renderer.material = trailMaterial;
		quads.Add(quad);
		return quad;
	}
	
	private Transform AddTrail(){
		Transform trailPoint = new GameObject("Trail Position").transform;
		trailPoint.parent = transform;
		trailPoint.localPosition = Vector3.zero;
		trailPositions.Add(trailPoint);
		return trailPoint;
	}

	private void OnDrawGizmos(){
		UpdateQuads();
		for(int i = 0; i < trailPositions.Count-1; i++){
			Gizmos.DrawLine(trailPositions[i].position, trailPositions[i+1].position);
		}
	}
	#endif
}
