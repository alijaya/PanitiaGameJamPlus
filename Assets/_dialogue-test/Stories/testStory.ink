INCLUDE global.ink

// Initialize actors
~ SetPerson1("Chef", "InitialPoint1")
~ SetPerson2("Karen", "InitialPoint2")

// Initialize first camera
~ OverrideCamera(false)
~ SwitchCamera("Main")

~ StartStory()

/* Story writing starts here */

# move: SpeakingPoint1
Chef: Hello there!

# move: SpeakingPoint2
Karen: Hi there!

Chef: How are <i>you</i> today?

Karen: I'm <big>good</big>, thanks.

~ SwitchCamera("Karen")

# flip: right
Karen: Now <size=1.5em>leave</size> me alone.

~ SwitchCamera("Chef")

Chef: Well, goodbye then.

~ SwitchCamera("Main")

# flip: left
Karen: Farewell.

~ Move("Chef", "InitialPoint1")
~ Move("Karen", "InitialPoint2")

/* Story writing ends here */