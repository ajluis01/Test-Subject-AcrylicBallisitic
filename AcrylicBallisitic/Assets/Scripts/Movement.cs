using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

//TODO: Call this anything else please
public class Movement : MonoBehaviour
{
    [SerializeField] int Speed = 15;
    [SerializeField] float BulletLineDuration = .05f;
    
    [SerializeField] float PenaltyDuration = .25f;
    [SerializeField] float ReloadTime = 2f;

    [SerializeField] Transform Gun;
    [SerializeField] GameObject BulletFX;

    //Called or will need to be called in different script, maybe player stats SO or Static is needed
    [SerializeField] public float MultiShotPenalty = .5f;
    [HideInInspector] public int penaltyLevel = 0;

    [SerializeField] public float PowerupDuration = 3.5f;

    [SerializeField] Animator animator;

    [SerializeField] ParticleSystem muzzleFlash;

    private InputAction movement;
    private Rigidbody rb;
    private InputAction attack;
    private float penaltyTimer = 0;
    Vector2 direction = Vector2.zero;
    Vector3 LookVec;
    private bool canShoot = true;
    private bool isPoweredUp = false;

    LayerMask wallCheck;
    
    void Start()
    {
        movement = InputSystem.actions.FindAction("Move");
        attack = InputSystem.actions.FindAction("Attack");

        attack.performed += Shoot;

        rb = GetComponent<Rigidbody>();

        wallCheck = LayerMask.GetMask("Wall");
        muzzleFlash.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        attack.performed -= Shoot;
    }

    void FixedUpdate()
    {
        //Movement
        direction = movement.ReadValue<Vector2>();
        Vector3 directionv3 = new Vector3(direction.x, 0, direction.y);

        if(direction.x != 0)
        {
            Vector3 directionX = new Vector3(direction.x, 0, 0);
            if(Physics.Raycast(transform.position, directionX, 2, wallCheck))
            {
                directionv3.x = 0;
            }
        }

        if(direction.y != 0)
        {
            Vector3 directionY = new Vector3(0, 0, direction.y);
            if (Physics.Raycast(transform.position, directionY, 2, wallCheck))
            {
                directionv3.z = 0;
            }
        }

        if (directionv3 != Vector3.zero)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        rb.MovePosition(rb.position + directionv3 * Speed * Time.deltaTime);

        //Rotation to look at cursor
        LookVec = SceneCamera.cursorPos;
        LookVec.y = transform.position.y;
        transform.LookAt(LookVec);

        //Multishot Penalty Timer
        if (penaltyTimer > 0)
        {
            penaltyTimer -= Time.deltaTime;
        }
        else
        {
            penaltyLevel = 0;
        }


    }


    public void Shoot(InputAction.CallbackContext context)
    {
        if (GameManager.GetManager().IsGracePeriod()) return;

        if(!canShoot)
        {
            Debug.Log("Can't shoot");
            return;
        }

        GameManager.GetManager().PlaySound("PLAYER_SHOOT", 0.5f);
        muzzleFlash?.gameObject.SetActive(true);
        muzzleFlash?.Play();

        //Will shoot past the cursor location and hit anything behind, can limit range to where was click if needed

        Vector3 ShootAtPoint = LookVec;
  
        if (MultiShotPenalty > 0)
        {
            ShootAtPoint.x += (Random.Range(MultiShotPenalty, MultiShotPenalty) * penaltyLevel);
            ShootAtPoint.z += (Random.Range(MultiShotPenalty, MultiShotPenalty) * penaltyLevel);
        }

        Vector3 GunShootDir = Vector3.Normalize(ShootAtPoint - Gun.position);

        

        Ray ray = new Ray(Gun.position, GunShootDir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 300))
        {
            //Determine Target and Deal Damage if Enemy
            bool hitSomething;
            if(hit.collider.CompareTag("Enemy"))
            {
                hitSomething = true;
                hit.collider.gameObject.GetComponent<PaintingController>().DoDamage(GameManager.GetManager().GetPlayerDamage());
                GameManager.GetManager().PlaySound("PLAYER_HIT");
            }
            else
            {
                hitSomething = false;
            }


            //Shooting
            if (!isPoweredUp)
            {
                GameManager.GetManager().UseBullet(hitSomething);
            }

            if (GameManager.GetManager().GetPlayerAmmoCount() == 0)
            {
                canShoot = false;
                Reload();
            }



            //Bullet Line
            GameObject BFXObj = Instantiate(BulletFX, Gun);
            LineRenderer BFXLine = BFXObj.GetComponent<LineRenderer>();
            BFXLine.SetPosition(0, Gun.position);
            BFXLine.SetPosition(1, hit.point);

            BFXLineFade(BFXObj);

            if (!isPoweredUp)
            {
                penaltyLevel++;
                SceneCamera.Inst.Shake(.5f * penaltyLevel);
                penaltyTimer = PenaltyDuration;
            }





            
        }
        else
        {
            //This should never happen currently
            Debug.Log("miss");
        }

        
    }

    public async void BFXLineFade(GameObject BFXObj)
    {
        float timer = 0;
        while (timer < BulletLineDuration)
        {
            timer += Time.deltaTime;
            await Task.Yield();
        }

        Destroy(BFXObj);

    }

    public async void Reload()
    {
        float timer = 0;
        bool perfectRound = true;

        //Reload faster if all bullets are marked as hits
        Ammo[] playerAmmo = GameManager.GetManager().GetPlayerAmmo();
        foreach(Ammo shot in playerAmmo)
        {
            if(shot == Ammo.Miss)
            {
                perfectRound = false;
                break;
            }
        }

        float reloadTimePerBullet = perfectRound ? .5f/6 : ReloadTime / 6;
        
        for (int i = 0; i < 6; i++)
        {
            float localReloadTime = reloadTimePerBullet;
            if (!perfectRound && playerAmmo[i] == Ammo.Hit)
                localReloadTime /= 2;
                   

            while (timer < localReloadTime)
            {
                timer += Time.deltaTime;
                await Task.Yield();
            }

            if (isPoweredUp) return;

            timer = 0;
            GameManager.GetManager()?.ReloadBullet();
            GameManager.GetManager()?.PlaySound("PLAYER_RELOAD");
        }

        canShoot = true;
    }

    public void DecreasePenalty()
    {
        if (penaltyLevel > 0)
            penaltyLevel--;

    }

    IEnumerator PowerUp()
    {
        isPoweredUp = true;
        canShoot = true;
        yield return new WaitForSeconds(PowerupDuration);
        isPoweredUp = false;
        GameManager.GetManager().AmmoPowerDown();
    }

    public void TriggerPowerUp()
    {
        StartCoroutine(PowerUp());
        GameManager.GetManager().AmmoPowerUp();
    }
}