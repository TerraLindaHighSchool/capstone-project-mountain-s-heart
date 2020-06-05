using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
//using GlobalEnums;
//using HutongGames.PlayMaker;
using UnityEngine;

/* Token: 0x02000740 RID: 1856
public class HerosController : MonoBehaviour
{

    public float fallTimer { get; private set; }


    public GeoCounter geoCounter { get; private set; }


    public PlayMakerFSM proxyFSM { get; private set; }


    public TransitionPoint sceneEntryGate { get; private set; }


    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event HeroController.HeroInPosition heroInPosition;


    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event HeroController.TakeDamageEvent OnTakenDamage;


    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event HeroController.HeroDeathEvent OnDeath;




    public static HeroController instance
    {
        get
        {
            if (HeroController._instance == null)
            {
                HeroController._instance = UnityEngine.Object.FindObjectOfType<HeroController>();
                if (HeroController._instance == null)
                {
                    UnityEngine.Debug.LogError("Couldn't find a Hero, make sure one exists in the scene.");
                }
                else
                {
                    UnityEngine.Object.DontDestroyOnLoad(HeroController._instance.gameObject);
                }
            }
            return HeroController._instance;
        }
    }

    // Token: 0x060020C0 RID: 8384 RVA: 0x000C42BC File Offset: 0x000C26BC
    private void Awake()
    {
        if (HeroController._instance == null)
        {
            HeroController._instance = this;
            UnityEngine.Object.DontDestroyOnLoad(this);
        }
        else if (this != HeroController._instance)
        {
            UnityEngine.Object.Destroy(base.gameObject);
            return;
        }
        this.SetupGameRefs();
        this.SetupPools();
    }

    // Token: 0x060020C1 RID: 8385 RVA: 0x000C4314 File Offset: 0x000C2714
    private void Start()
    {
        this.heroInPosition += delegate (bool A_1)
        {
            this.isHeroInPosition = true;
        };
        this.playerData = PlayerData.instance;
        this.ui = UIManager.instance;
        this.geoCounter = GameCameras.instance.geoCounter;
        if (this.superDash == null)
        {
            UnityEngine.Debug.Log("SuperDash came up null, locating manually");
            this.superDash = FSMUtility.LocateFSM(base.gameObject, "Superdash");
        }
        if (this.fsm_thornCounter == null)
        {
            UnityEngine.Debug.Log("Thorn Counter came up null, locating manually");
            this.fsm_thornCounter = FSMUtility.LocateFSM(this.transform.Find("Charm Effects").gameObject, "Thorn Counter");
        }
        if (this.dashBurst == null)
        {
            UnityEngine.Debug.Log("DashBurst came up null, locating manually");
            this.dashBurst = FSMUtility.GetFSM(this.transform.Find("Effects").Find("Dash Burst").gameObject);
        }
        if (this.spellControl == null)
        {
            UnityEngine.Debug.Log("SpellControl came up null, locating manually");
            this.spellControl = FSMUtility.LocateFSM(base.gameObject, "Spell Control");
        }
        if (this.playerData.equippedCharm_26)
        {
            this.nailChargeTime = this.NAIL_CHARGE_TIME_CHARM;
        }
        else
        {
            this.nailChargeTime = this.NAIL_CHARGE_TIME_DEFAULT;
        }
        if (this.gm.IsGameplayScene())
        {
            this.isGameplayScene = true;
            this.vignette.enabled = true;
            if (this.heroInPosition != null)
            {
                this.heroInPosition(false);
            }
            this.FinishedEnteringScene(true, false);
        }
        else
        {
            this.isGameplayScene = false;
            this.transform.SetPositionY(-2000f);
            this.vignette.enabled = false;
            this.AffectedByGravity(false);
        }
        this.CharmUpdate();
        if (this.acidDeathPrefab)
        {
            ObjectPool.CreatePool(this.acidDeathPrefab, 1);
        }
        if (this.spikeDeathPrefab)
        {
            ObjectPool.CreatePool(this.spikeDeathPrefab, 1);
        }
    }

    // Token: 0x060020C2 RID: 8386 RVA: 0x000C451C File Offset: 0x000C291C
    public void SceneInit()
    {
        if (this == HeroController._instance)
        {
            if (!this.gm)
            {
                this.gm = GameManager.instance;
            }
            if (this.gm.IsGameplayScene())
            {
                this.isGameplayScene = true;
                HeroBox.inactive = false;
            }
            else
            {
                this.isGameplayScene = false;
                this.acceptingInput = false;
                this.SetState(ActorStates.no_input);
                this.transform.SetPositionY(-2000f);
                this.vignette.enabled = false;
                this.AffectedByGravity(false);
            }
            this.transform.SetPositionZ(0.004f);
            if (!this.blockerFix)
            {
                if (this.playerData.killedBlocker)
                {
                    this.gm.SetPlayerDataInt("killsBlocker", 0);
                }
                this.blockerFix = true;
            }
            this.SetWalkZone(false);
        }
    }

    // Token: 0x060020C3 RID: 8387 RVA: 0x000C45F8 File Offset: 0x000C29F8
    private void Update()
    {
        if (Time.frameCount % 10 == 0)
        {
            this.Update10();
        }
        this.current_velocity = this.rb2d.velocity;
        this.FallCheck();
        this.FailSafeChecks();
        if (this.hero_state == ActorStates.running && !this.cState.dashing && !this.cState.backDashing && !this.controlReqlinquished)
        {
            if (this.cState.inWalkZone)
            {
                this.audioCtrl.StopSound(HeroSounds.FOOTSTEPS_RUN);
                this.audioCtrl.PlaySound(HeroSounds.FOOTSTEPS_WALK);
            }
            else
            {
                this.audioCtrl.StopSound(HeroSounds.FOOTSTEPS_WALK);
                this.audioCtrl.PlaySound(HeroSounds.FOOTSTEPS_RUN);
            }
            if (this.runMsgSent && this.rb2d.velocity.x > -0.1f && this.rb2d.velocity.x < 0.1f)
            {
                this.runEffect.GetComponent<PlayMakerFSM>().SendEvent("RUN STOP");
                this.runEffect.transform.SetParent(null, true);
                this.runMsgSent = false;
            }
            if (!this.runMsgSent && (this.rb2d.velocity.x < -0.1f || this.rb2d.velocity.x > 0.1f))
            {
                this.runEffect = this.runEffectPrefab.Spawn();
                this.runEffect.transform.SetParent(base.gameObject.transform, false);
                this.runMsgSent = true;
            }
        }
        else
        {
            this.audioCtrl.StopSound(HeroSounds.FOOTSTEPS_RUN);
            this.audioCtrl.StopSound(HeroSounds.FOOTSTEPS_WALK);
            if (this.runMsgSent)
            {
                this.runEffect.GetComponent<PlayMakerFSM>().SendEvent("RUN STOP");
                this.runEffect.transform.SetParent(null, true);
                this.runMsgSent = false;
            }
        }
        if (this.hero_state == ActorStates.dash_landing)
        {
            this.dashLandingTimer += Time.deltaTime;
            if (this.dashLandingTimer > this.DOWN_DASH_TIME)
            {
                this.BackOnGround();
            }
        }
        if (this.hero_state == ActorStates.hard_landing)
        {
            this.hardLandingTimer += Time.deltaTime;
            if (this.hardLandingTimer > this.HARD_LANDING_TIME)
            {
                this.SetState(ActorStates.grounded);
                this.BackOnGround();
            }
        }
        else if (this.hero_state == ActorStates.no_input)
        {
            if (this.cState.recoiling)
            {
                if ((!this.playerData.equippedCharm_4 && this.recoilTimer < this.RECOIL_DURATION) || (this.playerData.equippedCharm_4 && this.recoilTimer < this.RECOIL_DURATION_STAL))
                {
                    this.recoilTimer += Time.deltaTime;
                }
                else
                {
                    this.CancelDamageRecoil();
                    if ((this.prev_hero_state == ActorStates.idle || this.prev_hero_state == ActorStates.running) && !this.CheckTouchingGround())
                    {
                        this.cState.onGround = false;
                        this.SetState(ActorStates.airborne);
                    }
                    else
                    {
                        this.SetState(ActorStates.previous);
                    }
                    this.fsm_thornCounter.SendEvent("THORN COUNTER");
                }
            }
        }
        else if (this.hero_state != ActorStates.no_input)
        {
            this.LookForInput();
            if (this.cState.recoiling)
            {
                this.cState.recoiling = false;
                this.AffectedByGravity(true);
            }
            if (this.cState.attacking && !this.cState.dashing)
            {
                this.attack_time += Time.deltaTime;
                if (this.attack_time >= this.attackDuration)
                {
                    this.ResetAttacks();
                    this.animCtrl.StopAttack();
                }
            }
            if (this.cState.bouncing)
            {
                if (this.bounceTimer < this.BOUNCE_TIME)
                {
                    this.bounceTimer += Time.deltaTime;
                }
                else
                {
                    this.CancelBounce();
                    this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, 0f);
                }
            }
            if (this.cState.shroomBouncing && this.current_velocity.y <= 0f)
            {
                this.cState.shroomBouncing = false;
            }
            if (this.hero_state == ActorStates.idle)
            {
                if (!this.controlReqlinquished && !this.gm.isPaused)
                {
                    if (this.inputHandler.inputActions.up.IsPressed || this.inputHandler.inputActions.rs_up.IsPressed)
                    {
                        this.cState.lookingDown = false;
                        this.cState.lookingDownAnim = false;
                        if (this.lookDelayTimer >= this.LOOK_DELAY || (this.inputHandler.inputActions.rs_up.IsPressed && !this.cState.jumping && !this.cState.dashing))
                        {
                            this.cState.lookingUp = true;
                        }
                        else
                        {
                            this.lookDelayTimer += Time.deltaTime;
                        }
                        if (this.lookDelayTimer >= this.LOOK_ANIM_DELAY || this.inputHandler.inputActions.rs_up.IsPressed)
                        {
                            this.cState.lookingUpAnim = true;
                        }
                        else
                        {
                            this.cState.lookingUpAnim = false;
                        }
                    }
                    else if (this.inputHandler.inputActions.down.IsPressed || this.inputHandler.inputActions.rs_down.IsPressed)
                    {
                        this.cState.lookingUp = false;
                        this.cState.lookingUpAnim = false;
                        if (this.lookDelayTimer >= this.LOOK_DELAY || (this.inputHandler.inputActions.rs_down.IsPressed && !this.cState.jumping && !this.cState.dashing))
                        {
                            this.cState.lookingDown = true;
                        }
                        else
                        {
                            this.lookDelayTimer += Time.deltaTime;
                        }
                        if (this.lookDelayTimer >= this.LOOK_ANIM_DELAY || this.inputHandler.inputActions.rs_down.IsPressed)
                        {
                            this.cState.lookingDownAnim = true;
                        }
                        else
                        {
                            this.cState.lookingDownAnim = false;
                        }
                    }
                    else
                    {
                        this.ResetLook();
                    }
                }
                this.runPuffTimer = 0f;
            }
        }
        this.LookForQueueInput();
        if (this.drainMP)
        {
            this.drainMP_timer += Time.deltaTime;
            this.drainMP_seconds += Time.deltaTime;
            while (this.drainMP_timer >= this.drainMP_time)
            {
                this.MP_drained += 1f;
                this.drainMP_timer -= this.drainMP_time;
                this.TakeMP(1);
                this.gm.soulOrb_fsm.SendEvent("MP DRAIN");
                if (this.MP_drained == this.focusMP_amount)
                {
                    this.MP_drained -= this.drainMP_time;
                    this.proxyFSM.SendEvent("HeroCtrl-FocusCompleted");
                }
            }
        }
        if (this.cState.wallSliding)
        {
            if (this.airDashed)
            {
                this.airDashed = false;
            }
            if (this.doubleJumped)
            {
                this.doubleJumped = false;
            }
            if (this.cState.onGround)
            {
                this.FlipSprite();
                this.CancelWallsliding();
            }
            if (!this.cState.touchingWall)
            {
                this.FlipSprite();
                this.CancelWallsliding();
            }
            if (!this.CanWallSlide())
            {
                this.CancelWallsliding();
            }
            if (!this.playedMantisClawClip)
            {
                this.audioSource.PlayOneShot(this.mantisClawClip, 1f);
                this.playedMantisClawClip = true;
            }
            if (!this.playingWallslideClip)
            {
                if (this.wallslideClipTimer <= this.WALLSLIDE_CLIP_DELAY)
                {
                    this.wallslideClipTimer += Time.deltaTime;
                }
                else
                {
                    this.wallslideClipTimer = 0f;
                    this.audioCtrl.PlaySound(HeroSounds.WALLSLIDE);
                    this.playingWallslideClip = true;
                }
            }
        }
        else if (this.playedMantisClawClip)
        {
            this.playedMantisClawClip = false;
        }
        if (!this.cState.wallSliding && this.playingWallslideClip)
        {
            this.audioCtrl.StopSound(HeroSounds.WALLSLIDE);
            this.playingWallslideClip = false;
        }
        if (!this.cState.wallSliding && this.wallslideClipTimer > 0f)
        {
            this.wallslideClipTimer = 0f;
        }
        if (this.wallSlashing && !this.cState.wallSliding)
        {
            this.CancelAttack();
        }
        if (this.attack_cooldown > 0f)
        {
            this.attack_cooldown -= Time.deltaTime;
        }
        if (this.dashCooldownTimer > 0f)
        {
            this.dashCooldownTimer -= Time.deltaTime;
        }
        if (this.shadowDashTimer > 0f)
        {
            this.shadowDashTimer -= Time.deltaTime;
            if (this.shadowDashTimer <= 0f)
            {
                this.spriteFlash.FlashShadowRecharge();
            }
        }
        this.preventCastByDialogueEndTimer -= Time.deltaTime;
        if (!this.gm.isPaused)
        {
            if (this.inputHandler.inputActions.attack.IsPressed && this.CanNailCharge())
            {
                this.cState.nailCharging = true;
                this.nailChargeTimer += Time.deltaTime;
            }
            else if (this.cState.nailCharging || this.nailChargeTimer != 0f)
            {
                this.artChargeEffect.SetActive(false);
                this.cState.nailCharging = false;
                this.audioCtrl.StopSound(HeroSounds.NAIL_ART_CHARGE);
            }
            if (this.cState.nailCharging && this.nailChargeTimer > 0.5f && !this.artChargeEffect.activeSelf && this.nailChargeTimer < this.nailChargeTime)
            {
                this.artChargeEffect.SetActive(true);
                this.audioCtrl.PlaySound(HeroSounds.NAIL_ART_CHARGE);
            }
            if (this.artChargeEffect.activeSelf && (!this.cState.nailCharging || this.nailChargeTimer > this.nailChargeTime))
            {
                this.artChargeEffect.SetActive(false);
                this.audioCtrl.StopSound(HeroSounds.NAIL_ART_CHARGE);
            }
            if (!this.artChargedEffect.activeSelf && this.nailChargeTimer >= this.nailChargeTime)
            {
                this.artChargedEffect.SetActive(true);
                this.artChargedFlash.SetActive(true);
                this.artChargedEffectAnim.PlayFromFrame(0);
                GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
                this.audioSource.PlayOneShot(this.nailArtChargeComplete, 1f);
                this.audioCtrl.PlaySound(HeroSounds.NAIL_ART_READY);
                this.cState.nailCharging = true;
            }
            if (this.artChargedEffect.activeSelf && (this.nailChargeTimer < this.nailChargeTime || !this.cState.nailCharging))
            {
                this.artChargedEffect.SetActive(false);
                this.audioCtrl.StopSound(HeroSounds.NAIL_ART_READY);
            }
        }
        if (this.gm.isPaused && !this.inputHandler.inputActions.attack.IsPressed)
        {
            this.cState.nailCharging = false;
            this.nailChargeTimer = 0f;
        }
        if (this.cState.swimming && !this.CanSwim())
        {
            this.cState.swimming = false;
        }
        if (this.parryInvulnTimer > 0f)
        {
            this.parryInvulnTimer -= Time.deltaTime;
        }
    }

    // Token: 0x060020C4 RID: 8388 RVA: 0x000C521C File Offset: 0x000C361C
    private void FixedUpdate()
    {
        if (this.cState.recoilingLeft || this.cState.recoilingRight)
        {
            if ((float)this.recoilSteps <= this.RECOIL_HOR_STEPS)
            {
                this.recoilSteps++;
            }
            else
            {
                this.CancelRecoilHorizontal();
            }
        }
        if (this.cState.dead)
        {
            this.rb2d.velocity = new Vector2(0f, 0f);
        }
        if ((this.hero_state == ActorStates.hard_landing && !this.cState.onConveyor) || this.hero_state == ActorStates.dash_landing)
        {
            this.ResetMotion();
        }
        else if (this.hero_state == ActorStates.no_input)
        {
            if (this.cState.transitioning)
            {
                if (this.transitionState == HeroTransitionState.EXITING_SCENE)
                {
                    this.AffectedByGravity(false);
                    if (!this.stopWalkingOut)
                    {
                        this.rb2d.velocity = new Vector2(this.transition_vel.x, this.transition_vel.y + this.rb2d.velocity.y);
                    }
                }
                else if (this.transitionState == HeroTransitionState.ENTERING_SCENE)
                {
                    this.rb2d.velocity = this.transition_vel;
                }
                else if (this.transitionState == HeroTransitionState.DROPPING_DOWN)
                {
                    this.rb2d.velocity = new Vector2(this.transition_vel.x, this.rb2d.velocity.y);
                }
            }
            else if (this.cState.recoiling)
            {
                this.AffectedByGravity(false);
                this.rb2d.velocity = this.recoilVector;
            }
        }
        else if (this.hero_state != ActorStates.no_input)
        {
            if (this.hero_state == ActorStates.running)
            {
                if (this.move_input > 0f)
                {
                    if (this.CheckForBump(CollisionSide.right))
                    {
                        this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, this.BUMP_VELOCITY);
                    }
                }
                else if (this.move_input < 0f && this.CheckForBump(CollisionSide.left))
                {
                    this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, this.BUMP_VELOCITY);
                }
            }
            if (!this.cState.backDashing && !this.cState.dashing)
            {
                this.Move(this.move_input);
                if ((!this.cState.attacking || this.attack_time >= this.ATTACK_RECOVERY_TIME) && !this.cState.wallSliding && !this.wallLocked)
                {
                    if (this.move_input > 0f && !this.cState.facingRight)
                    {
                        this.FlipSprite();
                        this.CancelAttack();
                    }
                    else if (this.move_input < 0f && this.cState.facingRight)
                    {
                        this.FlipSprite();
                        this.CancelAttack();
                    }
                }
                if (this.cState.recoilingLeft)
                {
                    float num;
                    if (this.recoilLarge)
                    {
                        num = this.RECOIL_HOR_VELOCITY_LONG;
                    }
                    else
                    {
                        num = this.RECOIL_HOR_VELOCITY;
                    }
                    if (this.rb2d.velocity.x > -num)
                    {
                        this.rb2d.velocity = new Vector2(-num, this.rb2d.velocity.y);
                    }
                    else
                    {
                        this.rb2d.velocity = new Vector2(this.rb2d.velocity.x - num, this.rb2d.velocity.y);
                    }
                }
                if (this.cState.recoilingRight)
                {
                    float num2;
                    if (this.recoilLarge)
                    {
                        num2 = this.RECOIL_HOR_VELOCITY_LONG;
                    }
                    else
                    {
                        num2 = this.RECOIL_HOR_VELOCITY;
                    }
                    if (this.rb2d.velocity.x < num2)
                    {
                        this.rb2d.velocity = new Vector2(num2, this.rb2d.velocity.y);
                    }
                    else
                    {
                        this.rb2d.velocity = new Vector2(this.rb2d.velocity.x + num2, this.rb2d.velocity.y);
                    }
                }
            }
            if ((this.cState.lookingUp || this.cState.lookingDown) && Mathf.Abs(this.move_input) > 0.6f)
            {
                this.ResetLook();
            }
            if (this.cState.jumping)
            {
                this.Jump();
            }
            if (this.cState.doubleJumping)
            {
                this.DoubleJump();
            }
            if (this.cState.dashing)
            {
                this.Dash();
            }
            if (this.cState.casting)
            {
                if (this.cState.castRecoiling)
                {
                    if (this.cState.facingRight)
                    {
                        this.rb2d.velocity = new Vector2(-this.CAST_RECOIL_VELOCITY, 0f);
                    }
                    else
                    {
                        this.rb2d.velocity = new Vector2(this.CAST_RECOIL_VELOCITY, 0f);
                    }
                }
                else
                {
                    this.rb2d.velocity = Vector2.zero;
                }
            }
            if (this.cState.bouncing)
            {
                this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, this.BOUNCE_VELOCITY);
            }
            if (this.cState.shroomBouncing)
            {
            }
            if (this.wallLocked)
            {
                if (this.wallJumpedR)
                {
                    this.rb2d.velocity = new Vector2(this.currentWalljumpSpeed, this.rb2d.velocity.y);
                }
                else if (this.wallJumpedL)
                {
                    this.rb2d.velocity = new Vector2(-this.currentWalljumpSpeed, this.rb2d.velocity.y);
                }
                this.wallLockSteps++;
                if (this.wallLockSteps > this.WJLOCK_STEPS_LONG)
                {
                    this.wallLocked = false;
                }
                this.currentWalljumpSpeed -= this.walljumpSpeedDecel;
            }
            if (this.cState.wallSliding)
            {
                if (this.wallSlidingL && this.inputHandler.inputActions.right.IsPressed)
                {
                    this.wallUnstickSteps++;
                }
                else if (this.wallSlidingR && this.inputHandler.inputActions.left.IsPressed)
                {
                    this.wallUnstickSteps++;
                }
                else
                {
                    this.wallUnstickSteps = 0;
                }
                if (this.wallUnstickSteps >= this.WALL_STICKY_STEPS)
                {
                    this.CancelWallsliding();
                }
                if (this.wallSlidingL)
                {
                    if (!this.CheckStillTouchingWall(CollisionSide.left, false))
                    {
                        this.FlipSprite();
                        this.CancelWallsliding();
                    }
                }
                else if (this.wallSlidingR && !this.CheckStillTouchingWall(CollisionSide.right, false))
                {
                    this.FlipSprite();
                    this.CancelWallsliding();
                }
            }
        }
        if (this.rb2d.velocity.y < -this.MAX_FALL_VELOCITY && !this.inAcid && !this.controlReqlinquished && !this.cState.shadowDashing && !this.cState.spellQuake)
        {
            this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, -this.MAX_FALL_VELOCITY);
        }
        if (this.jumpQueuing)
        {
            this.jumpQueueSteps++;
        }
        if (this.doubleJumpQueuing)
        {
            this.doubleJumpQueueSteps++;
        }
        if (this.dashQueuing)
        {
            this.dashQueueSteps++;
        }
        if (this.attackQueuing)
        {
            this.attackQueueSteps++;
        }
        if (this.cState.wallSliding && !this.cState.onConveyorV)
        {
            if (this.rb2d.velocity.y > this.WALLSLIDE_SPEED)
            {
                this.rb2d.velocity = new Vector3(this.rb2d.velocity.x, this.rb2d.velocity.y - this.WALLSLIDE_DECEL);
                if (this.rb2d.velocity.y < this.WALLSLIDE_SPEED)
                {
                    this.rb2d.velocity = new Vector3(this.rb2d.velocity.x, this.WALLSLIDE_SPEED);
                }
            }
            if (this.rb2d.velocity.y < this.WALLSLIDE_SPEED)
            {
                this.rb2d.velocity = new Vector3(this.rb2d.velocity.x, this.rb2d.velocity.y + this.WALLSLIDE_DECEL);
                if (this.rb2d.velocity.y < this.WALLSLIDE_SPEED)
                {
                    this.rb2d.velocity = new Vector3(this.rb2d.velocity.x, this.WALLSLIDE_SPEED);
                }
            }
        }
        if (this.nailArt_cyclone)
        {
            if (this.inputHandler.inputActions.right.IsPressed && !this.inputHandler.inputActions.left.IsPressed)
            {
                this.rb2d.velocity = new Vector3(this.CYCLONE_HORIZONTAL_SPEED, this.rb2d.velocity.y);
            }
            else if (this.inputHandler.inputActions.left.IsPressed && !this.inputHandler.inputActions.right.IsPressed)
            {
                this.rb2d.velocity = new Vector3(-this.CYCLONE_HORIZONTAL_SPEED, this.rb2d.velocity.y);
            }
            else
            {
                this.rb2d.velocity = new Vector3(0f, this.rb2d.velocity.y);
            }
        }
        if (this.cState.swimming)
        {
            this.rb2d.velocity = new Vector3(this.rb2d.velocity.x, this.rb2d.velocity.y + this.SWIM_ACCEL);
            if (this.rb2d.velocity.y > this.SWIM_MAX_SPEED)
            {
                this.rb2d.velocity = new Vector3(this.rb2d.velocity.x, this.SWIM_MAX_SPEED);
            }
        }
        if (this.cState.superDashOnWall && !this.cState.onConveyorV)
        {
            this.rb2d.velocity = new Vector3(0f, 0f);
        }
        if (this.cState.onConveyor && ((this.cState.onGround && !this.cState.superDashing) || this.hero_state == ActorStates.hard_landing))
        {
            if (this.cState.freezeCharge || this.hero_state == ActorStates.hard_landing || this.controlReqlinquished)
            {
                this.rb2d.velocity = new Vector3(0f, 0f);
            }
            this.rb2d.velocity = new Vector2(this.rb2d.velocity.x + this.conveyorSpeed, this.rb2d.velocity.y);
        }
        if (this.cState.inConveyorZone)
        {
            if (this.cState.freezeCharge || this.hero_state == ActorStates.hard_landing)
            {
                this.rb2d.velocity = new Vector3(0f, 0f);
            }
            this.rb2d.velocity = new Vector2(this.rb2d.velocity.x + this.conveyorSpeed, this.rb2d.velocity.y);
            this.superDash.SendEvent("SLOPE CANCEL");
        }
        if (this.cState.slidingLeft && this.rb2d.velocity.x > -5f)
        {
            this.rb2d.velocity = new Vector2(-5f, this.rb2d.velocity.y);
        }
        if (this.landingBufferSteps > 0)
        {
            this.landingBufferSteps--;
        }
        if (this.ledgeBufferSteps > 0)
        {
            this.ledgeBufferSteps--;
        }
        if (this.headBumpSteps > 0)
        {
            this.headBumpSteps--;
        }
        if (this.jumpReleaseQueueSteps > 0)
        {
            this.jumpReleaseQueueSteps--;
        }
        this.positionHistory[1] = this.positionHistory[0];
        this.positionHistory[0] = this.transform.position;
        this.cState.wasOnGround = this.cState.onGround;
    }

    // Token: 0x060020C5 RID: 8389 RVA: 0x000C6054 File Offset: 0x000C4454
    private void Update10()
    {
        if (this.isGameplayScene)
        {
            this.OutOfBoundsCheck();
        }
        float scaleX = this.transform.GetScaleX();
        if (scaleX < -1f)
        {
            this.transform.SetScaleX(-1f);
        }
        if (scaleX > 1f)
        {
            this.transform.SetScaleX(1f);
        }
        if (this.transform.position.z != 0.004f)
        {
            this.transform.SetPositionZ(0.004f);
        }
    }

    // Token: 0x060020C6 RID: 8390 RVA: 0x000C60E1 File Offset: 0x000C44E1
    private void OnLevelUnload()
    {
        if (this.transform.parent != null)
        {
            this.SetHeroParent(null);
        }
    }

    // Token: 0x060020C7 RID: 8391 RVA: 0x000C6100 File Offset: 0x000C4500
    private void OnDisable()
    {
        if (this.gm != null)
        {
            this.gm.UnloadingLevel -= this.OnLevelUnload;
        }
    }

    // Token: 0x060020C8 RID: 8392 RVA: 0x000C612C File Offset: 0x000C452C
    private void Move(float move_direction)
    {
        if (this.cState.onGround)
        {
            this.SetState(ActorStates.grounded);
        }
        if (this.acceptingInput && !this.cState.wallSliding)
        {
            if (this.cState.inWalkZone)
            {
                this.rb2d.velocity = new Vector2(move_direction * this.WALK_SPEED, this.rb2d.velocity.y);
            }
            else if (this.inAcid)
            {
                this.rb2d.velocity = new Vector2(move_direction * this.UNDERWATER_SPEED, this.rb2d.velocity.y);
            }
            else if (this.playerData.equippedCharm_37 && this.cState.onGround && this.playerData.equippedCharm_31)
            {
                this.rb2d.velocity = new Vector2(move_direction * this.RUN_SPEED_CH_COMBO, this.rb2d.velocity.y);
            }
            else if (this.playerData.equippedCharm_37 && this.cState.onGround)
            {
                this.rb2d.velocity = new Vector2(move_direction * this.RUN_SPEED_CH, this.rb2d.velocity.y);
            }
            else
            {
                this.rb2d.velocity = new Vector2(move_direction * this.RUN_SPEED, this.rb2d.velocity.y);
            }
        }
    }

    // Token: 0x060020C9 RID: 8393 RVA: 0x000C62C4 File Offset: 0x000C46C4
    private void Jump()
    {
        if (this.jump_steps <= this.JUMP_STEPS)
        {
            if (this.inAcid)
            {
                this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, this.JUMP_SPEED_UNDERWATER);
            }
            else
            {
                this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, this.JUMP_SPEED);
            }
            this.jump_steps++;
            this.jumped_steps++;
            this.ledgeBufferSteps = 0;
        }
        else
        {
            this.CancelJump();
        }
    }

    // Token: 0x060020CA RID: 8394 RVA: 0x000C6374 File Offset: 0x000C4774
    private void DoubleJump()
    {
        if (this.doubleJump_steps <= this.DOUBLE_JUMP_STEPS)
        {
            if (this.doubleJump_steps > 3)
            {
                this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, this.JUMP_SPEED * 1.1f);
            }
            this.doubleJump_steps++;
        }
        else
        {
            this.CancelDoubleJump();
        }
        if (this.cState.onGround)
        {
            this.CancelDoubleJump();
        }
    }

    // Token: 0x060020CB RID: 8395 RVA: 0x000C63FC File Offset: 0x000C47FC
    private void Attack(AttackDirection attackDir)
    {
        if (Time.timeSinceLevelLoad - this.altAttackTime > this.ALT_ATTACK_RESET)
        {
            this.cState.altAttack = false;
        }
        this.cState.attacking = true;
        if (this.playerData.equippedCharm_32)
        {
            this.attackDuration = this.ATTACK_DURATION_CH;
        }
        else
        {
            this.attackDuration = this.ATTACK_DURATION;
        }
        if (this.cState.wallSliding)
        {
            this.wallSlashing = true;
            this.slashComponent = this.wallSlash;
            this.slashFsm = this.wallSlashFsm;
            if (this.playerData.equippedCharm_35)
            {
                if ((this.playerData.health == this.playerData.CurrentMaxHealth && !this.playerData.equippedCharm_27) || (this.joniBeam && this.playerData.equippedCharm_27))
                {
                    if (this.transform.localScale.x > 0f)
                    {
                        this.grubberFlyBeam = this.grubberFlyBeamPrefabR.Spawn(this.transform.position);
                    }
                    else
                    {
                        this.grubberFlyBeam = this.grubberFlyBeamPrefabL.Spawn(this.transform.position);
                    }
                    if (this.playerData.equippedCharm_13)
                    {
                        this.grubberFlyBeam.transform.SetScaleY(this.MANTIS_CHARM_SCALE);
                    }
                    else
                    {
                        this.grubberFlyBeam.transform.SetScaleY(1f);
                    }
                }
                if (this.playerData.health == 1 && this.playerData.equippedCharm_6 && this.playerData.healthBlue < 1)
                {
                    if (this.transform.localScale.x > 0f)
                    {
                        this.grubberFlyBeam = this.grubberFlyBeamPrefabR_fury.Spawn(this.transform.position);
                    }
                    else
                    {
                        this.grubberFlyBeam = this.grubberFlyBeamPrefabL_fury.Spawn(this.transform.position);
                    }
                    if (this.playerData.equippedCharm_13)
                    {
                        this.grubberFlyBeam.transform.SetScaleY(this.MANTIS_CHARM_SCALE);
                    }
                    else
                    {
                        this.grubberFlyBeam.transform.SetScaleY(1f);
                    }
                }
            }
        }
        else
        {
            this.wallSlashing = false;
            if (attackDir == AttackDirection.normal)
            {
                if (!this.cState.altAttack)
                {
                    this.slashComponent = this.normalSlash;
                    this.slashFsm = this.normalSlashFsm;
                    this.cState.altAttack = true;
                }
                else
                {
                    this.slashComponent = this.alternateSlash;
                    this.slashFsm = this.alternateSlashFsm;
                    this.cState.altAttack = false;
                }
                if (this.playerData.equippedCharm_35)
                {
                    if ((this.playerData.health >= this.playerData.CurrentMaxHealth && !this.playerData.equippedCharm_27) || (this.joniBeam && this.playerData.equippedCharm_27))
                    {
                        if (this.transform.localScale.x < 0f)
                        {
                            this.grubberFlyBeam = this.grubberFlyBeamPrefabR.Spawn(this.transform.position);
                        }
                        else
                        {
                            this.grubberFlyBeam = this.grubberFlyBeamPrefabL.Spawn(this.transform.position);
                        }
                        if (this.playerData.equippedCharm_13)
                        {
                            this.grubberFlyBeam.transform.SetScaleY(this.MANTIS_CHARM_SCALE);
                        }
                        else
                        {
                            this.grubberFlyBeam.transform.SetScaleY(1f);
                        }
                    }
                    if (this.playerData.health == 1 && this.playerData.equippedCharm_6 && this.playerData.healthBlue < 1)
                    {
                        if (this.transform.localScale.x < 0f)
                        {
                            this.grubberFlyBeam = this.grubberFlyBeamPrefabR_fury.Spawn(this.transform.position);
                        }
                        else
                        {
                            this.grubberFlyBeam = this.grubberFlyBeamPrefabL_fury.Spawn(this.transform.position);
                        }
                        if (this.playerData.equippedCharm_13)
                        {
                            this.grubberFlyBeam.transform.SetScaleY(this.MANTIS_CHARM_SCALE);
                        }
                        else
                        {
                            this.grubberFlyBeam.transform.SetScaleY(1f);
                        }
                    }
                }
            }
            else if (attackDir == AttackDirection.upward)
            {
                this.slashComponent = this.upSlash;
                this.slashFsm = this.upSlashFsm;
                this.cState.upAttacking = true;
                if (this.playerData.equippedCharm_35)
                {
                    if ((this.playerData.health >= this.playerData.CurrentMaxHealth && !this.playerData.equippedCharm_27) || (this.joniBeam && this.playerData.equippedCharm_27))
                    {
                        this.grubberFlyBeam = this.grubberFlyBeamPrefabU.Spawn(this.transform.position);
                        this.grubberFlyBeam.transform.SetScaleY(this.transform.localScale.x);
                        this.grubberFlyBeam.transform.localEulerAngles = new Vector3(0f, 0f, 270f);
                        if (this.playerData.equippedCharm_13)
                        {
                            this.grubberFlyBeam.transform.SetScaleY(this.grubberFlyBeam.transform.localScale.y * this.MANTIS_CHARM_SCALE);
                        }
                    }
                    if (this.playerData.health == 1 && this.playerData.equippedCharm_6 && this.playerData.healthBlue < 1)
                    {
                        this.grubberFlyBeam = this.grubberFlyBeamPrefabU_fury.Spawn(this.transform.position);
                        this.grubberFlyBeam.transform.SetScaleY(this.transform.localScale.x);
                        this.grubberFlyBeam.transform.localEulerAngles = new Vector3(0f, 0f, 270f);
                        if (this.playerData.equippedCharm_13)
                        {
                            this.grubberFlyBeam.transform.SetScaleY(this.grubberFlyBeam.transform.localScale.y * this.MANTIS_CHARM_SCALE);
                        }
                    }
                }
            }
            else if (attackDir == AttackDirection.downward)
            {
                this.slashComponent = this.downSlash;
                this.slashFsm = this.downSlashFsm;
                this.cState.downAttacking = true;
                if (this.playerData.equippedCharm_35)
                {
                    if ((this.playerData.health >= this.playerData.CurrentMaxHealth && !this.playerData.equippedCharm_27) || (this.joniBeam && this.playerData.equippedCharm_27))
                    {
                        this.grubberFlyBeam = this.grubberFlyBeamPrefabD.Spawn(this.transform.position);
                        this.grubberFlyBeam.transform.SetScaleY(this.transform.localScale.x);
                        this.grubberFlyBeam.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
                        if (this.playerData.equippedCharm_13)
                        {
                            this.grubberFlyBeam.transform.SetScaleY(this.grubberFlyBeam.transform.localScale.y * this.MANTIS_CHARM_SCALE);
                        }
                    }
                    if (this.playerData.health == 1 && this.playerData.equippedCharm_6 && this.playerData.healthBlue < 1)
                    {
                        this.grubberFlyBeam = this.grubberFlyBeamPrefabD_fury.Spawn(this.transform.position);
                        this.grubberFlyBeam.transform.SetScaleY(this.transform.localScale.x);
                        this.grubberFlyBeam.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
                        if (this.playerData.equippedCharm_13)
                        {
                            this.grubberFlyBeam.transform.SetScaleY(this.grubberFlyBeam.transform.localScale.y * this.MANTIS_CHARM_SCALE);
                        }
                    }
                }
            }
        }
        if (this.cState.wallSliding)
        {
            if (this.cState.facingRight)
            {
                this.slashFsm.FsmVariables.GetFsmFloat("direction").Value = 180f;
            }
            else
            {
                this.slashFsm.FsmVariables.GetFsmFloat("direction").Value = 0f;
            }
        }
        else if (attackDir == AttackDirection.normal && this.cState.facingRight)
        {
            this.slashFsm.FsmVariables.GetFsmFloat("direction").Value = 0f;
        }
        else if (attackDir == AttackDirection.normal && !this.cState.facingRight)
        {
            this.slashFsm.FsmVariables.GetFsmFloat("direction").Value = 180f;
        }
        else if (attackDir == AttackDirection.upward)
        {
            this.slashFsm.FsmVariables.GetFsmFloat("direction").Value = 90f;
        }
        else if (attackDir == AttackDirection.downward)
        {
            this.slashFsm.FsmVariables.GetFsmFloat("direction").Value = 270f;
        }
        this.altAttackTime = Time.timeSinceLevelLoad;
        this.slashComponent.StartSlash();
        if (this.playerData.equippedCharm_38)
        {
            this.fsm_orbitShield.SendEvent("SLASH");
        }
    }

    // Token: 0x060020CC RID: 8396 RVA: 0x000C6DE0 File Offset: 0x000C51E0
    private void Dash()
    {
        this.AffectedByGravity(false);
        this.ResetHardLandingTimer();
        if (this.dash_timer > this.DASH_TIME)
        {
            this.FinishedDashing();
        }
        else
        {
            float num;
            if (this.playerData.equippedCharm_16 && this.cState.shadowDashing)
            {
                num = this.DASH_SPEED_SHARP;
            }
            else
            {
                num = this.DASH_SPEED;
            }
            if (this.dashingDown)
            {
                this.rb2d.velocity = new Vector2(0f, -num);
            }
            else if (this.cState.facingRight)
            {
                if (this.CheckForBump(CollisionSide.right))
                {
                    this.rb2d.velocity = new Vector2(num, (!this.cState.onGround) ? this.BUMP_VELOCITY_DASH : this.BUMP_VELOCITY);
                }
                else
                {
                    this.rb2d.velocity = new Vector2(num, 0f);
                }
            }
            else if (this.CheckForBump(CollisionSide.left))
            {
                this.rb2d.velocity = new Vector2(-num, (!this.cState.onGround) ? this.BUMP_VELOCITY_DASH : this.BUMP_VELOCITY);
            }
            else
            {
                this.rb2d.velocity = new Vector2(-num, 0f);
            }
            this.dash_timer += Time.deltaTime;
        }
    }

    // Token: 0x060020CD RID: 8397 RVA: 0x000C6F4B File Offset: 0x000C534B
    private void BackDash()
    {
    }

    // Token: 0x060020CE RID: 8398 RVA: 0x000C6F4D File Offset: 0x000C534D
    private void ShadowDash()
    {
    }

    // Token: 0x060020CF RID: 8399 RVA: 0x000C6F4F File Offset: 0x000C534F
    private void SuperDash()
    {
    }

    // Token: 0x060020D0 RID: 8400 RVA: 0x000C6F54 File Offset: 0x000C5354
    public void FaceRight()
    {
        this.cState.facingRight = true;
        Vector3 localScale = this.transform.localScale;
        localScale.x = -1f;
        this.transform.localScale = localScale;
    }

    // Token: 0x060020D1 RID: 8401 RVA: 0x000C6F94 File Offset: 0x000C5394
    public void FaceLeft()
    {
        this.cState.facingRight = false;
        Vector3 localScale = this.transform.localScale;
        localScale.x = 1f;
        this.transform.localScale = localScale;
    }

    // Token: 0x060020D2 RID: 8402 RVA: 0x000C6FD1 File Offset: 0x000C53D1
    public void StartMPDrain(float time)
    {
        this.drainMP = true;
        this.drainMP_timer = 0f;
        this.MP_drained = 0f;
        this.drainMP_time = time;
        this.focusMP_amount = (float)this.playerData.GetInt("focusMP_amount");
    }

    // Token: 0x060020D3 RID: 8403 RVA: 0x000C700E File Offset: 0x000C540E
    public void StopMPDrain()
    {
        this.drainMP = false;
    }

    // Token: 0x060020D4 RID: 8404 RVA: 0x000C7017 File Offset: 0x000C5417
    public void SetBackOnGround()
    {
        this.cState.onGround = true;
    }

    // Token: 0x060020D5 RID: 8405 RVA: 0x000C7025 File Offset: 0x000C5425
    public void SetStartWithWallslide()
    {
        this.startWithWallslide = true;
    }

    // Token: 0x060020D6 RID: 8406 RVA: 0x000C702E File Offset: 0x000C542E
    public void SetStartWithJump()
    {
        this.startWithJump = true;
    }

    // Token: 0x060020D7 RID: 8407 RVA: 0x000C7037 File Offset: 0x000C5437
    public void SetStartWithFullJump()
    {
        this.startWithFullJump = true;
    }

    // Token: 0x060020D8 RID: 8408 RVA: 0x000C7040 File Offset: 0x000C5440
    public void SetStartWithDash()
    {
        this.startWithDash = true;
    }

    // Token: 0x060020D9 RID: 8409 RVA: 0x000C7049 File Offset: 0x000C5449
    public void SetStartWithAttack()
    {
        this.startWithAttack = true;
    }

    // Token: 0x060020DA RID: 8410 RVA: 0x000C7052 File Offset: 0x000C5452
    public void SetSuperDashExit()
    {
        this.exitedSuperDashing = true;
    }

    // Token: 0x060020DB RID: 8411 RVA: 0x000C705B File Offset: 0x000C545B
    public void SetQuakeExit()
    {
        this.exitedQuake = true;
    }

    // Token: 0x060020DC RID: 8412 RVA: 0x000C7064 File Offset: 0x000C5464
    public void SetTakeNoDamage()
    {
        this.takeNoDamage = true;
    }

    // Token: 0x060020DD RID: 8413 RVA: 0x000C706D File Offset: 0x000C546D
    public void EndTakeNoDamage()
    {
        this.takeNoDamage = false;
    }

    // Token: 0x060020DE RID: 8414 RVA: 0x000C7076 File Offset: 0x000C5476
    public void SetHeroParent(Transform newParent)
    {
        this.transform.parent = newParent;
        if (newParent == null)
        {
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        }
    }

    // Token: 0x060020DF RID: 8415 RVA: 0x000C709B File Offset: 0x000C549B
    public void IsSwimming()
    {
        this.cState.swimming = true;
    }

    // Token: 0x060020E0 RID: 8416 RVA: 0x000C70A9 File Offset: 0x000C54A9
    public void NotSwimming()
    {
        this.cState.swimming = false;
    }

    // Token: 0x060020E1 RID: 8417 RVA: 0x000C70B7 File Offset: 0x000C54B7
    public void EnableRenderer()
    {
        this.renderer.enabled = true;
    }

    // Token: 0x060020E2 RID: 8418 RVA: 0x000C70C5 File Offset: 0x000C54C5
    public void ResetAirMoves()
    {
        this.doubleJumped = false;
        this.airDashed = false;
    }

    // Token: 0x060020E3 RID: 8419 RVA: 0x000C70D5 File Offset: 0x000C54D5
    public void SetConveyorSpeed(float speed)
    {
        this.conveyorSpeed = speed;
    }

    // Token: 0x060020E4 RID: 8420 RVA: 0x000C70DE File Offset: 0x000C54DE
    public void SetConveyorSpeedV(float speed)
    {
        this.conveyorSpeedV = speed;
    }

    // Token: 0x060020E5 RID: 8421 RVA: 0x000C70E7 File Offset: 0x000C54E7
    public void EnterWithoutInput(bool flag)
    {
        this.enterWithoutInput = flag;
    }

    // Token: 0x060020E6 RID: 8422 RVA: 0x000C70F0 File Offset: 0x000C54F0
    public void SetDarkness(int darkness)
    {
        if (darkness > 0 && this.playerData.hasLantern)
        {
            this.wieldingLantern = true;
        }
        else
        {
            this.wieldingLantern = false;
        }
    }

    // Token: 0x060020E7 RID: 8423 RVA: 0x000C712C File Offset: 0x000C552C
    public void CancelHeroJump()
    {
        if (this.cState.jumping)
        {
            this.CancelJump();
            this.CancelDoubleJump();
            if (this.rb2d.velocity.y > 0f)
            {
                this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, 0f);
            }
        }
    }

    // Token: 0x060020E8 RID: 8424 RVA: 0x000C719C File Offset: 0x000C559C
    public void CharmUpdate()
    {
        if (this.playerData.equippedCharm_26)
        {
            this.nailChargeTime = this.NAIL_CHARGE_TIME_CHARM;
        }
        else
        {
            this.nailChargeTime = this.NAIL_CHARGE_TIME_DEFAULT;
        }
        if (this.playerData.equippedCharm_23 && !this.playerData.brokenCharm_23)
        {
            this.playerData.maxHealth = this.playerData.maxHealthBase + 2;
            this.playerData.MaxHealth();
        }
        else
        {
            this.playerData.maxHealth = this.playerData.maxHealthBase;
            this.playerData.MaxHealth();
        }
        if (this.playerData.equippedCharm_27)
        {
            this.playerData.joniHealthBlue = (int)((float)this.playerData.maxHealth * 1.4f);
            this.playerData.maxHealth = 1;
            this.playerData.MaxHealth();
            this.joniBeam = true;
        }
        else
        {
            this.playerData.joniHealthBlue = 0;
        }
        if (this.playerData.equippedCharm_40 && this.playerData.grimmChildLevel == 5)
        {
            this.carefreeShieldEquipped = true;
        }
        else
        {
            this.carefreeShieldEquipped = false;
        }
        this.playerData.UpdateBlueHealth();
    }

    // Token: 0x060020E9 RID: 8425 RVA: 0x000C72DC File Offset: 0x000C56DC
    public void checkEnvironment()
    {
        if (this.playerData.environmentType == 0)
        {
            this.footStepsRunAudioSource.clip = this.footstepsRunDust;
            this.footStepsWalkAudioSource.clip = this.footstepsWalkDust;
        }
        else if (this.playerData.environmentType == 1)
        {
            this.footStepsRunAudioSource.clip = this.footstepsRunGrass;
            this.footStepsWalkAudioSource.clip = this.footstepsWalkGrass;
        }
        else if (this.playerData.environmentType == 2)
        {
            this.footStepsRunAudioSource.clip = this.footstepsRunBone;
            this.footStepsWalkAudioSource.clip = this.footstepsWalkBone;
        }
        else if (this.playerData.environmentType == 3)
        {
            this.footStepsRunAudioSource.clip = this.footstepsRunSpa;
            this.footStepsWalkAudioSource.clip = this.footstepsWalkSpa;
        }
        else if (this.playerData.environmentType == 4)
        {
            this.footStepsRunAudioSource.clip = this.footstepsRunMetal;
            this.footStepsWalkAudioSource.clip = this.footstepsWalkMetal;
        }
        else if (this.playerData.environmentType == 6)
        {
            this.footStepsRunAudioSource.clip = this.footstepsRunWater;
            this.footStepsWalkAudioSource.clip = this.footstepsRunWater;
        }
        else if (this.playerData.environmentType == 7)
        {
            this.footStepsRunAudioSource.clip = this.footstepsRunGrass;
            this.footStepsWalkAudioSource.clip = this.footstepsWalkGrass;
        }
    }

    // Token: 0x060020EA RID: 8426 RVA: 0x000C746B File Offset: 0x000C586B
    public void SetBenchRespawn(string spawnMarker, string sceneName, int spawnType, bool facingRight)
    {
        this.playerData.SetBenchRespawn(spawnMarker, sceneName, spawnType, facingRight);
    }

    // Token: 0x060020EB RID: 8427 RVA: 0x000C747D File Offset: 0x000C587D
    public void SetHazardRespawn(Vector3 position, bool facingRight)
    {
        this.playerData.SetHazardRespawn(position, facingRight);
    }

    // Token: 0x060020EC RID: 8428 RVA: 0x000C748C File Offset: 0x000C588C
    public void AddGeo(int amount)
    {
        this.playerData.AddGeo(amount);
        this.geoCounter.AddGeo(amount);
    }

    // Token: 0x060020ED RID: 8429 RVA: 0x000C74A6 File Offset: 0x000C58A6
    public void ToZero()
    {
        this.geoCounter.ToZero();
    }

    // Token: 0x060020EE RID: 8430 RVA: 0x000C74B3 File Offset: 0x000C58B3
    public void AddGeoQuietly(int amount)
    {
        this.playerData.AddGeo(amount);
    }

    // Token: 0x060020EF RID: 8431 RVA: 0x000C74C1 File Offset: 0x000C58C1
    public void AddGeoToCounter(int amount)
    {
        this.geoCounter.AddGeo(amount);
    }

    // Token: 0x060020F0 RID: 8432 RVA: 0x000C74CF File Offset: 0x000C58CF
    public void TakeGeo(int amount)
    {
        this.playerData.TakeGeo(amount);
        this.geoCounter.TakeGeo(amount);
    }

    // Token: 0x060020F1 RID: 8433 RVA: 0x000C74E9 File Offset: 0x000C58E9
    public void UpdateGeo()
    {
        this.geoCounter.UpdateGeo();
    }

    // Token: 0x060020F2 RID: 8434 RVA: 0x000C74F6 File Offset: 0x000C58F6
    public bool CanInput()
    {
        return this.acceptingInput;
    }

    // Token: 0x060020F3 RID: 8435 RVA: 0x000C7500 File Offset: 0x000C5900
    public bool CanTalk()
    {
        bool result = false;
        if (this.CanInput() && this.hero_state != ActorStates.no_input && !this.controlReqlinquished && this.cState.onGround && !this.cState.attacking && !this.cState.dashing)
        {
            result = true;
        }
        return result;
    }

    // Token: 0x060020F4 RID: 8436 RVA: 0x000C7564 File Offset: 0x000C5964
    public void FlipSprite()
    {
        this.cState.facingRight = !this.cState.facingRight;
        Vector3 localScale = this.transform.localScale;
        localScale.x *= -1f;
        this.transform.localScale = localScale;
    }

    // Token: 0x060020F5 RID: 8437 RVA: 0x000C75B5 File Offset: 0x000C59B5
    public void NailParry()
    {
        this.parryInvulnTimer = this.INVUL_TIME_PARRY;
    }

    // Token: 0x060020F6 RID: 8438 RVA: 0x000C75C3 File Offset: 0x000C59C3
    public void NailParryRecover()
    {
        this.attackDuration = 0f;
        this.attack_cooldown = 0f;
        this.CancelAttack();
    }

    // Token: 0x060020F7 RID: 8439 RVA: 0x000C75E1 File Offset: 0x000C59E1
    public void QuakeInvuln()
    {
        this.parryInvulnTimer = this.INVUL_TIME_QUAKE;
    }

    // Token: 0x060020F8 RID: 8440 RVA: 0x000C75EF File Offset: 0x000C59EF
    public void CancelParryInvuln()
    {
        this.parryInvulnTimer = 0f;
    }

    // Token: 0x060020F9 RID: 8441 RVA: 0x000C75FC File Offset: 0x000C59FC
    public void CycloneInvuln()
    {
        this.parryInvulnTimer = this.INVUL_TIME_CYCLONE;
    }

    // Token: 0x060020FA RID: 8442 RVA: 0x000C760A File Offset: 0x000C5A0A
    public void SetWieldingLantern(bool set)
    {
        this.wieldingLantern = set;
    }

    // Token: 0x060020FB RID: 8443 RVA: 0x000C7614 File Offset: 0x000C5A14
    public void TakeDamage(GameObject go, CollisionSide damageSide, int damageAmount, int hazardType)
    {
        bool spawnDamageEffect = true;
        if (damageAmount > 0)
        {
            if (BossSceneController.IsBossScene)
            {
                int bossLevel = BossSceneController.Instance.BossLevel;
                if (bossLevel != 1)
                {
                    if (bossLevel == 2)
                    {
                        damageAmount = 9999;
                    }
                }
                else
                {
                    damageAmount *= 2;
                }
            }
            if (this.CanTakeDamage())
            {
                if (this.damageMode == DamageMode.HAZARD_ONLY && hazardType == 1)
                {
                    return;
                }
                if (this.cState.shadowDashing && hazardType == 1)
                {
                    return;
                }
                if (this.parryInvulnTimer > 0f && hazardType == 1)
                {
                    return;
                }
                VibrationMixer mixer = VibrationManager.GetMixer();
                if (mixer != null)
                {
                    mixer.StopAllEmissionsWithTag("heroAction");
                }
                bool flag = false;
                if (this.carefreeShieldEquipped && hazardType == 1)
                {
                    if (this.hitsSinceShielded > 7)
                    {
                        this.hitsSinceShielded = 7;
                    }
                    switch (this.hitsSinceShielded)
                    {
                        case 1:
                            if ((float)UnityEngine.Random.Range(1, 100) <= 10f)
                            {
                                flag = true;
                            }
                            break;
                        case 2:
                            if ((float)UnityEngine.Random.Range(1, 100) <= 20f)
                            {
                                flag = true;
                            }
                            break;
                        case 3:
                            if ((float)UnityEngine.Random.Range(1, 100) <= 30f)
                            {
                                flag = true;
                            }
                            break;
                        case 4:
                            if ((float)UnityEngine.Random.Range(1, 100) <= 50f)
                            {
                                flag = true;
                            }
                            break;
                        case 5:
                            if ((float)UnityEngine.Random.Range(1, 100) <= 70f)
                            {
                                flag = true;
                            }
                            break;
                        case 6:
                            if ((float)UnityEngine.Random.Range(1, 100) <= 80f)
                            {
                                flag = true;
                            }
                            break;
                        case 7:
                            if ((float)UnityEngine.Random.Range(1, 100) <= 90f)
                            {
                                flag = true;
                            }
                            break;
                        default:
                            flag = false;
                            break;
                    }
                    if (flag)
                    {
                        this.hitsSinceShielded = 0;
                        this.carefreeShield.SetActive(true);
                        damageAmount = 0;
                        spawnDamageEffect = false;
                    }
                    else
                    {
                        this.hitsSinceShielded++;
                    }
                }
                if (this.playerData.equippedCharm_5 && this.playerData.blockerHits > 0 && hazardType == 1 && this.cState.focusing && !flag)
                {
                    this.proxyFSM.SendEvent("HeroCtrl-TookBlockerHit");
                    this.audioSource.PlayOneShot(this.blockerImpact, 1f);
                    spawnDamageEffect = false;
                    damageAmount = 0;
                }
                else
                {
                    this.proxyFSM.SendEvent("HeroCtrl-HeroDamaged");
                }
                this.CancelAttack();
                if (this.cState.wallSliding)
                {
                    this.cState.wallSliding = false;
                    this.wallSlideVibrationPlayer.Stop();
                }
                if (this.cState.touchingWall)
                {
                    this.cState.touchingWall = false;
                }
                if (this.cState.recoilingLeft || this.cState.recoilingRight)
                {
                    this.CancelRecoilHorizontal();
                }
                if (this.cState.bouncing)
                {
                    this.CancelBounce();
                    this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, 0f);
                }
                if (this.cState.shroomBouncing)
                {
                    this.CancelBounce();
                    this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, 0f);
                }
                if (!flag)
                {
                    this.audioCtrl.PlaySound(HeroSounds.TAKE_HIT);
                }
                if (!this.takeNoDamage && !this.playerData.invinciTest)
                {
                    if (this.playerData.overcharmed)
                    {
                        this.playerData.TakeHealth(damageAmount * 2);
                    }
                    else
                    {
                        this.playerData.TakeHealth(damageAmount);
                    }
                }
                if (this.playerData.equippedCharm_3 && damageAmount > 0)
                {
                    if (this.playerData.equippedCharm_35)
                    {
                        this.AddMPCharge(this.GRUB_SOUL_MP_COMBO);
                    }
                    else
                    {
                        this.AddMPCharge(this.GRUB_SOUL_MP);
                    }
                }
                if (this.joniBeam && damageAmount > 0)
                {
                    this.joniBeam = false;
                }
                if (this.cState.nailCharging || this.nailChargeTimer != 0f)
                {
                    this.cState.nailCharging = false;
                    this.nailChargeTimer = 0f;
                }
                if (damageAmount > 0 && this.OnTakenDamage != null)
                {
                    this.OnTakenDamage();
                }
                if (this.playerData.health == 0)
                {
                    base.StartCoroutine(this.Die());
                }
                else if (hazardType == 2)
                {
                    base.StartCoroutine(this.DieFromHazard(HazardType.SPIKES, (!(go != null)) ? 0f : go.transform.rotation.z));
                }
                else if (hazardType == 3)
                {
                    base.StartCoroutine(this.DieFromHazard(HazardType.ACID, 0f));
                }
                else if (hazardType == 4)
                {
                    UnityEngine.Debug.Log("Lava death");
                }
                else if (hazardType == 5)
                {
                    base.StartCoroutine(this.DieFromHazard(HazardType.PIT, 0f));
                }
                else
                {
                    base.StartCoroutine(this.StartRecoil(damageSide, spawnDamageEffect, damageAmount));
                }
            }
            else if (this.cState.invulnerable && !this.cState.hazardDeath && !this.playerData.isInvincible)
            {
                if (hazardType == 2)
                {
                    if (!this.takeNoDamage)
                    {
                        this.playerData.TakeHealth(damageAmount);
                    }
                    this.proxyFSM.SendEvent("HeroCtrl-HeroDamaged");
                    if (this.playerData.health == 0)
                    {
                        base.StartCoroutine(this.Die());
                    }
                    else
                    {
                        this.audioCtrl.PlaySound(HeroSounds.TAKE_HIT);
                        base.StartCoroutine(this.DieFromHazard(HazardType.SPIKES, (!(go != null)) ? 0f : go.transform.rotation.z));
                    }
                }
                else if (hazardType == 3)
                {
                    this.playerData.TakeHealth(damageAmount);
                    this.proxyFSM.SendEvent("HeroCtrl-HeroDamaged");
                    if (this.playerData.health == 0)
                    {
                        base.StartCoroutine(this.Die());
                    }
                    else
                    {
                        base.StartCoroutine(this.DieFromHazard(HazardType.ACID, 0f));
                    }
                }
                else if (hazardType == 4)
                {
                    UnityEngine.Debug.Log("Lava damage");
                }
            }
        }
    }

    // Token: 0x060020FC RID: 8444 RVA: 0x000C7C9F File Offset: 0x000C609F
    public string GetEntryGateName()
    {
        if (this.sceneEntryGate != null)
        {
            return this.sceneEntryGate.name;
        }
        return string.Empty;
    }

    // Token: 0x060020FD RID: 8445 RVA: 0x000C7CC4 File Offset: 0x000C60C4
    public void AddMPCharge(int amount)
    {
        int mpreserve = this.playerData.MPReserve;
        this.playerData.AddMPCharge(amount);
        GameCameras.instance.soulOrbFSM.SendEvent("MP GAIN");
        if (this.playerData.MPReserve != mpreserve && this.gm && this.gm.soulVessel_fsm)
        {
            this.gm.soulVessel_fsm.SendEvent("MP RESERVE UP");
        }
    }

    // Token: 0x060020FE RID: 8446 RVA: 0x000C7D4C File Offset: 0x000C614C
    public void SoulGain()
    {
        int mpcharge = this.playerData.MPCharge;
        int num;
        if (mpcharge < this.playerData.maxMP)
        {
            num = 11;
            if (this.playerData.equippedCharm_20)
            {
                num += 3;
            }
            if (this.playerData.equippedCharm_21)
            {
                num += 8;
            }
        }
        else
        {
            num = 6;
            if (this.playerData.equippedCharm_20)
            {
                num += 2;
            }
            if (this.playerData.equippedCharm_21)
            {
                num += 6;
            }
        }
        int mpreserve = this.playerData.MPReserve;
        this.playerData.AddMPCharge(num);
        GameCameras.instance.soulOrbFSM.SendEvent("MP GAIN");
        if (this.playerData.MPReserve != mpreserve)
        {
            this.gm.soulVessel_fsm.SendEvent("MP RESERVE UP");
        }
    }

    // Token: 0x060020FF RID: 8447 RVA: 0x000C7E23 File Offset: 0x000C6223
    public void AddMPChargeSpa(int amount)
    {
        this.TryAddMPChargeSpa(amount);
    }

    // Token: 0x06002100 RID: 8448 RVA: 0x000C7E30 File Offset: 0x000C6230
    public bool TryAddMPChargeSpa(int amount)
    {
        int mpreserve = this.playerData.MPReserve;
        bool result = this.playerData.AddMPCharge(amount);
        this.gm.soulOrb_fsm.SendEvent("MP GAIN SPA");
        if (this.playerData.MPReserve != mpreserve)
        {
            this.gm.soulVessel_fsm.SendEvent("MP RESERVE UP");
        }
        return result;
    }

    // Token: 0x06002101 RID: 8449 RVA: 0x000C7E92 File Offset: 0x000C6292
    public void SetMPCharge(int amount)
    {
        this.playerData.MPCharge = amount;
        GameCameras.instance.soulOrbFSM.SendEvent("MP SET");
    }

    // Token: 0x06002102 RID: 8450 RVA: 0x000C7EB4 File Offset: 0x000C62B4
    public void TakeMP(int amount)
    {
        if (this.playerData.MPCharge > 0)
        {
            this.playerData.TakeMP(amount);
            if (amount > 1)
            {
                GameCameras.instance.soulOrbFSM.SendEvent("MP LOSE");
            }
        }
    }

    // Token: 0x06002103 RID: 8451 RVA: 0x000C7EEE File Offset: 0x000C62EE
    public void TakeMPQuick(int amount)
    {
        if (this.playerData.MPCharge > 0)
        {
            this.playerData.TakeMP(amount);
            if (amount > 1)
            {
                GameCameras.instance.soulOrbFSM.SendEvent("MP DRAIN");
            }
        }
    }

    // Token: 0x06002104 RID: 8452 RVA: 0x000C7F28 File Offset: 0x000C6328
    public void TakeReserveMP(int amount)
    {
        this.playerData.TakeReserveMP(amount);
        this.gm.soulVessel_fsm.SendEvent("MP RESERVE DOWN");
    }

    // Token: 0x06002105 RID: 8453 RVA: 0x000C7F4B File Offset: 0x000C634B
    public void AddHealth(int amount)
    {
        this.playerData.AddHealth(amount);
        this.proxyFSM.SendEvent("HeroCtrl-Healed");
    }

    // Token: 0x06002106 RID: 8454 RVA: 0x000C7F69 File Offset: 0x000C6369
    public void TakeHealth(int amount)
    {
        this.playerData.TakeHealth(amount);
        this.proxyFSM.SendEvent("HeroCtrl-HeroDamaged");
    }

    // Token: 0x06002107 RID: 8455 RVA: 0x000C7F87 File Offset: 0x000C6387
    public void MaxHealth()
    {
        this.proxyFSM.SendEvent("HeroCtrl-MaxHealth");
        this.playerData.MaxHealth();
    }

    // Token: 0x06002108 RID: 8456 RVA: 0x000C7FA4 File Offset: 0x000C63A4
    public void MaxHealthKeepBlue()
    {
        int healthBlue = this.playerData.healthBlue;
        this.playerData.MaxHealth();
        this.playerData.healthBlue = healthBlue;
        this.proxyFSM.SendEvent("HeroCtrl-Healed");
    }

    // Token: 0x06002109 RID: 8457 RVA: 0x000C7FE4 File Offset: 0x000C63E4
    public void AddToMaxHealth(int amount)
    {
        this.playerData.AddToMaxHealth(amount);
        this.gm.AwardAchievement("PROTECTED");
        if (this.playerData.maxHealthBase == this.playerData.maxHealthCap)
        {
            this.gm.AwardAchievement("MASKED");
        }
    }

    // Token: 0x0600210A RID: 8458 RVA: 0x000C8038 File Offset: 0x000C6438
    public void ClearMP()
    {
        this.playerData.ClearMP();
    }

    // Token: 0x0600210B RID: 8459 RVA: 0x000C8045 File Offset: 0x000C6445
    public void ClearMPSendEvents()
    {
        this.ClearMP();
        GameManager.instance.soulOrb_fsm.SendEvent("MP LOSE");
        GameManager.instance.soulVessel_fsm.SendEvent("MP RESERVE DOWN");
    }

    // Token: 0x0600210C RID: 8460 RVA: 0x000C8078 File Offset: 0x000C6478
    public void AddToMaxMPReserve(int amount)
    {
        this.playerData.AddToMaxMPReserve(amount);
        this.gm.AwardAchievement("SOULFUL");
        if (this.playerData.MPReserveMax == this.playerData.MPReserveCap)
        {
            this.gm.AwardAchievement("WORLDSOUL");
        }
    }

    // Token: 0x0600210D RID: 8461 RVA: 0x000C80CC File Offset: 0x000C64CC
    public void Bounce()
    {
        if (!this.cState.bouncing && !this.cState.shroomBouncing && !this.controlReqlinquished)
        {
            this.doubleJumped = false;
            this.airDashed = false;
            this.cState.bouncing = true;
        }
    }

    // Token: 0x0600210E RID: 8462 RVA: 0x000C8120 File Offset: 0x000C6520
    public void BounceHigh()
    {
        if (!this.cState.bouncing && !this.controlReqlinquished)
        {
            this.doubleJumped = false;
            this.airDashed = false;
            this.cState.bouncing = true;
            this.bounceTimer = -0.03f;
            this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, this.BOUNCE_VELOCITY);
        }
    }

    // Token: 0x0600210F RID: 8463 RVA: 0x000C8198 File Offset: 0x000C6598
    public void ShroomBounce()
    {
        this.doubleJumped = false;
        this.airDashed = false;
        this.cState.bouncing = false;
        this.cState.shroomBouncing = true;
        this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, this.SHROOM_BOUNCE_VELOCITY);
    }

    // Token: 0x06002110 RID: 8464 RVA: 0x000C81F4 File Offset: 0x000C65F4
    public void RecoilLeft()
    {
        if (!this.cState.recoilingLeft && !this.cState.recoilingRight && !this.playerData.equippedCharm_14 && !this.controlReqlinquished)
        {
            this.CancelDash();
            this.recoilSteps = 0;
            this.cState.recoilingLeft = true;
            this.cState.recoilingRight = false;
            this.recoilLarge = false;
            this.rb2d.velocity = new Vector2(-this.RECOIL_HOR_VELOCITY, this.rb2d.velocity.y);
        }
    }

    // Token: 0x06002111 RID: 8465 RVA: 0x000C8294 File Offset: 0x000C6694
    public void RecoilRight()
    {
        if (!this.cState.recoilingLeft && !this.cState.recoilingRight && !this.playerData.equippedCharm_14 && !this.controlReqlinquished)
        {
            this.CancelDash();
            this.recoilSteps = 0;
            this.cState.recoilingRight = true;
            this.cState.recoilingLeft = false;
            this.recoilLarge = false;
            this.rb2d.velocity = new Vector2(this.RECOIL_HOR_VELOCITY, this.rb2d.velocity.y);
        }
    }

    // Token: 0x06002112 RID: 8466 RVA: 0x000C8334 File Offset: 0x000C6734
    public void RecoilRightLong()
    {
        if (!this.cState.recoilingLeft && !this.cState.recoilingRight && !this.controlReqlinquished)
        {
            this.CancelDash();
            this.ResetAttacks();
            this.recoilSteps = 0;
            this.cState.recoilingRight = true;
            this.cState.recoilingLeft = false;
            this.recoilLarge = true;
            this.rb2d.velocity = new Vector2(this.RECOIL_HOR_VELOCITY_LONG, this.rb2d.velocity.y);
        }
    }

    // Token: 0x06002113 RID: 8467 RVA: 0x000C83C8 File Offset: 0x000C67C8
    public void RecoilLeftLong()
    {
        if (!this.cState.recoilingLeft && !this.cState.recoilingRight && !this.controlReqlinquished)
        {
            this.CancelDash();
            this.ResetAttacks();
            this.recoilSteps = 0;
            this.cState.recoilingRight = false;
            this.cState.recoilingLeft = true;
            this.recoilLarge = true;
            this.rb2d.velocity = new Vector2(-this.RECOIL_HOR_VELOCITY_LONG, this.rb2d.velocity.y);
        }
    }

    // Token: 0x06002114 RID: 8468 RVA: 0x000C845C File Offset: 0x000C685C
    public void RecoilDown()
    {
        this.CancelJump();
        if (this.rb2d.velocity.y > this.RECOIL_DOWN_VELOCITY && !this.controlReqlinquished)
        {
            this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, this.RECOIL_DOWN_VELOCITY);
        }
    }

    // Token: 0x06002115 RID: 8469 RVA: 0x000C84C1 File Offset: 0x000C68C1
    public void ForceHardLanding()
    {
        if (!this.cState.onGround)
        {
            this.cState.willHardLand = true;
        }
    }

    // Token: 0x06002116 RID: 8470 RVA: 0x000C84E0 File Offset: 0x000C68E0
    public void EnterSceneDreamGate()
    {
        this.IgnoreInputWithoutReset();
        this.ResetMotion();
        this.airDashed = false;
        this.doubleJumped = false;
        this.ResetHardLandingTimer();
        this.ResetAttacksDash();
        this.AffectedByGravity(false);
        this.sceneEntryGate = null;
        this.SetState(ActorStates.no_input);
        this.transitionState = HeroTransitionState.WAITING_TO_ENTER_LEVEL;
        this.vignetteFSM.SendEvent("RESET");
        if (this.heroInPosition != null)
        {
            this.heroInPosition(false);
        }
        this.FinishedEnteringScene(true, false);
    }

    // Token: 0x06002117 RID: 8471 RVA: 0x000C8560 File Offset: 0x000C6960
    public IEnumerator EnterScene(TransitionPoint enterGate, float delayBeforeEnter)
    {
        this.IgnoreInputWithoutReset();
        this.ResetMotion();
        this.airDashed = false;
        this.doubleJumped = false;
        this.ResetHardLandingTimer();
        this.ResetAttacksDash();
        this.AffectedByGravity(false);
        this.sceneEntryGate = enterGate;
        this.SetState(ActorStates.no_input);
        this.transitionState = HeroTransitionState.WAITING_TO_ENTER_LEVEL;
        this.vignetteFSM.SendEvent("RESET");
        if (!this.cState.transitioning)
        {
            this.cState.transitioning = true;
        }
        this.gatePosition = enterGate.GetGatePosition();
        if (this.gatePosition == GatePosition.top)
        {
            this.cState.onGround = false;
            this.enteringVertically = true;
            this.exitedSuperDashing = false;
            float x = enterGate.transform.position.x + enterGate.entryOffset.x;
            float y = enterGate.transform.position.y + enterGate.entryOffset.y;
            this.transform.SetPosition2D(x, y);
            if (this.heroInPosition != null)
            {
                this.heroInPosition(false);
            }
            yield return new WaitForSeconds(0.165f);
            if (!enterGate.customFade)
            {
                this.gm.FadeSceneIn();
            }
            if (delayBeforeEnter > 0f)
            {
                yield return new WaitForSeconds(delayBeforeEnter);
            }
            if (enterGate.entryDelay > 0f)
            {
                yield return new WaitForSeconds(enterGate.entryDelay);
            }
            yield return new WaitForSeconds(0.4f);
            if (this.exitedQuake)
            {
                this.IgnoreInput();
                this.proxyFSM.SendEvent("HeroCtrl-EnterQuake");
                yield return new WaitForSeconds(0.25f);
                this.FinishedEnteringScene(true, false);
            }
            else
            {
                this.rb2d.velocity = new Vector2(0f, this.SPEED_TO_ENTER_SCENE_DOWN);
                this.transitionState = HeroTransitionState.ENTERING_SCENE;
                this.transitionState = HeroTransitionState.DROPPING_DOWN;
                this.AffectedByGravity(true);
                if (enterGate.hardLandOnExit)
                {
                    this.cState.willHardLand = true;
                }
                yield return new WaitForSeconds(0.33f);
                this.transitionState = HeroTransitionState.ENTERING_SCENE;
                if (this.transitionState != HeroTransitionState.WAITING_TO_TRANSITION)
                {
                    this.FinishedEnteringScene(true, false);
                }
            }
        }
        else if (this.gatePosition == GatePosition.bottom)
        {
            this.cState.onGround = false;
            this.enteringVertically = true;
            this.exitedSuperDashing = false;
            if (enterGate.alwaysEnterRight)
            {
                this.FaceRight();
            }
            if (enterGate.alwaysEnterLeft)
            {
                this.FaceLeft();
            }
            float x2 = enterGate.transform.position.x + enterGate.entryOffset.x;
            float y2 = enterGate.transform.position.y + enterGate.entryOffset.y + 3f;
            this.transform.SetPosition2D(x2, y2);
            if (this.heroInPosition != null)
            {
                this.heroInPosition(false);
            }
            yield return new WaitForSeconds(0.165f);
            if (delayBeforeEnter > 0f)
            {
                yield return new WaitForSeconds(delayBeforeEnter);
            }
            if (enterGate.entryDelay > 0f)
            {
                yield return new WaitForSeconds(enterGate.entryDelay);
            }
            yield return new WaitForSeconds(0.4f);
            if (!enterGate.customFade)
            {
                this.gm.FadeSceneIn();
            }
            if (this.cState.facingRight)
            {
                this.transition_vel = new Vector2(this.SPEED_TO_ENTER_SCENE_HOR, this.SPEED_TO_ENTER_SCENE_UP);
            }
            else
            {
                this.transition_vel = new Vector2(-this.SPEED_TO_ENTER_SCENE_HOR, this.SPEED_TO_ENTER_SCENE_UP);
            }
            this.transitionState = HeroTransitionState.ENTERING_SCENE;
            this.transform.SetPosition2D(x2, y2);
            yield return new WaitForSeconds(this.TIME_TO_ENTER_SCENE_BOT);
            this.transition_vel = new Vector2(this.rb2d.velocity.x, 0f);
            this.AffectedByGravity(true);
            this.transitionState = HeroTransitionState.DROPPING_DOWN;
        }
        else if (this.gatePosition == GatePosition.left)
        {
            this.cState.onGround = true;
            this.enteringVertically = false;
            this.SetState(ActorStates.no_input);
            float x3 = enterGate.transform.position.x + enterGate.entryOffset.x;
            float y3 = this.FindGroundPointY(x3 + 2f, enterGate.transform.position.y, false);
            this.transform.SetPosition2D(x3, y3);
            if (this.heroInPosition != null)
            {
                this.heroInPosition(true);
            }
            this.FaceRight();
            yield return new WaitForSeconds(0.165f);
            if (!enterGate.customFade)
            {
                this.gm.FadeSceneIn();
            }
            if (delayBeforeEnter > 0f)
            {
                yield return new WaitForSeconds(delayBeforeEnter);
            }
            if (enterGate.entryDelay > 0f)
            {
                yield return new WaitForSeconds(enterGate.entryDelay);
            }
            yield return new WaitForSeconds(0.4f);
            if (this.exitedSuperDashing)
            {
                this.IgnoreInput();
                this.proxyFSM.SendEvent("HeroCtrl-EnterSuperDash");
                yield return new WaitForSeconds(0.25f);
                this.FinishedEnteringScene(true, false);
            }
            else
            {
                this.transition_vel = new Vector2(this.RUN_SPEED, 0f);
                this.transitionState = HeroTransitionState.ENTERING_SCENE;
                yield return new WaitForSeconds(0.33f);
                this.FinishedEnteringScene(true, true);
            }
        }
        else if (this.gatePosition == GatePosition.right)
        {
            this.cState.onGround = true;
            this.enteringVertically = false;
            this.SetState(ActorStates.no_input);
            float x4 = enterGate.transform.position.x + enterGate.entryOffset.x;
            float y4 = this.FindGroundPointY(x4 - 2f, enterGate.transform.position.y, false);
            this.transform.SetPosition2D(x4, y4);
            if (this.heroInPosition != null)
            {
                this.heroInPosition(true);
            }
            this.FaceLeft();
            yield return new WaitForSeconds(0.165f);
            if (!enterGate.customFade)
            {
                this.gm.FadeSceneIn();
            }
            if (delayBeforeEnter > 0f)
            {
                yield return new WaitForSeconds(delayBeforeEnter);
            }
            if (enterGate.entryDelay > 0f)
            {
                yield return new WaitForSeconds(enterGate.entryDelay);
            }
            yield return new WaitForSeconds(0.4f);
            if (this.exitedSuperDashing)
            {
                this.IgnoreInput();
                this.proxyFSM.SendEvent("HeroCtrl-EnterSuperDash");
                yield return new WaitForSeconds(0.25f);
                this.FinishedEnteringScene(true, false);
            }
            else
            {
                this.transition_vel = new Vector2(-this.RUN_SPEED, 0f);
                this.transitionState = HeroTransitionState.ENTERING_SCENE;
                yield return new WaitForSeconds(0.33f);
                this.FinishedEnteringScene(true, true);
            }
        }
        else if (this.gatePosition == GatePosition.door)
        {
            if (enterGate.alwaysEnterRight)
            {
                this.FaceRight();
            }
            if (enterGate.alwaysEnterLeft)
            {
                this.FaceLeft();
            }
            this.cState.onGround = true;
            this.enteringVertically = false;
            this.SetState(ActorStates.idle);
            this.SetState(ActorStates.no_input);
            this.exitedSuperDashing = false;
            this.animCtrl.PlayClip("Idle");
            this.transform.SetPosition2D(this.FindGroundPoint(enterGate.transform.position, false));
            if (this.heroInPosition != null)
            {
                this.heroInPosition(false);
            }
            yield return new WaitForEndOfFrame();
            if (delayBeforeEnter > 0f)
            {
                yield return new WaitForSeconds(delayBeforeEnter);
            }
            if (enterGate.entryDelay > 0f)
            {
                yield return new WaitForSeconds(enterGate.entryDelay);
            }
            yield return new WaitForSeconds(0.4f);
            if (!enterGate.customFade)
            {
                this.gm.FadeSceneIn();
            }
            float startTime = Time.realtimeSinceStartup;
            if (enterGate.dontWalkOutOfDoor)
            {
                yield return new WaitForSeconds(0.33f);
            }
            else
            {
                float clipLength = this.animCtrl.GetClipDuration("Exit Door To Idle");
                this.animCtrl.PlayClip("Exit Door To Idle");
                if (clipLength > 0f)
                {
                    yield return new WaitForSeconds(clipLength);
                }
                else
                {
                    yield return new WaitForSeconds(0.33f);
                }
            }
            this.FinishedEnteringScene(true, false);
        }
        yield break;
    }

    // Token: 0x06002118 RID: 8472 RVA: 0x000C858C File Offset: 0x000C698C
    public void LeaveScene(GatePosition? gate = null)
    {
        this.isHeroInPosition = false;
        this.IgnoreInputWithoutReset();
        this.ResetHardLandingTimer();
        this.SetState(ActorStates.no_input);
        this.SetDamageMode(DamageMode.NO_DAMAGE);
        this.transitionState = HeroTransitionState.EXITING_SCENE;
        this.CancelFallEffects();
        this.tilemapTestActive = false;
        this.SetHeroParent(null);
        this.StopTilemapTest();
        if (gate != null)
        {
            switch (gate.Value)
            {
                case GatePosition.top:
                    this.transition_vel = new Vector2(0f, this.MIN_JUMP_SPEED);
                    this.cState.onGround = false;
                    break;
                case GatePosition.right:
                    this.transition_vel = new Vector2(this.RUN_SPEED, 0f);
                    break;
                case GatePosition.left:
                    this.transition_vel = new Vector2(-this.RUN_SPEED, 0f);
                    break;
                case GatePosition.bottom:
                    this.transition_vel = Vector2.zero;
                    this.cState.onGround = false;
                    break;
            }
        }
        this.cState.transitioning = true;
    }

    // Token: 0x06002119 RID: 8473 RVA: 0x000C8690 File Offset: 0x000C6A90
    public IEnumerator BetaLeave(EndBeta betaEndTrigger)
    {
        if (!this.playerData.betaEnd)
        {
            this.endBeta = betaEndTrigger;
            this.IgnoreInput();
            this.playerData.disablePause = true;
            this.SetState(ActorStates.no_input);
            this.ResetInput();
            this.tilemapTestActive = false;
            yield return new WaitForSeconds(0.66f);
            GameObject.Find("Beta Ender").GetComponent<SimpleSpriteFade>().FadeIn();
            this.ResetMotion();
            yield return new WaitForSeconds(1.25f);
            this.playerData.betaEnd = true;
        }
        yield break;
    }

    // Token: 0x0600211A RID: 8474 RVA: 0x000C86B4 File Offset: 0x000C6AB4
    public IEnumerator BetaReturn()
    {
        this.rb2d.velocity = new Vector2(this.RUN_SPEED, 0f);
        if (!this.cState.facingRight)
        {
            this.FlipSprite();
        }
        GameObject.Find("Beta Ender").GetComponent<SimpleSpriteFade>().FadeOut();
        this.animCtrl.PlayClip("Run");
        yield return new WaitForSeconds(1.4f);
        this.SetState(ActorStates.grounded);
        this.SetStartingMotionState();
        this.AcceptInput();
        this.playerData.betaEnd = false;
        this.playerData.disablePause = false;
        this.tilemapTestActive = true;
        if (this.endBeta != null)
        {
            this.endBeta.Reactivate();
        }
        yield break;
    }

    // Token: 0x0600211B RID: 8475 RVA: 0x000C86D0 File Offset: 0x000C6AD0
    public IEnumerator Respawn()
    {
        this.playerData = PlayerData.instance;
        this.playerData.disablePause = true;
        base.gameObject.layer = 9;
        this.renderer.enabled = true;
        this.rb2d.isKinematic = false;
        this.cState.dead = false;
        this.cState.onGround = true;
        this.cState.hazardDeath = false;
        this.cState.recoiling = false;
        this.enteringVertically = false;
        this.airDashed = false;
        this.doubleJumped = false;
        this.CharmUpdate();
        this.MaxHealth();
        this.ClearMP();
        this.ResetMotion();
        this.ResetHardLandingTimer();
        this.ResetAttacks();
        this.ResetInput();
        this.CharmUpdate();
        Transform spawnPoint = this.LocateSpawnPoint();
        if (spawnPoint != null)
        {
            this.transform.SetPosition2D(this.FindGroundPoint(spawnPoint.transform.position, false));
            PlayMakerFSM component = spawnPoint.GetComponent<PlayMakerFSM>();
            if (component != null)
            {
                Vector3 vector = FSMUtility.GetVector3(component, "Adjust Vector");
            }
            else if (this.verboseMode)
            {
                UnityEngine.Debug.Log("Could not find Bench Control FSM on respawn point. Ignoring Adjustment offset.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Couldn't find the respawn point named " + this.playerData.respawnMarkerName + " within objects tagged with RespawnPoint");
        }
        if (this.verboseMode)
        {
            UnityEngine.Debug.Log("HC Respawn Type: " + this.playerData.respawnType);
        }
        GameCameras.instance.cameraFadeFSM.SendEvent("RESPAWN");
        if (this.playerData.respawnType == 1)
        {
            this.AffectedByGravity(false);
            PlayMakerFSM benchFSM = FSMUtility.LocateFSM(spawnPoint.gameObject, "Bench Control");
            if (benchFSM == null)
            {
                UnityEngine.Debug.LogError("HeroCtrl: Could not find Bench Control FSM on this spawn point, respawn type is set to Bench");
                yield break;
            }
            benchFSM.FsmVariables.GetFsmBool("RespawnResting").Value = true;
            yield return new WaitForEndOfFrame();
            if (this.heroInPosition != null)
            {
                this.heroInPosition(false);
            }
            this.proxyFSM.SendEvent("HeroCtrl-Respawned");
            this.FinishedEnteringScene(true, false);
            benchFSM.SendEvent("RESPAWN");
        }
        else
        {
            yield return new WaitForEndOfFrame();
            this.IgnoreInput();
            RespawnMarker respawnMarker = spawnPoint.GetComponent<RespawnMarker>();
            if (respawnMarker)
            {
                if (respawnMarker.respawnFacingRight)
                {
                    this.FaceRight();
                }
                else
                {
                    this.FaceLeft();
                }
            }
            else
            {
                UnityEngine.Debug.LogError("Spawn point does not contain a RespawnMarker");
            }
            if (this.heroInPosition != null)
            {
                this.heroInPosition(false);
            }
            if (this.gm.GetSceneNameString() != "GG_Atrium")
            {
                float clipLength = this.animCtrl.GetClipDuration("Wake Up Ground");
                this.animCtrl.PlayClip("Wake Up Ground");
                this.StopAnimationControl();
                this.controlReqlinquished = true;
                yield return new WaitForSeconds(clipLength);
                this.StartAnimationControl();
                this.controlReqlinquished = false;
            }
            this.proxyFSM.SendEvent("HeroCtrl-Respawned");
            this.FinishedEnteringScene(true, false);
        }
        this.playerData.disablePause = false;
        this.playerData.isInvincible = false;
        yield break;
    }

    // Token: 0x0600211C RID: 8476 RVA: 0x000C86EC File Offset: 0x000C6AEC
    public IEnumerator HazardRespawn()
    {
        this.cState.hazardDeath = false;
        this.cState.onGround = true;
        this.cState.hazardRespawning = true;
        this.ResetMotion();
        this.ResetHardLandingTimer();
        this.ResetAttacks();
        this.ResetInput();
        this.cState.recoiling = false;
        this.enteringVertically = false;
        this.airDashed = false;
        this.doubleJumped = false;
        this.transform.SetPosition2D(this.FindGroundPoint(this.playerData.hazardRespawnLocation, true));
        base.gameObject.layer = 9;
        this.renderer.enabled = true;
        yield return new WaitForEndOfFrame();
        if (this.playerData.hazardRespawnFacingRight)
        {
            this.FaceRight();
        }
        else
        {
            this.FaceLeft();
        }
        if (this.heroInPosition != null)
        {
            this.heroInPosition(false);
        }
        base.StartCoroutine(this.Invulnerable(this.INVUL_TIME * 2f));
        GameCameras.instance.cameraFadeFSM.SendEvent("RESPAWN");
        float clipLength = this.animCtrl.GetClipDuration("Hazard Respawn");
        this.animCtrl.PlayClip("Hazard Respawn");
        yield return new WaitForSeconds(clipLength);
        this.cState.hazardRespawning = false;
        this.rb2d.interpolation = RigidbodyInterpolation2D.Interpolate;
        this.FinishedEnteringScene(false, false);
        yield break;
    }

    // Token: 0x0600211D RID: 8477 RVA: 0x000C8707 File Offset: 0x000C6B07
    public void StartCyclone()
    {
        this.nailArt_cyclone = true;
    }

    // Token: 0x0600211E RID: 8478 RVA: 0x000C8710 File Offset: 0x000C6B10
    public void EndCyclone()
    {
        this.nailArt_cyclone = false;
    }

    // Token: 0x0600211F RID: 8479 RVA: 0x000C8719 File Offset: 0x000C6B19
    public bool GetState(string stateName)
    {
        return this.cState.GetState(stateName);
    }

    // Token: 0x06002120 RID: 8480 RVA: 0x000C8727 File Offset: 0x000C6B27
    public bool GetCState(string stateName)
    {
        return this.cState.GetState(stateName);
    }

    // Token: 0x06002121 RID: 8481 RVA: 0x000C8735 File Offset: 0x000C6B35
    public void SetCState(string stateName, bool value)
    {
        this.cState.SetState(stateName, value);
    }

    // Token: 0x06002122 RID: 8482 RVA: 0x000C8744 File Offset: 0x000C6B44
    public void ResetHardLandingTimer()
    {
        this.cState.willHardLand = false;
        this.hardLandingTimer = 0f;
        this.fallTimer = 0f;
        this.hardLanded = false;
    }

    // Token: 0x06002123 RID: 8483 RVA: 0x000C876F File Offset: 0x000C6B6F
    public void CancelSuperDash()
    {
        this.superDash.SendEvent("SLOPE CANCEL");
    }

    // Token: 0x06002124 RID: 8484 RVA: 0x000C8784 File Offset: 0x000C6B84
    public void RelinquishControlNotVelocity()
    {
        if (!this.controlReqlinquished)
        {
            this.prev_hero_state = ActorStates.idle;
            this.ResetInput();
            this.ResetMotionNotVelocity();
            this.SetState(ActorStates.no_input);
            this.IgnoreInput();
            this.controlReqlinquished = true;
            this.ResetLook();
            this.ResetAttacks();
            this.touchingWallL = false;
            this.touchingWallR = false;
        }
    }

    // Token: 0x06002125 RID: 8485 RVA: 0x000C87E0 File Offset: 0x000C6BE0
    public void RelinquishControl()
    {
        if (!this.controlReqlinquished && !this.cState.dead)
        {
            this.ResetInput();
            this.ResetMotion();
            this.IgnoreInput();
            this.controlReqlinquished = true;
            this.ResetLook();
            this.ResetAttacks();
            this.touchingWallL = false;
            this.touchingWallR = false;
        }
    }

    // Token: 0x06002126 RID: 8486 RVA: 0x000C883C File Offset: 0x000C6C3C
    public void RegainControl()
    {
        this.enteringVertically = false;
        this.doubleJumpQueuing = false;
        this.AcceptInput();
        this.hero_state = ActorStates.idle;
        if (this.controlReqlinquished && !this.cState.dead)
        {
            this.AffectedByGravity(true);
            this.SetStartingMotionState();
            this.controlReqlinquished = false;
            if (this.startWithWallslide)
            {
                this.wallSlideVibrationPlayer.Play();
                this.cState.wallSliding = true;
                this.cState.willHardLand = false;
                this.cState.touchingWall = true;
                this.airDashed = false;
                this.wallslideDustPrefab.enableEmission = true;
                this.startWithWallslide = false;
                if (this.transform.localScale.x < 0f)
                {
                    this.wallSlidingR = true;
                    this.touchingWallR = true;
                }
                else
                {
                    this.wallSlidingL = true;
                    this.touchingWallL = true;
                }
            }
            else if (this.startWithJump)
            {
                this.HeroJumpNoEffect();
                this.doubleJumpQueuing = false;
                this.startWithJump = false;
            }
            else if (this.startWithFullJump)
            {
                this.HeroJump();
                this.doubleJumpQueuing = false;
                this.startWithFullJump = false;
            }
            else if (this.startWithDash)
            {
                this.HeroDash();
                this.doubleJumpQueuing = false;
                this.startWithDash = false;
            }
            else if (this.startWithAttack)
            {
                this.DoAttack();
                this.doubleJumpQueuing = false;
                this.startWithAttack = false;
            }
            else
            {
                this.cState.touchingWall = false;
                this.touchingWallL = false;
                this.touchingWallR = false;
            }
        }
    }

    // Token: 0x06002127 RID: 8487 RVA: 0x000C89D4 File Offset: 0x000C6DD4
    public void PreventCastByDialogueEnd()
    {
        this.preventCastByDialogueEndTimer = 0.3f;
    }

    // Token: 0x06002128 RID: 8488 RVA: 0x000C89E4 File Offset: 0x000C6DE4
    public bool CanCast()
    {
        return !this.gm.isPaused && !this.cState.dashing && this.hero_state != ActorStates.no_input && !this.cState.backDashing && (!this.cState.attacking || this.attack_time >= this.ATTACK_RECOVERY_TIME) && !this.cState.recoiling && !this.cState.recoilFrozen && !this.cState.transitioning && !this.cState.hazardDeath && !this.cState.hazardRespawning && this.CanInput() && this.preventCastByDialogueEndTimer <= 0f;
    }

    // Token: 0x06002129 RID: 8489 RVA: 0x000C8ABC File Offset: 0x000C6EBC
    public bool CanFocus()
    {
        return !this.gm.isPaused && this.hero_state != ActorStates.no_input && !this.cState.dashing && !this.cState.backDashing && (!this.cState.attacking || this.attack_time >= this.ATTACK_RECOVERY_TIME) && !this.cState.recoiling && this.cState.onGround && !this.cState.transitioning && !this.cState.recoilFrozen && !this.cState.hazardDeath && !this.cState.hazardRespawning && this.CanInput();
    }

    // Token: 0x0600212A RID: 8490 RVA: 0x000C8B94 File Offset: 0x000C6F94
    public bool CanNailArt()
    {
        if (!this.cState.transitioning && this.hero_state != ActorStates.no_input && !this.cState.attacking && !this.cState.hazardDeath && !this.cState.hazardRespawning && this.nailChargeTimer >= this.nailChargeTime)
        {
            this.nailChargeTimer = 0f;
            return true;
        }
        this.nailChargeTimer = 0f;
        return false;
    }

    // Token: 0x0600212B RID: 8491 RVA: 0x000C8C18 File Offset: 0x000C7018
    public bool CanQuickMap()
    {
        return !this.gm.isPaused && !this.controlReqlinquished && this.hero_state != ActorStates.no_input && !this.cState.onConveyor && !this.cState.dashing && !this.cState.backDashing && (!this.cState.attacking || this.attack_time >= this.ATTACK_RECOVERY_TIME) && !this.cState.recoiling && !this.cState.transitioning && !this.cState.hazardDeath && !this.cState.hazardRespawning && !this.cState.recoilFrozen && this.cState.onGround && this.CanInput();
    }

    // Token: 0x0600212C RID: 8492 RVA: 0x000C8D0C File Offset: 0x000C710C
    public bool CanInspect()
    {
        return !this.gm.isPaused && !this.cState.dashing && this.hero_state != ActorStates.no_input && !this.cState.backDashing && (!this.cState.attacking || this.attack_time >= this.ATTACK_RECOVERY_TIME) && !this.cState.recoiling && !this.cState.transitioning && !this.cState.hazardDeath && !this.cState.hazardRespawning && !this.cState.recoilFrozen && this.cState.onGround && this.CanInput();
    }

    // Token: 0x0600212D RID: 8493 RVA: 0x000C8DE4 File Offset: 0x000C71E4
    public bool CanBackDash()
    {
        return !this.gm.isPaused && !this.cState.dashing && this.hero_state != ActorStates.no_input && !this.cState.backDashing && (!this.cState.attacking || this.attack_time >= this.ATTACK_RECOVERY_TIME) && !this.cState.preventBackDash && !this.cState.backDashCooldown && !this.controlReqlinquished && !this.cState.recoilFrozen && !this.cState.recoiling && !this.cState.transitioning && this.cState.onGround && this.playerData.canBackDash;
    }

    // Token: 0x0600212E RID: 8494 RVA: 0x000C8ECC File Offset: 0x000C72CC
    public bool CanSuperDash()
    {
        return !this.gm.isPaused && this.hero_state != ActorStates.no_input && !this.cState.dashing && !this.cState.hazardDeath && !this.cState.hazardRespawning && !this.cState.backDashing && (!this.cState.attacking || this.attack_time >= this.ATTACK_RECOVERY_TIME) && !this.cState.slidingLeft && !this.cState.slidingRight && !this.controlReqlinquished && !this.cState.recoilFrozen && !this.cState.recoiling && !this.cState.transitioning && this.playerData.hasSuperDash && (this.cState.onGround || this.cState.wallSliding);
    }

    // Token: 0x0600212F RID: 8495 RVA: 0x000C8FE4 File Offset: 0x000C73E4
    public bool CanDreamNail()
    {
        return !this.gm.isPaused && this.hero_state != ActorStates.no_input && !this.cState.dashing && !this.cState.backDashing && (!this.cState.attacking || this.attack_time >= this.ATTACK_RECOVERY_TIME) && !this.controlReqlinquished && !this.cState.hazardDeath && this.rb2d.velocity.y > -0.1f && !this.cState.hazardRespawning && !this.cState.recoilFrozen && !this.cState.recoiling && !this.cState.transitioning && this.playerData.hasDreamNail && this.cState.onGround;
    }

    // Token: 0x06002130 RID: 8496 RVA: 0x000C90EC File Offset: 0x000C74EC
    public bool CanDreamGate()
    {
        return !this.gm.isPaused && this.hero_state != ActorStates.no_input && !this.cState.dashing && !this.cState.backDashing && (!this.cState.attacking || this.attack_time >= this.ATTACK_RECOVERY_TIME) && !this.controlReqlinquished && !this.cState.hazardDeath && !this.cState.hazardRespawning && !this.cState.recoilFrozen && !this.cState.recoiling && !this.cState.transitioning && this.playerData.hasDreamGate && this.cState.onGround;
    }

    // Token: 0x06002131 RID: 8497 RVA: 0x000C91D4 File Offset: 0x000C75D4
    public bool CanInteract()
    {
        return this.CanInput() && this.hero_state != ActorStates.no_input && !this.gm.isPaused && !this.cState.dashing && !this.cState.backDashing && !this.cState.attacking && !this.controlReqlinquished && !this.cState.hazardDeath && !this.cState.hazardRespawning && !this.cState.recoilFrozen && !this.cState.recoiling && !this.cState.transitioning && this.cState.onGround;
    }

    // Token: 0x06002132 RID: 8498 RVA: 0x000C92A8 File Offset: 0x000C76A8
    public bool CanOpenInventory()
    {
        return (!this.gm.isPaused && this.hero_state != ActorStates.airborne && !this.controlReqlinquished && !this.cState.recoiling && !this.cState.transitioning && !this.cState.hazardDeath && !this.cState.hazardRespawning && !this.playerData.disablePause && this.CanInput()) || this.playerData.atBench;
    }

    // Token: 0x06002133 RID: 8499 RVA: 0x000C934A File Offset: 0x000C774A
    public void SetDamageMode(int invincibilityType)
    {
        if (invincibilityType != 0)
        {
            if (invincibilityType != 1)
            {
                if (invincibilityType == 2)
                {
                    this.damageMode = DamageMode.NO_DAMAGE;
                }
            }
            else
            {
                this.damageMode = DamageMode.HAZARD_ONLY;
            }
        }
        else
        {
            this.damageMode = DamageMode.FULL_DAMAGE;
        }
    }

    // Token: 0x06002134 RID: 8500 RVA: 0x000C9389 File Offset: 0x000C7789
    public void SetDamageModeFSM(int invincibilityType)
    {
        if (invincibilityType != 0)
        {
            if (invincibilityType != 1)
            {
                if (invincibilityType == 2)
                {
                    this.damageMode = DamageMode.NO_DAMAGE;
                }
            }
            else
            {
                this.damageMode = DamageMode.HAZARD_ONLY;
            }
        }
        else
        {
            this.damageMode = DamageMode.FULL_DAMAGE;
        }
    }

    // Token: 0x06002135 RID: 8501 RVA: 0x000C93C8 File Offset: 0x000C77C8
    public void ResetQuakeDamage()
    {
        if (this.damageMode == DamageMode.HAZARD_ONLY)
        {
            this.damageMode = DamageMode.FULL_DAMAGE;
        }
    }

    // Token: 0x06002136 RID: 8502 RVA: 0x000C93DD File Offset: 0x000C77DD
    public void SetDamageMode(DamageMode newDamageMode)
    {
        this.damageMode = newDamageMode;
        if (newDamageMode == DamageMode.NO_DAMAGE)
        {
            this.playerData.isInvincible = true;
        }
        else
        {
            this.playerData.isInvincible = false;
        }
    }

    // Token: 0x06002137 RID: 8503 RVA: 0x000C940A File Offset: 0x000C780A
    public void StopAnimationControl()
    {
        this.animCtrl.StopControl();
    }

    // Token: 0x06002138 RID: 8504 RVA: 0x000C9417 File Offset: 0x000C7817
    public void StartAnimationControl()
    {
        this.animCtrl.StartControl();
    }

    // Token: 0x06002139 RID: 8505 RVA: 0x000C9424 File Offset: 0x000C7824
    public void IgnoreInput()
    {
        if (this.acceptingInput)
        {
            this.acceptingInput = false;
            this.ResetInput();
        }
    }

    // Token: 0x0600213A RID: 8506 RVA: 0x000C943E File Offset: 0x000C783E
    public void IgnoreInputWithoutReset()
    {
        if (this.acceptingInput)
        {
            this.acceptingInput = false;
        }
    }

    // Token: 0x0600213B RID: 8507 RVA: 0x000C9452 File Offset: 0x000C7852
    public void AcceptInput()
    {
        this.acceptingInput = true;
    }

    // Token: 0x0600213C RID: 8508 RVA: 0x000C945B File Offset: 0x000C785B
    public void Pause()
    {
        this.PauseInput();
        this.PauseAudio();
        this.JumpReleased();
        this.cState.isPaused = true;
    }

    // Token: 0x0600213D RID: 8509 RVA: 0x000C947B File Offset: 0x000C787B
    public void UnPause()
    {
        this.cState.isPaused = false;
        this.UnPauseAudio();
        this.UnPauseInput();
    }

    // Token: 0x0600213E RID: 8510 RVA: 0x000C9495 File Offset: 0x000C7895
    public void NearBench(bool isNearBench)
    {
        this.cState.nearBench = isNearBench;
    }

    // Token: 0x0600213F RID: 8511 RVA: 0x000C94A3 File Offset: 0x000C78A3
    public void SetWalkZone(bool inWalkZone)
    {
        this.cState.inWalkZone = inWalkZone;
    }

    // Token: 0x06002140 RID: 8512 RVA: 0x000C94B1 File Offset: 0x000C78B1
    public void ResetState()
    {
        this.cState.Reset();
    }

    // Token: 0x06002141 RID: 8513 RVA: 0x000C94BE File Offset: 0x000C78BE
    public void StopPlayingAudio()
    {
        this.audioCtrl.StopAllSounds();
    }

    // Token: 0x06002142 RID: 8514 RVA: 0x000C94CB File Offset: 0x000C78CB
    public void PauseAudio()
    {
        this.audioCtrl.PauseAllSounds();
    }

    // Token: 0x06002143 RID: 8515 RVA: 0x000C94D8 File Offset: 0x000C78D8
    public void UnPauseAudio()
    {
        this.audioCtrl.UnPauseAllSounds();
    }

    // Token: 0x06002144 RID: 8516 RVA: 0x000C94E5 File Offset: 0x000C78E5
    private void PauseInput()
    {
        if (this.acceptingInput)
        {
            this.acceptingInput = false;
        }
        this.lastInputState = new Vector2(this.move_input, this.vertical_input);
    }

    // Token: 0x06002145 RID: 8517 RVA: 0x000C9510 File Offset: 0x000C7910
    private void UnPauseInput()
    {
        if (!this.controlReqlinquished)
        {
            if (this.inputHandler.inputActions.right.IsPressed)
            {
                this.move_input = this.lastInputState.x;
            }
            else if (this.inputHandler.inputActions.left.IsPressed)
            {
                this.move_input = this.lastInputState.x;
            }
            else
            {
                this.rb2d.velocity = new Vector2(0f, this.rb2d.velocity.y);
                this.move_input = 0f;
            }
            this.vertical_input = this.lastInputState.y;
            this.acceptingInput = true;
        }
    }

    // Token: 0x06002146 RID: 8518 RVA: 0x000C95D3 File Offset: 0x000C79D3
    public void SpawnSoftLandingPrefab()
    {
        this.softLandingEffectPrefab.Spawn(this.transform.position);
    }

    // Token: 0x06002147 RID: 8519 RVA: 0x000C95EC File Offset: 0x000C79EC
    public void AffectedByGravity(bool gravityApplies)
    {
        float gravityScale = this.rb2d.gravityScale;
        if (this.rb2d.gravityScale > Mathf.Epsilon && !gravityApplies)
        {
            this.prevGravityScale = this.rb2d.gravityScale;
            this.rb2d.gravityScale = 0f;
        }
        else if (this.rb2d.gravityScale <= Mathf.Epsilon && gravityApplies)
        {
            this.rb2d.gravityScale = this.prevGravityScale;
            this.prevGravityScale = 0f;
        }
    }

    // Token: 0x06002148 RID: 8520 RVA: 0x000C9680 File Offset: 0x000C7A80
    private void LookForInput()
    {
        if (this.acceptingInput && !this.gm.isPaused && this.isGameplayScene)
        {
            this.move_input = this.inputHandler.inputActions.moveVector.Vector.x;
            this.vertical_input = this.inputHandler.inputActions.moveVector.Vector.y;
            this.FilterInput();
            if (this.playerData.hasWalljump && this.CanWallSlide() && !this.cState.attacking)
            {
                if (this.touchingWallL && this.inputHandler.inputActions.left.IsPressed && !this.cState.wallSliding)
                {
                    this.airDashed = false;
                    this.doubleJumped = false;
                    this.wallSlideVibrationPlayer.Play();
                    this.cState.wallSliding = true;
                    this.cState.willHardLand = false;
                    this.wallslideDustPrefab.enableEmission = true;
                    this.wallSlidingL = true;
                    this.wallSlidingR = false;
                    this.FaceLeft();
                    this.CancelFallEffects();
                }
                if (this.touchingWallR && this.inputHandler.inputActions.right.IsPressed && !this.cState.wallSliding)
                {
                    this.airDashed = false;
                    this.doubleJumped = false;
                    this.wallSlideVibrationPlayer.Play();
                    this.cState.wallSliding = true;
                    this.cState.willHardLand = false;
                    this.wallslideDustPrefab.enableEmission = true;
                    this.wallSlidingL = false;
                    this.wallSlidingR = true;
                    this.FaceRight();
                    this.CancelFallEffects();
                }
            }
            if (this.cState.wallSliding && this.inputHandler.inputActions.down.WasPressed)
            {
                this.CancelWallsliding();
                this.FlipSprite();
            }
            if (this.wallLocked && this.wallJumpedL && this.inputHandler.inputActions.right.IsPressed && this.wallLockSteps >= this.WJLOCK_STEPS_SHORT)
            {
                this.wallLocked = false;
            }
            if (this.wallLocked && this.wallJumpedR && this.inputHandler.inputActions.left.IsPressed && this.wallLockSteps >= this.WJLOCK_STEPS_SHORT)
            {
                this.wallLocked = false;
            }
            if (this.inputHandler.inputActions.jump.WasReleased && this.jumpReleaseQueueingEnabled)
            {
                this.jumpReleaseQueueSteps = this.JUMP_RELEASE_QUEUE_STEPS;
                this.jumpReleaseQueuing = true;
            }
            if (!this.inputHandler.inputActions.jump.IsPressed)
            {
                this.JumpReleased();
            }
            if (!this.inputHandler.inputActions.dash.IsPressed)
            {
                if (this.cState.preventDash && !this.cState.dashCooldown)
                {
                    this.cState.preventDash = false;
                }
                this.dashQueuing = false;
            }
            if (!this.inputHandler.inputActions.attack.IsPressed)
            {
                this.attackQueuing = false;
            }
        }
    }

    // Token: 0x06002149 RID: 8521 RVA: 0x000C99D0 File Offset: 0x000C7DD0
    private void LookForQueueInput()
    {
        if (this.acceptingInput && !this.gm.isPaused && this.isGameplayScene)
        {
            if (this.inputHandler.inputActions.jump.WasPressed)
            {
                if (this.CanWallJump())
                {
                    this.DoWallJump();
                }
                else if (this.CanJump())
                {
                    this.HeroJump();
                }
                else if (this.CanDoubleJump())
                {
                    this.DoDoubleJump();
                }
                else if (this.CanInfiniteAirJump())
                {
                    this.CancelJump();
                    this.audioCtrl.PlaySound(HeroSounds.JUMP);
                    this.ResetLook();
                    this.cState.jumping = true;
                }
                else
                {
                    this.jumpQueueSteps = 0;
                    this.jumpQueuing = true;
                    this.doubleJumpQueueSteps = 0;
                    this.doubleJumpQueuing = true;
                }
            }
            if (this.inputHandler.inputActions.dash.WasPressed)
            {
                if (this.CanDash())
                {
                    this.HeroDash();
                }
                else
                {
                    this.dashQueueSteps = 0;
                    this.dashQueuing = true;
                }
            }
            if (this.inputHandler.inputActions.attack.WasPressed)
            {
                if (this.CanAttack())
                {
                    this.DoAttack();
                }
                else
                {
                    this.attackQueueSteps = 0;
                    this.attackQueuing = true;
                }
            }
            if (this.inputHandler.inputActions.jump.IsPressed)
            {
                if (this.jumpQueueSteps <= this.JUMP_QUEUE_STEPS && this.CanJump() && this.jumpQueuing)
                {
                    this.HeroJump();
                }
                else if (this.doubleJumpQueueSteps <= this.DOUBLE_JUMP_QUEUE_STEPS && this.CanDoubleJump() && this.doubleJumpQueuing)
                {
                    if (this.cState.onGround)
                    {
                        this.HeroJump();
                    }
                    else
                    {
                        this.DoDoubleJump();
                    }
                }
                if (this.CanSwim())
                {
                    if (this.hero_state != ActorStates.airborne)
                    {
                        this.SetState(ActorStates.airborne);
                    }
                    this.cState.swimming = true;
                }
            }
            if (this.inputHandler.inputActions.dash.IsPressed && this.dashQueueSteps <= this.DASH_QUEUE_STEPS && this.CanDash() && this.dashQueuing)
            {
                this.HeroDash();
            }
            if (this.inputHandler.inputActions.attack.IsPressed && this.attackQueueSteps <= this.ATTACK_QUEUE_STEPS && this.CanAttack() && this.attackQueuing)
            {
                this.DoAttack();
            }
        }
    }

    // Token: 0x0600214A RID: 8522 RVA: 0x000C9C78 File Offset: 0x000C8078
    private void HeroJump()
    {
        this.jumpEffectPrefab.Spawn(this.transform.position);
        this.audioCtrl.PlaySound(HeroSounds.JUMP);
        this.ResetLook();
        this.cState.recoiling = false;
        this.cState.jumping = true;
        this.jumpQueueSteps = 0;
        this.jumped_steps = 0;
        this.doubleJumpQueuing = false;
    }

    // Token: 0x0600214B RID: 8523 RVA: 0x000C9CDB File Offset: 0x000C80DB
    private void HeroJumpNoEffect()
    {
        this.ResetLook();
        this.jump_steps = 5;
        this.cState.jumping = true;
        this.jumpQueueSteps = 0;
        this.jumped_steps = 0;
        this.jump_steps = 5;
    }

    // Token: 0x0600214C RID: 8524 RVA: 0x000C9D0C File Offset: 0x000C810C
    private void DoWallJump()
    {
        this.wallPuffPrefab.SetActive(true);
        this.audioCtrl.PlaySound(HeroSounds.WALLJUMP);
        VibrationManager.PlayVibrationClipOneShot(this.wallJumpVibration, null, false, string.Empty);
        if (this.touchingWallL)
        {
            this.FaceRight();
            this.wallJumpedR = true;
            this.wallJumpedL = false;
        }
        else if (this.touchingWallR)
        {
            this.FaceLeft();
            this.wallJumpedR = false;
            this.wallJumpedL = true;
        }
        this.CancelWallsliding();
        this.cState.touchingWall = false;
        this.touchingWallL = false;
        this.touchingWallR = false;
        this.airDashed = false;
        this.doubleJumped = false;
        this.currentWalljumpSpeed = this.WJ_KICKOFF_SPEED;
        this.walljumpSpeedDecel = (this.WJ_KICKOFF_SPEED - this.RUN_SPEED) / (float)this.WJLOCK_STEPS_LONG;
        this.dashBurst.SendEvent("CANCEL");
        this.cState.jumping = true;
        this.wallLockSteps = 0;
        this.wallLocked = true;
        this.jumpQueueSteps = 0;
        this.jumped_steps = 0;
    }

    // Token: 0x0600214D RID: 8525 RVA: 0x000C9E1C File Offset: 0x000C821C
    private void DoDoubleJump()
    {
        this.dJumpWingsPrefab.SetActive(true);
        this.dJumpFlashPrefab.SetActive(true);
        this.dJumpFeathers.Play();
        VibrationManager.PlayVibrationClipOneShot(this.doubleJumpVibration, null, false, string.Empty);
        this.audioSource.PlayOneShot(this.doubleJumpClip, 1f);
        this.ResetLook();
        this.cState.jumping = false;
        this.cState.doubleJumping = true;
        this.doubleJump_steps = 0;
        this.doubleJumped = true;
    }

    // Token: 0x0600214E RID: 8526 RVA: 0x000C9EAC File Offset: 0x000C82AC
    private void DoHardLanding()
    {
        this.AffectedByGravity(true);
        this.ResetInput();
        this.SetState(ActorStates.hard_landing);
        this.CancelAttack();
        this.hardLanded = true;
        this.audioCtrl.PlaySound(HeroSounds.HARD_LANDING);
        this.hardLandingEffectPrefab.Spawn(this.transform.position);
    }

    // Token: 0x0600214F RID: 8527 RVA: 0x000C9F00 File Offset: 0x000C8300
    private void DoAttack()
    {
        this.ResetLook();
        this.cState.recoiling = false;
        if (this.playerData.equippedCharm_32)
        {
            this.attack_cooldown = this.ATTACK_COOLDOWN_TIME_CH;
        }
        else
        {
            this.attack_cooldown = this.ATTACK_COOLDOWN_TIME;
        }
        if (this.vertical_input > Mathf.Epsilon)
        {
            this.Attack(AttackDirection.upward);
            base.StartCoroutine(this.CheckForTerrainThunk(AttackDirection.upward));
        }
        else if (this.vertical_input < -Mathf.Epsilon)
        {
            if (this.hero_state != ActorStates.idle && this.hero_state != ActorStates.running)
            {
                this.Attack(AttackDirection.downward);
                base.StartCoroutine(this.CheckForTerrainThunk(AttackDirection.downward));
            }
            else
            {
                this.Attack(AttackDirection.normal);
                base.StartCoroutine(this.CheckForTerrainThunk(AttackDirection.normal));
            }
        }
        else
        {
            this.Attack(AttackDirection.normal);
            base.StartCoroutine(this.CheckForTerrainThunk(AttackDirection.normal));
        }
    }

    // Token: 0x06002150 RID: 8528 RVA: 0x000C9FE8 File Offset: 0x000C83E8
    private void HeroDash()
    {
        if (!this.cState.onGround && !this.inAcid)
        {
            this.airDashed = true;
        }
        this.ResetAttacksDash();
        this.CancelBounce();
        this.audioCtrl.StopSound(HeroSounds.FOOTSTEPS_RUN);
        this.audioCtrl.StopSound(HeroSounds.FOOTSTEPS_WALK);
        this.audioCtrl.PlaySound(HeroSounds.DASH);
        this.ResetLook();
        this.cState.recoiling = false;
        if (this.cState.wallSliding)
        {
            this.FlipSprite();
        }
        else if (this.inputHandler.inputActions.right.IsPressed)
        {
            this.FaceRight();
        }
        else if (this.inputHandler.inputActions.left.IsPressed)
        {
            this.FaceLeft();
        }
        this.cState.dashing = true;
        this.dashQueueSteps = 0;
        if (!this.cState.onGround && this.inputHandler.inputActions.down.IsPressed && this.playerData.equippedCharm_31)
        {
            this.dashBurst.transform.localPosition = new Vector3(-0.07f, 3.74f, 0.01f);
            this.dashBurst.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
            this.dashingDown = true;
        }
        else
        {
            this.dashBurst.transform.localPosition = new Vector3(4.11f, -0.55f, 0.001f);
            this.dashBurst.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            this.dashingDown = false;
        }
        if (this.playerData.equippedCharm_31)
        {
            this.dashCooldownTimer = this.DASH_COOLDOWN_CH;
        }
        else
        {
            this.dashCooldownTimer = this.DASH_COOLDOWN;
        }
        if (this.playerData.hasShadowDash && this.shadowDashTimer <= 0f)
        {
            this.shadowDashTimer = this.SHADOW_DASH_COOLDOWN;
            this.cState.shadowDashing = true;
            if (this.playerData.equippedCharm_16)
            {
                this.audioSource.PlayOneShot(this.sharpShadowClip, 1f);
                this.sharpShadowPrefab.SetActive(true);
            }
            else
            {
                this.audioSource.PlayOneShot(this.shadowDashClip, 1f);
            }
        }
        if (this.cState.shadowDashing)
        {
            if (this.dashingDown)
            {
                this.dashEffect = this.shadowdashDownBurstPrefab.Spawn(new Vector3(this.transform.position.x, this.transform.position.y + 3.5f, this.transform.position.z + 0.00101f));
                this.dashEffect.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
            }
            else if (this.transform.localScale.x > 0f)
            {
                this.dashEffect = this.shadowdashBurstPrefab.Spawn(new Vector3(this.transform.position.x + 5.21f, this.transform.position.y - 0.58f, this.transform.position.z + 0.00101f));
                this.dashEffect.transform.localScale = new Vector3(1.919591f, this.dashEffect.transform.localScale.y, this.dashEffect.transform.localScale.z);
            }
            else
            {
                this.dashEffect = this.shadowdashBurstPrefab.Spawn(new Vector3(this.transform.position.x - 5.21f, this.transform.position.y - 0.58f, this.transform.position.z + 0.00101f));
                this.dashEffect.transform.localScale = new Vector3(-1.919591f, this.dashEffect.transform.localScale.y, this.dashEffect.transform.localScale.z);
            }
            this.shadowRechargePrefab.SetActive(true);
            FSMUtility.LocateFSM(this.shadowRechargePrefab, "Recharge Effect").SendEvent("RESET");
            this.shadowdashParticlesPrefab.GetComponent<ParticleSystem>().enableEmission = true;
            VibrationManager.PlayVibrationClipOneShot(this.shadowDashVibration, null, false, string.Empty);
            this.shadowRingPrefab.Spawn(this.transform.position);
        }
        else
        {
            this.dashBurst.SendEvent("PLAY");
            this.dashParticlesPrefab.GetComponent<ParticleSystem>().enableEmission = true;
            VibrationManager.PlayVibrationClipOneShot(this.dashVibration, null, false, string.Empty);
        }
        if (this.cState.onGround && !this.cState.shadowDashing)
        {
            this.dashEffect = this.backDashPrefab.Spawn(this.transform.position);
            this.dashEffect.transform.localScale = new Vector3(this.transform.localScale.x * -1f, this.transform.localScale.y, this.transform.localScale.z);
        }
    }

    // Token: 0x06002151 RID: 8529 RVA: 0x000CA5BB File Offset: 0x000C89BB
    private void StartFallRumble()
    {
        this.fallRumble = true;
        this.audioCtrl.PlaySound(HeroSounds.FALLING);
        GameCameras.instance.cameraShakeFSM.Fsm.Variables.FindFsmBool("RumblingFall").Value = true;
    }

    // Token: 0x06002152 RID: 8530 RVA: 0x000CA5F8 File Offset: 0x000C89F8
    private void SetState(ActorStates newState)
    {
        if (newState == ActorStates.grounded)
        {
            if (Mathf.Abs(this.move_input) > Mathf.Epsilon)
            {
                newState = ActorStates.running;
            }
            else
            {
                newState = ActorStates.idle;
            }
        }
        else if (newState == ActorStates.previous)
        {
            newState = this.prev_hero_state;
        }
        if (newState != this.hero_state)
        {
            this.prev_hero_state = this.hero_state;
            this.hero_state = newState;
            this.animCtrl.UpdateState(newState);
        }
    }

    // Token: 0x06002153 RID: 8531 RVA: 0x000CA66C File Offset: 0x000C8A6C
    private void FinishedEnteringScene(bool setHazardMarker = true, bool preventRunBob = false)
    {
        if (this.isEnteringFirstLevel)
        {
            this.isEnteringFirstLevel = false;
        }
        else
        {
            this.playerData.disablePause = false;
        }
        this.cState.transitioning = false;
        this.transitionState = HeroTransitionState.WAITING_TO_TRANSITION;
        this.stopWalkingOut = false;
        if (this.exitedSuperDashing || this.exitedQuake)
        {
            this.controlReqlinquished = true;
            this.IgnoreInput();
        }
        else
        {
            this.SetStartingMotionState(preventRunBob);
            this.AffectedByGravity(true);
        }
        if (setHazardMarker)
        {
            if (this.gm.startedOnThisScene || this.sceneEntryGate == null)
            {
                this.playerData.SetHazardRespawn(this.transform.position, this.cState.facingRight);
            }
            else if (!this.sceneEntryGate.nonHazardGate)
            {
                this.playerData.SetHazardRespawn(this.sceneEntryGate.respawnMarker);
            }
        }
        if (this.exitedQuake)
        {
            this.SetDamageMode(DamageMode.HAZARD_ONLY);
        }
        else
        {
            this.SetDamageMode(DamageMode.FULL_DAMAGE);
        }
        if (this.enterWithoutInput || this.exitedSuperDashing || this.exitedQuake)
        {
            this.enterWithoutInput = false;
        }
        else
        {
            this.AcceptInput();
        }
        this.gm.FinishedEnteringScene();
        if (this.exitedSuperDashing)
        {
            this.exitedSuperDashing = false;
        }
        if (this.exitedQuake)
        {
            this.exitedQuake = false;
        }
        this.positionHistory[0] = this.transform.position;
        this.positionHistory[1] = this.transform.position;
        this.tilemapTestActive = true;
    }

    // Token: 0x06002154 RID: 8532 RVA: 0x000CA828 File Offset: 0x000C8C28
    private IEnumerator Die()
    {
        if (this.OnDeath != null)
        {
            this.OnDeath();
        }
        if (!this.cState.dead)
        {
            this.playerData.disablePause = true;
            this.boundsChecking = false;
            this.StopTilemapTest();
            this.cState.onConveyor = false;
            this.cState.onConveyorV = false;
            this.rb2d.velocity = new Vector2(0f, 0f);
            this.CancelRecoilHorizontal();
            string mapZone = this.gm.GetCurrentMapZone();
            if (mapZone == "DREAM_WORLD" || mapZone == "GODS_GLORY")
            {
                this.RelinquishControl();
                this.StopAnimationControl();
                this.AffectedByGravity(false);
                this.playerData.isInvincible = true;
                this.ResetHardLandingTimer();
                this.renderer.enabled = false;
                this.heroDeathPrefab.SetActive(true);
            }
            else
            {
                if (this.playerData.permadeathMode == 1)
                {
                    this.playerData.permadeathMode = 2;
                }
                this.AffectedByGravity(false);
                HeroBox.inactive = true;
                this.rb2d.isKinematic = true;
                this.SetState(ActorStates.no_input);
                this.cState.dead = true;
                this.ResetMotion();
                this.ResetHardLandingTimer();
                this.renderer.enabled = false;
                base.gameObject.layer = 2;
                this.heroDeathPrefab.SetActive(true);
                yield return null;
                base.StartCoroutine(this.gm.PlayerDead(this.DEATH_WAIT));
            }
        }
        yield break;
    }

    // Token: 0x06002155 RID: 8533 RVA: 0x000CA844 File Offset: 0x000C8C44
    private IEnumerator DieFromHazard(HazardType hazardType, float angle)
    {
        if (!this.cState.hazardDeath)
        {
            this.playerData.disablePause = true;
            this.StopTilemapTest();
            this.SetState(ActorStates.no_input);
            this.cState.hazardDeath = true;
            this.ResetMotion();
            this.ResetHardLandingTimer();
            this.AffectedByGravity(false);
            this.renderer.enabled = false;
            base.gameObject.layer = 2;
            if (hazardType == HazardType.SPIKES)
            {
                GameObject gameObject = this.spikeDeathPrefab.Spawn();
                gameObject.transform.position = this.transform.position;
                FSMUtility.SetFloat(gameObject.GetComponent<PlayMakerFSM>(), "Spike Direction", angle * 57.29578f);
            }
            else if (hazardType == HazardType.ACID)
            {
                GameObject gameObject2 = this.acidDeathPrefab.Spawn();
                gameObject2.transform.position = this.transform.position;
                gameObject2.transform.localScale = this.transform.localScale;
            }
            yield return null;
            base.StartCoroutine(this.gm.PlayerDeadFromHazard(0f));
        }
        yield break;
    }

    // Token: 0x06002156 RID: 8534 RVA: 0x000CA870 File Offset: 0x000C8C70
    private IEnumerator StartRecoil(CollisionSide impactSide, bool spawnDamageEffect, int damageAmount)
    {
        if (!this.cState.recoiling)
        {
            this.playerData.disablePause = true;
            this.ResetMotion();
            this.AffectedByGravity(false);
            if (impactSide == CollisionSide.left)
            {
                this.recoilVector = new Vector2(this.RECOIL_VELOCITY, this.RECOIL_VELOCITY * 0.5f);
                if (this.cState.facingRight)
                {
                    this.FlipSprite();
                }
            }
            else if (impactSide == CollisionSide.right)
            {
                this.recoilVector = new Vector2(-this.RECOIL_VELOCITY, this.RECOIL_VELOCITY * 0.5f);
                if (!this.cState.facingRight)
                {
                    this.FlipSprite();
                }
            }
            else
            {
                this.recoilVector = Vector2.zero;
            }
            this.SetState(ActorStates.no_input);
            this.cState.recoilFrozen = true;
            if (spawnDamageEffect)
            {
                this.damageEffectFSM.SendEvent("DAMAGE");
                if (damageAmount > 1)
                {
                    UnityEngine.Object.Instantiate<GameObject>(this.takeHitDoublePrefab, this.transform.position, this.transform.rotation);
                }
            }
            if (this.playerData.equippedCharm_4)
            {
                base.StartCoroutine(this.Invulnerable(this.INVUL_TIME_STAL));
            }
            else
            {
                base.StartCoroutine(this.Invulnerable(this.INVUL_TIME));
            }
            yield return this.takeDamageCoroutine = base.StartCoroutine(this.gm.FreezeMoment(this.DAMAGE_FREEZE_DOWN, this.DAMAGE_FREEZE_WAIT, this.DAMAGE_FREEZE_UP, 0.0001f));
            this.cState.recoilFrozen = false;
            this.cState.recoiling = true;
            this.playerData.disablePause = false;
        }
        yield break;
    }

    // Token: 0x06002157 RID: 8535 RVA: 0x000CA8A0 File Offset: 0x000C8CA0
    private IEnumerator Invulnerable(float duration)
    {
        this.cState.invulnerable = true;
        yield return new WaitForSeconds(this.DAMAGE_FREEZE_DOWN);
        this.invPulse.startInvulnerablePulse();
        yield return new WaitForSeconds(duration);
        this.invPulse.stopInvulnerablePulse();
        this.cState.invulnerable = false;
        this.cState.recoiling = false;
        yield break;
    }

    // Token: 0x06002158 RID: 8536 RVA: 0x000CA8C4 File Offset: 0x000C8CC4
    private IEnumerator FirstFadeIn()
    {
        yield return new WaitForSeconds(0.25f);
        this.gm.FadeSceneIn();
        this.fadedSceneIn = true;
        yield break;
    }

    // Token: 0x06002159 RID: 8537 RVA: 0x000CA8E0 File Offset: 0x000C8CE0
    private void FallCheck()
    {
        if (this.rb2d.velocity.y <= -1E-06f)
        {
            if (!this.CheckTouchingGround())
            {
                this.cState.falling = true;
                this.cState.onGround = false;
                this.cState.wallJumping = false;
                this.proxyFSM.SendEvent("HeroCtrl-LeftGround");
                if (this.hero_state != ActorStates.no_input)
                {
                    this.SetState(ActorStates.airborne);
                }
                if (this.cState.wallSliding)
                {
                    this.fallTimer = 0f;
                }
                else
                {
                    this.fallTimer += Time.deltaTime;
                }
                if (this.fallTimer > this.BIG_FALL_TIME)
                {
                    if (!this.cState.willHardLand)
                    {
                        this.cState.willHardLand = true;
                    }
                    if (!this.fallRumble)
                    {
                        this.StartFallRumble();
                    }
                }
                if (this.fallCheckFlagged)
                {
                    this.fallCheckFlagged = false;
                }
            }
        }
        else
        {
            this.cState.falling = false;
            this.fallTimer = 0f;
            if (this.transitionState != HeroTransitionState.ENTERING_SCENE)
            {
                this.cState.willHardLand = false;
            }
            if (this.fallCheckFlagged)
            {
                this.fallCheckFlagged = false;
            }
            if (this.fallRumble)
            {
                this.CancelFallEffects();
            }
        }
    }

    // Token: 0x0600215A RID: 8538 RVA: 0x000CAA38 File Offset: 0x000C8E38
    private void OutOfBoundsCheck()
    {
        if (this.isGameplayScene)
        {
            Vector2 vector = this.transform.position;
            if ((vector.y >= -60f && vector.y <= this.gm.sceneHeight + 60f && vector.x >= -60f && vector.x <= this.gm.sceneWidth + 60f) || this.cState.dead || !this.boundsChecking)
            {
            }
        }
    }

    // Token: 0x0600215B RID: 8539 RVA: 0x000CAAD8 File Offset: 0x000C8ED8
    private void ConfirmOutOfBounds()
    {
        if (this.boundsChecking)
        {
            UnityEngine.Debug.Log("Confirming out of bounds");
            Vector2 vector = this.transform.position;
            if (vector.y < -60f || vector.y > this.gm.sceneHeight + 60f || vector.x < -60f || vector.x > this.gm.sceneWidth + 60f)
            {
                if (!this.cState.dead)
                {
                    this.rb2d.velocity = Vector2.zero;
                    UnityEngine.Debug.LogFormat("Pos: {0} Transition State: {1}", new object[]
                    {
                        this.transform.position,
                        this.transitionState
                    });
                }
            }
            else
            {
                this.boundsChecking = false;
            }
        }
    }

    // Token: 0x0600215C RID: 8540 RVA: 0x000CABC4 File Offset: 0x000C8FC4
    private void FailSafeChecks()
    {
        if (this.hero_state == ActorStates.hard_landing)
        {
            this.hardLandFailSafeTimer += Time.deltaTime;
            if (this.hardLandFailSafeTimer > this.HARD_LANDING_TIME + 0.3f)
            {
                this.SetState(ActorStates.grounded);
                this.BackOnGround();
                this.hardLandFailSafeTimer = 0f;
            }
        }
        else
        {
            this.hardLandFailSafeTimer = 0f;
        }
        if (this.cState.hazardDeath)
        {
            this.hazardDeathTimer += Time.deltaTime;
            if (this.hazardDeathTimer > this.HAZARD_DEATH_CHECK_TIME && this.hero_state != ActorStates.no_input)
            {
                this.ResetMotion();
                this.AffectedByGravity(false);
                this.SetState(ActorStates.no_input);
                this.hazardDeathTimer = 0f;
            }
        }
        else
        {
            this.hazardDeathTimer = 0f;
        }
        if (this.rb2d.velocity.y == 0f && !this.cState.onGround && !this.cState.falling && !this.cState.jumping && !this.cState.dashing && this.hero_state != ActorStates.hard_landing && this.hero_state != ActorStates.no_input)
        {
            if (this.CheckTouchingGround())
            {
                this.floatingBufferTimer += Time.deltaTime;
                if (this.floatingBufferTimer > this.FLOATING_CHECK_TIME)
                {
                    if (this.cState.recoiling)
                    {
                        this.CancelDamageRecoil();
                    }
                    this.BackOnGround();
                    this.floatingBufferTimer = 0f;
                }
            }
            else
            {
                this.floatingBufferTimer = 0f;
            }
        }
    }

    // Token: 0x0600215D RID: 8541 RVA: 0x000CAD78 File Offset: 0x000C9178
    public Transform LocateSpawnPoint()
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag("RespawnPoint");
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].name == this.playerData.respawnMarkerName)
            {
                return array[i].transform;
            }
        }
        return null;
    }

    // Token: 0x0600215E RID: 8542 RVA: 0x000CADCB File Offset: 0x000C91CB
    private void CancelJump()
    {
        this.cState.jumping = false;
        this.jumpReleaseQueuing = false;
        this.jump_steps = 0;
    }

    // Token: 0x0600215F RID: 8543 RVA: 0x000CADE7 File Offset: 0x000C91E7
    private void CancelDoubleJump()
    {
        this.cState.doubleJumping = false;
        this.doubleJump_steps = 0;
    }

    // Token: 0x06002160 RID: 8544 RVA: 0x000CADFC File Offset: 0x000C91FC
    private void CancelDash()
    {
        if (this.cState.shadowDashing)
        {
            this.cState.shadowDashing = false;
        }
        this.cState.dashing = false;
        this.dash_timer = 0f;
        this.AffectedByGravity(true);
        this.sharpShadowPrefab.SetActive(false);
        if (this.dashParticlesPrefab.GetComponent<ParticleSystem>().enableEmission)
        {
            this.dashParticlesPrefab.GetComponent<ParticleSystem>().enableEmission = false;
        }
        if (this.shadowdashParticlesPrefab.GetComponent<ParticleSystem>().enableEmission)
        {
            this.shadowdashParticlesPrefab.GetComponent<ParticleSystem>().enableEmission = false;
        }
    }

    // Token: 0x06002161 RID: 8545 RVA: 0x000CAE9C File Offset: 0x000C929C
    private void CancelWallsliding()
    {
        this.wallslideDustPrefab.enableEmission = false;
        this.wallSlideVibrationPlayer.Stop();
        this.cState.wallSliding = false;
        this.wallSlidingL = false;
        this.wallSlidingR = false;
        this.touchingWallL = false;
        this.touchingWallR = false;
    }

    // Token: 0x06002162 RID: 8546 RVA: 0x000CAEE8 File Offset: 0x000C92E8
    private void CancelBackDash()
    {
        this.cState.backDashing = false;
        this.back_dash_timer = 0f;
    }

    // Token: 0x06002163 RID: 8547 RVA: 0x000CAF01 File Offset: 0x000C9301
    private void CancelDownAttack()
    {
        if (this.cState.downAttacking)
        {
            this.slashComponent.CancelAttack();
            this.ResetAttacks();
        }
    }

    // Token: 0x06002164 RID: 8548 RVA: 0x000CAF24 File Offset: 0x000C9324
    private void CancelAttack()
    {
        if (this.cState.attacking)
        {
            this.slashComponent.CancelAttack();
            this.ResetAttacks();
        }
    }

    // Token: 0x06002165 RID: 8549 RVA: 0x000CAF47 File Offset: 0x000C9347
    private void CancelBounce()
    {
        this.cState.bouncing = false;
        this.cState.shroomBouncing = false;
        this.bounceTimer = 0f;
    }

    // Token: 0x06002166 RID: 8550 RVA: 0x000CAF6C File Offset: 0x000C936C
    private void CancelRecoilHorizontal()
    {
        this.cState.recoilingLeft = false;
        this.cState.recoilingRight = false;
        this.recoilSteps = 0;
    }

    // Token: 0x06002167 RID: 8551 RVA: 0x000CAF8D File Offset: 0x000C938D
    private void CancelDamageRecoil()
    {
        this.cState.recoiling = false;
        this.recoilTimer = 0f;
        this.ResetMotion();
        this.AffectedByGravity(true);
        this.SetDamageMode(DamageMode.FULL_DAMAGE);
    }

    // Token: 0x06002168 RID: 8552 RVA: 0x000CAFBA File Offset: 0x000C93BA
    private void CancelFallEffects()
    {
        this.fallRumble = false;
        this.audioCtrl.StopSound(HeroSounds.FALLING);
        GameCameras.instance.cameraShakeFSM.Fsm.Variables.FindFsmBool("RumblingFall").Value = false;
    }

    // Token: 0x06002169 RID: 8553 RVA: 0x000CAFF4 File Offset: 0x000C93F4
    private void ResetAttacks()
    {
        this.cState.nailCharging = false;
        this.nailChargeTimer = 0f;
        this.cState.attacking = false;
        this.cState.upAttacking = false;
        this.cState.downAttacking = false;
        this.attack_time = 0f;
    }

    // Token: 0x0600216A RID: 8554 RVA: 0x000CB047 File Offset: 0x000C9447
    private void ResetAttacksDash()
    {
        this.cState.attacking = false;
        this.cState.upAttacking = false;
        this.cState.downAttacking = false;
        this.attack_time = 0f;
    }

    // Token: 0x0600216B RID: 8555 RVA: 0x000CB078 File Offset: 0x000C9478
    private void ResetMotion()
    {
        this.CancelJump();
        this.CancelDoubleJump();
        this.CancelDash();
        this.CancelBackDash();
        this.CancelBounce();
        this.CancelRecoilHorizontal();
        this.CancelWallsliding();
        this.rb2d.velocity = Vector2.zero;
        this.transition_vel = Vector2.zero;
        this.wallLocked = false;
        this.nailChargeTimer = 0f;
    }

    // Token: 0x0600216C RID: 8556 RVA: 0x000CB0DC File Offset: 0x000C94DC
    private void ResetMotionNotVelocity()
    {
        this.CancelJump();
        this.CancelDoubleJump();
        this.CancelDash();
        this.CancelBackDash();
        this.CancelBounce();
        this.CancelRecoilHorizontal();
        this.CancelWallsliding();
        this.transition_vel = Vector2.zero;
        this.wallLocked = false;
    }

    // Token: 0x0600216D RID: 8557 RVA: 0x000CB11A File Offset: 0x000C951A
    private void ResetLook()
    {
        this.cState.lookingUp = false;
        this.cState.lookingDown = false;
        this.cState.lookingUpAnim = false;
        this.cState.lookingDownAnim = false;
        this.lookDelayTimer = 0f;
    }

    // Token: 0x0600216E RID: 8558 RVA: 0x000CB157 File Offset: 0x000C9557
    private void ResetInput()
    {
        this.move_input = 0f;
        this.vertical_input = 0f;
    }

    // Token: 0x0600216F RID: 8559 RVA: 0x000CB170 File Offset: 0x000C9570
    private void BackOnGround()
    {
        if (this.landingBufferSteps <= 0)
        {
            this.landingBufferSteps = this.LANDING_BUFFER_STEPS;
            if (!this.cState.onGround && !this.hardLanded && !this.cState.superDashing)
            {
                this.softLandingEffectPrefab.Spawn(this.transform.position);
                VibrationManager.PlayVibrationClipOneShot(this.softLandVibration, null, false, string.Empty);
            }
        }
        this.cState.falling = false;
        this.fallTimer = 0f;
        this.dashLandingTimer = 0f;
        this.cState.willHardLand = false;
        this.hardLandingTimer = 0f;
        this.hardLanded = false;
        this.jump_steps = 0;
        if (this.cState.doubleJumping)
        {
            this.HeroJump();
        }
        this.SetState(ActorStates.grounded);
        this.cState.onGround = true;
        this.airDashed = false;
        this.doubleJumped = false;
        if (this.dJumpWingsPrefab.activeSelf)
        {
            this.dJumpWingsPrefab.SetActive(false);
        }
    }

    // Token: 0x06002170 RID: 8560 RVA: 0x000CB28C File Offset: 0x000C968C
    private void JumpReleased()
    {
        if (this.rb2d.velocity.y > 0f && this.jumped_steps >= this.JUMP_STEPS_MIN && !this.inAcid && !this.cState.shroomBouncing)
        {
            if (this.jumpReleaseQueueingEnabled)
            {
                if (this.jumpReleaseQueuing && this.jumpReleaseQueueSteps <= 0)
                {
                    this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, 0f);
                    this.CancelJump();
                }
            }
            else
            {
                this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, 0f);
                this.CancelJump();
            }
        }
        this.jumpQueuing = false;
        this.doubleJumpQueuing = false;
        if (this.cState.swimming)
        {
            this.cState.swimming = false;
        }
    }

    // Token: 0x06002171 RID: 8561 RVA: 0x000CB390 File Offset: 0x000C9790
    private void FinishedDashing()
    {
        this.CancelDash();
        this.AffectedByGravity(true);
        this.animCtrl.FinishedDash();
        this.proxyFSM.SendEvent("HeroCtrl-DashEnd");
        if (this.cState.touchingWall && !this.cState.onGround && (this.playerData.hasWalljump & (this.touchingWallL || this.touchingWallR)))
        {
            this.wallslideDustPrefab.enableEmission = true;
            this.wallSlideVibrationPlayer.Play();
            this.cState.wallSliding = true;
            this.cState.willHardLand = false;
            if (this.touchingWallL)
            {
                this.wallSlidingL = true;
            }
            if (this.touchingWallR)
            {
                this.wallSlidingR = true;
            }
            if (this.dashingDown)
            {
                this.FlipSprite();
            }
        }
    }

    // Token: 0x06002172 RID: 8562 RVA: 0x000CB46E File Offset: 0x000C986E
    private void SetStartingMotionState()
    {
        this.SetStartingMotionState(false);
    }

    // Token: 0x06002173 RID: 8563 RVA: 0x000CB478 File Offset: 0x000C9878
    private void SetStartingMotionState(bool preventRunDip)
    {
        this.move_input = ((!this.acceptingInput && !preventRunDip) ? 0f : this.inputHandler.inputActions.moveVector.X);
        this.cState.touchingWall = false;
        if (this.CheckTouchingGround())
        {
            this.cState.onGround = true;
            this.SetState(ActorStates.grounded);
            if (this.enteringVertically)
            {
                this.SpawnSoftLandingPrefab();
                this.animCtrl.playLanding = true;
                this.enteringVertically = false;
            }
        }
        else
        {
            this.cState.onGround = false;
            this.SetState(ActorStates.airborne);
        }
        this.animCtrl.UpdateState(this.hero_state);
    }

    // Token: 0x06002174 RID: 8564 RVA: 0x000CB532 File Offset: 0x000C9932
    [Obsolete("This was used specifically for underwater swimming in acid but is no longer in use.")]
    private void EnterAcid()
    {
        this.rb2d.gravityScale = this.UNDERWATER_GRAVITY;
        this.inAcid = true;
        this.cState.inAcid = true;
    }

    // Token: 0x06002175 RID: 8565 RVA: 0x000CB558 File Offset: 0x000C9958
    [Obsolete("This was used specifically for underwater swimming in acid but is no longer in use.")]
    private void ExitAcid()
    {
        this.rb2d.gravityScale = this.DEFAULT_GRAVITY;
        this.inAcid = false;
        this.cState.inAcid = false;
        this.airDashed = false;
        this.doubleJumped = false;
        if (this.inputHandler.inputActions.jump.IsPressed)
        {
            this.HeroJump();
        }
    }

    // Token: 0x06002176 RID: 8566 RVA: 0x000CB5B8 File Offset: 0x000C99B8
    private void TileMapTest()
    {
        if (this.tilemapTestActive && !this.cState.jumping)
        {
            Vector2 vector = this.transform.position;
            Vector2 direction = new Vector2(this.positionHistory[0].x - vector.x, this.positionHistory[0].y - vector.y);
            float magnitude = direction.magnitude;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, direction, magnitude, 256);
            if (raycastHit2D.collider != null)
            {
                UnityEngine.Debug.LogFormat("TERRAIN INGRESS {0} at {1} Jumping: {2}", new object[]
                {
                    this.gm.GetSceneNameString(),
                    vector,
                    this.cState.jumping
                });
                this.ResetMotion();
                this.rb2d.velocity = Vector2.zero;
                if (this.cState.dashing)
                {
                    this.FinishedDashing();
                    this.transform.SetPosition2D(this.positionHistory[1]);
                }
                if (this.cState.superDashing)
                {
                    this.transform.SetPosition2D(raycastHit2D.point);
                    this.superDash.SendEvent("HIT WALL");
                }
                if (this.cState.spellQuake)
                {
                    this.spellControl.SendEvent("Hero Landed");
                    this.transform.SetPosition2D(this.positionHistory[1]);
                }
                this.tilemapTestActive = false;
                this.tilemapTestCoroutine = base.StartCoroutine(this.TilemapTestPause());
            }
        }
    }

    // Token: 0x06002177 RID: 8567 RVA: 0x000CB75C File Offset: 0x000C9B5C
    private IEnumerator TilemapTestPause()
    {
        yield return new WaitForSeconds(0.1f);
        this.tilemapTestActive = true;
        yield break;
    }

    // Token: 0x06002178 RID: 8568 RVA: 0x000CB777 File Offset: 0x000C9B77
    private void StopTilemapTest()
    {
        if (this.tilemapTestCoroutine != null)
        {
            base.StopCoroutine(this.tilemapTestCoroutine);
            this.tilemapTestActive = false;
        }
    }

    // Token: 0x06002179 RID: 8569 RVA: 0x000CB798 File Offset: 0x000C9B98
    public IEnumerator CheckForTerrainThunk(AttackDirection attackDir)
    {
        bool terrainHit = false;
        float thunkTimer = this.NAIL_TERRAIN_CHECK_TIME;
        while (thunkTimer > 0f)
        {
            if (!terrainHit)
            {
                float num = 0.25f;
                float num2;
                if (attackDir == AttackDirection.normal)
                {
                    num2 = 2f;
                }
                else
                {
                    num2 = 1.5f;
                }
                float num3 = 1f;
                if (this.playerData.equippedCharm_18)
                {
                    num3 += 0.2f;
                }
                if (this.playerData.equippedCharm_13)
                {
                    num3 += 0.3f;
                }
                num2 *= num3;
                Vector2 size = new Vector2(0.45f, 0.45f);
                Vector2 origin = new Vector2(this.col2d.bounds.center.x, this.col2d.bounds.center.y + num);
                Vector2 origin2 = new Vector2(this.col2d.bounds.center.x, this.col2d.bounds.max.y);
                Vector2 origin3 = new Vector2(this.col2d.bounds.center.x, this.col2d.bounds.min.y);
                int layerMask = 33554688;
                RaycastHit2D raycastHit2D = default(RaycastHit2D);
                if (attackDir == AttackDirection.normal)
                {
                    if ((this.cState.facingRight && !this.cState.wallSliding) || (!this.cState.facingRight && this.cState.wallSliding))
                    {
                        raycastHit2D = Physics2D.BoxCast(origin, size, 0f, Vector2.right, num2, layerMask);
                    }
                    else
                    {
                        raycastHit2D = Physics2D.BoxCast(origin, size, 0f, Vector2.left, num2, layerMask);
                    }
                }
                else if (attackDir == AttackDirection.upward)
                {
                    raycastHit2D = Physics2D.BoxCast(origin2, size, 0f, Vector2.up, num2, layerMask);
                }
                else if (attackDir == AttackDirection.downward)
                {
                    raycastHit2D = Physics2D.BoxCast(origin3, size, 0f, Vector2.down, num2, layerMask);
                }
                if (raycastHit2D.collider != null && !raycastHit2D.collider.isTrigger)
                {
                    NonThunker component = raycastHit2D.collider.gameObject.GetComponent<NonThunker>();
                    bool flag = !(component != null) || !component.active;
                    if (flag)
                    {
                        terrainHit = true;
                        this.nailTerrainImpactEffectPrefab.Spawn(raycastHit2D.point, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f)));
                        if (attackDir == AttackDirection.normal)
                        {
                            if (this.cState.facingRight)
                            {
                                this.RecoilLeft();
                            }
                            else
                            {
                                this.RecoilRight();
                            }
                        }
                        else if (attackDir == AttackDirection.upward)
                        {
                            this.RecoilDown();
                        }
                    }
                }
                thunkTimer -= Time.deltaTime;
            }
            yield return null;
        }
        yield break;
    }

    // Token: 0x0600217A RID: 8570 RVA: 0x000CB7BC File Offset: 0x000C9BBC
    private bool CheckStillTouchingWall(CollisionSide side, bool checkTop = false)
    {
        Vector2 origin = new Vector2(this.col2d.bounds.min.x, this.col2d.bounds.max.y);
        Vector2 origin2 = new Vector2(this.col2d.bounds.min.x, this.col2d.bounds.center.y);
        Vector2 origin3 = new Vector2(this.col2d.bounds.min.x, this.col2d.bounds.min.y);
        Vector2 origin4 = new Vector2(this.col2d.bounds.max.x, this.col2d.bounds.max.y);
        Vector2 origin5 = new Vector2(this.col2d.bounds.max.x, this.col2d.bounds.center.y);
        Vector2 origin6 = new Vector2(this.col2d.bounds.max.x, this.col2d.bounds.min.y);
        float distance = 0.1f;
        RaycastHit2D raycastHit2D = default(RaycastHit2D);
        RaycastHit2D raycastHit2D2 = default(RaycastHit2D);
        RaycastHit2D raycastHit2D3 = default(RaycastHit2D);
        if (side == CollisionSide.left)
        {
            if (checkTop)
            {
                raycastHit2D = Physics2D.Raycast(origin, Vector2.left, distance, 256);
            }
            raycastHit2D2 = Physics2D.Raycast(origin2, Vector2.left, distance, 256);
            raycastHit2D3 = Physics2D.Raycast(origin3, Vector2.left, distance, 256);
        }
        else
        {
            if (side != CollisionSide.right)
            {
                UnityEngine.Debug.LogError("Invalid CollisionSide specified.");
                return false;
            }
            if (checkTop)
            {
                raycastHit2D = Physics2D.Raycast(origin4, Vector2.right, distance, 256);
            }
            raycastHit2D2 = Physics2D.Raycast(origin5, Vector2.right, distance, 256);
            raycastHit2D3 = Physics2D.Raycast(origin6, Vector2.right, distance, 256);
        }
        if (raycastHit2D2.collider != null)
        {
            bool flag = true;
            if (raycastHit2D2.collider.isTrigger)
            {
                flag = false;
            }
            SteepSlope component = raycastHit2D2.collider.GetComponent<SteepSlope>();
            if (component != null)
            {
                flag = false;
            }
            NonSlider component2 = raycastHit2D2.collider.GetComponent<NonSlider>();
            if (component2 != null)
            {
                flag = false;
            }
            if (flag)
            {
                return true;
            }
        }
        if (raycastHit2D3.collider != null)
        {
            bool flag2 = true;
            if (raycastHit2D3.collider.isTrigger)
            {
                flag2 = false;
            }
            SteepSlope component3 = raycastHit2D3.collider.GetComponent<SteepSlope>();
            if (component3 != null)
            {
                flag2 = false;
            }
            NonSlider component4 = raycastHit2D3.collider.GetComponent<NonSlider>();
            if (component4 != null)
            {
                flag2 = false;
            }
            if (flag2)
            {
                return true;
            }
        }
        if (checkTop && raycastHit2D.collider != null)
        {
            bool flag3 = true;
            if (raycastHit2D.collider.isTrigger)
            {
                flag3 = false;
            }
            SteepSlope component5 = raycastHit2D.collider.GetComponent<SteepSlope>();
            if (component5 != null)
            {
                flag3 = false;
            }
            NonSlider component6 = raycastHit2D.collider.GetComponent<NonSlider>();
            if (component6 != null)
            {
                flag3 = false;
            }
            if (flag3)
            {
                return true;
            }
        }
        return false;
    }

    // Token: 0x0600217B RID: 8571 RVA: 0x000CBB74 File Offset: 0x000C9F74
    public bool CheckForBump(CollisionSide side)
    {
        float num = 0.025f;
        float num2 = 0.2f;
        Vector2 vector = new Vector2(this.col2d.bounds.min.x + num2, this.col2d.bounds.min.y + 0.2f);
        Vector2 vector2 = new Vector2(this.col2d.bounds.min.x + num2, this.col2d.bounds.min.y - num);
        Vector2 vector3 = new Vector2(this.col2d.bounds.max.x - num2, this.col2d.bounds.min.y + 0.2f);
        Vector2 vector4 = new Vector2(this.col2d.bounds.max.x - num2, this.col2d.bounds.min.y - num);
        float num3 = 0.32f + num2;
        RaycastHit2D raycastHit2D = default(RaycastHit2D);
        RaycastHit2D raycastHit2D2 = default(RaycastHit2D);
        if (side == CollisionSide.left)
        {
            UnityEngine.Debug.DrawLine(vector2, vector2 + Vector2.left * num3, Color.cyan, 0.15f);
            UnityEngine.Debug.DrawLine(vector, vector + Vector2.left * num3, Color.cyan, 0.15f);
            raycastHit2D2 = Physics2D.Raycast(vector2, Vector2.left, num3, 256);
            raycastHit2D = Physics2D.Raycast(vector, Vector2.left, num3, 256);
        }
        else if (side == CollisionSide.right)
        {
            UnityEngine.Debug.DrawLine(vector4, vector4 + Vector2.right * num3, Color.cyan, 0.15f);
            UnityEngine.Debug.DrawLine(vector3, vector3 + Vector2.right * num3, Color.cyan, 0.15f);
            raycastHit2D2 = Physics2D.Raycast(vector4, Vector2.right, num3, 256);
            raycastHit2D = Physics2D.Raycast(vector3, Vector2.right, num3, 256);
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid CollisionSide specified.");
        }
        if (raycastHit2D2.collider != null && raycastHit2D.collider == null)
        {
            Vector2 vector5 = raycastHit2D2.point + new Vector2((side != CollisionSide.right) ? -0.1f : 0.1f, 1f);
            RaycastHit2D raycastHit2D3 = Physics2D.Raycast(vector5, Vector2.down, 1.5f, 256);
            Vector2 vector6 = raycastHit2D2.point + new Vector2((side != CollisionSide.right) ? 0.1f : -0.1f, 1f);
            RaycastHit2D raycastHit2D4 = Physics2D.Raycast(vector6, Vector2.down, 1.5f, 256);
            if (raycastHit2D3.collider != null)
            {
                UnityEngine.Debug.DrawLine(vector5, raycastHit2D3.point, Color.cyan, 0.15f);
                if (!(raycastHit2D4.collider != null))
                {
                    return true;
                }
                UnityEngine.Debug.DrawLine(vector6, raycastHit2D4.point, Color.cyan, 0.15f);
                float num4 = raycastHit2D3.point.y - raycastHit2D4.point.y;
                if (num4 > 0f)
                {
                    UnityEngine.Debug.Log("Bump Height: " + num4);
                    return true;
                }
            }
        }
        return false;
    }

    // Token: 0x0600217C RID: 8572 RVA: 0x000CBF54 File Offset: 0x000CA354
    public bool CheckNearRoof()
    {
        Vector2 origin = this.col2d.bounds.max;
        Vector2 origin2 = new Vector2(this.col2d.bounds.min.x, this.col2d.bounds.max.y);
        Vector2 vector = new Vector2(this.col2d.bounds.center.x, this.col2d.bounds.max.y);
        Vector2 origin3 = new Vector2(this.col2d.bounds.center.x + this.col2d.bounds.size.x / 4f, this.col2d.bounds.max.y);
        Vector2 origin4 = new Vector2(this.col2d.bounds.center.x - this.col2d.bounds.size.x / 4f, this.col2d.bounds.max.y);
        Vector2 direction = new Vector2(-0.5f, 1f);
        Vector2 direction2 = new Vector2(0.5f, 1f);
        Vector2 up = Vector2.up;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(origin2, direction, 2f, 256);
        RaycastHit2D raycastHit2D2 = Physics2D.Raycast(origin, direction2, 2f, 256);
        RaycastHit2D raycastHit2D3 = Physics2D.Raycast(origin3, up, 1f, 256);
        RaycastHit2D raycastHit2D4 = Physics2D.Raycast(origin4, up, 1f, 256);
        return raycastHit2D.collider != null || raycastHit2D2.collider != null || raycastHit2D3.collider != null || raycastHit2D4.collider != null;
    }

    // Token: 0x0600217D RID: 8573 RVA: 0x000CC18C File Offset: 0x000CA58C
    public bool CheckTouchingGround()
    {
        Vector2 vector = new Vector2(this.col2d.bounds.min.x, this.col2d.bounds.center.y);
        Vector2 vector2 = this.col2d.bounds.center;
        Vector2 vector3 = new Vector2(this.col2d.bounds.max.x, this.col2d.bounds.center.y);
        float distance = this.col2d.bounds.extents.y + 0.16f;
        UnityEngine.Debug.DrawRay(vector, Vector2.down, Color.yellow);
        UnityEngine.Debug.DrawRay(vector2, Vector2.down, Color.yellow);
        UnityEngine.Debug.DrawRay(vector3, Vector2.down, Color.yellow);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, Vector2.down, distance, 256);
        RaycastHit2D raycastHit2D2 = Physics2D.Raycast(vector2, Vector2.down, distance, 256);
        RaycastHit2D raycastHit2D3 = Physics2D.Raycast(vector3, Vector2.down, distance, 256);
        return raycastHit2D.collider != null || raycastHit2D2.collider != null || raycastHit2D3.collider != null;
    }

    // Token: 0x0600217E RID: 8574 RVA: 0x000CC320 File Offset: 0x000CA720
    private List<CollisionSide> CheckTouching(PhysLayers layer)
    {
        List<CollisionSide> list = new List<CollisionSide>(4);
        Vector3 center = this.col2d.bounds.center;
        float distance = this.col2d.bounds.extents.x + 0.16f;
        float distance2 = this.col2d.bounds.extents.y + 0.16f;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(center, Vector2.up, distance2, 1 << (int)layer);
        RaycastHit2D raycastHit2D2 = Physics2D.Raycast(center, Vector2.right, distance, 1 << (int)layer);
        RaycastHit2D raycastHit2D3 = Physics2D.Raycast(center, Vector2.down, distance2, 1 << (int)layer);
        RaycastHit2D raycastHit2D4 = Physics2D.Raycast(center, Vector2.left, distance, 1 << (int)layer);
        if (raycastHit2D.collider != null)
        {
            list.Add(CollisionSide.top);
        }
        if (raycastHit2D2.collider != null)
        {
            list.Add(CollisionSide.right);
        }
        if (raycastHit2D3.collider != null)
        {
            list.Add(CollisionSide.bottom);
        }
        if (raycastHit2D4.collider != null)
        {
            list.Add(CollisionSide.left);
        }
        return list;
    }

    // Token: 0x0600217F RID: 8575 RVA: 0x000CC45C File Offset: 0x000CA85C
    private List<CollisionSide> CheckTouchingAdvanced(PhysLayers layer)
    {
        List<CollisionSide> list = new List<CollisionSide>();
        Vector2 origin = new Vector2(this.col2d.bounds.min.x, this.col2d.bounds.max.y);
        Vector2 origin2 = new Vector2(this.col2d.bounds.center.x, this.col2d.bounds.max.y);
        Vector2 origin3 = new Vector2(this.col2d.bounds.max.x, this.col2d.bounds.max.y);
        Vector2 origin4 = new Vector2(this.col2d.bounds.min.x, this.col2d.bounds.center.y);
        Vector2 origin5 = new Vector2(this.col2d.bounds.max.x, this.col2d.bounds.center.y);
        Vector2 origin6 = new Vector2(this.col2d.bounds.min.x, this.col2d.bounds.min.y);
        Vector2 origin7 = new Vector2(this.col2d.bounds.center.x, this.col2d.bounds.min.y);
        Vector2 origin8 = new Vector2(this.col2d.bounds.max.x, this.col2d.bounds.min.y);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, Vector2.up, 0.16f, 1 << (int)layer);
        RaycastHit2D raycastHit2D2 = Physics2D.Raycast(origin2, Vector2.up, 0.16f, 1 << (int)layer);
        RaycastHit2D raycastHit2D3 = Physics2D.Raycast(origin3, Vector2.up, 0.16f, 1 << (int)layer);
        RaycastHit2D raycastHit2D4 = Physics2D.Raycast(origin3, Vector2.right, 0.16f, 1 << (int)layer);
        RaycastHit2D raycastHit2D5 = Physics2D.Raycast(origin5, Vector2.right, 0.16f, 1 << (int)layer);
        RaycastHit2D raycastHit2D6 = Physics2D.Raycast(origin8, Vector2.right, 0.16f, 1 << (int)layer);
        RaycastHit2D raycastHit2D7 = Physics2D.Raycast(origin8, Vector2.down, 0.16f, 1 << (int)layer);
        RaycastHit2D raycastHit2D8 = Physics2D.Raycast(origin7, Vector2.down, 0.16f, 1 << (int)layer);
        RaycastHit2D raycastHit2D9 = Physics2D.Raycast(origin6, Vector2.down, 0.16f, 1 << (int)layer);
        RaycastHit2D raycastHit2D10 = Physics2D.Raycast(origin6, Vector2.left, 0.16f, 1 << (int)layer);
        RaycastHit2D raycastHit2D11 = Physics2D.Raycast(origin4, Vector2.left, 0.16f, 1 << (int)layer);
        RaycastHit2D raycastHit2D12 = Physics2D.Raycast(origin, Vector2.left, 0.16f, 1 << (int)layer);
        if (raycastHit2D.collider != null || raycastHit2D2.collider != null || raycastHit2D3.collider != null)
        {
            list.Add(CollisionSide.top);
        }
        if (raycastHit2D4.collider != null || raycastHit2D5.collider != null || raycastHit2D6.collider != null)
        {
            list.Add(CollisionSide.right);
        }
        if (raycastHit2D7.collider != null || raycastHit2D8.collider != null || raycastHit2D9.collider != null)
        {
            list.Add(CollisionSide.bottom);
        }
        if (raycastHit2D10.collider != null || raycastHit2D11.collider != null || raycastHit2D12.collider != null)
        {
            list.Add(CollisionSide.left);
        }
        return list;
    }

    // Token: 0x06002180 RID: 8576 RVA: 0x000CC894 File Offset: 0x000CAC94
    private CollisionSide FindCollisionDirection(Collision2D collision)
    {
        Vector2 normal = collision.GetSafeContact().Normal;
        float x = normal.x;
        float y = normal.y;
        if (y >= 0.5f)
        {
            return CollisionSide.bottom;
        }
        if (y <= -0.5f)
        {
            return CollisionSide.top;
        }
        if (x < 0f)
        {
            return CollisionSide.right;
        }
        if (x > 0f)
        {
            return CollisionSide.left;
        }
        UnityEngine.Debug.LogError(string.Concat(new object[]
        {
            "ERROR: unable to determine direction of collision - contact points at (",
            normal.x,
            ",",
            normal.y,
            ")"
        }));
        return CollisionSide.bottom;
    }

    // Token: 0x06002181 RID: 8577 RVA: 0x000CC93C File Offset: 0x000CAD3C
    private bool CanJump()
    {
        if (this.hero_state == ActorStates.no_input || this.hero_state == ActorStates.hard_landing || this.hero_state == ActorStates.dash_landing || this.cState.wallSliding || this.cState.dashing || this.cState.backDashing || this.cState.jumping || this.cState.bouncing || this.cState.shroomBouncing)
        {
            return false;
        }
        if (this.cState.onGround)
        {
            return true;
        }
        if (this.ledgeBufferSteps > 0 && !this.cState.dead && !this.cState.hazardDeath && !this.controlReqlinquished && this.headBumpSteps <= 0 && !this.CheckNearRoof())
        {
            this.ledgeBufferSteps = 0;
            return true;
        }
        return false;
    }

    // Token: 0x06002182 RID: 8578 RVA: 0x000CCA3C File Offset: 0x000CAE3C
    private bool CanDoubleJump()
    {
        return this.playerData.hasDoubleJump && !this.controlReqlinquished && !this.doubleJumped && !this.inAcid && this.hero_state != ActorStates.no_input && this.hero_state != ActorStates.hard_landing && this.hero_state != ActorStates.dash_landing && !this.cState.dashing && !this.cState.wallSliding && !this.cState.backDashing && !this.cState.attacking && !this.cState.bouncing && !this.cState.shroomBouncing && !this.cState.onGround;
    }

    // Token: 0x06002183 RID: 8579 RVA: 0x000CCB11 File Offset: 0x000CAF11
    private bool CanInfiniteAirJump()
    {
        return this.playerData.infiniteAirJump && this.hero_state != ActorStates.hard_landing && !this.cState.onGround;
    }

    // Token: 0x06002184 RID: 8580 RVA: 0x000CCB44 File Offset: 0x000CAF44
    private bool CanSwim()
    {
        return this.hero_state != ActorStates.no_input && this.hero_state != ActorStates.hard_landing && this.hero_state != ActorStates.dash_landing && !this.cState.attacking && !this.cState.dashing && !this.cState.jumping && !this.cState.bouncing && !this.cState.shroomBouncing && !this.cState.onGround && this.inAcid;
    }

    // Token: 0x06002185 RID: 8581 RVA: 0x000CCBE4 File Offset: 0x000CAFE4
    private bool CanDash()
    {
        return this.hero_state != ActorStates.no_input && this.hero_state != ActorStates.hard_landing && this.hero_state != ActorStates.dash_landing && this.dashCooldownTimer <= 0f && !this.cState.dashing && !this.cState.backDashing && (!this.cState.attacking || this.attack_time >= this.ATTACK_RECOVERY_TIME) && !this.cState.preventDash && (this.cState.onGround || !this.airDashed || this.cState.wallSliding) && !this.cState.hazardDeath && this.playerData.canDash;
    }

    // Token: 0x06002186 RID: 8582 RVA: 0x000CCCC4 File Offset: 0x000CB0C4
    private bool CanAttack()
    {
        return this.attack_cooldown <= 0f && !this.cState.attacking && !this.cState.dashing && !this.cState.dead && !this.cState.hazardDeath && !this.cState.hazardRespawning && !this.controlReqlinquished && this.hero_state != ActorStates.no_input && this.hero_state != ActorStates.hard_landing && this.hero_state != ActorStates.dash_landing;
    }

    // Token: 0x06002187 RID: 8583 RVA: 0x000CCD64 File Offset: 0x000CB164
    private bool CanNailCharge()
    {
        return !this.cState.attacking && !this.controlReqlinquished && !this.cState.recoiling && !this.cState.recoilingLeft && !this.cState.recoilingRight && this.playerData.hasNailArt;
    }

    // Token: 0x06002188 RID: 8584 RVA: 0x000CCDD0 File Offset: 0x000CB1D0
    private bool CanWallSlide()
    {
        return (this.cState.wallSliding && this.gm.isPaused) || (!this.cState.touchingNonSlider && !this.inAcid && !this.cState.dashing && this.playerData.hasWalljump && !this.cState.onGround && !this.cState.recoiling && !this.gm.isPaused && !this.controlReqlinquished && !this.cState.transitioning && (this.cState.falling || this.cState.wallSliding) && !this.cState.doubleJumping && this.CanInput());
    }

    // Token: 0x06002189 RID: 8585 RVA: 0x000CCEC4 File Offset: 0x000CB2C4
    private bool CanTakeDamage()
    {
        return this.damageMode != DamageMode.NO_DAMAGE && this.transitionState == HeroTransitionState.WAITING_TO_TRANSITION && !this.cState.invulnerable && !this.cState.recoiling && !this.playerData.isInvincible && !this.cState.dead && !this.cState.hazardDeath && !BossSceneController.IsTransitioning;
    }

    // Token: 0x0600218A RID: 8586 RVA: 0x000CCF48 File Offset: 0x000CB348
    private bool CanWallJump()
    {
        return this.playerData.hasWalljump && !this.cState.touchingNonSlider && (this.cState.wallSliding || (this.cState.touchingWall && !this.cState.onGround));
    }

    // Token: 0x0600218B RID: 8587 RVA: 0x000CCFB0 File Offset: 0x000CB3B0
    private bool ShouldHardLand(Collision2D collision)
    {
        return !collision.gameObject.GetComponent<NoHardLanding>() && this.cState.willHardLand && !this.inAcid && this.hero_state != ActorStates.hard_landing;
    }

    // Token: 0x0600218C RID: 8588 RVA: 0x000CCFFC File Offset: 0x000CB3FC
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.cState.superDashing && (this.CheckStillTouchingWall(CollisionSide.left, false) || this.CheckStillTouchingWall(CollisionSide.right, false)))
        {
            this.superDash.SendEvent("HIT WALL");
        }
        if ((collision.gameObject.layer == 8 || collision.gameObject.CompareTag("HeroWalkable")) && this.CheckTouchingGround())
        {
            this.proxyFSM.SendEvent("HeroCtrl-Landed");
        }
        if (this.hero_state != ActorStates.no_input)
        {
            CollisionSide collisionSide = this.FindCollisionDirection(collision);
            if (collision.gameObject.layer == 8 || collision.gameObject.CompareTag("HeroWalkable"))
            {
                this.fallTrailGenerated = false;
                if (collisionSide == CollisionSide.top)
                {
                    this.headBumpSteps = this.HEAD_BUMP_STEPS;
                    if (this.cState.jumping)
                    {
                        this.CancelJump();
                        this.CancelDoubleJump();
                    }
                    if (this.cState.bouncing)
                    {
                        this.CancelBounce();
                        this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, 0f);
                    }
                    if (this.cState.shroomBouncing)
                    {
                        this.CancelBounce();
                        this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, 0f);
                    }
                }
                if (collisionSide == CollisionSide.bottom)
                {
                    if (this.cState.attacking)
                    {
                        this.CancelDownAttack();
                    }
                    if (this.ShouldHardLand(collision))
                    {
                        this.DoHardLanding();
                    }
                    else if (collision.gameObject.GetComponent<SteepSlope>() == null && this.hero_state != ActorStates.hard_landing)
                    {
                        this.BackOnGround();
                    }
                    if (this.cState.dashing && this.dashingDown)
                    {
                        this.AffectedByGravity(true);
                        this.SetState(ActorStates.dash_landing);
                        this.hardLanded = true;
                    }
                }
            }
        }
        else if (this.hero_state == ActorStates.no_input && this.transitionState == HeroTransitionState.DROPPING_DOWN && (this.gatePosition == GatePosition.bottom || this.gatePosition == GatePosition.top))
        {
            this.FinishedEnteringScene(true, false);
        }
    }

    // Token: 0x0600218D RID: 8589 RVA: 0x000CD238 File Offset: 0x000CB638
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (this.cState.superDashing && (this.CheckStillTouchingWall(CollisionSide.left, false) || this.CheckStillTouchingWall(CollisionSide.right, false)))
        {
            this.superDash.SendEvent("HIT WALL");
        }
        if (this.hero_state != ActorStates.no_input && collision.gameObject.layer == 8)
        {
            if (collision.gameObject.GetComponent<NonSlider>() == null)
            {
                this.cState.touchingNonSlider = false;
                if (this.CheckStillTouchingWall(CollisionSide.left, false))
                {
                    this.cState.touchingWall = true;
                    this.touchingWallL = true;
                    this.touchingWallR = false;
                }
                else if (this.CheckStillTouchingWall(CollisionSide.right, false))
                {
                    this.cState.touchingWall = true;
                    this.touchingWallL = false;
                    this.touchingWallR = true;
                }
                else
                {
                    this.cState.touchingWall = false;
                    this.touchingWallL = false;
                    this.touchingWallR = false;
                }
                if (this.CheckTouchingGround())
                {
                    if (this.ShouldHardLand(collision))
                    {
                        this.DoHardLanding();
                    }
                    else if (this.hero_state != ActorStates.hard_landing && this.hero_state != ActorStates.dash_landing && this.cState.falling)
                    {
                        this.BackOnGround();
                    }
                }
                else if (this.cState.jumping || this.cState.falling)
                {
                    this.cState.onGround = false;
                    this.proxyFSM.SendEvent("HeroCtrl-LeftGround");
                    this.SetState(ActorStates.airborne);
                }
            }
            else
            {
                this.cState.touchingNonSlider = true;
            }
        }
    }

    // Token: 0x0600218E RID: 8590 RVA: 0x000CD3DC File Offset: 0x000CB7DC
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (this.cState.recoilingLeft || this.cState.recoilingRight)
        {
            this.cState.touchingWall = false;
            this.touchingWallL = false;
            this.touchingWallR = false;
            this.cState.touchingNonSlider = false;
        }
        if (this.hero_state != ActorStates.no_input && !this.cState.recoiling && collision.gameObject.layer == 8)
        {
            if (!this.CheckTouchingGround())
            {
                if (!this.cState.jumping && !this.fallTrailGenerated && this.cState.onGround)
                {
                    if (this.playerData.environmentType != 6)
                    {
                        this.fsm_fallTrail.SendEvent("PLAY");
                    }
                    this.fallTrailGenerated = true;
                }
                this.cState.onGround = false;
                this.proxyFSM.SendEvent("HeroCtrl-LeftGround");
                this.SetState(ActorStates.airborne);
                if (this.cState.wasOnGround)
                {
                    this.ledgeBufferSteps = this.LEDGE_BUFFER_STEPS;
                }
            }
        }
    }

    // Token: 0x0600218F RID: 8591 RVA: 0x000CD500 File Offset: 0x000CB900
    private void SetupGameRefs()
    {
        if (this.cState == null)
        {
            this.cState = new HeroControllerStates();
        }
        this.gm = GameManager.instance;
        this.animCtrl = base.GetComponent<HeroAnimationController>();
        this.rb2d = base.GetComponent<Rigidbody2D>();
        this.col2d = base.GetComponent<Collider2D>();
        this.transform = base.GetComponent<Transform>();
        this.renderer = base.GetComponent<MeshRenderer>();
        this.audioCtrl = base.GetComponent<HeroAudioController>();
        this.inputHandler = this.gm.GetComponent<InputHandler>();
        this.proxyFSM = FSMUtility.LocateFSM(base.gameObject, "ProxyFSM");
        this.audioSource = base.GetComponent<AudioSource>();
        if (!this.footStepsRunAudioSource)
        {
            this.footStepsRunAudioSource = this.transform.Find("Sounds/FootstepsRun").GetComponent<AudioSource>();
        }
        if (!this.footStepsWalkAudioSource)
        {
            this.footStepsWalkAudioSource = this.transform.Find("Sounds/FootstepsWalk").GetComponent<AudioSource>();
        }
        this.invPulse = base.GetComponent<InvulnerablePulse>();
        this.spriteFlash = base.GetComponent<SpriteFlash>();
        this.gm.UnloadingLevel += this.OnLevelUnload;
        this.prevGravityScale = this.DEFAULT_GRAVITY;
        this.transition_vel = Vector2.zero;
        this.current_velocity = Vector2.zero;
        this.acceptingInput = true;
        this.positionHistory = new Vector2[2];
    }

    // Token: 0x06002190 RID: 8592 RVA: 0x000CD663 File Offset: 0x000CBA63
    private void SetupPools()
    {
    }

    // Token: 0x06002191 RID: 8593 RVA: 0x000CD668 File Offset: 0x000CBA68
    private void FilterInput()
    {
        if (this.move_input > 0.3f)
        {
            this.move_input = 1f;
        }
        else if (this.move_input < -0.3f)
        {
            this.move_input = -1f;
        }
        else
        {
            this.move_input = 0f;
        }
        if (this.vertical_input > 0.5f)
        {
            this.vertical_input = 1f;
        }
        else if (this.vertical_input < -0.5f)
        {
            this.vertical_input = -1f;
        }
        else
        {
            this.vertical_input = 0f;
        }
    }

    // Token: 0x06002192 RID: 8594 RVA: 0x000CD70C File Offset: 0x000CBB0C
    public Vector3 FindGroundPoint(Vector2 startPoint, bool useExtended = false)
    {
        float num = this.FIND_GROUND_POINT_DISTANCE;
        if (useExtended)
        {
            num = this.FIND_GROUND_POINT_DISTANCE_EXT;
        }
        RaycastHit2D raycastHit2D = Physics2D.Raycast(startPoint, Vector2.down, num, 256);
        if (raycastHit2D.collider == null)
        {
            UnityEngine.Debug.LogErrorFormat("FindGroundPoint: Could not find ground point below {0}, check reference position is not too high (more than {1} tiles).", new object[]
            {
                startPoint.ToString(),
                num
            });
        }
        return new Vector3(raycastHit2D.point.x, raycastHit2D.point.y + this.col2d.bounds.extents.y - this.col2d.offset.y + 0.01f, this.transform.position.z);
    }

    // Token: 0x06002193 RID: 8595 RVA: 0x000CD7EC File Offset: 0x000CBBEC
    private float FindGroundPointY(float x, float y, bool useExtended = false)
    {
        float num = this.FIND_GROUND_POINT_DISTANCE;
        if (useExtended)
        {
            num = this.FIND_GROUND_POINT_DISTANCE_EXT;
        }
        RaycastHit2D raycastHit2D = Physics2D.Raycast(new Vector2(x, y), Vector2.down, num, 256);
        if (raycastHit2D.collider == null)
        {
            UnityEngine.Debug.LogErrorFormat("FindGroundPoint: Could not find ground point below ({0},{1}), check reference position is not too high (more than {2} tiles).", new object[]
            {
                x,
                y,
                num
            });
        }
        return raycastHit2D.point.y + this.col2d.bounds.extents.y - this.col2d.offset.y + 0.01f;
    }

    // Token: 0x040022FD RID: 8957
    private bool verboseMode;

    // Token: 0x040022FE RID: 8958
    public HeroType heroType;

    // Token: 0x040022FF RID: 8959
    public float RUN_SPEED;

    // Token: 0x04002300 RID: 8960
    public float RUN_SPEED_CH;

    // Token: 0x04002301 RID: 8961
    public float RUN_SPEED_CH_COMBO;

    // Token: 0x04002302 RID: 8962
    public float WALK_SPEED;

    // Token: 0x04002303 RID: 8963
    public float UNDERWATER_SPEED;

    // Token: 0x04002304 RID: 8964
    public float JUMP_SPEED;

    // Token: 0x04002305 RID: 8965
    public float JUMP_SPEED_UNDERWATER;

    // Token: 0x04002306 RID: 8966
    public float MIN_JUMP_SPEED;

    // Token: 0x04002307 RID: 8967
    public int JUMP_STEPS;

    // Token: 0x04002308 RID: 8968
    public int JUMP_STEPS_MIN;

    // Token: 0x04002309 RID: 8969
    public int JUMP_TIME;

    // Token: 0x0400230A RID: 8970
    public int DOUBLE_JUMP_STEPS;

    // Token: 0x0400230B RID: 8971
    public int WJLOCK_STEPS_SHORT;

    // Token: 0x0400230C RID: 8972
    public int WJLOCK_STEPS_LONG;

    // Token: 0x0400230D RID: 8973
    public float WJ_KICKOFF_SPEED;

    // Token: 0x0400230E RID: 8974
    public int WALL_STICKY_STEPS;

    // Token: 0x0400230F RID: 8975
    public float DASH_SPEED;

    // Token: 0x04002310 RID: 8976
    public float DASH_SPEED_SHARP;

    // Token: 0x04002311 RID: 8977
    public float DASH_TIME;

    // Token: 0x04002312 RID: 8978
    public int DASH_QUEUE_STEPS;

    // Token: 0x04002313 RID: 8979
    public float BACK_DASH_SPEED;

    // Token: 0x04002314 RID: 8980
    public float BACK_DASH_TIME;

    // Token: 0x04002315 RID: 8981
    public float SHADOW_DASH_SPEED;

    // Token: 0x04002316 RID: 8982
    public float SHADOW_DASH_TIME;

    // Token: 0x04002317 RID: 8983
    public float SHADOW_DASH_COOLDOWN;

    // Token: 0x04002318 RID: 8984
    public float SUPER_DASH_SPEED;

    // Token: 0x04002319 RID: 8985
    public float DASH_COOLDOWN;

    // Token: 0x0400231A RID: 8986
    public float DASH_COOLDOWN_CH;

    // Token: 0x0400231B RID: 8987
    public float BACKDASH_COOLDOWN;

    // Token: 0x0400231C RID: 8988
    public float WALLSLIDE_SPEED;

    // Token: 0x0400231D RID: 8989
    public float WALLSLIDE_DECEL;

    // Token: 0x0400231E RID: 8990
    public float NAIL_CHARGE_TIME_DEFAULT;

    // Token: 0x0400231F RID: 8991
    public float NAIL_CHARGE_TIME_CHARM;

    // Token: 0x04002320 RID: 8992
    public float CYCLONE_HORIZONTAL_SPEED;

    // Token: 0x04002321 RID: 8993
    public float SWIM_ACCEL;

    // Token: 0x04002322 RID: 8994
    public float SWIM_MAX_SPEED;

    // Token: 0x04002323 RID: 8995
    public float TIME_TO_ENTER_SCENE_BOT;

    // Token: 0x04002324 RID: 8996
    public float TIME_TO_ENTER_SCENE_HOR;

    // Token: 0x04002325 RID: 8997
    public float SPEED_TO_ENTER_SCENE_HOR;

    // Token: 0x04002326 RID: 8998
    public float SPEED_TO_ENTER_SCENE_UP;

    // Token: 0x04002327 RID: 8999
    public float SPEED_TO_ENTER_SCENE_DOWN;

    // Token: 0x04002328 RID: 9000
    public float DEFAULT_GRAVITY;

    // Token: 0x04002329 RID: 9001
    public float UNDERWATER_GRAVITY;

    // Token: 0x0400232A RID: 9002
    public float ATTACK_DURATION;

    // Token: 0x0400232B RID: 9003
    public float ATTACK_DURATION_CH;

    // Token: 0x0400232C RID: 9004
    public float ALT_ATTACK_RESET;

    // Token: 0x0400232D RID: 9005
    public float ATTACK_RECOVERY_TIME;

    // Token: 0x0400232E RID: 9006
    public float ATTACK_COOLDOWN_TIME;

    // Token: 0x0400232F RID: 9007
    public float ATTACK_COOLDOWN_TIME_CH;

    // Token: 0x04002330 RID: 9008
    public float BOUNCE_TIME;

    // Token: 0x04002331 RID: 9009
    public float BOUNCE_SHROOM_TIME;

    // Token: 0x04002332 RID: 9010
    public float BOUNCE_VELOCITY;

    // Token: 0x04002333 RID: 9011
    public float SHROOM_BOUNCE_VELOCITY;

    // Token: 0x04002334 RID: 9012
    public float RECOIL_HOR_TIME;

    // Token: 0x04002335 RID: 9013
    public float RECOIL_HOR_VELOCITY;

    // Token: 0x04002336 RID: 9014
    public float RECOIL_HOR_VELOCITY_LONG;

    // Token: 0x04002337 RID: 9015
    public float RECOIL_HOR_STEPS;

    // Token: 0x04002338 RID: 9016
    public float RECOIL_DOWN_VELOCITY;

    // Token: 0x04002339 RID: 9017
    public float RUN_PUFF_TIME;

    // Token: 0x0400233A RID: 9018
    public float BIG_FALL_TIME;

    // Token: 0x0400233B RID: 9019
    public float HARD_LANDING_TIME;

    // Token: 0x0400233C RID: 9020
    public float DOWN_DASH_TIME;

    // Token: 0x0400233D RID: 9021
    public float MAX_FALL_VELOCITY;

    // Token: 0x0400233E RID: 9022
    public float MAX_FALL_VELOCITY_UNDERWATER;

    // Token: 0x0400233F RID: 9023
    public float RECOIL_DURATION;

    // Token: 0x04002340 RID: 9024
    public float RECOIL_DURATION_STAL;

    // Token: 0x04002341 RID: 9025
    public float RECOIL_VELOCITY;

    // Token: 0x04002342 RID: 9026
    public float DAMAGE_FREEZE_DOWN;

    // Token: 0x04002343 RID: 9027
    public float DAMAGE_FREEZE_WAIT;

    // Token: 0x04002344 RID: 9028
    public float DAMAGE_FREEZE_UP;

    // Token: 0x04002345 RID: 9029
    public float INVUL_TIME;

    // Token: 0x04002346 RID: 9030
    public float INVUL_TIME_STAL;

    // Token: 0x04002347 RID: 9031
    public float INVUL_TIME_PARRY;

    // Token: 0x04002348 RID: 9032
    public float INVUL_TIME_QUAKE;

    // Token: 0x04002349 RID: 9033
    public float INVUL_TIME_CYCLONE;

    // Token: 0x0400234A RID: 9034
    public float CAST_TIME;

    // Token: 0x0400234B RID: 9035
    public float CAST_RECOIL_TIME;

    // Token: 0x0400234C RID: 9036
    public float CAST_RECOIL_VELOCITY;

    // Token: 0x0400234D RID: 9037
    public float WALLSLIDE_CLIP_DELAY;

    // Token: 0x0400234E RID: 9038
    public int GRUB_SOUL_MP;

    // Token: 0x0400234F RID: 9039
    public int GRUB_SOUL_MP_COMBO;

    // Token: 0x04002350 RID: 9040
    private int JUMP_QUEUE_STEPS = 2;

    // Token: 0x04002351 RID: 9041
    private int JUMP_RELEASE_QUEUE_STEPS = 2;

    // Token: 0x04002352 RID: 9042
    private int DOUBLE_JUMP_QUEUE_STEPS = 10;

    // Token: 0x04002353 RID: 9043
    private int ATTACK_QUEUE_STEPS = 5;

    // Token: 0x04002354 RID: 9044
    private float DELAY_BEFORE_ENTER = 0.1f;

    // Token: 0x04002355 RID: 9045
    private float LOOK_DELAY = 0.85f;

    // Token: 0x04002356 RID: 9046
    private float LOOK_ANIM_DELAY = 0.25f;

    // Token: 0x04002357 RID: 9047
    private float DEATH_WAIT = 2.85f;

    // Token: 0x04002358 RID: 9048
    private float HAZARD_DEATH_CHECK_TIME = 3f;

    // Token: 0x04002359 RID: 9049
    private float FLOATING_CHECK_TIME = 0.18f;

    // Token: 0x0400235A RID: 9050
    private float NAIL_TERRAIN_CHECK_TIME = 0.12f;

    // Token: 0x0400235B RID: 9051
    private float BUMP_VELOCITY = 4f;

    // Token: 0x0400235C RID: 9052
    private float BUMP_VELOCITY_DASH = 5f;

    // Token: 0x0400235D RID: 9053
    private int LANDING_BUFFER_STEPS = 5;

    // Token: 0x0400235E RID: 9054
    private int LEDGE_BUFFER_STEPS = 2;

    // Token: 0x0400235F RID: 9055
    private int HEAD_BUMP_STEPS = 3;

    // Token: 0x04002360 RID: 9056
    private float MANTIS_CHARM_SCALE = 1.35f;

    // Token: 0x04002361 RID: 9057
    private float FIND_GROUND_POINT_DISTANCE = 10f;

    // Token: 0x04002362 RID: 9058
    private float FIND_GROUND_POINT_DISTANCE_EXT = 50f;

    // Token: 0x04002363 RID: 9059
    public ActorStates hero_state;

    // Token: 0x04002364 RID: 9060
    public ActorStates prev_hero_state;

    // Token: 0x04002365 RID: 9061
    public HeroTransitionState transitionState;

    // Token: 0x04002366 RID: 9062
    public DamageMode damageMode;

    // Token: 0x04002367 RID: 9063
    public float move_input;

    // Token: 0x04002368 RID: 9064
    public float vertical_input;

    // Token: 0x04002369 RID: 9065
    public float controller_deadzone = 0.2f;

    // Token: 0x0400236A RID: 9066
    public Vector2 current_velocity;

    // Token: 0x0400236B RID: 9067
    private bool isGameplayScene;

    // Token: 0x0400236C RID: 9068
    public bool isEnteringFirstLevel;

    // Token: 0x0400236D RID: 9069
    public Vector2 slashOffset;

    // Token: 0x0400236E RID: 9070
    public Vector2 upSlashOffset;

    // Token: 0x0400236F RID: 9071
    public Vector2 downwardSlashOffset;

    // Token: 0x04002370 RID: 9072
    public Vector2 spell1Offset;

    // Token: 0x04002371 RID: 9073
    private int jump_steps;

    // Token: 0x04002372 RID: 9074
    private int jumped_steps;

    // Token: 0x04002373 RID: 9075
    private int doubleJump_steps;

    // Token: 0x04002374 RID: 9076
    private float dash_timer;

    // Token: 0x04002375 RID: 9077
    private float back_dash_timer;

    // Token: 0x04002376 RID: 9078
    private float shadow_dash_timer;

    // Token: 0x04002377 RID: 9079
    private float attack_time;

    // Token: 0x04002378 RID: 9080
    private float attack_cooldown;

    // Token: 0x04002379 RID: 9081
    private Vector2 transition_vel;

    // Token: 0x0400237A RID: 9082
    private float altAttackTime;

    // Token: 0x0400237B RID: 9083
    private float lookDelayTimer;

    // Token: 0x0400237C RID: 9084
    private float bounceTimer;

    // Token: 0x0400237D RID: 9085
    private float recoilHorizontalTimer;

    // Token: 0x0400237E RID: 9086
    private float runPuffTimer;

    // Token: 0x04002380 RID: 9088
    private float hardLandingTimer;

    // Token: 0x04002381 RID: 9089
    private float dashLandingTimer;

    // Token: 0x04002382 RID: 9090
    private float recoilTimer;

    // Token: 0x04002383 RID: 9091
    private int recoilSteps;

    // Token: 0x04002384 RID: 9092
    private int landingBufferSteps;

    // Token: 0x04002385 RID: 9093
    private int dashQueueSteps;

    // Token: 0x04002386 RID: 9094
    private bool dashQueuing;

    // Token: 0x04002387 RID: 9095
    private float shadowDashTimer;

    // Token: 0x04002388 RID: 9096
    private float dashCooldownTimer;

    // Token: 0x04002389 RID: 9097
    private float nailChargeTimer;

    // Token: 0x0400238A RID: 9098
    private int wallLockSteps;

    // Token: 0x0400238B RID: 9099
    private float wallslideClipTimer;

    // Token: 0x0400238C RID: 9100
    private float hardLandFailSafeTimer;

    // Token: 0x0400238D RID: 9101
    private float hazardDeathTimer;

    // Token: 0x0400238E RID: 9102
    private float floatingBufferTimer;

    // Token: 0x0400238F RID: 9103
    private float attackDuration;

    // Token: 0x04002390 RID: 9104
    public float parryInvulnTimer;

    // Token: 0x04002391 RID: 9105
    [Space(6f)]
    [Header("Slash Prefabs")]
    public GameObject slashPrefab;

    // Token: 0x04002392 RID: 9106
    public GameObject slashAltPrefab;

    // Token: 0x04002393 RID: 9107
    public GameObject upSlashPrefab;

    // Token: 0x04002394 RID: 9108
    public GameObject downSlashPrefab;

    // Token: 0x04002395 RID: 9109
    public GameObject wallSlashPrefab;

    // Token: 0x04002396 RID: 9110
    public NailSlash normalSlash;

    // Token: 0x04002397 RID: 9111
    public NailSlash alternateSlash;

    // Token: 0x04002398 RID: 9112
    public NailSlash upSlash;

    // Token: 0x04002399 RID: 9113
    public NailSlash downSlash;

    // Token: 0x0400239A RID: 9114
    public NailSlash wallSlash;

    // Token: 0x0400239B RID: 9115
    public PlayMakerFSM normalSlashFsm;

    // Token: 0x0400239C RID: 9116
    public PlayMakerFSM alternateSlashFsm;

    // Token: 0x0400239D RID: 9117
    public PlayMakerFSM upSlashFsm;

    // Token: 0x0400239E RID: 9118
    public PlayMakerFSM downSlashFsm;

    // Token: 0x0400239F RID: 9119
    public PlayMakerFSM wallSlashFsm;

    // Token: 0x040023A0 RID: 9120
    [Space(6f)]
    [Header("Effect Prefabs")]
    public GameObject nailTerrainImpactEffectPrefab;

    // Token: 0x040023A1 RID: 9121
    public GameObject spell1Prefab;

    // Token: 0x040023A2 RID: 9122
    public GameObject takeHitPrefab;

    // Token: 0x040023A3 RID: 9123
    public GameObject takeHitDoublePrefab;

    // Token: 0x040023A4 RID: 9124
    public GameObject softLandingEffectPrefab;

    // Token: 0x040023A5 RID: 9125
    public GameObject hardLandingEffectPrefab;

    // Token: 0x040023A6 RID: 9126
    public GameObject runEffectPrefab;

    // Token: 0x040023A7 RID: 9127
    public GameObject backDashPrefab;

    // Token: 0x040023A8 RID: 9128
    public GameObject jumpEffectPrefab;

    // Token: 0x040023A9 RID: 9129
    public GameObject jumpTrailPrefab;

    // Token: 0x040023AA RID: 9130
    public GameObject fallEffectPrefab;

    // Token: 0x040023AB RID: 9131
    public ParticleSystem wallslideDustPrefab;

    // Token: 0x040023AC RID: 9132
    public GameObject artChargeEffect;

    // Token: 0x040023AD RID: 9133
    public GameObject artChargedEffect;

    // Token: 0x040023AE RID: 9134
    public GameObject artChargedFlash;

    // Token: 0x040023AF RID: 9135
    public tk2dSpriteAnimator artChargedEffectAnim;

    // Token: 0x040023B0 RID: 9136
    public GameObject shadowdashBurstPrefab;

    // Token: 0x040023B1 RID: 9137
    public GameObject shadowdashDownBurstPrefab;

    // Token: 0x040023B2 RID: 9138
    public GameObject dashParticlesPrefab;

    // Token: 0x040023B3 RID: 9139
    public GameObject shadowdashParticlesPrefab;

    // Token: 0x040023B4 RID: 9140
    public GameObject shadowRingPrefab;

    // Token: 0x040023B5 RID: 9141
    public GameObject shadowRechargePrefab;

    // Token: 0x040023B6 RID: 9142
    public GameObject dJumpWingsPrefab;

    // Token: 0x040023B7 RID: 9143
    public GameObject dJumpFlashPrefab;

    // Token: 0x040023B8 RID: 9144
    public ParticleSystem dJumpFeathers;

    // Token: 0x040023B9 RID: 9145
    public GameObject wallPuffPrefab;

    // Token: 0x040023BA RID: 9146
    public GameObject sharpShadowPrefab;

    // Token: 0x040023BB RID: 9147
    public GameObject grubberFlyBeamPrefabL;

    // Token: 0x040023BC RID: 9148
    public GameObject grubberFlyBeamPrefabR;

    // Token: 0x040023BD RID: 9149
    public GameObject grubberFlyBeamPrefabU;

    // Token: 0x040023BE RID: 9150
    public GameObject grubberFlyBeamPrefabD;

    // Token: 0x040023BF RID: 9151
    public GameObject grubberFlyBeamPrefabL_fury;

    // Token: 0x040023C0 RID: 9152
    public GameObject grubberFlyBeamPrefabR_fury;

    // Token: 0x040023C1 RID: 9153
    public GameObject grubberFlyBeamPrefabU_fury;

    // Token: 0x040023C2 RID: 9154
    public GameObject grubberFlyBeamPrefabD_fury;

    // Token: 0x040023C3 RID: 9155
    public GameObject carefreeShield;

    // Token: 0x040023C4 RID: 9156
    [Space(6f)]
    [Header("Hero Death")]
    public GameObject corpsePrefab;

    // Token: 0x040023C5 RID: 9157
    public GameObject spikeDeathPrefab;

    // Token: 0x040023C6 RID: 9158
    public GameObject acidDeathPrefab;

    // Token: 0x040023C7 RID: 9159
    public GameObject lavaDeathPrefab;

    // Token: 0x040023C8 RID: 9160
    public GameObject heroDeathPrefab;

    // Token: 0x040023C9 RID: 9161
    [Space(6f)]
    [Header("Hero Other")]
    public GameObject cutscenePrefab;

    // Token: 0x040023CA RID: 9162
    private GameManager gm;

    // Token: 0x040023CB RID: 9163
    private Rigidbody2D rb2d;

    // Token: 0x040023CC RID: 9164
    private Collider2D col2d;

    // Token: 0x040023CD RID: 9165
    private MeshRenderer renderer;

    // Token: 0x040023CE RID: 9166
    private new Transform transform;

    // Token: 0x040023CF RID: 9167
    private HeroAnimationController animCtrl;

    // Token: 0x040023D0 RID: 9168
    public HeroControllerStates cState;

    // Token: 0x040023D1 RID: 9169
    public PlayerData playerData;

    // Token: 0x040023D2 RID: 9170
    private HeroAudioController audioCtrl;

    // Token: 0x040023D3 RID: 9171
    private AudioSource audioSource;

    // Token: 0x040023D4 RID: 9172
    [HideInInspector]
    public UIManager ui;

    // Token: 0x040023D5 RID: 9173
    private InputHandler inputHandler;

    // Token: 0x040023D8 RID: 9176
    public PlayMakerFSM damageEffectFSM;

    // Token: 0x040023D9 RID: 9177
    private ParticleSystem dashParticleSystem;

    // Token: 0x040023DA RID: 9178
    private InvulnerablePulse invPulse;

    // Token: 0x040023DB RID: 9179
    private SpriteFlash spriteFlash;

    // Token: 0x040023DC RID: 9180
    public AudioSource footStepsRunAudioSource;

    // Token: 0x040023DD RID: 9181
    public AudioSource footStepsWalkAudioSource;

    // Token: 0x040023DE RID: 9182
    private float prevGravityScale;

    // Token: 0x040023DF RID: 9183
    private Vector2 recoilVector;

    // Token: 0x040023E0 RID: 9184
    private Vector2 lastInputState;

    // Token: 0x040023E1 RID: 9185
    public GatePosition gatePosition;

    // Token: 0x040023E3 RID: 9187
    private bool runMsgSent;

    // Token: 0x040023E4 RID: 9188
    private bool hardLanded;

    // Token: 0x040023E5 RID: 9189
    private bool fallRumble;

    // Token: 0x040023E6 RID: 9190
    public bool acceptingInput;

    // Token: 0x040023E7 RID: 9191
    private bool fallTrailGenerated;

    // Token: 0x040023E8 RID: 9192
    private bool drainMP;

    // Token: 0x040023E9 RID: 9193
    private float drainMP_timer;

    // Token: 0x040023EA RID: 9194
    private float drainMP_time;

    // Token: 0x040023EB RID: 9195
    private float MP_drained;

    // Token: 0x040023EC RID: 9196
    private float drainMP_seconds;

    // Token: 0x040023ED RID: 9197
    private float focusMP_amount;

    // Token: 0x040023EE RID: 9198
    private float dashBumpCorrection;

    // Token: 0x040023EF RID: 9199
    public bool controlReqlinquished;

    // Token: 0x040023F0 RID: 9200
    public bool enterWithoutInput;

    // Token: 0x040023F1 RID: 9201
    public bool lookingUpAnim;

    // Token: 0x040023F2 RID: 9202
    public bool lookingDownAnim;

    // Token: 0x040023F3 RID: 9203
    public bool carefreeShieldEquipped;

    // Token: 0x040023F4 RID: 9204
    private int hitsSinceShielded;

    // Token: 0x040023F5 RID: 9205
    private EndBeta endBeta;

    // Token: 0x040023F6 RID: 9206
    private int jumpQueueSteps;

    // Token: 0x040023F7 RID: 9207
    private bool jumpQueuing;

    // Token: 0x040023F8 RID: 9208
    private int doubleJumpQueueSteps;

    // Token: 0x040023F9 RID: 9209
    private bool doubleJumpQueuing;

    // Token: 0x040023FA RID: 9210
    private int jumpReleaseQueueSteps;

    // Token: 0x040023FB RID: 9211
    private bool jumpReleaseQueuing;

    // Token: 0x040023FC RID: 9212
    private int attackQueueSteps;

    // Token: 0x040023FD RID: 9213
    private bool attackQueuing;

    // Token: 0x040023FE RID: 9214
    public bool touchingWallL;

    // Token: 0x040023FF RID: 9215
    public bool touchingWallR;

    // Token: 0x04002400 RID: 9216
    public bool wallSlidingL;

    // Token: 0x04002401 RID: 9217
    public bool wallSlidingR;

    // Token: 0x04002402 RID: 9218
    private bool airDashed;

    // Token: 0x04002403 RID: 9219
    public bool dashingDown;

    // Token: 0x04002404 RID: 9220
    public bool wieldingLantern;

    // Token: 0x04002405 RID: 9221
    private bool startWithWallslide;

    // Token: 0x04002406 RID: 9222
    private bool startWithJump;

    // Token: 0x04002407 RID: 9223
    private bool startWithFullJump;

    // Token: 0x04002408 RID: 9224
    private bool startWithDash;

    // Token: 0x04002409 RID: 9225
    private bool startWithAttack;

    // Token: 0x0400240A RID: 9226
    private bool nailArt_cyclone;

    // Token: 0x0400240B RID: 9227
    private bool wallSlashing;

    // Token: 0x0400240C RID: 9228
    private bool doubleJumped;

    // Token: 0x0400240D RID: 9229
    public bool inAcid;

    // Token: 0x0400240E RID: 9230
    private bool wallJumpedR;

    // Token: 0x0400240F RID: 9231
    private bool wallJumpedL;

    // Token: 0x04002410 RID: 9232
    public bool wallLocked;

    // Token: 0x04002411 RID: 9233
    private float currentWalljumpSpeed;

    // Token: 0x04002412 RID: 9234
    private float walljumpSpeedDecel;

    // Token: 0x04002413 RID: 9235
    private int wallUnstickSteps;

    // Token: 0x04002414 RID: 9236
    private bool recoilLarge;

    // Token: 0x04002415 RID: 9237
    public float conveyorSpeed;

    // Token: 0x04002416 RID: 9238
    public float conveyorSpeedV;

    // Token: 0x04002417 RID: 9239
    private bool enteringVertically;

    // Token: 0x04002418 RID: 9240
    private bool playingWallslideClip;

    // Token: 0x04002419 RID: 9241
    private bool playedMantisClawClip;

    // Token: 0x0400241A RID: 9242
    public bool exitedSuperDashing;

    // Token: 0x0400241B RID: 9243
    public bool exitedQuake;

    // Token: 0x0400241C RID: 9244
    private bool fallCheckFlagged;

    // Token: 0x0400241D RID: 9245
    private int ledgeBufferSteps;

    // Token: 0x0400241E RID: 9246
    private int headBumpSteps;

    // Token: 0x0400241F RID: 9247
    private float nailChargeTime;

    // Token: 0x04002420 RID: 9248
    public bool takeNoDamage;

    // Token: 0x04002421 RID: 9249
    private bool joniBeam;

    // Token: 0x04002422 RID: 9250
    public bool fadedSceneIn;

    // Token: 0x04002423 RID: 9251
    private bool stopWalkingOut;

    // Token: 0x04002424 RID: 9252
    private bool boundsChecking;

    // Token: 0x04002425 RID: 9253
    private bool blockerFix;

    // Token: 0x04002426 RID: 9254
    [SerializeField]
    private Vector2[] positionHistory;

    // Token: 0x04002427 RID: 9255
    private bool tilemapTestActive;

    // Token: 0x04002428 RID: 9256
    private Vector2 groundRayOriginC;

    // Token: 0x04002429 RID: 9257
    private Vector2 groundRayOriginL;

    // Token: 0x0400242A RID: 9258
    private Vector2 groundRayOriginR;

    // Token: 0x0400242B RID: 9259
    private Coroutine takeDamageCoroutine;

    // Token: 0x0400242C RID: 9260
    private Coroutine tilemapTestCoroutine;

    // Token: 0x0400242D RID: 9261
    public AudioClip footstepsRunDust;

    // Token: 0x0400242E RID: 9262
    public AudioClip footstepsRunGrass;

    // Token: 0x0400242F RID: 9263
    public AudioClip footstepsRunBone;

    // Token: 0x04002430 RID: 9264
    public AudioClip footstepsRunSpa;

    // Token: 0x04002431 RID: 9265
    public AudioClip footstepsRunMetal;

    // Token: 0x04002432 RID: 9266
    public AudioClip footstepsRunWater;

    // Token: 0x04002433 RID: 9267
    public AudioClip footstepsWalkDust;

    // Token: 0x04002434 RID: 9268
    public AudioClip footstepsWalkGrass;

    // Token: 0x04002435 RID: 9269
    public AudioClip footstepsWalkBone;

    // Token: 0x04002436 RID: 9270
    public AudioClip footstepsWalkSpa;

    // Token: 0x04002437 RID: 9271
    public AudioClip footstepsWalkMetal;

    // Token: 0x04002438 RID: 9272
    public AudioClip nailArtCharge;

    // Token: 0x04002439 RID: 9273
    public AudioClip nailArtChargeComplete;

    // Token: 0x0400243A RID: 9274
    public AudioClip blockerImpact;

    // Token: 0x0400243B RID: 9275
    public AudioClip shadowDashClip;

    // Token: 0x0400243C RID: 9276
    public AudioClip sharpShadowClip;

    // Token: 0x0400243D RID: 9277
    public AudioClip doubleJumpClip;

    // Token: 0x0400243E RID: 9278
    public AudioClip mantisClawClip;

    // Token: 0x0400243F RID: 9279
    private GameObject slash;

    // Token: 0x04002440 RID: 9280
    private NailSlash slashComponent;

    // Token: 0x04002441 RID: 9281
    private PlayMakerFSM slashFsm;

    // Token: 0x04002442 RID: 9282
    private GameObject runEffect;

    // Token: 0x04002443 RID: 9283
    private GameObject backDash;

    // Token: 0x04002444 RID: 9284
    private GameObject jumpEffect;

    // Token: 0x04002445 RID: 9285
    private GameObject fallEffect;

    // Token: 0x04002446 RID: 9286
    private GameObject dashEffect;

    // Token: 0x04002447 RID: 9287
    private GameObject grubberFlyBeam;

    // Token: 0x04002448 RID: 9288
    private GameObject hazardCorpe;

    // Token: 0x04002449 RID: 9289
    public PlayMakerFSM vignetteFSM;

    // Token: 0x0400244A RID: 9290
    public SpriteRenderer heroLight;

    // Token: 0x0400244B RID: 9291
    public SpriteRenderer vignette;

    // Token: 0x0400244C RID: 9292
    public PlayMakerFSM dashBurst;

    // Token: 0x0400244D RID: 9293
    public PlayMakerFSM superDash;

    // Token: 0x0400244E RID: 9294
    public PlayMakerFSM fsm_thornCounter;

    // Token: 0x0400244F RID: 9295
    public PlayMakerFSM spellControl;

    // Token: 0x04002450 RID: 9296
    public PlayMakerFSM fsm_fallTrail;

    // Token: 0x04002451 RID: 9297
    public PlayMakerFSM fsm_orbitShield;

    // Token: 0x04002452 RID: 9298
    public VibrationData softLandVibration;

    // Token: 0x04002453 RID: 9299
    public VibrationData wallJumpVibration;

    // Token: 0x04002454 RID: 9300
    public VibrationPlayer wallSlideVibrationPlayer;

    // Token: 0x04002455 RID: 9301
    public VibrationData dashVibration;

    // Token: 0x04002456 RID: 9302
    public VibrationData shadowDashVibration;

    // Token: 0x04002457 RID: 9303
    public VibrationData doubleJumpVibration;

    // Token: 0x04002459 RID: 9305
    public bool isHeroInPosition = true;

    // Token: 0x0400245C RID: 9308
    private bool jumpReleaseQueueingEnabled;

    // Token: 0x0400245D RID: 9309
    private static HeroController _instance;

    // Token: 0x0400245E RID: 9310
    private const float PreventCastByDialogueEndDuration = 0.3f;

    // Token: 0x0400245F RID: 9311
    private float preventCastByDialogueEndTimer;

    // Token: 0x04002460 RID: 9312
    private Vector2 oldPos = Vector2.zero;

    // Token: 0x02000741 RID: 1857
    // (Invoke) Token: 0x06002196 RID: 8598
    public delegate void HeroInPosition(bool forceDirect);

    // Token: 0x02000742 RID: 1858
    // (Invoke) Token: 0x0600219A RID: 8602
    public delegate void TakeDamageEvent();

    // Token: 0x02000743 RID: 1859
    // (Invoke) Token: 0x0600219E RID: 8606
    public delegate void HeroDeathEvent();
}
*/