using System;
using Son.Economy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStatManager : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerRunStats _playerRunStats;
    [SerializeField] private PlayerExperienceManager _playerExperienceManager;
    [SerializeField] private TowerRunStats _towerRunStats;

    [Header("UI Levels")]
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _xp;
    [SerializeField] private Slider _xpSlider;
    [Header("UI Stats texts")]
    [SerializeField] private TextMeshProUGUI _atkPlayer;
    [SerializeField] private TextMeshProUGUI _aSPlayer;
    [SerializeField] private TextMeshProUGUI _hPPlayer;
    [SerializeField] private TextMeshProUGUI _skillPlayer;
    [SerializeField] private TextMeshProUGUI _atkTower;
    [SerializeField] private TextMeshProUGUI _aSTower;
    [SerializeField] private TextMeshProUGUI _hPTower;

    [Header("UI Stats Sliders")]
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
        if (_playerExperienceManager != null)
        {
            _level.text = _playerExperienceManager.currentLevel.ToString();
            _xp.text = _playerExperienceManager.currentExp + " / " + _playerExperienceManager.ExpToNextLevel;
            // _xpSlider.maxValue = _playerExperienceManager.ExpToNextLevel;
            //_xpSlider.minValue = _playerExperienceManager.currentExp / _playerExperienceManager.ExpToNextLevel;
        }
        // Player Stats
        if (_atkPlayerSlider != null && _atkPlayer != null)
        {
            _atkPlayer.text = _playerRunStats.TotalAttackDamage.ToString();
            // _atkPlayerSlider.maxValue = _playerRunStats.baseAttackDamage + 25;
            //_atkPlayerSlider.minValue = _playerRunStats.baseAttackDamage / (_playerRunStats.arrowData.damage + 25);
        }
        if (_aSPlayer != null && _aSPlayerSlider != null)
        {
            _aSPlayer.text = Math.Round(_playerRunStats.attackSpeed, 2).ToString();
            // _aSPlayerSlider.maxValue = _playerRunStats.attackSpeed + 2.5f;
            //_aSPlayerSlider.minValue = _playerRunStats.attackSpeed / (_playerRunStats.playerData.shootInterval + 2.5f);
        }
        if (_hPPlayer != null && _hPPlayerSlider != null)
        {
            _hPPlayer.text = _playerRunStats.currentHP.ToString();
            // _hPPlayerSlider.maxValue = _playerRunStats.currentHP + 25 * 5;
            //_hPPlayerSlider.minValue = _playerRunStats.currentHP / (_playerRunStats.hpData._maxHealth + 25 * 5);
        }
        if (_skillPlayer != null && _skillPlayerSlider != null)
        {
            _skillPlayer.text = _playerRunStats.TotalSwordDamage.ToString();
            // _skillPlayerSlider.maxValue = _playerRunStats.baseSwordDamage + 25;
            //_skillPlayerSlider.minValue = _playerRunStats.baseSwordDamage / (_playerRunStats.swordData.damage + 25);
        }
        // Tower Stats
        if (_atkTower != null && _atkTowerSlider != null)
        {
            _atkTower.text = _towerRunStats.TotalAttackDamage.ToString();
            // _atkTowerSlider.maxValue = _towerRunStats.baseAttackDamage + 50 * 5;
            //_atkTowerSlider.minValue = _towerRunStats.baseAttackDamage / (_towerRunStats.baseData.damage + 50 * 5);
        }
        if (_aSTower != null && _aSTowerSlider != null)
        {
            _aSTower.text = _towerRunStats.attackRange.ToString();
            // _aSTowerSlider.maxValue = _towerRunStats.attackRange + 15;
            //_aSTowerSlider.minValue = _towerRunStats.attackRange / (_towerRunStats.baseData.attackRange + 15);
        }
        if (_hPTower != null && _hPTowerSlider != null)
        {
            _hPTower.text = _towerRunStats.currentHP.ToString();
            // _hPTowerSlider.maxValue = _towerRunStats.currentHP + 500 * 5;
            //_hPTowerSlider.minValue = _towerRunStats.currentHP / (_towerRunStats.baseData.maxHealth + 500 * 5);
        }
        _gameObject.SetActive(true);
    }
    public void HideUIStats()
    {
        _gameObject.SetActive(false);
    }
}