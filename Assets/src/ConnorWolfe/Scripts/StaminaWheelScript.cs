using UnityEngine;
using UnityEngine.UI;
public class StaminaWheelScript : MonoBehaviour
{
    /* This script handles the stamina wheel UI elements
     *
     */
    public float maxStamina = 100f;
    [SerializeField] private float _regen = 10f;
    [SerializeField] private float _hideTime = 3f;
    [SerializeField] private Image _greenWheel;
    [SerializeField] private Image _redWheel;
    private float _currStamina;
    private float _showTimer = 0f;
    private float _regenTime = 1f;
    private float _regenTimer = 0f;
    private float _redBuffer = 0f;
    private bool _staminaExhausted = false;
    private bool _showWheel = true;
    private void Start()
    {
        _currStamina = maxStamina;
    }
    private void Update()
    {
        /*
                if (_currStamina == maxStamina && _showTimer < _hideTime)
                {
                    _showTimer += Time.deltaTime;
                }
        */
        if (!_staminaExhausted)
        {
            /* if (_currStamina > 0)
                        {
                            _currStamina -= 30 * Time.deltaTime;
                        }
                        else
                        {
                            _greenWheel.enabled = false;
                            _staminaExhausted = true;
                        }
            */

            if (_regenTimer >= _regenTime)
            {
                _currStamina += _regen * Time.deltaTime;
            }
            if (_currStamina <= 0)
            {
                _greenWheel.enabled = false;
                _staminaExhausted = true;
            }
            _redWheel.fillAmount = (_currStamina / maxStamina + _redBuffer);
            if (_redBuffer >= 0)
                _redBuffer -= _redBuffer * Time.deltaTime * 2.0f;
        }
        else
        {
            if (_currStamina < maxStamina)
            {
                _currStamina += (_regen * 0.5f) * Time.deltaTime;
            }
            else
            {
                _greenWheel.enabled = true;
                _staminaExhausted = false;
            }
            _redWheel.fillAmount = _currStamina / maxStamina;
        }
        if (_regenTimer <= _regenTime)
            _regenTimer += Time.deltaTime;
        _greenWheel.fillAmount = _currStamina / maxStamina;
    }
    // public functions //
    public void ChangeStamina(float amount)
    {
        _regenTimer = 0f;
        _redBuffer = 0.07f;
        if (_currStamina + amount >= maxStamina)
        {
            _currStamina = maxStamina;
            return;
        }
        else if (_currStamina + amount <= 0)
        {
            _currStamina = 0;
            return;
        }
        _currStamina += amount;
    }
    public bool IsExhausted()
    {
        return _staminaExhausted;
    }
}