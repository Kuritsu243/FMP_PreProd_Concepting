using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Environment;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Tutorial
{
    
    /** sources used: 
     * https://stackoverflow.com/questions/70073128/how-to-check-if-all-values-of-a-c-sharp-dictionary-are-true
     * https://imran-momin.medium.com/dictionaries-unity-c-69b48448445f
     * https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2?view=netframework-4.8
     */
    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private FloatingWallController floatingWallController;
        [SerializeField] private GameObject tutorialPistol;
        [Header("Input Prompts")] 
        [SerializeField] private Image keyPressW;
        [SerializeField] private Image keyPressWalt;
        [SerializeField] private Image keyPressS;
        [SerializeField] private Image keyPressSalt;
        [SerializeField] private Image keypressA;
        [SerializeField] private Image keypressAalt;
        [SerializeField] private Image keyPressD;
        [SerializeField] private Image keyPressDalt;
        
        
        

        
        
        
        private HighlightWeapon _pistolOutline;
        private bool _areWallsAppearing = false;
        private bool _isWeaponGlowing = false;

        public bool hasPlayerReachedLargeIsland;
        public Dictionary<string, bool> TutorialChecks;
        
        
        private void Start()
        {
            TutorialChecks = new Dictionary<string, bool>
            {
                { "Forward", false },
                { "Backwards", false },
                { "Left", false },
                { "Right", false },
                { "Jump", false }
            };

            _pistolOutline = tutorialPistol.GetComponent<HighlightWeapon>();
        }

        private void FixedUpdate()
        {
            if (keyPressW.color.a > 0)
            {
                LeanTween.alpha(keyPressW.gameObject, 0, 1.5f);
            }
            else
            {
                LeanTween.alpha(keyPressWalt.gameObject, 255, 1.5f);
            }
            // print dict values
            foreach (KeyValuePair<string, bool> kvp in TutorialChecks)
                Debug.Log(kvp.Key + kvp.Value);

            
            // if all checks in dict are true
            var allIsTrue = TutorialChecks.Values.All(value => value);
            Debug.LogWarning("completed?: " + allIsTrue);

            if (allIsTrue && !_areWallsAppearing)
                StartCoroutine(MakeWallsAppear());

            if (hasPlayerReachedLargeIsland && !_isWeaponGlowing)
                StartCoroutine(StartWeaponTutorial());

        }

        private IEnumerator MakeWallsAppear()
        {
            _areWallsAppearing = true;
            yield return new WaitForSeconds(2f);
            floatingWallController.TriggerWallMovement();
        }

        private IEnumerator StartWeaponTutorial()
        {
            _isWeaponGlowing = true;
            yield return new WaitForSeconds(1.2f);
            _pistolOutline.OutlineWeapon();
        }
    }
}
