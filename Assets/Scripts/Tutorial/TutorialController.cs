using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Environment;
using Player;
using TMPro;
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
    public static class ImageTweening
    {
        public static void ClearAlpha(ref Image img, bool loop)
        {
            var tempImg = img;
            if (loop)
            {
                LeanTween.value(img.gameObject, img.color.a, 0f, 3f).setOnUpdate(val =>
                {
                    Color c = tempImg.color;
                    c.a = val;
                    tempImg.color = c;
                }).setLoopPingPong();
            }
            else
            {
                LeanTween.value(img.gameObject, img.color.a, 0f, 3f).setOnUpdate(val =>
                {
                    Color c = tempImg.color;
                    c.a = val;
                    tempImg.color = c;
                });
            }
            img = tempImg;
        }

        public static void ClearTextAlpha(ref TextMeshProUGUI text, bool loop)
        {
            var tempTxt = text;
            
            if (loop)
            {
                LeanTween.value(text.gameObject, 1, 0f, 1.5f).setOnUpdate(val =>
                {
                    Color c = tempTxt.color;
                    c.a = val;
                    tempTxt.color = c;
                }).setLoopPingPong();
            }
            else
            {
                LeanTween.value(text.gameObject, 1, 0f, 1.5f).setOnUpdate(val =>
                {
                    Color c = tempTxt.color;
                    c.a = val;
                    tempTxt.color = c;
                });
            }

            text = tempTxt;
        }
        
        public static void FillTextAlpha(ref TextMeshProUGUI text, bool loop)
        {
            var tempTxt = text;
            
            if (loop)
            {
                LeanTween.value(text.gameObject, text.color.a, 1f, 1.5f).setOnUpdate(val =>
                {
                    Color c = tempTxt.color;
                    c.a = val;
                    tempTxt.color = c;
                }).setLoopPingPong();
            }
            else
            {
                LeanTween.value(text.gameObject, text.color.a, 1f, 1.5f).setOnUpdate(val =>
                {
                    Color c = tempTxt.color;
                    c.a = val;
                    tempTxt.color = c;
                });
            }

            text = tempTxt;
        }
        
        
        public static void FillAlpha(ref Image img, bool loop)
        {
            var tempImg = img;
            if (loop)
            {
                LeanTween.value(img.gameObject, img.color.a, 1f, 3f).setOnUpdate(val =>
                {
                    Color c = tempImg.color;
                    c.a = val;
                    tempImg.color = c;
                }).setLoopPingPong();
            }
            else
            {   
                LeanTween.value(img.gameObject, img.color.a, 1f, 3f).setOnUpdate(val =>
                {
                    Color c = tempImg.color;
                    c.a = val;
                    tempImg.color = c;
                }); 
            }
            img = tempImg;
        }

        public static void ChangePrompt(ref TextMeshProUGUI text, ref GameObject oldPrompt, ref GameObject newPrompt,
            ref Image newImg, ref Image newImgAlt, ref Image oldImg, ref Image oldImgAlt, ref Dictionary<int, string> textList, int textIndex)
        {
            var tempTxt = text;
            ClearAlpha(ref oldImg, false);
            ClearAlpha(ref oldImgAlt, false);
            ClearTextAlpha(ref text, false);

            LeanTween.value(text.gameObject, 1, 1f, 1.5f).setOnUpdate(f =>
            {
                Color c = tempTxt.color;
                c.a = f;
                tempTxt.color = c;
            });
            
            text.text = textList[textIndex];
            oldPrompt.SetActive(false);
            newPrompt.SetActive(true);
            
            ClearTextAlpha(ref text, true);
            ClearAlpha(ref newImg, true);
            FillAlpha(ref newImgAlt, true);
        }
    }
    
    
    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private FloatingWallController floatingWallController;
        [SerializeField] private GameObject tutorialPistol;
        [FormerlySerializedAs("Prompt_W")]
        [Header("Input Prompts")] 
        [SerializeField] private GameObject promptW;
        [SerializeField] private GameObject promptS;
        [SerializeField] private GameObject promptA;
        [SerializeField] private GameObject promptD;
        [SerializeField] private GameObject promptJump;
        [SerializeField] private GameObject promptComplete;
        
        [SerializeField] private Image keyPressW;
        [SerializeField] private Image keyPressWalt;
        [SerializeField] private Image keyPressS;
        [SerializeField] private Image keyPressSalt;
        [SerializeField] private Image keyPressA;
        [SerializeField] private Image keyPressAalt;
        [SerializeField] private Image keyPressD;
        [SerializeField] private Image keyPressDalt;
        [SerializeField] private Image keyPressSpace;
        [SerializeField] private Image keyPressSpacealt;
        [SerializeField] private Image keyPressComplete;
        [SerializeField] private Image keyPressCompletealt;
        
        
        
        
        [Header("Tutorial Text Prompts")] 
        [SerializeField] private TextMeshProUGUI tutorialTextHint;


        
        
        private HighlightWeapon _pistolOutline;
        private bool _areWallsAppearing = false;
        private bool _isWeaponGlowing = false;

        public bool hasPlayerReachedLargeIsland;
        public Dictionary<string, bool> TutorialChecks;
        public Dictionary<int, string> InputPromptTexts;
        
        private void Start()
        {
            promptW.SetActive(true);
            promptS.SetActive(false);
            promptA.SetActive(false);
            promptD.SetActive(false);
            promptJump.SetActive(false);
            promptComplete.SetActive(false);
            
            TutorialChecks = new Dictionary<string, bool>
            {
                { "Forward", false },
                { "Backwards", false },
                { "Left", false },
                { "Right", false },
                { "Jump", false }
            };

            InputPromptTexts = new Dictionary<int, string>
            {
                { 0, "Press W to move Forward"},
                { 1, "Press S to move Backwards"},
                { 2, "Press A to move Left"},
                { 3, "Press D to move Right"},
                { 4, "Press Space to Jump"},
                { 5, "Movement Tutorial Complete!"}
            };

            tutorialTextHint.text = InputPromptTexts[0];
            _pistolOutline = tutorialPistol.GetComponent<HighlightWeapon>();

            ImageTweening.ClearTextAlpha(ref tutorialTextHint, true);
            ImageTweening.ClearAlpha(ref keyPressW, true);
            ImageTweening.FillAlpha(ref keyPressWalt, true);
        }

        public void W_Pressed()
        {
            LeanTween.cancel(tutorialTextHint.gameObject);
            LeanTween.cancel(keyPressW.gameObject);
            LeanTween.cancel(keyPressWalt.gameObject);
            ImageTweening.ChangePrompt(ref tutorialTextHint, ref promptW, ref promptS, ref keyPressS,
                ref keyPressSalt, ref keyPressW, ref keyPressWalt, ref InputPromptTexts, 1);
        }

        public void S_Pressed()
        {
            LeanTween.cancel(tutorialTextHint.gameObject);
            LeanTween.cancel(keyPressS.gameObject);
            LeanTween.cancel(keyPressSalt.gameObject);
            ImageTweening.ChangePrompt(ref tutorialTextHint, ref promptS, ref promptA, ref keyPressA,
                ref keyPressAalt, ref keyPressS, ref keyPressSalt, ref InputPromptTexts, 2);
            
        }
        
        public void A_Pressed()
        {
            LeanTween.cancel(tutorialTextHint.gameObject);
            LeanTween.cancel(keyPressA.gameObject);
            LeanTween.cancel(keyPressAalt.gameObject);
            ImageTweening.ChangePrompt(ref tutorialTextHint, ref promptA, ref promptD, ref keyPressD,
                ref keyPressDalt, ref keyPressA, ref keyPressAalt, ref InputPromptTexts, 3);
        }

        public void D_Pressed()
        {
            LeanTween.cancel(tutorialTextHint.gameObject);
            LeanTween.cancel(keyPressD.gameObject);
            LeanTween.cancel(keyPressDalt.gameObject);
            ImageTweening.ChangePrompt(ref tutorialTextHint, ref promptD, ref promptJump, ref keyPressSpace,
                ref keyPressSpacealt, ref keyPressD, ref keyPressDalt, ref InputPromptTexts, 4);
        }

        public void Jump_Pressed()
        {
            LeanTween.cancel(tutorialTextHint.gameObject);
            LeanTween.cancel(keyPressD.gameObject);
            LeanTween.cancel(keyPressDalt.gameObject);
            
            ImageTweening.ClearAlpha(ref keyPressSpace, false);
            ImageTweening.ClearAlpha(ref keyPressSpacealt, false);
            ImageTweening.ClearTextAlpha(ref tutorialTextHint, false);

            // LeanTween.value(text.gameObject, 1, 1f, 1.5f).setOnUpdate(f =>
            // {
            //     Color c = tempTxt.color;
            //     c.a = f;
            //     tempTxt.color = c;
            // });
            
            tutorialTextHint.text = InputPromptTexts[5];
            promptJump.SetActive(false);
            promptComplete.SetActive(true);
            
            ImageTweening.ClearTextAlpha(ref tutorialTextHint, true);
            ImageTweening.ClearAlpha(ref keyPressComplete, true);
            // ImageTweening.FillAlpha(ref keyPressCompletealt, true);
            
            
            StartCoroutine(HideUIPrompt());
        }


        private IEnumerator HideUIPrompt()
        {
            yield return new WaitForSeconds(2f);
            LeanTween.value(tutorialTextHint.gameObject, 1f, 0f, 1.5f).setOnUpdate(f =>
            {
                Color c = tutorialTextHint.color;
                c.a = f;
                tutorialTextHint.color = c;
            }).setOnComplete(() =>
            {
                LeanTween.cancel(tutorialTextHint.gameObject);
                LeanTween.cancel(keyPressComplete.gameObject);
                LeanTween.cancel(keyPressCompletealt.gameObject); 
                promptComplete.SetActive(false);
                tutorialTextHint.gameObject.SetActive(false);
            }
            );
            // LeanTween.cancel(tutorialTextHint.gameObject);
            // LeanTween.cancel(keyPressComplete.gameObject);
            // LeanTween.cancel(keyPressCompletealt.gameObject);
            
        }

        private void FixedUpdate()
        {
       
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
