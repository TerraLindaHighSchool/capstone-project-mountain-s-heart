using System;
using System.Reflection;
using UnityEngine;

/* Token: 0x02000744 RID: 1860
[Serializable]
public class HeroControllerStates
{
    // Token: 0x060021A1 RID: 8609 RVA: 0x000D0021 File Offset: 0x000CE421
    public HeroControllerStates()
    {
        this.facingRight = false;
        this.Reset();
    }

    // Token: 0x060021A2 RID: 8610 RVA: 0x000D0038 File Offset: 0x000CE438
    public bool GetState(string stateName)
    {
        FieldInfo field = base.GetType().GetField(stateName);
        if (field != null)
        {
            return (bool)field.GetValue(HeroController.instance.cState);
        }
        Debug.LogError("HeroControllerStates: Could not find bool named" + stateName + "in cState");
        return false;
    }

    // Token: 0x060021A3 RID: 8611 RVA: 0x000D0084 File Offset: 0x000CE484
    public void SetState(string stateName, bool value)
    {
        FieldInfo field = base.GetType().GetField(stateName);
        if (field != null)
        {
            try
            {
                field.SetValue(HeroController.instance.cState, value);
            }
            catch (Exception arg)
            {
                Debug.LogError("Failed to set cState: " + arg);
            }
        }
        else
        {
            Debug.LogError("HeroControllerStates: Could not find bool named" + stateName + "in cState");
        }
    }

    // Token: 0x060021A4 RID: 8612 RVA: 0x000D0100 File Offset: 0x000CE500
    public void Reset()
    {
        this.onGround = false;
        this.jumping = false;
        this.falling = false;
        this.dashing = false;
        this.backDashing = false;
        this.touchingWall = false;
        this.wallSliding = false;
        this.transitioning = false;
        this.attacking = false;
        this.lookingUp = false;
        this.lookingDown = false;
        this.altAttack = false;
        this.upAttacking = false;
        this.downAttacking = false;
        this.bouncing = false;
        this.dead = false;
        this.hazardDeath = false;
        this.willHardLand = false;
        this.recoiling = false;
        this.recoilFrozen = false;
        this.invulnerable = false;
        this.casting = false;
        this.castRecoiling = false;
        this.preventDash = false;
        this.preventBackDash = false;
        this.dashCooldown = false;
        this.backDashCooldown = false;
    }

    // Token: 0x04002461 RID: 9313
    public bool facingRight;

    // Token: 0x04002462 RID: 9314
    public bool onGround;

    // Token: 0x04002463 RID: 9315
    public bool jumping;

    // Token: 0x04002464 RID: 9316
    public bool wallJumping;

    // Token: 0x04002465 RID: 9317
    public bool doubleJumping;

    // Token: 0x04002466 RID: 9318
    public bool nailCharging;

    // Token: 0x04002467 RID: 9319
    public bool shadowDashing;

    // Token: 0x04002468 RID: 9320
    public bool swimming;

    // Token: 0x04002469 RID: 9321
    public bool falling;

    // Token: 0x0400246A RID: 9322
    public bool dashing;

    // Token: 0x0400246B RID: 9323
    public bool superDashing;

    // Token: 0x0400246C RID: 9324
    public bool superDashOnWall;

    // Token: 0x0400246D RID: 9325
    public bool backDashing;

    // Token: 0x0400246E RID: 9326
    public bool touchingWall;

    // Token: 0x0400246F RID: 9327
    public bool wallSliding;

    // Token: 0x04002470 RID: 9328
    public bool transitioning;

    // Token: 0x04002471 RID: 9329
    public bool attacking;

    // Token: 0x04002472 RID: 9330
    public bool lookingUp;

    // Token: 0x04002473 RID: 9331
    public bool lookingDown;

    // Token: 0x04002474 RID: 9332
    public bool lookingUpAnim;

    // Token: 0x04002475 RID: 9333
    public bool lookingDownAnim;

    // Token: 0x04002476 RID: 9334
    public bool altAttack;

    // Token: 0x04002477 RID: 9335
    public bool upAttacking;

    // Token: 0x04002478 RID: 9336
    public bool downAttacking;

    // Token: 0x04002479 RID: 9337
    public bool bouncing;

    // Token: 0x0400247A RID: 9338
    public bool shroomBouncing;

    // Token: 0x0400247B RID: 9339
    public bool recoilingRight;

    // Token: 0x0400247C RID: 9340
    public bool recoilingLeft;

    // Token: 0x0400247D RID: 9341
    public bool dead;

    // Token: 0x0400247E RID: 9342
    public bool hazardDeath;

    // Token: 0x0400247F RID: 9343
    public bool hazardRespawning;

    // Token: 0x04002480 RID: 9344
    public bool willHardLand;

    // Token: 0x04002481 RID: 9345
    public bool recoilFrozen;

    // Token: 0x04002482 RID: 9346
    public bool recoiling;

    // Token: 0x04002483 RID: 9347
    public bool invulnerable;

    // Token: 0x04002484 RID: 9348
    public bool casting;

    // Token: 0x04002485 RID: 9349
    public bool castRecoiling;

    // Token: 0x04002486 RID: 9350
    public bool preventDash;

    // Token: 0x04002487 RID: 9351
    public bool preventBackDash;

    // Token: 0x04002488 RID: 9352
    public bool dashCooldown;

    // Token: 0x04002489 RID: 9353
    public bool backDashCooldown;

    // Token: 0x0400248A RID: 9354
    public bool nearBench;

    // Token: 0x0400248B RID: 9355
    public bool inWalkZone;

    // Token: 0x0400248C RID: 9356
    public bool isPaused;

    // Token: 0x0400248D RID: 9357
    public bool onConveyor;

    // Token: 0x0400248E RID: 9358
    public bool onConveyorV;

    // Token: 0x0400248F RID: 9359
    public bool inConveyorZone;

    // Token: 0x04002490 RID: 9360
    public bool spellQuake;

    // Token: 0x04002491 RID: 9361
    public bool freezeCharge;

    // Token: 0x04002492 RID: 9362
    public bool focusing;

    // Token: 0x04002493 RID: 9363
    public bool inAcid;

    // Token: 0x04002494 RID: 9364
    public bool slidingLeft;

    // Token: 0x04002495 RID: 9365
    public bool slidingRight;

    // Token: 0x04002496 RID: 9366
    public bool touchingNonSlider;

    // Token: 0x04002497 RID: 9367
    public bool wasOnGround;
}
*/