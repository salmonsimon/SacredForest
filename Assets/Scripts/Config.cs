public class Config 
{
    public const string SPAWN_POINT_TAG = "Spawn Point";
    public const string CINEMACHINE_CAMERA_TAG = "CinemachineCamera";

    public const string ANIMATOR_BEAR_TRAP_TRIGGER = "Activate";

    #region Characters

    #region Player

    #region Player Attack

    public const float ATTACK_COOLDOWN_DURATION = .5f;
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

    public const float DASH_FORCE = 25f;

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
}