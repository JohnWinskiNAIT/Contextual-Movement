# Day 15 Walkthrough: Intermediate Agents

## Demo Purpose

Day 14 agents reacted to current perception. Day 15 introduces intermediate agents that retain useful information and continue behaviours across frames.

```text
Reactive Agent
Perceive
Respond
Re-evaluate next frame
```

```text
Intermediate Agent
Perceive
Select a behaviour
Commit to movement
Continue across frames
Adapt when context changes
```

By the end of this demonstration, the Intermediate Human will test four local movement directions, select the legal direction that best supports its desired movement, maintain direction commitment, avoid unnecessary reversal, search configured Coin locations, remember the Chest, follow it after losing sight, and wander when no higher-priority behaviour applies.

The demo uses local contextual movement. It does not use nodes, graphs, routes, NavMesh, or pathfinding.

# Package Structure

```text
day15-intermediate-agents/
├── day15-walkthrough.md
├── instructor-notes.md
└── Day15/
    ├── Core/
    ├── Starter/
    ├── Teaching/Complete/
    ├── Scene/
    └── Prefabs/
```

# Supplied Scene and Framework

The package includes the Day 15 scene setup, Intermediate Human setup, configured Coins, and standardized scripts. Students begin in the supplied scene rather than converting the Day 14 scene during class.

The following boilerplate is supplied because it repeats concepts already taught:

- IntermediateAgentContext storage and helper methods
- IntermediatePerceptionSensor
- Rigidbody movement and rotation
- direction-to-target helpers
- persistent-target setters and clearers
- Coin-search indexing and target helpers
- configured Coin prefab and interaction scripts

Students implement the new Day 15 concepts:

1. four-direction availability
2. dot-product direction selection
3. direction commitment and reversal control
4. persistent wandering
5. ordered Intermediate Human rules

# Part 1: Open and Test the Supplied Scene

1. Import the package.
2. Open `Day15/Scene/Day_15_Scene.unity`.
3. Enter Play Mode.
4. Confirm the Intermediate Human, Coins, Key, Chest, Pit, Spike Trap, and walls are present.
5. Exit Play Mode.

## Compile Checkpoint

Resolve all compiler errors before continuing.

## Testing Checkpoint

Move scene objects when required to create isolated test conditions.

# Part 2: Review IntermediateAgentContext

Open `IntermediateAgentContext.cs`.

The script is supplied complete. It uses normal public fields and a normal `List<Transform>` property. It does not use `readonly`, `IReadOnlyList`, or other collection concepts outside the course scope.

Review these groups:

```text
Current perception
VisibleKey
VisibleCoin
VisibleChest
```

```text
Persistent behaviour
HasPersistentTarget
PersistentTarget
```

```text
Navigation
HasCommittedDirection
CurrentDirection
AvailableDirections
```

```text
Searching
KnownCoinLocations
HasSearchTarget
SearchTargetPosition
CurrentSearchIndex
```

## Discussion

The context stores information. It does not choose a behaviour.

## Testing Checkpoint

Select IntermediateHuman and confirm Known Coin Locations contains the configured Coin Transforms.

# Part 3: Review IntermediatePerceptionSensor

Open `IntermediatePerceptionSensor.cs`.

The supplied script reuses systems already taught:

```text
Physics.OverlapSphere
Vector3.Dot field of view
Physics.Raycast line of sight
Tags
Layer masks
```

Inspector masks:

```text
Perception Mask
Pickup
Target
```

```text
Obstruction Mask
Obstacle
```

Hazard is not included in either vision mask. The Pit affects movement but does not block vision.

## Testing Checkpoint

Move the Key, Coin, and Chest into and out of view. Move each behind a wall. Place the Pit between the agent and an object and confirm the Pit does not block perception.

# Part 4: Build Direction Availability

Open `IntermediateAvoidanceSensor.cs`.

Locate `Update()` in the Unity Methods region.

## Step 1: Test Forward

Under:

```csharp
// Test the forward direction.
```

Add:

```csharp
AddDirectionIfAvailable(transform.forward);
```

### Testing Checkpoint

Enter Play Mode. Move a wall directly in front of the agent. Confirm the list is empty when forward is blocked and contains forward when it is clear.

## Step 2: Test Right

Under:

```csharp
// Test the right direction.
```

Add:

```csharp
AddDirectionIfAvailable(transform.right);
```

### Testing Checkpoint

Place a wall on the right. Confirm right is excluded while forward remains available.

## Step 3: Test Backward

Under:

```csharp
// Test the backward direction.
```

Add:

```csharp
AddDirectionIfAvailable(-transform.forward);
```

### Testing Checkpoint

Place a wall behind the agent and confirm backward is excluded.

## Step 4: Test Left

Under:

```csharp
// Test the left direction.
```

Add:

```csharp
AddDirectionIfAvailable(-transform.right);
```

The completed method is:

```csharp
private void Update()
{
    context.ClearAvailableDirections();

    // Test the forward direction.
    AddDirectionIfAvailable(transform.forward);

    // Test the right direction.
    AddDirectionIfAvailable(transform.right);

    // Test the backward direction.
    AddDirectionIfAvailable(-transform.forward);

    // Test the left direction.
    AddDirectionIfAvailable(-transform.right);
}
```

## Discussion

The sensor now answers which local directions are open rather than only whether something is ahead.

## Compile Checkpoint

Save and compile.

## Testing Checkpoint

Move walls, the Spike Trap, and Pit around the agent. Walls and the Spike Trap remove directions. The Pit remains available when Can Cross Pits is enabled.

# Part 5: Select the Best Direction with a Dot Product

Open `IntermediateAgentController.cs`.

Locate `SelectBestAvailableDirection` in the Protected Methods region.

## Step 1: Handle No Available Directions

Under:

```csharp
// Handle the case where no directions are available.
```

Add:

```csharp
if (context.AvailableDirections.Count == 0)
{
    context.ClearCommittedDirection();
    return Vector3.zero;
}
```

## Step 2: Normalize the Desired Direction

Under:

```csharp
// Normalize the desired direction before comparing alignments.
```

Add:

```csharp
Vector3 normalizedDesiredDirection = desiredDirection.normalized;
```

## Step 3: Keep a Useful Committed Direction

Under:

```csharp
// Keep the committed direction while it remains available and useful.
```

Add:

```csharp
if (context.HasCommittedDirection && IsDirectionAvailable(context.CurrentDirection))
{
    float committedAlignment = Vector3.Dot(context.CurrentDirection, normalizedDesiredDirection);

    if (committedAlignment >= 0f)
    {
        return context.CurrentDirection;
    }
}
```

## Step 4: Track the Best Candidate

Under:

```csharp
// Track the best normal candidate.
```

Add:

```csharp
Vector3 bestDirection = Vector3.zero;
float bestAlignment = float.NegativeInfinity;
```

## Step 5: Compare Available Directions

Under:

```csharp
// Compare each direction with the desired direction.
```

Add:

```csharp
foreach (Vector3 candidateDirection in context.AvailableDirections)
{
    if (!canReverse && IsReverseDirection(candidateDirection))
    {
        continue;
    }

    float alignment = Vector3.Dot(candidateDirection.normalized, normalizedDesiredDirection);

    if (alignment <= bestAlignment)
    {
        continue;
    }

    bestAlignment = alignment;
    bestDirection = candidateDirection.normalized;
}
```

## Dot Product Discussion

For normalized vectors:

```text
1
Same direction

0
Perpendicular

-1
Opposite direction
```

The available direction with the highest score best supports the intended movement.

## Step 6: Permit Reversal at a Dead End

Under:

```csharp
// Permit reversal when no normal candidate is available.
```

Add:

```csharp
if (bestDirection.sqrMagnitude <= 0f)
{
    foreach (Vector3 candidateDirection in context.AvailableDirections)
    {
        float alignment = Vector3.Dot(candidateDirection.normalized, normalizedDesiredDirection);

        if (alignment <= bestAlignment)
        {
            continue;
        }

        bestAlignment = alignment;
        bestDirection = candidateDirection.normalized;
    }
}
```

## Step 7: Commit the Direction

Under:

```csharp
// Commit to the selected direction.
```

Replace the placeholder return with:

```csharp
context.SetCommittedDirection(bestDirection);
return bestDirection;
```

## Compile Checkpoint

Save and compile.

## Testing Checkpoint

Move a Key into view and place a wall in the direct path. Confirm the agent chooses the available direction most closely aligned with the Key instead of pressing into the wall.

# Part 6: Add Persistent Wandering

Remain in `IntermediateAgentController.cs`.

Locate `GetWanderDirection`.

## Step 1: Keep the Current Direction

Under:

```csharp
// Keep the committed wander direction while it remains available.
```

Add:

```csharp
if (context.HasCommittedDirection && IsDirectionAvailable(context.CurrentDirection))
{
    return context.CurrentDirection;
}
```

## Step 2: Handle No Available Directions

Under:

```csharp
// Stop when no directions are available.
```

Add:

```csharp
if (context.AvailableDirections.Count == 0)
{
    context.ClearCommittedDirection();
    return Vector3.zero;
}
```

## Step 3: Exclude Immediate Reversal

Under:

```csharp
// Build a list that excludes immediate reversal.
```

Add:

```csharp
List<Vector3> wanderDirections = new List<Vector3>();

foreach (Vector3 availableDirection in context.AvailableDirections)
{
    if (IsReverseDirection(availableDirection))
    {
        continue;
    }

    wanderDirections.Add(availableDirection);
}
```

## Step 4: Allow Dead-End Reversal

Under:

```csharp
// Permit reversal when the agent reaches a dead end.
```

Add:

```csharp
if (wanderDirections.Count == 0)
{
    wanderDirections.AddRange(context.AvailableDirections);
}
```

## Step 5: Select and Commit

Under:

```csharp
// Select and commit a random direction.
```

Replace the placeholder return with:

```csharp
int randomIndex = Random.Range(0, wanderDirections.Count);
Vector3 wanderDirection = wanderDirections[randomIndex].normalized;
context.SetCommittedDirection(wanderDirection);
return wanderDirection;
```

## Discussion

Random selection happens only when the agent needs a new direction. It does not select a new random direction every frame.

## Compile Checkpoint

Save and compile.

## Testing Checkpoint

Move all targets outside perception and temporarily clear the Coin-location list. Confirm continuous wandering, stable commitment, direction changes when blocked, and reversal only at a dead end.

# Part 7: Build the Intermediate Human Rules

Open `IntermediateHumanController.cs`.

Locate `GetDesiredDirection`.

## Step 1: Visible Key

Under:

```csharp
// Respond to a visible Key.
```

Add:

```csharp
if (context.VisibleKey != null)
{
    SetResponseTarget(context.VisibleKey);
    return GetDirectionTo(context.VisibleKey);
}
```

## Step 2: Discover the Chest

Under:

```csharp
// Discover and follow a visible Chest while carrying the Key.
```

Add:

```csharp
if (context.HasKey && context.VisibleChest != null)
{
    if (context.PersistentTarget != context.VisibleChest)
    {
        context.SetPersistentTarget(context.VisibleChest);
        context.ClearCommittedDirection();
    }

    SetResponseTarget(context.VisibleChest);
    return GetDirectionTo(context.VisibleChest);
}
```

## Step 3: Follow the Remembered Chest

Under:

```csharp
// Continue following the remembered Chest.
```

Add:

```csharp
if (context.HasKey && context.HasPersistentTarget && context.PersistentTarget != null)
{
    SetResponseTarget(context.PersistentTarget);
    return GetDirectionTo(context.PersistentTarget);
}
```

## Step 4: Clear Chest Memory

Under:

```csharp
// Clear the remembered Chest after the Key is lost.
```

Add:

```csharp
if (!context.HasKey && context.HasPersistentTarget)
{
    context.ClearPersistentTarget();
}
```

## Step 5: Visible Coin

Under:

```csharp
// Respond to a visible Coin.
```

Add:

```csharp
if (context.VisibleCoin != null)
{
    SetResponseTarget(context.VisibleCoin);
    return GetDirectionTo(context.VisibleCoin);
}
```

## Step 6: Search Known Coin Locations

Under:

```csharp
// Maintain and advance the remembered Coin search.
```

Add:

```csharp
EnsureCoinSearchTarget();
AdvanceCoinSearchIfRequired();

if (context.HasSearchTarget)
{
    return GetDirectionTo(context.SearchTargetPosition);
}
```

Leave the final zero return in place. The base controller interprets zero as the default Wander behaviour.

## Compile Checkpoint

Save and compile.

# Part 8: Test Persistent Chest Following

1. Begin without the Key.
2. Move the Chest into view.
3. Confirm it is perceived but not selected.
4. Collect the Key.
5. Move the Chest into view.
6. Confirm it becomes PersistentTarget.
7. Move the Chest behind a wall or outside perception.
8. Confirm VisibleChest clears.
9. Confirm PersistentTarget remains.
10. Confirm following continues.
11. Open the Chest.
12. Confirm the Key is consumed and persistent target clears.

# Part 9: Test Coin Searching

1. Confirm Coin Transforms are assigned in Known Coin Locations.
2. Move Coins outside immediate perception.
3. Confirm a search location is selected.
4. Place a wall in the direct path.
5. Confirm local direction selection avoids the wall while search persists.
6. Allow the agent to reach the location.
7. Confirm the index advances.
8. Move a Coin into view.
9. Confirm visible Coin response interrupts searching.
10. Collect it and confirm searching resumes.

# Part 10: Full Scene Test

Move objects as needed between tests.

- walls remove directions
- Spike Trap removes directions
- Pit remains traversable
- dot-product selection chooses the best legal direction
- committed movement remains stable
- Key interrupts search or wander
- Chest is remembered after discovery
- Coin visibility interrupts location search
- configured Coin locations are searched
- Wander runs only when no higher-priority rule applies

# Part 11: Troubleshooting

## No Available Directions

- confirm the Floor is not in Blocking Mask
- confirm the agent layer is not in Blocking Mask
- raise Sensor Height
- reduce Sensor Radius

## Wall Vibration

- remove old movement components
- confirm only IntermediateHumanController moves the Rigidbody
- confirm commitment is retained while available
- confirm immediate reversal is filtered

## Pit Blocks Perception

- remove Hazard from Perception Mask
- use Obstacle only in Obstruction Mask
- keep Pit on Hazard

## Human Does Not Wander

- remove current visible and persistent targets
- clear Coin locations for the isolated test
- confirm at least one direction is available

## Coin Search Does Not Advance

- confirm Coin Transforms are assigned
- increase Stop Distance slightly
- confirm AdvanceCoinSearchIfRequired is called

## Chest Is Not Remembered

- confirm HasKey is true
- confirm Chest tag and Target layer
- confirm IntermediatePerceptionSensor stores VisibleChest

# Part 12: Optional Orc Extension

Create an Intermediate Orc with the same framework. Suggested rules:

```text
Visible Human
Flee with reversal allowed

Visible Key
Move toward Key

Has Key and Visible Chest
Remember and follow Chest

Visible Coin
Move toward Coin

Known Coin Location
Search

Otherwise
Wander
```

# Part 13: Additional Practice

1. select Coin locations randomly
2. track visited locations
3. avoid selecting the same location twice
4. add a temporary random flee destination
5. change rule order
6. add minimum and maximum following distances
7. implement the Intermediate Orc

# Completion Checklist

- supplied Day 15 scene opens
- Intermediate Human uses only Intermediate components
- no `readonly` or `IReadOnly` types are used
- four local directions are evaluated
- dot product selects the best legal direction
- committed direction persists
- normal movement rejects immediate reversal
- dead-end movement permits reversal
- Wander persists across frames
- Coin locations are configured in the Inspector
- visible Coin interrupts searching
- Coin search persists across frames
- Chest remains remembered after visibility is lost
- Pit affects movement but not perception
- no pathfinding system is added
- no Console errors remain
