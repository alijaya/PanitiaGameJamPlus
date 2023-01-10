INCLUDE global.ink

// Initialize actors
~ SetPerson1("Chef", "InitialPoint1")
~ SetPerson2("Karen", "InitialPoint2")

// Initialize first camera
~ OverrideCamera(false)
~ SwitchCamera("Main")

~ StartStory()

/* Story writing starts here */

# move: SpeakingPoint1 # input: 1
Chef: Hello there!

# move: SpeakingPoint2
Karen: Hi there!

# input: 3
Chef: How are you today?

Karen: I'm <big>good</big>, thanks.

// ~ SwitchCamera("Karen")

# flip: right
Karen: Now <size=1.5em>leave</size> me alone.

// ~ SwitchCamera("Chef")

# input: 2
Chef: Well, goodbye then.

// ~ SwitchCamera("Main")

# flip: left
Karen: Farewell.

~ Move("Chef", "InitialPoint1")
~ Move("Karen", "InitialPoint2")

/* Story writing ends here */