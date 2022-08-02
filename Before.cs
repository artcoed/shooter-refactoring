[RequireComponent(typeof(Animator), typeof(PlayerInput), typeof(Health))]
public class Player : MonoBehaviour
{
    [SerializeField] private List<Weapon> _weapons;
    [SerializeField] private Transform _shootingPoint;
    [SerializeField] private int _money;

    private Health _health;
    private Weapon _currentWeapon;
    private Animator _animator;
    private PlayerInput _playerInput;
    private int _currentWeaponIndex = 0;


    public int Money => _money;
    public Health Health => _health;
    public Weapon CurrentWeapon => _currentWeapon;

    public event UnityAction MoneyChanged;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _health = GetComponent<Health>();

        _currentWeapon = _weapons[0];
        MoneyChanged?.Invoke();
    }

    private void OnEnable()
    {
        _playerInput.MouseRightButtonClicked += ShootCurrentWeapon;
        _health.OutOfHealth += Die;
    }

    private void OnDisable()
    {
        _playerInput.MouseRightButtonClicked -= ShootCurrentWeapon;
        _health.OutOfHealth -= Die;
    }

    private void ShootCurrentWeapon()
    {
        _currentWeapon.Shoot(_shootingPoint, _playerInput.GetMousePosition());
    }


    public void NextWeapon()
    {
        if (_currentWeaponIndex == _weapons.Count - 1)
        {
            _currentWeaponIndex = 0;
        }
        else
        {
            _currentWeaponIndex++;
        }
        _currentWeapon = _weapons[_currentWeaponIndex];
    }

    public void PreviousWeapon()
    {
        if (_currentWeaponIndex == 0)
        {
            _currentWeaponIndex = _weapons.Count - 1;
        }
        else
        {
            _currentWeaponIndex--;
        }
        _currentWeapon = _weapons[_currentWeaponIndex];
    }



    public void TryBuy(Weapon weapon)
    {
        if (!CanBuy(weapon))
        {
            throw new ArgumentOutOfRangeException("Not enough money");
        }
        _weapons.Add(weapon);
        weapon.Buy();
        AddMoney(-weapon.Price);
    }

    public bool CanBuy(Weapon weapon)
    {
        if (Money >= weapon.Price)
        {
            return true;
        }

        return false;
    }

    public void AddMoney(int money)
    {
        if (Money + money >= 0)
        {
            _money += money;
            MoneyChanged?.Invoke();
        }
    }


    private void Die()
    {
        Destroy(gameObject);
    }

}
