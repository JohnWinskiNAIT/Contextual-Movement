# Day 12 Walkthrough: Chase and Flee Agents

## Demo purpose

Day 12 extends the contextual movement systems from Days 10 and 11. The project already includes collision detection, hazard and obstacle classification, a forward avoidance sensor, and Rigidbody movement. This demonstration adds two opposite movement intents:

```text
Chase: move toward another agent.
Flee: move away from another agent.
```

Both behaviours continue to use the existing avoidance context. The demonstration also introduces inheritance so shared movement, rotation, Rigidbody configuration, and avoidance rules can be reused by multiple agent behaviours.

## Package structure

```text
day12-flee-and-chase/
├── day12-walkthrough.md
├── instructor-notes.md
└── Day12/
    ├── Core/
    │   └── AgentContext.cs
    ├── Starter/
    │   ├── AgentMovement.cs
    │   ├── ChaseAgent.cs
    │   └── FleeAgent.cs
    └── Teaching/
        └── Complete/
            ├── Complete_Day12_AgentContext.cs
            ├── Complete_Day12_AgentMovement.cs
            ├── Complete_Day12_ChaseAgent.cs
            └── Complete_Day12_FleeAgent.cs
```

The files in `Teaching/Complete` use unique filenames and class names so they can remain in the Unity project without creating duplicate-class errors. They are reference implementations and should not be attached to the demonstration prefabs unless the instructor intentionally uses them for recovery.

## Existing project requirements

Before beginning, the project should contain:

- `Day_11_Scene`
- `DetectionType.cs`
- `DetectionSource.cs`
- `DetectionAgent.cs`
- `AvoidanceSensor.cs`
- `CameraFollow.cs`
- A Rigidbody-controlled HumanAgent
- A Chest target from the previous demonstrations
- Hazard and obstacle prefabs with appropriate colliders

## Part 1: Open the existing project

1. Open the Module 3 Unity project used for Day 11.
2. Open `Day_11_Scene`.
3. Enter Play Mode.
4. Confirm the HumanAgent moves toward its current target.
5. Confirm the HumanAgent detects and attempts to avoid hazards and walls.
6. Confirm the red sensor and green avoidance visualizations appear in the Scene view.
7. Exit Play Mode.

### Discussion

The existing controller has two possible movement directions:

```text
Target direction
Avoidance direction
```

Day 12 adds a new distinction. The target can now be another moving agent, and the desired direction depends on whether the behaviour is chasing or fleeing.

### Testing checkpoint

Do not continue until the Day 11 scene runs without Console errors.

## Part 2: Import the Day 12 package

1. Import the supplied Day 12 package.
2. Create `Assets/Scripts/Day12` if it was not created automatically.
3. Copy `Day12/Core/AgentContext.cs` into the Day 12 scripts folder and replace the previous AgentContext implementation.
4. Copy the files from `Day12/Starter` into the Day 12 scripts folder.
5. Keep the reference files in `Day12/Teaching/Complete`.
6. Allow Unity to compile.

### Compile checkpoint

Expected result:

- No duplicate-class errors.
- AgentContext includes `HasAgentTarget` and `CurrentAgentTarget`.
- AgentMovement is an abstract class.
- ChaseAgent and FleeAgent compile as concrete components.

## Part 3: Create the Day 12 scene

1. Open `Day_11_Scene`.
2. Choose **File > Save As**.
3. Save the duplicate as `Day_12_Scene`.
4. Duplicate the Day 11 HumanAgent prefab or create a prefab variant before changing its movement components.
5. Create separate prefab variants for the chase and flee agents.

Suggested prefab names:

```text
HumanAgent_Day12_Chase
OrcAgent_Day12_Flee
```

### Discussion

The two prefabs reuse a shared movement base but provide different intent calculations. This keeps common Rigidbody and avoidance code in one place.

## Part 4: Optional level setup

This section is optional. A different completed layout may be supplied in the starter package.

If you are modifying the Day 11 room, add the Orc prefab and arrange the scene so both agents have room to move.

```text
#########################
#                  C    #
#                       #
#      #######          #
#                       #
#          T            #
#                       #
# H                O    #
#########################
```

```text
H = HumanAgent
O = OrcAgent
T = Spike Trap
C = Chest
```

For the first chase and flee tests, move obstacles and hazards away from the two agents. Reintroduce them after the movement intents work.

### Optional scene checkpoint

- HumanAgent and OrcAgent each have a Rigidbody.
- Each agent has AgentContext and AvoidanceSensor.
- Each agent's visual model faces the root object's positive Z direction.
- Neither agent starts inside another collider.
- The camera view includes both agents or follows the HumanAgent.

## Part 5: Expand AgentContext

Open `AgentContext.cs`.

Add the Boolean state:

```csharp
public bool HasAgentTarget;
```

Add the moving target reference:

```csharp
public Transform CurrentAgentTarget;
```

Update `PrintCurrentState` so it includes both values.

### Discussion

`CurrentTarget` remains available for object goals such as the Chest. `CurrentAgentTarget` stores another moving agent. Keeping the references separate makes the context explicit and prepares the project for later object-detection and response lessons.

### Compile checkpoint

Save the file and confirm Unity compiles without errors.

## Part 6: Create the reusable AgentMovement base class

Open `AgentMovement.cs`.

The class is abstract:

```csharp
public abstract class AgentMovement : MonoBehaviour
```

An abstract movement component defines shared implementation but is not attached directly. ChaseAgent and FleeAgent inherit its functionality.

### Step 1: Get required components

Replace `Awake` with:

```csharp
protected virtual void Awake()
{
    context = GetComponent<AgentContext>();
    rigidBody = GetComponent<Rigidbody>();
    rigidBody.useGravity = false;
    rigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
}
```

### Discussion

Y rotation remains available so each agent can face its selected movement direction. Both derived behaviours reuse this setup.

### Compile checkpoint

Save and confirm Unity compiles.

## Part 7: Define the intent direction contract

AgentMovement contains:

```csharp
protected abstract Vector3 GetIntentDirection();
```

The base class does not decide whether the agent chases or flees. It requires each derived class to provide a direction.

### Discussion

```text
Shared responsibility
Move, rotate, and apply avoidance.

Chase responsibility
Calculate a direction toward the target.

Flee responsibility
Calculate a direction away from the target.
```

No code is duplicated between the two agents for Rigidbody setup, rotation, or movement.

## Part 8: Build the chase direction

Open `ChaseAgent.cs`.

Replace `GetIntentDirection` with:

```csharp
protected override Vector3 GetIntentDirection()
{
    return context.CurrentAgentTarget.position - rigidBody.position;
}
```

### Vector discussion

```text
Chase direction = target position - agent position
```

This vector points from the chasing agent toward the target.

### Compile checkpoint

Save and confirm Unity compiles.

## Part 9: Build the flee direction

Open `FleeAgent.cs`.

Replace `GetIntentDirection` with:

```csharp
protected override Vector3 GetIntentDirection()
{
    return rigidBody.position - context.CurrentAgentTarget.position;
}
```

### Vector discussion

```text
Flee direction = agent position - target position
```

The subtraction order is reversed, so the result points away from the target.

### Compile checkpoint

Save and confirm Unity compiles.

## Part 10: Complete shared movement

Open `AgentMovement.cs` and replace `FixedUpdate` with:

```csharp
private void FixedUpdate()
{
    context.HasAgentTarget = context.CurrentAgentTarget != null;

    if (!context.HasAgentTarget)
    {
        return;
    }

    Vector3 intentDirection = GetIntentDirection();
    intentDirection.y = 0f;

    Vector3 movementDirection = context.IsAvoiding ? context.AvoidanceDirection : intentDirection;
    movementDirection.y = 0f;

    if (movementDirection.sqrMagnitude <= 0f)
    {
        return;
    }

    movementDirection.Normalize();

    Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
    Quaternion nextRotation = Quaternion.Slerp(rigidBody.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
    Vector3 nextPosition = rigidBody.position + movementDirection * moveSpeed * Time.fixedDeltaTime;

    rigidBody.MoveRotation(nextRotation);
    rigidBody.MovePosition(nextPosition);
}
```

### Discussion

The derived class supplies the intent direction. The base class applies the same priority rule used on Day 11:

```text
IF avoiding
    Use avoidance direction
ELSE
    Use chase or flee direction
```

Detection, avoidance, movement intent, facing, and Rigidbody movement now combine into agent behaviour.

### Compile checkpoint

Save and confirm Unity compiles.

## Part 11: Configure the chasing HumanAgent

1. Select the Day 12 HumanAgent prefab variant.
2. Remove or disable the Day 11 SimpleAgentController.
3. Add ChaseAgent.
4. Keep Rigidbody, AgentContext, DetectionAgent, and AvoidanceSensor.
5. Assign the OrcAgent transform to `CurrentAgentTarget`.
6. Confirm the Rigidbody allows Y rotation.
7. Apply the prefab changes.

### Chase checkpoint

Temporarily disable the OrcAgent's movement component.

1. Place the Orc in open space.
2. Enter Play Mode.
3. Confirm the HumanAgent turns toward the Orc.
4. Confirm the HumanAgent moves toward the Orc.
5. Move the Orc in the Scene view while running.
6. Confirm the HumanAgent continually updates its direction.
7. Exit Play Mode.

## Part 12: Configure the fleeing OrcAgent

1. Select the Day 12 OrcAgent prefab variant.
2. Add Rigidbody if it is not present.
3. Add AgentContext.
4. Add AvoidanceSensor.
5. Add FleeAgent.
6. Assign the HumanAgent transform to `CurrentAgentTarget`.
7. Configure the Rigidbody for top-down movement.
8. Confirm the Orc's forward direction matches the visible model.
9. Apply the prefab changes.

### Flee checkpoint

Temporarily disable ChaseAgent on the HumanAgent.

1. Enter Play Mode.
2. Confirm the Orc turns away from the HumanAgent.
3. Confirm the Orc moves away from the HumanAgent.
4. Move the HumanAgent in the Scene view while running.
5. Confirm the Orc updates its flee direction.
6. Exit Play Mode.

## Part 13: Add facing-vector Gizmos

Add the following method to ChaseAgent:

```csharp
private void OnDrawGizmosSelected()
{
    AgentContext agentContext = GetComponent<AgentContext>();

    if (agentContext == null || agentContext.CurrentAgentTarget == null)
    {
        return;
    }

    Vector3 chaseDirection = agentContext.CurrentAgentTarget.position - transform.position;
    chaseDirection.y = 0f;

    Gizmos.color = Color.blue;
    Gizmos.DrawLine(transform.position, transform.position + chaseDirection.normalized * 2f);
}
```

Add the following method to FleeAgent:

```csharp
private void OnDrawGizmosSelected()
{
    AgentContext agentContext = GetComponent<AgentContext>();

    if (agentContext == null || agentContext.CurrentAgentTarget == null)
    {
        return;
    }

    Vector3 fleeDirection = transform.position - agentContext.CurrentAgentTarget.position;
    fleeDirection.y = 0f;

    Gizmos.color = Color.cyan;
    Gizmos.DrawLine(transform.position, transform.position + fleeDirection.normalized * 2f);
}
```

### Visualization key

```text
Blue = Chase intent
Cyan = Flee intent
Red = Forward avoidance sensor
Green = Active avoidance direction
```

### Testing checkpoint

1. Select the HumanAgent and confirm the blue line points toward the Orc.
2. Select the OrcAgent and confirm the cyan line points away from the HumanAgent.
3. Move either agent in the Scene view and confirm the lines update.

## Part 14: Combine chase and flee

Enable ChaseAgent on the HumanAgent and FleeAgent on the OrcAgent.

### Full behaviour checkpoint

1. Enter Play Mode.
2. Confirm the HumanAgent pursues the OrcAgent.
3. Confirm the OrcAgent moves away from the HumanAgent.
4. Confirm both agents face their selected movement directions.
5. Confirm the target references remain assigned.
6. Confirm no Console exceptions appear.

### Discussion

Both agents use the same movement pipeline:

```text
Read target context
Calculate intent direction
Check avoidance context
Select final direction
Face final direction
Move
```

The only difference is the subtraction order used by the derived behaviour.

## Part 15: Reintroduce avoidance

This section may use the supplied level or the optional layout from Part 4.

1. Place a short wall where the HumanAgent encounters it while chasing.
2. Place a Spike Trap where the OrcAgent encounters it while fleeing.
3. Confirm the appropriate DetectionSource values are assigned.
4. Enter Play Mode.
5. Observe the intent and avoidance Gizmos.

### Expected priority

```text
Avoidance direction overrides chase direction.
Avoidance direction overrides flee direction.
```

When the forward sensor clears, each agent returns to its chase or flee intent.

### Competing-goals checkpoint

- Blue or cyan shows the behaviour intent.
- Red shows the forward sensor.
- Green appears while avoidance is active.
- The movement direction follows green while avoiding.
- The movement direction returns to blue or cyan after avoidance clears.

## Part 16: Full scene test

Run the completed scene and verify:

- `Day_12_Scene` is saved.
- HumanAgent uses ChaseAgent.
- OrcAgent uses FleeAgent.
- Each agent has the other assigned as CurrentAgentTarget.
- HasAgentTarget updates correctly.
- HumanAgent moves toward OrcAgent.
- OrcAgent moves away from HumanAgent.
- Both agents face their movement directions.
- Hazard and obstacle avoidance still work.
- Avoidance overrides chase and flee intent.
- Facing-vector Gizmos are correct.
- No exceptions appear in the Console.

## Troubleshooting

### An agent does not move

- Confirm CurrentAgentTarget is assigned.
- Confirm HasAgentTarget becomes true in Play Mode.
- Confirm the Rigidbody is not kinematic unless the chosen setup requires it.
- Confirm X and Z position are not frozen.
- Confirm move speed is greater than zero.

### The agent moves in the wrong direction

- For chasing, use target position minus agent position.
- For fleeing, use agent position minus target position.
- Confirm the correct component is attached to the prefab.

### The model faces backward

- Confirm the root object's positive Z direction represents forward.
- Rotate the visual child rather than changing the movement formula.

### The agents push or collide unexpectedly

- Adjust collider sizes.
- Use appropriate Rigidbody collision settings.
- Keep enough starting distance between agents.
- Add a stopping-distance or safe-distance extension if desired.

### Avoidance does not work on the Orc

- Confirm the Orc has AgentContext and AvoidanceSensor.
- Confirm sensor height intersects the relevant colliders.
- Confirm obstacle and hazard DetectionSource components are present.

### Duplicate class errors

- Confirm Starter classes use the normal names.
- Confirm files in Teaching/Complete use the `Complete_Day12_` prefix for both filenames and class names.

## Additional practice

### Challenge 1: Stop the chase

Add a stopping distance so the chasing HumanAgent does not collide with the OrcAgent.

### Challenge 2: Safe flee distance

Make the Orc flee only while the HumanAgent is within a configurable distance.

### Challenge 3: Speed differences

Give the chasing and fleeing agents different move speeds and observe the result.

### Challenge 4: Switch behaviours

Create a simple rule that changes one prefab between chase and flee behaviour.

### Challenge 5: Reusable target assignment

Create a public method that assigns CurrentAgentTarget at runtime.

### Challenge 6: Improved avoidance

Add angled sensors or combine the intent and avoidance directions instead of using a complete override.
