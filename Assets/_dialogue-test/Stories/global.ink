// External functions
EXTERNAL SetPerson1(personName)
EXTERNAL SetPerson2(personName)
EXTERNAL SwitchCamera(cameraName)
EXTERNAL OverrideCamera(trigger)

// Fallbacks for external functions below

=== function SetPerson1(personName)
~ return "person1"

=== function SetPerson2(personName)
~ return "person2"

=== function SwitchCamera(cameraName)
~ return "Main"

=== function OverrideCamera(trigger)
~ return false