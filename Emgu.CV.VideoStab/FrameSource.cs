﻿//----------------------------------------------------------------------------
//  Copyright (C) 2004-2013 by EMGU. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace Emgu.CV.VideoStab
{
   /// <summary>
   /// A FrameSource that can be used by the Video Stabilizer
   /// </summary>
   public abstract class FrameSource : UnmanagedObject
   {
      private IntPtr _frameBuffer;
      private Capture.CaptureModuleType _captureSource;

      /// <summary>
      /// Get or Set the capture type
      /// </summary>
      public Capture.CaptureModuleType CaptureSource
      {
         get
         {
            return _captureSource;
         }
         set
         {
            _captureSource = value;
         }
      }

      /// <summary>
      /// The unmanaged pointer the the frameSource
      /// </summary>
      public IntPtr FrameSourcePtr;

      /// <summary>
      /// Retrieve the next frame from the FrameSoure
      /// </summary>
      /// <returns></returns>
      public Image<Bgr, Byte> NextFrame()
      {
         if (!VideoStabInvoke.VideostabFrameSourceGetNextFrame(FrameSourcePtr, ref _frameBuffer) || _frameBuffer == IntPtr.Zero)
            return null;

         MIplImage iplImage = (MIplImage)Marshal.PtrToStructure(_frameBuffer, typeof(MIplImage));

         Image<Bgr, Byte> res;
         if (iplImage.NChannels == 1)
         {  //if the image captured is Grayscale, convert it to BGR
            res = new Image<Bgr, Byte>(iplImage.Width, iplImage.Height);
            using (Mat m = CvInvoke.CvArrToMat(_frameBuffer))
            {
               CvInvoke.CvtColor(m, res, Emgu.CV.CvEnum.ColorConversion.Gray2Bgr);
            }
         }
         else
         {
            res = new Image<Bgr, byte>(iplImage.Width, iplImage.Height, iplImage.WidthStep, iplImage.ImageData);
         }

         return res;
      }

      /// <summary>
      /// Release the unmanaged memory associated with this FrameSource
      /// </summary>
      protected override void DisposeObject()
      {
         if (_frameBuffer != IntPtr.Zero)
         {
            CvInvoke.cvReleaseImage(ref _frameBuffer);
         }
         FrameSourcePtr = IntPtr.Zero;
      }
   }
}