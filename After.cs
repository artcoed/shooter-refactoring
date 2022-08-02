using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public bool IsAttacked { get; private set; }
}

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Killer _killer;
    [SerializeField] private Buyer _buyer;

    [SerializeField] private int _killReward = 10;

    private void Update()
    {
        if (_playerInput.IsAttacked)
        {
            _killer.Attack();
            if (_killer.IsKilled)
                _buyer.Earn(_killReward);
        }
    }
}

public class Killer : MonoBehaviour
{
    [SerializeField] private Character _character;
    [SerializeField] private EnemyDetector _enemyDetector;
    
    public bool IsKilled { get; private set; }

    public void Attack()
    {
        var enemy = _enemyDetector.Detect();
        _character.Attack(enemy);
        IsKilled = enemy.IsDead;
    }
}

public class Buyer : MonoBehaviour
{ 
    [SerializeField] private WalletUI _walletUI;

    private Wallet _wallet;

    private void Awake()
    {
        _wallet = new Wallet();
    }

    public void Earn(int amount)
    {
        _wallet.Earn(amount);
        _walletUI.SetMoney(_wallet.Money);
    }
}

public class Character : MonoBehaviour
{
    [SerializeField] private int _startHealth = 100;

    private Health _health;
    private Weapon _weapon;

    private void Awake()
    {
        _health = new Health(_startHealth);
    }

    public void Attack(Enemy enemy)
    {
        _weapon.Attack(enemy);
    }

    public void TakeDamage(int amount)
    {
        _health.Spend(amount);

        if (_health.IsEmpty)
            Destroy(gameObject);
    }
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private int _startHealth = 100;

    private Health _health;

    public bool IsDead => _health.IsEmpty;

    private void Awake()
    {
        _health = new Health(_startHealth);
    }

    public void TakeDamage(int amount)
    {
        _health.Spend(amount);
    }
}

public class EnemyDetector
{
    [SerializeField] private Enemy _enemy;

    public Enemy Detect()
    {
        return _enemy;
    }
}

public class Inventory
{
    private readonly List<Weapon> _weapons;

    public Inventory(List<Weapon> weapons)
    {
        _weapons = weapons;
    }

    public Weapon Get(int index)
    {
        if (index >= _weapons.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        return _weapons[index];
    }

    public void Put(Weapon weapon)
    {
        _weapons.Add(weapon);
    }
}

public class Health
{
    private int _value;

    public bool IsEmpty => _value == 0;

    public Health(int value)
    {
        _value = value;
    }

    public void Spend(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        if (IsEmpty)
            throw new InvalidOperationException(nameof(Spend));

        _value = Mathf.Max(_value - amount, 0);
    }
}

public class Wallet
{
    public int Money { get; private set; }

    public bool CanSpend(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        return Money >= amount;
    }

    public void Spend(int amount)
    {
        if (CanSpend(amount) == false)
            throw new ArgumentOutOfRangeException(nameof(amount));

        Money -= amount;
    }

    public void Earn(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        Money += amount;
    }
}

public class WalletUI : MonoBehaviour
{ 
    public void SetMoney(int amount)
    {

    }
}

public class Shop
{
    private readonly WeaponFactory _weaponFactory;
    private readonly PriceList _priceList;

    public Shop(WeaponFactory weaponFactory, PriceList priceList)
    {
        _weaponFactory = weaponFactory;
        _priceList = priceList;
    }

    public bool CanBuy(WeaponType type, Wallet wallet)
    {
        return wallet.CanSpend(_priceList.Get(type));
    }

    public Weapon Buy(WeaponType type, Wallet wallet)
    {
        if (CanBuy(type, wallet) == false)
            throw new ArgumentOutOfRangeException(nameof(wallet));

        wallet.Spend(_priceList.Get(type));
        return _weaponFactory.Create(type);
    }
}

public class PriceList
{
    private readonly Dictionary<WeaponType, int> _weaponPrices;

    public PriceList(Dictionary<WeaponType, int> weaponPrices)
    {
        _weaponPrices = weaponPrices;
    }

    public int Get(WeaponType type)
    {
        if (_weaponPrices.TryGetValue(type, out int price))
            return price;

        throw new ArgumentOutOfRangeException(nameof(type));
    }
}

public class WeaponFactory : MonoBehaviour
{
    private Dictionary<WeaponType, Weapon> _weapons;

    public Weapon Create(WeaponType type)
    {
        if (_weapons.TryGetValue(type, out Weapon weapon))
            return Instantiate(weapon);

        throw new ArgumentOutOfRangeException(nameof(type));
    }
}

public class Weapon : MonoBehaviour
{
    [SerializeField] private int _damage;

    public void Attack(Enemy enemy)
    {
        enemy.TakeDamage(_damage);
    }
}

public enum WeaponType
{

}
