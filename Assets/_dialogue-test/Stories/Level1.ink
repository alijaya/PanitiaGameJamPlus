INCLUDE global.ink

/*

Camera list:
- Main
- Entrance
- Cashier

Position list:
- Outside
- FrontOfEntrance1
- FrontOfEntrance2
- OutsideCashier
- InsideCashier

Character list:
- Lorenzo
- Guido
- Cop 1
- Cop 2

*/

// Initialize actors
~ SetPerson1("Lorenzo", "Outside")
~ SetPerson2("Guido", "InsideCashier")

// Initialize first camera
~ OverrideCamera(false)
~ SwitchCamera("Entrance")

~ StartStory()

/* Story writing starts here */

~ Move("Lorenzo", "FrontOfEntrance1")

# flip: left
Lorenzo: Phew! That was a close call!

Lorenzo: Good thing these old feet still kicking. Literally.

Lorenzo: Anyway.. I could use some food right now.

Lorenzo: Speaking of food..

~ SwitchCamera("Cashier")

~ Move("Lorenzo", "OutsideCashier")

Lorenzo: Hi, pizza guy! I feel like rewarding myself some pizza after a totally honest day of work.

Lorenzo: Can I have one pepperoni pizza please?

# flip: right
Guido: Sure! One pepperoni pizza, coming right up!

/* Story writing ends here */