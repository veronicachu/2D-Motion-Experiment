# 2D-Motion-Experiment
## Unity/C# code to create a 2-D motion experiment
### University of California, Irvine

### Author: Veronica C. Chu

----

### Overview:
Experimental framework (i.e. blocks of randomly selected trials) for a feature-based attention study, where motion direction (i.e. left, right, up, down) is the target feature. Each experimental trial takes a target object (e.g. cube) and a target direction (e.g. left), then displays the leftward moving cube in an area with other cubes moving in random directions. Within the trial, the number of target leftward moving cubes will increase to create a coherent motion that an attending observer can notice; this can occur multiple times within a trial. At the end of the trial, the observer enters the number of coherent motion events they counted in an input box.

### Experiment Build:
To see program, go into 'Basic Motion SSVEP' folder and run 'Basic Motion SSVEP.exe'.

Instruction:
1. Enter either '1' or '2' when prompted with 'Enter Scene #' 
- Scene 1 adjusts the number of distractor items according to a psychophysical staircase procedure
- Scene 2 does not adjust the number of distractors, and uses either a default distractor number or takes the result of the staircase procedure from Scene 1
2. Hit 'spacebar' key to continue from "Wait for Experimenter Instructions"
3. An arrow pointing to the target movement direction (e.g. arrow points left when target direction is leftward motion) will appear for 2 seconds, then the trial will automatically start
4. Attend to the central square area (i.e. ignore the peripheral flickering moving dots) for the target direction (e.g. leftward motion). There should be a group of circles moving in the target direction and other distractor circles moving in random directions.
5. Count the number how many times the number of target moving circles increase in numbers 
- For example: The trial starts with 10 leftward motion circles and, at a random point during the trial, they can gradually increase over time to 20 leftward motion circles, then gradually decrease over time back to 10.
- This increase and return to the initial number of the target motion circles can occur multiple times within the trial
6. Enter the counted number of times the target motion circles increase/return to normal
7. The correct answer will appear after you give your input
8. Wait 4 seconds and hit the 'spacebar' key 
- This wait time is enforced by the program and was implemented to reduce motion aftereffects from affecting the next trial
9. Repeat the steps above until the end

### Example Clip:
