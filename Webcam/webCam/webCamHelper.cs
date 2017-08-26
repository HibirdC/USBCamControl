﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///---
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.Windows.Forms;

namespace Webcam
{
    public class webCamHelper
    {
        #region 
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice WebCam;
        private int pic_Width;       ///--picture width
        private int pic_Height;      ///---picture height
        private PictureBox pictureBox;  ///--preview the image
        private Bitmap bitmap;          ///--save capture
        #endregion

        #region

        public webCamHelper(ref PictureBox pictureBox,int newWidth, int newHeight)
        {
            this.pic_Height = newHeight;
            this.pic_Width = newWidth;
            this.pictureBox = new PictureBox();
            this.pictureBox = pictureBox;
        }
        /*
         * Get device list
         */
        public int GetCamList(ref Dictionary<string,string> dic)
        {
            this.videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            ///---Get device list
            foreach (FilterInfo item in videoDevices)
            {
                dic.Add(item.Name,item.MonikerString);
            }
            return videoDevices.Count;
        }
        ///--Open the Cam
        public void Open(string videoDeviceName = "")
        {
            ///--enum all use cam device
            this.videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (0 == videoDevices.Count)
            {
                ///---no avalid device
                throw new Exception("当前没有检测到摄像头输入设备");
            }
            ///--save device as Name
            string[] videoDevicesName = new string[videoDevices.Count];
            int deviceCount = 0;
            foreach (FilterInfo item in videoDevices)
            {
                videoDevicesName[deviceCount++] = item.Name;
            }
            ///---if request name is null ,use the first one
            if (string.Empty == videoDeviceName)
            {
                this.WebCam = new VideoCaptureDevice(videoDevices[0].MonikerString);
            }
            else
            {
                ///-search request device
                int index = 0;
                for (int i = 0; i < videoDevices.Count; i++)
                {
                    if (videoDeviceName == videoDevicesName[i])
                    {
                        index = i;
                        break;
                    }
                }
                ///---connect request device
                this.WebCam = new VideoCaptureDevice(videoDevices[index].MonikerString); 
            }
            ///--set device default tresolution
            this.WebCam.VideoResolution = this.WebCam.VideoCapabilities[0];
            ///--open
            this.WebCam.Start();
            this.WebCam.NewFrame += new
             NewFrameEventHandler(WebcamNewFrameCallBack);
        }
        ///---Callback
        private void WebcamNewFrameCallBack(object obj, NewFrameEventArgs eventArgs)
        {
            bitmap = (Bitmap)eventArgs.Frame.Clone();
            this.pictureBox.Image = bitmap;
            GC.Collect();
        }

        ///--Capture
        ///--Succ：true. fail：false
        public bool Capture(string filePath)
        {
            if (bitmap != null)
            {
                Zoom(ref this.bitmap, this.pic_Width, this.pic_Height).Save(filePath);
                return true;
            }
            else
            {
                return false;
            }
        }
        ///--Zoom the pic
        private Bitmap Zoom(ref Bitmap bitmap, int new_Width, int new_Height)
        {
            Bitmap newBitmap = new Bitmap(new_Width, new_Width);
            Graphics newGra= Graphics.FromImage(newBitmap);
            ///--set quality
            newGra.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            newGra.DrawImage(bitmap, new Rectangle(0, 0, new_Width, new_Width), new Rectangle(0, 0, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);
            ///--free the newGra
            newGra.Dispose();
            return newBitmap;
        }


        ///---Close
        public void Close()
        {
            if (this.WebCam != null)
            {
                if (this.WebCam.IsRunning)
                {
                    this.WebCam.Stop();
                }
            }
        }
        ///return is running
        public bool isRunning()
        {
            bool ret = false;
            if (this.WebCam != null)
            {
                if (this.WebCam.IsRunning)
                {
                    ret = true;
                }
            }
            return ret;
        }
        #endregion
    }
}
