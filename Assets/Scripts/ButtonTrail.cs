using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonTrail : MonoBehaviour {

	public Material trailMaterial;

	[HideInInspector]
	public List<Transform> trailPositions = new List<Transform>();
	[HideInInspector]
	public List<Transform> quads = new List<Transform>();

	public Color displayColor = Color.white;

	private Color _color = Color.white;
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
			_isOn = value;
			Color _c = (value)?new Color(color.r, color.g, color.b, 1f):
				new Color(color.r, color.g, color.b, 0.4f);
			color = _c;
		}
	}

	private void Awake(){
		UpdateMaterials();
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
	
	public void UpdateMaterials(){
		foreach(Transform tr in quads){
			tr.renderer.material = trailMaterial;
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
		if(quad.collider) DestroyImmediate(quad.collider);
	}

#if UNITY_EDITOR
	public bool realtimeUpdate = false;
	[ContextMenu("Reset this Shit")]
	private void Setup(){
		if(trailMaterial == null){
			Debug.LogError("No trail material found, exiting function.");
			return;
		}

		for(int i = transform.childCount-1; i>=0; i--){
			DestroyImmediate(transform.GetChild(i).gameObject);
		}

		trailPositions = new List<Transform>();
		quads = new List<Transform>();
		AddTrail();
		AddTrail();
		AddQuad();
	}

	public void AddTrailPosition(){
		if(trailMaterial == null){
			Debug.LogError("No trail material found, exiting function.");
			return;
		}
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
		if(realtimeUpdate)UpdateQuads();
		for(int i = 0; i < trailPositions.Count-1; i++){
			Gizmos.DrawLine(trailPositions[i].position, trailPositions[i+1].position);
		}
	}
	#endif
}
