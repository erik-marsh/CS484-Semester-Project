# CS484 Semester Project - Study Focuser

Created by Erik Marsh and Sean O'Kelly (Group 20)

## Software and Hardware Dependencies
### For the end user
* HTC Vive or HTC Vive Pro
* A USB webcam
  * To achieve the desired effect, the webcam must have an aspect ratio of 16:9.
  * Additionally, a webcam with a resolution of 1080p and a framerate of at least 30Hz (higher is better) is recommended.
* A USB extension cord is recommended.
* Something to attach the webcam to the front of the headset with
  * Velcro strips, electrical tape, duct tape, etc
### For development
* Unity 2019.4.22f1 or higher
* SteamVR 2.7.3 or higher

## How to use
* Attach the webcam to the head-mounted display of your VR system with the method of your choosing.
* Ensure the webcam will not fall out of place during use.
* Launch the Study Focuser app.

### Within the app
* Turn to face the main menu
* Point a Vive controller towards the "Start" button and use the trigger to select it. You will see a faded view of the real world.
* Locate an area you wish to focus on.
* Determine the shape of the rectangle that would contain this area.
* Use a Vive controller's trigger to place points that represent the corners of this rectangle.
* Press the grip button to focus on this area.
* To leave focus mode, press down on the center, top, or bottom of the trackpad.
* To exit the app, point your controller to the top of your field of view.
  * You will see a timer that tracks how long you have been in user mode for.
    * This timer will turn red after five minutes have passed
  * You will also see a button labeled "Exit." Use the trigger to select it. You will return to the main menu.
  * Select "Exit" on the main menu.

## Bugs and issues
There are a number of things that do not work optimally.
The main issue is that the front-facing cameras of the HTC Vive Pro are low-resolution and difficult to view as a video stream.
This led us to the compromise of using a webcam.
* Due to the limited field of view and resolution of consumer-grade webcams, the camera view does not fill the field of view of the user.
  * This breaks the illusion of AR for the user, and they will see the background of the Unity app.
  * A low-framerate webcam will appear to lag behind the tracking of the head-mounted display as well.
* The way that we used to "cut a hole" in the sphere that blocks out the environment from the user will only cut out an (approximately) square hole.
  * Regardless of the selection, a square plane will be used to "cut the hole," leading to the effect of blocking out less than the user intends to.
* The cutting out of the visible zone is accomplished by changing render priorities, which leads to some ugly rendering anomalies.
  * Objects on the default rendering layer will "smear" across any areas of the skybox that the user can see.
* Some transitions between modes do not fade in or out, which some users may find jarring.
