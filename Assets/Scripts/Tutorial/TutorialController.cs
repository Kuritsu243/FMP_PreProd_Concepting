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

        public static void AlphaPrompt(ref TextMeshProUGUI text, ref Image imgMain, ref Image imgAlt, bool loop)
        {
            ClearTextAlpha(ref text, loop);
            ClearAlpha(ref imgMain, loop);
            FillAlpha(ref imgAlt, loop);
        }


        public static void ChangePrompt(ref TextMeshProUGUI text, ref GameObject oldPrompt, ref GameObject newPrompt,
            ref Image newImg, ref Image newImgAlt, ref Image oldImg, ref Image oldImgAlt,
            ref Dictionary<int, string> textList, int textIndex)
        {
            var tempTxt = text;
            ClearAlpha(ref oldImg, false);
            ClearAlpha(ref oldImgAlt, false);
            ClearTextAlpha(ref text, false);
            LeanTween.cancel(text.gameObject);
            LeanTween.cancel(oldImg.gameObject);
            LeanTween.cancel(oldImgAlt.gameObject);
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

        public static void ChangeTextPromptOnly(ref TextMeshProUGUI text, ref Dictionary<int, string> textList,
            int textIndex)
        {
            var tempTxt = text;
            ClearTextAlpha(ref text, false);
            LeanTween.value(text.gameObject, 1f, 1f, 1.5f).setOnUpdate(f =>
            {
                Color c = tempTxt.color;
                c.a = f;
                tempTxt.color = c;
            });
            text.text = textList[textIndex];
            ClearTextAlpha(ref text, true);
        }
    }


    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private FloatingWallController floatingWallController;
        [SerializeField] private GameObject tutorialPistol;
        [SerializeField] private TutorialEnemyController tutorialEnemyController;
        [SerializeField] private GameObject endComputer;

        [FormerlySerializedAs("Prompt_W")] [Header("Input Prompts")] 
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

        [Header("Islands")] 
        [SerializeField] private GameObject enemyIsland;

        [Header("Portal")] 
        [SerializeField] private GameObject portal;
        [SerializeField] private SpriteRenderer portalSpriteRenderer;


        private HighlightWeapon _pistolOutline;
        private HighlightComputer _computerOutline;
        private bool _areWallsAppearing;
        private bool _isWeaponGlowing;
        private bool _hasEnemyIslandAppeared;
        public bool hasFiredPistolYet;


        public enum NextKeyPress
        {
            Forward,
            Backwards,
            Left,
            Right,
            Jump,
            Complete
        }

        public NextKeyPress nextKeyToPress;
        public Dictionary<string, bool> TutorialChecks;
        public Dictionary<string, bool> EnemyChecks;
        public Dictionary<string, bool> WallRunChecks;
        private Dictionary<int, string> _introductionTexts;
        private Dictionary<int, string> _inputPromptTexts;
        private Dictionary<int, string> _wallRunPromptTexts;
        private Dictionary<int, string> _weaponPromptTexts;
        private Dictionary<int, string> _enemyIslandTexts;
        private Dictionary<int, string> _challengeCompleteTexts;

        private void Start()
        {
            promptW.SetActive(false);
            promptS.SetActive(false);
            promptA.SetActive(false);
            promptD.SetActive(false);
            promptJump.SetActive(false);
            promptComplete.SetActive(false);
            enemyIsland.SetActive(false);
            portal.SetActive(false);


            TutorialChecks = new Dictionary<string, bool>
            {
                { "IntroductionComplete", false },
                { "Forward", false },
                { "Backwards", false },
                { "Left", false },
                { "Right", false },
                { "Jump", false }
            };

            WallRunChecks = new Dictionary<string, bool>
            {
                { "FirstWall", false },
                { "SecondWall", false },
                { "IslandReached", false }
            };

            EnemyChecks = new Dictionary<string, bool>
            {
                { "Equipped", false },
                { "Fired", false },
                { "Missed", false },
                { "Killed", false }
            };

            _introductionTexts = new Dictionary<int, string>
            {
                { 0, "Welcome to this tutorial!" },
                { 1, "I'll be your teacher today." },
                { 2, "First, lets familiarize ourselves with this Games Controls." }
            };


            _inputPromptTexts = new Dictionary<int, string>
            {
                { 0, "Press W to move Forward" },
                { 1, "Press S to move Backwards" },
                { 2, "Press A to move Left" },
                { 3, "Press D to move Right" },
                { 4, "Press Space to Jump" },
                { 5, "Movement Tutorial Complete!" },
            };

            _wallRunPromptTexts = new Dictionary<int, string>
            {
                { 0, "Huh, moving, floating walls. Didn't expect that." },
                { 1, "Try wall running to the next island." },
                { 2, "Jump between the walls by pressing Space." },
                { 3, "You did it! Nice work." },
                { 4, "Time to explore this island." }
            };

            _weaponPromptTexts = new Dictionary<int, string>
            {
                { 0, "Oh, a free gun!" },
                { 1, "Press F to pickup the gun." },
                { 2, "Look nearby the campfire, there's a person." },
                { 3, "Shoot the person by pressing Mouse1." },
                { 4, "SHOOT. THEM." },
                { 5, "It'd help if you actually aimed at the person." },
                { 6, "Good Job." },
                { 7, "So uhhh, what now..." },
                { 8, "Come here often?" }
            };

            _enemyIslandTexts = new Dictionary<int, string>
            {
                { 0, "Ah. Shit." },
                { 1, "These guys don't seem too happy." },
                { 2, "Time to kill them I guess." }
            };

            _challengeCompleteTexts = new Dictionary<int, string>
            {
                { 0, "That's those guys taken care of. " },
                { 1, "Huh, what's that device over there?" },
                { 2, "I should press this button." },
                { 3, "Hmm. It's doing nothing." },
                { 4, "Nevermind, spoke too soon." },
                { 5, "A giant portal! Lets go through it. Nothing bad ever happens with portals." }
            };


            tutorialTextHint.text = _introductionTexts[0];
            _pistolOutline = tutorialPistol.GetComponent<HighlightWeapon>();
            _computerOutline = endComputer.GetComponent<HighlightComputer>();
            tutorialEnemyController = GetComponent<TutorialEnemyController>();
            ImageTweening.ClearTextAlpha(ref tutorialTextHint, true);
            ImageTweening.ClearAlpha(ref keyPressW, true);
            ImageTweening.FillAlpha(ref keyPressWalt, true);
            nextKeyToPress = NextKeyPress.Forward;
            StartCoroutine(IntroductionText());
        }


        public bool IntroComplete()
        {
            return TutorialChecks["IntroductionComplete"];
        }

        public void OtherIslandReached()
        {
            WallRunChecks["IslandReached"] = true;
            if (!_isWeaponGlowing)
                StartCoroutine(StartWeaponTutorial());
        }

        public void PistolCollected()
        {
            EnemyChecks["Equipped"] = true;
            StartCoroutine(PistolRelatedDialogue());
        }

        public void ActuallyAim()
        {
            if (tutorialTextHint.text != _weaponPromptTexts[4]) return;
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _weaponPromptTexts, 5);
        }

        public void TutorialEnemyKilled()
        {
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _weaponPromptTexts, 6);
            StartCoroutine(SpawnEnemyIsland());
        }

        public void EnemyChallengeComplete()
        {
            StartCoroutine(EnemiesAreKilled());
        }

        private IEnumerator EnemiesAreKilled()
        {
            tutorialTextHint.gameObject.SetActive(true);
            ImageTweening.ClearTextAlpha(ref tutorialTextHint, true);
            yield return new WaitForSeconds(0.8f);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _challengeCompleteTexts, 0);
            yield return new WaitForSeconds(2.5f);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _challengeCompleteTexts, 1);
            _computerOutline.OutlineComputer();
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _challengeCompleteTexts, 2);
            
        }

        private IEnumerator SpawnPortal()
        {
            yield return new WaitForSeconds(1.2f);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _challengeCompleteTexts, 4);
            portal.SetActive(true);
            LeanTween.value(portal, 0f, 1f, 3.5f).setOnUpdate(f =>
            {
                Color c = portalSpriteRenderer.color;
                c.a = f;
                portalSpriteRenderer.color = c;
            }).setOnComplete(() =>
            {
                endComputer.LeanMoveLocalY(3.25f, 0.4f).setOnComplete(() =>
                {
                    endComputer.LeanRotateX(-25f, 0.5f).setOnComplete(() =>
                    {
                        endComputer.LeanMoveLocalY(3.825f, 0.4f).setOnComplete(() =>
                        {
                            endComputer.LeanRotateX(-45f, 0.25f).setOnComplete(() =>
                            {
                                endComputer.LeanMoveLocalX(-8.5f, 0.25f).setOnComplete(() =>
                                {
                                    endComputer.SetActive(false);
                                    ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint,
                                        ref _challengeCompleteTexts, 5);
                                });
                            });
                        });
                    });
                });
            });
        }

        private IEnumerator SpawnEnemyIsland()
        {
            yield return new WaitForSeconds(3f);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _weaponPromptTexts, 7);
            yield return new WaitForSeconds(2f);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _weaponPromptTexts, 8);
            enemyIsland.SetActive(true);
            yield return new WaitForSeconds(1f);
            LeanTween.moveY(enemyIsland, -2.5f, 5f).setOnComplete(() =>
            {
                LeanTween.moveX(enemyIsland, -2f, 3f);
                _hasEnemyIslandAppeared = true;
            });
            yield return new WaitUntil(() => _hasEnemyIslandAppeared);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _enemyIslandTexts, 0);
            yield return new WaitForSeconds(2f);
            LeanTween.moveX(enemyIsland, -2f, 3f);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _enemyIslandTexts, 1);
            yield return new WaitForSeconds(3f);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _enemyIslandTexts, 2);
            yield return new WaitForSeconds(1.2f);
            tutorialEnemyController.StartKillChallenge();
            LeanTween.cancel(tutorialTextHint.gameObject);
            LeanTween.value(tutorialTextHint.gameObject, 1f, 0f, 1.5f).setOnUpdate(f =>
            {
                Color c = tutorialTextHint.color;
                c.a = f;
                tutorialTextHint.color = c;
            }).setOnComplete(() => tutorialTextHint.gameObject.SetActive(false));
        }


        public void ComputerInteracted()
        {
            _computerOutline.StopOutline();
            StartCoroutine(FinalPrompts());
        }

        private IEnumerator FinalPrompts()
        {
            yield return new WaitForSeconds(1f);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _challengeCompleteTexts, 3);
            yield return new WaitForSeconds(2f);
            StartCoroutine(SpawnPortal());
        }

        private IEnumerator CompleteFirstPrompt()
        {
            yield return new WaitForSeconds(2f);
            LeanTween.value(tutorialTextHint.gameObject, 1f, 0f, 1.5f).setOnUpdate(f =>
            {
                Color c = tutorialTextHint.color;
                c.a = f;
                tutorialTextHint.color = c;
            }).setOnComplete(() =>
                {
                    LeanTween.cancel(keyPressComplete.gameObject);
                    LeanTween.cancel(keyPressCompletealt.gameObject);
                    promptComplete.SetActive(false);
                    StartCoroutine(StartWallRunPrompt());
                }
            );
        }

        private IEnumerator StartWallRunPrompt()
        {
            yield return new WaitForSeconds(2f);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _wallRunPromptTexts, 0);
            yield return new WaitForSeconds(2.8f);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _wallRunPromptTexts, 1);
            yield return new WaitUntil(() => WallRunChecks["FirstWall"]);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _wallRunPromptTexts, 2);
            yield return new WaitUntil(() => WallRunChecks["SecondWall"]);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _wallRunPromptTexts, 3);
            yield return new WaitUntil(() => WallRunChecks["IslandReached"]);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _wallRunPromptTexts, 4);
        }

        private void FixedUpdate()
        {
            var allIsTrue = TutorialChecks.Values.All(value => value);
            if (allIsTrue && !_areWallsAppearing)
                StartCoroutine(MakeWallsAppear());
        }

        private IEnumerator IntroductionText()
        {
            yield return new WaitForSeconds(2.5f);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _introductionTexts, 1);
            yield return new WaitForSeconds(2f);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _introductionTexts, 2);
            yield return new WaitForSeconds(1.6f);
            StartCoroutine(InputPrompts());
        }

        private IEnumerator InputPrompts()
        {
            ImageTweening.ClearTextAlpha(ref tutorialTextHint, false);
            yield return new WaitForSeconds(2f);
            tutorialTextHint.text = _inputPromptTexts[0];
            promptW.SetActive(true);
            ImageTweening.AlphaPrompt(ref tutorialTextHint, ref keyPressW, ref keyPressWalt, true);
            TutorialChecks["IntroductionComplete"] = true;
            yield return new WaitUntil(() => TutorialChecks["Forward"]);
            ImageTweening.ChangePrompt(ref tutorialTextHint, ref promptW, ref promptS, ref keyPressS,
                ref keyPressSalt, ref keyPressW, ref keyPressWalt, ref _inputPromptTexts, 1);
            nextKeyToPress = NextKeyPress.Backwards;
            yield return new WaitUntil(() => TutorialChecks["Backwards"]);
            ImageTweening.ChangePrompt(ref tutorialTextHint, ref promptS, ref promptA, ref keyPressA,
                ref keyPressAalt, ref keyPressS, ref keyPressSalt, ref _inputPromptTexts, 2);
            nextKeyToPress = NextKeyPress.Left;
            yield return new WaitUntil(() => TutorialChecks["Left"]);
            ImageTweening.ChangePrompt(ref tutorialTextHint, ref promptA, ref promptD, ref keyPressD,
                ref keyPressDalt, ref keyPressA, ref keyPressAalt, ref _inputPromptTexts, 3);
            nextKeyToPress = NextKeyPress.Right;
            yield return new WaitUntil(() => TutorialChecks["Right"]);
            ImageTweening.ChangePrompt(ref tutorialTextHint, ref promptD, ref promptJump, ref keyPressSpace,
                ref keyPressSpacealt, ref keyPressD, ref keyPressDalt, ref _inputPromptTexts, 4);
            nextKeyToPress = NextKeyPress.Jump;
            yield return new WaitUntil(() => TutorialChecks["Jump"]);
            nextKeyToPress = NextKeyPress.Complete;
            ImageTweening.AlphaPrompt(ref tutorialTextHint, ref keyPressSpace, ref keyPressSpacealt, false);
            tutorialTextHint.text = _inputPromptTexts[5];
            promptJump.SetActive(false);
            promptComplete.SetActive(true);
            ImageTweening.ClearTextAlpha(ref tutorialTextHint, true);
            ImageTweening.ClearAlpha(ref keyPressComplete, true);
            StartCoroutine(CompleteFirstPrompt());
        }

        private IEnumerator MakeWallsAppear()
        {
            _areWallsAppearing = true;
            yield return new WaitForSeconds(2f);
            floatingWallController.TriggerWallMovement();
        }

        private IEnumerator StartWeaponTutorial()
        {
            yield return new WaitForSeconds(1.2f);
            _isWeaponGlowing = true;
            StartCoroutine(ShowWeaponText());
            yield return new WaitForSeconds(1.2f);
            if (_pistolOutline)
                _pistolOutline.OutlineWeapon();
        }

        private IEnumerator ShowWeaponText()
        {
            yield return new WaitForSeconds(1.6f);
            tutorialTextHint.gameObject.SetActive(true);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _weaponPromptTexts, 0);
            if (EnemyChecks["Fired"]) yield break;
            yield return new WaitForSeconds(2f);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _weaponPromptTexts, 1);
        }

        private IEnumerator PistolRelatedDialogue()
        {
            yield return new WaitForSeconds(1.2f);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _weaponPromptTexts, 2);
            yield return new WaitForSeconds(1.8f);
            ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _weaponPromptTexts, 3);
            var timerStart = Time.time;
            yield return new WaitUntil(() => Time.time - timerStart > 5f || EnemyChecks["Fired"]);
            switch (EnemyChecks["Fired"])
            {
                case false when !EnemyChecks["Killed"]:
                    ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _weaponPromptTexts, 4);
                    break;
                case true when !EnemyChecks["Killed"]:
                    ImageTweening.ChangeTextPromptOnly(ref tutorialTextHint, ref _weaponPromptTexts, 5);
                    break;
            }
        }
    }
}