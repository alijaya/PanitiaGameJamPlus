// External functions
EXTERNAL SetPerson1(personName, movePoint)
EXTERNAL SetPerson2(personName, movePoint)
EXTERNAL StartStory()
EXTERNAL Move(personName, movePoint)
EXTERNAL SwitchCamera(cameraName)
EXTERNAL OverrideCamera(trigger)

// Fallbacks for external functions

=== function SetPerson1(personName, movePoint)
~ return "person1"

=== function SetPerson2(personName, movePoint)
~ return "person2"

=== function StartStory()
~ return

=== function Move(personName, movePoint)
~ return

=== function SwitchCamera(cameraName)
~ return "Main"

=== function OverrideCamera(trigger)
~ return false