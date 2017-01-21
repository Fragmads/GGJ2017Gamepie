using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePool : MonoBehaviour {

	#region Singleton

	public static WavePool Instance{
		get {
			return (WavePool.instance);
		}
	}
	private static WavePool instance;

	public void Awake(){

		if(WavePool.instance == null){
			WavePool.instance = this;
			GameObject.DontDestroyOnLoad(this.transform.root);

		}
		else{
			GameObject.Destroy(this.gameObject);
		}

	}

	#endregion


	#region Properties

	[Header ("WavePool")]
	[SerializeField]
	public int NumberOfWaveByImpulse = 8;
	[SerializeField]
	private int baseWaves = 10;
	[SerializeField]
	private Wave WavePrefab;

	public Transform Holder;

	// Lists
	private List<List<Wave>> waveObjects = new List<List<Wave>>();
	private List<List<Wave>> availableWave = new List<List<Wave>>();
	private List<List<Wave>> usedWaves = new List<List<Wave>>();



	#endregion

	public void Start(){

		this.Holder = new GameObject("Holder").transform;
		this.Holder.transform.SetParent(this.transform);

		// Put the prefabs away
		this.Holder.transform.position = Vector3.one * 9999f;

		// Create some List of waves prefabs
		for(int i=0; i< this.baseWaves; ++i){
			this.AddWaveImpulse();
		}


	}

	private void AddWaveImpulse(){

		List<Wave> waves = new List<Wave>();

		// Create 8 wave by impulse
		for(int k=0; k<this.NumberOfWaveByImpulse; ++k){

			Wave w = GameObject.Instantiate(this.WavePrefab);

			w.transform.SetParent(this.Holder);
			w.transform.localPosition = Vector3.zero;

			// Be sure that the wave is correctly orientated
			w.transform.Rotate(new Vector3(0f, 0f, (k/(float)this.NumberOfWaveByImpulse))*360);
			w.SetOrientation(k);

			waves.Add(w);

		}

		// Tell this wave who are it's siblings
		foreach(Wave w in waves){
			w.SetSiblings(waves);
			w.ResetUsed();
		}

		this.waveObjects.Add(waves);
		this.availableWave.Add(waves);

	}

	public List<Wave> GetWaveImpule(){

		// If needed, instantiate some waves
		if(this.availableWave.Count < 1){
			this.AddWaveImpulse();
		}

		if(this.availableWave.Count > 0){
			List<Wave> res = this.availableWave[0];
			this.availableWave.Remove(res);
			this.usedWaves.Add(res);

			return res;
		}

		// Bug case
		return null;
	}

	public void ReturnWaveImpulse(List<Wave> waves){

		this.usedWaves.Remove(waves);

		foreach(Wave w in waves){
			w.ResetUsed();
		}

		this.availableWave.Add(waves);

	}

}
