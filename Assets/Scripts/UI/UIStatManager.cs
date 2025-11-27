using System;
using Son.Economy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStatManager : MonoBehaviour
{
    [Header ("Scripts")] 
    [SerializeField] private PlayerRunStats _playerRunStats;
    [SerializeField] private PlayerExperienceManager _playerExperienceManager;
    [SerializeField] private TowerRunStats _towerRunStats;

    [Header ("UI Levels")]
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _xp;
    [SerializeField] private Slider _xpSlider;
    [Header ("UI Stats texts")]
    [SerializeField] private TextMeshProUGUI _atkPlayer;
    [SerializeField] private TextMeshProUGUI _aSPlayer;
    [SerializeField] private TextMeshProUGUI _hPPlayer;
    [SerializeField] private TextMeshProUGUI _skillPlayer;
    [SerializeField] private TextMeshProUGUI _atkTower;
    [SerializeField] private TextMeshProUGUI _aSTower;
    [SerializeField] private TextMeshProUGUI _hPTower;
    
    [Header ("UI Stats Sliders")]
    [SerializeField] private Slider _atkPlayerSlider;
    [SerializeField] private Slider _aSPlayerSlider;
    [SerializeField] private Slider _hPPlayerSlider;
    [SerializeField] private Slider _skillPlayerSlider;
    [SerializeField] private Slider _atkTowerSlider;
    [SerializeField] private Slider _aSTowerSlider;
    [SerializeField] private Slider _hPTowerSlider;

    public GameObject _gameObject;

    private void Awake()
    {
        _playerExperienceManager = FindFirstObjectByType<PlayerExperienceManager>();
    }

    public void UpdateUIStats()
    {
        if(_playerExperienceManager != null)
        {
            _level.text = _playerExperienceManager.currentLevel.ToString();
            _xp.text = _playerExperienceManager.currentExp + " / " + _playerExperienceManager.baseExpToLevelUp;
            _xpSlider.maxValue = _playerExperienceManager.baseExpToLevelUp;
            _xpSlider.value = _playerExperienceManager.currentExp;
        }
        // Player Stats
        if(_atkPlayerSlider != null && _atkPlayer != null)
        {
            _atkPlayer.text = _playerRunStats.TotalAttackDamage.ToString();
            _atkPlayerSlider.maxValue = _playerRunStats.baseAttackDamage + _playerRunStats.bonusAttackDamage * 5;
            _atkPlayerSlider.value = _playerRunStats.baseAttackDamage;
        }
        if(_aSPlayer != null && _aSPlayerSlider != null)
        {
            _aSPlayer.text = Math.Round(_playerRunStats.attackSpeed, 2).ToString();
            _aSPlayerSlider.maxValue = _playerRunStats.attackSpeed + 2.5f;
            _aSPlayerSlider.value = _playerRunStats.attackSpeed;
        }
        if(_hPPlayer != null && _hPPlayerSlider != null)
        {
            _hPPlayer.text = _playerRunStats.currentHP.ToString();
            _hPPlayerSlider.maxValue = _playerRunStats.currentHP + 25 * 5;
            _hPPlayerSlider.value = _playerRunStats.currentHP;
        }
        if(_skillPlayer != null && _skillPlayerSlider != null)
        {
            _skillPlayer.text = _playerRunStats.TotalSwordDamage.ToString();
            _skillPlayerSlider.maxValue = _playerRunStats.baseSwordDamage + _playerRunStats.bonusSwordDamage * 5;
            _skillPlayerSlider.value = _playerRunStats.baseSwordDamage;
        }
        // Tower Stats
        if(_atkTower != null && _atkTowerSlider != null)
        {
            _atkTower.text = _towerRunStats.TotalAttackDamage.ToString();
            _atkTowerSlider.maxValue = _towerRunStats.baseAttackDamage + _towerRunStats.bonusAttackDamage * 5;
            _atkTowerSlider.value = _towerRunStats.baseAttackDamage;
        }
        if(_aSTower != null && _aSTowerSlider != null)
        {
            _aSTower.text = _towerRunStats.attackRange.ToString();
            _aSTowerSlider.maxValue = _towerRunStats.attackRange + 15;
            _aSTowerSlider.value = _towerRunStats.attackRange;
        }
        if(_hPTower != null && _hPTowerSlider != null)
        {
            _hPTower.text = _towerRunStats.currentHP.ToString();
            _hPTowerSlider.maxValue = _towerRunStats.currentHP + 500 * 5;
            _hPTowerSlider.value = _towerRunStats.currentHP;
        }
        _gameObject.SetActive(true);
    }
    public void HideUIStats()
    {
        _gameObject.SetActive(false);
    }
}