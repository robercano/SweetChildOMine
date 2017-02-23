using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SCOM.Utils;

public class Miner : SelectableObject
{
    public int MaxLife;
	public int BuildPower;

    // TODO: Current weapon, this need to be refactored so the weapon selected
    // in the Weapon selector is used instead
    public Sprite Weapon;
    public AudioClip[] m_pickSounds;

    public AudioClip[] m_stepsSounds;
    public AudioClip m_gravelSound;
    public AudioClip m_attackSwing;

    [HideInInspector]
    public enum ColliderType
    {
        ColliderFeet,
        ColliderBody
    };

    public GameObject Target;
    public float Life
    {
        get; private set;
    }

	private FiniteStateMachine<Miner> FSM;

    private Animator m_animator;
    private Rigidbody2D m_rigidBody;
    private AudioSource m_audioSource;

    private float m_walkSpeed = 32.0f;
    private float m_runSpeed = 64.0f;
	private float m_faceDirection = 1.0f;
    private Vector2 m_movementTarget;

    private BoxCollider2D m_feetCollider;
    private BoxCollider2D m_bodyCollider;
    private PolygonCollider2D m_digColliderStraight;
    private PolygonCollider2D m_digColliderUp;
    private PolygonCollider2D m_digColliderDown;
    private BoxCollider2D m_attackCollider;

    private HashSet<GameObject> m_nearCaveColliders;
    private HashSet<GameObject> m_nearEnemies;

    private GameObject m_target;

    private int m_layerMask;

    // Character status
    private CharacterStatus m_characterStatus;

    // Weapons
    private UIContainer m_weaponSelector;
    private Inventory m_weaponInventory;
    private GameObject m_pickAxePrefab;
    private GameObject m_pickAxeInstance;
    private WeaponItem m_pickAxe;

    // Materials
    private UIContainer m_materialSelector;
    public Inventory MaterialInventory;

    // Build
    private UIContainer m_buildSelector;
    public Inventory m_buildInventory;
    private GameObject m_warehouseItemPrefab;
    private Item m_warehouseItem;

    // Input manager
    private InputManager m_inputManager;

    // Mining target
	public MineableObject MineableTarget {
		get {
			return m_mineableTarget;
		}
	}
    private MineableObject m_mineableTarget;
    private int m_mineableTargetAmount;
    private int m_mineableRemainingAmount;
    private bool m_actionWorked;
    private GameObject m_actionProgressDialogPrefab;
    private GameObject m_actionProgressDialogInstance;
    private ActionProgressDialog m_actionProgressDialog;

    // Building target
	public BuildableObject BuildableTarget {
		get {
			return m_buildableTarget;
		}
	}
    private BuildableObject m_buildableTarget;

    // Use this for initialization
    void Start()
    {
		FSM = new FiniteStateMachine<Miner> (this, MinerStateIdle.Instance);

        Life = MaxLife;

        m_animator = GetComponent<Animator>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_audioSource = GetComponent<AudioSource>();


        m_movementTarget = new Vector2(Mathf.Round(m_rigidBody.position.x), Mathf.Round(m_rigidBody.position.y));

        /* Get children components */
        m_feetCollider = transform.FindChild("FeetCollider").GetComponent<BoxCollider2D>();
        m_bodyCollider = transform.FindChild("BodyCollider").GetComponent<BoxCollider2D>();
        m_digColliderDown = transform.FindChild("DigColliderDown").GetComponent<PolygonCollider2D>();
        m_digColliderStraight = transform.FindChild("DigColliderStraight").GetComponent<PolygonCollider2D>();
        m_digColliderUp = transform.FindChild("DigColliderUp").GetComponent<PolygonCollider2D>();
        m_attackCollider = transform.FindChild("AttackCollider").GetComponent<BoxCollider2D>();

        m_nearCaveColliders = new HashSet<GameObject>();
        m_nearEnemies = new HashSet<GameObject>();

        m_digColliderDown.enabled = false;
        m_digColliderStraight.enabled = false;
        m_digColliderUp.enabled = false;
        m_attackCollider.enabled = false;

        m_target = GameObject.Instantiate(Target, new Vector3(m_movementTarget.x, m_movementTarget.y, 0.0f), Quaternion.identity);
        DisableVisibleTarget();

        m_layerMask = (1 << (LayerMask.NameToLayer("Cave Colliders")));

        // UI
        m_characterStatus = GameObject.FindObjectOfType<CharacterStatus>();
        m_weaponSelector = GameObject.Find("WeaponContainer").GetComponent<UIContainer>();
        m_materialSelector = GameObject.Find("InventoryContainer").GetComponent<UIContainer>();
        m_buildSelector = GameObject.Find("BuildContainer").GetComponent<UIContainer>();

        MaterialInventory = new Inventory(6, 100);
        m_weaponInventory = new Inventory(3, 100);
        m_buildInventory = new Inventory(3, 100);

        m_inputManager = GameObject.FindObjectOfType<InputManager>();

        m_mineableTarget = null;
        m_mineableTargetAmount = 0;
        m_mineableRemainingAmount = 0;
        m_actionWorked = false;

        m_onSelectedDelegate = ActivateMiner;

        m_actionProgressDialogPrefab = Resources.Load("UI/ActionProgressDialog") as GameObject;
        m_actionProgressDialogInstance = GameObject.Instantiate(m_actionProgressDialogPrefab, transform, false);
        m_actionProgressDialogInstance.transform.position = new Vector3(gameObject.transform.position.x,
                                                                        m_spriteRenderer.bounds.max.y + UIGlobals.PegDistanceToObject,
                                                                        0.0f);
        m_actionProgressDialog = m_actionProgressDialogInstance.GetComponent<ActionProgressDialog>();
        m_actionProgressDialog.ActionName = "Stop";
        m_actionProgressDialog.OnAction = OnActionTerminated;
        m_actionProgressDialog.Disable();

        // Weapons
        m_pickAxePrefab = Resources.Load("PickAxeWeapon") as GameObject;
        m_pickAxeInstance = GameObject.Instantiate(m_pickAxePrefab, transform, false);
        m_pickAxe = m_pickAxeInstance.GetComponent<WeaponItem>();
        m_weaponInventory.AddItem(m_pickAxe);

        // Build structures
        m_warehouseItemPrefab = Resources.Load("WarehouseItem") as GameObject;
        m_warehouseItem = m_warehouseItemPrefab.GetComponent<Item>();
        m_buildInventory.AddItem(m_warehouseItem);
	}

	// Update is called once per frame
	void FixedUpdate()
    {
        ProcessState();
    }

    public void ActivateMiner()
    {
        m_characterStatus.SetActiveMiner(this);
        m_weaponSelector.SetInventory(m_weaponInventory);
        m_materialSelector.SetInventory(MaterialInventory);
        m_buildSelector.SetInventory(m_buildInventory);
        m_inputManager.SetActiveMiner(this);
    }
    public void DeactivateMiner()
    {
        m_characterStatus.SetActiveMiner(null);
        m_weaponSelector.ClearAll();
        m_materialSelector.ClearAll();
        m_buildSelector.ClearAll();
    }
    public Sprite GetCurrentAvatar()
    {
        return m_spriteRenderer.sprite;
    }

    #region /* Movement target management */
    public void SetMovementTarget(Vector2 target)
	{
		m_movementTarget = new Vector2(Mathf.Round(target.x), Mathf.Round(target.y));
		float deltaX = GetDistanceToMovementTarget ();
		if (deltaX < -float.Epsilon) {
			m_faceDirection = -1.0f;
		} else if (deltaX > float.Epsilon) {
			m_faceDirection = 1.0f;

		}
		transform.localScale = new Vector2(m_faceDirection * Mathf.Abs(transform.localScale.x), transform.localScale.y);
		ActivateVisibleTarget();
	}
	float GetDistanceToMovementTarget()
	{
		return m_movementTarget.x - Mathf.Round(m_rigidBody.position.x);
	}
	public bool HasReachedMovementTarget()
	{
		float deltaX = GetDistanceToMovementTarget();
		if (deltaX >= -float.Epsilon && deltaX <= float.Epsilon) {
			return true;
		} else {
			return false;
		}
	}
    #endregion /* Movement target management */

    void GetMovementTargetFromMouse()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		m_movementTarget = new Vector2(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y));
    }

	void ResetMovementTarget() {
		m_movementTarget = m_rigidBody.position;
	}

    public void ActivateVisibleTarget()
    {
        m_target.transform.position = new Vector2(m_movementTarget.x, m_movementTarget.y);
        m_target.SetActive(true);
    }
    public void DisableVisibleTarget()
    {
        m_target.SetActive(false);
    }
	public void PlayAnimation(string animation)
	{
		m_animator.SetTrigger (animation);
	}
	public void ChangeState(FSMState<Miner> newState)
	{
		FSM.ChangeState (newState);
	}
    public FSMState<Miner> GetCurrentState()
    {
        return FSM.CurrentState;
    }
    public FSMState<Miner> GetPreviousState()
    {
        return FSM.PreviousState;
    }

    void ProcessState()
    {
		FSM.ProcessState ();
    }

    public void OnInputEvent(InputManager.InputEvent ev)
    {
        switch (ev)
        {
            case InputManager.InputEvent.LeftClick:
                SetMovementTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                ChangeState(MinerStateWalk.Instance);
                break;
            case InputManager.InputEvent.DoubleLeftClick:
                SetMovementTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                ChangeState(MinerStateRun.Instance);
                break;
            case InputManager.InputEvent.Space:
                ChangeState(MinerStateAttack.Instance);
                break;
        }
    }
   
    void EndMovement(bool resetTarget = true)
    {
        /* Round the coordinates so they are perfect pixel aligned */
        m_rigidBody.position = new Vector2(Mathf.Round(m_rigidBody.position.x), Mathf.Round(m_rigidBody.position.y));
        m_rigidBody.velocity = Vector2.zero;
    }

    #region /* Movement methods */
    public void MoveUp()
    {
        m_rigidBody.position = new Vector2(m_rigidBody.position.x, m_rigidBody.position.y + 1.0f);
    }
    public void FallDown()
    {
        /* Check if we need to fall down */
        Vector2 leftSource = new Vector2(m_feetCollider.bounds.min.x + 0.5f, m_feetCollider.bounds.min.y);
        Vector2 rightSource = new Vector2(m_feetCollider.bounds.max.x - 0.5f, m_feetCollider.bounds.min.y);

        m_feetCollider.enabled = false;
        m_bodyCollider.enabled = false;
        RaycastHit2D hitLeft = Physics2D.Raycast(leftSource, Vector2.down, Mathf.Infinity, m_layerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(rightSource, Vector2.down, Mathf.Infinity, m_layerMask);
        m_feetCollider.enabled = true;
        m_bodyCollider.enabled = true;

        if ((hitLeft.collider != null) && (hitRight.collider != null))
        {
            m_rigidBody.position = new Vector2(m_rigidBody.position.x, m_rigidBody.position.y - Mathf.Min(hitLeft.distance, hitRight.distance));
        }
    }
    public void Walk()
    {
        m_rigidBody.velocity = new Vector2(m_faceDirection * m_walkSpeed, m_rigidBody.velocity.y);
        FallDown();
    }
    public void StepForward()
    {
        if (transform.localScale.x <= 0.0f)
        {
            m_rigidBody.position = new Vector2(m_rigidBody.position.x - 1.0f, m_rigidBody.position.y);
        }
        else {
            m_rigidBody.position = new Vector2(m_rigidBody.position.x + 1.0f, m_rigidBody.position.y);
        }
        PlayAudioOneStep();
    }
    public void Run()
    {
        m_rigidBody.velocity = new Vector2(m_faceDirection * m_runSpeed, m_rigidBody.velocity.y);
        FallDown();
    }
    public void Stop()
    {
        EndMovement();
        FallDown();
    }
    #endregion /* Movement methods */

    #region /* Events receivers */
    public void OnCollisionEnter2DChild(Collision2D coll, ColliderType childCollider)
	{
		if (FSM.CurrentState == MinerStateWalk.Instance ||
			FSM.CurrentState == MinerStateRun.Instance ||
			FSM.CurrentState == MinerStateDigWalk.Instance ||
			FSM.CurrentState == MinerStateDig.Instance) {
			switch (childCollider) {
			case ColliderType.ColliderBody:
					// Check if any of the contact points is in the direction
					// of the current movement. If so we have collided against a wall,
					// readjust position and end movement
				for (int i = 0; i < coll.contacts.Length; ++i) {
					if (m_faceDirection * (m_rigidBody.position.x - coll.contacts [i].point.x) < 0.0f) {
						m_rigidBody.position = new Vector2 (coll.collider.bounds.center.x -
						m_faceDirection * (coll.collider.bounds.extents.x + m_bodyCollider.bounds.extents.x),
							m_rigidBody.position.y);
						Stop ();

						if (m_mineableTarget != null && coll.gameObject == m_mineableTarget.gameObject) {
							FSM.ChangeState (MinerStateMineMaterial.Instance);
						} else if (m_buildableTarget != null && coll.gameObject == m_buildableTarget.gameObject) {
							FSM.ChangeState (MinerStateBuild.Instance);
						} else if (FSM.CurrentState == MinerStateDigWalk.Instance && coll.gameObject.tag == "CaveCollider") {
							FSM.ChangeState (MinerStateDig.Instance);
						} else {
							ResetMovementTarget (); 
							FSM.ChangeState (MinerStateIdle.Instance);
						}
						break;
					}
				}
				break;
			case ColliderType.ColliderFeet:
					// Only move up if collider is in front of us
				if (m_faceDirection * (m_rigidBody.position.x - coll.collider.bounds.center.x) <= 0.0f)
					MoveUp ();
				break;
			}
		}
    }

    public void OnTriggerEnter2DChild(Collider2D coll)
    {
        if (coll.tag == "CaveCollider")
        {
            m_nearCaveColliders.Add(coll.gameObject);
        }
        else if (coll.tag == "Enemy")
        {
            m_nearEnemies.Add(coll.gameObject);
        }
    }

    public void OnTriggerExit2DChild(Collider2D coll)
    {
        if (coll.tag == "CaveCollider")
        {
            m_nearCaveColliders.Remove(coll.gameObject);
        }
        else if (coll.tag == "Enemy")
        {
            m_nearEnemies.Remove(coll.gameObject);
        }
    }

    public void OnMining()
    {
        if (m_mineableTarget == null)
            return;

        Item materialMined = null;
        m_actionWorked = m_mineableTarget.DoMine(m_pickAxe.Damage, m_mineableRemainingAmount, MaterialInventory.RemainingWeight, out materialMined);

        if (m_actionWorked)
        {
            PlayAudioPickAxe();
            m_mineableRemainingAmount -= materialMined.Amount;
            m_actionProgressDialog.Percentage = (100 * (m_mineableTargetAmount - m_mineableRemainingAmount) / m_mineableTargetAmount);

            MaterialInventory.AddItem(materialMined);
            m_buildInventory.RefreshInventory();
        }
    }

    public void OnMiningFinished()
    {
        if (m_mineableTarget == null || m_actionWorked == false || m_mineableRemainingAmount == 0)
        {
            OnActionTerminated();
        }
    }

    public void OnBuilding()
    {
        if (m_buildableTarget == null)
            return;

        int progress = 0;

		m_actionWorked = m_buildableTarget.DoBuild(BuildPower, out progress);

		if (m_actionWorked || progress == 100)
        {
            PlayAudioPickAxe();
            m_actionProgressDialog.Percentage = progress;
        }
    }

    public void OnBuildingFinished()
    {
        if (m_buildableTarget == null || m_actionWorked == false)
        {
            OnActionTerminated();
        }
    }

    public void OnDigging()
    {
        if (m_nearCaveColliders.Count == 0)
        {
            EndMovement();
            FallDown();
			FSM.ChangeState(MinerStateIdle.Instance);
            return;
        }

        PlayAudioPickAxe();
        foreach (GameObject go in m_nearCaveColliders)
        {
            go.SendMessage("PlayerHit", 2, SendMessageOptions.DontRequireReceiver);
        }
        m_nearCaveColliders.Clear();
    }

    public void OnStepForward()
    {
        StepForward();
    }

    public void OnStep()
    {
        PlayAudioOneStep();
    }
    private void OnAttack()
    {
        float pickSoundVolume = Random.Range(0.4f, 0.6f);
        m_audioSource.PlayOneShot(m_attackSwing, pickSoundVolume);

        foreach (GameObject go in m_nearEnemies)
        {
            if (go == null || go.Equals(null))
                continue;
            go.SendMessage("OnPlayerAttack");
        }
    }

    private void OnAttackFinished()
    {
        m_attackCollider.enabled = false;
        FSM.ChangeState(MinerStateIdle.Instance);
    }

    public void OnEnemyAttack(Collider2D coll, int damage)
    {
        if (m_bodyCollider.bounds.Intersects(coll.bounds))
        {
            Life -= damage;

            if (Life <= 0)
            {
                LevelManager.Instance.PlayerDestroyed();
                Destroy(gameObject);
            }
            else
            {
                m_spriteRenderer.color = Color.white;
                StopCoroutine(PlayerHitAnimation());
                StartCoroutine(PlayerHitAnimation());
            }
        }
    }

    public void OnActionStarted()
    {
        DisableDialog();
        m_actionProgressDialog.Enable();
    }

    public void OnActionTerminated()
    {
        m_mineableRemainingAmount = 0;
        m_mineableTargetAmount = 0;
        m_mineableTarget = null;
        m_buildableTarget = null;

        EnableDialog();
        m_actionProgressDialog.Disable();
    }
    #endregion /* Events receivers */

    #region /* Sound methods */
    private void PlayAudioPickAxe()
    {
        int pickSoundIdx = Random.Range(0, m_pickSounds.Length);
        float pickSoundVolume = Random.Range(0.4f, 0.6f);

        m_audioSource.PlayOneShot(m_pickSounds[pickSoundIdx], pickSoundVolume);
        PlayAudioGravelFall();
    }

    private void PlayAudioOneStep()
    {
        int stepSoundIdx = Random.Range(0, m_stepsSounds.Length);
        float stepSoundVolume = Random.Range(0.4f, 0.6f);

        m_audioSource.PlayOneShot(m_stepsSounds[stepSoundIdx], stepSoundVolume);
    }

    private void PlayAudioGravelFall()
    {
        AudioSource.PlayClipAtPoint(m_gravelSound, Camera.main.transform.position, 0.1f);
    }
    #endregion /* Sound methods */

    private IEnumerator PlayerHitAnimation()
    {
        for (int i = 0; i < 20; ++i)
        {
            m_spriteRenderer.color = new Color(1.0f, Random.Range(0.5f, 1.0f), 1.0f);
            yield return new WaitForSeconds(0.05f);
        }
        m_spriteRenderer.color = Color.white;
    }

	
	private void StartAction(string action, SelectableObject target, FSMState<Miner> nextState)
    {
        m_actionProgressDialog.Title = action + " " + target.Name;
        m_actionProgressDialog.Percentage = 0;

        SetMovementTarget(target.gameObject.transform.position);
        FSM.ChangeState(nextState);
    }

    public void MineMaterial(MineableObject obj, int numItems)
    {
        if (numItems == 0)
            return;

        m_mineableTarget = obj;
        m_mineableTargetAmount = numItems;
        m_mineableRemainingAmount = numItems;

		StartAction("Mining", m_mineableTarget, MinerStateWalk.Instance);
    }

    #region /* Building structures methods */
    public bool CheckRecipeForBuildableObject(GameObject buildableObject)
	{
		return CheckRecipeForBuildableObject(buildableObject.GetComponent<BuildableObject> ());
	}

	public bool CheckRecipeForBuildableObject(BuildableObject buildableObject)
	{
		if (buildableObject == null)
			return false;

		foreach (BuildableObject.RecipeItem recipeItem in buildableObject.Recipe) {
			if (MaterialInventory.GetItemAmount (recipeItem.Ingredient.Name) < recipeItem.Amount)
				return false;
		}
		return true;
	}

    public bool RequestMaterialsForStructure(BuildableObject obj)
    {
        if (CheckRecipeForBuildableObject(obj) == false)
        {
            return false;
        }

        foreach (BuildableObject.RecipeItem recipeItem in obj.Recipe)
        {
            int amount = recipeItem.Amount;
            if (MaterialInventory.RemoveItemAmount(recipeItem.Ingredient.Name, ref amount) == false)
            {
                Debug.Log("Something bad happened while building an structure");
                return false;
            }
        }

        //MaterialInventory.RefreshInventory();
        m_buildInventory.RefreshInventory();
        return true;
    }

    public void BuildStructure(BuildableObject obj)
    {
        m_buildableTarget = obj;
        StartAction("Building", m_buildableTarget, MinerStateWalk.Instance);
    }
    #endregion /* Building structures methods */

    #region /* Digging cave methods */
    public void StopDigging()
	{
		m_digColliderDown.enabled = false;
		m_digColliderUp.enabled = false;
		m_digColliderStraight.enabled = false;
	}
	public void StartDiggingUp() {
		m_digColliderDown.enabled = false;
		m_digColliderStraight.enabled = false;
		m_digColliderUp.enabled = true;
	}
	public void StartDiggingStraight() {
		m_digColliderDown.enabled = false;
		m_digColliderStraight.enabled = true;
		m_digColliderUp.enabled = false;
	}
	public void StartDiggingDown() {
		m_digColliderDown.enabled = true;
		m_digColliderStraight.enabled = false;
		m_digColliderUp.enabled = false;
	}

    public void DigCave(Vector2 digTarget)
    {
        EndMovement();

		SetMovementTarget(digTarget);

        if (m_movementTarget.y >= m_bodyCollider.bounds.max.y)
        {
			StartDiggingUp ();
        }
        else if (m_movementTarget.y <= m_bodyCollider.bounds.min.y)
        {
			StartDiggingDown ();
        }
        else
        {
			StartDiggingStraight ();
        }

        FSM.ChangeState(MinerStateDigWalk.Instance);
    }
    #endregion /* Digging cave methods */
};
