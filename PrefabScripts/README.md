# These are the steps for implementing the prefab scripts found in this folder

## This is the list of the steps to implement prefabs

1. Replace all original scripts with the new scripts provided. These new scripts allow you to turn your robots into prefabs and eliminate the need to drag the robot game objects into other objects.
2. Drag your existing robots from the scene hierarchy into the project library.
3. Unpack all the robots in the scene hierarchy so nothing breaks when you adjust the prefabs.
4. Drag all your prefabs from the project library into the RobotSpawnController and into the designated robot prefab arrays.
5. Under all the cameras ensure the alliance is set, and if it is a secondary camera.
6. Make 4 game objects and drag these into the Blue and Red spawns in the Robot Spawn Controller, their locations are where the robots will spawn.
7. Then if your robot climbs, check the box in the Drive Controller script labeled "Robot Climbs" and add the script "ClimbManager" to each climbing robot.
8. Also for climbing, add the tag "hookCollider" to each climbing trigger for every bot.
9. Lastly, under the robot's Player Input set the SourceDrop instance to be the robot's prefab and for the action select, DriveController.OnSourceDrop
10. You should then be able to delete the game objects in the scene hierarchy, but I would test first and ensure that everything works.

### Tips:
- Everywhere it asks for an alliance or if it is on the red or blue side, make sure that it is correct. That is how the scripts determine what objects to grab.
- Make sure all of your robots have the correct tags applied, Player for Blue, Player2 for Secondary Blue, etc for Red.
