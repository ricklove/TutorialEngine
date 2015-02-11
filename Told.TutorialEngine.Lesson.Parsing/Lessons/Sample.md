% TITLE = Move Cuby Left and Right 

// C-Style //Comments will be ignored by the parser (if they are not indented)
// Steps should be defined to help the user navigate the lesson
# STEP = Find Comment Where to Insert Code

// Each instruction should be marked with a dash
// Separate paragraphs with an extra blank line

- In order to move a Game Object, 
- we have to manipulate the Transform component on that Game Object. 
- The Transform holds the position, rotation, and Scale. 

- Moving an Object is known as "Translating" the Object. 
- The Unity API gives us a transform.Translate function that lets us move an object.  

- First we need to get the player's input.


// The File section will select the file and method of concern
// A FILE section is neccessary before any TEST sections
// This will set the content for that method in the file
// The rest of the files in the project will be based on the default tutorial template
// Code should be indented one tab (and a blank space should be before it)
## FILE = TranslateCuby.cs > Update

    // Get player's input

// The goal will be clearly highlighted for the user to have a clear objective
## GOAL

- Look in the editor and find the comment where we will enter code:

	// Get player's input

## SUMMARY

- Good
- We are ready to enter our code

------------------------------------------------

// Optional dashes "---" (at least 3 dashes) will clearly divide the Steps in the editor


# STEP = Get the Player's Input

// "## LESSON" can be omited if there is no "## FILE" section

- Now we need to enter the following code:

## GOAL

-Enter:

	float h = Input.getAxis(“horizontal”); 

// TEST will check the file content to ensure that the exact text has been inserted
// The tutorial engine will indicate mistakes and help the user enter the exact code
// The tutorial author should not worry about providing hints to help the user enter
// the correct code
## TEST

	// Get player's input
	float h = Input.getAxis(“horizontal”); 

// The explanation will highlight parts of the code to explain what function they provide
// Use the * symbol to mark the part of code to highlight
// Then provide indented statements to explain parts of that code
// Quote actual code in that explanation to call it out
// When the user hovers over a quoted word, it will be highlighted in the code sample and explanations
## EXPLANATION

* float h
    - a "float" is a number with a decimal point
    - "h" is the name of the variable that will hold the value for that number
* Input.getAxis
    - "Input" accesses the Input object that represents all user input
    - The "." dot indicates we want a property or value from the "Input" object
    - "getAxis" is the name of the method (or function) that we will call to get a value
* ("horizontal")
    - The "(" and ")" are used to pass a value to the "getAxis" method
    - '"horizontal"' tells the "getAxis" method which axis value we need
* ;
    - ";" marks the end of every C# statement
* float h = Input.getAxis(“horizontal”);
    - 'Input.getAxis(“horizontal”)' will give us the value of the user's left or right input
    - 'h' will be 1 (positive one) when the user hit's the right arrow
    - 'h' will be -1 (negative one) when the user hit's the left arrow

## SUMMARY

- Great!
- Now, we have the player's input in the variable called h.
- We will use that to move our player object.

------------------------------------------------

# STEP = Move the Player Game Object

- Remember that the "transform" of a game object controls it's position.
- The transform has a "translate" method that makes it easy to move a game object.

- Let's use the "translate" method of the "transform" to move the object.

## GOAL

- Enter:

    transform.translate(Vector3.right * time.deltaTime);

## TEST

	// Get player's input
	float h = Input.getAxis(“horizontal”); 
	transform.translate(Vector3.right * h * time.deltaTime);

## EXPLANATION

* transform.Translate
    - "transform" controls the position, rotation, and sclae (size) of an object
    - The "translate" method is an easy way to move an object
* (Vector3.right * h * time.deltaTime);
    - "(" and ")" surround the values that will be passed to the "translate" method
    - ";" marks the end of the statement
* Vector3.right
    - "Vector3" a vector is like saying '5 meters that way'. It points in a specific direction with a specific distance.
    - "Vector3.right" is the value of the vector that points 1 unit to the right 
* h
    - "h" is the input value that we got from the user's input
* h * time.deltaTime
    - "time.deltaTime" represents the amount of time that has passed since the last frame
    - a frame is a single tick in the game engine
    - games try to run at about 60 fps (60 frames per second) to look good to the user
    - "h * time.deltaTime" reduces the size of the user's input to match an amount that can be applied for a single frame
* Vector3.right * h * time.deltaTime
    - "Vector3.right * h * time.deltaTime" creates a value for moving right according to the user's input for the last frame
    - When "h" is negative, then the user is actually moving left instead of right
* transform.translate(Vector3.right * h * time.deltaTime);
    - all together, this will make our user move left or right the appropriate amount for a single frame

## SUMMARY

- Great job!
- Now the user can control Cuby and make him move left or right.

------------------------------------------------