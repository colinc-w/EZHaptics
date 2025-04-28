# EZHaptics
Simple unified Unity haptics for iOS and Android.

## Overview
EZHaptics gives you quick and painless access to the available iOS haptics outlined in the link below, and uses custom vibrations on Android to match the iOS haptics. 

The available haptic responses are: light, medium, heavy, rigid, soft, success, warning, error, and selection change.

https://developer.apple.com/design/human-interface-guidelines/playing-haptics#Notification

#### Implementation is as easy as initializing the HapticPlatform and calling the desired haptic feedback.
Example image: on collision haptics.triggermediumimpact

#### The included EZHaptics.cs gives you quick access to haptics using Unity Events.
Example image: Haptics assigned via Unity Event

---
## How to Install

Open package manager in Unity under Window > Package Manager. Click the [+] button in the upper left and select "Install package from git URL..." Paste the following link: https://github.com/colinc-w/EZHaptics.git 

Example image: Install via git

---
## How to use

### In Unity
Check the included EZHapticsExample scene to see a simple implementation. This can be built to device to test the various haptic patterns available. 

### On Device

#### iOS
Once you've built your project from Unity, open it in XCode. You'll need to ensure that the CoreHaptics Framework is included in your build. Select your project, select your desired build target, under "General" scroll down to "Frameworks, Libraries and Embedded Content". Click the [+] and search for "Haptics". You should see "CoreHaptics.framework". Add this then build as usual. 

#### Android
For Android there is no further work required. Simply build and install and you're good to go.

---
## Contents

### EZHaptics 
EZHaptics - MonoBehaviour with public methods for each haptic. Can be attached to an object and used with buttons, sliders, and other UI elements as shown in the example. The EZHaptics MonoBehaviour also initializes the HapticPlatform on start. You can do this manually with HapticPlatform.Initialize(); in your own script or simply attach this to an active object in your scene. 

### Haptics
Haptics - This is where the haptics are defined for Android, and where the calls are split depending on platform. Once the platform is initialized they can be called anywhere. Feel free to adjust the Android patterns to your liking, but be aware that the iOS haptics are defined by Apple and not customizable with this plugin. 

### HapticPlatform
HapticPlatform - This (in conjunction with the plugins and frameworks) handles communication with Android and iOS. Don't do anything with this. 

### EZHapticsExample Scene
This scene demonstrates a simple implementation of each haptic pattern available as well a couple UI elements with haptics implemented. Feel free to build this to device so you can feel what each option does. 