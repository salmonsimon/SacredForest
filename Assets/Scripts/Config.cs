public class Config 
{
    #region Tags

    public const string PLAYER_TAG = "Player";
    public const string SPAWN_POINT_TAG = "Spawn Point";
    public const string CINEMACHINE_CAMERA_TAG = "CinemachineCamera";

    #endregion

    #region General Animations

    public const string ANIMATOR_BEAR_TRAP_TRIGGER = "Activate";

    #region Camera Shake

    public const float CAMERASHAKE_HIT_AMPLITUDE = 6f;
    public const float CAMERASHAKE_HIT_DURATION = .2f;

    #endregion

    #endregion

    #region General

    public const float SMALL_DELAY = .1f;
    public const float MEDIUM_DELAY = .2f;

    #endregion

    #region Characters

    #region GENERAL ANIMATIONS

    public const string ANIMATOR_HURT_TRIGGER = "Hurt";
    public const string ANIMATOR_DEATH_TRIGGER = "Death";
    public const string ANIMATOR_IS_DEAD = "IsDead";

    #endregion

    #region Enemies

    public const string ENEMY_TAG = "Enemy"; 

    public const float ACTION_COOLDOWN_DURATION = 1f;

    public const float JUMP_BACK_FORCE = 250f;
    public const string MOVEMENT_ANIMATOR_JUMP_BACK_TRIGGER = "JumpBack";
    public const string MOVEMENT_ANIMATOR_IS_JUMPING_BACK = "IsJumpingBack";
    public const float JUMP_BACK_DURATION = .2f;
    public const float JUMP_BACK_COOLDOWN = 2f;
    public const float MOVEMENT_AFTER_JUMP_BACK_COOLDOWN = 1f;

    public const float ALERT_GROUP_DELAY = .2f;

    public const float MAGIC_PROJECTILE_SPEED = 1.5f;

    public const float STUN_DURATION = .5f;

    #endregion

    #region Player

    #region Player Attack

    public const float ATTACK_COOLDOWN_DURATION = .3f;
    public const float SECOND_ATTACK_DURATION = .1f;
    public const float WALLSLIDE_ATTACK_MOVEMENT_COOLDOWN = .3f;

    #endregion

    #region Player Movement

    public const float ADDED_FALL_GRAVITY = 0.3f;
    public const float ADDED_GRAVITY_LOW_JUMP = 1.8f;

    #endregion

    #endregion

    #region General Movement

    public const float RUN_SPEED = 3f;
    public const float MOVEMENT_SMOOTHING = .05f;

    public const float DASH_FORCE = 850f;

    public const float JUMP_FORCE = 200;
    public const float WALL_JUMP_FORCE_X = 400;
    public const float WALL_JUMP_FORCE_Y = 160;

    public const float FALL_SPEED_LIMIT = 8f;
    public const float WALL_SLIDING_VELOCITY = -1f;

    #region Animations

    public const string MOVEMENT_ANIMATOR_SPEED_Y = "SpeedY";

    public const string MOVEMENT_ANIMATOR_IS_WALL_SLIDING = "IsWallSliding";

    public const string MOVEMENT_ANIMATOR_IS_JUMPING = "IsJumping";
    public const string MOVEMENT_ANIMATOR_JUMP_TRIGGER = "Jump";

    public const string MOVEMENT_ANIMATOR_DASH_TRIGGER = "Dash";
    public const string MOVEMENT_ANIMATOR_IS_DASHING = "IsDashing";
    public const float DASH_DURATION = .1f;
    public const float DASH_COOLDOWN = .5f;

    #endregion

    #endregion

    #endregion

    #region Transitions
    public const string CROSSFADE_TRANSITION = "Crossfade";
    public const string CROSSFADE_START_TRIGGER = "Start";
    public const string CROSSFADE_END_TRIGGER = "End";
    public const float START_TRANSITION_DURATION = .5f;
    public const float END_TRANSITION_DURATION = 1f;
    #endregion

    #region Damage Types

    public const string SWORD_DAMAGE = "Sword";
    public const string DEFAULT_DAMAGE = "Default";
    public const string BLUDGEONING_DAMAGE = "Bludgeoning";
    public const string FIRE_DAMAGE = "Fire";

    #endregion

    #region Audio

    #region SFX

    public const string ARROW_HIT_SFX = "Arrow Hit";

    #endregion

    #endregion
}
