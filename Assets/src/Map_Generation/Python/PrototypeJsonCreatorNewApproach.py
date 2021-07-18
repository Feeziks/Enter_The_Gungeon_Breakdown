from PIL import Image
import os
import json

grandParentPath = "..\\Resources\\RoomPieces\\"
grandParentPathForwardSlash = "../Resources/RoomPieces/"

def getAllFilesOfType(dir, fileType):
  ret = []
  for curr_dir, subdirs, files in os.walk(dir):
    for file in files:
      if file.endswith(fileType):
        ret.append(curr_dir + "\\" + file)

  return ret

def seperateFilesByLevel(fileList):
  ret = dict()

  #The level name in each file is the string after Roompieces
  #the input files should be of the form "..\\Resources\\Roompieces\\<Level name is here>\\<individual file name is here>
  for file in fileList:
    levelName = file.split("\\")[3]

    if levelName not in ret.keys():
      ret[levelName] = []
    
    ret[levelName].append(file.split("\\")[-1].split('.')[0])

  return ret

def getPointsOfInterestInPhoto(pixels, width, height):
  ret = []

  w = width - 1
  h = height - 1

  x = [width / 2, w,  w,          w,  width / 2,  0,  0,          0]
  y = [0,         0,  height / 2, h,  h,          h,  height / 2, 0]

  for px, py in zip(x, y):
    ret.append(pixels[px, py])

  return ret

def buildPointsDict(pngDict):
  #We need to iterate by level and determine for each tile within a level what its valid neighbors are in any given direction
  points = dict()
  for level in pngFilesByLevel.keys():
    points[level] = dict()
    for image in pngFilesByLevel[level]:
      imageFile = Image.open(grandParentPath + "\\" + level + "\\" + image + ".png")
      imageWidth = imageFile.size[0]
      imageHeight = imageFile.size[1]
      imagePixels = imageFile.load()

      points[level][image] = getPointsOfInterestInPhoto(imagePixels, imageWidth, imageHeight)
  return points

def buildValidNeighborsDict(pointsDict):
  validNeighbors = dict()
  for level in pointsDict.keys():
    validNeighbors[level] = dict()

    for thisTile in pointsDict[level]:
      #The valid neighbors we make has to be ordered by the directions so we build those keys here
      validNeighbors[level][thisTile] = {'N' : [], 'NE' : [], 'E' : [], 'SE' : [], 'S' : [], 'SW' : [], 'W' : [], 'NW' : []}
      thisTilePoints = pointsDict[level][thisTile]

      for otherTile in pointsDict[level]:
        if otherTile is not thisTile:
          #we have to iterate the list in the reverse direction so we check other tile south vs our north etc
          otherTilePoints = pointsDict[level][otherTile]
          otherTilePoints.reverse()

          for i, direction in enumerate(validNeighbors[level][thisTile].keys()):
            if thisTilePoints[i] == otherTilePoints[i]:
              validNeighbors[level][thisTile][direction].append(otherTile)

  return validNeighbors

def buildClassFile(dict, level, fileText):
  fileText += "\tpublic static class " + level + "\n"
  fileText += "\t{\n"

  fileText += "\t\tstatic public List<Piece> all" + level + "Pieces;\n"
  fileText += "\t\t//---------------------------------------------------------------------------\n"
  fileText += "\t\t//---------- Piece Declerations ---------------------------------------------\n"
  fileText += "\t\t//---------------------------------------------------------------------------\n"
  
  for piece in dict[level].keys():
    fileText += "\t\tpublic static Piece " +  piece + ";\n"


  fileText += "\t\tstatic " + level + "()\n"
  fileText += "\t\t{\n"

  for piece in dict[level].keys():
    fileText += "\t\t\t" + piece + " = new Piece(\"" + piece + "\", \"" + grandParentPathForwardSlash + "/" + level + "/" + piece + "_Prefab.prefab\", ";
    fileText += "new Dictionary<string, List<Piece>>(){ "
    for direction in dict[level][piece].keys():
      fileText += "\n\t\t\t\t{\"" + direction + "\", new List<Piece>(){"
      for i, validNeighbor in enumerate(dict[level][piece][direction]):
        fileText += validNeighbor
        if(i != len(dict[level][piece][direction]) - 1):
            fileText += ", "

      fileText += "}}"
      if direction != "NW":
        fileText += ","

    fileText += "};\n"

  fileText += "\t\t}\n"


  fileText += "\t}\n\n";

  return fileText

def buildLevelPrototypeFiles(dict):
  for level in dict.keys():
    with open(grandParentPath + level + "\\test_stuff.cs", 'w') as file:
      fileText  = "using System.Collections;\n"
      fileText += "using System.Collections.Generic;\n"
      fileText += "using UnityEngine;\n"
      fileText += "\n\n";
      fileText += "namespace MapPieces\n"
      fileText += "{\n"
      fileText = buildClassFile(dict, level, fileText)
      fileText += "}"

      file.write(fileText)

if __name__ == "__main__":
  #Get all of the png images in the grand-parent directory
  #Remember to save the parent directory name as that is the name of the level
  pngFiles = getAllFilesOfType(grandParentPath, ".png")
  pngFilesByLevel = seperateFilesByLevel(pngFiles)
  points = buildPointsDict(pngFilesByLevel)
  validNeighbors = buildValidNeighborsDict(points)
  
  #Create our files based on the dicts
  buildLevelPrototypeFiles(validNeighbors)
