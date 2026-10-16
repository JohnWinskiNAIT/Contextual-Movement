# Day 14 Walkthrough: Contextual Responses

## Demo Purpose

Day 13 established what an agent can perceive. Day 14 maps that perception to movement responses and introduces short-term perception memory. Day 15 will add priorities and persistent selected goals.

The package does not include another runtime `AgentContext.cs` or `AvoidanceSensor.cs`. Modify the existing Day 13 scripts. Prefixed complete references are included.

# Part 1: Open and Test the Existing Project

Open `Day_13_Scene`. Move the Orc into/out of perception, move it behind a wall, and move a wall into each agent's SphereCast. Confirm perception, line of sight, and wall avoidance before continuing.

## Compile Checkpoint

Resolve all compiler errors.

## Testing Checkpoint

Move objects around the scene as needed. Confirm both agents have Rigidbody, Collider, and Y rotation available.

# Part 2: Import the Day 14 Package

Import Core, Starter, and Teaching/Complete. Keep the existing runtime AgentContext and AvoidanceSensor. Confirm only the prefixed reference copies are added.

## Compile Checkpoint

Confirm there are no duplicate-class errors.

# Part 3: Create the Day 14 Scene and Prefabs

Save as `Day_14_Scene`. Create HumanAgent_Day14 and OrcAgent_Day14 variants. Disable ChaseAgent, FleeAgent, and other old movement controllers. Keep Rigidbody, Collider, AgentContext, PerceptionSensor, and AvoidanceSensor.

# Part 4: Optional Level Setup

The starter scene may differ. Ensure Key, Chest, Pit, Spike Trap, walls, Human, and Orc can be repositioned for tests.

# Part 5: Modify the Existing AgentContext

Open the existing `AgentContext.cs`.

After `public bool HasAgentTarget;`, add:

```csharp
public bool HasKey;
public bool HasResponseTarget;
```

After the final Visible Transform, add:

```csharp
public Transform ResponseTarget;
public Transform LastPerceivedObject;
```

After `public Vector3 AvoidanceDirection;`, add:

```csharp
public Vector3 LastPerceivedPosition;
public float LastPerceivedTime;
```

Inside `AddPerceivedObject`, immediately after `PerceivedObjects.Add(perceivedTransform);`, add:

```csharp
LastPerceivedObject = perceivedTransform;
LastPerceivedPosition = perceivedTransform.position;
LastPerceivedTime = Time.time;
```

Do not clear these fields in `ClearPerception`.

## Compile Checkpoint

Save and confirm only one runtime AgentContext exists.

## Testing Checkpoint

Move the Key into view, then behind a wall. Current perception should clear while LastPerceived remains.

# Part 6: Configure the Completed Key System

Attach the supplied KeyPickup. Configure Pickup layer, Key tag, and trigger collider.

```csharp
using UnityEngine;
public class KeyPickup : MonoBehaviour
{
    #region Unity Methods
    private void OnTriggerEnter(Collider other)
    {
        AgentContext context = other.GetComponentInParent<AgentContext>();
        if (context == null) { return; }
        context.HasKey = true;
        Debug.Log($"{context.name} collected the Key.", context);
        gameObject.SetActive(false);
    }
    #endregion
}
```

## Testing Checkpoint

Move an agent into the trigger. Confirm HasKey and Key deactivation.

# Part 7: Configure the Completed Chest System

Attach the supplied ChestInteraction. Configure Target layer, Chest tag, and trigger collider.

```csharp
using UnityEngine;
public class ChestInteraction : MonoBehaviour
{
    #region Private Variables
    private bool isOpen;
    #endregion
    #region Public Properties
    public bool IsOpen => isOpen;
    #endregion
    #region Unity Methods
    private void OnTriggerEnter(Collider other)
    {
        if (isOpen) { return; }
        AgentContext context = other.GetComponentInParent<AgentContext>();
        if (context == null || !context.HasKey) { return; }
        isOpen = true;
        context.HasKey = false;
        Debug.Log($"{context.name} opened the Chest.", this);
    }
    #endregion
}
```

## Testing Checkpoint

Move an agent into the Chest without a Key, then repeat after collecting the Key.

# Part 8: Build AgentResponseController

Replace the Starter methods with the following code.

```csharp
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AgentContext))]
public abstract class AgentResponseController : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private float moveSpeed=3f; [SerializeField] private float turnSpeed=8f;
    [SerializeField] private float stopDistance=0.75f; [SerializeField] private float memoryDuration=2f;
    #endregion
    #region Protected Variables
    protected AgentContext context; protected Rigidbody rigidBody;
    #endregion
    #region Unity Methods
    protected virtual void Awake()
    {
        context=GetComponent<AgentContext>(); rigidBody=GetComponent<Rigidbody>(); rigidBody.useGravity=false;
        rigidBody.constraints=RigidbodyConstraints.FreezePositionY|RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationZ;
    }
    private void FixedUpdate()
    {
        context.ResponseTarget=null; context.HasResponseTarget=false; Vector3 responseDirection=GetResponseDirection();
        // If IsAvoiding is true, use AvoidanceDirection. Otherwise, use responseDirection.
        Vector3 movementDirection=context.IsAvoiding?context.AvoidanceDirection:responseDirection; movementDirection.y=0f;
        if (movementDirection.sqrMagnitude<=0f) { return; }
        if (!context.IsAvoiding && context.ResponseTarget!=null)
        {
            Vector3 toTarget=context.ResponseTarget.position-rigidBody.position; toTarget.y=0f;
            if (toTarget.magnitude<=stopDistance) { return; }
        }
        movementDirection.Normalize(); Quaternion targetRotation=Quaternion.LookRotation(movementDirection,Vector3.up);
        Quaternion nextRotation=Quaternion.Slerp(rigidBody.rotation,targetRotation,turnSpeed*Time.fixedDeltaTime);
        Vector3 nextPosition=rigidBody.position+movementDirection*moveSpeed*Time.fixedDeltaTime;
        rigidBody.MoveRotation(nextRotation); rigidBody.MovePosition(nextPosition);
    }
    private void Update() { if (context.ResponseTarget!=null) { Debug.DrawLine(transform.position,context.ResponseTarget.position,Color.magenta); } }
    #endregion
    #region Protected Methods
    protected abstract Vector3 GetResponseDirection();
    protected Vector3 GetDirectionTo(Transform target) { if(target==null)return Vector3.zero; Vector3 d=target.position-rigidBody.position; d.y=0f; return d; }
    protected Vector3 GetDirectionAwayFrom(Transform target) { if(target==null)return Vector3.zero; Vector3 d=rigidBody.position-target.position; d.y=0f; return d; }
    protected bool HasRecentPerception() { return context.LastPerceivedObject!=null && Time.time-context.LastPerceivedTime<=memoryDuration; }
    protected Vector3 GetDirectionToLastPerceivedPosition() { Vector3 d=context.LastPerceivedPosition-rigidBody.position; d.y=0f; return d; }
    protected void SetResponseTarget(Transform target) { context.ResponseTarget=target; context.HasResponseTarget=target!=null; }
    #endregion
}
```

The ternary line selects AvoidanceDirection when IsAvoiding is true and responseDirection otherwise.

## Compile Checkpoint

Save and compile.

# Part 9: Build Human Responses

Replace `GetResponseDirection()` with:

```csharp
using UnityEngine;
public class HumanResponseController : AgentResponseController
{
    protected override Vector3 GetResponseDirection()
    {
        if(context.VisiblePickup!=null&&context.VisiblePickup.CompareTag("Key")){SetResponseTarget(context.VisiblePickup);return GetDirectionTo(context.VisiblePickup);}
        if(context.HasKey&&context.VisibleTarget!=null&&context.VisibleTarget.CompareTag("Chest")){SetResponseTarget(context.VisibleTarget);return GetDirectionTo(context.VisibleTarget);}
        if(context.VisibleAgent!=null&&context.VisibleAgent.CompareTag("AIAgent")){AgentContext other=context.VisibleAgent.GetComponent<AgentContext>();if(other!=null&&other.HasKey){SetResponseTarget(context.VisibleAgent);return GetDirectionTo(context.VisibleAgent);}}
        if(HasRecentPerception())return GetDirectionToLastPerceivedPosition(); return Vector3.zero;
    }
}
```

Attach HumanResponseController.

## Testing Checkpoint

Move Key, Chest, and Orc into/out of Human view. Test Orc both without and with HasKey.

# Part 10: Build Orc Responses

Replace `GetResponseDirection()` with:

```csharp
using UnityEngine;
public class OrcResponseController : AgentResponseController
{
    protected override Vector3 GetResponseDirection()
    {
        if(context.VisibleAgent!=null&&context.VisibleAgent.CompareTag("Player")){SetResponseTarget(context.VisibleAgent);return GetDirectionAwayFrom(context.VisibleAgent);}
        if(context.VisiblePickup!=null&&context.VisiblePickup.CompareTag("Key")){SetResponseTarget(context.VisiblePickup);return GetDirectionTo(context.VisiblePickup);}
        if(context.HasKey&&context.VisibleTarget!=null&&context.VisibleTarget.CompareTag("Chest")){SetResponseTarget(context.VisibleTarget);return GetDirectionTo(context.VisibleTarget);}
        if(HasRecentPerception())return GetDirectionToLastPerceivedPosition(); return Vector3.zero;
    }
}
```

Attach OrcResponseController.

## Testing Checkpoint

Move Human into Orc view, then behind a wall. Move Key into view only after Human is hidden. Test Chest without and with HasKey.

# Part 11: Modify the Existing AvoidanceSensor

After `private LayerMask avoidanceMask;`, add:

```csharp
[SerializeField]
private bool canCrossPits;
```

Add this helper in Private Methods:

```csharp
private bool IsPit(Collider hitCollider)
{
    Transform current = hitCollider.transform;
    while (current != null)
    {
        if (current.CompareTag("Pit")) { return true; }
        current = current.parent;
    }
    return false;
}
```

Replace Update with:

```csharp
private void Update()
{
    Vector3 origin = transform.position + Vector3.up * sensorHeight;
    bool hasHit = Physics.SphereCast(origin, sensorRadius, transform.forward, out RaycastHit hit, detectionDistance, avoidanceMask, QueryTriggerInteraction.Collide);
    if (hasHit && canCrossPits && IsPit(hit.collider)) { hasHit = false; }
    context.IsAvoiding = hasHit;
    // If hasHit is true, use transform.right. Otherwise, use Vector3.zero.
    context.AvoidanceDirection = hasHit ? transform.right : Vector3.zero;
}
```

Human: Can Cross Pits true. Orc: false. Both avoidance masks include Obstacle and Hazard. Both still avoid walls and Spike Traps.

## Testing Checkpoint

Move a wall, Spike Trap, and Pit into each SphereCast path. Both avoid wall/trap; only Orc avoids Pit.

# Part 12: Test Response Visualization

Yellow is range, blue FOV, cyan LOS, red avoidance sensor, green avoidance direction, magenta response target. Move objects into valid and invalid contexts and confirm the line follows only the selected target.

# Part 13: Test Short-Term Memory

Set Memory Duration to 2. Move Key into view, then behind a wall. Movement should continue briefly toward LastPerceivedPosition and stop after expiration. Repeat with 0.5 and 3.

# Part 14: Full Scene Test

Move objects between tests: Human Key collection, Orc Key collection while Human hidden, Chest without/with Key, Orc without/with Key in Human view, Human in Orc view, walls, Spike Trap, Pit, memory, and magenta target line.

# Part 15: Troubleshooting

Duplicate scripts: keep existing runtime scripts and prefixed references only. No movement: disable old controllers and check Rigidbody constraints. Walls: collider children must use Obstacle and masks must include Obstacle. Pit: verify Pit tag, Hazard layer, QueryTriggerInteraction.Collide, and Can Cross Pits. Memory: move the target out of perception and confirm duration is positive.

# Part 16: Additional Practice

Add Powerup response, different memory durations, remembered-position visualization, another hazard tag, response cooldown, different speeds, or a HasKey indicator.

# Completion Checklist

Scene/prefabs saved; existing AgentContext and AvoidanceSensor modified; no duplicate runtime scripts; Key/Chest work; Human and Orc rules work; Human crosses Pit; Orc avoids Pit; both avoid walls/Spike Traps; memory expires; ResponseTarget is magenta; no priorities or pathfinding; no Console errors.
