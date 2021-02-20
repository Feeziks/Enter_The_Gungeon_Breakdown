from PIL import Image
import PIL
import os

if __name__ == "__main__":

    #Find the PNGs within a folder
    path = "..\\Resources\\RoomPieces\\"

    for root, subdirs, f in os.walk(path):
        for file in f:
            if file.endswith(".png"):
                #if they dont contain the "_F" tag already
                if "_F" not in file:
                    #create a version flipped around vertical axis
                    img = PIL.Image.open(root + "\\" + file)
                    img = img.transpose(PIL.Image.FLIP_LEFT_RIGHT)
                    #save it with the same name and a "_F" tag
                    img.save(root + "\\" + file.split(".png")[0] + "_F.png")