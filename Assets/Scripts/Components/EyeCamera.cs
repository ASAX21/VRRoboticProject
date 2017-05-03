﻿using System;
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
		public Texture2D tex;

        private RenderTexture rendTex;

        // Default to QQVGA size
        private void Awake()
        {
            rendTex = new RenderTexture(160,120,16);           
        }

        private void Start()
        {
            cameraComponent = GetComponent<Camera>();
            cameraComponent.targetTexture = rendTex;
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
            Debug.Log("Set Resolution: " + width + "x" + height);
            rendTex = new RenderTexture(width, height, 16);
            cameraComponent.targetTexture = rendTex;
        }

        // Determine camera output at each frame
		void OnPostRender()
        {
            RenderTexture.active = rendTex;
			tex = new Texture2D(rendTex.width, rendTex.height, TextureFormat.RGB24, false );
			Rect rect = new Rect( 0, 0, rendTex.width, rendTex.height );
			tex.ReadPixels( rect, 0, 0, false );
			tex.Apply( false );
            RenderTexture.active = null;
		}
    }
}
