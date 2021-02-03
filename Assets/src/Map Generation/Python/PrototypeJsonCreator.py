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
    floorTypes = {}

    for root, subdirs, f in os.walk(path):
        if(subdirs != []):
            for dir in subdirs:
                floorTypes[dir] = []

        for file in f:
            if file.endswith(".png"):
                files.append(root + "\\" + file)

    dictionary = {}
    thisFloorType = ""

    #Go through the images and get the pixels in the corners and centers
    for file in files:
        thisSprite = Image.open(file)
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

        prtototype_type = file.split("\\")[-2]
        filename = file.split("\\")[-1]

        floorTypes[prtototype_type].append(filename)

        prototypename = os.path.splitext(filename)[0]

        if not prtototype_type in dictionary:
            dictionary[prtototype_type] = {}

        dictionary[prtototype_type][prototypename] = {
            'sprite' : file.split("\\", 1)[1],
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

    for subDict, _ in dictionary.items():
        for this_prototype, _ in dictionary[subDict].items():
            neighbor_list = {}
            for i in directions:
                neighbor_list[i] = []
            for other_prototype, _ in dictionary[subDict].items():
                if(this_prototype != other_prototype):
                    for idx, _ in enumerate(directions):
                        if(dictionary[subDict][this_prototype]["sockets"][directions[idx]] == dictionary[subDict][other_prototype]["sockets"][directions_reverse[idx]]):
                            neighbor_list[directions[idx]].append(other_prototype)

            dictionary[subDict][this_prototype]["neighbor_list"] = neighbor_list
        

    with open('..\\Resources\\Prototypes.json', 'w') as outfile:
        json.dump(dictionary, outfile, indent=4)

    with open('..\\MapGeneration.cs', 'w') as outfile:
        outfile.write("using System.Collections;\n")
        outfile.write("using System.Collections.Generic;\n")
        outfile.write("using UnityEngine;\n")
        outfile.write("using System.Text.Json;\n")
        outfile.write("using System.Text.Json.Serialization;\n")
        outfile.write("\n\n")
        outfile.write("namespace MapGeneration\n")
        outfile.write("{\n")

        outfile.write("\tpublic static class Globals\n")
        outfile.write("\t{\n")
        outfile.write("\t\tpublic static readonly int NUM_FLOOR_TYPES = " + str(len(floorTypes.keys())) + ";\n")
        outfile.write("\t\tpublic static readonly int[] NUM_PIECES_PER_FLOOR = {")

        idx = 0
        string = ""

        for k, v in floorTypes.items():
            string = str(len(v))
            if idx != len(floorTypes.keys()) - 1:
                string += ", "
            idx += 1
            outfile.write(string)
        
        outfile.write("};\n")

        outfile.write("\t}\n\n")

        outfile.write("\tpublic class JSON_Data\n")
        outfile.write("\n")
        outfile.write("\t\tpublic 


        outfile.write("\t//Direction enum\n")
        outfile.write("\tpublic enum Direction : int\n")
        outfile.write("\t{\n")

        idx = 0
        string = ""

        for dir in directions:
            string = "\t\t" + dir + " = " + str(idx) + ",\n"
            idx = idx + 1
            outfile.write(string)
        
        outfile.write("\t\tDIRECTION_LENGTH = " + str(len(directions)) + "\n")
        outfile.write("\t}\n\n\n")

        outfile.write("\t//Reverse Direction enum\n")
        outfile.write("\tpublic enum Reverse_Direction : int\n")
        outfile.write("\t{\n")

        idx = 0
        string = ""
        for dir in directions_reverse:
            string = "\t\t" + dir + " = " + str(idx) + ",\n"
            idx = idx + 1
            outfile.write(string)
        
        outfile.write("\t\tREVERSE_DIRECTION_LENGTH = " + str(len(directions_reverse)) + "\n")
        outfile.write("\t}\n\n")

        outfile.write("}")



