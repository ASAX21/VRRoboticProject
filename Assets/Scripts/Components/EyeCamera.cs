using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotCommands;
using RobotComponents;


namespace RobotComponents
{
	public class EyeCamera : MonoBehaviour
    {

        public float value = 0;
		public Camera cameraComponent;
   
        public RenderTexture rendTex;
        private Texture2D tex;
        private Rect rect;

        // Default to QQVGA size
        private void Awake()
        {
            rendTex = new RenderTexture(160,120,16);           
        }

        private void Start()
        {
            cameraComponent = GetComponent<Camera>();
            cameraComponent.targetTexture = rendTex;
            tex = new Texture2D(rendTex.width, rendTex.height, TextureFormat.RGB24, false);
            rect = new Rect(0, 0, rendTex.width, rendTex.height);
        }

        // Need to reverse the order of the rows. RoBIOS uses top left
        // corner as (0,0), Unity uses bottom left.
        public byte[] GetBytes()
        {
            byte[] camOut = tex.GetRawTextureData();
            byte[] imgArray = new byte[camOut.Length];

            for (int i = 1; i <= rendTex.height; i++)
            {
                Array.Copy(camOut, camOut.Length - i * (rendTex.width * 3), imgArray, (i - 1) * (rendTex.width * 3), rendTex.width * 3);
            }
            return imgArray;
        }

        // Change the resolution of the renderer
        public void SetResolution(int width, int height)
        {
            rendTex = new RenderTexture(width, height, 16);
            tex.Resize(rendTex.width, rendTex.height, TextureFormat.RGB24, false);
            rect.Set(0, 0, rendTex.width, rendTex.height);
            cameraComponent.targetTexture = rendTex;
        }

        // Returns a string of Width x Height
        public string GetResolution()
        {
            return rect.width + " x " + rect.height;
        }

        // Determine camera output at each frame
		void OnPostRender()
        {
            RenderTexture.active = rendTex;
			tex.ReadPixels( rect, 0, 0, false );
			tex.Apply( false );
            RenderTexture.active = null;
		}
    }
}
