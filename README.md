# SignToImage

SignToImage is an application that aims to create a PC or VR experience where the user uses hand gestures to create images.

## How to setup the API

In order to generate images in the game, you need to setup a stable diffusion API.

### Host the API on Google Colab
Follow [this link](https://colab.research.google.com/drive/13w8GaqtwClHNFyxI2rxrywUY7cfcCBzj?usp=sharing) and follow the instructions on the page

You'll need a ngrok token. Go to https://ngrok.com/ to create an account and get your token.


### Host the API on your machine
Follow the installation instructions for the stable diffusion webui here :
https://github.com/AUTOMATIC1111/stable-diffusion-webui
In the installation folder, create a file config.json with the following content : 
```json
{
    "show_progress_every_n_steps": 1
}
```
Run the command
```
python launch.py --xformers --api
```
You can use ngrok to get a url accessible from anywhere : 
```
python launch.py --xformers --ngrok YOUR_NGROK_TOKEN --port 7860 --api
```

## Connect the app to the server

To connect the app to the server, you'll need to enter the link of the api (for example the ngrok link).

In the file PlayerDatas, put a line with the url of the server : "url": "https://example.com"

The first image may take more time to generate. To avoid that, you can use the webui to generate one image before using the app.

## Add the 3 files to the game :

To use the app in the best conditions, you need to put the 3 files (GesturesData, PlayerDatas and ReferenceHandsData, which are available in the google drive : https://drive.google.com/drive/folders/1xa1zd5TBMbu1ts0k5knnI5QYolIMGDEU?usp=sharing) in the folder "Android/data/com.DefaultCompany.TestVR/files" of your VR headset.

## How to use the app

### Add, delete and show signs
Type on the keyboard (using your left index finger) the word. Make sure the word is displayed on the panel in front of the keyboard. Then press save and make the sign with your hands.
Registered signs are displayed on the right. The green button let you choose the type of the word (Subject, Place or Other)
Use the red button to delete the sign.
The blue button let you visualize the sign for 10 seconds.

### Special signs
There are signs that have specific purposes. You can register them by giving the right name:
- "sos" : the sign that is used to start and finish a sentence.
- "move" : make the player move when it is used. The player moves in the direction pointed by the right index finger.
- "change" : change the color of the hands and the style of the images that will be generated.

## Generate an image
In order to generate an image, start with the sos sign. A particle effect indicates that you are making a sentence. Use different signs and finish with the sos sign. The image will appear on the canvas.

## Giant canvas
If you move next to the canvas (using the sign "move"), you will be teleported in a scene where the canvas is giant. You can still create new images in that scene.
To go back to the original scene, you just need to move out of the platform you are on.

## Retry the tutorial 
For that, you will need to replace the file PlayerDatas currently in your vr headset by a new version of it, findable in the google drive.

