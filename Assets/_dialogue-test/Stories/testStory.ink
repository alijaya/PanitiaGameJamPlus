// External functions - fallbacks at end of document
EXTERNAL SetPerson1(personName)
EXTERNAL SetPerson2(personName)

/* Story writing starts here */

// Initialize actors
~ SetPerson1("Chef")
~ SetPerson2("Karen")

Chef: Hello there!

Karen: Hi there!

Chef: How are <i>you</i> today?

Karen: I'm <big>good</big>, thanks.

# flip: right
Karen: Now <size=1.5em>leave</size> me alone.

Chef: Well, goodbye then.

# flip: left
Karen: Farewell. 

# flip: left # moveX: -10
NA

# enableSequence # flip: right # moveX: 10 # flip: left # moveX: 5
NA

/* Story writing ends here */

// Fallbacks for external functions below

=== function SetPerson1(personName)
~ return "person1"

=== function SetPerson2(personName)
~ return "person2"