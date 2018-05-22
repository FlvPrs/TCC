using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class VignettePulse : MonoBehaviour
{
	PostProcessVolume m_Volume;
	Vignette m_Vignette;

	public Color color;

	[Range(0f, 1f)]
	public float maxIntensity = 1f;
	public float speedModifier = 1f;

	public bool start = false;
	public bool sine;

	void Start()
	{
		m_Vignette = ScriptableObject.CreateInstance<Vignette>();
		m_Vignette.enabled.Override(true);
		m_Vignette.intensity.Override(0f);
		m_Vignette.smoothness.Override (1f);
		m_Vignette.roundness.Override (1f);
		m_Vignette.color.Override (color);

		m_Volume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, m_Vignette);
	}

	void Update()
	{
		if (start) {
			if(sine)
				m_Vignette.intensity.value = Mathf.Sin (Time.realtimeSinceStartup * speedModifier) * maxIntensity;
			else
				m_Vignette.intensity.value = maxIntensity;
		} else {
			m_Vignette.intensity.value = 0f;
		}
	}

	void Destroy()
	{
		RuntimeUtilities.DestroyVolume(m_Volume, true);
	}
}