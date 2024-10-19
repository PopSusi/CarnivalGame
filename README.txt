- Builder Pattern
|-> Instantiates Enemy at spawn point
|-> Loads Enemy Types (a custom scriptable object)
|-> Loads those values into the Enemy

- Observer Pattern
|-> UI listens to Enemy and Player Death
|-> On EnemyDie, increment score
|-> On PlayerDie, pause game, show final menu and the score

- Object Pool Pattern
|-> 10 Bullets are preloaded into a list, and deactivated
|-> On Mouse1 (Left Click), takes the first from the list
|-> With that first, move to player location, and activate causing movement
|-> Deletes after passing y > 15 units
|-> Player is listening for a death event, readds dead object to list of #
# potential bullets
