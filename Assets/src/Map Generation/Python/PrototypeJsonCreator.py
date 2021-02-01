from PIL import Image
import os

if __name__ == "__main__":
    #Get all images in the grand-parent directory
    path = "..\\Resources\\RoomPieces\\"
    
    files = []

    for file in os.listdir(path):
        if file.endswith(".png"):
            files.append(file)

    #Go through the images and get the pixels in the corners and centers
    for file in files:
        thisSprite = Image.open(path + file)
        halfX = thisSprite.size[0] / 2
        halfY = thisSprite.size[1] / 2
        pixels = thisSprite.load()
        #Get the pixels at the points of interest
        north       = pixels[halfX                  , 0]
        northEast   = pixels[thisSprite.size[0] - 1 , thisSprite.size[1] - 1]
        east        = pixels[thisSprite.size[0] - 1 , halfY]
        southEast   = pixels[thisSprite.size[0] - 1 , 0]
        south       = pixels[halfX                  , 0]
        southWest   = pixels[0                      , 0]
        west        = pixels[0                      , halfY]
        northWest   = pixels[0                      , thisSprite.size[1] - 1]
        
