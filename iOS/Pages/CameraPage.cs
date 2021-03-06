﻿using System;
using CoreGraphics;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using AVFoundation;
using Foundation;
using UIKit;
using PetID.Pages;

/*
 * AVFoundation Reference: http://red-glasses.com/index.php/tutorials/ios4-take-photos-with-live-video-preview-using-avfoundation/
 * Additional Camera Settings Reference: http://stackoverflow.com/questions/4550271/avfoundation-images-coming-in-unusably-dark
 * Custom Renderers: http://blog.xamarin.com/using-custom-uiviewcontrollers-in-xamarin.forms-on-ios/
 * Code mostly lifted from: https://github.com/pierceboggan/Moments/blob/master/Moments%20-%20XAML/Moments.iOS/Pages/CameraPage.cs
 */

[assembly:ExportRenderer(typeof(PetID.CameraPage), typeof(PetID.iOS.CameraPage))]
namespace PetID.iOS
{
    public class CameraPage : PageRenderer
    {
        AVCaptureSession captureSession;
        AVCaptureDeviceInput captureDeviceInput;
        UIButton toggleCameraButton;
        UIButton toggleFlashButton;
        UIView liveCameraStream;
        AVCaptureStillImageOutput stillImageOutput;
        UIButton takePhotoButton;

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            SetupUserInterface ();
            SetupEventHandlers ();

            AuthorizeCameraUse ();
            SetupLiveCameraStream ();
        }

        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

        public async void AuthorizeCameraUse ()
        {
            var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus (AVMediaType.Video);

            if (authorizationStatus != AVAuthorizationStatus.Authorized) {
                await AVCaptureDevice.RequestAccessForMediaTypeAsync (AVMediaType.Video);
            }
        }

        public void SetupLiveCameraStream ()
        {
            captureSession = new AVCaptureSession ();

            var viewLayer = liveCameraStream.Layer;
            var videoPreviewLayer = new AVCaptureVideoPreviewLayer (captureSession) {
                Frame = liveCameraStream.Bounds
            };
            liveCameraStream.Layer.AddSublayer (videoPreviewLayer);

            var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video);
            ConfigureCameraForDevice (captureDevice);
            captureDeviceInput = AVCaptureDeviceInput.FromDevice (captureDevice);

            var dictionary = new NSMutableDictionary();
            dictionary[AVVideo.CodecKey] = new NSNumber((int) AVVideoCodec.JPEG);
            stillImageOutput = new AVCaptureStillImageOutput () {
                OutputSettings = new NSDictionary ()
            };

            captureSession.AddOutput (stillImageOutput);
            captureSession.AddInput (captureDeviceInput);
            captureSession.StartRunning ();
        }

        public async void CapturePhoto ()
        {
            var videoConnection = stillImageOutput.ConnectionFromMediaType (AVMediaType.Video);
            var sampleBuffer = await stillImageOutput.CaptureStillImageTaskAsync (videoConnection);

            // var jpegImageAsBytes = AVCaptureStillImageOutput.JpegStillToNSData (sampleBuffer).ToArray ();
            var jpegImageAsNsData = AVCaptureStillImageOutput.JpegStillToNSData (sampleBuffer);
            // var image = new UIImage (jpegImageAsNsData);
            // var image2 = new UIImage (image.CGImage, image.CurrentScale, UIImageOrientation.UpMirrored);
            // var data = image2.AsJPEG ().ToArray ();

            // SendPhoto (data);
            SendPhoto (jpegImageAsNsData.ToArray ());
        }

        public void ToggleFrontBackCamera ()
        {
            var devicePosition = captureDeviceInput.Device.Position;
            if (devicePosition == AVCaptureDevicePosition.Front) {
                devicePosition = AVCaptureDevicePosition.Back;
            } else {
                devicePosition = AVCaptureDevicePosition.Front;
            }

            var device = GetCameraForOrientation (devicePosition);
            ConfigureCameraForDevice (device);

            captureSession.BeginConfiguration ();
            captureSession.RemoveInput (captureDeviceInput);
            captureDeviceInput = AVCaptureDeviceInput.FromDevice (device);
            captureSession.AddInput (captureDeviceInput);
            captureSession.CommitConfiguration ();
        }

        public void ConfigureCameraForDevice (AVCaptureDevice device)
        {
            var error = new NSError ();
            if (device.IsFocusModeSupported (AVCaptureFocusMode.ContinuousAutoFocus)) {
                device.LockForConfiguration (out error);
                device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
                device.UnlockForConfiguration ();
            } else if (device.IsExposureModeSupported (AVCaptureExposureMode.ContinuousAutoExposure)) {
                device.LockForConfiguration (out error);
                device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
                device.UnlockForConfiguration ();
            } else if (device.IsWhiteBalanceModeSupported (AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance)) {
                device.LockForConfiguration (out error);
                device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
                device.UnlockForConfiguration ();
            }
        }

        public void ToggleFlash ()
        {
            var device = captureDeviceInput.Device;

            var error = new NSError ();
            if (device.HasFlash) {
                if (device.FlashMode == AVCaptureFlashMode.On) {
                    device.LockForConfiguration (out error);
                    device.FlashMode = AVCaptureFlashMode.Off;
                    device.UnlockForConfiguration ();

                    toggleFlashButton.SetBackgroundImage (UIImage.FromFile ("NoFlashButton.png"), UIControlState.Normal);
                } else {
                    device.LockForConfiguration (out error);
                    device.FlashMode = AVCaptureFlashMode.On;
                    device.UnlockForConfiguration ();

                    toggleFlashButton.SetBackgroundImage (UIImage.FromFile ("FlashButton.png"), UIControlState.Normal);
                }
            }
        }

        public AVCaptureDevice GetCameraForOrientation (AVCaptureDevicePosition orientation)
        {
            var devices = AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video);

            foreach (var device in devices) {
                if (device.Position == orientation) {
                    return device;
                }
            }

            return null;
        }

        private void SetupUserInterface ()
        {
            var centerButtonX = View.Bounds.GetMidX () - 35f;
            var topLeftX = View.Bounds.X + 25;
            var topRightX = View.Bounds.Right - 65;
            var bottomButtonY = View.Bounds.Bottom - 85;
            var topButtonY = View.Bounds.Top + 15;
            var buttonWidth = 70;
            var buttonHeight = 70;

            liveCameraStream = new UIView () {
                Frame = new CGRect (0f, 0f, 320f, View.Bounds.Height)
            };

            takePhotoButton = new UIButton () {
                Frame = new CGRect (centerButtonX, bottomButtonY, buttonWidth, buttonHeight)
            };
            takePhotoButton.SetBackgroundImage (UIImage.FromFile ("TakePhotoButton.png"), UIControlState.Normal);

            toggleCameraButton = new UIButton () {
                Frame = new CGRect (topRightX, topButtonY + 5, 35, 26)
            };

            toggleCameraButton.SetBackgroundImage (UIImage.FromFile ("ToggleCameraButton.png"), UIControlState.Normal);

            toggleFlashButton = new UIButton () {
                Frame = new CGRect (topLeftX, topButtonY, 37, 37)
            };
            toggleFlashButton.SetBackgroundImage (UIImage.FromFile ("NoFlashButton.png"), UIControlState.Normal);

            View.Add (liveCameraStream);
            View.Add (takePhotoButton);
            View.Add (toggleCameraButton);
            View.Add (toggleFlashButton);
        }

        private void SetupEventHandlers ()
        {
            takePhotoButton.TouchUpInside += (object sender, EventArgs e) =>  {
                CapturePhoto ();
            };

            toggleCameraButton.TouchUpInside += (object sender, EventArgs e) =>  {
                ToggleFrontBackCamera ();
            };

            toggleFlashButton.TouchUpInside += (object sender, EventArgs e) => {
                ToggleFlash ();
            };
        }

        public async void SendPhoto (byte[] image)
        {
            var resultPage = new ResultPage(image);
            App.Current.MainPage.Navigation.PushModalAsync(resultPage);
            /*
            var navigationPage = new NavigationPage (new DrawMomentPage (image)) {
                BarBackgroundColor = Colors.NavigationBarColor,
                BarTextColor = Colors.NavigationBarTextColor
            };
            */

            //await App.Current.MainPage.Navigation.PushModalAsync (navigationPage, false);
        }
    }
}