using UnityEngine;
using System.Collections;

public class RenderFullscreen : MonoBehaviour {
    public RenderTexture m_texture;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // We are completely ignoring src
        m_texture.filterMode = FilterMode.Point; //Set filtering of the source image to point for hq2x to work
        Graphics.Blit(m_texture, dest/*, mtlUpscale*/); //Upscale the image
    }
}
