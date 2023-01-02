INCLUDE global.ink

/* Story writing starts here */

// Initialize actors
~ SetPerson1("Chef")
~ SetPerson2("Karen")

// Initialize first camera
~ OverrideCamera(false)
~ SwitchCamera("Main")

Chef: Hello there!

Karen: Hi there!

Chef: How are <i>you</i> today?

Karen: I'm <big>good</big>, thanks.

~ SwitchCamera("Karen")

# flip: right
Karen: Now <size=1.5em>leave</size> me alone.

~ SwitchCamera("Main")

Chef: Well, goodbye then.

# flip: left
Karen: Farewell. 

# flip: left # moveX: -10
NA

# enableSequence # flip: right # moveX: 10 # flip: left # moveX: 5
NA

/* Story writing ends here */