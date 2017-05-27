# PetID - Custom Vision in Xamarin.Forms
This simple app demonstrates use of the Azure Cognitive Services Custom Vision APIs located at [customvision.ai](http://www.customvision.ai).

It's simple: take a picture of a pet, get a prediction back of which pet it is.

See a [video](https://www.dropbox.com/s/zsvcrodotnw9iq5/customvision.mov?dl=0) of this app in action.

## How does it work? 
I took about 20 photos each of our three family pets: Ginger, a tan cat, Pepper, a white and black cat, and Mochi, a white dog. I uploaded and tagged these images in the Custom Vision portal.

The customvision.ai website uses the same RESTful APIs behind the scenes that are available in the SDK. This makes it extremely easy to consume.

The prediction code is in the iOS project under PredictionService.cs

## What do I do to get started?
* Familiarize yourself with this [sample code](https://github.com/Microsoft/Cognitive-CustomVision-Windows/blob/master/Samples/CustomVision.Sample/Program.cs) from the product team
* Head to customvision.ai and create a project, then upload & tag photos, and train the system
* Grab your Training API Key and Prediction API Key and put them in APIKeys.cs

## Limitations
* iOS only for now (Droid project is unloaded).
* The Cognitive Services Custom Vision NuGets are not PCL yet, so I have created a DependencyService that wraps these APIs for iOS and exposes them to Forms.
* The code will just pick your first project from customvision.ai. It assumes you have only one project.  
* The UI is extremely barebones

## Special Thanks
* @pierceboggan's [Moments app](https://github.com/pierceboggan/Moments) from which I stole the iOS custom renderer for the camera view

## Next steps
* Add Android support
* Make UI a little better
* Add photo library support and UI test
* Clean up README
* Add vidoes and screenshots to README
