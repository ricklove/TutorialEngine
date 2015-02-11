# Hours

## Hour 1

### 2015-02-08 4:11-5:20

- Create the project
- Create the sample lesson

## Hour 2-3

### 5:21-6:58

- Parse the lesson
- Create StringWithIndex

### 6:59-7:09

- Reintroduce the implemented code

## Hour 4

### 5:15-5:45

- Add TDD

### 5:46-6:28

- Continue implementing parser
- Start Parsing steps

## Hour 5

### 6:28-7:34

- Parse Instructions
- Parse Goal

## Hour 6

### 10:20-11:11

- Test that all LessonSpans will reconstruct document

## Hour 7

### 11:32-12:26

- Reconstruct document with skipped text

## Hour 8

### 13:08-13:41

- Test that all SkippedPreText contains only whitespace or comment lines

### 13:42-14:32

- Parse File 

## Hour 9

### 14:50-15:13

- Fix GOAL parsing
- Parse SUMMARY
- Parse TEST
- Start Parse EXPLANATION

### 15:14-15:20

- Add test: EXPLANATION needs TEST section
- Parse explanation

### 15:21-15:47

- Finish parsing

## Hour 10-11

### 4:15-4:45

- Design Tutorial Engine Architecture

### 4:46-6:01

- Move the Syntax Tree Classes to their own namespace
- Create Lesson Interfaces

## Hour 12

### 6:30-7:00

- Complete lesson tree class properties

### 7:01-7:37

- Define the tutorial engine interfaces

## Hour 13

### 8:45-8:52

- Create UnityTutorialEngine project

### 8:53-9:07

- Import the dll dependency in unity
- Create Editor Window

### 9:08-10:13

- Implement Tutorial Engine

## Hour 14

### 11:25-12:42

- Implement a scrolling view of comments

## Hour 15-16

### 13:30-15:30

- Add a chat head to the comments
- Display groups

## Hour 17-18

### 15:42-16:36

- Next will cause the comments to continue
- Show the goal

### 16:37-17:49

- Handle filesystem
- Create new scene

## Hour 19

### 3:44-4:36

- Separate Everything into appropriate projects

### 4:37-5:11

- Fix File Method Reference


# Tasks

- REFACTOR!
	- Refactor TutorialEngine
	- Refactor UnityTutorialEngine
- Persist editor state: EditorPrefs

## Tutorial Engine Design

- Create the tutorial engine components
	- Tutorial Controller
	- Code Editor Presenter
	- Instruction Presenter
	- [Advanced] Game Preview Presenter
	- File System Presenter

- Supported Scenarios:
	- Unity: Show the instructions in a unity editor window and modify the project files on the desktop  file system.
	- Unity + Remote View: Show the instructions on a device or in browser (still visible in the unity editor window) and modify the project files within unity. Use the LAN to sync the view with unity.
	- Browser: Create a SPA (Single Page App) that will show a simulated code editor and the instructions
	- (Advanced) App (or Browser Plugin): Run the actual game with an in game tutorial where the code simulates modifying the game. (All possible user edits would need to be pre-compiled into the game, and the app would simulate the user code actually affecting the game. - Because iTunes Store does not allow real scripting.)
	- [Sync everything at the same time]: Live sync all these up with the dgi server and the user can open up as many presenters as they want and they will all be synced.

- TutorialController
	- This will run the tutorial and send instructions to the presenters.

- ILessonCodeEditorPresenter

	- Display the code editor. 
	- In a simulated environment, this would show a custom code editor.
	- In a host environment (like Unity), this would open the external editor from the file system.
	- http://docs.unity3d.com/ScriptReference/AssetDatabase.OpenAsset.html


- ILessonInstructionPresenter

	- Display the instructions to the user
	- This may or may not be in the same environment as the file presenter.
	

- IGamePreviewPresenter
	
	- Display the game preview of what the game would be like based on the current state of the code.
	- In Unity, this will cause the game preview to run.
	- In a simulated environment, this depend on a pre-compiled game and will set the game to that state.
	- This limits the game state to only those which are expected by the lesson. (If the user puts in unexpected code, it won't preview.)

- IFileSystemPresenter

	- In Unity this will modify the project files.
	- This would not be needed for a simulated environment.