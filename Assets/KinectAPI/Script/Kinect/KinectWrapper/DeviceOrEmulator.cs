using UnityEngine;
using System.Collections;

public class DeviceOrEmulator : MonoBehaviour {

	public static DeviceOrEmulator singleInstance = null;
	
	public KinectSensor device;
	public KinectEmulator emulator;
	
	public bool useEmulator = false;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);

		if( null == singleInstance )
			singleInstance = this;
		
		if( this != singleInstance )
			Destroy(gameObject);

	}
	
	// Use this for initialization
	void Start () {
	
		if(useEmulator){
			emulator.enabled = true;
		}
		else {
			device.enabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public Kinect.KinectInterface getKinect() {
		if(useEmulator){
			return emulator;
		}
		return device;
	}
}
