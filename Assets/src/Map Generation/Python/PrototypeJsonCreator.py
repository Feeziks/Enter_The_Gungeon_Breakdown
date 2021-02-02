from PIL import Image
import os
import json


def convert_list_to_dict(a):
    it = iter(a)
    res_dct = dict(zip(it, it))
    return res_dct


if __name__ == "__main__":
    #Get all images in the grand-parent directory
    path = "..\\Resources\\RoomPieces\\"
    
    files = []

    for file in os.listdir(path):
        if file.endswith(".png"):
            files.append(file)

    dictionary = {}

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

        prototypename = "prototype_" + os.path.splitext(file)[0]

        dictionary[prototypename] = {
            'sprite' : file,
            'sockets' : {
                'north': north,
                'northeast': northEast,
                'east': east,
                'southeast': southEast,
                'south': south,
                'southwest': southWest,
                'west': west,
                'northwest': northWest
            }
        }

    directions =            ["north", "northeast", "east", "southeast", "south", "southwest", "west", "northwest"]
    directions_reverse =    ["south", "southwest", "west", "northwest", "north", "northeast", "east", "southeast"]

    for this_prototype, _ in dictionary.items():
        neighbor_list = {}
        for i in directions:
            neighbor_list[i] = []
        for other_prototype, _ in dictionary.items():
            if(this_prototype != other_prototype):
                for idx, _ in enumerate(directions):
                    if(dictionary[this_prototype]["sockets"][directions[idx]] == dictionary[other_prototype]["sockets"][directions_reverse[idx]]):
                        neighbor_list[directions[idx]].append(other_prototype)
                        
        dictionary[this_prototype]["neighbor_list"] = neighbor_list
        

    with open('test.txt', 'w') as outfile:
        json.dump(dictionary, outfile, indent=4)
    